namespace ProjectApi.Models
{
    public class WxApiLoginSession
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }
        public object hints { get; set; }
        public string openid { get; set; }
        public string session_key { get; set; }
        public string unionid { get; set; }
    }
}