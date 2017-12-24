using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSystem.Plugin
{
    /// <summary>
    /// 插件管理抽象类
    /// </summary>
    public abstract class Plugin_Base
    {
        /// <summary>
        /// 初始化加载
        /// </summary>
        public virtual void Load(string Name)
        {
            this.Name = Name;
        }

        public string Name { get; private set; }

    }

    



    public class PluginManager
    {

        public Dictionary<Type, Plugin_Base> _Plugins = new Dictionary<Type, Plugin_Base>()
        {
            //插件调用注册
            {typeof(XXT.XXT_Base),new XXT.XXT_BY() }
        };


        public PluginManager(string Name)
        {
            this.Name = Name;
        }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 初始化加载
        /// </summary>
        public void Load()
        {
            _Plugins.Values.ToList().ForEach(p =>
            {
                p.Load(Name);
            });
        }

        /// <summary>
        /// 获取插件实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetPlugin<T>() where T : Plugin_Base
        {
            return _Plugins.ContainsKey(typeof(T)) ? (T)_Plugins[typeof(T)] : default(T);
        }


        #region 数据

        private List<XXT.SendMsgInfo> _SendMsgInfos;
        /// <summary>
        /// 发送短信数据
        /// </summary>
        public List<XXT.SendMsgInfo> SendMsgInfos
        {
            get
            {
                if (_SendMsgInfos == null)
                {
                    _SendMsgInfos = new List<XXT.SendMsgInfo>();
                    _SendMsgInfos = DataList.Current[Name].DB.SendMsgInfos.Where(p => p.Name == Name).ToList();
                }
                return _SendMsgInfos;
            }
        }
        /// <summary>
        /// 短信记录添加
        /// </summary>
        /// <param name="sendMsgInfos"></param>
        public void AddSendMsgInfo(params XXT.SendMsgInfo[] sendMsgInfos)
        {
            lock (SendMsgInfos)
            {
                sendMsgInfos.ToList().ForEach(p =>
                {
                    p.Name = Name;
                    DataList.Current[Name].DB.SendMsgInfos.Add(p);
                    SendMsgInfos.Add(p);
                    
                });
            }
            DataList.Current.DB_Save();
        }
        /// <summary>
        /// 检查短信是否重复生成
        /// 重复返回false
        /// 不重复返回true
        /// </summary>
        /// <param name="student"></param>
        /// <param name="sendSMSEvery"></param>
        /// <returns></returns>
        public bool SendMsgInfo_CheckHave(DB.Student student,XXT.SendSMSEvery sendSMSEvery)
        {
            DateTime dateTime = DateTime.Now.Date;
            return DataList.Current[Name].DB.SendMsgInfos.Where(p => p.StudentId == student.Id && p.SendSMSEvery == sendSMSEvery && p.CreateTime > dateTime).Count() == 0;
        }

        #endregion

    }
}
