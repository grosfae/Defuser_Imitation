using Defuser_Imitation.Properties;

namespace Defuser_Imitation.Components.ViewModels
{
    public class OptionViewModel : ViewModelBase
    {
        public event EventHandler SettingsChangedEvent;
        protected virtual void OnSettingsChangedEvent()
        {
            if (SettingsChangedEvent != null)
            {
                SettingsChangedEvent(this, EventArgs.Empty);
            }
        }
        private List<string> typeCodeList = new List<string>
        {
            "Случайный код", "Свой код"
        };
        private List<string> fastDefuseList = new List<string>
        {
            "Не использовать", "Использовать"
        };
        private bool settingsChanged = false;
        private int activationPhaseCountdown = Settings.Default.ActivationPhaseCountdown;
        private int deactivationPhaseCountdown = Settings.Default.DeactivationPhaseCountdown;
        private int preparationPhaseCountdown = Settings.Default.PreparationPhaseCountdown;
        private int activationPhaseVolume = Settings.Default.ActivationPhaseVolume;
        private int deactivationPhaseVolume = Settings.Default.DeactivationPhaseVolume;
        private int preparationPhaseVolume = Settings.Default.PreparationPhaseVolume;
        private int complitionPhaseVolume = Settings.Default.ComplitionPhaseVolume;
        private int deviceTypeCode = Settings.Default.DeviceTypeCode;
        private int manualInputDigitsCount = Settings.Default.ManualInputDigitsCount;
        private int useUSBDevice = Settings.Default.UseUSBDevice;
        private string deviceCode = Settings.Default.DeviceCode;
        private string usbDeviceName = Settings.Default.USBDeviceName;
        public List<string> TypeCodeList
        {
            get
            {
                return typeCodeList;
            }
            set
            {
                typeCodeList = value;
                OnPropertyChanged(nameof(TypeCodeList));
            }
        }
        public List<string> FastDefuseList
        {
            get
            {
                return fastDefuseList;
            }
            set
            {
                fastDefuseList = value;
                OnPropertyChanged(nameof(FastDefuseList));
            }
        }
        public int ActivationPhaseCountdown
        {
            get
            {
                return activationPhaseCountdown;
            }
            set
            {
                activationPhaseCountdown = value;
                OnPropertyChanged(nameof(ActivationPhaseCountdown));
                SettingsHasBeenChanged();
            }
        }
        public int DeactivationPhaseCountdown
        {
            get
            {
                return deactivationPhaseCountdown;
            }
            set
            {
                deactivationPhaseCountdown = value;
                OnPropertyChanged(nameof(DeactivationPhaseCountdown));
                SettingsHasBeenChanged();
            }
        }
        public int PreparationPhaseCountdown
        {
            get
            {
                return preparationPhaseCountdown;
            }
            set
            {
                preparationPhaseCountdown = value;
                OnPropertyChanged(nameof(PreparationPhaseCountdown));
                SettingsHasBeenChanged();
            }
        }
        public int ActivationPhaseVolume
        {
            get
            {
                return activationPhaseVolume;
            }
            set
            {
                activationPhaseVolume = value;
                OnPropertyChanged(nameof(ActivationPhaseVolume));
                SettingsHasBeenChanged();
            }
        }
        public int DeactivationPhaseVolume
        {
            get
            {
                return deactivationPhaseVolume;
            }
            set
            {
                deactivationPhaseVolume = value;
                OnPropertyChanged(nameof(DeactivationPhaseVolume));
                SettingsHasBeenChanged();
            }
        }
        public int PreparationPhaseVolume
        {
            get
            {
                return preparationPhaseVolume;
            }
            set
            {
                preparationPhaseVolume = value;
                OnPropertyChanged(nameof(PreparationPhaseVolume));
                SettingsHasBeenChanged();
            }
        }
        public int ComplitionPhaseVolume
        {
            get
            {
                return complitionPhaseVolume;
            }
            set
            {
                complitionPhaseVolume = value;
                OnPropertyChanged(nameof(ComplitionPhaseVolume));
                SettingsHasBeenChanged();
            }
        }
        public int DeviceTypeCode
        {
            get
            {
                return deviceTypeCode;
            }
            set
            {
                deviceTypeCode = value;
                OnPropertyChanged(nameof(DeviceTypeCode));
                SettingsHasBeenChanged();
            }
        }
        public int ManualInputDigitsCount
        {
            get
            {
                return manualInputDigitsCount;
            }
            set
            {
                manualInputDigitsCount = value;
                OnPropertyChanged(nameof(ManualInputDigitsCount));
                SettingsHasBeenChanged();
            }
        }
        public int UseUSBDevice
        {
            get
            {
                return useUSBDevice;
            }
            set
            {
                useUSBDevice = value;
                OnPropertyChanged(nameof(UseUSBDevice));
                SettingsHasBeenChanged();
            }
        }
        public string DeviceCode
        {
            get
            {
                return deviceCode;
            }
            set
            {
                deviceCode = value;
                OnPropertyChanged(nameof(DeviceCode));
                SettingsHasBeenChanged();
            }
        }
        public string USBDeviceName
        {
            get
            {
                return usbDeviceName;
            }
            set
            {
                usbDeviceName = value;
                OnPropertyChanged(nameof(USBDeviceName));
                SettingsHasBeenChanged();
            }
        }
        public bool SettingsChanged
        {
            get
            {
                return settingsChanged;
            }
            set
            {
                settingsChanged = value;
                OnPropertyChanged(nameof(SettingsChanged));
            }
        }
        public void SettingsHasBeenChanged()
        {
            settingsChanged = true;
            OnSettingsChangedEvent();
        }
        private void CheckDeviceCode()
        {
            if (DeviceCode.Length < 8)
            {
                int codeLength = DeviceCode.Length;
                for (int i = 0; i < 8 - codeLength; i++)
                {
                    DeviceCode += 0;
                }
            }
        }
        private void CheckUSBDeviceName()
        {
            if (string.IsNullOrWhiteSpace(USBDeviceName))
            {
                USBDeviceName = "USB-Device-Name";
            }
        }
        public void SaveSettings()
        {
            CheckDeviceCode();
            CheckUSBDeviceName();
            Settings.Default.ActivationPhaseCountdown = activationPhaseCountdown;
            Settings.Default.DeactivationPhaseCountdown = deactivationPhaseCountdown;
            Settings.Default.PreparationPhaseCountdown = preparationPhaseCountdown;
            Settings.Default.ActivationPhaseVolume = activationPhaseVolume;
            Settings.Default.DeactivationPhaseVolume = deactivationPhaseVolume;
            Settings.Default.PreparationPhaseVolume = preparationPhaseVolume;
            Settings.Default.ComplitionPhaseVolume = complitionPhaseVolume;
            Settings.Default.DeviceTypeCode = deviceTypeCode;
            Settings.Default.ManualInputDigitsCount = manualInputDigitsCount;
            Settings.Default.UseUSBDevice = useUSBDevice;
            Settings.Default.DeviceCode = deviceCode;
            Settings.Default.USBDeviceName = usbDeviceName;
            Settings.Default.Save();
            SettingsChanged = false;
        }
        public void ResetSettings()
        {
            Settings.Default.Reset();
            Settings.Default.Save();
            ActivationPhaseCountdown = Settings.Default.ActivationPhaseCountdown;
            DeactivationPhaseCountdown = Settings.Default.DeactivationPhaseCountdown;
            PreparationPhaseCountdown = Settings.Default.PreparationPhaseCountdown;
            ActivationPhaseVolume = Settings.Default.ActivationPhaseVolume;
            DeactivationPhaseVolume = Settings.Default.DeactivationPhaseVolume;
            PreparationPhaseVolume = Settings.Default.PreparationPhaseVolume;
            ComplitionPhaseVolume = Settings.Default.ComplitionPhaseVolume;
            DeviceTypeCode = Settings.Default.DeviceTypeCode;
            ManualInputDigitsCount = Settings.Default.ManualInputDigitsCount;
            UseUSBDevice = Settings.Default.UseUSBDevice;
            DeviceCode = Settings.Default.DeviceCode;
            USBDeviceName = Settings.Default.USBDeviceName;
            SettingsChanged = false;
        }
    }
}
