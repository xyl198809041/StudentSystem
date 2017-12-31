using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataSystem.DB
{
    public static class ExClass
    {
        private static readonly TimeSpan StartTime = TimeSpan.Parse("8:00:00");
        private static readonly TimeSpan EndTime = TimeSpan.Parse("16:00:00");

        ///// <summary>
        ///// 距离现在少小时
        ///// 基本测试通过
        ///// </summary>
        ///// <param name="from"></param>
        ///// <returns></returns>
        //public static double ToNewTime(this DateTime From)
        //{
        //    DateTime from = From;
        //    DateTime to = DateTime.Now;
        //    if (to.TimeOfDay > EndTime.TimeOfDay) to = to.Date.Add(EndTime.TimeOfDay);
        //    if (from.TimeOfDay > EndTime.TimeOfDay) from = from.Date.Add(EndTime.TimeOfDay);
        //    if (to.TimeOfDay < StartTime.TimeOfDay) to = to.Date.Add(StartTime.TimeOfDay);
        //    if (from.TimeOfDay < StartTime.TimeOfDay) from = from.Date.Add(StartTime.TimeOfDay);

        //    double TimeHour = (to.TimeOfDay - from.TimeOfDay).TotalHours;
        //    double DateHour = (to.Date - from.Date).Days * (EndTime - StartTime).TotalHours;

        //    DateTime date = from.Date;
        //    while (true)
        //    {
        //        date = date.AddDays(1);
        //        if (date.Date >= to.Date) break;
        //        if (date.DayOfWeek == (DayOfWeek.Saturday | DayOfWeek.Sunday))
        //        {
        //            DateHour -= 8;
        //        }
        //    }
        //    if (to.Date == from.Date)
        //    {
        //        if (to.Date.DayOfWeek == (DayOfWeek.Saturday | DayOfWeek.Sunday))
        //        {
        //            TimeHour -= (to.TimeOfDay - from.TimeOfDay).TotalHours;
        //        }
        //    }
        //    else
        //    {
        //        if (to.Date.DayOfWeek == (DayOfWeek.Saturday | DayOfWeek.Sunday)) TimeHour -= (to.TimeOfDay - StartTime.TimeOfDay).TotalHours;
        //        if (from.Date.DayOfWeek == (DayOfWeek.Saturday | DayOfWeek.Sunday)) TimeHour -= (EndTime.TimeOfDay - from.TimeOfDay).TotalHours;
        //    }

        //    return DateHour + TimeHour;
        //}

        /// <summary>
        /// 获取本周一 ,保留时间部分
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime ThisWeekMonday(this DateTime dateTime)
        {
            return dateTime.AddDays(Convert.ToInt32(1 - Convert.ToInt32(DateTime.Now.DayOfWeek)));
        }
        /// <summary>
        /// 设置时间部分
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="Hour"></param>
        /// <param name="Min"></param>
        /// <returns></returns>
        public static DateTime AtTime(this DateTime dateTime,double Hour,double Min)
        {
            return dateTime.Date.AddHours(Hour).AddMinutes(Min);
        }
        /// <summary>
        /// 距离现在少小时
        /// 基本测试通过
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static double ToNewTime(this DateTime From)
        {
            DateTime from = From;
            DateTime to = DateTime.Now;
            DateTime temp;
            
            DateTime ThisWeek1 = DateTime.Now.Date.AddDays(Convert.ToInt32(1 - Convert.ToInt32(DateTime.Now.DayOfWeek)));

            //周五晚上,周六日
            if (From.DayOfWeek == (DayOfWeek.Saturday | DayOfWeek.Sunday) || (From.DayOfWeek == DayOfWeek.Friday && From.TimeOfDay > EndTime))
            {
                from = From.ThisWeekMonday().AddDays(7).AtTime(8, 0);
            }
            //周一到周五
            else if(From.TimeOfDay<StartTime)
            {
                from = From.Date.AtTime(8, 0);
            }
            else if (From.TimeOfDay > EndTime)
            {
                from = From.Date.AddDays(1).AtTime(8, 0);
            }

            //周六周日
            if(to.DayOfWeek==(DayOfWeek.Saturday | DayOfWeek.Sunday) )
            {
                to = to.ThisWeekMonday().AddDays(4).AtTime(16, 0);
            }
            //周一早上
            else if(to.DayOfWeek == DayOfWeek.Monday && to.TimeOfDay < StartTime)
            {
                to = to.ThisWeekMonday().AddDays(-3).AtTime(16, 0);
            }
            //周一到周五
            else if(to.TimeOfDay<StartTime)
            {
                to = to.Date.AddDays(-1).AtTime(16, 0);
            }
            else if(to.TimeOfDay > EndTime)
            {
                to = to.AtTime(16, 0);
            }

            if (to < from) return 0;

            double Hours = (to - from).TotalHours;
            temp = from.AddDays(1);
            while (temp.Date < to)
            {
                if (temp.DayOfWeek == (DayOfWeek.Saturday | DayOfWeek.Sunday)) Hours -= 1;
                temp= temp.AddDays(1);
            }

            return Hours ;
        }
        /// <summary>
        /// 距离现在多少天,当天算0
        /// </summary>
        /// <param name="From"></param>
        /// <returns></returns>
        public static double ToNewDay(this DateTime From)
        {
            return (DateTime.Now.Date - From.Date).Days;
        }

        public static void UserRunJCRule(this JCRule jcrule,StudentMsg msg,string cmd,Action<StudentMsg> action)
        {
            
        }
    }


    /// <summary>
    /// 普通规则
    /// </summary>
    public class JCRule : ModelBase
    {

        private string _JCRuleType= typeof(ExJCRule).FullName;
        /// <summary>
        /// 规则的类型,记录数据库
        /// </summary>
        public string JCRuleType
        {
            get => _JCRuleType;
            set
            {
                _JCRuleType = value;
                NotifyPropertyChanged(nameof(JCRuleType));
                _ExJCRule = null;
                NotifyPropertyChanged(nameof(ExJCRule));
            }
        }

        private IExJCRule _ExJCRule;
        /// <summary>
        /// 奖惩扩展
        /// </summary>
        public IExJCRule ExJCRule
        {
            get
            {
                if (_ExJCRule == null)
                {
                    _ExJCRule = (IExJCRule)typeof(ExJCRule).Assembly.CreateInstance(JCRuleType, false, System.Reflection.BindingFlags.Default, null, new object[] { this }, null, null);
                }
                return _ExJCRule;
            }
        }

        private string _Tilte = "奖惩";
        /// <summary>
        /// [必要]标题
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
        
        /// <summary>
        /// 是否包含任务
        /// </summary>
        public bool IsMission { get; set; } = false;


        private string _MissionMsg;
        /// <summary>
        /// [必要]任务内容(设置)
        /// </summary>
        public string MissionMsg
        {
            get { return _MissionMsg; }
            set
            {
                _MissionMsg = value;
                NotifyPropertyChanged(nameof(MissionMsg));
            }
        }


        /// <summary>
        /// 规则列表
        /// </summary>
        public List<Rule> Rules { get; set; } = new List<Rule>();

        public override string ToString()
        {
            return Tilte;
        }
    }


    /// <summary>
    /// 规则扩展类
    /// 定义扩展操作Icommand形式
    /// 自定义显示ValueMissionMsg
    /// 自动操作Run
    /// 用户操作UserRun
    /// </summary>
    public interface IExJCRule
    {
        /// <summary>
        /// 数据
        /// </summary>
        JCRule JCRule { get; }
        /// <summary>
        /// 服务器运行
        /// 用于操作
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool Run(StudentMsg msg);
        /// <summary>
        /// 客户端运行
        /// 用于提交服务器
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="CommandName"></param>
        void UserRun(StudentMsg msg, string CommandName);
        /// <summary>
        /// 显示任务内容
        /// </summary>
        string ValueMissionMsg(StudentMsg msg);
        /// <summary>
        /// 状态种类
        /// </summary>
        Dictionary<StudentMsg.StateType, string> StateTypes { get; }

    }
    /// <summary>
    /// 模板定义
    /// </summary>
    public class ExJCRule:IExJCRule
    {
        public ExJCRule() { throw new Exception(); }

        public ExJCRule(JCRule jCRule)
        {
            JCRule = jCRule;
        }

        public JCRule JCRule { get; }


        /// <summary>
        /// 显示动态任务内容
        /// </summary>
        public virtual string ValueMissionMsg(StudentMsg msg)
        {
            return JCRule.MissionMsg;
        }

        /// <summary>
        /// 运行处理msg
        /// 返回是否有更改
        /// </summary>
        /// <param name="msg"></param>
        public virtual bool Run(StudentMsg msg)
        {
            bool save = false;
            //分数初始化
            if (msg.Point != msg.Rule.Point)
            {
                msg.Point = msg.Rule.Point;
                save = true;
            }
            //状态初始化
            if (msg.State == StudentMsg.StateType.Ini)
            {
                msg.SetStateType(StudentMsg.StateType.Value);
                save = true;
            }
            //完成任务当天显示,过后清理
            else if ((int)msg.State >= 1 && (int)msg.State < 5 && msg.LastUpdateTime.Date != DateTime.Now.Date)
            {
                msg.SetStateType(StudentMsg.StateType.Save);
                save = true;
            }
            return save;
        }
        /// <summary>
        /// JCRule用户操作实现方法
        /// ICommand新建时调用即可
        /// </summary>
        /// <param name="Server"></param>
        /// <param name="CommandName"></param>
        /// <param name="Msg"></param>
        protected static void CommandRun(Action<StudentMsg> Server, string CommandName, StudentMsg Msg)
        {
            if (AppSet.IsWeb)
            {
                Server.Invoke(Msg);
                DataList.Current.DB_Save();
                DataList.Current.SendDataChanged(Msg.Name, nameof(Data.StudentMsgs));
            }
            else
            {
                //向服务器发送消息
                Data.DataServer.UserActionByJCRule(Msg.Id.ToString(), Msg.Rule.JCRule.Id.ToString(), CommandName, Msg.Name);
            }
        }
        /// <summary>
        /// 自动完成
        /// </summary>
        public ICommand _Command_JCRule_Down = new DelegateCommand<StudentMsg>((msg) =>
        {
            CommandRun((m) =>
            {
                m.SetStateType(StudentMsg.StateType.MissionDone);
            }, nameof(Command_JCRule_Down), msg);
        });
        /// <summary>
        /// 自动完成
        /// </summary>
        public ICommand Command_JCRule_Down
        {
            get
            {
                return _Command_JCRule_Down;
            }
        }

        public virtual void UserRun(StudentMsg msg, string CommandName)
        {

        }

        /// <summary>
        /// 状态种类
        /// </summary>
        [JsonIgnore]
        [NotMapped]
        public virtual Dictionary<StudentMsg.StateType, string> StateTypes { get; } = new Dictionary<StudentMsg.StateType, string>()
        {
            {StudentMsg.StateType.Value,"创建" },
            {StudentMsg.StateType.MissionDone,"完成" }
        };
    }
    /// <summary>
    /// 定时完成规则
    /// </summary>
    public class ExJCRule_AutoSave :ExJCRule,IExJCRule
    {
        public enum TimeMode
        {
            Hour = 0,
            Day = 1
        }

        public ExJCRule_AutoSave():base() { }
        public ExJCRule_AutoSave(JCRule jCRule) : base(jCRule) { }

        /// <summary>
        /// 自动完成时间
        /// </summary>
        [JsonIgnore]
        [NotMapped]
        public int Hour
        {
            get
            {
                if (!JCRule.ExData.ContainsKey(nameof(Hour)))
                {
                    JCRule.ExData.Add(nameof(Hour), 8);
                }
                return Convert.ToInt32(JCRule.ExData[nameof(Hour)]);
            }
            set
            {
                JCRule.ExData[nameof(Hour)] = value;
            }
        }
        /// <summary>
        /// 自动完成时间
        /// </summary>
        [JsonIgnore]
        [NotMapped]
        public int Day
        {
            get
            {
                if (!JCRule.ExData.ContainsKey(nameof(Day)))
                {
                    JCRule.ExData.Add(nameof(Day), 1);
                }
                return Convert.ToInt32(JCRule.ExData[nameof(Day)]);
            }
            set => JCRule.ExData[nameof(Day)] = value;
        }
        /// <summary>
        /// 保存模式
        /// 时间还是天数
        /// </summary>
        [JsonIgnore]
        [NotMapped]
        public TimeMode AutoSave_TimeMode
        {
            get
            {
                if (!JCRule.ExData.ContainsKey(nameof(AutoSave_TimeMode)))
                {
                    JCRule.ExData.Add(nameof(AutoSave_TimeMode), TimeMode.Hour);
                }
                return (TimeMode)Convert.ToInt32(JCRule.ExData[nameof(AutoSave_TimeMode)]);
            }
            set => JCRule.ExData[nameof(AutoSave_TimeMode)] = value;
        }

        public override bool Run(StudentMsg msg)
        {
            bool save = false;
            if (AutoSave_TimeMode == TimeMode.Hour)
            {
                if (msg.State == StudentMsg.StateType.Value && Hour < msg.CreateTime.ToNewTime())
                {
                    msg.SetStateType(StudentMsg.StateType.MissionDone);
                    save = true;
                }
            }
            else if (AutoSave_TimeMode == TimeMode.Day)
            {
                if(msg.State==StudentMsg.StateType.Value && Day <= msg.CreateTime.ToNewDay())
                {
                    msg.SetStateType(StudentMsg.StateType.MissionDone);
                    save = true;
                }
            }
            return base.Run(msg) || save;
        }


    }

    /// <summary>
    /// 超时增加
    /// </summary>
    public class ExJCRule_AddByTime : ExJCRule, IExJCRule
    {
        public ExJCRule_AddByTime() : base() { }

        public ExJCRule_AddByTime(JCRule jCRule) : base(jCRule) { }

        public override Dictionary<StudentMsg.StateType, string> StateTypes
        {
            get
            {
                var list = base.StateTypes;
                list.Add(StudentMsg.StateType.MissionOutTime, "超时");
                return list;
            }
        }

        /// <summary>
        /// 规定完成时间
        /// </summary>
        [JsonIgnore]
        [NotMapped]
        public int Hour
        {
            get
            {
                if (!JCRule.ExData.ContainsKey(nameof(Hour)))
                {
                    JCRule.ExData.Add(nameof(Hour), 8);
                }
                return Convert.ToInt32(JCRule.ExData[nameof(Hour)]);
            }
            set
            {
                JCRule.ExData[nameof(Hour)] = value;
            }
        }
        /// <summary>
        /// 每增加多少小时,翻倍
        /// </summary>
        [JsonIgnore]
        [NotMapped]
        public int AddHour
        {
            get
            {
                if (!JCRule.ExData.ContainsKey(nameof(AddHour)))
                {
                    JCRule.ExData.Add(nameof(AddHour), 8);
                }
                return Convert.ToInt32(JCRule.ExData[nameof(AddHour)]);
            }
            set
            {
                JCRule.ExData[nameof(AddHour)] = value;
            }
        }
        /// <summary>
        /// 初始完成数量
        /// </summary>
        [JsonIgnore]
        [NotMapped]
        public int StartNum
        {
            get
            {
                if (!JCRule.ExData.ContainsKey(nameof(StartNum)))
                {
                    JCRule.ExData.Add(nameof(StartNum), 1);
                }
                return Convert.ToInt32(JCRule.ExData[nameof(StartNum)]);
            }
            set
            {
                JCRule.ExData[nameof(StartNum)] = value;
            }
        }

        /// <summary>
        /// 翻倍数量
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public int AddNum
        {
            get
            {
                if (!JCRule.ExData.ContainsKey(nameof(AddNum)))
                {
                    JCRule.ExData.Add(nameof(AddNum), 1);
                }
                return Convert.ToInt32(JCRule.ExData[nameof(AddNum)]);
            }
            set => JCRule.ExData[nameof(AddNum)] = value;
        }

        /// <summary>
        /// 数量标记
        /// </summary>
        private static string Num = "{Num}";

        private static ICommand _Command_ExJCRule_AddByTime_Edit_AddNum = new DelegateCommand<ExJCRule_AddByTime>(p =>
          {
              p.JCRule.MissionMsg += Num;
          });
        /// <summary>
        /// 编辑
        /// 加入数量
        /// </summary>
        public ICommand Command_ExJCRule_AddByTime_Edit_AddNum { get => _Command_ExJCRule_AddByTime_Edit_AddNum; }

        public bool SetNum(StudentMsg msg)
        {
            double time= msg.CreateTime.ToNewTime();
            int num;
            if (time < Hour)
            {
                num = StartNum;
            }
            else
            { 
                num= ((int)((time - Hour) / AddHour)) * AddNum + StartNum;
            }
            if (num.ToString() != (string)msg.ExData[Num])
            {
                msg.ExData[Num] = num.ToString();
                return true;
            }
            else return false;
        }

        public override bool Run(StudentMsg msg)
        {
            var save= base.Run(msg);
            if (!msg.ExData.ContainsKey(Num))
            {
                msg.ExData.Add(Num, "");
            }
            save = SetNum(msg);
            if(msg.State==StudentMsg.StateType.Value && msg.CreateTime.ToNewTime() > Hour)
            {
                msg.SetStateType(StudentMsg.StateType.MissionOutTime);
                save = true;
            }

            return save;
        }

        public override string ValueMissionMsg(StudentMsg msg)
        {
            if (msg.ExData.ContainsKey(Num))
            {
                return JCRule.MissionMsg.Replace(Num, (string)msg.ExData[Num]);
            }
            else
            {
                return "";
            }
        }
    }


    public static class JCRuleType
    {
        public static Dictionary<Type, string> JCRuleTypeList { get; } = new Dictionary<Type, string>()
        {
            {typeof(ExJCRule),"普通任务"},
            {typeof(ExJCRule_AutoSave),"自动完成"},
            {typeof(ExJCRule_AddByTime),"超时翻倍" }
        };
    }
}
