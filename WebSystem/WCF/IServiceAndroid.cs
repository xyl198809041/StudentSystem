using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WebSystem.WCF
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IServiceAndroid”。
    [ServiceContract]
    public interface IServiceAndroid
    {
        [OperationContract]
        void DoWork();
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        [OperationContract]
        string Login(string UserName, string Password);

        /// <summary>
        /// 学生列表
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        [OperationContract]
        string Students(string Name);
        /// <summary>
        /// 班规列表
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        [OperationContract]
        string Rules(string Name);
        /// <summary>
        /// 添加记录
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="StudentId"></param>
        /// <param name="RuleId"></param>
        /// <returns></returns>
        [OperationContract]
        string AddStuentMsg(string Name, string StudentId, string RuleId);
        /// <summary>
        /// 获取班级记录
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        [OperationContract]
        string StudentMsgs(string Name);
        /// <summary>
        /// 获取某学生记录
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="StudentId"></param>
        /// <returns></returns>
        [OperationContract]
        string StudentMsgsByStudent(string Name, string StudentId);
    }
}
