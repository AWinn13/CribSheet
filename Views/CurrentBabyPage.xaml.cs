using CribSheet.Data;

namespace CribSheet.Views;

public partial class CurrentBabyPage : ContentPage
{
	public CurrentBabyPage(CribSheetDatabase db)
	{
		InitializeComponent();
		BindingContext = new ViewModels.CurrentBabyViewModel(db);
  }
}