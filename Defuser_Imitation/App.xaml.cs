using Defuser_Imitation.Components;
using System.Windows;

namespace Defuser_Imitation
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static GitHubUpdater gitHubUpdater = new GitHubUpdater("grosfae", "Defuser_Imitation");
    }
}
