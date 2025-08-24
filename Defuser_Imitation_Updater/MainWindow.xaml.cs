using Defuser_Imitation_Updater.Components;
using Defuser_Imitation_Updater.Components.UserControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Defuser_Imitation_Updater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer animationTimer;
        public MainWindow()
        {
            InitializeComponent();
            Header.MouseLeftButtonDown += new MouseButtonEventHandler(Window_MouseDown);
            animationTimer = new DispatcherTimer();
            animationTimer.Interval = TimeSpan.FromSeconds(0.7);
            animationTimer.Tick += AnimationTimer_Tick;
            
            TbState.Text = "Необходимо запустить основное приложение";
            if(Updater.CheckInternetConnection() == true)
            {
                Task.Run(UpdateMainApp);
            }
        }

        private void AnimationTimer_Tick(object? sender, EventArgs e)
        {
            CreateMapSpot();
        }

        private async Task UpdateMainApp()
        {
            await Updater.DownloadAndInstallUpdate(PbUpdate, TbState,TbRatio);
        }
        private void CreateMapSpot()
        {
            Random random = new();
            MapSpotControl mapSpotControl = new();
            Canvas.SetLeft(mapSpotControl, random.Next(15, 760));
            Canvas.SetTop(mapSpotControl, random.Next(50, 380));
            MapCanvas.Children.Add(mapSpotControl);
        }
        private void MinButton_Click(object sender, RoutedEventArgs e)
        {
            animationTimer.Stop();
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            animationTimer.Stop();
            Application.Current.Shutdown();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                animationTimer.Stop();
            }
            else
            {
                animationTimer.Start();
            }
        }
    }
}