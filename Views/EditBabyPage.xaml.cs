using CribSheet.ViewModels;

namespace CribSheet.Views;

public partial class EditBabyPage : ContentPage
{
	public EditBabyPage(EditBabyViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
