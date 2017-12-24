using System;
using System.Collections.Generic;
using DataSystem.DB;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DataSystem.Plugin.XXT
{

    public static class ExClass
    {
        /// <summary>
        /// 短信关键词
        /// </summary>
        public static Dictionary<string, Func<XXT_StudentMsg, string>> XXT_BuildDictionary { get; } = new Dictionary<string, Func<XXT_StudentMsg, string>>()
        {
            {"{姓名}",p=>p.StudentMsg.Student.StudentName },
            {"{分数}",p=>p.StudentMsg.Point.ToString() }
        };
        /// <summary>
        /// 短信频率
        /// </summary>
        public static List<SendSMSEvery> SendSMSEveryList { get; } = Enum.GetValues(typeof(SendSMSEvery)).Cast<SendSMSEvery>().ToList();
    }

    public class XXT_RuleByStateType: NotifyPropertyChangedBase
    {

        private StudentMsg.StateType _StateType;
        /// <summary>
        /// 状态类型
        /// </summary>
        public StudentMsg.StateType StateType
        {
            get { return _StateType; }
            set
            {
                _StateType = value;
                NotifyPropertyChanged(nameof(StateType));
            }
        }


        private string _Name;
        /// <summary>
        /// 显示名字
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                NotifyPropertyChanged(nameof(Name));
            }
        }


        private bool _XXT_isNeedSend;
        /// <summary>
        /// 是否发送短信
        /// </summary>
        public bool XXT_isNeedSend
        {
            get { return _XXT_isNeedSend; }
            set
            {
                _XXT_isNeedSend = value;
                NotifyPropertyChanged(nameof(XXT_isNeedSend));
            }
        }


        private string _XXT_SMSText;
        /// <summary>
        /// 短信内容模板
        /// </summary>
        public string XXT_SMSText
        {
            get { return _XXT_SMSText; }
            set
            {
                _XXT_SMSText = value;
                NotifyPropertyChanged(nameof(XXT_SMSText));
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }

   
    

    /// <summary>
    /// 班规短信操作类
    /// </summary>
    public class XXT_Rule:NotifyPropertyChangedBase
    {
        

        public XXT_Rule(Rule Rule)
        {
            this.Rule = Rule;
        }
        /// <summary>
        /// 班规
        /// </summary>
        public Rule Rule { get; private set; }
        /// <summary>
        /// 校讯通规则列表
        /// </summary>
        public List<XXT_RuleByStateType> XXT_Rules
        {
            get
            {
                if (!Rule.ExData.ContainsKey(nameof(XXT_Rules)))
                {
                    var list = new List<XXT_RuleByStateType>();
                    Rule.JCRule.ExJCRule.StateTypes.ToList().ForEach(p =>
                    {
                        list.Add(new XXT_RuleByStateType()
                        {
                            StateType = p.Key,
                            Name = p.Value
                        });
                    });
                    Rule.ExData.Add(nameof(XXT_Rules), list);
                    
                }
                return (List<XXT_RuleByStateType>)Rule.ExData[nameof(XXT_Rules)];
            }
            set
            {
                Rule.ExData[nameof(XXT_Rules)] = value;
                NotifyPropertyChanged(nameof(XXT_Rules));
            }
        }
        /// <summary>
        /// 根据StateType返回规则
        /// </summary>
        /// <param name="stateType"></param>
        /// <returns></returns>
        public XXT_RuleByStateType GetXXT_RuleByStateType(StudentMsg.StateType stateType)
        {
            return XXT_Rules.Where(p => p.StateType == stateType).First();
        }
    }

    public class XXT_StudentMsg:NotifyPropertyChangedBase
    {
        public XXT_StudentMsg(StudentMsg StudentMsg)
        {
            this.StudentMsg = StudentMsg;
        }
        /// <summary>
        /// 学生信息
        /// </summary>
        public StudentMsg StudentMsg { get; private set; }

        /// <summary>
        /// 短信列表
        /// </summary>
        public List<Guid> XXT_SMSList
        {
            get
            {
                if (!StudentMsg.ExData.ContainsKey(nameof(XXT_SMSList)))
                {
                    StudentMsg.ExData.Add(nameof(XXT_SMSList), false);
                }
                return (List<Guid>)StudentMsg.ExData[nameof(XXT_SMSList)];
            }
            set
            {
                StudentMsg.ExData[nameof(XXT_SMSList)] = value;
                NotifyPropertyChanged(nameof(XXT_SMSList));
            }
        }

        private XXT_Rule _XXT_Rule;
        /// <summary>
        /// 校讯通规则
        /// </summary>
        public XXT_Rule XXT_Rule
        {
            get
            {
                if (_XXT_Rule == null)
                {
                    _XXT_Rule = new XXT_Rule(StudentMsg.Rule);
                }
                return _XXT_Rule;
            }
        }

        
        /// <summary>
        /// 根据学生信息,生成动态短信内容
        /// </summary>
        /// <returns></returns>
        public string GetSMSText()
        {
            string SMSText = XXT_Rule.GetXXT_RuleByStateType(StudentMsg.State).XXT_SMSText;
            ExClass.XXT_BuildDictionary.ToList().ForEach(p =>
            {
                SMSText = SMSText.Replace(p.Key, p.Value.Invoke(this));
            });
            return SMSText;
        }
    }
}
