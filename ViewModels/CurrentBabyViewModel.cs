using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CribSheet.Data;
using CribSheet.Models;
using System.Collections.ObjectModel;

namespace CribSheet.ViewModels
{
  public partial class CurrentBabyViewModel : ObservableObject, IQueryAttributable
  {
    #region Fields

    private readonly CribSheetDatabase _database;

    #endregion

    #region Constructor

    public CurrentBabyViewModel(CribSheetDatabase database)
    {
      _database = database;
    }

    #endregion

    #region Properties

    [ObservableProperty]
    private Baby? currentBaby;

    [ObservableProperty]
    private ObservableCollection<FeedingRecord>? feedingRecords;

    [ObservableProperty]
    private ObservableCollection<SleepRecord>? sleepRecords;

    [ObservableProperty]
    private ObservableCollection<PottyRecord>? pottyRecords;

    public string Name => CurrentBaby?.Name ?? "Unknown Baby";
    public long BabyId => CurrentBaby?.BabyId ?? 0;
    public DateTime? Dob => CurrentBaby?.Dob;

    #endregion

    #region Navigation

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
      if (query.ContainsKey("Baby"))
      {
        CurrentBaby = (Baby)query["Baby"];
        _ = LoadBabyDataAsync();
      }
    }

    #endregion

    #region Commands

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

    [RelayCommand]
    private async Task NavigateToNewPotty()
    {
      if (CurrentBaby == null) return;

      await Shell.Current.GoToAsync(
        nameof(Views.NewPottyRecordPage),
        new Dictionary<string, object>
        {
          { "BabyId", CurrentBaby.BabyId }
        });
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
    #endregion

    #region Data Methods

    public async Task RefreshData()
    {
      await LoadBabyDataAsync();
    }

    private async Task LoadBabyDataAsync()
    {
      if (CurrentBaby == null) return;

      try
      {
        FeedingRecords = new ObservableCollection<FeedingRecord>(
          await _database.GetFeedingRecordsAsync(CurrentBaby.BabyId));

        SleepRecords = new ObservableCollection<SleepRecord>(
          await _database.GetSleepRecordsAsync(CurrentBaby.BabyId));

        PottyRecords = new ObservableCollection<PottyRecord>(
          await _database.GetPottyRecordsAsync(CurrentBaby.BabyId));
      }
      catch (Exception ex)
      {
        await Shell.Current.DisplayAlertAsync("Error",
          $"Failed to load baby data: {ex.Message}", "OK");
      }
    }

    #endregion
  }
}
