using Xamarin.Essentials;

namespace WkXamarinToolBox.Services.DeviceInfos
{
    public static class DeviceInfosService
    {
        public static bool HasInternetConnection() =>
            Connectivity.NetworkAccess != NetworkAccess.None;

        private static DisplayInfo displayInfo => DeviceDisplay.MainDisplayInfo;

        public static double ScreenWidthToXamarinWidth() =>
            displayInfo.Width / displayInfo.Density;

        public static double ScreenHeightToXamarinHeight() =>
            displayInfo.Height / displayInfo.Density;

        public static string GetAllDeviceBasicInformation() =>
            $"{DeviceInfo.Platform} {DeviceInfo.DeviceType} | " +
            $"Language:{DeviceInfo.Idiom} | " +
            $"Version:{DeviceInfo.VersionString} | " +
            $"{DeviceInfo.Manufacturer} {DeviceInfo.Model} | " +
            $"{DeviceInfo.Name}";


    }
}
