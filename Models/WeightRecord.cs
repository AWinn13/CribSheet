using SQLite;

namespace CribSheet.Models
{
  public class WeightRecord
  {
    [PrimaryKey, AutoIncrement]
    public long WeightId { get; set; }

    [Indexed(Name = "idx_weight_baby_time", Order = 1)]
    public long BabyId { get; set; }

    [Indexed(Name = "idx_weight_baby_time", Order = 2)]
    public DateTime RecordedAt { get; set; }

    public long WeightOz { get; set; }

    public string? Notes { get; set; }

    [Ignore]
    public long Pounds => WeightOz / 16;

    [Ignore]
    public long Ounces => WeightOz % 16;
  }
}
