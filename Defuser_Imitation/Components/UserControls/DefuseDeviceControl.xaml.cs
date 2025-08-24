using Defuser_Imitation.Pages;
using Defuser_Imitation.Properties;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Timer = System.Timers.Timer;

namespace Defuser_Imitation.Components.UserControls
{
    /// <summary>
    /// Логика взаимодействия для DefuseDeviceControl.xaml
    /// </summary>
    public partial class DefuseDeviceControl : UserControl
    {
        private Grid _playPageGrid;
        private DispatcherTimer deactivationPhaseTimer = new DispatcherTimer();
        private Timer usbCheckTimer = new Timer();

        private int deactivationPhaseCountdown = Settings.Default.DeactivationPhaseCountdown;
        private string defuseCode = MiscUtilities.DeviceCode();
        private int manualDigitCount = Settings.Default.ManualInputDigitsCount;
        private int useUSBDevice = Settings.Default.UseUSBDevice;
        private string usbDeviceName = Settings.Default.USBDeviceName;

        private string inputedCode = string.Empty;
        private int digitBlockIndex = 0;
        private int[] _manualDigitBoxIndexArray;
        private bool pauseActive = false;

        private List<int> indicatorActiveBlockList = new();
        private List<int> indicatorDeactiveBlockList = new();

