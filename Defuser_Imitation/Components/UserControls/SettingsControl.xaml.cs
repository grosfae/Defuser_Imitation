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
        private OptionViewModel optionViewModel = new OptionViewModel();
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
        private void SetSettingInformation(UIElement settingBlock)
        {
            if (settingBlock == BlockTypeCode)
            {
                TbSettingHeader.Text = "ТИП КОДА";
                TbSettingDescription.Text = "Установка типа кода устройства в зависимости от желаемого режима.\n" +
                    "Код устройства используется для его активации/деактивации.\n" +
                    "Случайный код: код устройства генерируется случайно при каждом новом запуске раунда.\n" +
                    "Свой код: пользователь устанавливает собственный код.";
            }
            else if (settingBlock == BlockDeviceCode)
            {
                TbSettingHeader.Text = "КОД УСТРОЙСТВА";
                TbSettingDescription.Text = "Установка собственного кода устройства. Код устройства используется для его активации/деактивации. \n" +
                    "Для применения введённого кода необходимо нажать клавишу Enter.";
            }
            else if(settingBlock == BlockManualDigit)
            {
                TbSettingHeader.Text = "СИМВОЛЫ РУЧНОГО ВВОДА";
                TbSettingDescription.Text = "Установка количества вводимых вручную элементов кода устройства на этапах активации и деацтивации. Остальные элементы будут кода устройства будут введены автоматически.";
            }
            else if (settingBlock == BlockActivationPhase)
            {
                TbSettingHeader.Text = "ВРЕМЯ ЭТАПА АКТИВАЦИИ";
                TbSettingDescription.Text = "Установка таймера обратного отсчета в секундах для этапа активации.";
            }
            else if(settingBlock == BlockDeactivationPhase)
            {
                TbSettingHeader.Text = "ВРЕМЯ ЭТАПА ДЕАКТИВАЦИИ";
                TbSettingDescription.Text = "Установка таймера обратного отсчета в секундах для этапа деактивации.";
            }
            else if (settingBlock == BlockPreparationPhase)
            {
                TbSettingHeader.Text = "ВРЕМЯ ПОДГОТОВИТЕЛЬНОЙ ФАЗЫ";
                TbSettingDescription.Text = "Установка таймера обратного отсчета в секундах для подготовительной фазы перед началом игры.";
            }
            else if (settingBlock == BlockActivationPhaseVolume)
            {
                TbSettingHeader.Text = "ГРОМКОСТЬ ЗВУКОВ ЭТАПА АКТИВАЦИИ";
                TbSettingDescription.Text = "Установка громкости этапа еактивации.";
            }
            else if(settingBlock == BlockDeactivationPhaseVolume)
            {
                TbSettingHeader.Text = "ГРОМКОСТЬ ЗВУКОВ ЭТАПА ДЕАКТИВАЦИИ";
                TbSettingDescription.Text = "Установка громкости звуков этапа деактивации.";
            }
            else if(settingBlock == BlockPreparationPhaseVolume)
            {
                TbSettingHeader.Text = "ГРОМКОСТЬ ЗВУКОВ ФАЗЫ ПОДГОТОВКИ";
                TbSettingDescription.Text = "Установка громкости звуков фазы подготовки перед началом раунда.";
            }
            else if(settingBlock == BlockComplitionPhaseVolume)
            {
                TbSettingHeader.Text = "ГРОМКОСТЬ ЗВУКОВ ИСХОДА РАУНДА";
                TbSettingDescription.Text = "Установка громкости звуков экрана исхода раунда.";
            }
            else if(settingBlock == BlockUSBDevice)
            {
                TbSettingHeader.Text = "ИСПОЛЬЗОВАТЬ УСКОРЕННУЮ ДЕАКТИВАЦИЮ";
                TbSettingDescription.Text = "Установка параметра использования ускоренной деактивации устройства.\n" +
                    "Ускоренная деактивация работает при вставке физического USB-носителя в соответствующий разъем активированного устройства.\n" +
                    "Не использовать: отключает данную функцию.\n" +
                    "Использовать: включает данную функцию.";
            }
            else if(settingBlock == BlockUSBDeviceName)
            {
                TbSettingHeader.Text = "НАИМЕНОВАНИЕ НОСИТЕЛЯ";
                TbSettingDescription.Text = "Установка наименования USB-носителя для ускоренной деактивации устройства.\n" +
                    "Для применения введённого наименования необходимо нажать клавишу Enter.";
            }
        }
        private void SettingBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            SetSettingInformation((UIElement)sender);
        }
        private void SettingBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            TbSettingHeader.Text = "";
            TbSettingDescription.Text = "";
        }

        private void SettingsDefaultBtn_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.Reset();
            optionViewModel.SaveSettings();
        }
        private void SettingsSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            optionViewModel.SaveSettings();
            SettingsSaveBtn.IsEnabled = false;
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
            optionViewModel.SaveSettings();
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
