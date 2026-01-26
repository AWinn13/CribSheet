using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CribSheet.Data;
using CribSheet.Models;
using System.Collections.ObjectModel;

namespace CribSheet.ViewModels
{
  public partial class NewPottyRecordViewModel : BaseViewModel, IQueryAttributable
  {
    #region Fields

    private readonly CribSheetDatabase _database;
    private long _babyId;

    #endregion

    #region Constructor

    public NewPottyRecordViewModel(CribSheetDatabase database)
    {
      _database = database;
      InitializeDefaults();
    }

    #endregion

    #region Properties

    public ObservableCollection<string> DiaperTypes { get; private set; } = new();

    [ObservableProperty]
    private string? selectedDiaperType;

    [ObservableProperty]
    private DateTime pottyDate;

    [ObservableProperty]
    private TimeSpan pottyTime;

    [ObservableProperty]
    private string? notes;

    #endregion

    #region Navigation

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
      if (query.ContainsKey("BabyId"))
      {
        _babyId = (long)query["BabyId"];
      }
    }

    #endregion

    #region Commands

    [RelayCommand]
    private async Task SavePotty()
    {
      if (!ValidateForm())
        return;

      try
      {
        var pottyRecord = CreatePottyRecord();
        await _database.AddPottyRecordAsync(pottyRecord);
        await NavigateBack();
      }
      catch (Exception ex)
      {
        await Shell.Current.DisplayAlertAsync("Error",
          $"Failed to save diaper record: {ex.Message}", "OK");
      }
    }

    #endregion

    #region Private Methods

    private void InitializeDefaults()
    {
      PottyDate = DateTime.Now.Date;
      PottyTime = DateTime.Now.TimeOfDay;

      DiaperTypes = new ObservableCollection<string>(
        Enum.GetNames(typeof(DiaperType))
      );

      SelectedDiaperType = DiaperTypes.FirstOrDefault();
    }

    private bool ValidateForm()
    {
      if (string.IsNullOrWhiteSpace(SelectedDiaperType))
      {
        Shell.Current.DisplayAlertAsync("Error", "Please select a diaper type", "OK");
        return false;
      }

      return true;
    }

    private PottyRecord CreatePottyRecord()
    {
      return new PottyRecord
      {
        BabyId = _babyId,
        Time = PottyDate.Add(PottyTime),
        Type = Enum.Parse<DiaperType>(SelectedDiaperType),
        Notes = Notes
      };
    }

    #endregion
  }
}
