using CribSheet.ViewModels;

namespace CribSheet.Views;

public partial class AddBabyPage : ContentPage
{
  public AddBabyPage(BabyViewModel viewModel)
  {
    InitializeComponent();
    BindingContext = viewModel;
  }
}