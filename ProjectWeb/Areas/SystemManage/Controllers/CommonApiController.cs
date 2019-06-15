using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.SystemManage.Application;
using ProjectShare.Database;

namespace ProjectWeb.Areas.SystemManage.Controllers
{
    public class CommonApiController : ControllerBase
    {
        private CommonApiApp app = new CommonApiApp();

        #region 导入匹配列表
        /// <summary>
        /// 导入匹配列表
        /// </summary>
        /// <param name="guide">是否获取【导购员】列表</param>
        /// <param name="sales">是否获取【业务员】列表</param>
        /// <param name="distributor">是否获取【经销商】列表</param>
        /// <param name="company">是否获取【分公司】列表</param>
        /// <param name="area">是否获取【业务片区】列表</param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListForMatching(bool guide = true, bool sales = true, bool distributor = true,
                    bool company = true, bool area = true)
        {
            object retObj = app.GetListForMatching(guide, sales, distributor, company, area);
            return Content(retObj.ToJson());
        }

        #endregion
    }
}