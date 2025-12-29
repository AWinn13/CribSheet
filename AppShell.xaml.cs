using CribSheet.Services;
using CribSheet.Views;

namespace CribSheet
{
  public partial class AppShell : Shell
  {
    public AppShell()
    {
      InitializeComponent();

      // Register routes for navigation
      Routing.RegisterRoute(nameof(NewFeedingRecordPage), typeof(NewFeedingRecordPage));
      Routing.RegisterRoute(nameof(NewPottyRecordPage), typeof(NewPottyRecordPage));
      Routing.RegisterRoute(nameof(NewSleepRecordPage), typeof(NewSleepRecordPage));
    }
  }
}
