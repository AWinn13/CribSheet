using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CribSheet.Data;
using CribSheet.Models;
using CribSheet.Services;

namespace CribSheet.ViewModels
{
  public partial class EditBabyViewModel : BaseViewModel
  {
    #region Fields

    private readonly CribSheetDatabase _database;
    private double weight;
    private Baby? _currentBaby;

    #endregion

    #region Constructor

    public EditBabyViewModel(CribSheetDatabase database, ICurrentBaby currentBabyService)
      : base(currentBabyService)
    {
      _database = database;
      _currentBaby = CurrentBabyService.SelectedBaby;
      LoadBabyData();
    }

    #endregion

    #region Properties

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private DateTime birthDate;

    [ObservableProperty]
    private double lbs;

    [ObservableProperty]
    private double oz;

    #endregion

    #region Commands

    [RelayCommand]
    private async Task UpdateBaby()
    {
      if (!ValidateForm())
        return;

      try
      {
        if (_currentBaby == null) return;

        _currentBaby.Name = Name;
        _currentBaby.Dob = BirthDate;
        _currentBaby.Weight = (long)((Lbs * 16) + Oz);

        var result = await _database.UpdateBabyAsync(_currentBaby);

        if (result == 1)
        {
          await NavigateBackWithBaby();
        }
        else
        {
          await Shell.Current.DisplayAlertAsync("Error",
            "Failed to update baby information.", "OK");
        }
      }
      catch (Exception ex)
      {
        await Shell.Current.DisplayAlertAsync("Error",
          $"Failed to update baby: {ex.Message}", "OK");
      }
    }

    [RelayCommand]
    private async Task Cancel()
    {
      await NavigateBackWithBaby();
    }

    #endregion

    #region Validation

    partial void OnNameChanged(string value)
    {
      ValidateName(value);
    }

    partial void OnBirthDateChanged(DateTime value)
    {
      ValidateBirthDate(value);
    }

    partial void OnLbsChanged(double value)
    {
      ValidateWeight();
    }

    partial void OnOzChanged(double value)
    {
      ValidateWeight();
    }

    private bool ValidateForm()
    {
      ValidateName(Name);
      ValidateBirthDate(BirthDate);
      ValidateWeight();
      return !HasErrors;
    }

    private void ValidateName(string value)
    {
      if (string.IsNullOrWhiteSpace(value))
      {
       AddError(nameof(Name), "Name cannot be empty.");
      }
      else
      {
        ClearErrors(nameof(Name));
      }
    }

    private void ValidateBirthDate(DateTime value)
    {
      if (value >= DateTime.Now)
      {
        AddError(nameof(BirthDate), "Birthdate cannot be in the future.");
      }
      else
      {
        ClearErrors(nameof(BirthDate));
      }
    }

    private void ValidateWeight()
    {
      double totalWeight = (Lbs * 16) + Oz;
      if (totalWeight <= 0)
      {
        AddError(nameof(weight), "Weight must be greater than zero.");
      }
      else
      {
        ClearErrors(nameof(weight));
      }
    }

    #endregion

    #region Private Methods

    private void LoadBabyData()
    {
      if (_currentBaby == null) return;

      Name = _currentBaby.Name ?? string.Empty;
      BirthDate = _currentBaby.Dob ?? DateTime.Now;

      // Convert weight from ounces to pounds and ounces
      long totalOunces = _currentBaby.Weight;
      Lbs = totalOunces / 16;
      Oz = totalOunces % 16;
    }

    private async Task NavigateBackWithBaby()
    {
      await NavigateBack(new Dictionary<string, object>
       {
          { "Baby", _currentBaby }
       });
    }

    #endregion
  }
}
