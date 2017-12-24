using DataSystem;
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

namespace ClientSystem.Layout
{
    /// <summary>
    /// UserControl_StudentMsgAdd.xaml 的交互逻辑
    /// </summary>
    public partial class UserControl_StudentMsgAdd : UserControl
    {
        public UserControl_StudentMsgAdd()
        {
            InitializeComponent();
        }



        private void userControl_Loaded(object sender, RoutedEventArgs e)
        {
            StudentSelect.EventAdd += StudentSelect_EventAdd;
        }
        
        

        /// <summary>
        /// 添加学生信息
        /// 由学生选择触发
        /// </summary>
        /// <param name="obj"></param>
        private async void StudentSelect_EventAdd(DataSystem.DB.Student obj)
        {
            if (RuleSelect.SelectRule != null)
            {
                Guid id = Guid.NewGuid();
                AddStudentMsgStateList.Insert(0,new AddStudentMsgState()
                {
                    Id = id,
                    State = UI.UserControl_ProgressButton.ProgressType.Start,
                    Rule=RuleSelect.SelectRule,
                    Student=obj
                });
                await Data.Current.AddStudentMsgAsync(obj.Id, RuleSelect.SelectRule.Id);
                AddStudentMsgStateList.Where(p => p.Id == id).First().State = UI.UserControl_ProgressButton.ProgressType.Done;
            }
        }




        public System.Collections.ObjectModel.ObservableCollection<AddStudentMsgState> AddStudentMsgStateList { get; set; } = new System.Collections.ObjectModel.ObservableCollection<AddStudentMsgState>();
        
    }

    /// <summary>
    /// 提交信息状态
    /// 提交列表显示
    /// </summary>
    public class AddStudentMsgState : NotifyPropertyChangedBase
    {

        private Guid _Id;
        /// <summary>
        /// 标识
        /// </summary>
        public Guid Id
        {
            get { return _Id; }
            set
            {
                _Id = value;
                NotifyPropertyChanged(nameof(Id));
            }
        }

        public Student Student { get; set; }

        public Rule Rule { get; set; }

        private UI.UserControl_ProgressButton.ProgressType _State;
        /// <summary>
        /// 提交状态
        /// </summary>
        public UI.UserControl_ProgressButton.ProgressType State
        {
            get { return _State; }
            set
            {
                _State = value;
                NotifyPropertyChanged(nameof(State));
            }
        }

    }
}
