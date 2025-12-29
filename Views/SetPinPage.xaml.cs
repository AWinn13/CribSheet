using CribSheet.Services;

namespace CribSheet.Views;

public partial class SetPinPage : ContentPage
{
  public SetPinPage(IPinService pinStore)
  {
    InitializeComponent();
    BindingContext = new ViewModels.SetPinViewModel(pinStore);
  }

  private async void Button_Clicked(object sender, EventArgs e)
  {
    bool answer = await DisplayAlertAsync("Question?", "Would you like to be reminded to set up a PIN?", "Yes", "No");
    if (answer)
    {
       
      // User clicked Yes
    }
    else
    {
      Preferences.Set("RemindPinSetup", false);
      if (BindingContext is ViewModels.SetPinViewModel vm && vm != null)
      {
        await vm.CancelPinSetupCommand.ExecuteAsync(null);
      }
    }
  }
}