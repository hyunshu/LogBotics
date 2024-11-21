
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting; 
#if WINDOWS
	using FRC_App.Platforms.Windows; 
#endif


namespace FRC_App;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
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
