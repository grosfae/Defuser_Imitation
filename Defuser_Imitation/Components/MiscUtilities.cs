using Defuser_Imitation.Properties;
using System.Management;
using System.Windows.Media;

namespace Defuser_Imitation.Components
{
    public class MiscUtilities
    {
        public static Dictionary<string, MediaPlayer> soundPlayers = [];

        public static void PreloadSound(string key, string filePath)
        {
            MediaPlayer player = new();
            player.Open(new Uri(filePath));
            soundPlayers[key] = player;
        }
        public static void DownloadSounds()
        {
            PreloadSound("plant_stage_sound", Environment.CurrentDirectory + @"/Resources/Sounds/plant_stage_sound.mp3");
            PreloadSound("plant_stage_last_seconds_sound", Environment.CurrentDirectory + @"/Resources/Sounds/plant_stage_last_seconds_sound.mp3");
            PreloadSound("active_device_sound", Environment.CurrentDirectory + @"/Resources/Sounds/active_device_sound.mp3");
            PreloadSound("active_device_last_seconds_sound", Environment.CurrentDirectory + @"/Resources/Sounds/active_device_last_seconds_sound.mp3");
            PreloadSound("round_finish_sound", Environment.CurrentDirectory + @"/Resources/Sounds/round_finish_sound.mp3");
        }
        public static void ResetSoundPosition()
        {
            foreach (var player in soundPlayers.Values)
            {
                player.Position = TimeSpan.Zero;
            }
        }
        public static string DeviceCode()
        {
            string deviceCode = string.Empty;
            switch (Settings.Default.DeviceTypeCode)
            {
                case 0:
                    Random random = new();
                    for (int i = 0; i < 8; i++)
                    {
                        deviceCode += random.Next(0,10);
                    }
                    break;
                case 1:
                    deviceCode = Settings.Default.DeviceCode;
                    break;
            }
            return deviceCode;
        }
        public static bool CheckUSBDevice(string UsbDeviceName)
        {
            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_PnPEntity"))
            collection = searcher.Get();
            foreach (var device in collection)
            {
                if ((string)device.GetPropertyValue("Name") == UsbDeviceName)
                {
                    return true;
                }
            }
            collection.Dispose();
            return false;
        }
    }
}
