using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CribSheet.Data;
using CribSheet.Models;
using System.Collections.ObjectModel;

namespace CribSheet.ViewModels
{
  public partial class HomeViewModel : ObservableObject
  {
    #region Fields

    private readonly CribSheetDatabase _database;
    private Baby? selectedBaby;

    #endregion

    #region Constructor

    public HomeViewModel(CribSheetDatabase database)
    {
      _database = database;
      _ = LoadBabiesAsync();
    }

    #endregion

    #region Properties

    [ObservableProperty]
    private ObservableCollection<Baby>? babies = new();

    public Baby? SelectedBaby
    {
      get => selectedBaby;
      set
      {
        if (SetProperty(ref selectedBaby, value) && value != null)
        {
          _ = NavigateToBaby(value);
          SetProperty(ref selectedBaby, null);
        }
      }
    }

    #endregion

    #region Commands

    [RelayCommand]
    private async Task AddNewBaby()
    {
      await Shell.Current.GoToAsync("//AddBabyPage");
    }

    #endregion

    #region Data Methods

    private async Task LoadBabiesAsync()
    {
      try
      {
        var babyList = await _database.GetBabiesAsync();
        Babies = new ObservableCollection<Baby>(babyList);
      }
      catch (Exception ex)
      {
        await Shell.Current.DisplayAlert("Error",
          $"Failed to load babies: {ex.Message}", "OK");
      }
    }

    public async Task RefreshBabies()
    {
      await LoadBabiesAsync();
    }

    #endregion

    #region Navigation

    private async Task NavigateToBaby(Baby baby)
    {
      if (baby == null) return;

      await Shell.Current.GoToAsync(
        "//CurrentBabyPage",
        true,
        new Dictionary<string, object>
        {
          { "Baby", baby }
        });
    }

    #endregion
  }
}
