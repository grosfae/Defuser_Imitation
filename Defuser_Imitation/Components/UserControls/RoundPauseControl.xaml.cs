using Defuser_Imitation.Components.ViewModels;
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
    /// Логика взаимодействия для RoundPauseControl.xaml
    /// </summary>
    public partial class RoundPauseControl : UserControl
    {
        public event EventHandler? ContinueRound;
        public event EventHandler? ExitToMenu;
        protected virtual void OnContinueRound()
        {
            ContinueRound?.Invoke(this, EventArgs.Empty);
        }
        protected virtual void OnExitToMenu()
        {
            ExitToMenu?.Invoke(this, EventArgs.Empty);
        }
        private Grid PlayPageGrid;
        public RoundPauseControl(Grid playPageGrid)
        {
            InitializeComponent();
            PlayPageGrid = playPageGrid;
        }
        private void SelfDispose()
        {
            if (PlayPageGrid != null && PlayPageGrid.Children.Contains(this))
            {
                PlayPageGrid.Children.Remove(this);
            }
        }

        private void ResumeBtn_Click(object sender, RoutedEventArgs e)
        {
            SelfDispose();
            OnContinueRound();
        }

        private void MenuBtn_Click(object sender, RoutedEventArgs e)
        {
            SelfDispose();
            OnExitToMenu();
        }
    }
}
