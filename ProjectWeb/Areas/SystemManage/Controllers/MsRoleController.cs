using Base.Code;
using System.Web.Mvc;
using System.Linq;
using ProjectShare.Database;
using ProjectWeb.Areas.SystemManage.Application;
using System.Collections.Generic;
using ProjectShare.Models;
using ProjectShare.Process;

namespace ProjectWeb.Areas.SystemManage.Controllers
{
    public class MsRoleController : ControllerBase
    {
        private MsRoleApp app = new MsRoleApp();

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult AuthorityEdit()
        {
            return View();
        }


        #region 查询角色、获取角色
        /// <summary>
        /// 分页获取角色列表信息
        /// </summary>       
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, daoben_ms_role role)
        {
            var data = new
            {
                rows = app.GetList(pagination, role),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }
        #endregion

        #region 新增角色
        /// <summary>
        /// 新增角色
        /// </summary>       
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Add(daoben_ms_role role)
        {
            string result = app.AddRole(role);
            if (result == "success")
                return Success("操作成功。", role.id);
            else
                return Error(result);
        }
        #endregion     

        /// <summary>
        /// 查看角色对应账户
        /// </summary>       
        /// <returns></returns>
        //public ActionResult GetAccountJson(Pagination pagination, int roleID)
        //{
        //    var data = new
        //    {
        //        rows = app.GetAccountList(pagination,roleID),
        //        total = pagination.records,
        //    };
        //    return Content(data.ToJson());
        //}
        /// <summary>
        /// 修改角色
        /// </summary>       
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(daoben_ms_role role)
        {
            string result = app.EditRole(role);
            if (result == "success")
                return Success("修改成功。", role.id);
            else
                return Error(result);
        }

        #region 注销/启用
        /// <summary>
        /// 注销角色及其该角色下的所有账户
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Active(int id)
        {
            string result = app.ActiveRole(id);
            if (result == "success")
                return Success("操作成功。", id);
            else
                return Error(result);
        }
        #endregion

        /// <summary>
        /// 删除角色
        /// </summary>       
        /// <returns></returns>
        /// 
        public ActionResult Delete(int id)
        {
            string result = app.DeleteRole(id);
            if (result == "success")
                return Success("删除成功。", id);
            else
                return Error(result);
        }

        /// <summary>
        /// 获取角色选择列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetSelectJson()
        {
            var data = app.GetIdNameList();
            return Content(data.ToJson());
        }


        #region 权限设置
        SysAuthorityApp authApp = new SysAuthorityApp();
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetAuthorityTree(int role_id)
        {
            string authTreeStr = authApp.GetAuthority(role_id);
            return Content(authTreeStr);
        }

        /// <summary>
        /// 更新角色权限
        /// </summary>
        /// <param name="roleId">角色id</param>
        /// <param name="authList">权限</param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult EditAuthority(int role_id, string auth_list_str)
        {
            List<daoben_sys_authority> list = auth_list_str.ToList<daoben_sys_authority>();
            string result = authApp.EditAuthority(role_id, list);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }

        #endregion

    }
}