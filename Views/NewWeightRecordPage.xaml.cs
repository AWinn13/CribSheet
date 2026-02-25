using CribSheet.ViewModels;

namespace CribSheet.Views;

public partial class NewWeightRecordPage : ContentPage
{
  public NewWeightRecordPage(NewWeightRecordViewModel viewModel)
  {
    InitializeComponent();
    BindingContext = viewModel;
  }
}
