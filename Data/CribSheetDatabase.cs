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
      Type[] dbtables = [typeof(BabyGroup), typeof(Baby), typeof(FeedingRecord), typeof(SleepRecord), typeof(PottyRecord), typeof(WeightRecord)];
      await database.CreateTablesAsync(CreateFlags.None, dbtables);
    }
    public async Task<BabyGroup> AddBabyGroupAsync(BabyGroup group)
    {
      await Init();
      await database.InsertAsync(group); // populates group.GroupId after insert
      return group;
    }

    public async Task<List<Baby>> GetBabiesByGroupAsync(long groupId)
    {
      await Init();
      return await database.Table<Baby>().Where(b => b.GroupId == groupId).ToListAsync();
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

    public async Task<int> DeleteBabyAsync(Baby baby)
    {
      await Init();
      return await database.DeleteAsync(baby);
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

    public async Task<List<WeightRecord>> GetWeightRecordsAsync(long babyId)
    {
      await Init();
      return await database.Table<WeightRecord>()
          .Where(wr => wr.BabyId == babyId)
          .OrderByDescending(wr => wr.RecordedAt)
          .ToListAsync();
    }

    public async Task<bool> WeightRecordsExist(long babyId)
    {
      await Init();
      var count = await database.Table<WeightRecord>().Where(wr => wr.BabyId == babyId).CountAsync();
      return count > 0;
    }

    public async Task<int> AddWeightRecordAsync(WeightRecord weightRecord)
    {
      await Init();
      return await database.InsertAsync(weightRecord);
    }

    public async Task<int> DeleteWeightRecordAsync(WeightRecord weightRecord)
    {
      await Init();
      return await database.DeleteAsync(weightRecord);
    }

    /// <summary>
    /// If a baby has a weight set but no WeightRecords yet, seeds the first record
    /// from Baby.Weight so history is preserved going forward.
    /// </summary>
    public async Task MigrateExistingWeightAsync(Baby baby)
    {
      await Init();
      if (baby.Weight <= 0) return;

      bool hasRecords = await WeightRecordsExist(baby.BabyId);
      if (hasRecords) return;

      var seedRecord = new WeightRecord
      {
        BabyId = baby.BabyId,
        RecordedAt = baby.CreatedAt,
        WeightOz = baby.Weight,
        Notes = "Migrated from initial baby record"
      };

      await database.InsertAsync(seedRecord);
    }
  }
}