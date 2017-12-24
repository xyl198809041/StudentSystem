using DataSystem.DB;
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
using System.Globalization;

namespace ClientSystem.UI.MissionType
{
    /// <summary>
    /// UserControl_JCRule.xaml 的交互逻辑
    /// </summary>
    public partial class UserControl_JCRule : UserControl
    {
        public UserControl_JCRule()
        {
            InitializeComponent();
        }
        
    }
    public class MissionValue_Value : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "1";
            StudentMsg msg = (StudentMsg)value;
            return msg.Rule.JCRule.ExJCRule.ValueMissionMsg(msg);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
