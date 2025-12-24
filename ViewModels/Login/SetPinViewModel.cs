using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CribSheet.Services;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace CribSheet.ViewModels
{
    public partial class SetPinViewModel: PinViewModelBase
  {
    private readonly IPinService _pinService;

    [ObservableProperty]
    private bool _showPinSetup;

    public SetPinViewModel(IPinService pinService)
    {
      _pinService = pinService;
    }

    [RelayCommand]
    private Task CreatePIN()
    {
      ShowPinSetup = true;
      return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task CancelPinSetupAsync()
    {
      Pin = string.Empty;
      ConfirmPin = string.Empty;
      ShowPinSetup = false;
      await Shell.Current.GoToAsync("//HomePage");
    }

    [RelayCommand]
    private async Task SavePinAsync()
    {
      if(!string.Equals(Pin, ConfirmPin))
      {
        await Shell.Current.DisplayAlertAsync("Error", "PINs do not match.", "OK");
        Pin = string.Empty;
        ConfirmPin = string.Empty;
        return;
      }
      if (!IsPinComplete)
        return;

      await _pinService.SavePinAsync(Pin);
      await Shell.Current.GoToAsync("//HomePage");
    }
  }
}
