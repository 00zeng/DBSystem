using System.Web.Mvc;
using Base.Code;
using ProjectWeb.Areas.HumanResource.Application;
using ProjectShare.Database;
using System;
using System.Collections.Generic;
using ProjectShare.Models;
using System.IO;

namespace ProjectWeb.Areas.HumanResource.Controllers
{
    public class EmployeeManageController : ControllerBase
    {
        private EmployeeManageApp app = new EmployeeManageApp();

        #region 页面显示
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult EmployeeAdd()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult Import()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult EmployeeShow()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult EmployeeEdit()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult EditNew()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult ResignIndex()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult NewEmpIndex()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult EditPersonalInfo()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult EditJobInfo()
        {
            return View();
        }
        public ActionResult EditAccountInfo()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult EditPictureInfo()
        {
            return View();
        }
        #endregion

        //导出excel  
        public FileResult Export(Pagination pagination, daoben_hr_emp_job jobInfo, QueryTime queryTime, string name = null)
        {
            string fileName = "在职员工_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            Stream fileStream = app.ExportExcel(pagination, jobInfo, name, queryTime);
            return File(fileStream, "application/vnd.ms-excel", fileName);
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, daoben_hr_emp_job jobInfo, QueryTime queryTime, string name = null)
        {
            var data = new
            {
                rows = app.GetList(pagination, jobInfo, name, queryTime),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListResign(Pagination pagination, daoben_hr_emp_resigned_job jobInfo, QueryTime queryTime,
            string name = null)
        {
            var data = new
            {
                rows = app.GetListResign(pagination, jobInfo, name, queryTime),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetListNew(Pagination pagination, string name = null)
        {
            var data = new
            {
                rows = app.GetListNew(pagination, name),
                total = pagination.records,
            };
            return Content(data.ToJson());
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Add(daoben_hr_emp_info addInfo, daoben_hr_emp_job addJobInfo,
                    daoben_ms_account addAccountInfo, List<daoben_distributor_guide> distributorList = null, List<daoben_hr_emp_file> image_list = null)
        {
            string result = app.Add(addInfo, addJobInfo, addAccountInfo, distributorList, image_list);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult EditPersonalInfo(daoben_hr_emp_info editInfo)
        {
            string result = app.EditPersonalInfo(editInfo);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult EditJobInfo(daoben_hr_emp_job editInfo)
        {
            string result = app.EditJobInfo(editInfo);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }

        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult EmpResign(string id, DateTime resignTime)
        {
            string result = app.EmpResign(id, resignTime);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(daoben_hr_emp_info addInfo, daoben_hr_emp_job addJobInfo,
                    List<daoben_hr_emp_file> add_image_list = null, List<int> del_image_list = null)
        {
            string result = app.Edit(addInfo, addJobInfo, add_image_list, del_image_list);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult EditInfo(daoben_hr_emp_info addInfo, List<daoben_hr_emp_file> add_image_list = null, List<int> del_image_list = null)
        {
            string result = app.Edit(addInfo, null, add_image_list, del_image_list);
            if (result == "success")
                return Success("操作成功。");
            else
                return Error(result);
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetInfo(string id)
        {
            string data = app.GetInfo(id);
            return Content(data);
        }
        //[HttpGet]
        //[HandlerAjaxOnly]
        //public ActionResult GetNewEmpInfo(string id)
        //{
        //    string data = app.GetNewEmpInfo(id);
        //    return Content(data);
        //}

        #region 停用/启用账户
        /// <summary>
        /// 停用/启用
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
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetEmpByName(string name)
        {
            string data = app.GetEmpByName(name);
            return Content(data);
        }

        /// <summary>
        /// 获取分公司下区域-员工的树形列表
        /// </summary>
        /// <param name="company_id">0表示获取登录人所在的分公司，>0表示获取指定的分公司</param>
        /// <param name="emp_type">0-所有员工；1-行政人员；12-培训师；13-培训经理；20-业务员；21-业务经理；3-导购员;</param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetEmpTree(int emp_type = 3, int company_id = 0, string guide_category = null)
        {
            var data = app.GetEmpTree(company_id, emp_type, guide_category);
            return Content(data);
        }

        /// <summary>
        /// 根据机构获取区域选择列表以及该区域员工数量
        /// </summary>
        /// <param name="company_id">0-表示登录人所在机构</param>
        /// <param name="emp_type">0-所有员工；20-业务员；21-业务经理；3-导购员;</param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetAreaEmpCountList(int company_id = 0, int emp_type = 3)
        {
            var data = app.GetAreaEmpCountList(company_id, emp_type);
            return Content(data);
        }

        /// <summary>
        /// 根据机构获取员工数量
        /// </summary>
        /// <param name="company_id">0-表示登录人所在机构</param>
        /// <param name="emp_type">0-所有员工数量；20-业务员数量；21-业务经理数量；3-导购员数量</param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetEmpCount(int company_id = 0, int emp_type = 3)
        {
            var data = app.GetEmpCount(company_id, emp_type);
            return Content(data.ToJson());
        }

        /// <summary>
        /// 获取区域内员工的选择列表
        /// </summary>
        /// <param name="company_id">仅当area_id==0时有效：0表示获取登录人所在的机构，>0表示获取指定的机构</param>
        /// <param name="area_id">0-指所有区域，</param>
        /// <param name="emp_type">0-所有员工数量；20-业务员数量；21-业务经理数量；3-导购员数量</param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetAreaEmpList(int area_id, int emp_type = 3, int company_id = 0)
        {
            var data = app.GetAreaEmpList(company_id, area_id, emp_type);
            return Content(data);
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetAreaOrDeptEmpList()
        {
            var data = app.GetAreaOrDeptEmpList();
            return Content(data);
        }

        /// <summary>
        /// 获取当前登录人的机构下全体部门或区域列表，职位列表，员工列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetAreaDeptPosEmpList()
        {
            string data = app.GetAreaDeptPosEmpList();
            return Content(data);
        }
        /// <summary>
        /// 雇员类别信息修改，仅仅支持实习转员工
        /// </summary>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult empCategoryChange(string id)
        {
            if (string.IsNullOrEmpty(id))
                return Error("ID不能为空");

            string result = app.EmpCategoryChange(id);
            if (result == "success")
                return Success("操作成功。", id);
            else
                return Error(result);
        }

        /// <summary>
        /// 获取当前登录人的机构下全体员工列表（不含下属机构）
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetMyCompanyEmpList(bool exclude_emp_status = false, DateTime? exclude_entry_date = null)
        {
            string data = app.GetMyCompanyEmpList(exclude_emp_status, exclude_entry_date);
            return Content(data);
        }

        /// <summary>
        /// 根据机构id获取机构下全体员工列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetEmpListByCompanyId(bool exclude_emp_status = false, DateTime? exclude_entry_date = null, int company_id = 0)
        {
            string data = app.GetEmpListByCompanyId(exclude_emp_status, exclude_entry_date, company_id);
            return Content(data);
        }
        /// <summary>
        /// 员工导入
        /// </summary>
        /// <param name="empInfoListStr">员工个人信息数据</param>
        /// <param name="empJobListStr">员工职位信息数据</param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult Import(string empInfoListStr, string empJobListStr)
        {
            List<daoben_hr_emp_info> empInfoList = empInfoListStr.ToList<daoben_hr_emp_info>();
            List<daoben_hr_emp_job> empJobList = empJobListStr.ToList<daoben_hr_emp_job>();

            string result = app.Import(empInfoList, empJobList);
            if (result == "success")
                return Success("导入成功。");
            else
                return Error(result);
        }
        /// <summary>
        /// 根据当前登录人的事业部ID获取全部的职位
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetPosList(int companyId = 0)
        {
            var data = app.GetPosList(companyId);
            return Content(data);
        }

        /// <summary>
        /// 根据当前登录人的事业部ID获取全部的区域
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetAreaList(int companyId = 0)
        {
            var data = app.GetAreaList(companyId);
            return Content(data);
        }

        /// <summary>
        /// 导购员列表-实销导入信息匹配
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGuideList()
        {
            string data = app.GetGuideList();
            return Content(data);
        }

        /// <summary>
        /// 业务员列表-实销导入信息匹配
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetSalesList()
        {
            string data = app.GetSalesList();
            return Content(data);
        }


        /// <summary>
        /// 业务员列表-实销导入信息匹配
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetAllEmpId()
        {
            string data = app.GetAllEmpId();
            return Content(data);
        }
    }
}