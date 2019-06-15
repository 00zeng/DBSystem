using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.FinancialAccounting.Application;
using ProjectShare.Database;
using System.Collections.Generic;
using System.Collections;
using ProjectShare.Models;

namespace ProjectWeb.Areas.FinancialAccounting.Controllers
{
    public class PositionSalaryController : ControllerBase
    {
        private PositionSalaryApp app = new PositionSalaryApp();

        #region 页面显示
        #region 部门薪资
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult DeptSetting()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult DeptShow()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult DeptApprove()
        {
            return View();
        }
        #endregion
        #region 职等薪资
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult GradeSetting()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult GradeShow()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult GradeApprove()
        {
            return View();
        }
        #endregion
        #region 导购薪资
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult GuideSetting()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult GuideShow()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult GuideApprove()
        {
            return View();
        }
        #endregion
        #region 业务薪资
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult SalesSetting()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult SalesShow()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult SalesApprove()
        {
            return View();
        }
        #endregion
        #region 其他设置
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult OtherSetting()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult OtherShow()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult OtherApprove()
        {
            return View();
        }
        #endregion
        [HttpGet]
        [HandlerAuthorize]       
        public ActionResult TrainerSetting()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult SalaryApprove()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult SalarySetting()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult SalaryApproveShow()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult SalaryShow()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult TrainerShow()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult TrainerApproveShow()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult TrainerKPISetting()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult TrainerApprove()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult TrainerManageShow()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult TrainerManageKPISetting()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult TrainerManageApprove()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult TrainerManageApproveShow()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult PosIndex()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult TrainerKPI()
        {
            return View();
        }

        #endregion

        //   #region 查看 

