using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CribSheet.Data;
using CribSheet.Models;
using System.Collections.ObjectModel;

namespace CribSheet.ViewModels
{
  public partial class SleepRecordsViewModel(CribSheetDatabase database) : BaseViewModel, IQueryAttributable
  {
    private readonly CribSheetDatabase _database = database;

    [ObservableProperty]
    private Baby? currentBaby;

    [ObservableProperty]
    private ObservableCollection<SleepRecord>? sleepRecords;

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
      if (query.ContainsKey("Baby"))
      {
        CurrentBaby = (Baby)query["Baby"];
        await LoadBabyDataAsync();
      }
    }

    [RelayCommand]
    private async Task NavigateToNewSleep()
    {
      if (CurrentBaby == null) return;

      await Shell.Current.GoToAsync(
        nameof(Views.NewSleepRecordPage),
        new Dictionary<string, object>
        {
          { "BabyId", CurrentBaby.BabyId }
        });
    }

    private async Task LoadBabyDataAsync()
    {
      if (CurrentBaby == null) return;
      try
      {
        SleepRecords = new ObservableCollection<SleepRecord>(
          await _database.GetSleepRecordsAsync(CurrentBaby.BabyId));
      }
      catch (Exception ex)
      {
        await Shell.Current.DisplayAlertAsync("Error",
          $"Failed to load sleep data: {ex.Message}", "OK");
      }
    }

    [RelayCommand]
    private async Task DeleteSleep(SleepRecord record)
    {
      if (record == null) return;

      bool checkDelete = await Shell.Current.DisplayAlertAsync("Confirm", "Are you sure you want to delete this sleep record?", "Yes", "Cancel");
      if (!checkDelete) return;

      int rowsRemoved = await _database.DeleteSleepRecordAsync(record);
      if (rowsRemoved == 0)
      {
        await Shell.Current.DisplayAlertAsync("Error", "Failed to delete sleep record.", "OK");
      }
      else
      {
        SleepRecords?.Remove(record);
      }
    }

    [RelayCommand]
    private async Task EditSleep(SleepRecord record)
    {
      // Placeholder for future edit functionality
      await Shell.Current.DisplayAlertAsync("Info", "Edit functionality coming soon!", "OK");
    }
  }
}
