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
using DataSystem.Plugin.XXT;

namespace ClientSystem.Layout
{
    /// <summary>
    /// UserControl_RuleAdd.xaml 的交互逻辑
    /// </summary>
    public partial class UserControl_RuleAdd : UserControl
    {
        public UserControl_RuleAdd()
        {
            InitializeComponent();
        }




        public Rule SelectRule
        {
            get { return (Rule)GetValue(SelectRuleProperty); }
            set { SetValue(SelectRuleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectRule.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectRuleProperty =
            DependencyProperty.Register("SelectRule", typeof(Rule), typeof(UserControl_RuleAdd), new PropertyMetadata(new Rule()
            {
                JCRule = new JCRule()
            },SelectRuleChanged));

        private static void SelectRuleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
            {
                d.SetValue(e.Property, e.OldValue);
            }
        }

        private ListCollectionView _ListCollectionView_RuleList;
        /// <summary>
        /// 班规显示列表
        /// </summary>
        public ListCollectionView ListCollectionView_RuleList
        {
            get
            {
                if (_ListCollectionView_RuleList == null)
                {
                    _ListCollectionView_RuleList = new ListCollectionView(Data.Current.Rules);
                    _ListCollectionView_RuleList.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Rule.Group)));
                    _ListCollectionView_RuleList.IsLiveGrouping = true;
                }
                return _ListCollectionView_RuleList;
            }
        }

        public List<Rule> EditRules = new List<Rule>();

        private void CustomControl_OpenWin_Closed(UI.CustomControl_OpenWin obj)
        {
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (!EditRules.Contains(SelectRule)) EditRules.Add(SelectRule);
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            Rule rule = Data.Current.LocalAddRule();
            SelectRule = rule;
            EditRules.Add(SelectRule);
        }

        private void ComboBox_Group_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ListView_RuleList.SelectedItem = SelectRule;
        }

        private async void Button_UpLoad_Click(object sender, RoutedEventArgs e)
        {
            EditRules.ForEach(p => p.JCRuleId = p.JCRule.Id);
            Button_UpLoad.ProgressValue = UI.UserControl_ProgressButton.ProgressType.Start;
            await Data.Current.SaveChangeRules(EditRules);
            Button_UpLoad.ProgressValue = UI.UserControl_ProgressButton.ProgressType.Done;
            Win.Close(new TimeSpan(2*10*1000*1000));
        }

        private void Button_JCRuleAddWin_Click(object sender, RoutedEventArgs e)
        {
            Win.Close();
            DialogHost.Show(new UserControl_JCRuleAdd());
        }

        private void Button_AddSMSTextkey_Click(object sender, RoutedEventArgs e)
        {
            XXT_RuleByStateType data = (XXT_RuleByStateType)((Button)sender).Tag;
            data.XXT_SMSText += ((Button)sender).Content;
        }

    }

    public class RuleToXXT_Rule_Value : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Rule rule = (Rule)value;
            return new DataSystem.Plugin.XXT.XXT_Rule(rule);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SendSMSEveryToString_Value : IValueConverter
    {
        private List<string> IntToString = new List<string>()
        {
            "一次",
            "每天",
            "每周"
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return IntToString[(int)value];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (DataSystem.Plugin.XXT.SendSMSEvery)IntToString.IndexOf((string)value);
        }
    }

    public class StateType_Value : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<XXT_RuleByStateType> dictionary = (List<XXT_RuleByStateType>)value;
            var list = new ListCollectionView(dictionary);
            list.GroupDescriptions.Add(new PropertyGroupDescription());
            return list;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
}
