using Defuser_Imitation.Components;
using Defuser_Imitation.Pages;
using System.Windows;
using System.Windows.Navigation;

namespace Defuser_Imitation
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Content = new MenuPage();
        }
        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if(MainFrame.Content is PlayPage)
            {
                MiscUtilities.ResetSoundPosition();
            }
            MainFrame.NavigationService.RemoveBackEntry();
        }
    }
}