        // 当前薪资
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListCurrent(Pagination pagination, daoben_salary_position positionInfo,QueryTime queryTime)
        {
            if (positionInfo == null)
                positionInfo = new daoben_salary_position();
            positionInfo.effect_status = 1;
            var data = new
            {
                rows = app.GetList(pagination, positionInfo, queryTime),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListHistory(Pagination pagination, daoben_salary_position positionInfo,QueryTime queryTime)
        {
            if (positionInfo == null)
                positionInfo = new daoben_salary_position();
            positionInfo.effect_status = 0;
            var data = new
            {
                rows = app.GetList(pagination, positionInfo, queryTime),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        // 我的审批
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListApprove(Pagination pagination, daoben_salary_position positionInfo)
        {
            var data = new
            {
                rows = app.GetListApprove(pagination, positionInfo),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        /// <summary>
        /// 岗位薪资调整、培训KPI调整
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="company_id">筛选条件</param>
        /// <param name="type">0：岗位薪资调整；1：培训KPI调整</param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListPosition(Pagination pagination, daoben_salary_position positionInfo)
        {
            int type = 0;
            var data = new
            {
                rows = app.GetListPosition(pagination, positionInfo, type),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListTrainerKpi(Pagination pagination, daoben_salary_position positionInfo)
        {
            int type = 1;
            var data = new
            {
                rows = app.GetListPosition(pagination, positionInfo, type),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListDept(Pagination pagination, int company_id = 0)
        {
            var data = new
            {
                rows = app.GetListDept(pagination, company_id),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        #region 导购薪资
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetCurrentGuide(int company_id)
        {
            var data = app.GetCurrentGuide(company_id);
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGuide(string id)
        {
            var data = app.GetGuide(id);
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SetGuide(daoben_salary_position setting_info, daoben_salary_position_guide info_main,
                    List<daoben_salary_position_guide_sub> info_list,daoben_salary_position_traffic trafficInfo, List<IntIdNamePair> companyList)
        {
            string result = app.SetGuide(setting_info, info_main, info_list, trafficInfo, companyList);
            if (result == "success")
                return Success("设置成功，请等待财务经理审核。", setting_info.id);
            else
                return Error(result);
        }
        #endregion

        #region 业务薪资
        //[HttpGet]
        //[HandlerAjaxOnly]
        //public ActionResult GetCurrentSales(int company_id)
        //{
        //    var data = app.GetCurrentSales(company_id);
        //    return Content(data.ToJson());
        //}
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetSales(string id)
        {
            var data = app.GetSales(id);
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SetSales(daoben_salary_position setting_info,daoben_salary_position_sales salesInfo,
            List<daoben_salary_position_sales_sub> subList,daoben_salary_position_traffic trafficInfo, List<IntIdNamePair> companyList)
        {
            string result = app.SetSales(setting_info, salesInfo,subList, trafficInfo, companyList);
            if (result == "success")
                return Success("设置成功，请等待财务经理审核。", setting_info.id);
            else
                return Error(result);
        }
        #endregion

        #region 培训师薪资
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetCurrentTrainer(int company_id)
        {
            var data = app.GetCurrentTrainer(company_id);
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTrainer(string id)
        {
            var data = app.GetTrainer(id);
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SetTrainer(daoben_salary_position setting_info, daoben_salary_position_trainer info_main,
            daoben_salary_position_traffic trafficInfo, List<IntIdNamePair> companyList)
        {
            string result = app.SetTrainer(setting_info, info_main,trafficInfo,companyList);
            if (result == "success")
                return Success("设置成功，请等待总经理审核。", setting_info.id);
            else
                return Error(result);
        }
        #endregion

        #region 培训经理薪资
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetCurrentTrainerManager(int company_id)
        {
            var data = app.GetCurrentTrainerManager(company_id);
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTrainerManager(string id)
        {
            var data = app.GetTrainerManager(id);
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SetTrainerManager(daoben_salary_position setting_info, daoben_salary_position_trainermanager info_main,
            daoben_salary_position_traffic trafficInfo, List<IntIdNamePair> companyList)
        {
            string result = app.SetTrainerManager(setting_info, info_main,trafficInfo,companyList);
            if (result == "success")
                return Success("设置成功，请等待总经理审核。", setting_info.id);
            else
                return Error(result);
        }
        #endregion

        #region 部门薪资
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetCurrentDept(int dept_id,int company_id)
        {
            var data = app.GetCurrentDept(dept_id, company_id);
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetDept(string id)
        {
            var data = app.GetDept(id);
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SetDept(daoben_salary_position setting_info, List<daoben_salary_position_dept> info_list, daoben_salary_position_traffic trafficInfo)
        {
            string result = app.SetDept(setting_info, info_list, trafficInfo);
            if (result == "success")
                return Success("设置成功，请等待总经理审核。", setting_info.id);
            else
                return Error(result);
        }
        #endregion

        #region 工龄工资
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetCurrentSeniority()
        {
            var data = app.GetCurrentSeniority();
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetSeniority(string id)
        {
            var data = app.GetSeniority(id);
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SetSeniority(List<daoben_salary_position_seniority> info_list, daoben_salary_position setting_info,daoben_salary_position_other otherInfo, List<IntIdNamePair> companyList)
        {
            string result = app.SetSeniority(info_list, otherInfo, setting_info,companyList);
            if (result == "success")
                return Success("设置成功，请等待财务经理审核。", setting_info.id);
            else
                return Error(result);
        }
        #endregion

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetImport(string id)
        {
            var data = app.GetImport(id);
            return Content(data.ToJson());
        }
       
        /// <param name="importListStr">导入的数据</param>
        /// <param name="import_info">主表信息</param>
        /// <param name="companyList">选择分公司的列表：非公版选择分公司的列表</param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Import(string importListStr, daoben_salary_position importInfo, List<IntIdNamePair> companyList)
        {
            List<daoben_salary_position_grade> importList = importListStr.ToList<daoben_salary_position_grade>();
            string result = app.Import(importList, importInfo, companyList);
            if (result == "success")
                return Success("导入成功，请等待财务经理审核。", importInfo.file_name);
            else
                return Error(result);
        }
        /// <summary>
        /// 大量数据
        /// </summary>
        //[HttpPost]
        //[HandlerAjaxOnly]
        //[ValidateAntiForgeryToken]
        //public ActionResult ImportString(string import_list, daoben_salary_position import_info)
        //{
        //    List<daoben_salary_position_grade> importList = import_list.ToList<daoben_salary_position_grade>();
        //    string result = app.Import(importList, import_info);
        //    if (result == "success")
        //        return Success("导入成功，请等待财务经理审核。", import_info.file_name);
        //    else
        //        return Error(result);
        //}
        #region 审批
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(daoben_salary_position_approve approve_info)
        {
            string result = app.Approve(approve_info);
            if (result == "success")
                return Success("操作成功。", approve_info.salary_position_id);
            else
                return Error(result);
        }
        #endregion

        /// <summary>
        /// 撤销申请
        /// </summary>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return Error("ID不能为空");
            string result = app.Delete(id);
            if (result == "success")
                return Success("操作成功。", id);
            else
                return Error(result);
        }

    }
}