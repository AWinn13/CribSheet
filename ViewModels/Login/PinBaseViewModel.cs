using CommunityToolkit.Mvvm.ComponentModel;
using System.Net.NetworkInformation;

namespace CribSheet.ViewModels
{
  

  public partial class PinViewModelBase : ObservableObject
  {
    [ObservableProperty]
    protected string pin = string.Empty;
    [ObservableProperty]
    protected string confirmPin = string.Empty;

    protected const int PinLength = 4;

    protected bool IsPinComplete => Pin.Length == PinLength;
  }
}
