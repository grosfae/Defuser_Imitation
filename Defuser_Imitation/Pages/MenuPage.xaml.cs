using Defuser_Imitation.Components;
using Defuser_Imitation.Components.UserControls;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace Defuser_Imitation.Pages
{
    /// <summary>
    /// Логика взаимодействия для MenuPage.xaml
    /// </summary>
    public partial class MenuPage : Page
    {
        private bool timerAvailable;
        public MenuPage()
        {
            InitializeComponent();
            TbTime.Text = DateTime.Now.ToString("t");
            TbDate.Text = DateTime.Now.ToString("D");
            timerAvailable = true;
            Time_Set();
            SetSearchUpdateText();
            Task.Run(GitHubCheckUpdateAsync);
        }
        private async Task GitHubCheckUpdateAsync()
        {
            try
            {
                await App.gitHubUpdater.CheckUpdate();
                if (App.gitHubUpdater._canUpdate == true)
                {
                    App.Current.Dispatcher.Invoke(SetNewUpdateText);
                    return;
                }
            }
            catch (Exception exception)
            {
                App.Current.Dispatcher.Invoke(SetDefaultUpdateText);
                LoggerSerivce.Write(exception);
            }
            App.Current.Dispatcher.Invoke(SetDefaultUpdateText);
        }
        private async Task GitHubInstallUpdateAsync()
        {
            try
            {
                await App.gitHubUpdater.CheckUpdate();
                if (App.gitHubUpdater._canUpdate == true)
                {
                    App.Current.Dispatcher.Invoke(SetSearchUpdateText);
                    App.gitHubUpdater.OpenUpdater();
                    return;
                }
            }
            catch (Exception exception)
            {
                App.Current.Dispatcher.Invoke(SetDefaultUpdateText);
                LoggerSerivce.Write(exception);
            }
        }
        private void SetSearchUpdateText()
        {
            string updateDefaultText = "Пожалуйста, подождите".ToUpper();
            string updateVersionDefaultText = "Поиск обновлений...".ToUpper();
            TbUpdateText.Text = updateDefaultText;
            TbUpdateVersion.Text = updateVersionDefaultText;
        }
        private void SetDefaultUpdateText()
        {
            string updateDefaultText = "Установлена последняя версия".ToUpper();
            string updateVersionDefaultText = "Текущая версия: ".ToUpper() + App.gitHubUpdater._currentVersion;
            TbUpdateText.Text = updateDefaultText;
            TbUpdateVersion.Text = updateVersionDefaultText;
        }
        private void SetNewUpdateText()
        {
            string updateDefaultText = "Доступно обновление".ToUpper();
            string updateVersionDefaultText = "Новая версия: ".ToUpper() + App.gitHubUpdater._lastestGitHubVersion;
            TbUpdateText.Text = updateDefaultText;
            TbUpdateVersion.Text = updateVersionDefaultText;
        }
        private void CreateMapSpot()
        {
            Random random = new Random();
            MapSpotControl mapSpotControl = new MapSpotControl();
            Canvas.SetLeft(mapSpotControl, random.Next(15, 1800));
            Canvas.SetTop(mapSpotControl, random.Next(50, 900));
            MapCanvas.Children.Add(mapSpotControl);
        }
        private async void Time_Set()
        {
            while (timerAvailable == true)
            {
                CreateMapSpot();
                DateTime now = DateTime.Now;
                int millisecondsUntilNextSecond = 1000 - now.Millisecond;
                await Task.Delay(millisecondsUntilNextSecond);
                TbTime.Text = DateTime.Now.ToString("t");
                TbDate.Text = DateTime.Now.ToString("D");
            }
        }
        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            timerAvailable = false;
            NavigationService.Navigate(new PlayPage());
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            timerAvailable = false;
            App.Current.Shutdown();
        }
        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            SettingsControl SettingsWindow = new SettingsControl();
            Panel.SetZIndex(SettingsWindow, 3);
            Grid.SetColumnSpan(SettingsWindow, 2);
            SettingsWindow.Opacity = 0;
            PageGrid.Children.Add(SettingsWindow);
            SettingsWindow.BeginAnimation(OpacityProperty, new DoubleAnimation()
            {
                Duration = TimeSpan.FromSeconds(0.2),
                To = 1,
            });
        }
        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            SetSearchUpdateText();
            Task.Run(() => GitHubInstallUpdateAsync());
        }
        private void GitHubBtn_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/grosfae?tab=repositories");
        }
        private void GuideBtn_Click(object sender, RoutedEventArgs e)
        {
            GuideControl guideControl = new GuideControl();
            Panel.SetZIndex(guideControl, 3);
            Grid.SetColumnSpan(guideControl, 2);
            guideControl.Opacity = 0;
            PageGrid.Children.Add(guideControl);
            guideControl.BeginAnimation(OpacityProperty, new DoubleAnimation()
            {
                Duration = TimeSpan.FromSeconds(0.2),
                To = 1,
            });
        }
    }
}
