using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CribSheet.Data;
using CribSheet.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace CribSheet.ViewModels
{
  public partial class HomeViewModel : ObservableObject
  {
    CribSheetDatabase _database;

    [ObservableProperty]
    private ObservableCollection<Baby>? babies = new();

    [ObservableProperty]
    private Baby? selectedBaby;

    public HomeViewModel(CribSheetDatabase database)
    {
      _database = database;
      AddNewBabyCommand = new AsyncRelayCommand(AddNewBaby);
      GoToBabyCommand = new AsyncRelayCommand<Baby>(async (baby) =>
      {
        if (baby != null)
        {
          await Shell.Current.GoToAsync("//CurrentBabyPage", true, new Dictionary<string, object>
          {
            { "Baby", baby }
          });
        }
      });
      LoadBabiesAsync();
    }

    private async Task AddNewBaby()
    {
      await Shell.Current.GoToAsync("//AddBabyPage");
    }

    public IAsyncRelayCommand AddNewBabyCommand { get; }
    public IAsyncRelayCommand GoToBabyCommand { get; }

    private async void LoadBabiesAsync()
    {
      Babies = new ObservableCollection<Baby>(await _database.GetBabiesAsync());
    }

   
  }
}
