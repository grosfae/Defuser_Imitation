using Defuser_Imitation.Pages;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Defuser_Imitation.Components.UserControls
{
    /// <summary>
    /// Логика взаимодействия для CountdownControl.xaml
    /// </summary>
    public partial class CountdownControl : UserControl
    {
        public event EventHandler StartRound;
        protected virtual void OnStartRound()
        {
            StartRound?.Invoke(this, EventArgs.Empty);
        }
        private Grid _playPageGrid { get; set; }
        private int _countdown { get; set; }
        private DispatcherTimer countdownControlTimer = new DispatcherTimer();
        public CountdownControl(Grid playPageGrid, int countdown)
        {
            InitializeComponent();
            _playPageGrid = playPageGrid;
            _countdown = countdown;
            TbCountdown.Text = _countdown.ToString();
            Panel.SetZIndex(this, 2);
            countdownControlTimer.Interval = TimeSpan.FromSeconds(1);
            countdownControlTimer.Tick += CountdownControlTimer_Tick;
            CheckSoundPosition();
            MiscUtilities.SoundPlayers["round_start_sound"].MediaEnded += CountdownControl_MediaEnded;
            MiscUtilities.SoundPlayers["round_start_sound"].Play();
        }
        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            countdownControlTimer.Start();
        }
        private void CountdownControl_MediaEnded(object? sender, EventArgs e)
        {
            CheckSoundPosition();
        }
        private void CheckSoundPosition()
        {
            if (MiscUtilities.SoundPlayers["round_start_sound"].NaturalDuration < TimeSpan.FromSeconds(_countdown))
            {
                MiscUtilities.SoundPlayers["round_start_sound"].Position = TimeSpan.FromSeconds(0);
            }
            else
            {
                MiscUtilities.SoundPlayers["round_start_sound"].Position = MiscUtilities.SoundPlayers["round_start_sound"].NaturalDuration.TimeSpan - TimeSpan.FromSeconds(_countdown);
            }
        }
        private void CountdownControlTimer_Tick(object? sender, EventArgs e)
        {
            _countdown--;
            TbCountdown.Text = _countdown.ToString();
            if (_countdown == 0)
            {
                MiscUtilities.SoundPlayers["round_start_sound"].Stop();
                countdownControlTimer.Stop();
                DoubleAnimation disappearAnimation = new DoubleAnimation()
                { 
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.1)
                };
                disappearAnimation.Completed += DisappearAnimation_Completed;
                this.BeginAnimation(OpacityProperty, disappearAnimation);
            }
        }
        private void DisappearAnimation_Completed(object? sender, EventArgs e)
        {
            SelfDispose();
        }
        private void SelfDispose()
        {
            OnStartRound();
            var playPage = (PlayPage)_playPageGrid.Parent;
            if (_playPageGrid != null && _playPageGrid.Children.Contains(this))
            {
                _playPageGrid.Children.Remove(this);
            }
        }
        public void PauseCountdown()
        {
            MiscUtilities.SoundPlayers["round_start_sound"].Pause();
            countdownControlTimer.Stop();
        }
        public void ContinueCountdown()
        {
            CheckSoundPosition();
            MiscUtilities.SoundPlayers["round_start_sound"].Play();
            countdownControlTimer.Start();
        }
    }
}
