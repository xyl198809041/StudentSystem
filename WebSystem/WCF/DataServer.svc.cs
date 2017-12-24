using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DataSystem;
using System.Threading;
using DataSystem.DB;
using System.Windows.Input;

namespace WebSystem.WCF
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“DataServer”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 DataServer.svc 或 DataServer.svc.cs，然后开始调试。
    public class DataServer : IDataServer
    {
        public static ClientList ClientList = ClientList.Current;


        public void DoWork(string n)
        {
            ClientList.AddNewMsg(n, nameof(Data.StudentMsgs));

        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public string Login(string UserName, string Password)
        {
            return DataList.Current.Login(UserName, Password).ToJson();
        }



        public string GetNewMsg(string Guid, string Name)
        {
            ClientList.Link(Guid, Name);

            int n = 0;
            while (n++ <= 10)
            {
                var rt = ClientList.GetNewMsg(Guid);
                if (rt == null) Thread.Sleep(1000);
                else return rt.ToJson();
            }
            return "";
        }

        #region 学生信息
        public string GetStudentMsgs(string Name)
        {
            lock (DataList.Current[Name].StudentMsgs)
            {
                return DataList.Current[Name].StudentMsgs.Where(p => p.State > 0).ToList().ToJson();
            }
        }
        
        public bool AddStudentMsg(string Name,string StudentId,string RuleId)
        {
            StudentMsg msg= new StudentMsg()
            {
                StudentId = Guid.Parse( StudentId),
                RuleId=new Guid(RuleId)
            };
            lock (DataList.Current[Name].StudentMsgs)
            {
                DataList.Current[Name].AddStudentMsg(msg);
            }
            ClientList.AddNewMsg(Name, nameof(Data.StudentMsgs));
            return true;
        }

        #endregion




        #region 学生
        public string GetStudents(string Name)
        {
            return DataList.Current[Name].Students.ToJson();
        }
        #endregion

        #region 班规
        public string GetRules(string Name)
        {
            return DataList.Current[Name].Rules.ToJson();
        }

        public void UserActionByJCRule(string StudentMsgId, string JCRuleId,string CommandName, string Name)
        {
            JCRule jCRule = DataList.Current[Name].JCRules.Find(p => p.Id.ToString() == JCRuleId);
            ICommand command = (ICommand)jCRule.ExJCRule.GetType().GetProperty(CommandName).GetValue(jCRule.ExJCRule);
            command.Execute(DataList.Current[Name].StudentMsgs.Find(p => p.Id.ToString() == StudentMsgId));
        }
        public bool UpdateRule(string Name, string Rule)
        {
            List<Rule> rList = Rule.ToObj<List<Rule>>();
            var New_Rlist = rList.Select(r =>
            {
                Rule New_Rule = DataList.Current[Name].Rules.Where(p => p.Id == r.Id).FirstOrDefault();
                if (New_Rule == null) New_Rule = new Rule() { Name = Name };
                New_Rule.JCRuleId = r.JCRuleId;
                New_Rule.Tilte = r.Tilte;
                New_Rule.Point = r.Point;
                New_Rule.Group = r.Group;
                New_Rule.ExDataStr = r.ExDataStr;
                return New_Rule;
            }).ToList();
            lock (DataList.Current[Name].Rules)
            {
                DataList.Current[Name].AddRule(New_Rlist.Where(p => !DataList.Current[Name].Rules.Contains(p)).ToArray());
            }
            ClientList.AddNewMsg(Name, nameof(Data.Rules));
            ClientList.AddNewMsg(Name, nameof(Data.StudentMsgs));
            return true;
        }


        #endregion

        #region 奖惩


        public string GetJCRules(string Name)
        {
            return DataList.Current[Name].JCRules.ToJson();
        }



        public bool UpDateJCRule(string Name, string JCRule)
        {
            List<JCRule> rList = JCRule.ToObj<List<JCRule>>();
            var New_Rlist = rList.Select(r =>
            {
                JCRule New_JCRule = DataList.Current[Name].JCRules.Where(p => p.Id == r.Id).FirstOrDefault();
                if (New_JCRule == null) New_JCRule = new JCRule() { Name = Name };
                New_JCRule.Tilte = r.Tilte;
                New_JCRule.ExDataStr = r.ExDataStr;
                New_JCRule.MissionMsg = r.MissionMsg;
                New_JCRule.IsMission = r.IsMission;
                New_JCRule.JCRuleType = r.JCRuleType;
                return New_JCRule;
            }).ToList();
            lock (DataList.Current[Name].JCRules)
            {
                DataList.Current[Name].AddJCRule(New_Rlist.Where(p => !DataList.Current[Name].JCRules.Contains(p)).ToArray());
            }
            ClientList.AddNewMsg(Name, nameof(Data.JCRules));
            ClientList.AddNewMsg(Name, nameof(Data.Rules));
            ClientList.AddNewMsg(Name, nameof(Data.StudentMsgs));
            return true;
        }



        #endregion



        #region 其他

        /// <summary>
        /// 获取学生总分
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public string GetStudentSumPoints(string Name)
        {
            return DataList.Current[Name].GetStudentSumPoints().ToJson();
        }


        #endregion

    }

}
