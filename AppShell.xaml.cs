
using CribSheet.Views;

namespace CribSheet
{
  public partial class AppShell : Shell
  {
    public AppShell()
    {
      InitializeComponent();
       
      // Register routes for navigation
      Routing.RegisterRoute(nameof(EditBabyPage), typeof(EditBabyPage));
      Routing.RegisterRoute(nameof(NewFeedingRecordPage), typeof(NewFeedingRecordPage));
      Routing.RegisterRoute(nameof(NewPottyRecordPage), typeof(NewPottyRecordPage));
      Routing.RegisterRoute(nameof(NewSleepRecordPage), typeof(NewSleepRecordPage));
     // Routing.RegisterRoute(nameof(FeedingRecordsPage), typeof(FeedingRecordsPage));
      //Routing.RegisterRoute(nameof(SleepRecordsPage), typeof(SleepRecordsPage));
      //Routing.RegisterRoute(nameof(PottyRecordsPage), typeof(PottyRecordsPage));
    }
  }
}
