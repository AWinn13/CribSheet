using CribSheet.ViewModels;

namespace CribSheet.Views;

public partial class NewSleepRecordPage : ContentPage
{
	public NewSleepRecordPage(NewSleepRecordViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
