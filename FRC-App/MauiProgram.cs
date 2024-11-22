
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using SkiaSharp.Views.Maui.Controls.Hosting; 
#if WINDOWS
	using FRC_App.Platforms.Windows; 
#endif

using FRC_App.Models;
using FRC_App.Services;

namespace FRC_App;

public static class MauiProgram
{
	public static void Main()
    {
    }

	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureLifecycleEvents(events =>
            {
				#if WINDOWS
                events.AddWindows(windows => windows.OnPlatformMessage(async (window, args) =>
                       {
                           if (args.MessageId == Convert.ToUInt32("0010", 16))
                           {
                               // Window close request
							   User currentUser = UserSession.CurrentUser;
								if (currentUser != null) {
									await UserDatabase.logout(currentUser);
								}
								System.Threading.Thread.Sleep(1000);
                           }
                       }));
				#endif
            })
			.UseSkiaSharp()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

			#if WINDOWS
				builder.Services.AddSingleton<IFilePicker, WindowsFilePicker>();
				Console.WriteLine("WindowsFilePicker is registered!");
			#endif

			#if DEBUG
				builder.Logging.AddDebug();
			#endif

		return builder.Build();
	}
}
