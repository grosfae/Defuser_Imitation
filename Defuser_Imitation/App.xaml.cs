using Defuser_Imitation.Components;
using System.Windows;

namespace Defuser_Imitation
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static bool UpdateHasBeenAutoChecked = false;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            GitHubUpdater.CheckAndClearTempFolder();
            MiscUtilities.DownloadSounds();
            MiscUtilities.CheckSettings();
        }
    }
}
