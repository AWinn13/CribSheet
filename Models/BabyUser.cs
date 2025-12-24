using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace CribSheet.Models
{
  [Table("baby_user")]
  public class BabyUser 
  {
    [PrimaryKey]
    [Column("id")]
    public long Id { get; set; }

    [Column("baby_id")]
    public long BabyId { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; } // auth.users.id is uuid


    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation

    public Baby? Baby { get; set; }

    // Note: if you have a User model (auth.users) you can add a Reference to it
    // [Reference(typeof(User))]
    // public User? User { get; set; }
  }
}
