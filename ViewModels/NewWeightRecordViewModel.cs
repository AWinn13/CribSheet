using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CribSheet.Data;
using CribSheet.Models;
using CribSheet.Services;

namespace CribSheet.ViewModels
{
  public partial class NewWeightRecordViewModel : BaseViewModel
  {
    #region Fields

    private readonly CribSheetDatabase _database;

    #endregion

    #region Constructor

    public NewWeightRecordViewModel(CribSheetDatabase database, ICurrentBaby currentBabyService)
      : base(currentBabyService)
    {
      _database = database;
      RecordedDate = DateTime.Now.Date;
      RecordedTime = DateTime.Now.TimeOfDay;
    }

    #endregion

    #region Properties

    [ObservableProperty]
    private DateTime recordedDate;

    [ObservableProperty]
    private TimeSpan recordedTime;

    [ObservableProperty]
    private string? pounds;

    [ObservableProperty]
    private string? ounces;

    [ObservableProperty]
    private string? notes;

    #endregion

    #region Commands

    [RelayCommand]
    private async Task SaveWeight()
    {
      if (!ValidateForm())
        return;

      try
      {
        long lbs = string.IsNullOrWhiteSpace(Pounds) ? 0 : long.Parse(Pounds);
        long oz = string.IsNullOrWhiteSpace(Ounces) ? 0 : long.Parse(Ounces);
        long totalOz = lbs * 16 + oz;

        var record = new WeightRecord
        {
          BabyId = CurrentBabyService.BabyId,
          RecordedAt = RecordedDate.Add(RecordedTime),
          WeightOz = totalOz,
          Notes = Notes
        };

        await _database.AddWeightRecordAsync(record);

        // Keep Baby.Weight in sync with the latest entry
        var baby = await _database.GetBabyAsync(CurrentBabyService.BabyId);
        baby.Weight = totalOz;
        await _database.UpdateBabyAsync(baby);
        if (CurrentBabyService.SelectedBaby != null)
          CurrentBabyService.SelectedBaby.Weight = totalOz;

        await NavigateBack(new Dictionary<string, object> { { "refresh", true } });
      }
      catch (Exception ex)
      {
        await Shell.Current.DisplayAlertAsync("Error",
          $"Failed to save weight record: {ex.Message}", "OK");
      }
    }

    #endregion

    #region Private Methods

    private bool ValidateForm()
    {
      bool hasPounds = !string.IsNullOrWhiteSpace(Pounds);
      bool hasOunces = !string.IsNullOrWhiteSpace(Ounces);

      if (!hasPounds && !hasOunces)
      {
        Shell.Current.DisplayAlertAsync("Error", "Please enter a weight.", "OK");
        return false;
      }

      if (hasPounds && (!long.TryParse(Pounds, out long lbs) || lbs < 0))
      {
        Shell.Current.DisplayAlertAsync("Error", "Please enter a valid number of pounds.", "OK");
        return false;
      }

      if (hasOunces && (!long.TryParse(Ounces, out long oz) || oz < 0 || oz > 15))
      {
        Shell.Current.DisplayAlertAsync("Error", "Ounces must be between 0 and 15.", "OK");
        return false;
      }

      return true;
    }

    #endregion
  }
}
