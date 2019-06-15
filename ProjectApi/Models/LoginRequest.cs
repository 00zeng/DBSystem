namespace ProjectApi.Models
{
    public class LoginRequest
    {
        /// <summary>
        /// 微信返回的code
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 小程序APP_ID
        /// </summary>
        public string appid { get; set; }
        /// <summary>
        /// 防伪校验码
        /// </summary>
        public string v_code { get; set; }

    }
}