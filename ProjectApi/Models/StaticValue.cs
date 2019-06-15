using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectApi.Models
{
    public class StaticValue
    {
        public const string WX_SESSION_URL = "https://api.weixin.qq.com/sns/jscode2session?appid={0}&secret={1}&js_code={2}&grant_type=authorization_code";
        public const string WX_SECRET = "b9d66f1e3e875d521e349dc86c391a62";

        public const string API_VALIDATE = "YbhWURkGpWyjXNrNh3OXtkytN5swLpTW";

        public enum ApiErrCode
        {
            SUCCESS = 0,        // 正常
            PARAM = 6000,       // 参数错误
            VALIDATE = 6001,    // 防伪校验码错误
            SYSTEM = 6002       // 系统出错
        }
    }
}