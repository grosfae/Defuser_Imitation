using Defuser_Imitation.Components.ViewModels;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace Defuser_Imitation.Components.UserControls
{
    /// <summary>
    /// Логика взаимодействия для GuideControl.xaml
    /// </summary>
    public partial class GuideControl : UserControl
    {
        private GuideChapterViewModel guideChapterViewModel;
        private bool isClosed = false;
        public GuideControl()
        {
            InitializeComponent();
            guideChapterViewModel = new GuideChapterViewModel();
            DataContext = guideChapterViewModel;
            guideChapterViewModel.SelectedGuideChapter = guideChapterViewModel.GuideChapters[0];
            LoadFlowDocumentFromResources(RTBGuide, guideChapterViewModel.SelectedGuideChapter.DocumentPath);
        }
        private void GuideChapterBtn_Click(object sender, RoutedEventArgs e)
        {
            guideChapterViewModel.SelectedGuideChapter = (GuideChapter)((Button)sender).DataContext;
            ScrollGuide.ScrollToTop();
            LoadFlowDocumentFromResources(RTBGuide, guideChapterViewModel.SelectedGuideChapter.DocumentPath);
        }
        private void LoadFlowDocumentFromResources(RichTextBox richTextBox,string resourceUri)
        {
            var streamInfo = Application.GetResourceStream(new Uri(resourceUri));
            if (streamInfo != null)
            {
                using (Stream stream = streamInfo.Stream)
                {
                    FlowDocument flowDocument = (FlowDocument)XamlReader.Load(stream);
                    richTextBox.Document = flowDocument;
                }
            }

        }
        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            SelfDispose();
        }
        private void RTBGuide_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void RTBGuide_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.KeyDown += UserControl_KeyDown;
        }
        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                SelfDispose();
            }
        }
        private void SelfDispose()
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation()
            {
                Duration = TimeSpan.FromSeconds(0.2),
                To = 0,

            };
            doubleAnimation.Completed += DoubleAnimation_Completed;
            this.BeginAnimation(OpacityProperty, doubleAnimation);
        }
        private void DoubleAnimation_Completed(object sender, EventArgs e)
        {
            if (isClosed == false)
            {
                isClosed = true;
                var window = Window.GetWindow(this);
                window.KeyDown -= UserControl_KeyDown;
                var parentElement = this.Parent as Grid;
                if (parentElement != null && parentElement.Children.Contains(this))
                {
                    parentElement.Children.Remove(this);
                }
            }
        }
    }
}
