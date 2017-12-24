using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSystem.DB
{
    //用于数据传输类构建


    /// <summary>
    /// 总分信息,一般不更新
    /// </summary>
    public class StudentSumPoint : NotifyPropertyChangedBase
    {
        /// <summary>
        /// 学生
        /// </summary>
        public Student Student { get; set; }

        /// <summary>
        /// 总分
        /// </summary>
        public int SumPoint { get; set; }

        /// <summary>
        /// 上周总分
        /// </summary>
        public int LastWeekPoint { get; set; }

        /// <summary>
        /// 本周总分
        /// </summary>
        public int ThisWeekPoint { get; set; }


    }
}
