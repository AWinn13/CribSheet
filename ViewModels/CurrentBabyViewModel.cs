using CommunityToolkit.Mvvm.ComponentModel;
using CribSheet.Data;
using CribSheet.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CribSheet.ViewModels
{
  public class CurrentBabyViewModel : ObservableObject, IQueryAttributable, INotifyPropertyChanged
  {
    CribSheetDatabase _database;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="db"></param>
    public CurrentBabyViewModel(CribSheetDatabase db)
    {
       _database = db;
    }

    public Baby? CurrentBaby { get; private set; }
    public string Name => CurrentBaby?.Name ?? "Unknown Baby";

    public long BabyId => CurrentBaby?.BabyId ?? 0;

    public DateTime? Dob => CurrentBaby?.Dob;
    public ObservableCollection<FeedingRecord>? FeedingRecords { get; set; }
    public ObservableCollection<SleepRecord>? SleepRecords { get; set; }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
       CurrentBaby = (Baby)query["Baby"];
      OnPropertyChanged(nameof(CurrentBaby));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual async void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null)
        handler(this, new PropertyChangedEventArgs(propertyName));
      if (propertyName == nameof(CurrentBaby))
      {
        // Load related data when CurrentBaby changes
        // For example, load FeedingRecords and SleepRecords for the CurrentBaby
        FeedingRecords = new ObservableCollection<FeedingRecord>(await _database.GetFeedingRecordsAsync(CurrentBaby.BabyId));
        SleepRecords = new ObservableCollection<SleepRecord>(await _database.GetSleepRecordsAsync(CurrentBaby.BabyId));
      }

      }
    }
}
