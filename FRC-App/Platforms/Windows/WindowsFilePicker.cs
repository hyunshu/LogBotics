using Microsoft.Maui.Controls.Platform;
using Windows.Storage.Pickers;
using FRC_App;

public interface IFilePicker
{
    Task<List<string>> PickFilesAsync();
}

namespace FRC_App.Platforms.Windows
{
public class WindowsFilePicker : IFilePicker
{
    public async Task<List<string>> PickFilesAsync()
    {
        var picker = new FileOpenPicker
        {
            ViewMode = PickerViewMode.Thumbnail,
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary
        };

        // Add file type filters
        picker.FileTypeFilter.Add(".csv"); // Add specific extensions if needed

        // Initialize the picker with a valid window handle
        var hwnd = ((MauiWinUIWindow)App.Current.Windows[0].Handler.PlatformView).WindowHandle;
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

        // Pick files
        var files = await picker.PickMultipleFilesAsync();

        return files.Select(file => file.Path).ToList();
    }
}
}
