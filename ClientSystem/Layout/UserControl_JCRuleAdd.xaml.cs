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
using DataSystem.DB;
using DataSystem;
using MaterialDesignThemes.Wpf;

namespace ClientSystem.Layout
{
    /// <summary>
    /// UserControl_JCRuleAdd.xaml 的交互逻辑
    /// </summary>
    public partial class UserControl_JCRuleAdd : UserControl
    {
        public UserControl_JCRuleAdd()
        {
            InitializeComponent();
        }

        


        public JCRule SelectJCRule
        {
            get { return (JCRule)GetValue(SelectJCRuleProperty); }
            set { SetValue(SelectJCRuleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectJCRule.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectJCRuleProperty =
            DependencyProperty.Register("SelectJCRule", typeof(JCRule), typeof(UserControl_JCRuleAdd), new PropertyMetadata(null,SelectJCRuleChanged));

        private static void SelectJCRuleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
            {
                d.SetValue(e.Property, e.OldValue);
            }
        }

        public List<JCRule> EditJCRules { get; set; } = new List<JCRule>();


        private void ComboBox_JCRuleTypeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            KeyValuePair<Type, string> Type = (KeyValuePair<Type, string>)ComboBox_JCRuleTypeList.SelectedItem;
            if (SelectJCRule.ExJCRule.GetType() == Type.Key) return;
            SelectJCRule.JCRuleType = Type.Key.FullName;
            
        }

        private void CustomControl_OpenWin_Closed(UI.CustomControl_OpenWin obj)
        {
            DialogHost.Show(new UserControl_RuleAdd());
        }

        private async void Button_Update_Click(object sender, RoutedEventArgs e)
        {
            Button_Update.ProgressValue = UI.UserControl_ProgressButton.ProgressType.Start;
            await Data.Current.SaveChangeJCRules(EditJCRules);
            Button_Update.ProgressValue = UI.UserControl_ProgressButton.ProgressType.Done;
            Win.Close(new TimeSpan(2 * 10 * 1000 * 1000));
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            JCRule jCRule = Data.Current.LocalAddJCRule();
            SelectJCRule = jCRule;
            EditJCRules.Add(SelectJCRule);
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (!EditJCRules.Contains(SelectJCRule)) EditJCRules.Add(SelectJCRule);
        }
    }

    public class JCRuleToType_Value : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            JCRule jCRule = (JCRule)value;
            return JCRuleType.JCRuleTypeList.First(p => p.Key == jCRule.ExJCRule.GetType());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ExJCRule_AutoSave_SaveMode_Value : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value == System.Convert.ToInt32(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (ExJCRule_AutoSave.TimeMode)System.Convert.ToInt32(parameter);
        }
    }
}
