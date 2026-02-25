using CribSheet.ViewModels;

namespace CribSheet.Views;

public partial class AllRecordsPage : ContentPage
{
  private readonly AllRecordsViewModel _viewModel;

  public AllRecordsPage(AllRecordsViewModel viewModel)
  {
    InitializeComponent();
    BindingContext = _viewModel = viewModel;
  }

}
