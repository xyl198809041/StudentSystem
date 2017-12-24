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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientSystem.Layout
{
    /// <summary>
    /// UserControl_Main.xaml 的交互逻辑
    /// </summary>
    public partial class UserControl_Main : UserControl
    {
        public UserControl_Main()
        {
            InitializeComponent();
            
        }

        private void OpenWin_AddStudentMsg_Click(object sender, RoutedEventArgs e)
        {
            DialogHost.Show(new UserControl_StudentMsgAdd());
            //DialogHost.Show(new UserControl_OpenWin());
        }

        private void OpenWin_AddRule_Click(object sender, RoutedEventArgs e)
        {
            DialogHost.Show(new UserControl_RuleAdd());
            //DialogHost.Show(new UserControl_JCRuleAdd());
        }

        private void UserControl_AccessKeyPressed(object sender, AccessKeyPressedEventArgs e)
        {

        }

    }
}
