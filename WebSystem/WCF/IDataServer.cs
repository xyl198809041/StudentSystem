using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WebSystem.WCF
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IDataServer”。
    [ServiceContract(Namespace =nameof(DataServer))]
    public interface IDataServer
    {
        [OperationContract]
        void DoWork(string b);
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        [OperationContract]
        string Login(string UserName, string Password);

        /// <summary>
        /// 获取学生信息数据
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        [OperationContract]
        string GetStudentMsgs(string Name);
        /// <summary>
        /// 添加学生信息
        /// </summary>
        /// <param name="StudentMsg"></param>
        [OperationContract]
        bool AddStudentMsg(string Name, string StudentId,string RuleId);
        /// <summary>
        /// 获取学生列表
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        [OperationContract]
        string GetStudents(string Name);
        /// <summary>
        /// 获取班规列表
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        [OperationContract]
        string GetRules(string Name);
        /// <summary>
        /// 获取新信息
        /// </summary>
        /// <param name="Guid"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        [OperationContract]
        string GetNewMsg(string Guid, string Name);
        /// <summary>
        /// 客户端操作任务
        /// 例如,任务完成
        /// </summary>
        /// <param name="StudentMsgId"></param>
        /// <param name="JCRuleId"></param>
        /// <param name="Name"></param>
        [OperationContract]
        void UserActionByJCRule(string StudentMsgId, string JCRuleId,string CommandName, string Name);


        /// <summary>
        /// 获取奖惩列表
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        [OperationContract]
        string GetJCRules(string Name);

        /// <summary>
        /// 更新或添加Rule
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Rule"></param>
        /// <returns></returns>
        [OperationContract]
        bool UpdateRule(string Name, string Rule);
        /// <summary>
        /// 更新或添加JCRule
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="JCRule"></param>
        /// <returns></returns>
        [OperationContract]
        bool UpDateJCRule(string Name, string JCRule);
        /// <summary>
        /// 获取总分信息
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        [OperationContract]
        string GetStudentSumPoints(string Name);
        /// <summary>
        /// 更新设置
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Property"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        [OperationContract]
        bool UpdateSetting(string Name, string Setting);
        /// <summary>
        /// 获取设置
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        [OperationContract]
        string GetSetting(string Name);
    }
    
}
