using DataSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Button_Login_Click(object sender, RoutedEventArgs e)
        {
            if ((string)Button_Login.Content == "登录")
            {
                LoginAndOpenWin();
            }
            else
            {
                Properties.Settings.Default.AutoLogin = false;
                Properties.Settings.Default.Save();
                Button_Login.Content = "登录";
            }
        }

        private void LoginAndOpenWin()
        {
            Properties.Settings.Default.Save();
            if (Data.Login(Properties.Settings.Default.UserName, Properties.Settings.Default.PassWord))
            {
                if (Data.User.Role == DataSystem.DB.UserRole.Student)
                {
                    new Main().Show();
                }
                else if(Data.User.Role==DataSystem.DB.UserRole.Teacher)
                {
                    new TeacherMain().Show();
                }
                else
                {
                    throw new Exception();
                }
            }
            Close();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.AutoLogin)
            {
                Task.Run(() =>
                {
                    int i = 5;
                    while (i > 0 && Properties.Settings.Default.AutoLogin)
                    {
                        this.Dispatcher.Invoke(() => Button_Login.Content = $"取消({i}秒后自动登录)");
                        Thread.Sleep(1000);
                        i--;
                    }
                    if (!Properties.Settings.Default.AutoLogin)
                    {
                        return;
                    }
                    Dispatcher.Invoke(LoginAndOpenWin);
                });
            }
        }
    }
}
