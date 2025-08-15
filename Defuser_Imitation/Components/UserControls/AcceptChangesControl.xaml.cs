using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Defuser_Imitation.Components.UserControls
{
    /// <summary>
    /// Логика взаимодействия для AcceptChangesControl.xaml
    /// </summary>
    public partial class AcceptChangesControl : UserControl
    {
        public event EventHandler AcceptChanges;
        public event EventHandler DiscardChanges;
        protected virtual void OnAcceptChanges()
        {
            if (AcceptChanges != null)
            {
                AcceptChanges(this, EventArgs.Empty);
            }
        }
        protected virtual void OnDiscardChanges()
        {
            if (DiscardChanges != null)
            {
                DiscardChanges(this, EventArgs.Empty);
            }
        }
        public AcceptChangesControl()
        {
            InitializeComponent();
            this.Opacity = 0;
            DoubleAnimation MessageAppearAnimation = new DoubleAnimation()
            {
                Duration = TimeSpan.FromSeconds(0.2),
                To = 1,

            }; 
            this.BeginAnimation(OpacityProperty, MessageAppearAnimation);
        }
        private void SelfDispose()
        {
            var ParentElement = this.Parent as Grid;
            if (ParentElement != null && ParentElement.Children.Contains(this))
            {
                ParentElement.Children.Remove(this);
            }
        }

        private void AcceptBtn_Click(object sender, RoutedEventArgs e)
        {
            OnAcceptChanges();
            SelfDispose();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            OnDiscardChanges();
            SelfDispose();
        }
    }
}
