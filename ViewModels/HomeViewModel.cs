using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CribSheet.Data;
using CribSheet.Models;
using CribSheet.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CribSheet.ViewModels
{
  public partial class HomeViewModel : BaseViewModel
  {
    #region Fields

    private readonly CribSheetDatabase _database;
    private Baby? selectedBaby;


    #endregion

    #region Constructor

    public HomeViewModel(CribSheetDatabase database, ICurrentBaby currentBabyService)
      : base(currentBabyService)
    {
      _database = database;
      CurrentBabyService.Clear();
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
          SetProperty(ref selectedBaby, null);
          OnPropertyChanged(nameof(SelectedBaby));
        }
      }
    }

    #endregion

    #region Commands

    [RelayCommand]
    private static async Task AddNewBaby()
    {
      await Shell.Current.GoToAsync("//AddBabyPage");
    }

    [RelayCommand]
    private async Task DeleteBaby (Baby baby)
    {
      if (baby == null) return;
      bool confirm = await Shell.Current.DisplayAlertAsync("Confirm Delete",
        $"Are you sure you want to delete {baby.Name}?", "Yes", "No");
      if (confirm)
      {
        await _database.DeleteBabyAsync(baby);
        await LoadBabiesAsync();
      }  
    }

    #endregion

    #region Data Methods

    private async Task LoadBabiesAsync()
    {
      try
      {
        var babyList = await _database.GetBabiesAsync();
        foreach (var kid in babyList)
        {
          
        }
        Babies = new ObservableCollection<Baby>(babyList);
      }
      catch (Exception ex)
      {
        await Shell.Current.DisplayAlertAsync("Error",
          $"Failed to load babies: {ex.Message}", "OK");
      }
    }

    public async Task RefreshBabies()
    {
      await LoadBabiesAsync();
    }

    #endregion

    #region Navigation

    private static async Task NavigateToBaby(Baby baby)
    {
      if (baby == null) return;

      await Shell.Current.GoToAsync("//CurrentBabyPage");
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
      base.OnPropertyChanged(e);
      if (e.PropertyName == nameof(SelectedBaby) && SelectedBaby != null)
      {
        CurrentBabyService.SelectedBaby = SelectedBaby;
        CurrentBabyService.BabyId = SelectedBaby.BabyId;
        _ = NavigateToBaby(SelectedBaby);
      }
    }
    #endregion
  }
}
