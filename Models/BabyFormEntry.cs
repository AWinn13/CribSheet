using CommunityToolkit.Mvvm.ComponentModel;

namespace CribSheet.Models
{
  /// <summary>
  /// UI-only model representing one baby's form fields on the Add Baby page.
  /// Not stored directly in the database.
  /// </summary>
  public partial class BabyFormEntry : ObservableObject
  {
    [ObservableProperty]
    private string label = string.Empty;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private double lbs;

    [ObservableProperty]
    private double oz;
  }
}
