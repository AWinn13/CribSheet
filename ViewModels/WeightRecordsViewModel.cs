using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CribSheet.Data;
using CribSheet.Models;
using CribSheet.Services;
using System.Collections.ObjectModel;

namespace CribSheet.ViewModels
{
  public partial class WeightRecordsViewModel : BaseViewModel
  {
    private readonly CribSheetDatabase _database;

    public WeightRecordsViewModel(CribSheetDatabase database, ICurrentBaby currentBabyService)
      : base(currentBabyService)
    {
      _database = database;
      _ = LoadWeightRecordsAsync();
    }

    [ObservableProperty]
    private ObservableCollection<WeightRecord>? weightRecords;

    private async Task LoadWeightRecordsAsync()
    {
      try
      {
        var babyId = CurrentBabyService.BabyId;
        WeightRecords = new ObservableCollection<WeightRecord>(
          await _database.GetWeightRecordsAsync(babyId));
      }
      catch (Exception ex)
      {
        await Shell.Current.DisplayAlertAsync("Error",
          $"Failed to load weight records: {ex.Message}", "OK");
      }
    }

    [RelayCommand]
    private async Task DeleteWeight(WeightRecord record)
    {
      if (record == null) return;

      bool confirm = await Shell.Current.DisplayAlertAsync("Confirm",
        "Are you sure you want to delete this weight record?", "Yes", "Cancel");
      if (!confirm) return;

      int rowsRemoved = await _database.DeleteWeightRecordAsync(record);
      if (rowsRemoved == 0)
      {
        await Shell.Current.DisplayAlertAsync("Error", "Failed to delete weight record.", "OK");
      }
      else
      {
        WeightRecords?.Remove(record);
      }
    }
  }
}
