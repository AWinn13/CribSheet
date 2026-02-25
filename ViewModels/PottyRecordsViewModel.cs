using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CribSheet.Data;
using CribSheet.Models;
using CribSheet.Services;
using System.Collections.ObjectModel;

namespace CribSheet.ViewModels
{
  public partial class PottyRecordsViewModel : BaseViewModel
  {
    private readonly CribSheetDatabase _database;

    public PottyRecordsViewModel(CribSheetDatabase database, ICurrentBaby currentBabyService)
      : base(currentBabyService)
    {
      _database = database;
      CurrentBaby = CurrentBabyService.SelectedBaby;
      _ = LoadBabyDataAsync();
    }

    [ObservableProperty]
    private Baby? currentBaby;

    [ObservableProperty]
    private ObservableCollection<PottyRecord>? pottyRecords;


    private async Task LoadBabyDataAsync()
    {
      if (CurrentBaby == null) return;
      try
      {
        PottyRecords = new ObservableCollection<PottyRecord>(
          await _database.GetPottyRecordsAsync(CurrentBaby.BabyId));
      }
      catch (Exception ex)
      {
        await Shell.Current.DisplayAlertAsync("Error",
          $"Failed to load diaper data: {ex.Message}", "OK");
      }
    }

    [RelayCommand]
    private async Task DeletePotty(PottyRecord record)
    {
      if (record == null) return;

      bool checkDelete = await Shell.Current.DisplayAlertAsync("Confirm", "Are you sure you want to delete this diaper record?", "Yes", "Cancel");
      if (!checkDelete) return;

      int rowsRemoved = await _database.DeletePottyRecordAsync(record);
      if (rowsRemoved == 0)
      {
        await Shell.Current.DisplayAlertAsync("Error", "Failed to delete diaper record.", "OK");
      }
      else
      {
        PottyRecords?.Remove(record);
      }
    }

    [RelayCommand]
    private async Task EditPotty(PottyRecord record)
    {
      // Placeholder for future edit functionality
      await Shell.Current.DisplayAlertAsync("Info", "Edit functionality coming soon!", "OK");
    }
  }
}