        public DefuseDeviceControl(Grid playPageGrid, int[] manualDigitBoxIndexArray)
        {
            InitializeComponent();
            _manualDigitBoxIndexArray = manualDigitBoxIndexArray;
            _playPageGrid = playPageGrid;
            ((Page)playPageGrid.Parent).KeyDown += PlayPage_KeyDown;

            MiscUtilities.SoundPlayers["defuse_stage_sound"].MediaEnded += ActiveDeviceSoundPlayer_MediaEnded;

            DrawIndicatorElements();
            DisplayDeviceCodeBlock();

            deactivationPhaseTimer.Tick += deactivationPhaseTimer_Tick;
            deactivationPhaseTimer.Interval = TimeSpan.FromSeconds(1);
            TbCountdown.Text = deactivationPhaseCountdown.ToString();   
        }
        private void DefuseDeviceControl_Loaded(object sender, RoutedEventArgs e)
        {  
            SetOriginalSignal();
            SetDeviceSignal();
            FocusFirstDigitBlock();
            deactivationPhaseTimer.Start();
            if (useUSBDevice == 1)
            {
                usbCheckTimer.Interval = 300;
                usbCheckTimer.Elapsed += USBCheckTimer_Elapsed;
                usbCheckTimer.Enabled = true;
            }
            MiscUtilities.SoundPlayers["defuse_stage_sound"].Play();
        }
        private void ActiveDeviceSoundPlayer_MediaEnded(object sender, EventArgs e)
        {
            if (deactivationPhaseCountdown > 15)
            {
                MiscUtilities.SoundPlayers["defuse_stage_sound"].Position = TimeSpan.FromSeconds(0);
            }
            else
            {
                if (MiscUtilities.SoundPlayers["defuse_stage_sound"].NaturalDuration < TimeSpan.FromSeconds(15))
                {
                    MiscUtilities.SoundPlayers["defuse_stage_sound"].Position = TimeSpan.FromSeconds(0);
                }
                else
                {
                    MiscUtilities.SoundPlayers["defuse_stage_sound"].Position = MiscUtilities.SoundPlayers["defuse_stage_sound"].NaturalDuration.TimeSpan - TimeSpan.FromSeconds(deactivationPhaseCountdown);
                }
            }
        }
        private void deactivationPhaseTimer_Tick(object sender, EventArgs e)
        {
            if (deactivationPhaseCountdown == 15 && MiscUtilities.SoundPlayers["defuse_stage_sound"].Position < MiscUtilities.SoundPlayers["defuse_stage_sound"].NaturalDuration - TimeSpan.FromSeconds(15))
            {
                if (MiscUtilities.SoundPlayers["defuse_stage_sound"].NaturalDuration < TimeSpan.FromSeconds(15))
                {
                    MiscUtilities.SoundPlayers["defuse_stage_sound"].Position = TimeSpan.FromSeconds(0);
                }
                else
                {
                    MiscUtilities.SoundPlayers["defuse_stage_sound"].Position = MiscUtilities.SoundPlayers["defuse_stage_sound"].NaturalDuration.TimeSpan - TimeSpan.FromSeconds(deactivationPhaseCountdown);
                }
            }
            if (deactivationPhaseCountdown != 1)
            {
                deactivationPhaseCountdown--;
                TbCountdown.Text = deactivationPhaseCountdown.ToString();
            }
            else
            {
                TbCountdown.Text = "0";
                FinishRound(1);
            }
        }
        private void USBCheckTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (MiscUtilities.CheckUSBDevice(usbDeviceName) == true)
            {
                usbCheckTimer.Stop();
                Application.Current.Dispatcher.Invoke(() => FinishRound(3));
            }
        }
        private void DrawIndicatorElements()
        {
            IndicatorGrid.Children.Clear();
            int currentRowIndex = 0;
            int currentColumnIndex = 0;
            for (int i = 0; i < 27; i++)
            {
                Rectangle indicatorBlock = new Rectangle();
                indicatorBlock.Fill = new SolidColorBrush(Color.FromRgb(16, 49, 62));
                indicatorBlock.Focusable = false;
                indicatorBlock.Margin = new Thickness(4, 4, 4, 4);
                if (i < 9)
                {
                    currentRowIndex = 0;
                }
                else if (i >= 9 && i < 18)
                {
                    currentRowIndex = 1;
                }
                else if (i >= 18 && i < 27)
                {
                    currentRowIndex = 2;
                }
                Grid.SetRow(indicatorBlock, currentRowIndex);
                Grid.SetColumn(indicatorBlock, currentColumnIndex);
                IndicatorGrid.Children.Add(indicatorBlock);
                indicatorDeactiveBlockList.Add(i);

                currentColumnIndex++;
                if (currentColumnIndex == 9)
                {
                    currentColumnIndex = 0;
                }
            }
            ShuffleArrayClass.Shuffle(indicatorDeactiveBlockList);
            for (int indicatorIndex = 0; indicatorIndex < defuseCode.Length - manualDigitCount; indicatorIndex++)
            {
                ActivateIndicators();
            }
        }
        private void ActivateIndicators()
        {
            if (indicatorDeactiveBlockList.Count == 0)
            {
                return;
            }
            for (int i = 0; i < 3; i++)
            {
                var indicatorDeactiveListElement = indicatorDeactiveBlockList[0];
                Rectangle gridChild = (Rectangle)IndicatorGrid.Children[indicatorDeactiveListElement];
                gridChild.Fill.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation()
                {
                    To = Color.FromRgb(1, 255, 229),
                    Duration = TimeSpan.FromSeconds(0.3),
                    BeginTime = TimeSpan.FromSeconds(i * 0.1)
                });
                indicatorActiveBlockList.Add(indicatorDeactiveListElement);
                indicatorDeactiveBlockList.Remove(indicatorDeactiveListElement);
            } 
        }
        private void DeactivateIndicators()
        {
            if (indicatorActiveBlockList.Count > 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    var indicatorActiveListElement = indicatorActiveBlockList[0];
                    Rectangle indicatorCell = (Rectangle)IndicatorGrid.Children[indicatorActiveListElement];
                    indicatorCell.Fill.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation()
                    {
                        To = Color.FromRgb(16, 49, 62),
                        Duration = TimeSpan.FromSeconds(0.3),
                        BeginTime = TimeSpan.FromSeconds(i * 0.1)
                    });
                    indicatorDeactiveBlockList.Add(indicatorActiveListElement);
                    indicatorActiveBlockList.Remove(indicatorActiveListElement);
                }
            }
        }
        private void DisplayDeviceCodeBlock()
        {
            char[] defuseCodeCharArray = defuseCode.ToCharArray();
            int currentDigitBlockIndex = 0;
            foreach (UIElement child in StDeviceCode.Children)
            {
                if (child is TextBlock)
                {
                    continue;
                }
                else if (child is TextBox)
                {
                    TextBox digitBlock = (TextBox)child;
                    if (_manualDigitBoxIndexArray.Contains(currentDigitBlockIndex))
                    {
                        digitBlock.IsEnabled = true;
                        digitBlock.TextChanged += DigitBox_TextChanged;
                        digitBlock.PreviewKeyDown += DigitBox_PreviewKeyDown;
                        digitBlock.PreviewTextInput += DigitBox_PreviewTextInput;
                        digitBlock.GotFocus += DigitBlock_GotFocus;
                        currentDigitBlockIndex++;
                        continue;
                    }
                    digitBlock.Text = defuseCodeCharArray[currentDigitBlockIndex].ToString();
                    digitBlock.IsEnabled = false;
                    currentDigitBlockIndex++;
                }
            } 
        }
        private void FocusFirstDigitBlock()
        {
            foreach (UIElement child in StDeviceCode.Children)
            {
                if (child is TextBox)
                {
                    TextBox digitBlock = (TextBox)child;
                    if (digitBlock.IsEnabled == true)
                    {
                        digitBlock.Focus();
                        break;
                    }
                }
            }
        }
        private void FocusNextDigitBlock(UIElement currentDigitBlock)
        {
            int elementIndex = StDeviceCode.Children.IndexOf(currentDigitBlock);
            for (int i = elementIndex + 1; i < StDeviceCode.Children.Count; i++)
            {
                UIElement iterationChild = StDeviceCode.Children[i];
                if (iterationChild is not TextBox)
                {
                    continue;
                }
                TextBox digitBlock = (TextBox)iterationChild;
                if (digitBlock.IsEnabled == true)
                {
                    digitBlock.Focus();
                    break;
                }
            }
        }
        private void FocusPreviousDigitBlock(UIElement currentDigitBlock)
        {
            int elementIndex = StDeviceCode.Children.IndexOf(currentDigitBlock);
            for (int i = elementIndex - 1; i > -1; i--)
            {
                UIElement iterationChild = StDeviceCode.Children[i];
                if (iterationChild is not TextBox)
                {
                    continue;
                }
                TextBox digitBlock = (TextBox)iterationChild;
                if (digitBlock.IsEnabled == true)
                {
                    digitBlock.Focus();
                    break;
                }
            }
        }
        private void DigitBlock_GotFocus(object sender, RoutedEventArgs e)
        {
            digitBlockIndex = StDeviceCode.Children.IndexOf((TextBox)sender);
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
                SolidColorBrush localColorResource = (SolidColorBrush)TryFindResource("LocalColor");
                localColorResource.Color = MiscUtilities.DefaultColor;
                if (string.IsNullOrWhiteSpace(((TextBox)sender).Text))
                {
                    FocusPreviousDigitBlock((UIElement)sender);
                }
            }
        }
        private void DigitBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            inputedCode = MiscUtilities.SetDeviceCode(StDeviceCode);
            SetDeviceSignal();
            TextBox digitBlock = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(digitBlock.Text))
            {
                DeactivateIndicators();
            }
            else
            {
                ActivateIndicators();
                if (inputedCode.Length == 8)
                {
                    if (inputedCode == defuseCode)
                    {
                        ActivateIndicators();
                        FinishRound(3);
                        return;
                    }
                    SolidColorBrush localColorResource = (SolidColorBrush)TryFindResource("LocalColor");
                    localColorResource.Color = MiscUtilities.RedColor;
                    return;
                }
                FocusNextDigitBlock(digitBlock);
            }
        }
        private void SetOriginalSignal()
        {
            int settingsDeviceCodeSumm = 0;
            for (int i = 0; i < defuseCode.Length; i++)
            {
                settingsDeviceCodeSumm += int.Parse(defuseCode[i].ToString()) * (i + 1) / defuseCode.Length;
            }
            double amplitude = settingsDeviceCodeSumm / 3 / 1.7;
            double frequency = 0.15;
            for (int x = 0; x < 200; x++)
            {
                double y = amplitude * Math.Sin(frequency * x);
                OriginalSignalFigure.Segments.Add(new LineSegment(new Point(x, y), true));
                DeviceSignalFigure.Segments.Add(new LineSegment(new Point(x, y), true));
            }
        }
        private void SetDeviceSignal()
        {
            double defuseCodeSumm = 0;
            for (int i = 0; i < inputedCode.Length; i++)
            {
                defuseCodeSumm += int.Parse(inputedCode[i].ToString()) * (i + 1) / inputedCode.Length;
            }
            double amplitude = defuseCodeSumm / 3 / 1.7;
            double frequency = 0.15;
            foreach (LineSegment lineSegment in DeviceSignalFigure.Segments.Cast<LineSegment>())
            {
                Point point = lineSegment.Point;
                double y = amplitude * Math.Sin(frequency * point.X);
                point.Y = y; 
                lineSegment.BeginAnimation(LineSegment.PointProperty, new PointAnimation()
                {
                    To = point,
                    Duration = TimeSpan.FromSeconds(1),
                    EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut }
                });
            }
        }
        private void FinishRound(int ResultCode)
        {
            deactivationPhaseTimer.Stop();
            usbCheckTimer.Stop();
            StDeviceCode.IsEnabled = false;
            MiscUtilities.SoundPlayers["defuse_stage_sound"].Stop();
            var playPage = (PlayPage)_playPageGrid.Parent;
            playPage.KeyDown -= PlayPage_KeyDown;
            
            RoundResultControl roundResultControl = new RoundResultControl(_playPageGrid);
            roundResultControl.roundResultCode = ResultCode;
            roundResultControl.BeginAnimation(OpacityProperty, new DoubleAnimation()
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(1)
            });
            _playPageGrid.Children.Add(roundResultControl);
        }   
        private void PlayPage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (pauseActive == false)
                {
                    PauseRound();
                }
            }
        }
        private void RoundPauseControl_ContinueRound(object sender, EventArgs e)
        {
            ContinueRound();
        }
        private void RoundPauseControl_ExitToMenu(object sender, EventArgs e)
        {
            var playPage = (PlayPage)_playPageGrid.Parent;
            if (playPage != null)
            {
                playPage.NavigationService.Navigate(new MenuPage());
            }
        }
        private void ContinueRound()
        {
            pauseActive = false;
            deactivationPhaseTimer.Start();
            if (useUSBDevice == 1)
            {
                usbCheckTimer.Start();
            }
            StDeviceCode.IsEnabled = true;
            StDeviceCode.Children[digitBlockIndex].Focus();
            MiscUtilities.SoundPlayers["defuse_stage_sound"].Play();
        }
        private void PauseRound()
        {
            pauseActive = true;
            deactivationPhaseTimer.Stop();
            usbCheckTimer.Stop();
            StDeviceCode.IsEnabled = false;
            MiscUtilities.SoundPlayers["defuse_stage_sound"].Pause();
            RoundPauseControl roundPauseControl = new RoundPauseControl(_playPageGrid);
            Panel.SetZIndex(roundPauseControl, 2);
            roundPauseControl.ContinueRound += RoundPauseControl_ContinueRound;
            roundPauseControl.ExitToMenu += RoundPauseControl_ExitToMenu;
            _playPageGrid.Children.Add(roundPauseControl);
        }
    }
}
