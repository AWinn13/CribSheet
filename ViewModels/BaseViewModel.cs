using CommunityToolkit.Mvvm.ComponentModel;
using CribSheet.Services;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace CribSheet.ViewModels
{
  public partial class BaseViewModel : ObservableObject, INotifyDataErrorInfo
  {
    #region Service

    protected readonly ICurrentBaby CurrentBabyService;

    protected BaseViewModel(ICurrentBaby currentBabyService)
    {
      CurrentBabyService = currentBabyService;
    }

    #endregion

    #region INotifyDataErrorInfo Implementation

    private readonly Dictionary<string, List<string>> _errors = new();

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

    protected void AddError(string propertyName, string errorMessage)
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

    protected void ClearErrors(string propertyName)
    {
      if (_errors.ContainsKey(propertyName))
      {
        _errors.Remove(propertyName);
        OnErrorsChanged(propertyName);
      }
    }

    protected void OnErrorsChanged(string propertyName)
    {
      ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
      OnPropertyChanged(nameof(HasErrors));
    }

    #endregion

    #region Navigation

    protected async Task NavigateBack()
    {
      await Shell.Current.GoToAsync("..");
    }

    protected async Task NavigateBack(IDictionary<string, object> parameters)
    {
      await Shell.Current.GoToAsync("..", parameters);
    }

    #endregion
  }
}

