using CribSheet.Models;

namespace CribSheet.Services
{
  public class CurrentBabyService : ICurrentBaby
  {
    public long BabyId { get; set; } = -1;
    public Baby? SelectedBaby { get; set; }

    public void Clear()
    {
      BabyId = -1;
      SelectedBaby = null;
    }

    public Baby GetCurrentBaby()
    {
      return SelectedBaby ?? throw new InvalidOperationException("No baby selected.");
    }
  }
}
