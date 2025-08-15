using System.Windows;
using System.Windows.Controls;

namespace Defuser_Imitation.Components.UserControls
{
    /// <summary>
    /// Логика взаимодействия для MessageControl.xaml
    /// </summary>
    public partial class MessageControl : UserControl
    {
        //public MessageBoxResult messageBoxResult;
        private readonly TaskCompletionSource<MessageBoxResult> messageBoxResult = new();
        public MessageControl()
        {
            InitializeComponent();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            messageBoxResult.SetResult(MessageBoxResult.Cancel);
            SelfDispose();
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            messageBoxResult.SetResult(MessageBoxResult.Yes);
            SelfDispose();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            messageBoxResult.SetResult(MessageBoxResult.Cancel);
            SelfDispose();
        }
        private void SelfDispose()
        {
            var parent = this.Parent as Grid;
            if (parent != null && parent.Children.Contains(this))
            {
                parent.Children.Remove(this);
            }

        }
        public Task<MessageBoxResult> ShowMessage(Panel hostPanel, string title, string message, string messageDescription)
        {
            TbTitle.Text = title;
            TbMessage.Text = message;
            TbMessageDescription.Text = messageDescription;
            this.Height = App.Current.MainWindow.ActualHeight;
            this.Width = App.Current.MainWindow.ActualWidth;
            Grid.SetZIndex(this, 3);
            hostPanel.Children.Add(this);
            return messageBoxResult.Task;
        }
    }
}
