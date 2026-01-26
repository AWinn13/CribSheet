using CribSheet.ViewModels;

namespace CribSheet.Views;

public partial class SleepRecordsPage : ContentPage
{
	public SleepRecordsPage(SleepRecordsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
