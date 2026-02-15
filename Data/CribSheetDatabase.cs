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
      await database.CreateTablesAsync<Baby, SleepRecord, FeedingRecord, PottyRecord>();
    }
    public async Task<int> AddBabyAsync(Baby baby)
    {
      await Init();
      return await database.InsertAsync(baby);
    }

    public async Task<Baby> GetBabyAsync(long babyId)
    {
      await Init();
      return await database.GetAsync<Baby>(babyId);
    }

    public async Task<int> UpdateBabyAsync(Baby baby)
    {
      await Init();
      return await database.UpdateAsync(baby);
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

    public async Task<bool> FeedingRecordsExist(long babyId)
    {
           await Init();
      var count = await database.Table<FeedingRecord>().Where(fr => fr.BabyId == babyId).CountAsync();
      return count > 0;

    }

    public async Task<bool> SleepingRecordsExist(long babyId)
    {
      await Init();
      var count = await database.Table<SleepRecord>().Where(fr => fr.BabyId == babyId).CountAsync();
      return count > 0;
    }

    public async Task<bool> PottyRecordsExist(long babyId)
    {
      await Init();
      var count = await database.Table<PottyRecord>().Where(fr => fr.BabyId == babyId).CountAsync();
      return count > 0;
    }

    public async Task<List<SleepRecord>> GetSleepRecordsAsync(long babyId)
    {
      await Init();
      return await database.Table<SleepRecord>()
          .Where(fr => fr.BabyId == babyId)
          .ToListAsync();
    }

    public async Task<int> AddFeedingRecordAsync(FeedingRecord feedingRecord)
    {
      await Init();
      return await database.InsertAsync(feedingRecord);
    }

    public async Task<int> DeleteFeedingRecordAsync(FeedingRecord feedingRecord)
    {
      await Init();
      return await database.DeleteAsync(feedingRecord);
    }

    public async Task<List<PottyRecord>> GetPottyRecordsAsync(long babyId)
    {
      await Init();
      return await database.Table<PottyRecord>()
          .Where(pr => pr.BabyId == babyId)
          .ToListAsync();
    }

    public async Task<int> AddPottyRecordAsync(PottyRecord pottyRecord)
    {
      await Init();
      return await database.InsertAsync(pottyRecord);
    }

    public async Task<int> AddSleepRecordAsync(SleepRecord sleepRecord)
    {
      await Init();
      return await database.InsertAsync(sleepRecord);
    }

    public async Task<int> DeleteSleepRecordAsync(SleepRecord sleepRecord)
    {
      await Init();
      return await database.DeleteAsync(sleepRecord);
    }

    public async Task<int> DeletePottyRecordAsync(PottyRecord pottyRecord)
    {
      await Init();
      return await database.DeleteAsync(pottyRecord);
    }
  }
}