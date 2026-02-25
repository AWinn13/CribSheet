using CribSheet.ViewModels;

namespace CribSheet.Views;

public partial class WeightRecordsPage : ContentPage
{
  private readonly WeightRecordsViewModel _viewModel;

  public WeightRecordsPage(WeightRecordsViewModel viewModel)
  {
    InitializeComponent();
    BindingContext = _viewModel = viewModel;
  }
}
