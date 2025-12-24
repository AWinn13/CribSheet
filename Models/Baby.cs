using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace CribSheet.Models
{


  public class Baby
  {
    [PrimaryKey, AutoIncrement]
    public long BabyId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? Name { get; set; }

    public long? Weight { get; set; }

    public DateTime? Dob { get; set; } // date only in DB; DateTime works fine

    // Navigation

    //public List<BabyUser> BabyUsers { get; set; } = new();


    //public List<FeedingRecord> FeedingRecords { get; set; } = new();

    //public List<SleepRecord> SleepRecords { get; set; } = new();

    public int GetBabyAge()
    {
      if (Dob == null)
      {
        return -1;
      }
      var today = DateTime.Today;
      var age = today.Year - Dob.Value.Year;
      if (Dob.Value.Date > today.AddMonths(-age * 12)) age--;
      // if (Dob.Value.Date > today.AddYears(-age)) age--;
      return age;
    }
  }
}
