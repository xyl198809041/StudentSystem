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
    /// UserControl_StudentMsgListView.xaml 的交互逻辑
    /// </summary>
    public partial class UserControl_StudentMsgListView : UserControl
    {
        public UserControl_StudentMsgListView()
        {
            InitializeComponent();
        }
    }

    /// <summary>
    /// 学生信息放入listview
    /// </summary>
    public class StudentMsgList_Value : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<StudentMsg> list = (ObservableCollection<StudentMsg>)value;
            //var view = new ListCollectionView(list);
            //return view;
            List<StudentMsg> list1 = new List<StudentMsg>();
            list.ToList().Where(p => (int)p.State >= 1).GroupBy(p => p.Student).OrderByDescending(p => p.Sum(q => q.Point)).ToList().ForEach(p => list1.AddRange(p.ToList()));
            var view = new ListCollectionView(list1);
            view.GroupDescriptions.Add(new PropertyGroupDescription(nameof(StudentMsg.Student)));
            return view;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 计算每位同学总分
    /// </summary>
    public class SumPoint_Value : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ReadOnlyObservableCollection<object> list = (ReadOnlyObservableCollection<object>)value;
            return list.Cast<StudentMsg>().Sum(p => p.Point);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 分数显示
    /// </summary>
    public class Point_Value : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "分数:" + value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
