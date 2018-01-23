using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DataSystem;
using DataSystem.DB;

namespace WebSystem.WCF
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“ServiceAndroid”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 ServiceAndroid.svc 或 ServiceAndroid.svc.cs，然后开始调试。
    public class ServiceAndroid : IServiceAndroid
    {
        private static DataServer dataServer = new DataServer();

        public void DoWork()
        {
        }

        public string Login(string UserName,string Password)
        {
            User user = DataList.Current.Login(UserName, Password);
            return user.Name;
        }

        public string Students(string Name)
        {
            if (DataList.Current.Count(p => p.Name == Name) == 0) return "";
            var rt = DataList.Current[Name].Students.Select(p => new { id = p.Id, name = p.Name, studentname = p.StudentName }).ToList().ToJsonForWeb();
            return rt;
        }

        public string Rules(string Name)
        {
            if (DataList.Current.Count(p => p.Name == Name) == 0) return "";
            var rt = DataList.Current[Name].Rules.GroupBy(p => p.Group).Select(p => new { Group = p.Key, RuleItems = p.Select(q => new { id = q.Id, tilte = q.Tilte }).ToList() }).ToList().ToJsonForWeb();
            return rt;
        }
        /// <summary>
        /// 1   成功
        /// 0   失败
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="StudentId"></param>
        /// <param name="RuleId"></param>
        /// <returns></returns>
        public string AddStuentMsg(string Name, string StudentId, string RuleId)
        {
            if (DataList.Current.Count(p => p.Name == Name) == 0) return "0";
            Rule rule = DataList.Current[Name].Rules.Find(p => p.Id.ToString() == RuleId);
            Student student = DataList.Current[Name].Students.Find(p => p.Id.ToString() == StudentId);
            if (rule == null || student == null) return "0";
            return dataServer.AddStudentMsg(Name, StudentId, RuleId) ? "1" : "0";
        }

        public string StudentMsgs(string Name)
        {
            throw new NotImplementedException();
        }

        public string StudentMsgsByStudent(string Name, string StudentId)
        {
            throw new NotImplementedException();
        }
    }
}
