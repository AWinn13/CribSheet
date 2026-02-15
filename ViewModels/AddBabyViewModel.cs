using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CribSheet.Data;
using CribSheet.Models;
using CribSheet.Services;

namespace CribSheet.ViewModels
{
  public partial class BabyViewModel : BaseViewModel
  {
    #region Fields

    private readonly CribSheetDatabase _database;
    private double weight;

    #endregion

    #region Constructor

    public BabyViewModel(CribSheetDatabase database, ICurrentBaby currentBabyService)
      : base(currentBabyService)
    {
      _database = database;
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
    private async Task AddBaby()
    {
      if (!ValidateForm())
        return;

      try
      {
        var baby = CreateBaby();
        var result = await _database.AddBabyAsync(baby);

        if (result == 1)
        {
          await NavigateToHomePage();
        }
        else
        {
          await Shell.Current.DisplayAlertAsync("Error",
            "Failed to add baby to database.", "OK");
        }
      }
      catch (Exception ex)
      {
        await Shell.Current.DisplayAlertAsync("Error",
          $"Failed to add baby: {ex.Message}", "OK");
      }
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

    private Baby CreateBaby()
    {
      return new Baby
      {
        CreatedAt = DateTime.Now,
        Dob = BirthDate,
        Name = Name,
        Weight = (long)((Lbs * 16) + Oz)
      };
    }

    private async Task NavigateToHomePage()
    {
      await Shell.Current.GoToAsync("//HomePage");
    }

    #endregion
  }
}
