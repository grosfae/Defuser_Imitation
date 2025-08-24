using Defuser_Imitation.Pages;
using Defuser_Imitation.Properties;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;


namespace Defuser_Imitation.Components.UserControls
{
    /// <summary>
    /// Логика взаимодействия для PlantDeviceControl.xaml
    /// </summary>
    public partial class PlantDeviceControl : UserControl
    {
        private Grid _playPageGrid;
        private CountdownControl? countdownControl;
        private DispatcherTimer activationPhaseTimer = new DispatcherTimer();
        private DispatcherTimer matrixAnimationTimer = new DispatcherTimer();

        private int activationPhaseCountdown = Settings.Default.ActivationPhaseCountdown;
        private string activationCode = MiscUtilities.DeviceCode();
        private int manualDigitCount = Settings.Default.ManualInputDigitsCount;

        private int[] manualDigitBoxIndexArray = new int[Settings.Default.ManualInputDigitsCount];

        private int digitBlockIndex = 0;
        private bool pauseActive = false;
        public PlantDeviceControl(Grid playPageGrid)
        {
            InitializeComponent();
            _playPageGrid = playPageGrid;
            ((Page)_playPageGrid.Parent).KeyDown += PlayPage_KeyDown;

            SignalPb.Maximum = Settings.Default.ActivationPhaseCountdown * 100;
            SignalPb.Value = SignalPb.Maximum;

            DrawMatrix();
            DisplayDeviceCodeBlock();

            activationPhaseTimer.Tick += activationPhaseTimer_Tick;
            activationPhaseTimer.Interval = TimeSpan.FromSeconds(1);           

            MiscUtilities.SoundPlayers["plant_stage_sound"].MediaEnded += RoundSoundPlayer_MediaEnded;

            matrixAnimationTimer.Tick += MatrixAnimationTimer_Tick;
            matrixAnimationTimer.Interval = TimeSpan.FromMilliseconds(50);
        }

        private void PlantDeviceControl_Loaded(object sender, RoutedEventArgs e)
        {
            int countdownToStart = Settings.Default.PreparationPhaseCountdown;
            if (countdownToStart > 0)
            {
                StDeviceCode.PreviewKeyDown += DisableKeyboard_PreviewKeyDown;
                countdownControl = new CountdownControl(_playPageGrid, countdownToStart);
                countdownControl.StartRound += CountdownControl_StartRound;
                _playPageGrid.Children.Add(countdownControl);              
            }
            else
            {
                activationPhaseTimer.Start();
                matrixAnimationTimer.Start();
                MiscUtilities.SoundPlayers["plant_stage_sound"].Play();
                SignalUnitAnimation();
            }
            FocusFirstDigitBlock();
        }
        private void DisableKeyboard_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            List<Key> keys = new List<Key>() {Key.Escape, Key.System, Key.LeftAlt, Key.RightAlt, Key.F4, Key.OemBackTab};
            if (!keys.Contains(e.Key))
            {
                e.Handled = true;
            }
        }
        private void CountdownControl_StartRound(object? sender, EventArgs e)
        {
            countdownControl = null;
            StDeviceCode.PreviewKeyDown -= DisableKeyboard_PreviewKeyDown;
            activationPhaseTimer.Start();
            matrixAnimationTimer.Start();
            MiscUtilities.SoundPlayers["plant_stage_sound"].Play();
            SignalUnitAnimation();
        }

