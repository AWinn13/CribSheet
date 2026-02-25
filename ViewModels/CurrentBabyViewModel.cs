using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CribSheet.Data;
using CribSheet.Models;
using CribSheet.Services;
using System.Collections.ObjectModel;

namespace CribSheet.ViewModels
{
  public partial class CurrentBabyViewModel : BaseViewModel
  {
    #region Fields

    private readonly CribSheetDatabase _database;
    // Prevents OnSelectedGroupBabyChanged from firing a refresh during initial load
    private bool _suppressGroupBabyChange;

    #endregion

    #region Constructor

    public CurrentBabyViewModel(CribSheetDatabase database, ICurrentBaby currentBabyService)
      : base(currentBabyService)
    {
      _database = database;
      name = string.Empty;
      _ = LoadBabyDataAsync();
    }

    #endregion

    #region Properties

    [ObservableProperty]
    private Baby? currentBaby;

    private string name;
    public string Name
    {
      get => name;
      set
      {
        if (name != value)
        {
          name = value;
          OnPropertyChanged();
        }
      }
    }

    public DateTime? Dob => CurrentBaby?.Dob;

    [ObservableProperty]
    private long pounds;

    [ObservableProperty]
    private long ounces;

    [ObservableProperty]
    private int ageInMonths;

    [ObservableProperty]
    private bool noFeedingExists;

    [ObservableProperty]
    private bool noSleepExists;

    [ObservableProperty]
    private bool noPottyExists;

    [ObservableProperty]
    private bool noWeightExists;

    [ObservableProperty]
    private ObservableCollection<Baby> groupBabies = new();

    [ObservableProperty]
    private bool hasMultiples;

    [ObservableProperty]
    private Baby? selectedGroupBaby;

    #endregion

    #region Property Change Handlers

    partial void OnSelectedGroupBabyChanged(Baby? value)
    {
      if (value == null || _suppressGroupBabyChange) return;

      CurrentBabyService.BabyId = value.BabyId;
      CurrentBabyService.SelectedBaby = value;
      _ = RefreshForBabyAsync(value);
    }

    #endregion

    #region Commands

    [RelayCommand]
    private async Task NavigateToFeedings()
    {
      await Shell.Current.GoToAsync(nameof(Views.FeedingRecordsPage));
    }

    [RelayCommand]
    private async Task NavigateToSleep()
    {
      await Shell.Current.GoToAsync(nameof(Views.SleepRecordsPage));
    }

    [RelayCommand]
    private async Task NavigateToPotty()
    {
      await Shell.Current.GoToAsync(nameof(Views.PottyRecordsPage));
    }

    [RelayCommand]
    private async Task NavigateToAllRecords()
    {
      await Shell.Current.GoToAsync(nameof(Views.AllRecordsPage));
    }

    [RelayCommand]
    private async Task NavigateToWeightRecords()
    {
      await Shell.Current.GoToAsync(nameof(Views.WeightRecordsPage));
    }


    [RelayCommand]
    private async Task EditBaby()
    {
      await Shell.Current.GoToAsync(nameof(Views.EditBabyPage));
    }

    #endregion

    #region Data Methods

    public async Task RefreshData()
    {
      await LoadBabyDataAsync();
    }

    /// <summary>
    /// Full load: fetches the baby, builds the group tab list, then refreshes display data.
    /// </summary>
    private async Task LoadBabyDataAsync()
    {
      try
      {
        var baby = await _database.GetBabyAsync(CurrentBabyService.BabyId);
        if (baby == null || baby.Name == null) return;

        // Build sibling tab list
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

        // Set the selected tab without triggering a reload loop
        _suppressGroupBabyChange = true;
        SelectedGroupBaby = GroupBabies.FirstOrDefault(b => b.BabyId == baby.BabyId) ?? baby;
        _suppressGroupBabyChange = false;

        await RefreshForBabyAsync(baby);
      }
      catch (Exception ex)
      {
        await Shell.Current.DisplayAlertAsync("Error",
          $"Failed to load baby data: {ex.Message}", "OK");
      }
    }

    /// <summary>
    /// Refreshes the display data (name, age, weight, record existence) for a given baby.
    /// Does not rebuild the group tab list — safe to call when switching tabs.
    /// </summary>
    private async Task RefreshForBabyAsync(Baby baby)
    {
      await _database.MigrateExistingWeightAsync(baby);

      CurrentBaby = baby;
      Name = baby.Name ?? string.Empty;
      AgeInMonths = baby.Age ?? 0;
      Pounds = baby.Weight / 16;
      Ounces = baby.Weight % 16;

      NoFeedingExists = !await _database.FeedingRecordsExist(baby.BabyId);
      NoSleepExists = !await _database.SleepingRecordsExist(baby.BabyId);
      NoPottyExists = !await _database.PottyRecordsExist(baby.BabyId);
      NoWeightExists = !await _database.WeightRecordsExist(baby.BabyId);
    }

    #endregion
  }
}
