using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.SystemManage.Application;
using ProjectShare.Database;

namespace ProjectWeb.Areas.SystemManage.Controllers
{
    public class MsAccountController : ControllerBase
    {
        private MsAccountApp app = new MsAccountApp();
        #region 页面显示
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult UpdateRole()
        {
            return View();
        }

        #endregion


        #region 账户列表显示
        /// <summary>
        /// 分页显示账户列表
        /// </summary>       
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, daoben_ms_account account, string account_name = null)
        {
            var data = new
            {
                rows = app.GetList(pagination, account, account_name),
                total = pagination.records
            };
            return Content(data.ToJson());
        }

        #endregion

        #region 新增账户
        /// <summary>
        /// 新增账户
        /// </summary>       
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Add(daoben_ms_account accountInfo)
        {
            string result = app.AddAccount(accountInfo);
            if (result == "success")
                return Success("操作成功。", accountInfo.id);
            else
                return Error(result);
        }
        #endregion

        #region 重置密码
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(int id, string new_password)
        {
            string result = app.ResetPassword(id, new_password);
            if (result == "success")
                return Success("操作成功。", id);
            else
                return Error(result);
        }
        #endregion
        #region 启用/注销账户
        /// <summary>
        /// 注销/启用
        /// </summary>        
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Active(int id)
        {           
            string result = app.ActiveAccount(id);
            if (result == "success")
                return Success("操作成功。", id);
            else
                return Error(result);
        }
        #endregion
        #region 更换角色
        /// <summary>
        /// 新增账户
        /// </summary>       
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateRole(int id, int role_id)
        {
            string result = app.UpdateRole(id, role_id);
            if (result == "success")
                return Success("操作成功。", id);
            else
                return Error(result);
        }
        #endregion

        #region 删除账户
        /// <summary>
        /// 删除角色
        /// </summary>       
        /// <returns></returns>
        /// 
        public ActionResult Delete(int id)
        {
            string result = app.DeleteAccount(id);
            if (result == "success")
                return Success("删除成功。", id);
            else
                return Error(result);
        }
        #endregion

    }
}