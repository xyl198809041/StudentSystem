using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using DataSystem.DB;
using System.Threading;
using System.Data.Entity.Infrastructure;

namespace DataSystem
{
    /// <summary>
    /// 每个用户的数据
    /// </summary>
    public class DataByName
    {

        #region 系统
        private SystemDataDB _DB;
        private User _user;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="user"></param>
        /// <param name="DB"></param>
        public DataByName(User user, SystemDataDB DB)
        {
            _user = user;
            _DB = DB;
        }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Name
        {
            get
            {
                return _user.Name;
            }
        }
        /// <summary>
        /// 用户名
        /// </summary>
        public User User
        {
            get
            {
                return _user;
            }
        }
        /// <summary>
        /// 加载数据
        /// </summary>
        public void Load()
        {
            //加载数据
            var d1 = JCRules;
            var d2 = Rules;
            var d4 = Students;
            var d3 = StudentMsgs;


            //end

            //插件管理系统初始化
            PluginManager = new Plugin.PluginManager(Name);
            PluginManager.Load();

            //班规处理线程启动
            FluentScheduler.JobManager.AddJob(RuleRun, p => p.ToRunEvery(1).Seconds());
            //end
        }
        /// <summary>
        /// 直接访问数据库,原则上不直接操作
        /// </summary>
        public SystemDataDB DB
        {
            get
            {
                return _DB;
            }
        }
        /// <summary>
        /// 插件管理系统
        /// </summary>
        public Plugin.PluginManager PluginManager { get; private set; }
        #endregion


        #region 测试
        /// <summary>
        /// 
        /// </summary>
        public IQueryable<Test_Model> TestModels
        {
            get
            {
                return _DB.Test_Models.Where(p => p.Name == Name);
            }
        }

        #endregion

        #region 学生

        private List<Student> _Students;
        public List<Student> Students
        {
            get
            {
                if (_Students == null)
                {
                    _Students = new List<Student>(DB.Students.Where(p => p.Name == Name));
                }
                return _Students;
            }
        }
        /// <summary>
        /// 添加学生
        /// </summary>
        /// <param name="student"></param>
        public void AddStudent(params Student[] student)
        {
            student.ToList().ForEach(p =>
            {
                p.Name = Name;
                DB.Students.Add(p);
                Students.Add(p);
            });
            DataList.Current.DB_Save();
        }

        #endregion


        #region 规则

        private List<JCRule> _JCRules;
        /// <summary>
        /// 规则列表
        /// </summary>
        public List<JCRule> JCRules
        {
            get
            {
                if (_JCRules == null)
                {
                    _JCRules = new List<JCRule>(DB.JCRules.Where(p => p.Name == Name));
                }
                return _JCRules;
            }
        }

        /// <summary>
        /// 添加规则
        /// </summary>
        /// <param name="jCRule"></param>
        public void AddJCRule(params JCRule[] jCRule)
        {
            jCRule.ToList().ForEach(p =>
            {
                p.Name=Name;
                DB.JCRules.Add(p);
                JCRules.Add(p);
            });
            DataList.Current.DB_Save();
        }
        #endregion


        #region 班规

        private List<Rule> _Rules;
        /// <summary>
        /// 班规列表
        /// </summary>
        public List<Rule> Rules
        {
            get
            {
                if (_Rules == null)
                {
                    _Rules = new List<Rule>(DB.Rules.Where(p => p.Name == Name));
                }
                return _Rules;
            }
        }

        /// <summary>
        /// 添加班规
        /// </summary>
        /// <param name="rule"></param>
        public void AddRule(params Rule[] rule)
        {
            lock (Rules)
            {
                rule.ToList().ForEach(p =>
                {
                    p.Name = Name;
                    DB.Rules.Add(p);
                    Rules.Add(p);
                });
            }
            DataList.Current.DB_Save();
        }
        
        /// <summary>
        /// 班规处理信息统筹线程
        /// </summary>
        private void RuleRun()
        {
            lock (Rules)
            {
                lock (StudentMsgs)
                {
                    bool save = false;
                    Rules.ForEach(p =>
                    {
                        if (p.Run()) save = true; ;
                    });
                    if (save)
                    {
                        DataList.Current.DB_Save();
                        DataList.Current.SendDataChanged(Name, nameof(Data.StudentMsgs));
                    }
                }
            }
        }

        #endregion



