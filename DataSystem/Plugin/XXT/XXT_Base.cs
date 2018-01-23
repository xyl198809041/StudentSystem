using DataSystem.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataSystem.Plugin.XXT
{
    /// <summary>
    /// 短信发送频率
    /// </summary>
    public enum SendSMSEvery
    {
        /// <summary>
        /// 一次,当天任务内容
        /// </summary>
        Once = 0,
        /// <summary>
        /// 每周,小结
        /// </summary>
        Week = 2,
        /// <summary>
        /// 每天,小结
        /// </summary>
        Day = 1,
        
    }




    /// <summary>
    /// 校讯通基类
    /// </summary>
    public abstract class XXT_Base :Plugin_Base
    {

        public override void Load(string Name)
        {
            base.Load(Name);
            //短信服务启动
            FluentScheduler.JobManager.AddJob(CheckNeedSendMsg, p => p.ToRunEvery(1).Minutes());
            lock (typeof(XXT_Base))
            {
                if (!IsSendTestSMS)
                {

                    //SendMsg("15858291872", $"短信服务已开启\\n服务类型:{GetType().Name}\\n日期:{DateTime.Now.ToShortDateString()}\\n时间:{DateTime.Now.ToShortTimeString()}");

                    IsSendTestSMS = true;
                }
            }
            //短信生成任务
            FluentScheduler.JobManager.AddJob(BuildSMSEveryDay, p => p.ToRunEvery(1).Days().At(17, 45));
            //FluentScheduler.JobManager.AddJob(BuildSMSEveryWeek, p => p.ToRunEvery(1).Weeks().On(DayOfWeek.Friday).At(17, 45));

        }
        /// <summary>
        /// 检查短信列表中未发送短信,并发送
        /// </summary>
        private void CheckNeedSendMsg()
        {
            List<SendMsgInfo> list;
            bool NeedSave = false;
            lock (DataList.Current[Name].PluginManager.SendMsgInfos)
            {
                list = DataList.Current[Name].PluginManager.SendMsgInfos.Where(p => !p.Succeed).ToList();
            }
            list.ForEach(p =>
            {
                SendMsg(p);
                NeedSave = true;
            });
            if (NeedSave) DataList.Current.DB_Save();
        }

        private static bool IsSendTestSMS = false;

        /// <summary>
        /// 内部发送短信
        /// </summary>
        /// <param name="PhoneNum"></param>
        /// <param name="SMSText"></param>
        /// <returns></returns>
        protected abstract bool SendMsg(string PhoneNum,string SMSText);
        /// <summary>
        /// 发送短信
        /// 两次尝试
        /// </summary>
        /// <param name="Msg"></param>
        /// <returns></returns>
        public bool SendMsg(SendMsgInfo Msg)
        {
            if (SendMsg(Msg.Student.PhoneNum, Msg.SMSText))
            {
                Msg.Succeed = true;
                return true;
            }
            var b= SendMsg(Msg.Student.PhoneNum, Msg.SMSText);
            Msg.Succeed = b;
            return b;
        }

        #region 短信生成

        /// <summary>
        /// 信息列表
        /// </summary>
        public IEnumerable<XXT_StudentMsg> XXT_StudentMsg
        {
            get => DataList.Current[Name].StudentMsgs.Select(p => new XXT_StudentMsg(p)).Where(p => p.XXT_Rule.GetXXT_RuleByStateType(p.StudentMsg.State)?.XXT_isNeedSend ?? false);
        }

        /// <summary>
        /// 短信开头
        /// </summary>
        private Func<Student, string> _SMS_KaiTou_Today = p => $"{p.StudentName}家长你好,孩子今天学校学习情况如下:\\n";

        private Func<Student, string> _SMS_JieWei = p => $"谢谢配合\\n{DateTime.Now.ToShortDateString()}";

        /// <summary>
        /// 每天发送,每天任务内容发送,不包含小结
        /// </summary>
        private void BuildSMSEveryDay()
        {
            lock (DataList.Current)
            {

                var MsgListByStudent = XXT_StudentMsg.GroupBy(p => p.StudentMsg.Student).Where(p => DataList.Current[Name].PluginManager.SendMsgInfo_CheckHave(p.Key, SendSMSEvery.Once));

                var SendMsgList= MsgListByStudent.ToList().Select(student =>
                {
                    string SMSText = _SMS_KaiTou_Today.Invoke(student.Key);
                    student.ToList().ForEach(studentmsg =>
                    {
                        int i = 1;
                        SMSText += $"({i++}).{studentmsg.GetSMSText()}\\n";
                    });
                    SMSText += _SMS_JieWei.Invoke(student.Key);
                    return new SendMsgInfo()
                    {
                        Student=student.Key,
                        SMSText = SMSText
                    };
                }).ToArray();

                DataList.Current[Name].PluginManager.AddSendMsgInfo(SendMsgList);
            }
        }

        /// <summary>
        /// 每周发送
        /// </summary>
        private void BuildSMSEveryWeek()
        {

        }

        #endregion
    }

    /// <summary>
    /// 短信发送数据记录
    /// </summary>
    public class SendMsgInfo: ModelBase
    {


        //private Guid _StudentMsgId;
        ///// <summary>
        ///// 信息
        ///// </summary>
        //public Guid StudentMsgId
        //{
        //    get { return _StudentMsgId; }
        //    set
        //    {
        //        _StudentMsgId = value;
        //        NotifyPropertyChanged(nameof(StudentMsgId));
        //    }
        //}


        //private StudentMsg _StudentMsg;
        ///// <summary>
        ///// 信息
        ///// </summary>
        //[ForeignKey(nameof(StudentMsgId))]

        //public StudentMsg StudentMsg
        //{
        //    get { return _StudentMsg; }
        //    set
        //    {
        //        _StudentMsg = value;
        //        NotifyPropertyChanged(nameof(StudentMsg));
        //    }
        //}



        private Guid _StudentId;
        /// <summary>
        /// 学生
        /// </summary>
        public Guid StudentId
        {
            get { return _StudentId; }
            set
            {
                _StudentId = value;
                NotifyPropertyChanged(nameof(StudentId));
            }
        }


        private Student _Student;
        /// <summary>
        /// 学生
        /// </summary>
        [ForeignKey(nameof(StudentId))]
        public Student Student
        {
            get { return _Student; }
            set
            {
                _Student = value;
                NotifyPropertyChanged(nameof(Student));
            }
        }

        public SendSMSEvery SendSMSEvery { get; set; } = SendSMSEvery.Once;

        private string  _SMSText;
        /// <summary>
        /// 短信内容
        /// </summary>
        public string  SMSText
        {
            get { return _SMSText; }
            set
            {
                _SMSText = value;
                NotifyPropertyChanged(nameof(SMSText));
            }
        }



        private DateTime _SendTime = DateTime.MaxValue;
        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime SendTime
        {
            get { return _SendTime; }
            set
            {
                _SendTime = value;
                NotifyPropertyChanged(nameof(SendTime));
            }
        }



        private bool _Succeed = false;
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Succeed
        {
            get { return _Succeed; }
            set
            {
                _Succeed = value;
                if (value) SendTime = DateTime.Now;
                NotifyPropertyChanged(nameof(Succeed));
            }
        }




    }
}
