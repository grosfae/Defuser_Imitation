using Defuser_Imitation.Components.ViewModels;
using Defuser_Imitation.Properties;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Defuser_Imitation.Components.UserControls
{
    /// <summary>
    /// Логика взаимодействия для SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        OptionViewModel optionViewModel = new OptionViewModel();
        private bool isClosed = false;
        public SettingsControl()
        {
            InitializeComponent();
            DataContext = optionViewModel;
            optionViewModel.SettingsChangedEvent += OptionViewModel_SettingsChangedEvent;
        }
        
        private void SelfDispose()
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation()
            {
                Duration = TimeSpan.FromSeconds(0.2),
                To = 0,

            };
            doubleAnimation.Completed += DoubleAnimation_Completed;
            this.BeginAnimation(OpacityProperty, doubleAnimation);
        }
        private void CloseSettings()
        {
            if(optionViewModel.SettingsChanged == true)
            {
                var AcceptChangesControlAsChild = MainGrid.Children.OfType<AcceptChangesControl>().FirstOrDefault();
                if (MainGrid != null && MainGrid.Children.Contains(AcceptChangesControlAsChild))
                {
                    MainGrid.Children.Remove(AcceptChangesControlAsChild);
                    SelfDispose();
                    return;
                }
                AcceptChangesControl AcceptChangesControl = new AcceptChangesControl();
                AcceptChangesControl.AcceptChanges += AcceptChangesControl_AcceptChanges;
                AcceptChangesControl.DiscardChanges += AcceptChangesControl_DiscardChanges;
                MainGrid.Children.Add(AcceptChangesControl);
                return;
            }
            SelfDispose();
        }
        private void DoubleAnimation_Completed(object sender, EventArgs e)
        {
            if (isClosed == false)
            {
                isClosed = true;
                var window = Window.GetWindow(this);
                window.KeyDown -= UserControl_KeyDown;

                var ParentElement = this.Parent as Grid;
                if (ParentElement != null && ParentElement.Children.Contains(this))
                {
                    ParentElement.Children.Remove(this);
                }
            }
        }
        private void SaveSettings()
        {
            Settings.Default.DefuseCountdown = optionViewModel.DefuseCountdown;
            Settings.Default.RoundCountdown = optionViewModel.RoundCountdown;
            Settings.Default.ActiveDeviceVolume = optionViewModel.ActiveDeviceVolume;
            Settings.Default.RoundVolume = optionViewModel.RoundVolume;
            Settings.Default.DeviceTypeCode = optionViewModel.DeviceTypeCode;
            Settings.Default.ManualInputDigitsCount = optionViewModel.ManualInputDigitsCount;
            Settings.Default.UseUSBDevice = optionViewModel.UseUSBDevice;
            if(optionViewModel.DeviceCode.Length < 8)
            {
                int CodeLength = optionViewModel.DeviceCode.Length;
                for (int i = 0; i < 8 - CodeLength; i++)
                {
                    optionViewModel.DeviceCode += 0;
                }
                
            }
            Settings.Default.DeviceCode = optionViewModel.DeviceCode;
            Settings.Default.USBDeviceName = optionViewModel.USBDeviceName;
            Settings.Default.Save();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.KeyDown += UserControl_KeyDown;
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CloseSettings();
            }
        }
        private void SettingBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            Grid SettingBlock = sender as Grid;
            if (SettingBlock == BlockTypeCode)
            {
                TbSettingHeader.Text = "ТИП КОДА";
                TbSettingDescription.Text = "Установка типа кода устройства в зависимости от желаемого режима.\n" +
                    "Код устройства используется для его активации/деактивации.\n" +
                    "Случайный код: код устройства генерируется случайно.\n" +
                    "Свой код: пользователь вводит собственный код.";
            }
            if (SettingBlock == BlockDeviceCode)
            {
                TbSettingHeader.Text = "КОД УСТРОЙСТВА";
                TbSettingDescription.Text = "Установка собственного кода устройства. Код устройства используется для его активации/деактивации.";
            }
            if (SettingBlock == BlockManualDigit)
            {
                TbSettingHeader.Text = "СИМВОЛЫ РУЧНОГО ВВОДА";
                TbSettingDescription.Text = "Установка количества вводимых в ручную элементов кода устройства. Остальные элементы будут кода устройства будут уже введены.";
            }
            if (SettingBlock == BlockDeviceTimer)
            {
                TbSettingHeader.Text = "ТАЙМЕР УСТРОЙСТВА";
                TbSettingDescription.Text = "Установка таймера обратного отсчета в секундах для активированного устройства.";
            }
            if (SettingBlock == BlockRoundTimer)
            {
                TbSettingHeader.Text = "ТАЙМЕР РАУНДА";
                TbSettingDescription.Text = "Установка таймера обратного отсчета в секундах для раунда.";
            }
            if (SettingBlock == BlockDeviceVolume)
            {
                TbSettingHeader.Text = "ГРОМКОСТЬ АКТИВИРОВАННОГО УСТРОЙСТВА";
                TbSettingDescription.Text = "Установка громкости таймера обратного отсчета активированного устройства.";
            }
            if (SettingBlock == BlockRoundVolume)
            {
                TbSettingHeader.Text = "ГРОМКОСТЬ ТАЙМЕРА РАУНДА";
                TbSettingDescription.Text = "Установка громкости таймера обратного отсчета раунда.";
            }
            if (SettingBlock == BlockUSBDevice)
            {
                TbSettingHeader.Text = "ИСПОЛЬЗОВАТЬ УСКОРЕННУЮ ДЕАКТИВАЦИЮ";
                TbSettingDescription.Text = "Установка параметра использования ускоренной деактивации устройства.\n" +
                    "Ускоренная деактивация заключается в вставке физического USB-носителя в соответствующий разъем активированного устройства.\n" +
                    "Не использовать: отключает данную функцию.\n" +
                    "Использовать: включает данную функцию.";
            }
            if (SettingBlock == BlockUSBDeviceName)
            {
                TbSettingHeader.Text = "НАИМЕНОВАНИЕ НОСИТЕЛЯ";
                TbSettingDescription.Text = "Установка наименования USB-носителя для ускоренной деактивации устройства.\n";
            }
        }
        private void SettingBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            TbSettingHeader.Text = "";
            TbSettingDescription.Text = "";
        }

        private void SettingsDefaultBtn_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.Reset();
            DataContext = null;
            optionViewModel = new OptionViewModel();
            optionViewModel.SettingsChangedEvent += OptionViewModel_SettingsChangedEvent;
            DataContext = optionViewModel;
        }
        private void SettingsSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            SettingsSaveBtn.IsEnabled = false;
            DataContext = null;
            optionViewModel = new OptionViewModel();
            optionViewModel.SettingsChangedEvent += OptionViewModel_SettingsChangedEvent;
            DataContext = optionViewModel;
        }
        private void SwchTypeCode_SelectedItemChanged(object sender, EventArgs e)
        {
            switch (SwchTypeCode.SelectedIndex)
            {
                case 0:
                    BlockDeviceCode.IsEnabled = false;
                    BlockDeviceCode.Opacity = 0.6;
                    break;
                case 1:
                    BlockDeviceCode.IsEnabled = true;
                    BlockDeviceCode.Opacity = 1;
                    break;
            }
        }

        private void SwchFastDefuse_SelectedItemChanged(object sender, EventArgs e)
        {
            switch (SwchFastDefuse.SelectedIndex)
            {
                case 0:
                    BlockUSBDeviceName.IsEnabled = false;
                    BlockUSBDeviceName.Opacity = 0.6;
                    break;
                case 1:
                    BlockUSBDeviceName.IsEnabled = true;
                    BlockUSBDeviceName.Opacity = 1;
                    break;
            }
        }

        private void OptionViewModel_SettingsChangedEvent(object sender, EventArgs e)
        {
            if (optionViewModel.SettingsChanged == true)
            {
                SettingsSaveBtn.IsEnabled = true;
            }
        }
        private void AcceptChangesControl_AcceptChanges(object sender, EventArgs e)
        {
            SaveSettings();
            SettingsSaveBtn.IsEnabled = false;
            DataContext = null;
            optionViewModel = new OptionViewModel();
            optionViewModel.SettingsChangedEvent += OptionViewModel_SettingsChangedEvent;
            DataContext = optionViewModel;
        }
        private void AcceptChangesControl_DiscardChanges(object sender, EventArgs e)
        {
            CloseSettings();
        }
        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            CloseSettings();
        }

        private void CodeBoxControl_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if(Regex.IsMatch(e.Text, @"[0-9]") == false)
            {
                e.Handled = true;
            }
        }
        private void NoneSpaceBar_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }
    }
}
