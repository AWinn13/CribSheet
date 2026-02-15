
using Microsoft.Extensions.DependencyInjection;

namespace CribSheet
{
  public partial class App : Application
  {
    public App()
    {
      InitializeComponent();

    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
      return new Window(new AppShell());
    }

    protected override void OnStart()
    {
      MainThread.BeginInvokeOnMainThread(async () =>
      {
        await Shell.Current.GoToAsync("//HomePage");
      });
    }
  }
}