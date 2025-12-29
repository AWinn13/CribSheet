using CribSheet.ViewModels;

namespace CribSheet.Views;

public partial class NewFeedingRecordPage : ContentPage
{
	public NewFeedingRecordPage(NewFeedingRecordViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
