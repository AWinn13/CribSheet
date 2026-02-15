using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CribSheet.Data;
using CribSheet.Models;
using CribSheet.Services;
using System.Collections.ObjectModel;

namespace CribSheet.ViewModels
{
  public partial class NewFeedingRecordViewModel : BaseViewModel
  {
    #region Fields

    private readonly CribSheetDatabase _database;

    #endregion

    #region Constructor

    public NewFeedingRecordViewModel(CribSheetDatabase database, ICurrentBaby currentBabyService)
      : base(currentBabyService)
    {
      _database = database;
      InitializeDefaults();
    }

    #endregion

    #region Properties

    public ObservableCollection<string> FeedingTypes { get; private set; } = new();

    [ObservableProperty]
    private string? selectedFeedingType;

    [ObservableProperty]
    private DateTime feedingDate;

    [ObservableProperty]
    private TimeSpan feedingTime;

    [ObservableProperty]
    private string? amountMl;

    [ObservableProperty]
    private string? durationMinutes;

    [ObservableProperty]
    private string? notes;

    #endregion

    #region Commands

    [RelayCommand]
    private async Task SaveFeeding()
    {
      if (!ValidateForm())
        return;

      try
      {
        var feedingRecord = CreateFeedingRecord();
        await _database.AddFeedingRecordAsync(feedingRecord);
        await NavigateBack();
      }
      catch (Exception ex)
      {
        await Shell.Current.DisplayAlertAsync("Error",
          $"Failed to save feeding record: {ex.Message}", "OK");
      }
    }

    #endregion

    #region Private Methods

    private void InitializeDefaults()
    {
      FeedingDate = DateTime.Now.Date;
      FeedingTime = DateTime.Now.TimeOfDay;

      FeedingTypes = new ObservableCollection<string>(
        Enum.GetNames(typeof(FeedingType))
      );

      SelectedFeedingType = FeedingTypes.FirstOrDefault();
    }

    private bool ValidateForm()
    {
      if (string.IsNullOrWhiteSpace(SelectedFeedingType))
      {
        Shell.Current.DisplayAlertAsync("Error", "Please select a feeding type", "OK");
        return false;
      }

      return true;
    }

    private FeedingRecord CreateFeedingRecord()
    {
      return new FeedingRecord
      {
        BabyId = CurrentBabyService.BabyId,
        Time = FeedingDate.Add(FeedingTime),
        Type = Enum.Parse<FeedingType>(SelectedFeedingType),
        AmountMl = string.IsNullOrWhiteSpace(AmountMl) ? null : int.Parse(AmountMl),
        DurationMinutes = string.IsNullOrWhiteSpace(DurationMinutes) ? null : int.Parse(DurationMinutes),
        Notes = Notes
      };
    }

    #endregion
  }
}
