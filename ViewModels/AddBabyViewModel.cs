using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CribSheet.Data;
using CribSheet.Models;
using System.Collections;
using System.ComponentModel;

namespace CribSheet.ViewModels
{
  public partial class BabyViewModel : ObservableObject, INotifyDataErrorInfo
  {
    #region Fields

    private readonly CribSheetDatabase _database;
    private readonly Dictionary<string, List<string>> _errors = new();
    private double weight;

    #endregion

    #region Constructor

    public BabyViewModel(CribSheetDatabase database)
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

    #region INotifyDataErrorInfo Implementation

    public bool HasErrors => _errors.Any();

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public IEnumerable GetErrors(string? propertyName)
    {
      if (string.IsNullOrEmpty(propertyName))
        return Enumerable.Empty<string>();

      if (_errors.ContainsKey(propertyName))
        return _errors[propertyName];

      return Enumerable.Empty<string>();
    }

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
          await Shell.Current.DisplayAlert("Error",
            "Failed to add baby to database.", "OK");
        }
      }
      catch (Exception ex)
      {
        await Shell.Current.DisplayAlert("Error",
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

    private void AddError(string propertyName, string errorMessage)
    {
      if (!_errors.ContainsKey(propertyName))
      {
        _errors[propertyName] = new List<string>();
      }

      if (!_errors[propertyName].Contains(errorMessage))
      {
        _errors[propertyName].Add(errorMessage);
        OnErrorsChanged(propertyName);
      }
    }

    private void ClearErrors(string propertyName)
    {
      if (_errors.ContainsKey(propertyName))
      {
        _errors[propertyName].Clear();
        OnErrorsChanged(propertyName);
      }
    }

    private void OnErrorsChanged(string propertyName)
    {
      ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    #endregion
  }
}
