using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CribSheet.Data;
using CribSheet.Models;
using CribSheet.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace CribSheet.ViewModels
{
  public partial class NewSleepRecordViewModel : BaseViewModel
  {
    #region Fields

    private readonly CribSheetDatabase _database;
    private DateTime? _timerStartTime;
    private Stopwatch _stopwatch;
    private IDispatcherTimer? _timer;

    #endregion

    #region Constructor

    public NewSleepRecordViewModel(CribSheetDatabase database, ICurrentBaby currentBabyService)
      : base(currentBabyService)
    {
      _database = database;
      _stopwatch = new Stopwatch();
      InitializeDefaults();
    }

    #endregion

    #region Properties

    public ObservableCollection<string> SleepTypes { get; private set; } = new();
    public ObservableCollection<string> WakeReasons { get; private set; } = new();

    [ObservableProperty]
    private string? selectedSleepType;

    [ObservableProperty]
    private string? selectedWakeReason;

    [ObservableProperty]
    private DateTime startDate;

    [ObservableProperty]
    private TimeSpan startTime;

    [ObservableProperty]
    private DateTime endDate;

    [ObservableProperty]
    private TimeSpan endTime;

    [ObservableProperty]
    private string? notes;

    [ObservableProperty]
    private string elapsedTime = "00:00:00";

    [ObservableProperty]
    private bool isTimerRunning;

    [ObservableProperty]
    private string timerStatus = "Timer not started";

    public bool CanStartTimer => !IsTimerRunning;
    public bool CanEditTimes => !IsTimerRunning;

    #endregion

    #region Commands

    [RelayCommand]
    private void StartTimer()
    {
      _timerStartTime = DateTime.Now;
      StartDate = _timerStartTime.Value.Date;
      StartTime = _timerStartTime.Value.TimeOfDay;

      _stopwatch.Restart();
      IsTimerRunning = true;
      TimerStatus = "Timer running...";

      // Create and start the dispatcher timer
      _timer = Application.Current?.Dispatcher.CreateTimer();
      if (_timer != null)
      {
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += OnTimerTick;
        _timer.Start();
      }

      OnPropertyChanged(nameof(CanStartTimer));
      OnPropertyChanged(nameof(CanEditTimes));
    }

    [RelayCommand]
    private void StopTimer()
    {
      if (_timer != null)
      {
        _timer.Stop();
        _timer.Tick -= OnTimerTick;
        _timer = null;
      }

      _stopwatch.Stop();
      IsTimerRunning = false;

      if (_timerStartTime.HasValue)
      {
        var endDateTime = DateTime.Now;
        EndDate = endDateTime.Date;
        EndTime = endDateTime.TimeOfDay;
        TimerStatus = $"Sleep duration: {ElapsedTime}";
      }

      OnPropertyChanged(nameof(CanStartTimer));
      OnPropertyChanged(nameof(CanEditTimes));
    }

    [RelayCommand]
    private void ResetTimer()
    {
      if (IsTimerRunning)
      {
        StopTimer();
      }
      _stopwatch.Reset();
      ElapsedTime = "00:00:00";
      TimerStatus = "Timer not started";
      _timerStartTime = null;
      OnPropertyChanged(nameof(CanStartTimer));
      OnPropertyChanged(nameof(CanEditTimes));
    }

    [RelayCommand]
    private async Task SaveSleep()
    {
      if (!ValidateForm())
        return;

      // Stop timer if still running
      if (IsTimerRunning)
      {
        StopTimer();
      }

      try
      {
        var sleepRecord = CreateSleepRecord();
        await _database.AddSleepRecordAsync(sleepRecord);
        await NavigateBack();
      }
      catch (Exception ex)
      {
        await Shell.Current.DisplayAlertAsync("Error",
          $"Failed to save sleep record: {ex.Message}", "OK");
      }
    }

    #endregion

    #region Private Methods

    private void InitializeDefaults()
    {
      var now = DateTime.Now;
      StartDate = now.Date;
      StartTime = now.TimeOfDay;
      EndDate = now.Date;
      EndTime = now.TimeOfDay;

      SleepTypes = new ObservableCollection<string>(
        Enum.GetNames(typeof(SleepType))
      );

      WakeReasons = new ObservableCollection<string>(
        Enum.GetNames(typeof(WakeReason))
      );

      SelectedSleepType = SleepTypes.FirstOrDefault();
      SelectedWakeReason = WakeReasons.FirstOrDefault();
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
      var elapsed = _stopwatch.Elapsed;
      ElapsedTime = $"{elapsed.Hours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}";
    }

    private bool ValidateForm()
    {
      if (string.IsNullOrWhiteSpace(SelectedSleepType))
      {
        Shell.Current.DisplayAlertAsync("Error", "Please select a sleep type", "OK");
        return false;
      }

      var startDateTime = StartDate.Add(StartTime);
      var endDateTime = EndDate.Add(EndTime);

      if (endDateTime < startDateTime)
      {
        Shell.Current.DisplayAlertAsync("Error", "End time cannot be before start time", "OK");
        return false;
      }

      return true;
    }

    private SleepRecord CreateSleepRecord()
    {
      var startDateTime = StartDate.Add(StartTime);
      var endDateTime = EndDate.Add(EndTime);

      return new SleepRecord
      {
        BabyId = CurrentBabyService.BabyId,
        StartTime = startDateTime,
        EndTime = endDateTime > startDateTime ? endDateTime : (DateTime?)null,
        TypeOfSleep = Enum.Parse<SleepType>(SelectedSleepType ?? "Nap"),
        WhyNoSleep = string.IsNullOrWhiteSpace(SelectedWakeReason)
          ? WakeReason.Other
          : Enum.Parse<WakeReason>(SelectedWakeReason),
        Notes = Notes
      };
    }

    #endregion
  }
}
