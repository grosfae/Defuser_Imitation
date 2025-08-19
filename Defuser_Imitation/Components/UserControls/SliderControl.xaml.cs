using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Defuser_Imitation.Components.UserControls
{
    /// <summary>
    /// Логика взаимодействия для SliderControl.xaml
    /// </summary>
    public partial class SliderControl : UserControl
    {
        public static DependencyProperty MaximumProperty = DependencyProperty.Register(
            nameof(Maximum), typeof(int), typeof(SliderControl), new PropertyMetadata(default(int)));
        public static DependencyProperty MinimumProperty = DependencyProperty.Register(
            nameof(Minimum), typeof(int), typeof(SliderControl), new PropertyMetadata(default(int)));
        public static DependencyProperty SliderValueProperty = DependencyProperty.Register(
            nameof(SliderValue), typeof(int), typeof(SliderControl), new PropertyMetadata(default(int)));
        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }
        public int Minimum
        {
            get { return (int)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public int SliderValue
        {
            get { return (int)GetValue(SliderValueProperty); }
            set { SetValue(SliderValueProperty, value); }
        }

        public SliderControl()
        {
            InitializeComponent();
        }
        private void LeftBtn_Click(object sender, RoutedEventArgs e)
        {
            SliderValue -= 1;
        }
        private void RightBtn_Click(object sender, RoutedEventArgs e)
        {
            SliderValue += 1;
        }

        private void SliderElement_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            CurrentSliderValueCheck();
        }
        private void CurrentSliderValueCheck()
        {
            if (SliderValue == Maximum)
            {
                RightBtn.IsEnabled = false;
            }
            else
            {
                RightBtn.IsEnabled = true;
            }
            if (SliderValue == Minimum)
            {
                LeftBtn.IsEnabled = false;
            }
            else
            {
                LeftBtn.IsEnabled = true;
            }
        }
        private void ArrowBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Button)sender).Opacity = 0.6;
        }

        private void ArrowBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            if (button.IsEnabled == true)
            {
                button.Opacity = 1;
            }
            else
            {
                button.Opacity = 0.6;
            }
        }

        private void ArrowBtn_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Button button = (Button)sender;
            if(button.IsEnabled == false)
            {
                button.Opacity = 0.6;
            }
            else
            {
                button.Opacity = 1;
            }
        }

        private void userControl_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentSliderValueCheck();
            SliderElement.ValueChanged += SliderElement_ValueChanged;
        }
    }
}
