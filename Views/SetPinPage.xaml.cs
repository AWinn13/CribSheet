using CribSheet.Services;

namespace CribSheet.Views;

public partial class SetPinPage : ContentPage
{
	public SetPinPage(IPinService pinStore)
	{
		InitializeComponent();
		BindingContext = new ViewModels.SetPinViewModel(pinStore);
  }
}