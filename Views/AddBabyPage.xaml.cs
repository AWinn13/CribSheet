using CribSheet.Data;

namespace CribSheet.Views;

public partial class AddBabyPage : ContentPage
{
	public AddBabyPage(CribSheetDatabase db)
	{
		InitializeComponent();
    BindingContext = new ViewModels.BabyViewModel(db);

  }

}