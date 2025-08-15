using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Defuser_Imitation.Components.ViewModels
{
    public class GuideChapterViewModel : ViewModelBase
    {
        private ObservableCollection<GuideChapter> _guideChapters;
        private GuideChapter _selectedGuideChapter {  get; set; }
        public ObservableCollection<GuideChapter> GuideChapters
        {
            get
            {
                return _guideChapters;
            }
            set
            {
                _guideChapters = value;
                OnPropertyChanged(nameof(GuideChapters));
            }
        }
        public GuideChapter SelectedGuideChapter
        {
            get
            {
                return _selectedGuideChapter;
            }
            set
            {
                _selectedGuideChapter = value;
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
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
