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
    /// Логика взаимодействия для MapSpotControl.xaml
    /// </summary>
    public partial class MapSpotControl : UserControl
    {
        public MapSpotControl()
        {
            InitializeComponent();
            SetParameters();
            var storyboard = (Storyboard)FindResource("MapSpotAnimation");
            storyboard.Begin();
        }
        private void SetParameters()
        {
            viewbox.Opacity = 0;

            spotBorder.Height = 0;
            spotBorder.Width = 0;

            innerBorder.Height = 0;
            innerBorder.Width = 0;

            coreBorder.Height = 0;
            coreBorder.Width = 0;

            Random random = new Random();
            viewbox.Height = random.Next(20, 51);
            viewbox.Width = random.Next(20, 51);
        }
        private void Storyboard_Completed(object sender, EventArgs e)
        {
            var Parent = this.Parent as Canvas;
            Parent.Children.Remove(this);
        }
    }
}
