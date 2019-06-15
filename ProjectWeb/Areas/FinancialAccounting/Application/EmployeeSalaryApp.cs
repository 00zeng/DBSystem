using Base.Code;
using MySqlSugar;
using ProjectShare.Database;
using ProjectShare.Models;
using ProjectShare.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectWeb.Areas.SystemManage.Application;
using System.Collections;
using ProjectWeb.Areas.SalaryCalculate.Application;

namespace ProjectWeb.Areas.FinancialAccounting.Application
{
    public class EmployeeSalaryApp
    {
        MsAccountApp msAccount = new MsAccountApp();

        #region 查看
        /// <summary>
        /// 查看全部员工信息
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="leavingInfo"></param>
        /// <returns></returns>
        public object GetGridJson(Pagination pagination, daoben_hr_emp_job queryInfo, int approve_status, QueryTime queryTime)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "create_time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            using (var db = SugarDao.GetInstance())
            {
                //查看全部
                var qable = db.Queryable<daoben_hr_emp_job>();
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (myCompanyInfo.category == "事业部")
                        qable.Where(a => (a.company_id_parent == myCompanyInfo.id || a.company_id == myCompanyInfo.id));
                    else
                        qable.Where(a => a.company_id == myCompanyInfo.id);
                }
            //    todo
                //if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                //{
                //    if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER)
                //    {
                //        qable.Where<daoben_hr_emp_job>((a, b) => b.company_id_parent == myCompanyInfo.id || b.company_id == myCompanyInfo.id);
                //    }
                //    else if (myPositionInfo.positionType < ConstData.POSITION_OFFICE_NORMAL && myPositionInfo.positionType != 0)
                //    {
                //        qable.Where<daoben_hr_emp_job>((a, b) => b.company_id_parent == myCompanyInfo.id || b.company_id == myCompanyInfo.id);
                //        qable.Where(a => a.approve_status == 0 && a.category != 1);
                //    }
                //    else
                //        return null;
                //}
                //姓名 机构 部门 职位名 审批状态 入职时间 * 2
                if (!string.IsNullOrEmpty(queryInfo.name))
                    qable.Where(b => b.name.Contains(queryInfo.name));
                if (queryInfo.company_id > 0)
                    qable.Where(b => b.company_id == queryInfo.company_id);
                if (queryInfo.dept_id > 0)
                    qable.Where(b => b.dept_id == queryInfo.dept_id);
                if (queryInfo.position_id > 0)
                    qable.Where(b => b.position_id == queryInfo.position_id);
                //if (approve_status != 0)  // 0表示查找全部
                //{
                //    int status = approve_status;
                //    if (status == 100)   // 已审批
                //        qable.Where(a => a.approve_status == 100);
                //    else if (status == -100)    // 审批不通过
                //        qable.Where(a => a.approve_status < 0);
                //    else if (status == 1)    // 审批中
                //        qable.Where(a => a.approve_status > 0 && a.approve_status < 100);
                //    else if (status == -1)    // 未审批
                //        qable.Where(a => a.approve_status == 0);
                //}
                if (queryTime != null)
                {
                    if (queryTime.startTime1 != null)
                        qable.Where(a => a.entry_date >= queryTime.startTime1);
                    if (queryTime.startTime2 != null)
                    {
                        queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                        qable.Where(a => a.entry_date < queryTime.startTime2);
                    }
                }


                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                                .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public object GetHistoryList(Pagination pagination, daoben_hr_emp_job queryInfo, int category, QueryTime queryTime)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.create_time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            using (var db = SugarDao.GetInstance())
            {
                //历史记录
                var qable = db.Queryable<daoben_salary_emp>()
                        .JoinTable<daoben_hr_emp_job>((a, b) => b.id == a.emp_id);
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (myCompanyInfo.category == "事业部")
                        qable.Where<daoben_hr_emp_job>((a, b) =>( b.company_id_parent == myCompanyInfo.id || b.company_id == myCompanyInfo.id));
                    else
                        qable.Where<daoben_hr_emp_job>((a, b) => b.company_id == myCompanyInfo.id);
                }
                if (!string.IsNullOrEmpty(queryInfo.name))
                    qable.Where<daoben_hr_emp_job>((a, b) => b.name.Contains(queryInfo.name));
                if (queryInfo.dept_id > 0)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.dept_id == queryInfo.dept_id);
                if (queryInfo.position_id > 0)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_id == queryInfo.position_id);
                if (queryInfo.company_id > 0)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.company_id == queryInfo.company_id);
                if (!string.IsNullOrEmpty(queryInfo.position_name))
                    qable.Where<daoben_hr_emp_job>((a, b) => b.position_name.Contains(queryInfo.position_name));
                if (queryTime.startTime1 != null)
                    qable.Where<daoben_hr_emp_job>((a, b) => a.create_time >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    DateTime tempTime = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where<daoben_hr_emp_job>((a, b) => a.create_time < tempTime);
                }
                if (queryTime.endTime1 != null)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.entry_date >= queryTime.endTime1);
                if (queryTime.endTime2 != null)
                {
                    DateTime tempTime = queryTime.endTime2.ToDate().AddDays(1);
                    qable.Where<daoben_hr_emp_job>((a, b) => b.entry_date < tempTime);
                }
                if (category > 0)
                    qable.Where(a => a.category == category);

                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                                .Select("b.id as emp_id,a.id,b.position_type,b.status as status,b.name,b.work_number,b.emp_category,b.position_name,b.grade,b.area_l1_name,b.area_l2_name,b.dept_name,b.company_name,b.supervisor_name,b.entry_date,a.effect_status,a.creator_name as creator_name,a.category as category,a.id as id,a.create_time as create_time")
                                .ToJsonPage(pagination.page, pagination.rows, ref records);

                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public object GetOfficeList(Pagination pagination, daoben_hr_emp_job queryInfo, QueryTime queryTime)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "entry_date" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            using (var db = SugarDao.GetInstance())
            {
                //查看全部员工 行政
                var qable = db.Queryable<daoben_hr_emp_job>()
                    .Where(a => a.position_type <= ConstData.POSITION_OFFICE_NORMAL);

                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (myCompanyInfo.category == "事业部")
                        qable.Where(a => (a.company_id_parent == myCompanyInfo.id || a.company_id == myCompanyInfo.id));
                    else
                        qable.Where(a => a.company_id == myCompanyInfo.id);
                }

                //姓名 机构 部门 职位名  入职时间 * 2
                if (!string.IsNullOrEmpty(queryInfo.name))
                    qable.Where(a => a.name.Contains(queryInfo.name));
                if(queryInfo!=null)
                {
                    if (queryInfo.company_id > 0)
                        qable.Where(a => a.company_id == queryInfo.company_id);
                    if (queryInfo.dept_id > 0)
                        qable.Where(a => a.dept_id == queryInfo.dept_id);
                    if (queryInfo.position_id > 0)
                        qable.Where(a => a.position_id == queryInfo.position_id);
                    if (!string.IsNullOrEmpty(queryInfo.position_name))
                        qable.Where(a => a.position_name.Contains(queryInfo.position_name));
                }               
                if (queryTime != null)
                {
                    if (queryTime.startTime1 != null)
                        qable.Where(a => a.entry_date >= queryTime.startTime1);
                    if (queryTime.startTime2 != null)
                    {
                        queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                        qable.Where(a => a.entry_date < queryTime.startTime2);
                    }
                }

                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                       .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public object GetTrainerList(Pagination pagination, daoben_hr_emp_job queryInfo, QueryTime queryTime)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "entry_date" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            using (var db = SugarDao.GetInstance())
            {
                //查看培训师，培训经理
                var qable = db.Queryable<daoben_hr_emp_job>()
                    .Where(a => (a.position_type == ConstData.POSITION_TRAINER || a.position_type == ConstData.POSITION_TRAINERMANAGER));

                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (myCompanyInfo.category == "事业部")
                        qable.Where(a => (a.company_id_parent == myCompanyInfo.id || a.company_id == myCompanyInfo.id));
                    else
                        qable.Where(a => a.company_id == myCompanyInfo.id);
                }

                //姓名 机构 部门 职位名  入职时间 * 2
                if (queryInfo != null)
                {
                    if (!string.IsNullOrEmpty(queryInfo.name))
                        qable.Where(a => a.name.Contains(queryInfo.name));
                    if (queryInfo.company_id > 0)
                        qable.Where(a => a.company_id == queryInfo.company_id);
                    if (queryInfo.dept_id > 0)
                        qable.Where(a => a.dept_id == queryInfo.dept_id);
                    if (queryInfo.position_id > 0)
                        qable.Where(a => a.position_id == queryInfo.position_id);
                    if (!string.IsNullOrEmpty(queryInfo.position_name))
                        qable.Where(a => a.position_name.Contains(queryInfo.position_name));
                }
                if (queryTime != null)
                {
                    if (queryTime.startTime1 != null)
                        qable.Where(a => a.entry_date >= queryTime.startTime1);
                    if (queryTime.startTime2 != null)
                    {
                        queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                        qable.Where(a => a.entry_date < queryTime.startTime2);
                    }
                }

                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                       .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        /// <summary>
        /// 补助设置
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryInfo"></param>
        /// <returns></returns>
        public object GetSubList(Pagination pagination, daoben_hr_emp_job queryInfo)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "entry_date" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            using (var db = SugarDao.GetInstance())
            {
                //查看全部行政人员，由部门经理发起
                var qable = db.Queryable<daoben_hr_emp_job>().Where(a => a.position_type <= ConstData.POSITION_OFFICE_NORMAL);

                //if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                //{
                //    if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER)
                //        qable.Where(a => a.company_id == myCompanyInfo.id);
                //    else if (myPositionInfo.positionType == ConstData.POSITION_DEPT_M)
                //        qable.Where(a => a.dept_id == myPositionInfo.id);
                //    else
                //        qable.Where(a => a.supervisor_id == LoginInfo.empId);
                //}
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (myCompanyInfo.category == "事业部")
                        qable.Where(a => (a.company_id_parent == myCompanyInfo.id || a.company_id == myCompanyInfo.id));
                    else
                        qable.Where(a => a.company_id == myCompanyInfo.id);
                }


                //姓名 机构 部门 职位名 
                if (queryInfo != null)
                {
                    if (!string.IsNullOrEmpty(queryInfo.name))
                        qable.Where(a => a.name.Contains(queryInfo.name));
                    if (!string.IsNullOrEmpty(queryInfo.work_number))
                        qable.Where(a => a.work_number.Contains(queryInfo.work_number));
                    if (queryInfo.company_id > 0)
                        qable.Where(a => a.company_id == queryInfo.company_id);
                    if (queryInfo.dept_id > 0)
                        qable.Where(a => a.dept_id == queryInfo.dept_id);
                    if (queryInfo.position_id > 0)
                        qable.Where(a => a.position_id == queryInfo.position_id);
                }
                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                       .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }
        #endregion

        public string AddGeneral(daoben_salary_emp addInfo, daoben_salary_emp_general addGeneralInfo)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            if (addInfo == null || addGeneralInfo == null)
                return "信息错误：设置内容不能为空";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            addInfo.id = Common.GuId();
            addInfo.category = 1;
            addInfo.approve_status = 0;
            addInfo.create_time = DateTime.Now;
            addInfo.creator_job_history_id = LoginInfo.jobHistoryId;
            addInfo.creator_name = LoginInfo.empName;
            addInfo.create_time = DateTime.Now;
            if (addInfo.effect_now)
                addInfo.effect_date = DateTime.Now;
            addGeneralInfo.main_id = addInfo.id;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    bool isExist = db.Queryable<daoben_hr_emp_job>().Any(a => a.id == addInfo.emp_id);
                    if (!isExist)
                        return "信息错误：指定的员工信息不存在";
                    db.CommandTimeOut = 30;
                    db.BeginTran();

                    db.Insert(addInfo);
                    db.DisableInsertColumns = new string[] { "id" };
                    db.Insert(addGeneralInfo);

                    db.CommitTran();

                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return ex.Message;
                }
                //12.23临时设为已审批
                approveTemp(addInfo.id);
                return "success";
            }
        }

        public string AddSales(daoben_salary_emp mainInfo, List<IdNamePair> empList, daoben_salary_emp_sales salesInfo, List<daoben_salary_emp_sales_sub> subList)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            if (empList.Count() < 1 || mainInfo == null || subList == null || subList.Count() == 0)
                return "信息错误：设置内容不能为空";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            List<daoben_salary_emp> totalSalaryList = new List<daoben_salary_emp>();
            List<daoben_salary_emp_sales> totalSalesList = new List<daoben_salary_emp_sales>();
            List<daoben_salary_emp_sales_sub> totalSubList = new List<daoben_salary_emp_sales_sub>();
            DateTime now = DateTime.Now;
            if (mainInfo.effect_now)
                mainInfo.effect_date = now;
            foreach (var e in empList)
            {
                daoben_salary_emp tempSalary = new daoben_salary_emp()
                {
                    id = Common.GuId(),
                    approve_status = 0,
                    category = 4,
                    create_time = now,
                    creator_job_history_id = LoginInfo.jobHistoryId,
                    creator_name = LoginInfo.empName,
                    effect_date = mainInfo.effect_date,
                    effect_now = mainInfo.effect_now,
                    effect_status = mainInfo.effect_status,
                    emp_id = e.id,
                };
                totalSalaryList.Add(tempSalary);
                daoben_salary_emp_sales tempSales = new daoben_salary_emp_sales()
                {
                    salary_position_id = tempSalary.id,
                    activity_target = salesInfo.activity_target,
                    note = salesInfo.note,
                    buyout_rebate_mode = salesInfo.buyout_rebate_mode,
                    normal_rebate_mode = salesInfo.normal_rebate_mode,
                    target_content = salesInfo.target_content,
                    target_mode = salesInfo.target_mode,
                    total_penalty = salesInfo.total_penalty,
                    total_reward = salesInfo.total_reward,
                    total_sale_amount = salesInfo.total_sale_amount,
                    total_sale_count = salesInfo.total_sale_count,
                    resign_deposit = salesInfo.resign_deposit,
                    traffic_subsidy = salesInfo.traffic_subsidy,
                    intern_fix_salary = salesInfo.intern_fix_salary,
                    intern_ratio_salary = salesInfo.intern_ratio_salary,
                    intern_salary_type = salesInfo.intern_salary_type,
                    id = Common.GuId(),
                };
                List<daoben_salary_emp_sales_sub> tempSubList = new List<daoben_salary_emp_sales_sub>();
                foreach (var a in subList)
                {
                    daoben_salary_emp_sales_sub tempSub = new daoben_salary_emp_sales_sub()
                    {
                        main_id = tempSales.id,
                        buyout_rebate = a.buyout_rebate,
                        sale_rebate = a.sale_rebate,
                        target_max = a.target_max,
                        target_min = a.target_min,
                    };
                    tempSubList.Add(tempSub);
                }
                totalSubList.AddRange(tempSubList);
                totalSalesList.Add(tempSales);
            }
            //addGeneralInfo.main_id = addInfo.id;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    //已审批    12.29                 
                    object inactiveObj = new
                    {
                        effect_status = 2,
                        approve_status = 100,
                    };
                    List<string> empIdList = empList.Select(t => t.id).ToList();
                    db.Update<daoben_salary_emp>(inactiveObj, a => empIdList.Contains(a.emp_id) && a.category == 4);
                    foreach (var a in totalSalaryList)
                    {
                        a.effect_status = 1;
                        a.approve_status = 100;
                    }
                    if (totalSalaryList != null && totalSalaryList.Count() > 25)
                        db.SqlBulkCopy(totalSalaryList);
                    else if (totalSalaryList != null && totalSalaryList.Count() > 0)
                        db.InsertRange(totalSalaryList);
                    if (totalSalesList != null && totalSalesList.Count() > 25)
                        db.SqlBulkCopy(totalSalesList);
                    else if (totalSalesList != null && totalSalesList.Count() > 0)
                        db.InsertRange(totalSalesList);
                    db.DisableInsertColumns = new string[] { "id" };
                    if (totalSubList != null && totalSubList.Count() > 25)
                        db.SqlBulkCopy(totalSubList);
                    else if (totalSubList != null && totalSubList.Count() > 0)
                        db.InsertRange(totalSubList);

                    db.CommitTran();

                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return ex.Message;
                }
                //12.29临时设为已审批
                //List<string> idList = empList.Select(t => t.id).ToList();
                //approveTemp(idList);
                return "success";
            }
        }
        public string AddTrainer(daoben_salary_emp addInfo, List<daoben_salary_emp_trainer> subList)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            if (addInfo == null || subList == null || subList.Count() == 0)
                return "信息错误：设置内容不能为空";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            addInfo.category = 5;
            addInfo.approve_status = 0;
            addInfo.create_time = DateTime.Now;
            addInfo.creator_job_history_id = LoginInfo.jobHistoryId;
            addInfo.creator_name = LoginInfo.empName;
            addInfo.create_time = DateTime.Now;
            if (addInfo.effect_now)
                addInfo.effect_date = DateTime.Now;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_hr_emp_job empJob = db.Queryable<daoben_hr_emp_job>()
                        .SingleOrDefault(a => a.id == addInfo.emp_id);
                    if (empJob == null)
                        return "信息错误：指定的员工信息不存在";
                    if (empJob.position_type == ConstData.POSITION_TRAINER)
                    {
                        if (subList.Count() != 8)
                            return "信息错误：需要指定全部的KPI考核项";
                    }
                    else if (empJob.position_type == ConstData.POSITION_TRAINERMANAGER)
                    {
                        if (subList.Count() != 3)
                            return "信息错误：需要指定全部的KPI考核项";
                    }
                    else
                        return "信息错误";
                    addInfo.id = Common.GuId();
                    foreach (var a in subList)
                    {
                        a.main_id = addInfo.id;
                    }
                    db.Insert(addInfo);
                    db.DisableInsertColumns = new string[] { "id" };
                    if (subList != null && subList.Count() > 25)
                        db.SqlBulkCopy(subList);
                    else if (subList != null && subList.Count() > 0)
                        db.InsertRange(subList);

                    db.CommitTran();

                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return ex.Message;
                }

                approveTemp(addInfo.id);
                return "success";
            }
        }
        public string AddGuide(List<IdNamePair> empList, daoben_salary_emp mainInfo, daoben_salary_emp_general addGeneral)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            if (empList.Count() < 1 || addGeneral == null)
                return "信息错误：设置内容不能为空";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            List<daoben_salary_emp_general> totalAddGenral = new List<daoben_salary_emp_general>();
            List<daoben_salary_emp> totalSalaryList = new List<daoben_salary_emp>();
            DateTime now = DateTime.Now;
            if (mainInfo.effect_now)
                mainInfo.effect_date = now;
            foreach (var e in empList)
            {
                daoben_salary_emp tempSalary = new daoben_salary_emp()
                {
                    id = Common.GuId(),
                    approve_status = 0,
                    category = 1,
                    create_time = now,
                    creator_job_history_id = LoginInfo.jobHistoryId,
                    creator_name = LoginInfo.empName,
                    effect_date = mainInfo.effect_date,
                    effect_now = mainInfo.effect_now,
                    effect_status = mainInfo.effect_status,
                    emp_id = e.id,
                };
                totalSalaryList.Add(tempSalary);
                daoben_salary_emp_general tempGeneral = new daoben_salary_emp_general()
                {
                    main_id = tempSalary.id,
                    resign_deposit = addGeneral.resign_deposit,
                    traffic_subsidy = addGeneral.traffic_subsidy,
                    intern_salary_type = addGeneral.intern_salary_type,
                    intern_fix_salary = addGeneral.intern_fix_salary,
                    intern_ratio_salary = addGeneral.intern_ratio_salary,
                    guide_salary_base = addGeneral.guide_salary_base,
                    guide_base_type = addGeneral.guide_base_type,
                    guide_annualbonus_type = addGeneral.guide_annualbonus_type,
                    guide_standard_commission = addGeneral.guide_standard_commission,
                    guide_standard_salary = addGeneral.guide_standard_salary
                };
                totalAddGenral.Add(tempGeneral);
            }
            //addGeneralInfo.main_id = addInfo.id;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    //已审批    12.29                 
                    object inactiveObj = new
                    {
                        effect_status = 2,
                        approve_status = 100,
                    };
                    List<string> empIdList = empList.Select(t => t.id).ToList();
                    db.Update<daoben_salary_emp>(inactiveObj, a => empIdList.Contains(a.emp_id) && a.category == 1);
                    foreach (var a in totalSalaryList)
                    {
                        a.effect_status = 1;
                        a.approve_status = 100;
                    }

                    if (totalSalaryList != null && totalSalaryList.Count() > 25)
                        db.SqlBulkCopy(totalSalaryList);
                    else if (totalSalaryList != null && totalSalaryList.Count() > 0)
                        db.InsertRange(totalSalaryList);
                    db.DisableInsertColumns = new string[] { "id" };
                    if (totalAddGenral != null && totalAddGenral.Count() > 25)
                        db.SqlBulkCopy(totalAddGenral);
                    else if (totalAddGenral != null && totalAddGenral.Count() > 0)
                        db.InsertRange(totalAddGenral);

                    db.CommitTran();

                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return ex.Message;
                }
                //12.29临时设为已审批
                //List<string> idList = empList.Select(t => t.id).ToList();
                //approveTemp(idList);
                return "success";
            }
        }
        public string AddKpi(daoben_salary_emp addInfo, daoben_salary_emp_kpi_subsidy subsidyInfo)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            if (addInfo == null || subsidyInfo == null)
                return "信息错误：设置内容不能为空";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            addInfo.id = Common.GuId();
            addInfo.category = 2;
            addInfo.approve_status = 0;
            addInfo.create_time = DateTime.Now;
            addInfo.creator_job_history_id = LoginInfo.jobHistoryId;
            addInfo.creator_name = LoginInfo.empName;
            addInfo.create_time = DateTime.Now;
            subsidyInfo.main_id = addInfo.id;
            if (addInfo.effect_now)
                addInfo.effect_date = DateTime.Now;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_hr_emp_job jobInfo = db.Queryable<daoben_hr_emp_job>().InSingle(addInfo.emp_id);
                    if (jobInfo == null || jobInfo.position_type == 0)
                        return "信息错误：指定的员工信息不存在或类型错误";

                    db.CommandTimeOut = 30;
                    db.BeginTran();

                    db.Insert(addInfo);
                    db.DisableInsertColumns = new string[] { "id" };
                    db.Insert(subsidyInfo);

                    db.CommitTran();

                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
                //12.23临时设为已审批
                approveTemp(addInfo.id);
                return "success";
            }
        }
        public string AddSubsidy(daoben_salary_emp addInfo, daoben_salary_emp_subsidy subsidyInfo)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            if (addInfo == null || subsidyInfo == null)
                return "信息错误：设置内容不能为空";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            addInfo.id = Common.GuId();
            addInfo.category = 3;
            addInfo.approve_status = 0;
            addInfo.create_time = DateTime.Now;
            addInfo.creator_job_history_id = LoginInfo.jobHistoryId;
            addInfo.creator_name = LoginInfo.empName;
            addInfo.create_time = DateTime.Now;
            subsidyInfo.main_id = addInfo.id;
            if (addInfo.effect_now)
                addInfo.effect_date = DateTime.Now;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_hr_emp_job jobInfo = db.Queryable<daoben_hr_emp_job>().InSingle(addInfo.emp_id);
                    if (jobInfo == null || jobInfo.position_type == 0)
                        return "信息错误：指定的员工信息不存在或类型错误";

                    db.CommandTimeOut = 30;
                    db.BeginTran();

                    db.Insert(addInfo);
                    db.DisableInsertColumns = new string[] { "id" };
                    db.Insert(subsidyInfo);

                    db.CommitTran();

                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
                //12.23临时设为已审批
                approveTemp(addInfo.id);
                return "success";
            }
        }
        /// <summary>
        /// 查看页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetInfo(string id)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_hr_emp_job jobInfo = db.Queryable<daoben_hr_emp_job>().InSingle(id);
                    if (jobInfo == null)
                        return "信息错误：指定的员工信息不存在";

                    #region 职等薪资 

                    daoben_payroll_office payrollInfo = new daoben_payroll_office();
                    if (jobInfo.position_type != ConstData.POSITION_GUIDE)
                    {
                        int gradeCagetory = 0;
                        if (jobInfo.dept_id == 0)
                            gradeCagetory = 1; // 无部门时，默认为 1-行政管理
                        else
                            gradeCagetory = db.Queryable<daoben_org_dept>()
                                    .Where(a => a.id == jobInfo.dept_id).Select(a => a.grade_category).SingleOrDefault();
                        if (gradeCagetory == 0)
                            return "信息错误：该员工职等类型有误，请联系系统管理员";

                        daoben_salary_position_grade spGrade = db.Queryable<daoben_salary_position_grade>()
                                    .JoinTable<daoben_salary_position>((a, b) => a.salary_position_id == b.id)
                                    .Where<daoben_salary_position>((a, b) => b.category == 4 && b.effect_status == 1
                                    && b.company_id == jobInfo.company_id && a.grade_category == gradeCagetory && a.grade == jobInfo.grade)
                                    .Select("a.base_salary, a.position_salary, a.house_subsidy, a.attendance_reward, a.kpi_advice,a.total").SingleOrDefault();
                        if (spGrade == null)
                            return "信息错误：无该员工对应的职等工资信息，请先导入";
                        payrollInfo.base_salary = spGrade.base_salary;
                        payrollInfo.position_salary = spGrade.position_salary;
                        payrollInfo.house_subsidy = spGrade.house_subsidy;
                        payrollInfo.attendance_reward = spGrade.attendance_reward;
                        payrollInfo.position_standard_salary = spGrade.total;
                    }
                    #endregion


                    if (jobInfo.position_type == ConstData.POSITION_SALES || jobInfo.position_type == ConstData.POSITION_SALESMANAGER)
                    {
                        #region 业务薪资
                        daoben_salary_emp salaryInfo = db.Queryable<daoben_salary_emp>()
                                    .SingleOrDefault(a => a.emp_id == jobInfo.id && a.effect_status == 1 && a.category == 4);
                        if (salaryInfo != null)
                        {
                            daoben_salary_emp_sales salesInfo = db.Queryable<daoben_salary_emp_sales>()
                                .SingleOrDefault(a => a.salary_position_id == salaryInfo.id);
                            if (salesInfo == null)
                                return null;
                            payrollInfo.traffic_subsidy = salesInfo.traffic_subsidy;
                            payrollInfo.resign_deposit = salesInfo.resign_deposit;

                            List<daoben_salary_emp_sales_sub> subList = db.Queryable<daoben_salary_emp_sales_sub>()
                                .Where(a => a.main_id == salesInfo.id).ToList();
                            object returnObj = new
                            {
                                jobInfo = jobInfo,
                                salaryInfo = salaryInfo,
                                salesInfo = salesInfo,
                                subList = subList,
                                payrollInfo = payrollInfo
                            };
                            return returnObj.ToJson();
                        }
                        else
                        {
                            //读取岗位薪资
                            daoben_salary_emp_sales salesInfo = new daoben_salary_emp_sales();
                            daoben_salary_position posSlaryInfo = db.Queryable<daoben_salary_position>()
                                    .SingleOrDefault(t => t.position_id == jobInfo.position_id && t.effect_status == 1 && t.approve_status == 100
                                    && t.category == jobInfo.position_type);    // category 与 jobInfo.position_type 相对应（仅业务员/业务经理）
                            List<daoben_salary_emp_sales_sub> subList = null;
                            daoben_salary_position_sales tempInfo = db.Queryable<daoben_salary_position_sales>()
                            .Where(a => a.salary_position_id == posSlaryInfo.id)
                            .SingleOrDefault();
                            if (posSlaryInfo == null)
                                return null;
                            salesInfo.id = tempInfo.id;
                            salesInfo.salary_position_id = tempInfo.salary_position_id;
                            salesInfo.activity_target = tempInfo.activity_target;
                            salesInfo.target_content = tempInfo.target_content;
                            salesInfo.target_mode = tempInfo.target_mode;
                            salesInfo.normal_rebate_mode = tempInfo.normal_rebate_mode;
                            salesInfo.buyout_rebate_mode = tempInfo.buyout_rebate_mode;
                            subList = db.Queryable<daoben_salary_position_sales_sub>()
                                    .Where(a => a.main_id == salesInfo.id)
                                    .Select<daoben_salary_emp_sales_sub>("*").ToList();
                            object returnObj = new
                            {
                                jobInfo = jobInfo,
                                salaryInfo = posSlaryInfo,
                                salesInfo = salesInfo,
                                subList = subList,
                                payrollInfo = payrollInfo
                            };
                            return returnObj.ToJson();
                        }
                        #endregion
                    }
                    else if (jobInfo.position_type == ConstData.POSITION_TRAINER || jobInfo.position_type == ConstData.POSITION_TRAINERMANAGER)
                    {
                        #region 培训薪资
                        daoben_salary_emp salaryInfo = db.Queryable<daoben_salary_emp>()
                            .SingleOrDefault(a => a.emp_id == jobInfo.id && a.effect_status == 1 && a.category == 1);
                        daoben_salary_emp salarySubInfo = db.Queryable<daoben_salary_emp>()
                            .SingleOrDefault(a => a.emp_id == jobInfo.id && a.effect_status == 1 && a.category == 5);
                        daoben_salary_emp_general trainerInfo = new daoben_salary_emp_general();
                        List<daoben_salary_emp_trainer> trainerSubList = new List<daoben_salary_emp_trainer>();


                        if (salaryInfo != null)
                            trainerInfo = db.Queryable<daoben_salary_emp_general>()
                                .SingleOrDefault(a => a.main_id == salaryInfo.id);
                        else
                        {
                            trainerInfo = db.Queryable<daoben_salary_position_other>()
                                    .JoinTable<daoben_salary_position>((a, b) => a.salary_position_id == b.id)
                                    .Where<daoben_salary_position>((a, b) => b.company_id == jobInfo.company_id && b.effect_status == 1 && b.category == 3)
                                    .Select<daoben_salary_emp_general>("a.salary_position_id as main_id, a.resign_deposit,a.intern_salary_type,a.intern_fix_salary,a.intern_ratio_salary")
                                    .SingleOrDefault();
                        }
                        if (salarySubInfo != null)
                            trainerSubList = db.Queryable<daoben_salary_emp_trainer>()
                                .Where(a => a.main_id == salarySubInfo.id).ToList();

                        if (jobInfo.position_type == ConstData.POSITION_TRAINER)
                        {
                            daoben_salary_position mainPosKPI = db.Queryable<daoben_salary_position>()
                                        .SingleOrDefault(t => t.category == 11 && t.effect_status == 1 && t.position_id == jobInfo.position_id);
                            daoben_salary_position_trainer positionKPI = db.Queryable<daoben_salary_position_trainer>()
                                        .JoinTable<daoben_salary_position>((a, b) => a.salary_position_id == b.id)
                                        .Where<daoben_salary_position>((a, b) => b.effect_status == 1 && b.position_id == jobInfo.position_id)
                                        .Select("a.*").SingleOrDefault();
                            object returnObj = new
                            {
                                jobInfo = jobInfo,
                                salaryInfo = salaryInfo,
                                trainerInfo = trainerInfo,
                                salarySubInfo = salarySubInfo,
                                trainerSubList = trainerSubList,
                                positionKPI = positionKPI,
                                mainPosKPI = mainPosKPI,
                                payrollInfo = payrollInfo
                            };
                            return returnObj.ToJson();
                        }
                        else
                        {
                            daoben_salary_position mainPosKPI = db.Queryable<daoben_salary_position>()
                                        .SingleOrDefault(t => t.category == 12 && t.effect_status == 1 && t.position_id == jobInfo.position_id);
                            daoben_salary_position_trainermanager positionKPI = db.Queryable<daoben_salary_position_trainermanager>()
                                        .JoinTable<daoben_salary_position>((a, b) => a.salary_position_id == b.id)
                                        .Where<daoben_salary_position>((a, b) => b.effect_status == 1 && b.position_id == jobInfo.position_id)
                                        .Select("a.*").SingleOrDefault();
                            object returnObj = new
                            {
                                jobInfo = jobInfo,
                                salaryInfo = salaryInfo,
                                trainerInfo = trainerInfo,
                                salarySubInfo = salarySubInfo,
                                trainerSubList = trainerSubList,
                                positionKPI = positionKPI,
                                mainPosKPI = mainPosKPI,
                                payrollInfo = payrollInfo
                            };
                            return returnObj.ToJson();
                        }
                        #endregion
                    }
                    else if (jobInfo.position_type == ConstData.POSITION_GUIDE)
                    {
                        //提供给前端，导购员的增员奖励等信息，通过职位ID-薪资主表-导购员薪资表
                        daoben_salary_position_guide guidePosInfo = new daoben_salary_position_guide();
                        daoben_salary_position posSalaryInfo = db.Queryable<daoben_salary_position>()
                                .SingleOrDefault(a => a.position_id == jobInfo.position_id && a.effect_status == 1 && a.category == 1 && a.approve_status == 100);
                        if (posSalaryInfo != null)
                            guidePosInfo = db.Queryable<daoben_salary_position_guide>()
                                    .SingleOrDefault(a => a.salary_position_id == posSalaryInfo.id);

                        daoben_salary_emp salaryInfo = db.Queryable<daoben_salary_emp>()
                            .SingleOrDefault(a => a.emp_id == jobInfo.id && a.effect_status == 1 && a.category == 1);
                        daoben_salary_emp_general guideGeneralInfo = null;
                        if (salaryInfo != null)
                        {
                            guideGeneralInfo = db.Queryable<daoben_salary_emp_general>()
                                .SingleOrDefault(a => a.main_id == salaryInfo.id);
                        }
                        else
                        {
                            //导购员 交通补贴 离职押金 只有在员工设置才有
                            guideGeneralInfo = db.Queryable<daoben_salary_position_other>()
                                    .JoinTable<daoben_salary_position>((a, b) => a.salary_position_id == b.id)
                                    .Where<daoben_salary_position>((a, b) => b.company_id == jobInfo.company_id && b.effect_status == 1 && b.category == 3)
                                    .Select<daoben_salary_emp_general>("a.salary_position_id as main_id, a.resign_deposit,a.intern_salary_type,a.intern_fix_salary,a.intern_ratio_salary")
                                    .SingleOrDefault();
                        }
                        object returnObj = new
                        {
                            jobInfo = jobInfo,
                            generalSalaryInfo = salaryInfo,
                            generalInfo = guideGeneralInfo,
                            guidePosInfo = guidePosInfo,
                            guidePosMainInfo = posSalaryInfo,
                        };
                        return returnObj.ToJson();

                    }
                    else
                    {
                        //基本薪资
                        daoben_salary_emp generalSalaryInfo = db.Queryable<daoben_salary_emp>()
                            .SingleOrDefault(a => a.emp_id == id && a.category == 1 && a.effect_status == 1 && a.approve_status == 100);
                        daoben_salary_emp_general generalInfo = new daoben_salary_emp_general();
                        DateTime? generalEffectTime = null;
                        if (generalSalaryInfo != null)
                        {
                            generalInfo = db.Queryable<daoben_salary_emp_general>().SingleOrDefault(a => a.main_id == generalSalaryInfo.id);
                            generalEffectTime = generalSalaryInfo.effect_date;
                        }
                        else
                        {
                            generalInfo = db.Queryable<daoben_salary_position_other>()
                                    .JoinTable<daoben_salary_position>((a, b) => a.salary_position_id == b.id)
                                    .Where<daoben_salary_position>((a, b) => b.company_id == jobInfo.company_id && b.effect_status == 1 && b.category == 3)
                                    .Select<daoben_salary_emp_general>("a.salary_position_id as main_id, a.resign_deposit,a.intern_salary_type,a.intern_fix_salary,a.intern_ratio_salary")
                                    .SingleOrDefault();
                        }
                        //KPI补贴
                        daoben_salary_emp kpiSubSalaryInfo = db.Queryable<daoben_salary_emp>()
                                .SingleOrDefault(a => a.emp_id == id && a.category == 2 && a.effect_status == 1 && a.approve_status == 100);
                        daoben_salary_emp_kpi_subsidy kpiSubInfo = new daoben_salary_emp_kpi_subsidy();
                        DateTime? kpiEffectTime = null;
                        if (kpiSubSalaryInfo != null)
                        {
                            kpiSubInfo = db.Queryable<daoben_salary_emp_kpi_subsidy>().SingleOrDefault(a => a.main_id == kpiSubSalaryInfo.id);
                            kpiEffectTime = kpiSubSalaryInfo.effect_date;
                        }
                        //薪资补助
                        daoben_salary_emp SubSalaryInfo = db.Queryable<daoben_salary_emp>()
                                .SingleOrDefault(a => a.emp_id == id && a.category == 3 && a.effect_status == 1 && a.approve_status == 100);
                        daoben_salary_emp_subsidy SubInfo = new daoben_salary_emp_subsidy();
                        DateTime? subEffectTime = null;
                        if (SubSalaryInfo != null)
                        {
                            SubInfo = db.Queryable<daoben_salary_emp_subsidy>().SingleOrDefault(a => a.main_id == SubSalaryInfo.id);
                            subEffectTime = SubSalaryInfo.effect_date;
                        }


                        object resultObj = new
                        {
                            jobInfo = jobInfo,
                            generalSalaryInfo = generalSalaryInfo,
                            generalInfo = generalInfo,
                            kpiSubInfo = kpiSubInfo,
                            kpiEffectTime = kpiEffectTime,
                            SubInfo = SubInfo,
                            subEffectTime = subEffectTime,
                            payrollInfo = payrollInfo
                        };
                        return resultObj.ToJson();
                    }
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
        /// <summary>
        /// 审批/设置页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetSalaryInfo(string id)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_salary_emp salaryInfo = db.Queryable<daoben_salary_emp>().SingleOrDefault(a => a.id == id);
                    if (salaryInfo == null)
                        return null;
                    daoben_hr_emp_job jobInfo = db.Queryable<daoben_hr_emp_job>().InSingle(salaryInfo.emp_id);
                    if (salaryInfo.category == 1)
                    {
                        daoben_salary_emp_general generalInfo = db.Queryable<daoben_salary_emp_general>()
                                .SingleOrDefault(a => a.main_id == salaryInfo.id);
                        if (generalInfo == null)
                            return null;
                        List<daoben_salary_emp_approve> approveInfoList = db.Queryable<daoben_salary_emp_approve>().Where(a => a.main_id == salaryInfo.id).ToList();
                        object resultObj = new
                        {
                            jobInfo = jobInfo,
                            salaryInfo = salaryInfo,
                            generalInfo = generalInfo,
                            approveInfoList = approveInfoList
                        };
                        return resultObj.ToJson();
                    }
                    else if (salaryInfo.category == 2)
                    {
                        daoben_salary_emp_kpi_subsidy kpiSubInfo = db.Queryable<daoben_salary_emp_kpi_subsidy>()
                                .SingleOrDefault(a => a.main_id == salaryInfo.id);
                        if (kpiSubInfo == null)
                            return null;
                        List<daoben_salary_emp_approve> approveInfoList = db.Queryable<daoben_salary_emp_approve>().Where(a => a.main_id == salaryInfo.id).ToList();
                        object resultObj = new
                        {
                            jobInfo = jobInfo,
                            salaryInfo = salaryInfo,
                            kpiSubInfo = kpiSubInfo,
                            approveInfoList = approveInfoList
                        };
                        return resultObj.ToJson();
                    }
                    else if (salaryInfo.category == 3)
                    {
                        daoben_salary_emp_subsidy subsidyInfo = db.Queryable<daoben_salary_emp_subsidy>()
                                .SingleOrDefault(a => a.main_id == salaryInfo.id);
                        if (subsidyInfo == null)
                            return null;
                        List<daoben_salary_emp_approve> approveInfoList = db.Queryable<daoben_salary_emp_approve>().Where(a => a.main_id == salaryInfo.id).ToList();
                        object resultObj = new
                        {
                            jobInfo = jobInfo,
                            salaryInfo = salaryInfo,
                            subsidyInfo = subsidyInfo,
                            approveInfoList = approveInfoList
                        };
                        return resultObj.ToJson();
                    }
                    else if (salaryInfo.category == 4)
                    {
                        daoben_salary_emp_sales salesInfo = db.Queryable<daoben_salary_emp_sales>()
                            .SingleOrDefault(t => t.salary_position_id == salaryInfo.id);
                        if (salesInfo == null)
                            return null;
                        List<daoben_salary_emp_sales_sub> subList = db.Queryable<daoben_salary_emp_sales_sub>()
                            .Where(a => a.main_id == salesInfo.id).ToList();
                        object resultObj = new
                        {
                            jobInfo = jobInfo,
                            salaryInfo = salaryInfo,
                            salesInfo = salesInfo,
                            subList = subList
                        };
                        return resultObj.ToJson();
                    }
                    else if (salaryInfo.category == 5)
                    {
                        List<daoben_salary_emp_trainer> subList = db.Queryable<daoben_salary_emp_trainer>()
                            .Where(a => a.main_id == salaryInfo.id).ToList();
                        object resultObj = new
                        {
                            jobInfo = jobInfo,
                            salaryInfo = salaryInfo,
                            subList = subList
                        };
                        return resultObj.ToJson();
                    }
                    else
                    {
                        object resultObj = new
                        {
                            jobInfo = jobInfo,
                            salaryInfo = salaryInfo,
                        };
                        return resultObj.ToJson();
                    }
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
        public string Approve(daoben_salary_emp_approve approveInfo)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            object upObj = null;
            object inactiveUpObj = null;
            object empUpObj = null;
            approveInfo.approve_id = LoginInfo.accountId;
            approveInfo.approve_name = LoginInfo.empName;
            approveInfo.approve_time = DateTime.Now;
            approveInfo.approve_position_id = myPositionInfo.id;
            approveInfo.approve_position_name = myPositionInfo.name;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_salary_emp empSalary = db.Queryable<daoben_salary_emp>().InSingle(approveInfo.main_id);
                    if (empSalary == null)
                        return "信息错误：不存在对应的员工薪资信息";
                    daoben_hr_emp_job empInfo = db.Queryable<daoben_hr_emp_job>().InSingle(empSalary.emp_id);
                    if (empInfo == null)
                        return "信息错误：指定的员工信息不存在";


                    if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER && approveInfo.status > 0)
                    {
                        //approveInfo.status = 100; //以100作为审批完成的标志
                        //if (empSalary.category == 1 && empInfo.salary_blank)    // 新入职员工首次设置工资
                        //    empUpObj = new { salary_blank = false };
                        if (empSalary.effect_now || empSalary.effect_date <= DateTime.Now)
                        {
                            upObj = new { approve_status = 100, effect_status = 1, effect_date = DateTime.Now };
                            if (empUpObj == null)
                                inactiveUpObj = new { effect_status = 2, invalid_date = DateTime.Now };
                        }
                        else
                            upObj = new { approve_status = 100, effect_status = -1 };
                        //TODO 审批通过之后~~
                    }
                    else
                    {
                        if (approveInfo.status > 0)
                            empSalary.approve_status = 1 + empSalary.approve_status;
                        else
                            empSalary.approve_status = 0 - 1 - empSalary.approve_status;
                        upObj = new { approve_status = approveInfo.status };
                    }
                    db.BeginTran();
                    if (empUpObj != null)
                        db.Update<daoben_hr_emp_job>(empUpObj, a => a.id == empSalary.emp_id);
                    else if (inactiveUpObj != null)
                    {
                        db.Update<daoben_salary_emp>(inactiveUpObj,
                                a => a.emp_id == empSalary.emp_id && a.category == empSalary.category && a.effect_status == 1);
                    }
                    db.Update<daoben_salary_emp>(upObj, a => a.id == empSalary.id);
                    db.DisableInsertColumns = new string[] { "id" };
                    db.Insert(approveInfo);

                    db.CommitTran();
                    return "success";
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return ex.Message;
                }
            }
        }

        public string Delete(string id)
        {
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_salary_emp mainInfo = db.Queryable<daoben_salary_emp>().SingleOrDefault(a => a.id == id);
                    if (mainInfo == null || mainInfo.approve_status != 0)
                        //审批过后，将不能撤回
                        return "撤回失败：指定的申请信息不存在或者已被审批";
                    else
                    {
                        db.CommandTimeOut = 30;
                        db.BeginTran();
                        if (mainInfo.category == 1)
                            db.Delete<daoben_salary_emp_general>(a => a.main_id == id);
                        else if (mainInfo.category == 2)
                            db.Delete<daoben_salary_emp_kpi_subsidy>(a => a.main_id == id);
                        else if (mainInfo.category == 3)
                            db.Delete<daoben_salary_emp_subsidy>(a => a.main_id == id);
                        db.Delete<daoben_salary_emp>(a => a.id == id);
                        db.CommitTran();
                        return "success";
                    }
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return "系统出错：" + ex.Message;
                }
            }
        }

        public decimal GetPosSalary(string id)
        {
            decimal posSalary = 0;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_hr_emp_job empInfo = db.Queryable<daoben_hr_emp_job>().SingleOrDefault(a => a.id == id);
                    if (empInfo == null)
                        return posSalary;
                    int gradeCagetory = 0;
                    if (empInfo.dept_id == 0)
                        gradeCagetory = 1; // 无部门时，默认为 1-行政管理
                    else
                        gradeCagetory = db.Queryable<daoben_org_dept>()
                                .Where(a => a.id == empInfo.dept_id).Select<int>("grade_category").SingleOrDefault();
                    if (gradeCagetory == 0)
                        return posSalary;
                    daoben_salary_position_grade spGrade = db.Queryable<daoben_salary_position_grade>()
                                .JoinTable<daoben_salary_position>((a, b) => a.salary_position_id == b.id)
                                .Where<daoben_salary_position>((a, b) => b.category == 4 && b.effect_status == 1
                                && b.company_id == empInfo.company_id && a.grade_category == gradeCagetory && a.grade == empInfo.grade)
                                .Select("a.base_salary, a.position_salary, a.house_subsidy, a.attendance_reward, a.kpi_advice,a.total").SingleOrDefault();
                    if (spGrade == null)
                        return posSalary;
                    posSalary = spGrade.position_salary;
                    return posSalary;
                }
                catch (Exception ex)
                {
                    return posSalary;
                }
            }
        }

        private void approveTemp(string id)
        {
            using (var db = SugarDao.GetInstance())
            {
                daoben_salary_emp mainInfo = db.Queryable<daoben_salary_emp>().InSingle(id);
                object upObj = new
                {
                    effect_status = 1,
                    approve_status = 100,
                };
                object inactiveObj = new
                {
                    effect_status = 2,
                };
                object upObj2 = new
                {
                    effect_status = -1,
                    approve_status = 100,
                };
                if (mainInfo.effect_now || mainInfo.effect_date <= DateTime.Now)
                {
                    db.Update<daoben_salary_emp>(inactiveObj, a => a.emp_id == mainInfo.emp_id && a.category == mainInfo.category);
                    db.Update<daoben_salary_emp>(upObj, a => a.id == id);
                }
                else
                {
                    db.Update<daoben_salary_emp>(upObj2, a => a.id == id);
                }

            }
        }

        /// <summary>
        /// 获取KPI考核项
        /// </summary>
        /// <param name="id">员工ID</param>
        /// <returns></returns>
        public string GetTrainerSetInfo(string id)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (string.IsNullOrEmpty(id))
                return "信息错误：ID不能为空";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            using (var db = SugarDao.GetInstance())
            {
                daoben_hr_emp_job empInfo = db.Queryable<daoben_hr_emp_job>().Where(a => a.id == id).SingleOrDefault();
                if (empInfo == null)
                    return "系统出错：员工信息不存在";
                List<daoben_salary_emp_trainer> empKPIList = db.Queryable<daoben_salary_emp_trainer>()
                            .JoinTable<daoben_salary_emp>((a, b) => a.main_id == b.id)
                            .Where<daoben_salary_emp>((a, b) => b.effect_status == 1 && b.emp_id == empInfo.id)
                            .Select("a.id, a.kpi_type,a.is_included,a.area_l1_id,a.area_l1_name")
                            .ToList();
                if (empInfo.position_type == ConstData.POSITION_TRAINER)
                {
                    daoben_salary_position_trainer positionKPI = db.Queryable<daoben_salary_position_trainer>()
                                .JoinTable<daoben_salary_position>((a, b) => a.salary_position_id == b.id)
                                .Where<daoben_salary_position>((a, b) => b.effect_status == 1 && b.position_id == empInfo.position_id)
                                .Select("a.*").SingleOrDefault();
                    object retObj = new
                    {
                        empInfo = empInfo,
                        positionKPI = positionKPI,
                        empKPIList = empKPIList,
                    };
                    return retObj.ToJson();
                }
                else
                {
                    daoben_salary_position_trainermanager positionKPI = db.Queryable<daoben_salary_position_trainermanager>()
                                .JoinTable<daoben_salary_position>((a, b) => a.salary_position_id == b.id)
                                .Where<daoben_salary_position>((a, b) => b.effect_status == 1 && b.position_id == empInfo.position_id)
                                .Select("a.*").SingleOrDefault();
                    object retObj = new
                    {
                        empInfo = empInfo,
                        positionKPI = positionKPI,
                        empKPIList = empKPIList,
                    };
                    return retObj.ToJson();
                }
            }

        }

    }
}
