using Defuser_Imitation.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Defuser_Imitation.Components.UserControls
{
    /// <summary>
    /// Логика взаимодействия для RoundResultControl.xaml
    /// </summary>
    public partial class RoundResultControl : UserControl
    {
        Grid PlayPageGrid;
        public int roundResultCode;
        public RoundResultControl(Grid playPageGrid)
        {
            InitializeComponent();
            PlayPageGrid = playPageGrid;
            Opacity = 0;

            MiscUtilities.soundPlayers["round_finish_sound"].Volume = Properties.Settings.Default.RoundVolume / 100f;
            MiscUtilities.soundPlayers["round_finish_sound"].Play();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            GridFirstOrbit.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, new DoubleAnimation
            {
                From = 0,
                To = 360,
                Duration = TimeSpan.FromSeconds(1),
                RepeatBehavior = RepeatBehavior.Forever,
                EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut }
            });
            GridSecondOrbit.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, new DoubleAnimation
            {
                From = 0,
                To = -360,
                Duration = TimeSpan.FromSeconds(1),
                RepeatBehavior = RepeatBehavior.Forever,
                EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut }
            });
            SolidColorBrush FirstOrbitColorResource = TryFindResource("FirstOrbitColor") as SolidColorBrush;
            SolidColorBrush SecondOrbitColorResource = TryFindResource("SecondOrbitColor") as SolidColorBrush;
            switch (roundResultCode)
            {
                case 0:
                    break;
                case 1:
                    SetOrbitAnimation();
                    TbWinnerTeam.Text = "ПОБЕДА АТАКИ";
                    TbRoundResultDescription.Text = "Устройство успешно сработало";
                    AttackerGlyph.Visibility = Visibility.Visible;
                    AttackerSphere.RenderTransform.SetValue(ScaleTransform.ScaleXProperty, 1.2);
                    AttackerSphere.RenderTransform.SetValue(ScaleTransform.ScaleYProperty, 1.2);
                    FirstOrbitColorResource.Color = Color.FromRgb(255, 255, 255);
                    AttackerPath.Fill = TryFindResource("ElementThemeColor") as SolidColorBrush;
                    break;
                case 2:
                    SetOrbitAnimation();
                    TbWinnerTeam.Text = "ПОБЕДА ЗАЩИТЫ";
                    TbRoundResultDescription.Text = "Команда атаки не успела установить устройство";
                    DefenderGlyph.Visibility = Visibility.Visible;
                    DefenderSphere.RenderTransform.SetValue(ScaleTransform.ScaleXProperty, 1.2);
                    DefenderSphere.RenderTransform.SetValue(ScaleTransform.ScaleYProperty, 1.2);
                    SecondOrbitColorResource.Color = Color.FromRgb(255, 255, 255);
                    DefenderPath.Fill = TryFindResource("ElementThemeColor") as SolidColorBrush;
                    break;
                case 3:
                    SetOrbitAnimation();
                    TbWinnerTeam.Text = "ПОБЕДА ЗАЩИТЫ";
                    TbRoundResultDescription.Text = "Команда защиты обезвредила устройство";
                    DefenderGlyph.Visibility = Visibility.Visible;
                    DefenderSphere.RenderTransform.SetValue(ScaleTransform.ScaleXProperty, 1.2);
                    DefenderSphere.RenderTransform.SetValue(ScaleTransform.ScaleYProperty, 1.2);
                    SecondOrbitColorResource.Color = Color.FromRgb(255, 255, 255);
                    DefenderPath.Fill = TryFindResource("ElementThemeColor") as SolidColorBrush;
                    break;
            }
        }
        private void SetOrbitAnimation()
        {
            GridFirstOrbit.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, new DoubleAnimation
            {
                From = 0,
                To = 360,
                Duration = TimeSpan.FromSeconds(15),
                RepeatBehavior = RepeatBehavior.Forever,
            });
            GridSecondOrbit.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, new DoubleAnimation
            {
                From = 0,
                To = -360,
                Duration = TimeSpan.FromSeconds(20),
                RepeatBehavior = RepeatBehavior.Forever,
            });
        }

        private void PlayAgainBtn_Click(object sender, RoutedEventArgs e)
        {
            MiscUtilities.soundPlayers["round_finish_sound"].Stop();
            var playPage = PlayPageGrid.Parent as PlayPage;
            playPage.NavigationService.Navigate(new PlayPage());
        }

        private void MenuBtn_Click(object sender, RoutedEventArgs e)
        {
            MiscUtilities.soundPlayers["round_finish_sound"].Stop();
            var playPage = PlayPageGrid.Parent as PlayPage;
            playPage.NavigationService.Navigate(new MenuPage());
        }
    }
}
