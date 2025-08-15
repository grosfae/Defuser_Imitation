using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Defuser_Imitation_Updater.Components.UserControls
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
            if (Parent != null)
            {
                Parent.Children.Remove(this);
            }
        }
    }
}
