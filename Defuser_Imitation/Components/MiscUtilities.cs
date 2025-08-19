using Defuser_Imitation.Properties;
using System.Management;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Defuser_Imitation.Components
{
    public class MiscUtilities
    {
        public static Color redColor = Color.FromRgb(191, 25, 25);
        public static Color defaultColor = Color.FromRgb(60, 152, 149);

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
            PreloadSound("defuse_stage_sound", Environment.CurrentDirectory + @"/Resources/Sounds/defuse_stage_sound.mp3");
            PreloadSound("round_start_sound", Environment.CurrentDirectory + @"/Resources/Sounds/round_start_sound.mp3");
            PreloadSound("round_finish_sound", Environment.CurrentDirectory + @"/Resources/Sounds/round_finish_sound.mp3");
        }
        public static void SetSoundsVolume()
        {
            soundPlayers["plant_stage_sound"].Volume = Properties.Settings.Default.ActivationPhaseVolume / 100f;
            soundPlayers["defuse_stage_sound"].Volume = Properties.Settings.Default.DeactivationPhaseVolume / 100f;
            soundPlayers["round_start_sound"].Volume = Properties.Settings.Default.PreparationPhaseVolume / 100f;
            soundPlayers["round_finish_sound"].Volume = Properties.Settings.Default.ComplitionPhaseVolume / 100f;
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
        public static string SetDeviceCode(Panel digitBlocksHostPanel)
        {
            string inputedCode = string.Empty;
            foreach (UIElement CodeBlockChild in digitBlocksHostPanel.Children)
            {
                if (CodeBlockChild is TextBox)
                {
                    var TextBox = (TextBox)CodeBlockChild;
                    if (!string.IsNullOrWhiteSpace(TextBox.Text))
                    {
                        inputedCode += TextBox.Text;
                    }
                }
            }
            return inputedCode;
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
        public static void CheckSettings()
        {
            bool invalidSettings = false;
            if (Settings.Default.ActivationPhaseCountdown < 20 || Settings.Default.ActivationPhaseCountdown > 240)
            {
                Settings.Default.ActivationPhaseCountdown = 120;
                invalidSettings = true;
            }
            if (Settings.Default.DeactivationPhaseCountdown < 20 || Settings.Default.DeactivationPhaseCountdown > 120)
            {
                Settings.Default.DeactivationPhaseCountdown = 60;
                invalidSettings = true;
            }
            if (Settings.Default.PreparationPhaseCountdown < 0 || Settings.Default.PreparationPhaseCountdown > 120)
            {
                Settings.Default.PreparationPhaseCountdown = 10;
                invalidSettings = true;
            }
            if (Settings.Default.ActivationPhaseVolume < 0 || Settings.Default.ActivationPhaseVolume > 100)
            {
                Settings.Default.ActivationPhaseVolume = 50;
                invalidSettings = true;
            }
            if (Settings.Default.DeactivationPhaseVolume < 0 || Settings.Default.DeactivationPhaseVolume > 100)
            {
                Settings.Default.DeactivationPhaseVolume = 50;
                invalidSettings = true;
            }
            if (Settings.Default.PreparationPhaseVolume < 0 || Settings.Default.PreparationPhaseVolume > 100)
            {
                Settings.Default.PreparationPhaseVolume = 50;
                invalidSettings = true;
            }
            if (Settings.Default.ComplitionPhaseVolume < 0 || Settings.Default.ComplitionPhaseVolume > 100)
            {
                Settings.Default.ComplitionPhaseVolume = 50;
                invalidSettings = true;
            }
            if (Settings.Default.DeviceTypeCode < 0 || Settings.Default.DeviceTypeCode > 1)
            {
                Settings.Default.DeviceTypeCode = 0;
                invalidSettings = true;
            }
            if (Settings.Default.ManualInputDigitsCount < 1 || Settings.Default.ManualInputDigitsCount > 8)
            {
                Settings.Default.ManualInputDigitsCount = 1;
                invalidSettings = true;
            }
            if (Settings.Default.UseUSBDevice < 0 || Settings.Default.UseUSBDevice > 1)
            {
                Settings.Default.UseUSBDevice = 0;
                invalidSettings = true;
            }
            if (int.TryParse(Settings.Default.DeviceCode, out _) == false && Settings.Default.DeviceCode.Length != 8)
            {
                Settings.Default.DeviceCode = "00000000";
                invalidSettings = true;
            }
            if (Settings.Default.USBDeviceName.Length < 1)
            {
                Settings.Default.USBDeviceName = "USB-Device-Name";
                invalidSettings = true;
            }
            if (invalidSettings == true)
            {
                Settings.Default.Save();
            }
        }
    }
}