        private void RoundSoundPlayer_MediaEnded(object sender, EventArgs e)
        {
            if (activationPhaseCountdown > 15)
            {
                MiscUtilities.SoundPlayers["plant_stage_sound"].Position = TimeSpan.FromSeconds(0);
            }
            else
            {
                if (MiscUtilities.SoundPlayers["plant_stage_sound"].NaturalDuration < TimeSpan.FromSeconds(15))
                {
                    MiscUtilities.SoundPlayers["plant_stage_sound"].Position = TimeSpan.FromSeconds(0);
                }
                else
                {
                    MiscUtilities.SoundPlayers["plant_stage_sound"].Position = MiscUtilities.SoundPlayers["plant_stage_sound"].NaturalDuration.TimeSpan - TimeSpan.FromSeconds(activationPhaseCountdown);
                }
            }
        }
        private void MatrixAnimationTimer_Tick(object sender, EventArgs e)
        {
            MatrixDigitAnimation();
        }
        private void activationPhaseTimer_Tick(object sender, EventArgs e)
        {
            if (activationPhaseCountdown == 15 && MiscUtilities.SoundPlayers["plant_stage_sound"].Position < MiscUtilities.SoundPlayers["plant_stage_sound"].NaturalDuration - TimeSpan.FromSeconds(15))
            {
                if (MiscUtilities.SoundPlayers["plant_stage_sound"].NaturalDuration < TimeSpan.FromSeconds(15))
                {
                    MiscUtilities.SoundPlayers["plant_stage_sound"].Position = TimeSpan.FromSeconds(0);
                }
                else
                {
                    MiscUtilities.SoundPlayers["plant_stage_sound"].Position = MiscUtilities.SoundPlayers["plant_stage_sound"].NaturalDuration.TimeSpan - TimeSpan.FromSeconds(activationPhaseCountdown);
                }
            }
            if (activationPhaseCountdown != 1)
            {
                SignalUnitAnimation();
                activationPhaseCountdown--;
            }
            else
            {
                SignalUnitAnimation();
                FinishRound(2);
            }
        }
        private void DisplayDeviceCodeBlock()
        {
            int[] indexArray = new int[activationCode.Length];
            for (int i = 0; i < indexArray.Length; i++)
            {
                indexArray[i] = i;
            }
            indexArray.Shuffle();
            Array.Resize(ref indexArray, manualDigitCount);
            manualDigitBoxIndexArray = indexArray;
            char[] activationCodeCharArray = activationCode.ToCharArray();
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
                    if (indexArray.Contains(currentDigitBlockIndex))
                    {
                        digitBlock.IsEnabled = true;
                        digitBlock.TextChanged += DigitBox_TextChanged;
                        digitBlock.PreviewKeyDown += DigitBox_PreviewKeyDown;
                        digitBlock.PreviewTextInput += DigitBox_PreviewTextInput;
                        digitBlock.GotFocus += DigitBlock_GotFocus;
                        currentDigitBlockIndex++;
                        continue;
                    }
                    digitBlock.Text = activationCodeCharArray[currentDigitBlockIndex].ToString();
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
            digitBlockIndex = StDeviceCode.Children.IndexOf(sender as TextBox);
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
            string inputedCode = MiscUtilities.SetDeviceCode(StDeviceCode);

            TextBox digitBlock = (TextBox)sender;
            if (!string.IsNullOrWhiteSpace(digitBlock.Text))
            {
                if (inputedCode.Length == 8)
                {
                    if (inputedCode == activationCode)
                    {
                        StopTimers();
                        var playPage = (PlayPage)_playPageGrid.Parent;
                        playPage.KeyDown -= PlayPage_KeyDown;
                        if (_playPageGrid != null && _playPageGrid.Children.Contains(this))
                        {
                            _playPageGrid.Children.Remove(this);
                        }
                        DefuseDeviceControl defuseDeviceControl = new DefuseDeviceControl(_playPageGrid, manualDigitBoxIndexArray);
                        _playPageGrid.Children.Add(defuseDeviceControl);
                        return;
                    }
                    else
                    {
                        SolidColorBrush localColorResource = TryFindResource("LocalColor") as SolidColorBrush;
                        localColorResource.Color = MiscUtilities.RedColor;
                        return;
                    }
                }
                FocusNextDigitBlock(digitBlock);
            }
        }
        private void SignalUnitAnimation()
        {
            SignalPb.BeginAnimation(ProgressBar.ValueProperty, new DoubleAnimation
            {
                From = SignalPb.Value,
                To = SignalPb.Value - 100,
                Duration = TimeSpan.FromSeconds(1),
            });
        }
        private void DrawMatrix()
        {
            int currentRowIndex = 0;
            int currentColumnIndex = 0;
            Random random = new Random();
            for (int i = 0; i < 40; i++)
            {
                TextBlock matrixBlock = new TextBlock();
                matrixBlock.Text = random.Next(0, 10).ToString();
                matrixBlock.Foreground = new SolidColorBrush(Color.FromRgb(60, 152, 149));
                matrixBlock.Background = new SolidColorBrush(Color.FromRgb(60, 152, 149));
                matrixBlock.Background.Opacity = 0;
                matrixBlock.Width = 40 / 5;
                matrixBlock.Focusable = false;
                matrixBlock.FontFamily = (FontFamily)FindResource("DigitFont");
                matrixBlock.Margin = new Thickness(4, 0, 4, 0);
                matrixBlock.TextAlignment = TextAlignment.Center;
                matrixBlock.HorizontalAlignment = HorizontalAlignment.Center;
                matrixBlock.VerticalAlignment = VerticalAlignment.Center;
                if (i < 10)
                {
                    currentRowIndex = 0;
                }
                else if (i >= 10 && i < 20)
                {
                    currentRowIndex = 1;
                }
                else if (i >= 20 && i < 30)
                {
                    currentRowIndex = 2;
                }
                else if (i >= 30 && i < 40)
                {
                    currentRowIndex = 3;
                }
                Grid.SetRow(matrixBlock, currentRowIndex);
                Grid.SetColumn(matrixBlock, currentColumnIndex);
                MatrixGrid.Children.Add(matrixBlock);

                currentColumnIndex++;
                if (currentColumnIndex == 10)
                {
                    currentColumnIndex = 0;
                }
            }
        }
        private void MatrixDigitAnimation()
        {
            int matrixBlockCount = MatrixGrid.Children.Count;
            Random random = new Random();
            TextBlock matrixBlock = (TextBlock)MatrixGrid.Children[random.Next(0, matrixBlockCount)];
            matrixBlock.Background.BeginAnimation(Brush.OpacityProperty, new DoubleAnimation
            {
                From = 0,
                To = 1,
                AutoReverse = true,
                Duration = TimeSpan.FromSeconds(0.1)
            });
            matrixBlock.Text = random.Next(0, 10).ToString();
        }
        private void FinishMatrixAnimation()
        {
            if (MatrixGrid.Children.Count == 40)
            {
                for (int i = 0; i < 40; i++)
                {
                    TextBlock matrixBlock = (TextBlock)MatrixGrid.Children[i];
                    matrixBlock.Background.BeginAnimation(Brush.OpacityProperty, new DoubleAnimation
                    {
                        From = 1,
                        To = 0,
                        Duration = TimeSpan.FromSeconds(0.2)
                    });
                    matrixBlock.Text = "0";
                }
            }
        }
        private void StopTimers()
        {
            activationPhaseTimer.Stop();
            matrixAnimationTimer.Stop();
            MiscUtilities.SoundPlayers["plant_stage_sound"].Stop();
        }
        private void FinishRound(int ResultCode)
        {
            var playPage = (PlayPage)_playPageGrid.Parent;
            playPage.KeyDown -= PlayPage_KeyDown;
            StDeviceCode.IsEnabled = false;
            StopTimers();
            FinishMatrixAnimation();
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
            StopTimers();
            var playPage = (Page)_playPageGrid.Parent;
            playPage.NavigationService.Navigate(new MenuPage());
        }
        private void ContinueRound()
        {
            pauseActive = false;
            if (countdownControl != null)
            {
                countdownControl.ContinueCountdown();
            }
            else
            {
                activationPhaseTimer.Start();
                matrixAnimationTimer.Start();   
                MiscUtilities.SoundPlayers["plant_stage_sound"].Play();
            }
            StDeviceCode.IsEnabled = true;
            StDeviceCode.Children[digitBlockIndex].Focus();
        }
        private void PauseRound()
        {
            pauseActive = true;
            if (countdownControl != null)
            {
                countdownControl.PauseCountdown();
            }
            activationPhaseTimer.Stop();
            matrixAnimationTimer.Stop();
            MiscUtilities.SoundPlayers["plant_stage_sound"].Pause();
            StDeviceCode.IsEnabled = false;
            RoundPauseControl roundPauseControl = new RoundPauseControl(_playPageGrid);
            Panel.SetZIndex(roundPauseControl, 3);
            roundPauseControl.ContinueRound += RoundPauseControl_ContinueRound;
            roundPauseControl.ExitToMenu += RoundPauseControl_ExitToMenu;
            _playPageGrid.Children.Add(roundPauseControl);
        }
    }
}
