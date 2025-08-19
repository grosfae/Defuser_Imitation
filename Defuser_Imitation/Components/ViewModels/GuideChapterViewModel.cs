using System.Collections.ObjectModel;
using System.Windows;

namespace Defuser_Imitation.Components.ViewModels
{
    public class GuideChapterViewModel : ViewModelBase
    {
        private ObservableCollection<GuideChapter> guideChapters;
        private GuideChapter selectedGuideChapter {  get; set; }
        public ObservableCollection<GuideChapter> GuideChapters
        {
            get
            {
                return guideChapters;
            }
            set
            {
                guideChapters = value;
                OnPropertyChanged(nameof(GuideChapters));
            }
        }
        public GuideChapter SelectedGuideChapter
        {
            get
            {
                return selectedGuideChapter;
            }
            set
            {
                selectedGuideChapter = value;
                OnPropertyChanged(nameof(SelectedGuideChapter));
            }
        }
        public GuideChapterViewModel()
        {
            LoadData();
        }
        private void LoadData()
        {
            try
            {
                GuideChapters = new ObservableCollection<GuideChapter>()
                {
                    new GuideChapter()
                    {
                        Name = "Этап активации".ToUpper(),
                        Description = "",
                        DocumentPath = @"pack://application:,,,/Defuser_Imitation;component/Resources/Guide/Plant_Stage_Guide.xml",
                    },
                    new GuideChapter()
                    {
                        Name = "Этап деактивации".ToUpper(),
                        Description = "",
                        DocumentPath = @"pack://application:,,,/Defuser_Imitation;component/Resources/Guide/Defuse_Stage_Guide.xml",
                    },
                    new GuideChapter()
                    {
                        Name = "Пауза во время игры".ToUpper(),
                        Description = "",
                        DocumentPath = @"pack://application:,,,/Defuser_Imitation;component/Resources/Guide/Game_Pause_Guide.xml",
                    },
                    new GuideChapter()
                    {
                        Name = "Звуки приложения".ToUpper(),
                        Description = "",
                        DocumentPath = @"pack://application:,,,/Defuser_Imitation;component/Resources/Guide/Sounds_Guide.xml",
                    },
                };
            }
            catch (Exception exception)
            {
                LoggerSerivce.Write(exception);
                MessageBox.Show($"{exception.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
