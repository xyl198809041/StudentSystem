namespace DataSystem.DB
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using System.Threading;

    public partial class SystemDataDB : DbContext
    {
        public SystemDataDB()
            : base("name=SystemDataDB")
        {
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }

        /// <summary>
        /// 用户表
        /// </summary>
        public DbSet<User> Users { get; set; }
        /// <summary>
        /// 测试数据表
        /// </summary>
        public DbSet<Test_Model> Test_Models { get; set; }
        /// <summary>
        /// 学生
        /// </summary>
        public DbSet<Student> Students { get; set; }

        /// <summary>
        /// 规则列表
        /// </summary>
        public DbSet<JCRule> JCRules { get; set; }
        /// <summary>
        /// 班规列表
        /// </summary>
        public DbSet<Rule> Rules { get; set; }
        /// <summary>
        /// 信息列表
        /// </summary>
        public DbSet<StudentMsg> StudentMsgs { get; set; }
        /// <summary>
        /// 设置(每个Name一个)
        /// </summary>
        public DbSet<Setting> Settings { get; set; }
        /// <summary>
        /// 短信发送记录
        /// </summary>
        public DbSet<Plugin.XXT.SendMsgInfo> SendMsgInfos { get; set; }
        /// <summary>
        /// 考试成绩数据
        /// </summary>
        public DbSet<TestPoint> TestPoints { get; set; }

        /// <summary>
        /// 考试信息设置
        /// 需要设置的信息填在这里
        /// </summary>
        public DbSet<TestSet> TestSets { get; set; }

    }

    public interface IDataModel
    {
        /// <summary>
        /// 所有者
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// 新建时间
        /// </summary>
        DateTime CreateTime { get; set; }
        /// <summary>
        /// 主键
        /// </summary>
        Guid Id { get; set; }
    }
    /// <summary>
    /// 数据基类
    /// </summary>
    public class ModelBase :NotifyPropertyChangedBase, IDataModel, INotifyPropertyChanged
    {
        /// <summary>
        /// 对象比较
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class ModelComparer<T> : IEqualityComparer<T>
            where T:ModelBase
        {
            public bool Equals(T x, T y)
            {
                if (x == null && y == null) return true;
                else if (x == null) return false;
                else if (y == null) return false;
                else return x.Id == y.Id;
            }

            public int GetHashCode(T obj)
            {
                return obj.Id.GetHashCode();
            }
        }
        
        /// <summary>
        /// [必要]所有者
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 主键
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();
        /// <summary>
        /// 标记删除
        /// </summary>
        public bool IsDel { get; set; } = false;

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            else if (obj as ModelBase == null) return false;
            else return ((ModelBase)obj).Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <summary>
        /// 扩展数据数据库保存
        /// </summary>
        public string ExDataStr
        {
            get
            {
                return ExData.ToJson();
            }
            set
            {
                if (value != null) ExData = value.ToObj<Dictionary<string, object>>();
            }
        }

        /// <summary>
        /// 扩展数据储存
        /// </summary>
        public Dictionary<string, object> ExData { get; set; } = new Dictionary<string, object>();
    }

    public class Test_Model :ModelBase , IDataModel
    {

    
        /// <summary>
        /// 内容
        /// </summary>
        public string Str { get; set; }
    }
    /// <summary>
    /// 用户名角色
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// 学生
        /// </summary>
        Student=5,
        /// <summary>
        /// 教师
        /// </summary>
        Teacher=10,
        /// <summary>
        /// 管理员
        /// </summary>
        Admin=15
    }

    public class User
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; set; } = "";
        /// <summary>
        /// 用户名
        /// </summary>
        [Key]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; set; } = "123456";

        /// <summary>
        /// 角色
        /// </summary>
        public UserRole Role { get; set; } = UserRole.Student;

        /// <summary>
        /// 是否拥有Teacher及以上权限
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public bool IsTeacher { get => (int)Role >= (int)UserRole.Teacher; }
    }

    public class Student:ModelBase
    {
        /// <summary>
        /// [必要]
        /// 学生姓名
        /// </summary>
        public string StudentName { get; set; }

        /// <summary>
        /// [必要]
        /// 电话
        /// </summary>
        public string PhoneNum { get; set; }

        private string _PY;
        /// <summary>
        /// 拼音首字母
        /// </summary>
        public string PY
        {
            get
            {
                if (_PY == null)
                {
                    _PY = StudentName.ToPinYinAbbr();
                }
                return _PY;
            }
        }

        public override string ToString()
        {
            return StudentName;
        }
    }




    public class Rule : ModelBase
    {
        private string _Tilte = "班规名称";
        /// <summary>
        /// 名称
        /// </summary>
        public string Tilte
        {
            get { return _Tilte; }
            set
            {
                _Tilte = value;
                NotifyPropertyChanged(nameof(Tilte));
            }
        }

        private string _Group = "测试";
        /// <summary>
        /// 分组
        /// </summary>
        public string Group
        {
            get { return _Group; }
            set
            {
                _Group = value;
                NotifyPropertyChanged(nameof(Group));
            }
        }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string ValueTilte { get { return $"{Group}-{Tilte}"; } }
        /// <summary>
        /// 分数
        /// </summary>
        public int Point { get; set; } = 0;
        /// <summary>
        /// 信息列表
        /// </summary>
        [JsonIgnore]
        public List<StudentMsg> StudentMsgs { get; set; } = new List<StudentMsg>();
        /// <summary>
        /// 规则id
        /// </summary>
        public Guid JCRuleId { get; set; }

        private JCRule _JCRule;
        /// <summary>
        /// 规则
        /// </summary>
        [ForeignKey(nameof(JCRuleId))]
        public JCRule JCRule
        {
            get { return _JCRule; }
            set
            {
                _JCRule = value;
                NotifyPropertyChanged(nameof(JCRule));
            }
        }
        /// <summary>
        /// 班规运行
        /// 统筹运行,无需调用
        /// </summary>
        public bool Run()
        {
            bool save = false;
            for (int i = 0; i < StudentMsgs.Count; i++)
            {
                if (JCRule.ExJCRule.Run(StudentMsgs[i])) save = true;
            }
            return save;
        }

        /// <summary>
        /// 显示名称
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ValueTilte;
        }


    }

    public class StudentMsg : ModelBase
    {
        /// <summary>
        /// 班规id
        /// </summary>
        public Guid RuleId { get; set; }
        /// <summary>
        /// 班规
        /// </summary>
        [ForeignKey(nameof(RuleId))]
        public Rule Rule { get; set; }
        /// <summary>
        /// 学生id
        /// </summary>
        public Guid StudentId { get; set; }

        /// <summary>
        /// 学生
        /// </summary>
        [ForeignKey(nameof(StudentId))]
        public Student Student { get; set; }
        /// <summary>
        /// 分数
        /// </summary>
        public int Point { get; set; }

        /// <summary>
        /// 状态类型
        /// 状态
        /// 大于0      显示
        /// 小于0      存档 不显示
        /// 0-5        任务完成,任务列表不显示
        /// 6-10       正常显示
        /// 小于-10    系统特殊操作
        /// </summary>
        public enum StateType
        {
            /// <summary>
            /// 初始化
            /// </summary>
            Ini = 0,
            /// <summary>
            /// 正常显示
            /// </summary>
            Value = 10,
            /// <summary>
            /// 超时完成
            /// </summary>
            MissionOutTime=8,
            /// <summary>
            /// 任务完成
            /// </summary>
            MissionDone=2,
            /// <summary>
            /// 任务失败
            /// </summary>
            MissionFail=1,
            /// <summary>
            /// 存档保存
            /// </summary>
            Save = -1,
            /// <summary>
            /// 删除分数
            /// </summary>
            DelNum = -11,
            /// <summary>
            /// 删除记录
            /// </summary>
            DelMsg = -12
        }

        private StateType _State = StateType.Ini;
        /// <summary>
        /// 状态
        /// 大于0      显示
        /// 小于0      存档 不显示
        /// 0-5        任务完成,任务列表不显示
        /// 6-10       正常显示
        /// 小于-10    系统特殊操作
        /// </summary>
        public StateType State
        {
            get { return _State; }
            set
            {
                _State = value;
            }
        }
        /// <summary>
        /// 设置状态并备份
        /// 系统和用户操作一般用这个
        /// </summary>
        /// <param name="State"></param>
        public void SetStateType(StateType State)
        {
            lock (this)
            {
                this.State = State;
                DateTime dateTime = DateTime.Now;
                Thread.Sleep(1);
                StateLog.Add(dateTime, State);
            }
        }

        private Dictionary<DateTime, StateType> _StateLog;
        /// <summary>
        /// 操作日志
        /// </summary>
        [JsonIgnore]
        [NotMapped]
        public Dictionary<DateTime, StateType> StateLog
        {
            get
            {
                if (_StateLog == null)
                {
                    if (!ExData.ContainsKey(nameof(StateLog)))
                    {
                        ExData.Add(nameof(StateLog), new Dictionary<DateTime, StateType>());
                    }
                    _StateLog = (Dictionary<DateTime, StateType>)ExData[nameof(StateLog)];
                }
                return _StateLog;
            }
        }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public DateTime LastUpdateTime
        {
            get
            {
                if (StateLog.Count == 0) return CreateTime;
                else return StateLog.Last().Key;
            }
        }

    }


    public class Setting : ModelBase
    {

        private string _ClassAppMsg = "测试";
        /// <summary>
        /// 软件显示消息
        /// </summary>
        public string ClassAppMsg
        {
            get { return _ClassAppMsg; }
            set
            {
                _ClassAppMsg = value;
                NotifyPropertyChanged(nameof(ClassAppMsg));
            }
        }



    }

    /// <summary>
    /// 考试成绩
    /// 不作为归属，实时统计使用
    /// </summary>
    public class TestPoint
    {
        /// <summary>
        /// id
        /// </summary>
        public Guid Id { get; set; } = new Guid();

        /// <summary>
        /// 姓名
        /// </summary>
        [MaxLength(255)]
        public string StudentName { get; set; }
        /// <summary>
        /// 班级
        /// </summary>
        public int Class { get; set; }
        /// <summary>
        /// 数学成绩
        /// </summary>
        public double ShuXue_Point { get; set; }
        
        public double YuWen_Point { get; set; }
        
        public double YingYu_Point { get; set; }
        
        public double KeXue_Point { get; set; }
        
        public double SiZheng_Point { get; set; }
        /// <summary>
        /// 导入统一id
        /// </summary>
        public Guid TestSetId { get; set; }
        /// <summary>
        /// 考试相关设置
        /// </summary>
        [ForeignKey(nameof(TestSetId))]
        public TestSet TestSet { get; set; }
    }

    public class TestSet
    {

        public Guid Id { get; set; }
        /// <summary>
        /// 考试
        /// </summary>
        public List<TestPoint> TestPoints { get; set; }

        /// <summary>
        /// 年级
        /// </summary>
        public int Grade { get; set; }
        /// <summary>
        /// 考试年份
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// 考试月份
        /// </summary>
        public int Month { get; set; }
        /// <summary>
        /// 考试名称
        /// </summary>
        [MaxLength(255)]
        public string TestName { get; set; }
    }
}
