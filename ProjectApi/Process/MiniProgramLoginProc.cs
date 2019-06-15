using Base.Code;
using System.Linq;
using MySqlSugar;
using ProjectShare.Database;
using System;
using System.Collections;
using System.Web;
using ProjectApi.Models;
using ProjectShare.Process;

namespace ProjectApi.Process
{
    public class MiniProgramLoginProc
    {

        public object MiniProgramLogin(string code, string appid)
        {
            Hashtable ht = new Hashtable();
            ht.Add("appid", appid);
            ht.Add("secret", StaticValue.WX_SECRET);
            ht.Add("js_code", code);
            ht.Add("grant_type", "authorization_code");
            try
            {
                string url = string.Format(StaticValue.WX_SESSION_URL, appid, StaticValue.WX_SECRET, code);
                string wxRes = HttpMethods.HttpGet(url);
                if (string.IsNullOrEmpty(wxRes))
                    return null;
                WxApiLoginSession wxSession = wxRes.ToObject<WxApiLoginSession>();
                if (wxSession == null || string.IsNullOrEmpty(wxSession.openid))
                    return null;
                jwt_customer_account cusAccount = null;
                using (var db = SugarDao.GetInstance())
                {
                    #region 检查/新添加用户
                    if (!string.IsNullOrEmpty(wxSession.unionid))
                    {
                        // unionid 为本司全平台ID，可跨平台使用
                        // openid 为特定APP的ID，不可跨平台使用
                        cusAccount = db.Queryable<jwt_customer_account>()
                                .Where(a => a.unionid == wxSession.unionid).FirstOrDefault();
                    }
                    if (cusAccount == null)
                    {
                        cusAccount = db.Queryable<jwt_customer_account>()
                            .Where(a => a.openid == wxSession.openid).FirstOrDefault();
                        if (cusAccount != null)
                        {
                            // 兼容旧版本或未认证等没返回过unionid的情况
                            db.Update<jwt_customer_account>(new { unionid = wxSession.unionid },
                                        a => a.id == cusAccount.id);
                        }
                        else
                        {
                            cusAccount = new jwt_customer_account();
                            cusAccount.id = Common.GuId();
                            cusAccount.openid = wxSession.openid;
                            cusAccount.unionid = wxSession.unionid;
                            cusAccount.reg_appid = appid;
                            cusAccount.reg_time = DateTime.Now;
                            db.Insert(cusAccount);
                        }
                    }
                    #endregion
                    jwt_customer_session cusSession = db.Queryable<jwt_customer_session>()
                            .Where(a => a.wx_session_key == wxSession.session_key).FirstOrDefault();
                    if (cusSession != null && cusSession.expire_time > DateTime.Now)
                    {
                        db.Delete<jwt_customer_session>(a => a.jwt_session_id == cusSession.jwt_session_id);
                        HttpRuntime.Cache.Remove(cusSession.jwt_session_id);
                        cusSession = null;
                    }
                    if (cusSession == null)
                    {
                        cusSession = new jwt_customer_session();
                        cusSession.jwt_session_id = Common.GuId();
                        cusSession.customer_id = cusAccount.id;
                        cusSession.wx_session_key = wxSession.session_key;
                        cusSession.openid = wxSession.openid;
                        cusSession.app_id = appid;
                        cusSession.create_time = DateTime.Now;
                        cusSession.expire_time = DateTime.Now.AddMinutes(30);
                        db.Insert(cusSession);
                        HttpRuntime.Cache.Insert(cusSession.jwt_session_id,
                                    cusSession, null, DateTime.MaxValue, TimeSpan.FromMinutes(30));
                    }
                    object loginRes = new
                    {
                        jwt_session = cusSession.jwt_session_id,
                        fill_info = (string.IsNullOrEmpty(cusAccount.phone) ? true : false)
                    };
                    return loginRes;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public object Test(string key)
        {
            jwt_customer_session cusSession = (jwt_customer_session)HttpRuntime.Cache.Get(key);
            if (cusSession != null)
                return cusSession;

            cusSession = new jwt_customer_session();
            cusSession.jwt_session_id = Common.GuId();
            cusSession.create_time = DateTime.Now;
            cusSession.expire_time = DateTime.Now.AddMinutes(30);
            HttpRuntime.Cache.Insert(cusSession.jwt_session_id,
                        cusSession, null, DateTime.MaxValue, TimeSpan.FromSeconds(30));
            return cusSession;
        }
    }
}
