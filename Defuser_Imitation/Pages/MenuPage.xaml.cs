using Defuser_Imitation.Components;
using Defuser_Imitation.Components.UserControls;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace Defuser_Imitation.Pages
{
    /// <summary>
    /// Логика взаимодействия для MenuPage.xaml
    /// </summary>
    public partial class MenuPage : Page
    {
        private DispatcherTimer menuTimer = new DispatcherTimer();
        private bool checkingUpdate;
        private bool needToDownloadUpdate;
        public MenuPage()
        {
            InitializeComponent();
            SetTime();
            menuTimer.Interval = TimeSpan.FromSeconds(1);
            menuTimer.Tick += MenuTimer_Tick;
            menuTimer.Start();
            SetVersionText();            
            if (App.UpdateHasBeenAutoChecked == false)
            {
                Task.Run(GitHubCheckUpdateAsync);
                App.UpdateHasBeenAutoChecked = true;
            }
        }

        private void MenuTimer_Tick(object? sender, EventArgs e)
        {
            SetTime();
        }
        private void CreateMapSpot()
        {
            Random random = new Random();
            MapSpotControl mapSpotControl = new MapSpotControl();
            Canvas.SetLeft(mapSpotControl, random.Next(15, 1800));
            Canvas.SetTop(mapSpotControl, random.Next(50, 900));
            MapCanvas.Children.Add(mapSpotControl);
        }
        private void SetTime()
        {
            CreateMapSpot();
            TbTime.Text = DateTime.Now.ToString("t");
            TbDate.Text = DateTime.Now.ToString("D");
        }
        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            menuTimer.Stop();
            MiscUtilities.SetSoundsVolume();
            NavigationService.Navigate(new PlayPage());
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            menuTimer.Stop();
            App.Current.Shutdown();
        }
        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            SettingsControl settingsWindow = new SettingsControl();
            Panel.SetZIndex(settingsWindow, 3);
            settingsWindow.Opacity = 0;
            PageGrid.Children.Add(settingsWindow);
            settingsWindow.BeginAnimation(OpacityProperty, new DoubleAnimation()
            {
                Duration = TimeSpan.FromSeconds(0.2),
                To = 1,
            });
        }
        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            if(checkingUpdate == true)
            {
                return;
            }
            needToDownloadUpdate = true;
            Task.Run(GitHubCheckUpdateAsync);
        }
        private void GitHubBtn_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/grosfae?tab=repositories");
        }
        private void GuideBtn_Click(object sender, RoutedEventArgs e)
        {
            GuideControl guideControl = new GuideControl();
            Panel.SetZIndex(guideControl, 3);
            guideControl.Opacity = 0;
            PageGrid.Children.Add(guideControl);
            guideControl.BeginAnimation(OpacityProperty, new DoubleAnimation()
            {
                Duration = TimeSpan.FromSeconds(0.2),
                To = 1,
            });
        }
        private void SetVersionText()
        {
            string version = $"{GitHubUpdater.CurrentVersion} version";
            TbVersion.Text = version;
        }
        private async Task GitHubCheckUpdateAsync()
        {
            if (checkingUpdate == true)
            {
                return;
            }
            checkingUpdate = true;
            App.Current.Dispatcher.Invoke(SetSearchUpdateText);
            await GitHubUpdater.CheckUpdate();
            try
            {
                if (GitHubUpdater.CanUpdate == true)
                {
                    if(needToDownloadUpdate == true)
                    {
                        MessageBoxResult messageBoxResult = await App.Current.Dispatcher.Invoke(() => new MessageControl().ShowMessage(PageGrid, "Обновление", "Вы точно хотите обновить приложение?", "Приложение будет закрыто, после чего запустится обновление."));
                        if (messageBoxResult != MessageBoxResult.Yes)
                        {
                            needToDownloadUpdate = false;
                            checkingUpdate = false;
                            App.Current.Dispatcher.Invoke(SetNewUpdateText);
                            return;
                        }
                        GitHubUpdater.OpenUpdater();
                        return;
                    }
                    App.Current.Dispatcher.Invoke(SetNewUpdateText);
                    checkingUpdate = false;
                    return;
                }
            }
            catch (Exception exception)
            {
                App.Current.Dispatcher.Invoke(SetDefaultUpdateText);
                LoggerSerivce.Write(exception);
                checkingUpdate = false;
            }
            App.Current.Dispatcher.Invoke(SetDefaultUpdateText);
            checkingUpdate = false;
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
            string updateVersionDefaultText = "Текущая версия: ".ToUpper() + GitHubUpdater.CurrentVersion;
            TbUpdateText.Text = updateDefaultText;
            TbUpdateVersion.Text = updateVersionDefaultText;
        }
        private void SetNewUpdateText()
        {
            string updateDefaultText = "Доступно обновление".ToUpper();
            string updateVersionDefaultText = "Новая версия: ".ToUpper() + GitHubUpdater.LastestGitHubVersion;
            TbUpdateText.Text = updateDefaultText;
            TbUpdateVersion.Text = updateVersionDefaultText;
        }
    }
}
