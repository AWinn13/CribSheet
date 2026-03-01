namespace CribSheet.Models
{
  public class SleepCalendarDay
  {
    public DateTime Date { get; set; }
    public List<SleepRecord> Records { get; set; } = new();

    public string TotalSleepText
    {
      get
      {
        var totalMinutes = Records
          .Where(r => r.Duration.HasValue)
          .Sum(r => r.Duration!.Value.TotalMinutes);
        int hours = (int)(totalMinutes / 60);
        int minutes = (int)(totalMinutes % 60);
        return $"{hours}h {minutes}m";
      }
    }
  }
}
