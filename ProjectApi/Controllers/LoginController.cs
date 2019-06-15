using ProjectApi.Process;
using ProjectApi.Models;
using System.Web.Http;

namespace ProjectApi.Controllers
{
    public class LoginController : ApiController
    {
        // POST: api/Login
        public object Post([FromBody]LoginRequest loginReq)
        {
            // return "POST: " + loginReq.code;
            object returnObj = null;
            if (loginReq == null)
            {
                returnObj = new
                {
                    errCode = StaticValue.ApiErrCode.PARAM,
                    errMsg = "参数错误"
                };
                return returnObj;
            }
            if (loginReq.v_code != "YbhWURkGpWyjXNrNh3OXtkytN5swLpTW") //防伪
            {
                returnObj = new
                {
                    errCode = StaticValue.ApiErrCode.VALIDATE,
                    errMsg = "EVIL"
                };
                return returnObj;
            }
            if (string.IsNullOrEmpty(loginReq.code) || string.IsNullOrEmpty(loginReq.appid))
            {
                returnObj = new
                {
                    errCode = StaticValue.ApiErrCode.PARAM,
                    errMsg = "参数错误"
                };
                return returnObj;
            }

            MiniProgramLoginProc loginApp = new MiniProgramLoginProc();
            object loginRes = loginApp.MiniProgramLogin(loginReq.code, loginReq.appid);
            if (loginRes == null)
            {
                returnObj = new
                {
                    errCode = StaticValue.ApiErrCode.SYSTEM,
                    errMsg = "系统错误"
                };
                return returnObj;
            }

            returnObj = new
            {
                errCode = StaticValue.ApiErrCode.SUCCESS,
                errMsg = "成功",
                data = loginRes
            };
            return returnObj;
        }

    }
}
