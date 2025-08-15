using Defuser_Imitation.Pages;
using Defuser_Imitation.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Defuser_Imitation.Components.UserControls
{
    /// <summary>
    /// Логика взаимодействия для DefuseDeviceControl.xaml
    /// </summary>
    public partial class DefuseDeviceControl : UserControl
    {
        private Grid PlayPageGrid;
        private DispatcherTimer DefuseTimer = new();
        private System.Timers.Timer UsbCheckTimer = new();

        private int DefuseCountdown = Settings.Default.DefuseCountdown;
        private string DefuseCode = MiscUtilities.DeviceCode();
        private int ManualDigitCount = Settings.Default.ManualInputDigitsCount;
        private int UseUSBDevice = Settings.Default.UseUSBDevice;
        private string USBDeviceName = Settings.Default.USBDeviceName;

        private int DigitBlockIndex = 0;
        private bool PauseActive = false;

        private int[] ManualDigitBoxIndexArray = new int[Settings.Default.ManualInputDigitsCount];
        private List<int> IndicatorActiveBlockList = new();
        private List<int> IndicatorDeactiveBlockList = new();

        private SinDrawGeometry SignalWaveOriginal = new();
        private SinDrawGeometry SignalWaveDevice = new();

        public DefuseDeviceControl(Grid playPageGrid)
        {
            InitializeComponent();
            PlayPageGrid = playPageGrid;
            ((Page)PlayPageGrid.Parent).KeyDown += PlayPage_KeyDown;

            MiscUtilities.soundPlayers["active_device_sound"].Volume = Properties.Settings.Default.RoundVolume / 100f;
            MiscUtilities.soundPlayers["active_device_last_seconds_sound"].Volume = Properties.Settings.Default.RoundVolume / 100f;
            MiscUtilities.soundPlayers["active_device_sound"].MediaEnded += ActiveDeviceSoundPlayer_MediaEnded;
            MiscUtilities.soundPlayers["active_device_sound"].Play();

            DrawIndicatorElements();
            DisplayDeviceCodeBlock();

            DefuseTimer.Tick += DefuseTimer_Tick;
            DefuseTimer.Interval = TimeSpan.FromSeconds(1);
            DefuseTimer.Start();

            TbCountdown.Text = DefuseCountdown.ToString();

            if (UseUSBDevice == 1)
            {
                UsbCheckTimer.Interval = 300;
                UsbCheckTimer.Elapsed += UsbCheckTimer_Elapsed;
                UsbCheckTimer.Enabled = true;
            }
        }
        private void ActiveDeviceSoundPlayer_MediaEnded(object sender, EventArgs e)
        {
            if (DefuseCountdown > 15)
            {
                MiscUtilities.soundPlayers["active_device_sound"].Play();
            }
        }
        private void DefuseTimer_Tick(object sender, EventArgs e)
        {
            if (DefuseCountdown == 15)
            {
                MiscUtilities.soundPlayers["active_device_sound"].Stop();
                MiscUtilities.soundPlayers["active_device_last_seconds_sound"].Play();
            }
            if (DefuseCountdown != 1)
            {
                DefuseCountdown--;
                TbCountdown.Text = DefuseCountdown.ToString();
            }
            else
            {
                TbCountdown.Text = "0";
                FinishRound(1);
            }
        }
        private void UsbCheckTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (MiscUtilities.CheckUSBDevice(USBDeviceName) == true)
            {
                UsbCheckTimer.Stop();
                Application.Current.Dispatcher.Invoke(() => FinishRound(3));
            }
        }
        private void DrawIndicatorElements()
        {
            IndicatorGrid.Children.Clear();
            int CurrentRowIndex = 0;
            int CurrentColumnIndex = 0;
            for (int i = 0; i < 27; i++)
            {
                Rectangle IndicatorBlock = new Rectangle();
                IndicatorBlock.Fill = new SolidColorBrush(Color.FromRgb(16, 49, 62));
                IndicatorBlock.Focusable = false;
                IndicatorBlock.Margin = new Thickness(4, 4, 4, 4);
                if (i < 9)
                {
                    CurrentRowIndex = 0;
                }
                else if (i >= 9 && i < 18)
                {
                    CurrentRowIndex = 1;
                }
                else if (i >= 18 && i < 27)
                {
                    CurrentRowIndex = 2;
                }
                Grid.SetRow(IndicatorBlock, CurrentRowIndex);
                Grid.SetColumn(IndicatorBlock, CurrentColumnIndex);
                IndicatorGrid.Children.Add(IndicatorBlock);
                IndicatorDeactiveBlockList.Add(i);

                CurrentColumnIndex++;
                if (CurrentColumnIndex == 9)
                {
                    CurrentColumnIndex = 0;
                }
            }
            ShuffleArrayClass.Shuffle(IndicatorDeactiveBlockList);
            for (int indicatorIndex = 0; indicatorIndex < DefuseCode.Length - ManualDigitCount; indicatorIndex++)
            {
                ActivateIndicators();
            }
        }
        private void DisplayDeviceCodeBlock()
        {
            int[] DigitArray = new int[8] { 0, 1, 2, 3, 4, 5, 6, 7 };
            ShuffleArrayClass.Shuffle(DigitArray);
            Array.Resize(ref DigitArray, ManualDigitCount);

            char[] CodeCharArray = DefuseCode.ToCharArray();
            int CurrentChildIndex = 0;
            int CurrentCodeCharIndex = 0;

            int ManualDigitBoxIndex = 0;
            foreach (var child in StDeviceCode.Children)
            {
                if (CurrentChildIndex == 3 || CurrentChildIndex == 7)
                {
                    CurrentChildIndex++;
                    continue;
                }

                TextBox DigitBox = (TextBox)child;
                if (DigitArray.Contains(CurrentCodeCharIndex))
                {
                    DigitBox.IsEnabled = true;
                    DigitBox.TextChanged += DigitBox_TextChanged;
                    DigitBox.PreviewKeyDown += DigitBox_PreviewKeyDown;
                    DigitBox.PreviewTextInput += DigitBox_PreviewTextInput;
                    ManualDigitBoxIndexArray[ManualDigitBoxIndex] = CurrentChildIndex;
                    ManualDigitBoxIndex++;
                    CurrentCodeCharIndex++;
                    CurrentChildIndex++;
                    continue;
                }
                DigitBox.Text = CodeCharArray[CurrentCodeCharIndex].ToString();
                DigitBox.IsEnabled = false;
                CurrentCodeCharIndex++;
                CurrentChildIndex++;
            } 
        }
        private void DigitBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"[0-9]") == false)
            {
                e.Handled = true;
            }
        }
        private void DigitBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                SolidColorBrush LocalColorResource = (SolidColorBrush)TryFindResource("LocalColor");
                LocalColorResource.Color = Color.FromRgb(60, 152, 149);
                UIElement CodeBox = (UIElement)sender;
                if (((TextBox)CodeBox).Text.Length == 0)
                {
                    int ElementIndex = StDeviceCode.Children.IndexOf(CodeBox);
                    for (int i = ElementIndex - 1; i > -1; i--)
                    {
                        CodeBox = StDeviceCode.Children[i];
                        if (CodeBox is TextBox && CodeBox.IsEnabled == true)
                        {
                            CodeBox.Focus();
                            break;
                        }
                    }
                }
                int DefuseCodeSumm = 0;
                string InputedCode = "";
                foreach (UIElement CodeBlockChild in StDeviceCode.Children)
                {
                    if (CodeBlockChild is TextBox)
                    {
                        InputedCode += ((TextBox)CodeBlockChild).Text;
                    }
                }
                for (int i = 0; i < InputedCode.Length; i++)
                {
                    DefuseCodeSumm += int.Parse(InputedCode[i].ToString());
                }
                SignalWaveDevice.Scale = DefuseCodeSumm + 100;
                SignalWaveDevice.Multiplier = DefuseCodeSumm / DefuseCode.Length;
                PathDeviceSignal.Data = SignalWaveDevice.Sinusoid;

            }
        }
        private void DigitBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UIElement CodeBox = (UIElement)sender;
            if(((TextBox)CodeBox).Text.Length == 0)
            {
                DeactivateIndicators();
            }

            if (((TextBox)CodeBox).Text.Length > 0)
            {
                int DefuseCodeSumm = 0;
                string InputedCode = "";

                foreach (UIElement CodeBlockChild in StDeviceCode.Children)
                {
                    if (CodeBlockChild is TextBox)
                    {
                        InputedCode += ((TextBox)CodeBlockChild).Text;
                    }
                }

                for (int i = 0; i < InputedCode.Length; i++)
                {
                    DefuseCodeSumm += int.Parse(InputedCode[i].ToString());
                }
                SignalWaveDevice.Scale = DefuseCodeSumm + 100;
                SignalWaveDevice.Multiplier = DefuseCodeSumm / DefuseCode.Length;
                PathDeviceSignal.Data = SignalWaveDevice.Sinusoid;

                ActivateIndicators();
                
                if (InputedCode.Length == 8)
                {
                    if (InputedCode == DefuseCode)
                    {
                        ActivateIndicators();
                        FinishRound(3);
                        return;
                    }
                    else
                    {
                        SolidColorBrush LocalColorResource = (SolidColorBrush)TryFindResource("LocalColor");
                        LocalColorResource.Color = Color.FromRgb(191, 25, 25);
                    }
                }
                int ElementIndex = StDeviceCode.Children.IndexOf(CodeBox);
                if (ElementIndex < 11)
                {
                    for (int i = ElementIndex + 1; i < StDeviceCode.Children.Count; i++)
                    {
                        CodeBox = StDeviceCode.Children[i];
                        if (CodeBox is TextBox && CodeBox.IsEnabled == true)
                        {
                            CodeBox.Focus();
                            break;
                        }
                    }
                }
            }
        }
        private void ActivateIndicators()
        {
            for (int i = 0; i < 3; i++)
            {
                var IndicatorDeactiveListElement = IndicatorDeactiveBlockList[0];
                Rectangle GridChild = (Rectangle)IndicatorGrid.Children[IndicatorDeactiveListElement];
                GridChild.Fill.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation()
                {
                    To = Color.FromRgb(1, 255, 229),
                    Duration = TimeSpan.FromSeconds(0.3),
                    BeginTime = TimeSpan.FromSeconds(i * 0.1)
                });
                IndicatorActiveBlockList.Add(IndicatorDeactiveListElement);
                IndicatorDeactiveBlockList.Remove(IndicatorDeactiveListElement);
            }
        }
        private void DeactivateIndicators()
        {
            for (int i = 0; i < 3; i++)
            {
                var IndicatorActiveListElement = IndicatorActiveBlockList[0];
                Rectangle IndicatorCell = (Rectangle)IndicatorGrid.Children[IndicatorActiveListElement];
                IndicatorCell.Fill.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation()
                {
                    To = Color.FromRgb(16, 49, 62),
                    Duration = TimeSpan.FromSeconds(0.3),
                    BeginTime = TimeSpan.FromSeconds(i * 0.1)
                });
                IndicatorDeactiveBlockList.Add(IndicatorActiveListElement);
                IndicatorActiveBlockList.Remove(IndicatorActiveListElement);
            }
        }
        private void DefuseDeviceControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetSignalWaves();
            SetFocusOnDigitBlock();
            PathOriginalSignal.RenderTransform.BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation()
            {
                Duration = TimeSpan.FromSeconds(2),
                From = 0,
                To = 212,
                RepeatBehavior = RepeatBehavior.Forever
            });
            PathDeviceSignal.RenderTransform.BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation()
            {
                Duration = TimeSpan.FromSeconds(2),
                From = 0,
                To = 212,
                RepeatBehavior = RepeatBehavior.Forever
            });
        }
        private void SetSignalWaves()
        {
            SignalWaveOriginal.X0 = 0;
            SignalWaveOriginal.X1 = 20;           
            SignalWaveOriginal.StepsCount = 400;

            SignalWaveDevice.X0 = 0;
            SignalWaveDevice.X1 = 20;
            SignalWaveDevice.StepsCount = 400;

            int SettingsDeviceCodeSumm = 0;
            for (int i = 0; i < DefuseCode.Length; ++i)
            {
                SettingsDeviceCodeSumm += int.Parse(DefuseCode[i].ToString());
            }
            SignalWaveOriginal.Scale = SettingsDeviceCodeSumm + 100;
            SignalWaveOriginal.Multiplier = SettingsDeviceCodeSumm / DefuseCode.Length;
            PathOriginalSignal.Data = SignalWaveOriginal.Sinusoid;
            SetSignalWavesAnimation();
        }
        private void SetSignalWavesAnimation()
        {
            PathOriginalSignal.RenderTransform.BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation()
            {
                Duration = TimeSpan.FromSeconds(2),
                From = 0,
                To = 212,
                RepeatBehavior = RepeatBehavior.Forever
            });
            PathDeviceSignal.RenderTransform.BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation()
            {
                Duration = TimeSpan.FromSeconds(2),
                From = 0,
                To = 212,
                RepeatBehavior = RepeatBehavior.Forever
            });
        }
        private void ResetSignalWavesAnimation()
        {
            PathOriginalSignal.RenderTransform.BeginAnimation(TranslateTransform.XProperty, null);
            PathDeviceSignal.RenderTransform.BeginAnimation(TranslateTransform.XProperty, null);
        }
        private void FinishRound(int ResultCode)
        {
            DefuseTimer.Stop();
            UsbCheckTimer.Stop();
            StDeviceCode.IsEnabled = false;
            MiscUtilities.soundPlayers["active_device_sound"].Stop();
            MiscUtilities.soundPlayers["active_device_last_seconds_sound"].Stop();
            ResetSignalWavesAnimation();
            var PlayPage = (PlayPage)PlayPageGrid.Parent;
            PlayPage.KeyDown -= PlayPage_KeyDown;
            
            RoundResultControl roundResultControl = new RoundResultControl(PlayPageGrid);
            roundResultControl.roundResultCode = ResultCode;

            ContentGridBitmapBlur.BeginAnimation(BlurBitmapEffect.RadiusProperty, new DoubleAnimation()
            {
                From = 0,
                To = 50,
                Duration = TimeSpan.FromSeconds(1),
                EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut }
            });
            roundResultControl.BeginAnimation(OpacityProperty, new DoubleAnimation()
            {
                From = 0,
                To = 1,
                BeginTime = TimeSpan.FromSeconds(1),
                Duration = TimeSpan.FromSeconds(1)
            });
            PlayPageGrid.Children.Add(roundResultControl);
        }
        private void DigitBlock_GotFocus(object sender, RoutedEventArgs e)
        {
            DigitBlockIndex = StDeviceCode.Children.IndexOf((TextBox)sender);
        }
        private void SetFocusOnDigitBlock()
        {
            foreach (var Child in StDeviceCode.Children)
            {
                if (Child is TextBox)
                {
                    TextBox CodeBox = (TextBox)Child;
                    if (CodeBox.IsEnabled == true)
                    {
                        CodeBox.Focus();
                        break;
                    }
                }
            }
        }
        private void PlayPage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (PauseActive == false)
                {
                    PauseRound();
                }
                else
                {
                    ContinueRound();
                }
            }
        }
        private void RoundPauseControl_ContinueRound(object sender, EventArgs e)
        {
            ContinueRound();
        }
        private void RoundPauseControl_ExitToMenu(object sender, EventArgs e)
        {
            var PlayPage = PlayPageGrid.Parent as PlayPage;
            PlayPage.NavigationService.Navigate(new MenuPage());
        }
        private void ContinueRound()
        {
            PauseActive = false;
            DefuseTimer.Start();
            if (UseUSBDevice == 1)
            {
                UsbCheckTimer.Start();
            }
            StDeviceCode.IsEnabled = true;
            StDeviceCode.Children[DigitBlockIndex].Focus();
            if (DefuseCountdown > 15)
            {
                MiscUtilities.soundPlayers["active_device_sound"].Play();
            }
            else
            {
                MiscUtilities.soundPlayers["active_device_last_seconds_sound"].Play();
            }
            SetSignalWavesAnimation();
        }
        private void PauseRound()
        {
            PauseActive = true;
            ResetSignalWavesAnimation();
            DefuseTimer.Stop();
            UsbCheckTimer.Stop();
            StDeviceCode.IsEnabled = false;
            MiscUtilities.soundPlayers["active_device_sound"].Pause();
            MiscUtilities.soundPlayers["active_device_last_seconds_sound"].Pause();
            RoundPauseControl roundPauseControl = new RoundPauseControl(PlayPageGrid);
            Panel.SetZIndex(roundPauseControl, 2);
            roundPauseControl.ContinueRound += RoundPauseControl_ContinueRound;
            roundPauseControl.ExitToMenu += RoundPauseControl_ExitToMenu;
            PlayPageGrid.Children.Add(roundPauseControl);
        }
    }
}
