using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CribSheet.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace CribSheet.ViewModels
{
  public partial class LoginViewModel : ObservableObject
  {
    private readonly IPinService _pinService;


    [ObservableProperty]
    private string pin;


    [ObservableProperty]
    private string errorMessage;

    [ObservableProperty]
    private bool isError;

    public LoginViewModel(IPinService pinService)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
      _pinService = pinService;
      LoginAsyncCommand = new AsyncRelayCommand(LoginAsync);
    }

    public IAsyncRelayCommand LoginAsyncCommand { get; }


    private async Task LoginAsync()
    {

      bool success = await _pinService.ValidatePinAsync(Pin);
      if (!success)
      {
        IsError = true;
        ErrorMessage = "Invalid PIN. Please try again.";
        Pin = string.Empty;
        return;
      }
      ErrorMessage = string.Empty;
      await Shell.Current.GoToAsync("//HomePage");
      
    }


  }
}
