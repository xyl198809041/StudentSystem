using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientSystem.UI
{
    /// <summary>
    /// UserControl_ScrollingLabel.xaml 的交互逻辑
    /// </summary>
    public partial class UserControl_ScrollingLabel : UserControl
    {
        public UserControl_ScrollingLabel()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 内容依赖属性
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(UserControl_ScrollingLabel));
        private Timer _Timer = new Timer();
        private int _n = 0;

        /// <summary>
        /// 内容
        /// </summary>
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }









        private void userControl_Loaded(object sender, RoutedEventArgs e)
        {
            _Timer.Interval = 1000;
            _Timer.Enabled = true;
            _Timer.Elapsed += _Timer_Elapsed;
        }

        private void _Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (_n >= (Text + "     ").Length) _n = 0;
                label.Text = label.Text + (Text + "     ").Substring(_n, 1);
                _n++;
                if (label.Text.Length > 255)
                {
                    label.Text.Remove(0, 100);
                }
            }));
        }
    }
}
