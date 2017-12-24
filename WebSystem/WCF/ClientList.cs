using DataSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSystem.WCF
{
    /// <summary>
    /// 连接客户端列表
    /// </summary>
    public class ClientList : Dictionary<string, ClientData>
    {
        /// <summary>
        /// 隐藏构造函数
        /// </summary>
        private ClientList()
        {
            DataList.Current.DataChanged += AddNewMsg;
        }
        private static ClientList _Current;
        /// <summary>
        /// 当前实例
        /// </summary>
        public static ClientList Current
        {
            get
            {
                if (_Current == null)
                {
                    _Current = new ClientList();
                }
                return _Current;
            }
        }

        /// <summary>
        /// 登录更新,保持登录状态
        /// </summary>
        /// <param name="Guid"></param>
        /// <param name="Name"></param>
        public void Link(string Guid, string Name)
        {
            lock (this)
            {
                if (!ContainsKey(Guid))
                {
                    Add(Guid, new ClientData()
                    {
                        Guid = Guid,
                        Name=Name
                    });
                }
            }
            this[Guid].LinkTime = DateTime.Now;
        }

        private void DelOffLink()
        {
            lock (this)
            {
                this.Where(p => p.Value.LinkTime < DateTime.Now.AddMinutes(-1)).ToList().ForEach(p =>
                {
                    Remove(p.Key);
                });
            }
        }
        /// <summary>
        /// 获取新消息
        /// </summary>
        /// <param name="Guid"></param>
        /// <returns></returns>
        public List<WCFNewMsg> GetNewMsg(string Guid)
        {
            lock (this)
            {
                if (this[Guid].Count != 0)
                {
                    var list= this[Guid].Values.ToList();
                    this[Guid].Clear();
                    return list;
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 添加新消息
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="MsgType"></param>
        public void AddNewMsg(string Name,string PropertyName)
        {
            lock (this)
            {
                this.Where(p => p.Value.Name == Name).ToList().ForEach(p =>
                {
                    if(!p.Value.ContainsKey(PropertyName))
                    {
                        p.Value.Add(PropertyName, new WCFNewMsg()
                        {
                            PropertyName = PropertyName
                        });
                    }
                });
            }
        }


    }

    /// <summary>
    /// 连接客户端信息
    /// Data中注册的属性名称
    /// </summary>
    public class ClientData:Dictionary<string,WCFNewMsg>
    {
        /// <summary>
        /// 数据名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// id
        /// </summary>
        public string Guid { get; set; }
        /// <summary>
        /// 最新连接时间
        /// </summary>
        public DateTime LinkTime { get; set; }

    }
}