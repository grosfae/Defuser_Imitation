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
                return Settings.Default.ActivationPhaseCountdown;
            }
            set
            {
                Settings.Default.ActivationPhaseCountdown = value;
                OnPropertyChanged(nameof(ActivationPhaseCountdown));
                SettingsHasBeenChanged();
            }
        }
        public int DeactivationPhaseCountdown
        {
            get
            {
                return Settings.Default.DeactivationPhaseCountdown;
            }
            set
            {
                Settings.Default.DeactivationPhaseCountdown = value;
                OnPropertyChanged(nameof(DeactivationPhaseCountdown));
                SettingsHasBeenChanged();
            }
        }
        public int PreparationPhaseCountdown
        {
            get
            {
                return Settings.Default.PreparationPhaseCountdown;
            }
            set
            {
                Settings.Default.PreparationPhaseCountdown = value;
                OnPropertyChanged(nameof(PreparationPhaseCountdown));
                SettingsHasBeenChanged();
            }
        }
        public int ActivationPhaseVolume
        {
            get
            {
                return Settings.Default.ActivationPhaseVolume;
            }
            set
            {
                Settings.Default.ActivationPhaseVolume = value;
                OnPropertyChanged(nameof(ActivationPhaseVolume));
                SettingsHasBeenChanged();
            }
        }
        public int DeactivationPhaseVolume
        {
            get
            {
                return Settings.Default.DeactivationPhaseVolume;
            }
            set
            {
                Settings.Default.DeactivationPhaseVolume = value;
                OnPropertyChanged(nameof(DeactivationPhaseVolume));
                SettingsHasBeenChanged();
            }
        }
        public int PreparationPhaseVolume
        {
            get
            {
                return Settings.Default.PreparationPhaseVolume;
            }
            set
            {
                Settings.Default.PreparationPhaseVolume = value;
                OnPropertyChanged(nameof(PreparationPhaseVolume));
                SettingsHasBeenChanged();
            }
        }
        public int ComplitionPhaseVolume
        {
            get
            {
                return Settings.Default.ComplitionPhaseVolume;
            }
            set
            {
                Settings.Default.ComplitionPhaseVolume = value;
                OnPropertyChanged(nameof(ComplitionPhaseVolume));
                SettingsHasBeenChanged();
            }
        }
        public int DeviceTypeCode
        {
            get
            {
                return Settings.Default.DeviceTypeCode;
            }
            set
            {
                Settings.Default.DeviceTypeCode = value;
                OnPropertyChanged(nameof(DeviceTypeCode));
                SettingsHasBeenChanged();
            }
        }
        public int ManualInputDigitsCount
        {
            get
            {
                return Settings.Default.ManualInputDigitsCount;
            }
            set
            {
                Settings.Default.ManualInputDigitsCount = value;
                OnPropertyChanged(nameof(ManualInputDigitsCount));
                SettingsHasBeenChanged();
            }
        }
        public int UseUSBDevice
        {
            get
            {
                return Settings.Default.UseUSBDevice;
            }
            set
            {
                Settings.Default.UseUSBDevice = value;
                OnPropertyChanged(nameof(UseUSBDevice));
                SettingsHasBeenChanged();
            }
        }
        public string DeviceCode
        {
            get
            {
                return Settings.Default.DeviceCode;
            }
            set
            {
                Settings.Default.DeviceCode = value;
                OnPropertyChanged(nameof(DeviceCode));
                SettingsHasBeenChanged();
            }
        }
        public string USBDeviceName
        {
            get
            {
                return Settings.Default.USBDeviceName;
            }
            set
            {
                Settings.Default.USBDeviceName = value;
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
        public void SaveSettings()
        {
            if (DeviceCode.Length < 8)
            {
                int CodeLength = DeviceCode.Length;
                for (int i = 0; i < 8 - CodeLength; i++)
                {
                    DeviceCode += 0;
                }
            }
            Settings.Default.Save();
            SettingsChanged = false;
        }
    }
}
