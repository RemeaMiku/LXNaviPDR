using CommunityToolkit.Maui;
using LXNavi.Views;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Core.Hosting;

namespace LXNavi;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
        .UseMauiApp<App>()
        .UseMauiCommunityToolkit()
        .ConfigureSyncfusionCore()
        .UseMauiMaps()
        .ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
        });
        builder.Services.AddSingleton<SensorsService>();
        builder.Services.AddSingleton<DeadReckoningService>();
        builder.Services.AddSingleton(Geolocation.Default);
        builder.Services.AddSingleton(Connectivity.Current);
        builder.Services.AddSingleton<TrackDisplayViewModel>();
        builder.Services.AddSingleton<SensorsViewModel>();
        builder.Services.AddSingleton<SensorDetailViewModel>();
        builder.Services.AddSingleton<OptionsViewModel>();
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<SensorDetailPage>();
        builder.Services.AddSingleton<TrackDisplayPage>();
        builder.Services.AddSingleton<OptionsPage>();
        builder.ConfigureMauiHandlers(handlers =>
        {
#if ANDROID
            handlers.AddHandler<Microsoft.Maui.Controls.Maps.Map, CustomMapHandler>();
#endif
        });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
