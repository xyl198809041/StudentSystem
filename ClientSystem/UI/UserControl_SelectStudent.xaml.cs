using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace ClientSystem.UI
{
    /// <summary>
    /// UserControl_SelectStudent.xaml 的交互逻辑
    /// </summary>
    public partial class UserControl_SelectStudent : UserControl, INotifyPropertyChanged
    {
        public UserControl_SelectStudent()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action<DataSystem.DB.Student> EventAdd;

        private void Combobox_StudentList_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && StudentList.Count==1)
            {
                Button_Add_Click(null, null);
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StudentList)));
            if (StudentList.Count == 1)
            {
                Combobox_StudentList.SelectedIndex = 0;
                Combobox_StudentList.IsDropDownOpen = false;
            }
            else
            {
                Combobox_StudentList.IsDropDownOpen = true;
            }
            
        }

        /// <summary>
        /// 候选学生列表
        /// </summary>
        public List<DataSystem.DB.Student> StudentList
        {
            get
            {
                return DataSystem.Data.Current.Students.Where(p => p.StudentName.IndexOf(Combobox_StudentList.Text) != -1 || p.PY.IndexOf(Combobox_StudentList.Text.ToUpper()) != -1).ToList();
            }
        }
        /// <summary>
        /// 发送选中学生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            EventAdd?.Invoke(StudentList[0] as DataSystem.DB.Student);
            Combobox_StudentList.Text = "";
        }
    }
}
