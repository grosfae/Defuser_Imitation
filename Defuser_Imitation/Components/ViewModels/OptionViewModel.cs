using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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
        private List<string> _typeCodeList = new List<string>
        {
            "Случайный код", "Свой код"
        };
        private List<string> _fastDefuseList = new List<string>
        {
            "Не использовать", "Использовать"
        };
        private bool _settingsChanged = false;
        private int _defuseCountdown = Properties.Settings.Default.DefuseCountdown;
        private int _roundCountdown = Properties.Settings.Default.RoundCountdown;
        private int _activeDeviceVolume = Properties.Settings.Default.ActiveDeviceVolume;
        private int _roundVolume = Properties.Settings.Default.RoundVolume;
        private int _deviceTypeCode = Properties.Settings.Default.DeviceTypeCode;
        private int _manualInputDigitsCount = Properties.Settings.Default.ManualInputDigitsCount;
        private int _useUSBDevice = Properties.Settings.Default.UseUSBDevice;
        private string _deviceCode = Properties.Settings.Default.DeviceCode;
        private string _uSBDeviceName = Properties.Settings.Default.USBDeviceName;
        public List<string> TypeCodeList
        {
            get
            {
                return _typeCodeList;
            }
            set
            {
                _typeCodeList = value;
                OnPropertyChanged(nameof(TypeCodeList));
            }
        }
        public List<string> FastDefuseList
        {
            get
            {
                return _fastDefuseList;
            }
            set
            {
                _fastDefuseList = value;
                OnPropertyChanged(nameof(FastDefuseList));
            }
        }
        public int DefuseCountdown
        {
            get
            {
                return _defuseCountdown;
            }
            set
            {
                _defuseCountdown = value;
                OnPropertyChanged(nameof(DefuseCountdown));
                SettingsHasBeenChanged();
            }
        }
        public int RoundCountdown
        {
            get
            {
                return _roundCountdown;
            }
            set
            {
                _roundCountdown = value;
                OnPropertyChanged(nameof(RoundCountdown));
                SettingsHasBeenChanged();
            }
        }
        public int ActiveDeviceVolume
        {
            get
            {
                return _activeDeviceVolume;
            }
            set
            {
                _activeDeviceVolume = value;
                OnPropertyChanged(nameof(ActiveDeviceVolume));
                SettingsHasBeenChanged();
            }
        }
        public int RoundVolume
        {
            get
            {
                return _roundVolume;
            }
            set
            {
                _roundVolume = value;
                OnPropertyChanged(nameof(RoundVolume));
                SettingsHasBeenChanged();
            }
        }
        public int DeviceTypeCode
        {
            get
            {
                return _deviceTypeCode;
            }
            set
            {
                _deviceTypeCode = value;
                OnPropertyChanged(nameof(DeviceTypeCode));
                SettingsHasBeenChanged();
            }
        }
        public int ManualInputDigitsCount
        {
            get
            {
                return _manualInputDigitsCount;
            }
            set
            {
                _manualInputDigitsCount = value;
                OnPropertyChanged(nameof(ManualInputDigitsCount));
                SettingsHasBeenChanged();
            }
        }
        public int UseUSBDevice
        {
            get
            {
                return _useUSBDevice;
            }
            set
            {
                _useUSBDevice = value;
                OnPropertyChanged(nameof(UseUSBDevice));
                SettingsHasBeenChanged();
            }
        }
        public string DeviceCode
        {
            get
            {
                return _deviceCode;
            }
            set
            {
                _deviceCode = value;
                OnPropertyChanged(nameof(DeviceCode));
                SettingsHasBeenChanged();
            }
        }
        public string USBDeviceName
        {
            get
            {
                return _uSBDeviceName;
            }
            set
            {
                _uSBDeviceName = value;
                OnPropertyChanged(nameof(USBDeviceName));
                SettingsHasBeenChanged();
            }
        }
        public bool SettingsChanged
        {
            get
            {
                return _settingsChanged;
            }
            set
            {
                _settingsChanged = value;
                OnPropertyChanged(nameof(SettingsChanged));
            }
        }
        public OptionViewModel()
        {
        
        }
        public void SettingsHasBeenChanged()
        {
            _settingsChanged = true;
            OnSettingsChangedEvent();
        }
    }
}
