using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CribSheet.Data;
using CribSheet.Models;
using System.Collections.ObjectModel;
using static System.Net.Mime.MediaTypeNames;

namespace CribSheet.ViewModels
{
  public partial class CurrentBabyViewModel(CribSheetDatabase database) : BaseViewModel, IQueryAttributable
  {
    #region Fields

    private readonly CribSheetDatabase _database = database;

    #endregion
    #region Constructor

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
    public long BabyId => CurrentBaby?.BabyId ?? 0;
    public DateTime? Dob => CurrentBaby?.Dob;

    [ObservableProperty]
    private long weight;

    [ObservableProperty]
    private int ageInMonths;

    #endregion

    #region Navigation

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
      if (query.ContainsKey("Baby"))
      {
        CurrentBaby = (Baby)query["Baby"];
        if (CurrentBaby == null) return;
        name = CurrentBaby.Name;
        AgeInMonths = CurrentBaby.GetBabyAge();
        RefreshProperties();
        LoadBabyDataAsync();
      }

    }

    #endregion

    #region Commands

    [RelayCommand]
    private async Task NavigateToFeedings()
    {
      if (CurrentBaby == null) return;
      await Shell.Current.GoToAsync(
       nameof(Views.FeedingRecordsPage),
       new Dictionary<string, object>
       {
          { "Baby", CurrentBaby}
       });
    }

    [RelayCommand]
    private async Task NavigateToSleep()
    {
      if (CurrentBaby == null) return;
      await Shell.Current.GoToAsync(
       nameof(Views.SleepRecordsPage),
       new Dictionary<string, object>
       {
          { "Baby", CurrentBaby}
       });
    }

    [RelayCommand]
    private async Task NavigateToPotty()
    {
      if (CurrentBaby == null) return;
      await Shell.Current.GoToAsync(
       nameof(Views.PottyRecordsPage),
       new Dictionary<string, object>
       {
          { "Baby", CurrentBaby}
       });
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
    private async Task EditBaby()
    {
      if (CurrentBaby == null) return;

      await Shell.Current.GoToAsync(
        nameof(Views.EditBabyPage),
        new Dictionary<string, object>
        {
          { "Baby", CurrentBaby }
        });
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
        Weight = CurrentBaby.Weight;
        // Sleep and Potty records are now loaded in their respective dedicated pages
        // This method is kept for future data loading if needed
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
