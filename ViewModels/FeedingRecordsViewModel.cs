using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CribSheet.Data;
using CribSheet.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CribSheet.ViewModels
{
  public partial class FeedingRecordsViewModel(CribSheetDatabase database) : BaseViewModel, IQueryAttributable
  {
    private readonly CribSheetDatabase _database = database;

    private int babyId;

    [ObservableProperty]
    private Baby? currentBaby;

    [ObservableProperty]
    private ObservableCollection<FeedingRecord>? feedingRecords;
    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
      if (query.ContainsKey("Baby"))
      {
        CurrentBaby = (Baby)query["Baby"];
        await LoadBabyDataAsync();
      }

    }

    [RelayCommand]
    private async Task NavigateToNewFeeding()
    {
      if (CurrentBaby == null) return;

      await Shell.Current.GoToAsync(
        nameof(Views.NewFeedingRecordPage),
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
