using System;
using System.Collections.Generic;
using System.Text;
using CribSheet.Models;
using SQLite;

namespace CribSheet.Data
{
  public class CribSheetDatabase
  {
    SQLiteAsyncConnection database;

    async Task Init()
    {
      if (database is not null)
      {
        return;
      }
       
      database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
      await database.CreateTablesAsync<Baby, SleepRecord, FeedingRecord>();
    }
    public async Task AddBabyAsync(Baby baby)
    {
      await Init();
      await database.InsertAsync(baby);
    }

    public async Task<List<Baby>> GetBabiesAsync()
    {
      await Init();
      return await database.Table<Baby>().ToListAsync();
    }

    public async Task<List<FeedingRecord>> GetFeedingRecordsAsync(long babyId)
    {
      await Init();
      return await database.Table<FeedingRecord>()
          .Where(fr => fr.BabyId == babyId)
          .ToListAsync();
    }

    public async Task<List<SleepRecord>> GetSleepRecordsAsync(long babyId)
    {
      await Init();
      return await database.Table<SleepRecord>()
          .Where(fr => fr.BabyId == babyId)
          .ToListAsync();
    }
  }
}