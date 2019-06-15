using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.DistributorManage.Application;
using ProjectShare.Database;

namespace ProjectWeb.Areas.DistributorManage.Controllers
{
    public class ClerkManageController : ControllerBase
    {
        private ClerkManageApp app = new ClerkManageApp();

        /// <summary>
        /// 在职店员列表
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryInfo">查询信息</param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, daoben_distributor_clerk queryInfo)
        {

            var data = new
            {
                rows = app.GetList(pagination, queryInfo),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }
        /// <summary>
        /// 离职店员列表
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryInfo">查询信息</param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJsonResign(Pagination pagination, daoben_distributor_clerk_resign queryInfo)
        {

            var data = new
            {
                rows = app.GetGridJsonResign(pagination, queryInfo),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetInfo(string id)
        {
            var data = app.GetInfo(id);
            return Content(data);
        }
        /// <summary>
        /// 离职店员信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetResignInfo(string id)
        {
            var data = app.GetResignInfo(id);
            return Content(data);
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Add(daoben_distributor_clerk addInfo, daoben_ms_account addAccountInfo)
        {
            string result = app.Add(addInfo, addAccountInfo);
            if (result == "success")
                return Success("操作成功。", addInfo.id);
            else
                return Error(result);
        }

        /// <summary>
        /// 修改信息：手机微信入职时间
        /// </summary>
        /// <param name="editInfo">editInfo.id不能为空</param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(daoben_distributor_clerk editInfo)
        {
            string result = app.Edit(editInfo);
            if (result == "success")
                return Success("操作成功。", editInfo.id);
            else
                return Error(result);
        }

        #region 启用/注销账户
        /// <summary>
        /// 注销/启用
        /// </summary>        
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Active(string id)
        {
            string result = app.AccountActive(id);
            if (result == "success")
                return Success("操作成功。", id);
            else
                return Error(result);
        }
        #endregion
        /// <summary>
        /// 离职
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Resign(string id)
        {
            string result = app.Resign(id);
            if (result == "success")
                return Success("操作成功。", id);
            else
                return Error(result);
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            string result = app.Delete(id);
            if (result == "success")
                return Success("操作成功。", id);
            else
                return Error(result);
        }
    }
}