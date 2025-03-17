using CommunityToolkit.Maui;
using MauiApp_IA_IOT.ViewModels;
using MauiApp_IA_IOT.Views;
using Microsoft.Extensions.Logging;

namespace MauiApp_IA_IOT
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<ChatViewModel>();

            builder.Services.AddSingleton<ChatPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
