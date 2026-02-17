using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CribSheet.Data;
using CribSheet.Models;
using CribSheet.Services;

namespace CribSheet.ViewModels
{
  public partial class CurrentBabyViewModel : BaseViewModel
  {
    #region Fields

    private readonly CribSheetDatabase _database;

    #endregion
    #region Constructor
    public CurrentBabyViewModel(CribSheetDatabase database, ICurrentBaby currentBabyService)
      : base(currentBabyService)
    {
      _database = database;
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

    #endregion

    #region Navigation

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
    private async Task NavigateToNewFeeding()
    {
      await Shell.Current.GoToAsync(nameof(Views.NewFeedingRecordPage));
    }

    [RelayCommand]
    private async Task NavigateToNewPotty()
    {
      await Shell.Current.GoToAsync(nameof(Views.NewPottyRecordPage));
    }

    [RelayCommand]
    private async Task NavigateToNewSleep()
    {
      await Shell.Current.GoToAsync(nameof(Views.NewSleepRecordPage));
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

    private async Task LoadBabyDataAsync()
    {
      CurrentBaby = await _database.GetBabyAsync(CurrentBabyService.BabyId);
      if (CurrentBaby.Name == null || CurrentBaby == null) return;
      Name = CurrentBaby.Name;
      try
      {
        NoFeedingExists = !await _database.FeedingRecordsExist(CurrentBaby.BabyId);
        NoSleepExists = !await _database.SleepingRecordsExist(CurrentBaby.BabyId);
        NoPottyExists = !await _database.PottyRecordsExist(CurrentBaby.BabyId);
        AgeInMonths = (int)CurrentBaby.Age;
        Pounds = CurrentBaby.Weight / 16;
        Ounces = CurrentBaby.Weight % 16;
      }
      catch (Exception ex)
      {
        await Shell.Current.DisplayAlertAsync("Error",
          $"Failed to load baby data: {ex.Message}", "OK");
      }
    }
        #endregion


        private void RefreshProperties()
    {
      OnPropertyChanged(nameof(Name));
    }
  }
}
