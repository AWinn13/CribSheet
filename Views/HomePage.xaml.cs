using CribSheet.Data;

namespace CribSheet.Views;

public partial class HomePage : ContentPage
{
  public HomePage(CribSheetDatabase db)
  {
    InitializeComponent();
    BindingContext = new ViewModels.HomeViewModel(db);
  }

}