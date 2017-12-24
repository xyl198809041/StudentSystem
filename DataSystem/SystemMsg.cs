using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSystem
{
    public static class SystemMsg
    {
        /// <summary>
        /// 成功
        /// </summary>
        public const string Success = nameof(Success);
    }




    public static class SystemMsg_DB
    {
        /// <summary>
        /// 用户已存在
        /// </summary>
        public const string HaveUser = nameof(SystemMsg_DB) + ":" + nameof(HaveUser);

        
    }
}
