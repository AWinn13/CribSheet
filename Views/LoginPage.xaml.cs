using CribSheet.Services;

namespace CribSheet.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage(IPinService pinService)
	{
		InitializeComponent();
		BindingContext = new ViewModels.LoginViewModel(pinService);
  }

}