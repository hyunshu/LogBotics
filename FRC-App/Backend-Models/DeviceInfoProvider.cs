using Microsoft.Maui.Devices;

namespace FRC_App.Services
{
    public class DeviceInfoProvider : IDeviceInfoProvider
    {
        public string Name => DeviceInfo.Name;
        public string Platform => DeviceInfo.Platform.ToString();
        public string VersionString => DeviceInfo.VersionString;
    }
}
