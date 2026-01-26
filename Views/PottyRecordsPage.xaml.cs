using CribSheet.ViewModels;

namespace CribSheet.Views;

public partial class PottyRecordsPage : ContentPage
{
	public PottyRecordsPage(PottyRecordsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
