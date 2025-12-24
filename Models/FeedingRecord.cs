using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace CribSheet.Models
{
  public enum FeedingType
  {
    Bottle,
    Breast,
    Solid
  }

  public class FeedingRecord 
  {
    [PrimaryKey, AutoIncrement]
    public long FeedingId { get; set; }

    [Indexed(Name = "idx_feeding_baby_time", Order = 1)]
    public long BabyId { get; set; }

    [Indexed(Name = "idx_feeding_baby_time", Order = 2)]
    public DateTime Time { get; set; }

    public FeedingType Type { get; set; }

    public int? AmountMl { get; set; }
    public int? DurationMinutes { get; set; }

    public string? Notes { get; set; }

  }
}
