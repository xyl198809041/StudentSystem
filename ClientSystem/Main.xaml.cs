using DataSystem;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ClientSystem
{
    /// <summary>
    /// Main.xaml 的交互逻辑
    /// </summary>
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();
            Height = SystemParameters.WorkArea.Height;
            Width = SystemParameters.WorkArea.Width;
            Top = SystemParameters.WorkArea.Top;
            Left = SystemParameters.WorkArea.Left;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            //数据初始化
            var a = Data.Current;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            AppSet.IsRunning = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            DialogHost.Show(new Layout.UserControl_InputBox());
        }
    }
}
