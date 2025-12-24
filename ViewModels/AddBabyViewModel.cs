
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CribSheet.Data;
using CribSheet.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CribSheet.ViewModels
{
  public partial class BabyViewModel : ObservableObject, INotifyDataErrorInfo
  {
    private CribSheetDatabase _database;

    // The errors dictionary
    private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private DateTime birthDate;

    [ObservableProperty]
    private double lbs;

    [ObservableProperty]
    private double oz;

    [ObservableProperty]
    private List<Baby>? babies = new();

    private double weight;

    public BabyViewModel(CribSheetDatabase database)
    {
      AddBabyAsyncCommand = new AsyncRelayCommand(AddBabyAsync);
      _database = database;
    }

    public IAsyncRelayCommand AddBabyAsyncCommand { get; }

    private async Task AddBabyAsync()
    {
      var baby = new Baby
      {
        CreatedAt = DateTime.Now,
        Dob = this.BirthDate,
        Name = this.Name,
        Weight = (long)((this.Lbs * 16) + this.Oz)
      };
      ValidateName(baby.Name);
      ValidateBirthDate(baby.Dob ?? DateTime.MinValue);
      ValidateWeight();
      if (HasErrors)
      {
        // Validation failed, return early
        return;
      }

      await _database.AddBabyAsync(baby);
      var babyList = await _database.GetBabiesAsync();
      Babies = babyList.ToList();

    }

    // IDataErrorInfo implementation (for validation)
    public bool HasErrors => _errors.Any();

    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    public IEnumerable<string> GetErrors(string propertyName)
    {
      if (_errors.ContainsKey(propertyName))
        return _errors[propertyName];
      return Enumerable.Empty<string>();
    }

    // Validate Name property
    partial void OnNameChanged(string value)
    {
      ValidateName(value);
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

    // Validate BirthDate property
    partial void OnBirthDateChanged(DateTime value)
    {
      ValidateBirthDate(value);
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

    // Validate Lbs and Oz properties (Weight)
    partial void OnLbsChanged(double value)
    {
      ValidateWeight();
    }

    partial void OnOzChanged(double value)
    {
      ValidateWeight();
    }

    private void ValidateWeight()
    {
      double totalWeight = (this.Lbs * 16) + this.Oz;
      if (totalWeight <= 0)
      {
        AddError(nameof(weight), "Weight must be greater than zero.");
      }
      else
      {
        ClearErrors(nameof(weight));
      }
    }

    // Helper methods for adding/clearing errors
    private void AddError(string propertyName, string errorMessage)
    {
      if (!_errors.ContainsKey(propertyName))
      {
        _errors[propertyName] = new List<string>();
      }
      _errors[propertyName].Add(errorMessage);
      OnErrorsChanged(propertyName);
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

    IEnumerable INotifyDataErrorInfo.GetErrors(string? propertyName)
    {
      return GetErrors(propertyName);
    }
  }
}
