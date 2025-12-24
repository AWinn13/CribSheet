using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace CribSheet.Models
{
  public enum DiaperType
  {
    Wet,
    Dirty,
    Both
  }
  public class PottyRecord
  {
    [PrimaryKey, AutoIncrement]
    public long PottyId { get; set; }

    [Indexed(Name = "idx_potty_baby_time", Order = 1)]
    public long BabyId { get; set; }

    [Indexed(Name = "idx_diaper_baby_time", Order = 2)]
    public DateTime Time { get; set; }

    public DiaperType Type { get; set; }

    public string? Notes { get; set; }
  }
}