        #region 信息
        private List<StudentMsg> _StudentMsgs;
        /// <summary>
        /// 信息列表
        /// </summary>
        public List<StudentMsg> StudentMsgs
        {
            get
            {
                if (_StudentMsgs == null)
                {
                    _StudentMsgs = new List<StudentMsg>(DB.StudentMsgs.Where(p => p.Name == Name));
                }
                return _StudentMsgs;
            }
        }

        /// <summary>
        /// 日常记录添加
        /// 添加信息
        /// </summary>
        /// <param name="studentMsg"></param>
        public void AddStudentMsg(params StudentMsg[] studentMsg)
        {
            studentMsg.ToList().ForEach(p =>
            {
                p.Name = Name;
                DB.StudentMsgs.Add(p);
                StudentMsgs.Add(p);
            });
            DataList.Current.DB_Save();
        }
        #endregion


        #region 其他

        public List<StudentSumPoint> GetStudentSumPoints()
        {
            DateTime Today = DateTime.Now.Date;
            //上周一
            DateTime LastWeek1 = DateTime.Now.Date.AddDays(Convert.ToInt32(1 - Convert.ToInt32(DateTime.Now.DayOfWeek)) - 7);
            //本周一
            DateTime ThisWeek1 = DateTime.Now.Date.AddDays(Convert.ToInt32(1 - Convert.ToInt32(DateTime.Now.DayOfWeek)));
            return DB.StudentMsgs.Where(p => p.Name == Name && p.CreateTime < Today && p.State < 0).GroupBy(p => p.Student).Select(p => new StudentSumPoint()
            {
                SumPoint = p.Sum(q => q.Point),
                LastWeekPoint = p.Where(q => q.CreateTime < ThisWeek1 && q.CreateTime >= LastWeek1).Sum(q => (int?)q.Point) ?? 0,
                ThisWeekPoint = p.Where(q => q.CreateTime >= ThisWeek1).ToList().Sum(q => (int?)q.Point) ?? 0,
                Student = p.Key
            }).ToList();
        }

        #endregion


    }

    /// <summary>
    /// 数据表
    /// 服务器端使用数据
    /// </summary>
    public class DataList:List<DataByName>
    {
        #region 系统设置

        #endregion



        private static DataList _Current;
        /// <summary>
        /// 当前实例
        /// </summary>
        public static DataList Current
        {
            get
            {
                if (_Current == null)
                {
                    _Current = new DataList();
                    _Current.Load();
                }
                return _Current;
            }
        }
        /// <summary>
        /// 隐藏构造函数
        /// </summary>
        private DataList() { }

        private SystemDataDB _DB = new SystemDataDB();
        
        /// <summary>
        /// 加载数据
        /// </summary>
        private void Load()
        {
            _DB.Users.ToList().ForEach(p =>
            {
                var data = new DataByName(p,_DB);
                data.Load();
                Add(data);
            });
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public User Login(string UserName,string Password)
        {
            var users= _DB.Users.Where(p => p.UserName == UserName && p.PassWord == p.PassWord);
            if (users.Count() == 0) return new User();
            else return users.First();
        }
        /// <summary>
        /// 数据库保存
        /// </summary>
        public void DB_Save()
        {
            lock (_DB)
            {
                try
                {
                    _DB.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    // Update original values from the database 
                    var entry = ex.Entries.Single();
                    entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                }
            }
        }
        /// <summary>
        /// 获取指定用户名的数据
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public DataByName this[string Name]
        {
            get
            {
                return this.Where(p => p.Name == Name).FirstOrDefault();
            }
        }
        /// <summary>
        /// 获取指定用户名的数据
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public DataByName this[User user]
        {
            get
            {
                return this[user.Name];
            }
        }
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public string AddUser(User user)
        {
            if (_DB.Users.Find(user.Name) == null)
            {
                _DB.Users.Add(user);
                DB_Save();
                var data = new DataByName(user,_DB);
                data.Load();
                Add(data);
                return SystemMsg.Success;
            }
            else return SystemMsg_DB.HaveUser;
        }

        #region WCF消息发送
        /// <summary>
        /// WCF新信息监听
        /// 数据名,注册方法名
        /// </summary>
        public event Action<string, string> DataChanged;
        /// <summary>
        /// 数据更新消息发送方法
        /// 数据更新提示客户端时使用
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="PropertyName"></param>
        public void SendDataChanged(string Name,string PropertyName)
        {
            DataChanged?.Invoke(Name, PropertyName);
        }
        #endregion
    }
    /// <summary>
    /// 新信息返回数据类
    /// </summary>
    public class WCFNewMsg
    {

        /// <summary>
        /// 注册方法对应
        /// </summary>
        public string PropertyName { get; set; }

        

    }
}
