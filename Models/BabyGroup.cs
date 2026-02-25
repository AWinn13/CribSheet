using SQLite;

namespace CribSheet.Models
{
  public class BabyGroup
  {
    [PrimaryKey, AutoIncrement]
    public long GroupId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  }
}
