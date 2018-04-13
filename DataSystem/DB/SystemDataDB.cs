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
        /// �û���
        /// </summary>
        public DbSet<User> Users { get; set; }
        /// <summary>
        /// �������ݱ�
        /// </summary>
        public DbSet<Test_Model> Test_Models { get; set; }
        /// <summary>
        /// ѧ��
        /// </summary>
        public DbSet<Student> Students { get; set; }

        /// <summary>
        /// �����б�
        /// </summary>
        public DbSet<JCRule> JCRules { get; set; }
        /// <summary>
        /// ����б�
        /// </summary>
        public DbSet<Rule> Rules { get; set; }
        /// <summary>
        /// ��Ϣ�б�
        /// </summary>
        public DbSet<StudentMsg> StudentMsgs { get; set; }
        /// <summary>
        /// ����(ÿ��Nameһ��)
        /// </summary>
        public DbSet<Setting> Settings { get; set; }
        /// <summary>
        /// ���ŷ��ͼ�¼
        /// </summary>
        public DbSet<Plugin.XXT.SendMsgInfo> SendMsgInfos { get; set; }
        /// <summary>
        /// ���Գɼ�����
        /// </summary>
        public DbSet<TestPoint> TestPoints { get; set; }

        /// <summary>
        /// ������Ϣ����
        /// ��Ҫ���õ���Ϣ��������
        /// </summary>
        public DbSet<TestSet> TestSets { get; set; }

    }

    public interface IDataModel
    {
        /// <summary>
        /// ������
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// �½�ʱ��
        /// </summary>
        DateTime CreateTime { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        Guid Id { get; set; }
    }
    /// <summary>
    /// ���ݻ���
    /// </summary>
    public class ModelBase :NotifyPropertyChangedBase, IDataModel, INotifyPropertyChanged
    {
        /// <summary>
        /// ����Ƚ�
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
        /// [��Ҫ]������
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;
        /// <summary>
        /// ����
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();
        /// <summary>
        /// ���ɾ��
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
        /// ��չ�������ݿⱣ��
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
        /// ��չ���ݴ���
        /// </summary>
        public Dictionary<string, object> ExData { get; set; } = new Dictionary<string, object>();
    }

    public class Test_Model :ModelBase , IDataModel
    {

    
        /// <summary>
        /// ����
        /// </summary>
        public string Str { get; set; }
    }
    /// <summary>
    /// �û�����ɫ
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// ѧ��
        /// </summary>
        Student=5,
        /// <summary>
        /// ��ʦ
        /// </summary>
        Teacher=10,
        /// <summary>
        /// ����Ա
        /// </summary>
        Admin=15
    }

    public class User
    {
        /// <summary>
        /// �û���
        /// </summary>
        public string Name { get; set; } = "";
        /// <summary>
        /// �û���
        /// </summary>
        [Key]
        public string UserName { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public string PassWord { get; set; } = "123456";

        /// <summary>
        /// ��ɫ
        /// </summary>
        public UserRole Role { get; set; } = UserRole.Student;

        /// <summary>
        /// �Ƿ�ӵ��Teacher������Ȩ��
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public bool IsTeacher { get => (int)Role >= (int)UserRole.Teacher; }
    }

    public class Student:ModelBase
    {
        /// <summary>
        /// [��Ҫ]
        /// ѧ������
        /// </summary>
        public string StudentName { get; set; }

        /// <summary>
        /// [��Ҫ]
        /// �绰
        /// </summary>
        public string PhoneNum { get; set; }

        private string _PY;
        /// <summary>
        /// ƴ������ĸ
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
        private string _Tilte = "�������";
        /// <summary>
        /// ����
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

        private string _Group = "����";
        /// <summary>
        /// ����
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
        /// ��ʾ����
        /// </summary>
        public string ValueTilte { get { return $"{Group}-{Tilte}"; } }
        /// <summary>
        /// ����
        /// </summary>
        public int Point { get; set; } = 0;
        /// <summary>
        /// ��Ϣ�б�
        /// </summary>
        [JsonIgnore]
        public List<StudentMsg> StudentMsgs { get; set; } = new List<StudentMsg>();
        /// <summary>
        /// ����id
        /// </summary>
        public Guid JCRuleId { get; set; }

        private JCRule _JCRule;
        /// <summary>
        /// ����
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
        /// �������
        /// ͳ������,�������
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
        /// ��ʾ����
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
        /// ���id
        /// </summary>
        public Guid RuleId { get; set; }
        /// <summary>
        /// ���
        /// </summary>
        [ForeignKey(nameof(RuleId))]
        public Rule Rule { get; set; }
        /// <summary>
        /// ѧ��id
        /// </summary>
        public Guid StudentId { get; set; }

        /// <summary>
        /// ѧ��
        /// </summary>
        [ForeignKey(nameof(StudentId))]
        public Student Student { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public int Point { get; set; }

        /// <summary>
        /// ״̬����
        /// ״̬
        /// ����0      ��ʾ
        /// С��0      �浵 ����ʾ
        /// 0-5        �������,�����б���ʾ
        /// 6-10       ������ʾ
        /// С��-10    ϵͳ�������
        /// </summary>
        public enum StateType
        {
            /// <summary>
            /// ��ʼ��
            /// </summary>
            Ini = 0,
            /// <summary>
            /// ������ʾ
            /// </summary>
            Value = 10,
            /// <summary>
            /// ��ʱ���
            /// </summary>
            MissionOutTime=8,
            /// <summary>
            /// �������
            /// </summary>
            MissionDone=2,
            /// <summary>
            /// ����ʧ��
            /// </summary>
            MissionFail=1,
            /// <summary>
            /// �浵����
            /// </summary>
            Save = -1,
            /// <summary>
            /// ɾ������
            /// </summary>
            DelNum = -11,
            /// <summary>
            /// ɾ����¼
            /// </summary>
            DelMsg = -12
        }

        private StateType _State = StateType.Ini;
        /// <summary>
        /// ״̬
        /// ����0      ��ʾ
        /// С��0      �浵 ����ʾ
        /// 0-5        �������,�����б���ʾ
        /// 6-10       ������ʾ
        /// С��-10    ϵͳ�������
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
        /// ����״̬������
        /// ϵͳ���û�����һ�������
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
        /// ������־
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
        /// ������ʱ��
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

        private string _ClassAppMsg = "����";
        /// <summary>
        /// �����ʾ��Ϣ
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
    /// ���Գɼ�
    /// ����Ϊ������ʵʱͳ��ʹ��
    /// </summary>
    public class TestPoint
    {
        /// <summary>
        /// id
        /// </summary>
        public Guid Id { get; set; } = new Guid();

        /// <summary>
        /// ����
        /// </summary>
        [MaxLength(255)]
        public string StudentName { get; set; }
        /// <summary>
        /// �༶
        /// </summary>
        public int Class { get; set; }
        /// <summary>
        /// ��ѧ�ɼ�
        /// </summary>
        public double ShuXue_Point { get; set; }
        
        public double YuWen_Point { get; set; }
        
        public double YingYu_Point { get; set; }
        
        public double KeXue_Point { get; set; }
        
        public double SiZheng_Point { get; set; }
        /// <summary>
        /// ����ͳһid
        /// </summary>
        public Guid TestSetId { get; set; }
        /// <summary>
        /// �����������
        /// </summary>
        [ForeignKey(nameof(TestSetId))]
        public TestSet TestSet { get; set; }
    }

    public class TestSet
    {

        public Guid Id { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public List<TestPoint> TestPoints { get; set; }

        /// <summary>
        /// �꼶
        /// </summary>
        public int Grade { get; set; }
        /// <summary>
        /// �������
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// �����·�
        /// </summary>
        public int Month { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        [MaxLength(255)]
        public string TestName { get; set; }
    }
}
