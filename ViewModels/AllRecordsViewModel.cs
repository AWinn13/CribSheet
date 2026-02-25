using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CribSheet.Data;
using CribSheet.Models;
using CribSheet.Services;
using System.Collections.ObjectModel;

namespace CribSheet.ViewModels
{
  public partial class AllRecordsViewModel : BaseViewModel, IQueryAttributable
  {
    private readonly CribSheetDatabase _database;
    private bool _suppressGroupBabyChange;

    public AllRecordsViewModel(CribSheetDatabase database, ICurrentBaby currentBabyService)
      : base(currentBabyService)
    {
      _database = database;
      ShowFeeding = true;
      _ = InitializeAsync();
    }

    #region Record type tab properties

    public List<string> TabOptions { get; } = new() { "Feeding", "Diaper", "Sleep", "Weight" };

    [ObservableProperty]
    private string selectedTab = "Feeding";

    [ObservableProperty]
    private bool showFeeding;

    [ObservableProperty]
    private bool showDiaper;

    [ObservableProperty]
    private bool showSleep;

    [ObservableProperty]
    private bool showWeight;

    partial void OnSelectedTabChanged(string value)
    {
      ShowFeeding = value == "Feeding";
      ShowDiaper = value == "Diaper";
      ShowSleep = value == "Sleep";
      ShowWeight = value == "Weight";
    }

    #endregion

    #region Sibling tab properties

    [ObservableProperty]
    private ObservableCollection<Baby> groupBabies = new();

    [ObservableProperty]
    private bool hasMultiples;

    [ObservableProperty]
    private Baby? selectedGroupBaby;

    partial void OnSelectedGroupBabyChanged(Baby? value)
    {
      if (value == null || _suppressGroupBabyChange) return;
      CurrentBabyService.BabyId = value.BabyId;
      CurrentBabyService.SelectedBaby = value;
      _ = LoadAllRecordsAsync();
    }

    #endregion

    #region Record collections

    [ObservableProperty]
    private ObservableCollection<FeedingRecord>? feedingRecords;

    [ObservableProperty]
    private ObservableCollection<PottyRecord>? pottyRecords;

    [ObservableProperty]
    private ObservableCollection<SleepRecord>? sleepRecords;

    [ObservableProperty]
    private ObservableCollection<WeightRecord>? weightRecords;

    #endregion

    #region IQueryAttributable

    async void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
    {
      if (query.ContainsKey("refresh"))
      {
        await LoadAllRecordsAsync();
      }
    }

    #endregion

    #region Commands — Navigation

  

    #endregion

    #region Commands — Feeding

    [RelayCommand]
    private async Task EditFeeding(FeedingRecord record)
    {
      await Shell.Current.DisplayAlertAsync("Info", "Edit functionality coming soon!", "OK");
    }

    [RelayCommand]
    private async Task DeleteFeeding(FeedingRecord record)
    {
      if (record == null) return;
      bool confirm = await Shell.Current.DisplayAlertAsync("Confirm",
        "Are you sure you want to delete this feeding record?", "Yes", "Cancel");
      if (!confirm) return;

      int rows = await _database.DeleteFeedingRecordAsync(record);
      if (rows == 0) await Shell.Current.DisplayAlertAsync("Error", "Failed to delete feeding record.", "OK");
      else FeedingRecords?.Remove(record);
    }

    #endregion

    #region Commands — Diaper

    [RelayCommand]
    private async Task EditPotty(PottyRecord record)
    {
      await Shell.Current.DisplayAlertAsync("Info", "Edit functionality coming soon!", "OK");
    }

    [RelayCommand]
    private async Task DeletePotty(PottyRecord record)
    {
      if (record == null) return;
      bool confirm = await Shell.Current.DisplayAlertAsync("Confirm",
        "Are you sure you want to delete this diaper record?", "Yes", "Cancel");
      if (!confirm) return;

      int rows = await _database.DeletePottyRecordAsync(record);
      if (rows == 0) await Shell.Current.DisplayAlertAsync("Error", "Failed to delete diaper record.", "OK");
      else PottyRecords?.Remove(record);
    }

    #endregion

    #region Commands — Sleep

    [RelayCommand]
    private async Task EditSleep(SleepRecord record)
    {
      await Shell.Current.DisplayAlertAsync("Info", "Edit functionality coming soon!", "OK");
    }

    [RelayCommand]
    private async Task DeleteSleep(SleepRecord record)
    {
      if (record == null) return;
      bool confirm = await Shell.Current.DisplayAlertAsync("Confirm",
        "Are you sure you want to delete this sleep record?", "Yes", "Cancel");
      if (!confirm) return;

      int rows = await _database.DeleteSleepRecordAsync(record);
      if (rows == 0) await Shell.Current.DisplayAlertAsync("Error", "Failed to delete sleep record.", "OK");
      else SleepRecords?.Remove(record);
    }

    #endregion

    #region Commands — Weight

    [RelayCommand]
    private async Task DeleteWeight(WeightRecord record)
    {
      if (record == null) return;
      bool confirm = await Shell.Current.DisplayAlertAsync("Confirm",
        "Are you sure you want to delete this weight record?", "Yes", "Cancel");
      if (!confirm) return;

      int rows = await _database.DeleteWeightRecordAsync(record);
      if (rows == 0) await Shell.Current.DisplayAlertAsync("Error", "Failed to delete weight record.", "OK");
      else WeightRecords?.Remove(record);
    }

    #endregion

    #region Data loading

    /// <summary>
    /// Full init: builds the sibling group tab list then loads records.
    /// Called once from the constructor.
    /// </summary>
    private async Task InitializeAsync()
    {
      try
      {
        var baby = await _database.GetBabyAsync(CurrentBabyService.BabyId);
        if (baby == null) return;

        if (baby.IsAMultiple && baby.GroupId != null)
        {
          var siblings = await _database.GetBabiesByGroupAsync(baby.GroupId.Value);
          GroupBabies = new ObservableCollection<Baby>(siblings);
        }
        else
        {
          GroupBabies = new ObservableCollection<Baby>();
        }

        HasMultiples = GroupBabies.Count > 1;

        _suppressGroupBabyChange = true;
        SelectedGroupBaby = GroupBabies.FirstOrDefault(b => b.BabyId == baby.BabyId) ?? baby;
        _suppressGroupBabyChange = false;

        await LoadAllRecordsAsync();
      }
      catch (Exception ex)
      {
        await Shell.Current.DisplayAlertAsync("Error",
          $"Failed to initialize records: {ex.Message}", "OK");
      }
    }

    /// <summary>
    /// Loads the 4 record collections for the current baby.
    /// Safe to call on refresh or sibling tab switch.
    /// </summary>
    private async Task LoadAllRecordsAsync()
    {
      var babyId = CurrentBabyService.BabyId;
      try
      {
        FeedingRecords = new ObservableCollection<FeedingRecord>(
          await _database.GetFeedingRecordsAsync(babyId));
        PottyRecords = new ObservableCollection<PottyRecord>(
          await _database.GetPottyRecordsAsync(babyId));
        SleepRecords = new ObservableCollection<SleepRecord>(
          await _database.GetSleepRecordsAsync(babyId));
        WeightRecords = new ObservableCollection<WeightRecord>(
          await _database.GetWeightRecordsAsync(babyId));
      }
      catch (Exception ex)
      {
        await Shell.Current.DisplayAlertAsync("Error",
          $"Failed to load records: {ex.Message}", "OK");
      }
    }

    #endregion
  }
}
