using DataSystem;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
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

        private void Button_UpdateClassAppMsg_Click(object sender, RoutedEventArgs e)
        {
            //typeof(Data).GetProperty(nameof(Data.Setting)).SetValue(Data.Current, new DataSystem.DB.Setting()
            //{
            //    Name = "1509",
            //    ClassAppMsg = "23229"
            //});
            //Data.Current.Setting = new DataSystem.DB.Setting()
            //{
            //    Name = "1509",
            //    ClassAppMsg = "23229"

            //};
            ////Data.Current.Setting = Data.Current.Setting;
                
            //Data.Current.Setting.ClassAppMsg += "2";

            var s= Data.Current.SaveSetting().Result;
            MessageBox.Show(s ? "成功" : "失败");
        }
    }

    public class Visibility_Value : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SumPointByLastWeek_Value : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ListCollectionView listCollectionView = new ListCollectionView((List<DataSystem.DB.StudentSumPoint>)value);
            listCollectionView.SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(DataSystem.DB.StudentSumPoint.LastWeekPoint), System.ComponentModel.ListSortDirection.Descending));
            return listCollectionView;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class SumPointByThisWeek_Value : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ListCollectionView listCollectionView = new ListCollectionView((List<DataSystem.DB.StudentSumPoint>)value);
            listCollectionView.SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(DataSystem.DB.StudentSumPoint.ThisWeekPoint), System.ComponentModel.ListSortDirection.Descending));
            return listCollectionView;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class SumPointBySumPoint_Value : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ListCollectionView listCollectionView = new ListCollectionView((List<DataSystem.DB.StudentSumPoint>)value);
            listCollectionView.SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(DataSystem.DB.StudentSumPoint.SumPoint), System.ComponentModel.ListSortDirection.Descending));
            return listCollectionView;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
