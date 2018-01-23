using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Net.Http;

namespace DataSystem
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class ExClass
    {
        private static JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All,
            ReferenceLoopHandling= ReferenceLoopHandling.Ignore
        };
        /// <summary>
        /// 带类型序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj, serializerSettings);
        }
        /// <summary>
        /// 序列化用于Web,纯信息
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJsonForWeb(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static object ToObj(this string json)
        {   
            return JsonConvert.DeserializeObject(json, serializerSettings);
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T ToObj<T>(this string json)
        {
            return (T)ToObj(json);
        }


        /// <summary>
        /// httpclient post
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="Url"></param>
        /// <param name="Form"></param>
        /// <returns></returns>
        public static T Post<T>(this HttpClient httpClient,string Url,Dictionary<string,string> Form)
        {
            var rtstr = httpClient.PostAsync(Url, new FormUrlEncodedContent(Form)).Result.Content.ReadAsStringAsync().Result;
            try
            {
                return JsonConvert.DeserializeObject<T>(rtstr);

            }
            catch
            {
                return default(T);
            }
        }
        /// <summary>
        /// httpclient post
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="httpClient"></param>
        /// <param name="Url"></param>
        /// <param name="Form"></param>
        /// <returns></returns>
        public static T Post<T>(this HttpClient httpClient,string Url,string Form)
        {
            var rtstr = httpClient.PostAsync(Url, new StringContent(Form)).Result.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<T>(rtstr);
        }



        #region 时间


        /// <summary>
        /// 日期转换为习惯用语
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToHumanDateString(this DateTime dateTime)
        {
            if (dateTime.Date == DateTime.Now.Date)
            {
                return "今天";
            }
            else if (dateTime.Date == DateTime.Now.Date.AddDays(-1))
            {
                return "昨天";
            }
            else if (dateTime.Date >= DateTime.Now.ThisWeekMonday())
            {
                return "本" + dateTime.ToString("ddd");
            }
            else if (dateTime.Date >= DateTime.Now.ThisWeekMonday().AddDays(-7))
            {
                return "上" + dateTime.ToString("ddd");
            }
            else return dateTime.ToString("MM月dd日,dddd");
        }
        /// <summary>
        /// 设置时间部分
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="Hour"></param>
        /// <param name="Min"></param>
        /// <returns></returns>
        public static DateTime AtTime(this DateTime dateTime, double Hour, double Min)
        {
            return dateTime.Date.AddHours(Hour).AddMinutes(Min);
        }
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
        /// 当前星期几是否是本月的最后一个星期几
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static bool IsLastWeekOfMonth(this DateTime dateTime)
        {
            return dateTime.Month != dateTime.AddDays(7).Month;
        }
        /// <summary>
        /// 当前星期几是否是本月的最后一个星期几
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static bool IsLastWeekOfMonth(this DateTime dateTime,DayOfWeek dayOfWeek)
        {
            if (dateTime.DayOfWeek != dayOfWeek) return false;
            return IsLastWeekOfMonth(dateTime);
        }

        #endregion

    }

    /// <summary>
    /// 绑定数据基类
    /// </summary>
    public class NotifyPropertyChangedBase:INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
    }


}
