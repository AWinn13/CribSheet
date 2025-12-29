using CribSheet.ViewModels;

namespace CribSheet.Views;

public partial class CurrentBabyPage : ContentPage
{
	private readonly CurrentBabyViewModel _viewModel;

	public CurrentBabyPage(CurrentBabyViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		// Refresh data when returning to this page
		await _viewModel.RefreshData();
	}
}