# CribSheet

A cross-platform baby tracking app built with .NET MAUI. Track feedings, diaper changes, sleep sessions, and weight history for one baby or a group of multiples (twins, triplets, quadruplets).

## Features

- **Baby Management** — Add a baby with name, date of birth, and initial weight. Edit details at any time.
- **Multiples Support** — Register twins, triplets, or quadruplets as a linked group. A sibling tab bar appears at the bottom of relevant pages so you can switch between babies without leaving the screen.
- **Feeding Records** — Log breast/bottle feedings with time, amount (ml), type, and duration.
- **Diaper Records** — Record diaper changes with type and optional notes.
- **Sleep Records** — Log sleep sessions with start/end times, computed duration, sleep type, and notes.
- **Weight History** — Record weight updates over time (stored in lbs/oz). An initial weight entry is automatically seeded from the baby's profile on first load.
- **All Records View** — A tabbed page that shows Feeding, Diaper, Sleep, and Weight records in one place, with delete and (stub) edit actions per record.
- **CSV Export** — Export feeding records to a CSV file.

## Tech Stack

| Layer | Technology |
|---|---|
| UI Framework | [.NET MAUI](https://learn.microsoft.com/dotnet/maui/) (.NET 10) |
| MVVM | [CommunityToolkit.Mvvm](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/) 8.4 |
| UI Helpers | [CommunityToolkit.Maui](https://github.com/CommunityToolkit/Maui) 13 |
| Local Database | [sqlite-net-pcl](https://github.com/praeclarum/sqlite-net) 1.9 |
| JSON | Newtonsoft.Json 13 |

## Supported Platforms

| Platform | Minimum Version |
|---|---|
| Android | API 21 (Android 5.0) |
| iOS | 15.0 |
| macOS (Mac Catalyst) | 15.0 |
| Windows | 10.0.17763 (1809) |

## Project Structure

```
CribSheet/
├── App.xaml / App.xaml.cs          # Application entry point
├── AppShell.xaml / .cs             # Shell navigation routes
├── MauiProgram.cs                  # DI container setup
├── Constants.cs                    # Database path constant
│
├── Data/
│   └── CribSheetDatabase.cs        # SQLite async wrapper; all DB operations
│
├── Models/
│   ├── Baby.cs                     # Core baby entity (Name, DOB, Weight, IsAMultiple, GroupId)
│   ├── BabyGroup.cs                # Links multiples to a shared group ID
│   ├── BabyFormEntry.cs            # UI-only ObservableObject for the Add Baby form
│   ├── FeedingRecord.cs            # Feeding log entry
│   ├── PottyRecord.cs              # Diaper change log entry
│   ├── SleepRecord.cs              # Sleep session log entry
│   ├── WeightRecord.cs             # Historical weight entry (stored in total ounces)
│   ├── MedicalRecord.cs            # (Future use)
│   └── MilestoneRecord.cs          # (Future use)
│
├── Services/
│   ├── ICurrentBaby.cs             # Interface: tracks the active baby across pages
│   └── CurrentBabyService.cs       # Singleton implementation of ICurrentBaby
│
├── ViewModels/
│   ├── BaseViewModel.cs            # ObservableValidator base; NavigateBack, error helpers
│   ├── HomeViewModel.cs            # Baby list; navigate to add/select baby
│   ├── AddBabyViewModel.cs         # Add one baby or a group of multiples
│   ├── EditBabyViewModel.cs        # Edit existing baby details
│   ├── CurrentBabyViewModel.cs     # Baby dashboard; sibling tabs for multiples
│   ├── AllRecordsViewModel.cs      # Tabbed all-records view; sibling tabs
│   ├── FeedingRecordsViewModel.cs  # Feeding list + CSV export
│   ├── NewFeedingRecordViewModel.cs
│   ├── PottyRecordsViewModel.cs    # Diaper list
│   ├── NewPottyRecordViewModel.cs
│   ├── SleepRecordsViewModel.cs    # Sleep list
│   ├── NewSleepRecordViewModel.cs
│   ├── WeightRecordsViewModel.cs   # Weight history list
│   └── NewWeightRecordViewModel.cs
│
├── Views/
│   ├── HomePage.xaml               # Baby selection list
│   ├── AddBabyPage.xaml            # Add baby (single or multiples)
│   ├── EditBabyPage.xaml
│   ├── CurrentBabyPage.xaml        # Baby dashboard with action buttons
│   ├── AllRecordsPage.xaml         # Tabbed all-records page
│   ├── FeedingRecordsPage.xaml
│   ├── NewFeedingRecordPage.xaml
│   ├── PottyRecordsPage.xaml
│   ├── NewPottyRecordPage.xaml
│   ├── SleepRecordsPage.xaml
│   ├── NewSleepRecordPage.xaml
│   ├── WeightRecordsPage.xaml
│   └── NewWeightRecordPage.xaml
│
├── Converters/
│   └── OzToLbsOzConverter.cs       # Converts total ounces to a lbs/oz display string
│
└── Resources/
    ├── AppIcon/
    ├── Fonts/
    ├── Images/                     # SVG/PNG icons (edit, delete, scale, crib, etc.)
    ├── Raw/
    ├── Splash/
    └── Styles/
        ├── Colors.xaml
        └── Styles.xaml
```

## Architecture

The app follows the **MVVM** pattern throughout:

- **Models** are plain C# classes decorated with SQLite-net attributes (`[PrimaryKey]`, `[AutoIncrement]`, `[Indexed]`, `[Ignore]`).
- **ViewModels** inherit from `BaseViewModel` (which extends `ObservableValidator`) and use `[ObservableProperty]` and `[RelayCommand]` source generators from CommunityToolkit.Mvvm.
- **Views** use compiled XAML bindings (`x:DataType`) for type safety and performance.
- **Shell navigation** (`Shell.Current.GoToAsync`) is used for all page transitions. Query parameters pass refresh signals between pages.
- **`ICurrentBaby`** is a singleton service injected into ViewModels to share the currently selected baby across pages without coupling ViewModels together.
- **`CribSheetDatabase`** is a single async SQLite connection wrapper. Tables are created/migrated via `CreateTablesAsync` on first access; SQLite-net automatically adds new columns to existing tables.

### Multiples / Sibling Tab Bar

When a group of multiples is registered, each `Baby` record stores `IsAMultiple = true` and a shared `GroupId` pointing to a `BabyGroup` row. On pages that support sibling switching (`CurrentBabyPage`, `AllRecordsPage`), a horizontal `CollectionView` tab bar is pinned to the bottom of the screen. Selecting a sibling tab updates `ICurrentBaby` and reloads the page data for that baby. A `_suppressGroupBabyChange` guard flag prevents re-entrant reload loops during initial setup.

### Weight Migration

On the first load after the weight-history feature was introduced, `MigrateExistingWeightAsync` checks whether a baby already has weight records. If not, it seeds a `WeightRecord` from the baby's stored `Weight` value and their `CreatedAt` date, preserving the original data.

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (17.12+) with the **.NET MAUI** workload, **or** the MAUI workload installed via `dotnet workload install maui`
- For Android: Android SDK (installed automatically by Visual Studio or `dotnet workload`)
- For iOS/macOS: a Mac with Xcode 16+

### Build and Run

```bash
# Clone the repository
git clone <repo-url>
cd CribSheet

# Restore packages
dotnet restore CribSheet.slnx

# Run on Android emulator
dotnet build CribSheet/CribSheet.csproj -f net10.0-android

# Run on Windows
dotnet run --project CribSheet/CribSheet.csproj -f net10.0-windows10.0.19041.0
```

Or open `CribSheet.slnx` in Visual Studio, select your target platform/device from the toolbar, and press **F5**.

### Database

The SQLite database is stored in the app's local data directory (`FileSystem.AppDataDirectory`). No configuration is required — the database and all tables are created automatically on first launch.
