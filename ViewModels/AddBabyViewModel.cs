using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CribSheet.Data;
using CribSheet.Models;
using CribSheet.Services;
using System.Collections.ObjectModel;

namespace CribSheet.ViewModels
{
  public partial class BabyViewModel : BaseViewModel
  {
    #region Fields

    private readonly CribSheetDatabase _database;

    #endregion

    #region Constructor

    public BabyViewModel(CribSheetDatabase database, ICurrentBaby currentBabyService)
      : base(currentBabyService)
    {
      _database = database;
      BabyEntries = new ObservableCollection<BabyFormEntry>
      {
        new BabyFormEntry { Label = "Baby" }
      };
    }

    #endregion

    #region Properties

    [ObservableProperty]
    private DateTime birthDate = DateTime.Today;

    [ObservableProperty]
    private bool isMultiple;

    [ObservableProperty]
    private string? selectedMultipleCount;

    public List<string> MultipleCountOptions { get; } = new()
    {
      "Twins",
      "Triplets",
      "Quadruplets"
    };

    [ObservableProperty]
    private ObservableCollection<BabyFormEntry> babyEntries;

    #endregion

    #region Property Change Handlers

    partial void OnIsMultipleChanged(bool value)
    {
      if (value)
      {
        SelectedMultipleCount = MultipleCountOptions[0];
        UpdateBabyEntries(2);
      }
      else
      {
        SelectedMultipleCount = null;
        UpdateBabyEntries(1);
      }
    }

    partial void OnSelectedMultipleCountChanged(string? value)
    {
      if (value == null) return;
      UpdateBabyEntries(GetCountFromOption(value));
    }

    #endregion

    #region Commands

    [RelayCommand]
    private async Task AddBaby()
    {
      if (!ValidateForm())
        return;

      try
      {
        if (IsMultiple)
        {
          var group = new BabyGroup { CreatedAt = DateTime.Now };
          await _database.AddBabyGroupAsync(group);

          foreach (var entry in BabyEntries)
          {
            var baby = CreateBabyFromEntry(entry, isAMultiple: true, groupId: group.GroupId);
            await _database.AddBabyAsync(baby);
          }
        }
        else
        {
          var baby = CreateBabyFromEntry(BabyEntries[0], isAMultiple: false, groupId: null);
          await _database.AddBabyAsync(baby);
        }

        await NavigateToHomePage();
      }
      catch (Exception ex)
      {
        await Shell.Current.DisplayAlertAsync("Error",
          $"Failed to add baby: {ex.Message}", "OK");
      }
    }

    #endregion

    #region Validation

    partial void OnBirthDateChanged(DateTime value)
    {
      ValidateBirthDate(value);
    }

    private bool ValidateForm()
    {
      ClearErrors("BabyForm");
      ValidateBirthDate(BirthDate);

      foreach (var entry in BabyEntries)
      {
        if (string.IsNullOrWhiteSpace(entry.Name))
        {
          AddError("BabyForm", $"{entry.Label} name cannot be empty.");
        }

        double totalWeight = (entry.Lbs * 16) + entry.Oz;
        if (totalWeight <= 0)
        {
          AddError("BabyForm", $"{entry.Label} weight must be greater than zero.");
        }
      }

      return !HasErrors;
    }

    private void ValidateBirthDate(DateTime value)
    {
      if (value >= DateTime.Now)
      {
        AddError(nameof(BirthDate), "Birthdate cannot be in the future.");
      }
      else
      {
        ClearErrors(nameof(BirthDate));
      }
    }

    #endregion

    #region Private Methods

    private Baby CreateBabyFromEntry(BabyFormEntry entry, bool isAMultiple, long? groupId)
    {
      return new Baby
      {
        CreatedAt = DateTime.Now,
        Dob = BirthDate,
        Name = entry.Name,
        Weight = (long)((entry.Lbs * 16) + entry.Oz),
        IsAMultiple = isAMultiple,
        GroupId = groupId
      };
    }

    private void UpdateBabyEntries(int count)
    {
      while (BabyEntries.Count < count)
      {
        BabyEntries.Add(new BabyFormEntry { Label = $"Baby {BabyEntries.Count + 1}" });
      }

      while (BabyEntries.Count > count)
      {
        BabyEntries.RemoveAt(BabyEntries.Count - 1);
      }

      // Re-label all entries so they stay consistent after removal
      for (int i = 0; i < BabyEntries.Count; i++)
      {
        BabyEntries[i].Label = count == 1 ? "Baby" : $"Baby {i + 1}";
      }
    }

    private static int GetCountFromOption(string option) => option switch
    {
      "Twins" => 2,
      "Triplets" => 3,
      "Quadruplets" => 4,
      _ => 2
    };

    private async Task NavigateToHomePage()
    {
      await Shell.Current.GoToAsync("//HomePage");
    }

    #endregion
  }
}
