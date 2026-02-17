using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CribSheet.Data;
using CribSheet.Models;
using CribSheet.Services;
using System.Collections.ObjectModel;
using System.Text;

namespace CribSheet.ViewModels
{
  public partial class FeedingRecordsViewModel : BaseViewModel
  {
    private readonly CribSheetDatabase _database;

    public FeedingRecordsViewModel(CribSheetDatabase database, ICurrentBaby currentBabyService)
      : base(currentBabyService)
    {
      _database = database;
      CurrentBaby = CurrentBabyService.SelectedBaby;
      _ = LoadBabyDataAsync();
    }

    [ObservableProperty]
    private Baby? currentBaby;

    [ObservableProperty]
    private ObservableCollection<FeedingRecord>? feedingRecords;

    [RelayCommand]
    private async Task NavigateToNewFeeding()
    {
      await Shell.Current.GoToAsync(nameof(Views.NewFeedingRecordPage));
    }

    private async Task LoadBabyDataAsync()
    {
      if (CurrentBaby == null) return;
      try
      {
        FeedingRecords = new ObservableCollection<FeedingRecord>(
          await _database.GetFeedingRecordsAsync(CurrentBaby.BabyId));

      }
      catch (Exception ex)
      {
        await Shell.Current.DisplayAlertAsync("Error",
          $"Failed to load baby data: {ex.Message}", "OK");
      }
    }

    [RelayCommand]
    private async Task ExportToCsv()
    {
      if (FeedingRecords == null || FeedingRecords.Count == 0)
      {
        await Shell.Current.DisplayAlertAsync("Info", "No feeding records to export.", "OK");
        return;
      }

      try
      {
        var csv = new StringBuilder();
        csv.AppendLine("FeedingId,BabyId,Time,Type,AmountMl,DurationMinutes,Notes");

        foreach (var record in FeedingRecords)
        {
          var notes = record.Notes?.Replace("\"", "\"\"") ?? "";
          csv.AppendLine($"{record.FeedingId},{record.BabyId},{record.Time:yyyy-MM-dd HH:mm:ss},{record.Type},{record.AmountMl},{record.DurationMinutes},\"{notes}\"");
        }

        var fileName = $"FeedingRecords_{CurrentBaby?.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
        var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
        await File.WriteAllTextAsync(filePath, csv.ToString());

        await Share.RequestAsync(new ShareFileRequest
        {
          Title = "Export Feeding Records",
          File = new ShareFile(filePath)
        });
      }
      catch (Exception ex)
      {
        await Shell.Current.DisplayAlertAsync("Error", $"Failed to export: {ex.Message}", "OK");
      }
    }

    [RelayCommand]
    private async Task DeleteFeeding(FeedingRecord record)
    {
      if (record == null) return;

      bool checkDelete = await Shell.Current.DisplayAlertAsync("Confirm", "Are you sure you want to delete this feeding record?", "Yes", "Cancel");
      if (!checkDelete) return;

      int reowRemoved = await _database.DeleteFeedingRecordAsync(record);
      if (reowRemoved == 0)
      {
        await Shell.Current.DisplayAlertAsync("Error", "Failed to delete feeding record.", "OK");
      }
      else
      {
        FeedingRecords?.Remove(record);
      }

    }
  }
}
