using CommunityToolkit.Maui;
using CribSheet.Data;
using CribSheet.ViewModels;
using CribSheet.Views;
using CribSheet.Services;
using Microsoft.Extensions.Logging;

namespace CribSheet
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    // Lato Font Family
                    fonts.AddFont("Lato-Regular.ttf", "LatoRegular");
                    fonts.AddFont("Lato-Bold.ttf", "LatoBold");
                    fonts.AddFont("Lato-Italic.ttf", "LatoItalic");
                    fonts.AddFont("Lato-BoldItalic.ttf", "LatoBoldItalic");
                    fonts.AddFont("Lato-Light.ttf", "LatoLight");
                    fonts.AddFont("Lato-LightItalic.ttf", "LatoLightItalic");
                    fonts.AddFont("Lato-Thin.ttf", "LatoThin");
                    fonts.AddFont("Lato-ThinItalic.ttf", "LatoThinItalic");
                    fonts.AddFont("Lato-Black.ttf", "LatoBlack");
                    fonts.AddFont("Lato-BlackItalic.ttf", "LatoBlackItalic");

                    // Keep OpenSans for backward compatibility
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
      // Register Services
      builder.Services.AddSingleton<CribSheetDatabase>();
      builder.Services.AddSingleton<ICurrentBaby, CurrentBaby>();


      // Register ViewModels
      builder.Services.AddTransient<HomeViewModel>();
      builder.Services.AddTransient<BabyViewModel>();
      builder.Services.AddTransient<CurrentBabyViewModel>();
      builder.Services.AddTransient<EditBabyViewModel>();
      builder.Services.AddTransient<NewFeedingRecordViewModel>();
      builder.Services.AddTransient<NewPottyRecordViewModel>();
      builder.Services.AddTransient<NewSleepRecordViewModel>();
      builder.Services.AddTransient<FeedingRecordsViewModel>();
      builder.Services.AddTransient<SleepRecordsViewModel>();
      builder.Services.AddTransient<PottyRecordsViewModel>();

      // Register Views
      builder.Services.AddTransient<HomePage>();
      builder.Services.AddTransient<AddBabyPage>();
      builder.Services.AddTransient<CurrentBabyPage>();
      builder.Services.AddTransient<EditBabyPage>();
      builder.Services.AddTransient<NewFeedingRecordPage>();
      builder.Services.AddTransient<NewPottyRecordPage>();
      builder.Services.AddTransient<NewSleepRecordPage>();
      builder.Services.AddTransient<FeedingRecordsPage>();
      builder.Services.AddTransient<SleepRecordsPage>();
      builder.Services.AddTransient<PottyRecordsPage>();





#if DEBUG
      builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
