using DataSystem.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
    /// UserControl_MissionList.xaml 的交互逻辑
    /// </summary>
    public partial class UserControl_MissionList : UserControl
    {
        public UserControl_MissionList()
        {
            InitializeComponent();
        }
    }



    public class MissionList_Value : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<StudentMsg> list = (ObservableCollection<StudentMsg>)value;
            List<StudentMsg> list1 = new List<StudentMsg>();
            list.ToList().Where(p => p.Rule.JCRule.IsMission && (int)p.State >= 5).GroupBy(p => p.Student).OrderByDescending(p => p.Count()).ToList().ForEach(p => list1.AddRange(p.ToList()));
            ListCollectionView view = new ListCollectionView(list1);
            view.GroupDescriptions.Add(new PropertyGroupDescription(nameof(StudentMsg.Student)));
            return view;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
