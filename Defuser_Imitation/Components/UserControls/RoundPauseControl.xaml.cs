using System.Windows;
using System.Windows.Controls;

namespace Defuser_Imitation.Components.UserControls
{
    /// <summary>
    /// Логика взаимодействия для RoundPauseControl.xaml
    /// </summary>
    public partial class RoundPauseControl : UserControl
    {
        public event EventHandler ContinueRound;
        public event EventHandler ExitToMenu;
        protected virtual void OnContinueRound()
        {
            ContinueRound?.Invoke(this, EventArgs.Empty);
        }
        protected virtual void OnExitToMenu()
        {
            ExitToMenu?.Invoke(this, EventArgs.Empty);
        }
        private Grid _playPageGrid;
        public RoundPauseControl(Grid playPageGrid)
        {
            InitializeComponent();
            _playPageGrid = playPageGrid;
        }
        private void SelfDispose()
        {
            if (_playPageGrid != null && _playPageGrid.Children.Contains(this))
            {
                _playPageGrid.Children.Remove(this);
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
