using Defuser_Imitation.Components.UserControls;
using System.Windows.Controls;

namespace Defuser_Imitation.Pages
{
    /// <summary>
    /// Логика взаимодействия для PlayPage.xaml
    /// </summary>
    public partial class PlayPage : Page
    {
        public PlayPage()
        {
            InitializeComponent();
            MainPlayGrid.Children.Add(new PlantDeviceControl(MainPlayGrid));
        }
    }
}
