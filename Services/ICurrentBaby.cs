using CribSheet.Models;

namespace CribSheet.Services
{
  public interface ICurrentBaby
  {
    long BabyId { get; set; }
    Baby? SelectedBaby { get; set; }
    Baby GetCurrentBaby();
    void Clear();
  }
}
