using CommunityToolkit.Maui;
using CribSheet.Data;
using CribSheet.Services;
using CribSheet.ViewModels;
using CribSheet.Views;
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
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
      // Register Services
      builder.Services.AddSingleton<CribSheetDatabase>();
      builder.Services.AddSingleton<IPinService, PinService>();

      // Register ViewModels
      builder.Services.AddTransient<LoginViewModel>();
      builder.Services.AddTransient<SetPinViewModel>();
      builder.Services.AddTransient<HomeViewModel>();
      builder.Services.AddTransient<BabyViewModel>();

      // Register Views
      builder.Services.AddTransient<LoginPage>();
      builder.Services.AddTransient<SetPinPage>();
      builder.Services.AddTransient<HomePage>();
      builder.Services.AddTransient<AddBabyPage>();
      builder.Services.AddTransient<CurrentBabyPage>();






#if DEBUG
      builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
