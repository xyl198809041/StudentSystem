using DataSystem.DB;
using DataSystem.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace DataSystem
{
    /// <summary>
    /// 客户端使用数据 ,单一数据名
    /// 必须设置IsRunning关闭
    /// </summary>
    public class Data:NotifyPropertyChangedBase,INotifyPropertyChanged
    {
        /// <summary>
        /// 客户端唯一id
        /// </summary>
        public static string Id = Guid.NewGuid().ToString();
        /// <summary>
        /// 数据名
        /// </summary>
        public static string Name { get => User.Name; }
        /// <summary>
        /// 用户信息
        /// </summary>
        public static User User { get; private set; }
        /// <summary>
        /// WPF系统进行调度
        /// UI访问进程
        /// </summary>
        public static System.Windows.Threading.Dispatcher Dispatcher
        {
            get
            {
                return System.Windows.Threading.Dispatcher.CurrentDispatcher;
            }
        }
        /// <summary>
        /// 登录,设置Name
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public static bool Login(string UserName,string Password)
        {
            User user= DataServer.Login(UserName, Password).ToObj<User>();
            if (user.Name == "")
            {
                return false;
            }
            User = user;
            return true;
        }

        /// <summary>
        /// 不使用
        /// </summary>
        public Data() { }

        private static Data _Current;
        /// <summary>
        /// 当前实例
        /// </summary>
        public static Data Current
        {
            get
            {
                if (_Current == null)
                {
                    if (Name == null) throw new Exception("未登录");
                    _Current = new Data();
                    _Current.Load();
                }
                return _Current;
            }
        }

        private static DataServerReference.DataServerClient _DataServer = new DataServerReference.DataServerClient();

        /// <summary>
        /// 客户端连接服务器接口
        /// 数据类内部由_Dataserver处理
        /// </summary>
        public static DataServerReference.DataServerClient DataServer
        {
            get
            {
                return _DataServer;
            }
        }
        private Thread GetNewWCFMsg_Thread;
        /// <summary>
        /// 注册网络数据同步属性
        /// </summary>
        private Dictionary<string, Func<object,object>> RegWebDataProperty = new Dictionary<string, Func<object,object>>();

        public Plugin.PluginManager PluginManager { get; private set; }


        /// <summary>
        /// 初始化
        /// 初始化属性值
        /// 注册同步属性
        /// </summary>
        protected virtual void Load()
        {

            //数据初始化
            
            Rules = new ObservableCollection<Rule>(DataServer.GetRules(Name).ToObj<List<Rule>>());
            Students = new ObservableCollection<Student>(DataServer.GetStudents(Name).ToObj<List<Student>>());
            StudentMsgs= new ObservableCollection<StudentMsg>(DataServer.GetStudentMsgs(Name).ToObj<List<StudentMsg>>());
            JCRules = new ObservableCollection<JCRule>(DataServer.GetJCRules(Name).ToObj<List<JCRule>>());
            StudentSumPoints = DataServer.GetStudentSumPoints(Name).ToObj<List<StudentSumPoint>>();
            Setting = DataServer.GetSetting(Name).ToObj<Setting>();
            //end

            //插件初始化()
            //PluginManager = new Plugin.PluginManager(Name);
            //PluginManager.Load();

            //end


            //注册
            RegWebDataProperty.Add(nameof(Data.StudentMsgs), (obj) =>
            {
                return new ObservableCollection<StudentMsg>(ExClass.ToObj<List<StudentMsg>>(DataServer.GetStudentMsgs(Name)));
            });
            RegWebDataProperty.Add(nameof(Data.Rules), (obj) =>
            {
                return new ObservableCollection<Rule>(ExClass.ToObj<List<Rule>>(DataServer.GetRules(Name)));
            });
            RegWebDataProperty.Add(nameof(Data.Students), (obj) =>
            {
                return new ObservableCollection<Student>(ExClass.ToObj<List<Student>>(DataServer.GetStudents(Name)));
            });
            RegWebDataProperty.Add(nameof(Data.JCRules), obj =>
            {
                //return new ObservableCollection<JCRule>(ExClass.ToObj<List<JCRule>>(DataServer.GetJCRules(Name)));
                return new ObservableCollection<JCRule>(DataServer.GetJCRules(Name).ToObj<List<JCRule>>());
            });
            RegWebDataProperty.Add(nameof(Setting), obj =>
            {
                Setting setting = (Setting)obj;
                Setting New = DataServer.GetSetting(Name).ToObj<Setting>();
                setting.ClassAppMsg = New.ClassAppMsg;
                return null;
            });
            //end


            GetNewWCFMsg_Thread = new Thread(() =>
              {
                  while (AppSet.IsRunning)
                  {
                      var rt= DataServer.GetNewMsg(Id, Name);
                      if (rt != "")
                      {
                          var msgs = rt.ToObj<List<WCFNewMsg>>();
                          msgs.ForEach(p =>
                          {
                              System.Reflection.PropertyInfo property = GetType().GetProperty(p.PropertyName);
                              object rtOjb = RegWebDataProperty[p.PropertyName].Invoke(property.GetValue(this));
                              if (rtOjb != null) Dispatcher.Invoke(() =>
                                 {
                                     property.SetValue(this, rtOjb);
                                 });
                          });
                      }
                  }
              });
            GetNewWCFMsg_Thread.Start();
        }

        #region 学生信息



        private ObservableCollection<StudentMsg> _StudentMsgs = new ObservableCollection<StudentMsg>();
        /// <summary>
        /// 学生信息列表
        /// </summary>
        public ObservableCollection<StudentMsg> StudentMsgs
        {
            get
            {
                return _StudentMsgs;
            }
            set
            {
                _StudentMsgs = value;
                NotifyPropertyChanged(nameof(StudentMsgs));
            }
        }

        /// <summary>
        /// 添加学生信息
        /// 客户端只能添加id
        /// </summary>
        /// <param name="studentMsg"></param>
        public bool AddStudentMsg(Guid  StudentId, Guid RuleId)
        {
            return DataServer.AddStudentMsg(Name, StudentId.ToString(), RuleId.ToString());
        }
        /// <summary>
        /// 添加学生信息
        /// 客户端只能添加id
        /// </summary>
        /// <param name="studentMsg"></param>
        public Task<bool> AddStudentMsgAsync(Guid StudentId, Guid RuleId)
        {
            return DataServer.AddStudentMsgAsync(Name, StudentId.ToString(), RuleId.ToString());
        }
        #endregion


        #region 学生

        private ObservableCollection<Student> _Students = new ObservableCollection<Student>();
        /// <summary>
        /// 学生列表
        /// </summary>
        public ObservableCollection<Student> Students
        {
            get
            {
                return _Students;
            }
            set
            {
                _Students = value;
                NotifyPropertyChanged(nameof(Students));
            }
        }


        #endregion

        #region 班规

        private ObservableCollection<Rule> _Rules = new ObservableCollection<Rule>();
        /// <summary>
        /// 班规列表
        /// </summary>
        public ObservableCollection<Rule> Rules
        {
            get
            {
                return _Rules;
            }
            set
            {
                _Rules = value;
                NotifyPropertyChanged(nameof(Rules));
            }
        }
        /// <summary>
        /// 保存修改过的Rule
        /// </summary>
        /// <param name="ChangeRules"></param>
        public Task<bool> SaveChangeRules(List<Rule> ChangeRules)
        {
            return DataServer.UpdateRuleAsync(Name, ChangeRules.ToJson());
        }
        /// <summary>
        /// 新建并添加
        /// </summary>
        /// <returns></returns>
        public Rule LocalAddRule()
        {
            Rule rule = new Rule
            {
                Name = Name,
                JCRule = JCRules.Count == 0 ? new JCRule() : JCRules.First()
            };
            Rules.Add(rule);
            return rule;
        }

        #endregion

        #region 奖惩

        private ObservableCollection<JCRule> _JCRules = new ObservableCollection<JCRule>();
        /// <summary>
        /// 奖惩列表
        /// </summary>
        public ObservableCollection<JCRule> JCRules
        {
            get
            {
                return _JCRules;
            }
            set
            {
                _JCRules = value;
                NotifyPropertyChanged(nameof(JCRules));
            }
        }
        /// <summary>
        /// 新建并添加
        /// </summary>
        /// <returns></returns>
        public JCRule LocalAddJCRule()
        {
            JCRule jCRule = new JCRule()
            {
                Name = Name
            };
            JCRules.Add(jCRule);
            return jCRule;
        }
        /// <summary>
        /// 保存修改过的JCRule
        /// </summary>
        /// <param name="ChangeRules"></param>
        public Task<bool> SaveChangeJCRules(List<JCRule> ChangeJCRules)
        {
            return DataServer.UpDateJCRuleAsync(Name, ChangeJCRules.ToJson());
        }
        #endregion

        #region 其他数据



        private List<StudentSumPoint> _StudentSumPoints;
        /// <summary>
        /// 学生总分信息
        /// </summary>
        public List<StudentSumPoint> StudentSumPoints
        {
            get { return _StudentSumPoints; }
            set
            {
                _StudentSumPoints = value;
                NotifyPropertyChanged(nameof(StudentSumPoints));
            }
        }


        private Setting _Setting = new Setting();
        /// <summary>
        /// 设置
        /// </summary>
        public Setting Setting
        {
            get { return _Setting; }
            set
            {
                _Setting = value;
                NotifyPropertyChanged(nameof(Setting));
            }
        }
        /// <summary>
        /// 保存Setting
        /// </summary>
        /// <returns></returns>
        public Task<bool> SaveSetting()
        {
            var rt = DataServer.UpdateSettingAsync(Name, Setting.ToJson());
            //Setting = null;
            return rt;
        }

        #endregion
    }

    
}
