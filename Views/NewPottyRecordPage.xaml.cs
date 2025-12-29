using CribSheet.ViewModels;

namespace CribSheet.Views;

public partial class NewPottyRecordPage : ContentPage
{
	public NewPottyRecordPage(NewPottyRecordViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
