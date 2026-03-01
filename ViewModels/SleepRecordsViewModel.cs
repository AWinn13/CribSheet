using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CribSheet.Data;
using CribSheet.Models;
using CribSheet.Services;
using System.Collections.ObjectModel;

namespace CribSheet.ViewModels
{
  public partial class SleepRecordsViewModel : BaseViewModel
  {
    private readonly CribSheetDatabase _database;

    // 24 hour labels used by the vertical timeline in the view
    public static IReadOnlyList<string> HourLabels { get; } = Enumerable.Range(0, 24)
      .Select(h => h == 0 ? "12am" : h < 12 ? $"{h}am" : h == 12 ? "12pm" : $"{h - 12}pm")
      .ToList();

    public SleepRecordsViewModel(CribSheetDatabase database, ICurrentBaby currentBabyService)
      : base(currentBabyService)
    {
      _database = database;
      CurrentBaby = CurrentBabyService.SelectedBaby;
      _ = LoadBabyDataAsync();
    }

    [ObservableProperty]
    private Baby? currentBaby;

    [ObservableProperty]
    private ObservableCollection<SleepRecord>? sleepRecords;

    [ObservableProperty]
    private ObservableCollection<SleepRecord> daySleepRecords = new();

    [ObservableProperty]
    private DateTime selectedDate = DateTime.Today;

    private async Task LoadBabyDataAsync()
    {
      if (CurrentBaby == null) return;
      try
      {
        SleepRecords = new ObservableCollection<SleepRecord>(
          await _database.GetSleepRecordsAsync(CurrentBaby.BabyId));

        // Start on the most recent day that has records, or today
        if (SleepRecords.Any())
          SelectedDate = SleepRecords.Max(r => r.StartTime.Date);

        RefreshDayView();
      }
      catch (Exception ex)
      {
        await Shell.Current.DisplayAlertAsync("Error",
          $"Failed to load sleep data: {ex.Message}", "OK");
      }
    }

    private void RefreshDayView()
    {
      DaySleepRecords = new ObservableCollection<SleepRecord>(
        SleepRecords?
          .Where(r => r.StartTime.Date == SelectedDate.Date)
          .OrderBy(r => r.StartTime)
        ?? Enumerable.Empty<SleepRecord>());
    }

    [RelayCommand]
    private void PreviousDay()
    {
      SelectedDate = SelectedDate.AddDays(-1);
      RefreshDayView();
    }

    [RelayCommand]
    private void NextDay()
    {
      SelectedDate = SelectedDate.AddDays(1);
      RefreshDayView();
    }

    [RelayCommand]
    private async Task TapSleep(SleepRecord record)
    {
      if (record == null) return;

      string title = record.EndTime.HasValue
        ? $"{record.TypeOfSleep}: {record.StartTime:h:mm tt} – {record.EndTime.Value:h:mm tt}"
        : $"{record.TypeOfSleep}: {record.StartTime:h:mm tt} (ongoing)";

      string? action = await Shell.Current.DisplayActionSheet(title, "Cancel", "Delete", "Edit");

      if (action == "Delete")
        await DeleteSleep(record);
      else if (action == "Edit")
        await EditSleep(record);
    }

    [RelayCommand]
    private async Task DeleteSleep(SleepRecord record)
    {
      if (record == null) return;

      bool confirm = await Shell.Current.DisplayAlertAsync("Confirm",
        "Are you sure you want to delete this sleep record?", "Yes", "Cancel");
      if (!confirm) return;

      int rowsRemoved = await _database.DeleteSleepRecordAsync(record);
      if (rowsRemoved == 0)
      {
        await Shell.Current.DisplayAlertAsync("Error", "Failed to delete sleep record.", "OK");
      }
      else
      {
        SleepRecords?.Remove(record);
        RefreshDayView();
      }
    }

    [RelayCommand]
    private async Task EditSleep(SleepRecord record)
    {
      await Shell.Current.DisplayAlertAsync("Info", "Edit functionality coming soon!", "OK");
    }
  }
}
