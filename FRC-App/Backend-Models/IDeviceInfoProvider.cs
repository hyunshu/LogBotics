namespace FRC_App.Services
{
    public interface IDeviceInfoProvider
    {
        string Name { get; }
        string Platform { get; }
        string VersionString { get; }
    }
}
