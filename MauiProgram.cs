using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Maps.Handlers;
#if ANDROID
using CRadventure.Platforms.Android;
#endif

namespace CRadventure
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                 .UseMauiApp<App>()
                 .UseMauiMaps()
                 .ConfigureFonts(fonts =>
                 {
                     fonts.AddFont("KaushanScript-Regular.ttf", "Kaushan");
                     fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                     fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");

                     // Quitamos o dejamos Kaushan si la quieres conservar, 
                     // y registramos las dos variantes de Montserrat:
                     fonts.AddFont("Montserrat-Bold.ttf", "MontserratBold");
                     fonts.AddFont("Montserrat-Regular.ttf", "MontserratRegular");
                 });

#if ANDROID
            MapHandler.Mapper.AppendToMapping("CustomMapStyle", (handler, view) =>
            {
                if (handler is MapHandler mapHandler)
                {
                    MapStyleHelper.ApplyStyle(mapHandler);
                }
            });
#endif

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
