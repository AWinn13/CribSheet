using CribSheet.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CribSheet
{
  public partial class App : Application
  {
    private readonly IPinService _pinService;
    public App(IPinService pinService)
    {
      InitializeComponent();
      _pinService = pinService;

    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
      return new Window(new AppShell());
    }

    protected override void OnStart()
    {
      MainThread.BeginInvokeOnMainThread(async () =>
      {
        if (await _pinService.HasPinAsync())
          await Shell.Current.GoToAsync("//LoginPage");
        else
          await Shell.Current.GoToAsync("//SetPinPage");
      });
    }
  }
}