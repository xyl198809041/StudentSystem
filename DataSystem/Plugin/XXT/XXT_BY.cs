using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace DataSystem.Plugin.XXT
{
    /// <summary>
    /// 北苑校讯通服务
    /// </summary>
    public class XXT_BY : XXT_Base
    {
        private static string UserName = "18658181306";
        private static string Password = "19880904";

        /// <summary>
        /// 服务器网址
        /// </summary>
        private static string BaseUrl { get; } = "http://wxt.10010zj.com.cn/";
        /// <summary>
        /// 登录网址
        /// </summary>
        private static string LoginUrl { get { return BaseUrl + "ajaxDologin.do"; } }
        /// <summary>
        /// 发短信网址
        /// </summary>
        private static string SendMsgUrl { get { return BaseUrl + "interaction/saveSMSApp.do"; } }

        private static HttpClient httpClient = new HttpClient();


        private static bool IsLogin = false;

        private bool Login()
        {
            lock (httpClient)
            {
                var rt = httpClient.Post<JsonObj.XXT_BY.Login>(LoginUrl, new Dictionary<string, string>()
                {
                    { "loginPhone", UserName},
                    { "password", Password }
                });
                var b = rt.Code == "200";
                IsLogin = b;
                return b;
            }
        }

        protected override bool SendMsg(string PhoneNum,string SMSText)
        {
            lock (httpClient)
            {
                if (!IsLogin) Login();
                string UpdataStr = @"{""names"":"""",""content"":""" + SMSText + @""",""category"":9,""smsType"":0,""groupSms"":0,""users"":[{""id"":0,""name"":""" + PhoneNum + @""",""active"":true}]}";
                var rt = httpClient.Post<JsonObj.XXT_BY.SendMsg>(SendMsgUrl, new Dictionary<string, string>()
                {
                    {"data",UpdataStr }
                });
                bool s = rt?.Code == "200";
                if (!s) IsLogin = false;
                return s;
            }
        }

    }
}
