using Defuser_Imitation_Updater.Components;
using Defuser_Imitation_Updater.Components.UserControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Defuser_Imitation_Updater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _animationActive = true;
        public MainWindow()
        {
            InitializeComponent();
            Header.MouseLeftButtonDown += new MouseButtonEventHandler(Window_MouseDown);
            StartMapAnimation();
            if(Updater.CheckInternetConnection() == true)
            {
                Task.Run(UpdateMainApp);
            }
        }
        private async Task UpdateMainApp()
        {
            await Updater.DownloadAndInstallUpdate(PbUpdate, TbState,TbRatio);
        }
        private async void StartMapAnimation()
        {
            while (_animationActive == true)
            {
                CreateMapSpot();
                await Task.Delay(400);
            }
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
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}