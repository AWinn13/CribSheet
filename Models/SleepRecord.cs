using SQLite;
namespace CribSheet.Models
{

  public class SleepRecord
  {
    [PrimaryKey, AutoIncrement]
    public long SleepId { get; set; }

    [Indexed(Name = "idx_sleep_baby_time", Order = 1)]
    public long BabyId { get; set; }

    [Indexed(Name = "idx_sleep_baby_time", Order = 2)]
    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public string? Notes { get; set; }

  }
}
