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

    public SleepType TypeOfSleep { get; set; }

    public WakeReason WhyNoSleep { get; set; }
    public string? Notes { get; set; }

  [Ignore]
    public TimeSpan? Duration =>
       EndTime.HasValue ? EndTime.Value - StartTime : null;

  }
  public enum SleepType
  {
    Night,
    Nap
  }

  public enum WakeReason
  {
    Diaper,
    Feeding,
    Other
  }
}
