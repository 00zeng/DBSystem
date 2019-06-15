using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Code
{
    public class web_login
    {
        public string LoginToken { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserID { get; set; }
        /// <summary>
        /// Phone
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// UserName
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// LoginTime
        /// </summary>
        public DateTime? LoginTime { get; set; }
        /// <summary>
        /// ExpireTime
        /// </summary>
        public DateTime? ExpireTime { get; set; }

        /// <summary>
        /// APP/门户环信账号
        /// </summary>
        public string PADUser { get; set; }
        /// <summary>
        /// 用户头像
        /// </summary>
        public string Photo { get; set; }

    }
}
