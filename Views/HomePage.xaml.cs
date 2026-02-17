using CribSheet.ViewModels;

namespace CribSheet.Views;

public partial class HomePage : ContentPage
{
  public HomePage(HomeViewModel viewModel)
  {
    InitializeComponent();
    BindingContext = viewModel;
  }
}