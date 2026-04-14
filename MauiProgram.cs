using Microsoft.Extensions.Logging;
using Microsoft.Maui.Networking;
using Lab1.Pages;
using Lab1.Services;
using Lab1.Services.Http;

namespace Lab1
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>() // класс запускаемый при старте прилож
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddTransient<IDbService, SQLiteService>();
            builder.Services.AddTransient<SQLitePage>();

            builder.Services.AddTransient<IRateService, RateService>();
            builder.Services.AddTransient<ConverterPage>();
            builder.Services.AddHttpClient("NB RB", opt => opt.BaseAddress = new Uri("https://www.nbrb.by/api/exrates/rates"));
            builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);


#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build(); 
        }
    }
}
