using Defuser_Imitation.Pages;
using Defuser_Imitation.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Defuser_Imitation.Components.UserControls
{
    /// <summary>
    /// Логика взаимодействия для PlantDeviceControl.xaml
    /// </summary>
    public partial class PlantDeviceControl : UserControl
    {
        private Grid PlayPageGrid;
        private DispatcherTimer RoundTimer = new DispatcherTimer();
        private DispatcherTimer MatrixAnimationTimer = new DispatcherTimer();

        private int RoundCountdown = Settings.Default.RoundCountdown;
        private string PlantCode = MiscUtilities.DeviceCode();
        private int ManualDigitCount = Settings.Default.ManualInputDigitsCount;

        private int[] ManualDigitBoxIndexArray = new int[Settings.Default.ManualInputDigitsCount];

        private int DigitBlockIndex = 0;
        private bool PauseActive = false;
        public PlantDeviceControl(Grid playPageGrid)
        {
            InitializeComponent();
            PlayPageGrid = playPageGrid;
            (PlayPageGrid.Parent as Page).KeyDown += PlayPage_KeyDown;

            SignalPb.Maximum = Settings.Default.RoundCountdown * 100;
            SignalPb.Value = SignalPb.Maximum;

            DrawMatrix();
            DisplayDeviceCodeBlock();

            SignalUnitAnimation();

            RoundTimer.Tick += RoundTimer_Tick;
            RoundTimer.Interval = TimeSpan.FromSeconds(1);
            RoundTimer.Start();

            MiscUtilities.soundPlayers["plant_stage_sound"].Volume = Properties.Settings.Default.RoundVolume / 100f;
            MiscUtilities.soundPlayers["plant_stage_last_seconds_sound"].Volume = Properties.Settings.Default.RoundVolume / 100f;

            MiscUtilities.soundPlayers["plant_stage_sound"].MediaEnded += RoundSoundPlayer_MediaEnded;
            MiscUtilities.soundPlayers["plant_stage_sound"].Play();
            
            MatrixAnimationTimer.Tick += MatrixAnimationTimer_Tick;
            MatrixAnimationTimer.Interval = TimeSpan.FromMilliseconds(10);
            MatrixAnimationTimer.Start();
        }

        private void RoundSoundPlayer_MediaEnded(object sender, EventArgs e)
        {
            if (RoundCountdown > 16)
            {
                MiscUtilities.soundPlayers["plant_stage_sound"].Play();
            }
        }
        private void MatrixAnimationTimer_Tick(object sender, EventArgs e)
        {
            MatrixDigitAnimation();
        }
        private void RoundTimer_Tick(object sender, EventArgs e)
        {
            if (RoundCountdown == 16)
            {
                MiscUtilities.soundPlayers["plant_stage_sound"].Stop();
                MiscUtilities.soundPlayers["plant_stage_last_seconds_sound"].Play();
            }
            if (RoundCountdown != 1)
            {
                SignalUnitAnimation();
                RoundCountdown--;
            }
            else
            {
                SignalUnitAnimation();
                FinishRound(2);
            }
        }

        private void DisplayDeviceCodeBlock()
        {
            int[] DigitArray = new int[8] { 0, 1, 2, 3, 4, 5, 6, 7 };
            ShuffleArrayClass.Shuffle(DigitArray);
            Array.Resize(ref DigitArray, ManualDigitCount);

            char[] CodeCharArray = PlantCode.ToCharArray();
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

                TextBox DigitBox = child as TextBox;

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
                SolidColorBrush LocalColorResource = TryFindResource("LocalColor") as SolidColorBrush;
                LocalColorResource.Color = Color.FromRgb(60, 152, 149);
                UIElement CodeBox = sender as UIElement;
                if ((CodeBox as TextBox).Text.Length == 0)
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
            }
        }
        private void DigitBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UIElement CodeBox = sender as UIElement;
            if ((CodeBox as TextBox).Text.Length > 0)
            {
                string InputedCode = "";
                foreach (UIElement CodeBlockChild in StDeviceCode.Children)
                {
                    if (CodeBlockChild is TextBox)
                    {
                        InputedCode += (CodeBlockChild as TextBox).Text;
                    }
                }
                if (InputedCode.Length == 8)
                {
                    if (InputedCode == PlantCode)
                    {
                        var PlayPage = PlayPageGrid.Parent as PlayPage;
                        PlayPage.KeyDown -= PlayPage_KeyDown;
                        RoundTimer.Stop();
                        MatrixAnimationTimer.Stop();
                        MiscUtilities.soundPlayers["plant_stage_sound"].Stop();
                        MiscUtilities.soundPlayers["plant_stage_last_seconds_sound"].Stop();
                        if (PlayPageGrid != null && PlayPageGrid.Children.Contains(this))
                        {
                            PlayPageGrid.Children.Remove(this);
                        }
                        DefuseDeviceControl defuseDeviceControl = new DefuseDeviceControl(PlayPageGrid);
                        PlayPageGrid.Children.Add(defuseDeviceControl);
                        return;
                    }
                    else
                    {
                        SolidColorBrush LocalColorResource = TryFindResource("LocalColor") as SolidColorBrush;
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
            int CurrentRowIndex = 0;
            int CurrentColumnIndex = 0;
            Random random = new Random();
            for (int i = 0; i < 40; i++)
            {
                TextBlock DigitBlock = new TextBlock();
                DigitBlock.Text = random.Next(0, 10).ToString();
                DigitBlock.Foreground = new SolidColorBrush(Color.FromRgb(60, 152, 149));
                DigitBlock.Background = new SolidColorBrush(Color.FromRgb(60, 152, 149));
                DigitBlock.Background.Opacity = 0;
                DigitBlock.Width = 40 / 5;
                DigitBlock.Focusable = false;
                DigitBlock.FontFamily = (FontFamily)FindResource("DigitFont");
                DigitBlock.Margin = new Thickness(4, 0, 4, 0);
                DigitBlock.TextAlignment = TextAlignment.Center;
                DigitBlock.HorizontalAlignment = HorizontalAlignment.Center;
                DigitBlock.VerticalAlignment = VerticalAlignment.Center;
                if (i < 10)
                {
                    CurrentRowIndex = 0;
                }
                else if (i >= 10 && i < 20)
                {
                    CurrentRowIndex = 1;
                }
                else if (i >= 20 && i < 30)
                {
                    CurrentRowIndex = 2;
                }
                else if (i >= 30 && i < 40)
                {
                    CurrentRowIndex = 3;
                }
                Grid.SetRow(DigitBlock, CurrentRowIndex);
                Grid.SetColumn(DigitBlock, CurrentColumnIndex);
                MatrixGrid.Children.Add(DigitBlock);

                CurrentColumnIndex++;
                if (CurrentColumnIndex == 10)
                {
                    CurrentColumnIndex = 0;
                }
            }
        }
        private void MatrixDigitAnimation()
        {
            if (MatrixGrid.Children.Count == 40)
            {
                Random random = new Random();
                TextBlock DigitBlock = (TextBlock)MatrixGrid.Children[random.Next(0, 40)];
                DigitBlock.Background.BeginAnimation(Brush.OpacityProperty, new DoubleAnimation
                {
                    From = 0,
                    To = 1,
                    AutoReverse = true,
                    Duration = TimeSpan.FromSeconds(0.1)
                });
                DigitBlock.Text = random.Next(0, 10).ToString();
            }
        }
        private void FinishMatrixAnimation()
        {
            if (MatrixGrid.Children.Count == 40)
            {
                for (int i = 0; i < 40; i++)
                {
                    TextBlock DigitBlock = (TextBlock)MatrixGrid.Children[i];

                    DigitBlock.Background.BeginAnimation(Brush.OpacityProperty, new DoubleAnimation
                    {
                        From = 1,
                        To = 0,
                        Duration = TimeSpan.FromSeconds(0.2)
                    });
                    DigitBlock.Text = "0";
                }
            }
        }
        private void FinishRound(int ResultCode)
        {
            var PlayPage = PlayPageGrid.Parent as PlayPage;
            PlayPage.KeyDown -= PlayPage_KeyDown;
            MiscUtilities.soundPlayers["plant_stage_sound"].Stop();
            MiscUtilities.soundPlayers["plant_stage_last_seconds_sound"].Stop();
            StDeviceCode.IsEnabled = false;
            RoundTimer.Stop();
            MatrixAnimationTimer.Stop();
            FinishMatrixAnimation();

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
            DigitBlockIndex = StDeviceCode.Children.IndexOf(sender as TextBox);
        }
        private void SetFocusOnDigitBlock()
        {
            foreach (var Child in StDeviceCode.Children)
            {
                if (Child is TextBox)
                {
                    TextBox CodeBox = Child as TextBox;
                    if (CodeBox.IsEnabled == true)
                    {
                        CodeBox.Focus();
                        break;
                    }
                }
            }
        }
        private void PlantDeviceControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetFocusOnDigitBlock();
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
            var PlayPage = PlayPageGrid.Parent as Page;
            PlayPage.NavigationService.Navigate(new MenuPage());
        }
        private void ContinueRound()
        {
            PauseActive = false;
            RoundTimer.Start();
            MatrixAnimationTimer.Start();
            StDeviceCode.IsEnabled = true;
            StDeviceCode.Children[DigitBlockIndex].Focus();
            if (RoundCountdown > 16)
            {
                MiscUtilities.soundPlayers["plant_stage_sound"].Play();
            }
            else
            {
                MiscUtilities.soundPlayers["plant_stage_last_seconds_sound"].Play();
            }
        }
        private void PauseRound()
        {
            PauseActive = true;
            RoundTimer.Stop();
            MatrixAnimationTimer.Stop();
            StDeviceCode.IsEnabled = false;
            MiscUtilities.soundPlayers["plant_stage_sound"].Pause();
            MiscUtilities.soundPlayers["plant_stage_last_seconds_sound"].Pause();
            RoundPauseControl roundPauseControl = new RoundPauseControl(PlayPageGrid);
            Panel.SetZIndex(roundPauseControl, 2);
            roundPauseControl.ContinueRound += RoundPauseControl_ContinueRound;
            roundPauseControl.ExitToMenu += RoundPauseControl_ExitToMenu;
            PlayPageGrid.Children.Add(roundPauseControl);
        }
    }
}
