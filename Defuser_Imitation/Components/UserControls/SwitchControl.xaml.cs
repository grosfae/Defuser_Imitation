using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Defuser_Imitation.Components.UserControls
{
    /// <summary>
    /// Логика взаимодействия для SwitchControl.xaml
    /// </summary>
    public partial class SwitchControl : UserControl
    {
        public static DependencyProperty SwitchListProperty = DependencyProperty.Register(
            nameof(SwitchList), typeof(List<string>), typeof(SwitchControl), new PropertyMetadata(default(List<string>)));

        public static DependencyProperty SelectedItemProperty = DependencyProperty.Register(
            nameof(SelectedItem), typeof(string), typeof(SwitchControl), new PropertyMetadata(default(string)));

        public static DependencyProperty SelectedIndexProperty = DependencyProperty.Register(
            nameof(SelectedIndex), typeof(int), typeof(SwitchControl), new PropertyMetadata(default(int)));

        public List<string> SwitchList
        {
            get { return (List<string>)GetValue(SwitchListProperty); }
            set { SetValue(SwitchListProperty, value); }
        }
        public string SelectedItem
        {
            get { return (string)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); OnSelectedItemChanged(); }
        }
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }
        public event EventHandler SelectedItemChanged;
        protected virtual void OnSelectedItemChanged()
        {
            if (SelectedItemChanged != null)
            {
                SelectedItemChanged(this, EventArgs.Empty);
            }
        }
        public SwitchControl()
        {
            InitializeComponent();       
        }
        private void userControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (SwitchList != null)
            {
                if (SelectedItem == null)
                {
                    SelectedItem = SwitchList[SelectedIndex];
                }

                for (int i = 0; i < SwitchList.Count; i++)
                {
                    RadioButton radioButton = new RadioButton
                    {
                        Height = 10,
                        Width = 17,
                        Style = (Style)FindResource("SmallRectangleRadio"),
                        Margin = new Thickness(2, 0, 2, 0),
                        VerticalAlignment = VerticalAlignment.Bottom
                    };
                    radioButton.Checked += RadioButton_Checked;
                    radioButton.Unchecked += RadioButton_Unchecked;
                    radioButton.MouseEnter += RadioButton_MouseEnter;
                    radioButton.MouseLeave += RadioButton_MouseLeave;

                    StOptionRadioButtons.Children.Add(radioButton);
                    if (SelectedItem == SwitchList[i])
                    {
                        radioButton.IsChecked = true;
                    }

                }
            }
        }
        private void RadioButton_MouseEnter(object sender, MouseEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.IsChecked == false)
            {
                radioButton.BeginAnimation(HeightProperty, new DoubleAnimation
                {
                    Duration = TimeSpan.FromSeconds(0.1),
                    To = 14
                });
                radioButton.BeginAnimation(OpacityProperty, new DoubleAnimation
                {
                    Duration = TimeSpan.FromSeconds(0.1),
                    To = 1
                });
            }
        }
        private void RadioButton_MouseLeave(object sender, MouseEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.IsChecked == false)
            {
                radioButton.BeginAnimation(HeightProperty, new DoubleAnimation
                {
                    Duration = TimeSpan.FromSeconds(0.1),
                    To = 10
                });
                radioButton.BeginAnimation(OpacityProperty, new DoubleAnimation
                {
                    Duration = TimeSpan.FromSeconds(0.1),
                    To = 0.4
                });
            }
        }
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            radioButton.BeginAnimation(HeightProperty, new DoubleAnimation
            {
                Duration = TimeSpan.FromSeconds(0.1),
                To = 14
            });
            radioButton.BeginAnimation(OpacityProperty, new DoubleAnimation
            {
                Duration = TimeSpan.FromSeconds(0.1),
                To = 1
            });
            SelectedItem = SwitchList[SelectedIndex];
            if (SelectedIndex == 0)
            {
                LeftBtn.IsEnabled = false;
            }
            else
            {
                LeftBtn.IsEnabled = true;
            }
            if (SelectedIndex == SwitchList.Count - 1)
            {
                RightBtn.IsEnabled = false;
            }
            else
            {
                RightBtn.IsEnabled = true;
            }
        }

        private void RadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            radioButton.BeginAnimation(HeightProperty, new DoubleAnimation
            {
                Duration = TimeSpan.FromSeconds(0.1),
                To = 10
            });
            radioButton.BeginAnimation(OpacityProperty, new DoubleAnimation
            {
                Duration = TimeSpan.FromSeconds(0.1),
                To = 0.4
            });
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation() 
            { 
                Duration = TimeSpan.FromSeconds(0.1),
                To = 1.1
            };
            DoubleAnimation opacityAnimation = new DoubleAnimation
            {
                Duration = TimeSpan.FromSeconds(0.1),
                To = 1
            };
            this.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, doubleAnimation);
            this.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, doubleAnimation);
            this.Background.BeginAnimation(SolidColorBrush.OpacityProperty, new DoubleAnimation
            {
                Duration = TimeSpan.FromSeconds(0.1),
                To = 1
            });
            this.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation
            {
                Duration = TimeSpan.FromSeconds(0.1),
                To = (Color)ColorConverter.ConvertFromString("#FF2BA1E7")
            });

            RectElement.BeginAnimation(OpacityProperty, opacityAnimation);
            LeftBtn.Visibility = Visibility.Visible;
            RightBtn.Visibility = Visibility.Visible;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation()
            {
                Duration = TimeSpan.FromSeconds(0.1),
                To = 1
            };
            DoubleAnimation opacityAnimation = new DoubleAnimation
            {
                Duration = TimeSpan.FromSeconds(0.1),
                To = 0
            };
            this.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, doubleAnimation);
            this.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, doubleAnimation);
            this.Background.BeginAnimation(SolidColorBrush.OpacityProperty, new DoubleAnimation
            {
                Duration = TimeSpan.FromSeconds(0.1),
                To = 0.6
            });
            this.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation
            {
                Duration = TimeSpan.FromSeconds(0.1),
                To = (Color)ColorConverter.ConvertFromString("White")
            });

            RectElement.BeginAnimation(OpacityProperty, opacityAnimation);
            LeftBtn.Visibility = Visibility.Collapsed;
            RightBtn.Visibility = Visibility.Collapsed;
        }
        private void SelectItem()
        {
            SelectedItem = SwitchList[SelectedIndex];
            ((RadioButton)StOptionRadioButtons.Children[SelectedIndex]).IsChecked = true;
        }
        private void LeftBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedIndex > 0)
            {
                SelectedIndex -= 1;
                SelectItem();
            }
        }
        private void RightBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedIndex < SwitchList.Count - 1)
            {
                SelectedIndex += 1;
                SelectItem();
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
            if (button.IsEnabled == false)
            {
                button.Opacity = 0.6;
            }
            else
            {
                button.Opacity = 1;
            }
        }

    }
}
