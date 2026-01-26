using CribSheet.ViewModels;

namespace CribSheet.Views;

public partial class FeedingRecordsPage : ContentPage
{
  private readonly FeedingRecordsViewModel _viewModel;
  public FeedingRecordsPage(FeedingRecordsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
  }
}