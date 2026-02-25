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

    public long Weight { get; set; }

    public DateTime? Dob { get; set; } // date only in DB; DateTime works fine

    public bool IsAMultiple { get; set; }

    public long? GroupId { get; set; }

    [Ignore]
    public int? Age { get => GetBabyAge(); }


    public int GetBabyAge()
    {
      var today =  DateTime.Today;

      int months = (today.Year - Dob.Value.Year) * 12
                 + today.Month - Dob.Value.Month;

      // If we haven't reached the birth day this month yet, subtract one month
      if (today.Day < Dob.Value.Day)
        months--;

      return Math.Max(months, 0);
    }
  }
}
