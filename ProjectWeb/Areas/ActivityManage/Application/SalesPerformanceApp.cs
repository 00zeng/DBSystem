using Base.Code;
using MySqlSugar;
using NPOI.HSSF.UserModel;
using ProjectShare.Database;
using ProjectShare.Models;
using ProjectShare.Process;
using ProjectWeb.Application;
using ProjectWeb.Areas.SalaryCalculate.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ProjectWeb.Areas.ActivityManage.Application
{
    public class SalesPerformanceApp
    {
        public object GetList(Pagination pagination, daoben_activity_sales_perf queryInfo, QueryTime queryTime)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "start_date" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;

            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    // 业务员/业务经理
                    var qable = db.Queryable<daoben_activity_sales_perf>();
                    if (myCompanyInfo.category == "事业部")
                        qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                    else if (myCompanyInfo.category == "分公司")
                        qable.Where(a => a.company_id == myCompanyInfo.id);

                    if (queryInfo != null)
                    {
                        if (queryInfo.company_id > 0)
                            qable.Where(a => a.company_id == queryInfo.company_id);
                        if (queryInfo.activity_status != 0)
                            qable.Where(a => a.activity_status == queryInfo.activity_status);
                        if (queryInfo.category > 0)
                            qable.Where(a => a.category == queryInfo.category);
                        if (queryInfo.approve_status != 0)  // 0表示查找全部
                        {
                            int status = queryInfo.approve_status;
                            if (status == 100)   // 已审批
                                qable.Where(a => a.approve_status == 100);
                            else if (status == -100)    // 审批不通过
                                qable.Where(a => a.approve_status < 0);
                            else if (status == 1)    // 审批中
                                qable.Where(a => a.approve_status > 0 && a.approve_status < 100);
                            else if (status == -1)    // 未审批
                                qable.Where(a => a.approve_status == 0);
                        }
                        if (!string.IsNullOrEmpty(queryInfo.name))
                            qable.Where(a => a.name.Contains(queryInfo.name));
                    }
                    if (queryTime != null)
                    {
                        if (queryTime.startTime1 != null)
                            qable.Where(a => a.create_time >= queryTime.startTime1);
                        if (queryTime.startTime2 != null)
                        {
                            queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                            qable.Where(a => a.create_time < queryTime.startTime2);
                        }
                    }

                    var list = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                           .Select("id, name,category,company_name,activity_status,approve_status,emp_count,start_date,end_date,creator_name, create_time")
                           .ToPageList(pagination.page, pagination.rows, ref records);
                    pagination.records = records;
                    return list;
                }
            }
            catch (Exception ex)
            {
                return "系统出错：" + ex.Message;
            }
        }

        // 我的审批
        public object GetListApprove(Pagination pagination, daoben_hr_emp_job queryInfo)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "start_date" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;

            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    var qable = db.Queryable<daoben_activity_sales_perf>();

                    if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)    // 跳过超级管理员，避免判断公司/职位信息是否存在
                    {
                        if (myPositionInfo.positionType == ConstData.POSITION_GM2)
                        {   // 分公司总经理/分公司总经理助理
                            qable.Where(a => a.approve_status == 0 && a.company_id == myCompanyInfo.id);
                        }
                        else if (myPositionInfo.positionType == ConstData.POSITION_GM1
                                    || myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT1)
                        {   // 事业部总经理/事业部总经理助理
                            qable.Where(a => a.approve_status == 1 && a.company_id_parent == myCompanyInfo.id);

                        }
                        else if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER)
                        {   // 财务经理
                            qable.Where(a => a.approve_status == 2 && a.company_id_parent == myCompanyInfo.id);
                        }
                        else
                            return "无权限，非法访问";
                    }
                    if (queryInfo != null)
                    {
                        //if (!string.IsNullOrEmpty(queryInfo.name))
                        //    qable.Where(a => a.emp_name.Contains(queryInfo.name));
                        if (queryInfo.company_id != 0)
                            qable.Where(a => a.company_id == queryInfo.company_id);

                    }

                    string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                                .Select("id,name,category,emp_count,activity_status,approve_status,start_date,end_date,target_mode,target_content,creator_name,create_time")
                                .ToJsonPage(pagination.page, pagination.rows, ref records);
                    pagination.records = records;
                    if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                        return null;
                    return listStr.ToJson();
                }
            }
            catch (Exception ex)
            {
                return "系统出错：" + ex.Message;
            }
        }

        /// <summary>
        /// 查看所有
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryInfo"></param>
        /// <returns></returns>
        public object GetListRequest(Pagination pagination, daoben_hr_emp_job queryInfo)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "entry_date" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;

            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    // 业务员/业务经理/导购员
                    var qable = db.Queryable<daoben_hr_emp_job>()
                            .Where(a => a.company_id == myCompanyInfo.id && a.position_type >= ConstData.POSITION_SALESMANAGER);

                    if (queryInfo != null)
                    {
                        if (!string.IsNullOrEmpty(queryInfo.name))
                            qable.Where(a => a.name.Contains(queryInfo.name));
                        if (queryInfo.area_l1_id != 0)
                            qable.Where(a => a.area_l1_id == queryInfo.area_l1_id);
                    }
                    string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                            .ToJsonPage(pagination.page, pagination.rows, ref records);
                    pagination.records = records;
                    if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                        return null;
                    return listStr.ToJson();
                }
            }
            catch (Exception ex)
            {
                return "系统出错：" + ex.Message;
            }
        }

        /// <summary>
        /// 从串码表获取具体销售信息
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryInfo"></param>
        /// <param name="queryMainInfo"></param>
        /// <returns></returns>
        public object GetSaleList(Pagination pagination, daoben_product_sn queryInfo, daoben_activity_sales_perf queryMainInfo)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (queryMainInfo == null)
                return "信息错误，获取销售详情失败";
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;

            using (var db = SugarDao.GetInstance())
            {
                DateTime startTime = queryMainInfo.start_date.ToDate();
                DateTime endTime = queryMainInfo.end_date.ToDate().AddDays(1);

                var qable = db.Queryable<daoben_product_sn>();
                string fieldStr = ("b.emp_name, a.phone_sn, a.model, a.color, a.price_wholesale, a.price_sale,");
                if (queryMainInfo.target_content == 1) //实销
                {
                    if (queryMainInfo.emp_category == 20) //业务员
                        qable.JoinTable<daoben_activity_sales_perf_emp>((a, b) => a.sales_id == b.emp_id);
                    else if (queryMainInfo.emp_category == 21) //业务经理
                        qable.JoinTable<daoben_activity_sales_perf_emp>((a, b) => a.sales_m_id == b.emp_id);

                    qable.Where<daoben_activity_sales_perf_emp>((a, b) => b.main_id == queryMainInfo.id && a.sale_type > 0
                                                                && a.sale_time >= startTime && a.sale_time < endTime);
                    fieldStr += ("a.sale_time as time");
                }
                else if (queryMainInfo.target_content == 2) //下货
                {
                    if (queryMainInfo.emp_category == 20) //业务员
                        qable.JoinTable<daoben_activity_sales_perf_emp>((a, b) => a.out_sales_id == b.emp_id);
                    else if (queryMainInfo.emp_category == 21) //业务经理
                        qable.JoinTable<daoben_activity_sales_perf_emp>((a, b) => a.out_sales_m_id == b.emp_id);

                    qable.Where<daoben_activity_sales_perf_emp>((a, b) => b.main_id == queryMainInfo.id && a.sale_type > -1
                                                                && a.outstorage_time >= startTime && a.outstorage_time < endTime);
                    fieldStr += ("a.outstorage_time as time");
                }
                if (queryMainInfo.target_product == 2) //指定机型
                {
                    qable.JoinTable<daoben_activity_sales_perf_product>((a, c) => a.model == c.model && a.color == c.color)
                         .Where<daoben_activity_sales_perf_product>((a, c) => c.main_id == queryMainInfo.id);
                }

                if (queryInfo != null) //匹配查询参数
                {
                    if (!string.IsNullOrEmpty(queryInfo.phone_sn))
                        qable.Where(a => a.phone_sn.Contains(queryInfo.phone_sn));
                    if (!string.IsNullOrEmpty(queryInfo.model))
                        qable.Where(a => a.model.Contains(queryInfo.model));
                }
                if (!string.IsNullOrEmpty(queryMainInfo.name))
                    qable.Where<daoben_activity_sales_perf_emp>((a, b) => b.emp_name.Contains(queryMainInfo.name));

                var listStr = qable.Select(fieldStr).OrderBy(pagination.sidx + " " + pagination.sord)
                                   .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public string Add(daoben_activity_sales_perf addInfo, List<daoben_activity_sales_perf_emp> empList,
                List<daoben_activity_sales_perf_product> productList)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            if (addInfo == null)
                return "信息错误，操作失败!";
            if (addInfo.category < 1 || addInfo.category > 3)
                return "信息错误：考核类型不正确!";
            if (addInfo.emp_category < 20 || addInfo.emp_category > 21)
                return "信息错误：指定参与员工的类型不正确";
            //DateTime curMonth = DateTime.Now.Date.AddDays(0 - DateTime.Now.Day);

            //if (addInfo.start_date < curMonth)
            //    return "信息错误：考核时间不能早于本月!";
            if (addInfo.target_product == 2)
                if (productList == null || productList.Count() <= 0)
                    return "信息错误：指定机型为空";
            if (addInfo.target_product < 0 || addInfo.target_product > 2)
                return "信息错误：错误的机型参数target_product";
            addInfo.creator_position_name = myPositionInfo.name;
            addInfo.creator_job_history_id = LoginInfo.jobHistoryId;
            addInfo.creator_name = LoginInfo.empName;
            addInfo.create_time = DateTime.Now;
            addInfo.activity_status = -2;//
            addInfo.approve_status = 0;
            if (addInfo.target_product == 2)
            {
                if (productList == null || productList.Count() <= 0)
                    return "信息错误：没有指定机型";
            }
            if (empList == null || empList.Count() <= 0)
                return "信息错误：没有指定参与员工";
            addInfo.emp_count = empList.Count();
            addInfo.id = Common.GuId();
            foreach (var a in empList)
                a.main_id = addInfo.id;
            if (productList != null)
            {
                foreach (var a in productList)
                    a.main_id = addInfo.id;
            }
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    daoben_org_company companyInfo = db.Queryable<daoben_org_company>()
                            .Where(a => a.id == addInfo.company_id).Select("id,name,parent_id,link_name").SingleOrDefault();
                    if (companyInfo == null)
                        return "信息错误：指定的机构不存在";
                    addInfo.company_id = companyInfo.id;
                    addInfo.company_id_parent = companyInfo.parent_id;
                    addInfo.company_name = companyInfo.link_name;
                    db.Insert(addInfo);
                    if (empList != null && empList.Count() >= 25)
                        db.SqlBulkCopy(empList);
                    else if (empList != null && empList.Count() > 0)
                        db.InsertRange(empList);
                    if (productList != null && productList.Count() >= 25)
                        db.SqlBulkCopy(productList);
                    else if (productList != null && productList.Count() > 0)
                        db.InsertRange(productList);
                }
            }
            catch (Exception ex)
            {
                return "系统出错：" + ex.Message;
            }
            List<string> idList = new List<string>();
            ApproveTmp(addInfo.id);
            return "success";
        }
        public object GetInfo(string id)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (string.IsNullOrEmpty(id))
                return null;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            using (var db = SugarDao.GetInstance())
            {
                daoben_activity_sales_perf perfInfo = db.Queryable<daoben_activity_sales_perf>().SingleOrDefault(a => a.id == id);
                if (perfInfo == null)
                    return null;
                List<daoben_activity_sales_perf_emp> empList = db.Queryable<daoben_activity_sales_perf_emp>()
                            .Where(a => a.main_id == perfInfo.id).ToList();
                List<daoben_activity_sales_perf_product> productList = db.Queryable<daoben_activity_sales_perf_product>()
                            .Where(a => a.main_id == perfInfo.id).ToList();
                List<daoben_activity_sales_perf_approve> approveList = db.Queryable<daoben_activity_sales_perf_approve>()
                            .Where(a => a.main_id == id).ToList();

                object retObj = new
                {
                    perfInfo = perfInfo,
                    empList = empList,
                    productList = productList,
                    approveList = approveList,
                };
                return retObj;
            }
        }
        public string Alter(string id, DateTime alterDate)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (string.IsNullOrEmpty(id))
                return "信息错误，操作失败!";
            daoben_activity_sales_perf_alter alterInfo = new daoben_activity_sales_perf_alter
            {
                id = Common.GuId(),
                main_id = id,
                alter_end_date = alterDate,
                approve_status = 0,
                creator_position_name = LoginInfo.positionInfo.name,
                creator_job_history_id = LoginInfo.jobHistoryId,
                creator_name = LoginInfo.empName,
                create_time = DateTime.Now
            };
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    daoben_activity_sales_perf mainInfo = db.Queryable<daoben_activity_sales_perf>().SingleOrDefault(a => a.id == id);
                    if (mainInfo == null)
                        return "信息错误：考核信息不存在!";
                    if (mainInfo.activity_status == 2)
                        return "操作失败：考核已结束!";
                    if (mainInfo.activity_status == -2)
                        return "操作失败：考核未审批通过!";
                    if (alterDate < mainInfo.start_date)
                        return "信息错误：结束时间不能小于开始时间!";
                    alterInfo.orig_end_date = mainInfo.end_date;
                    db.Insert(alterInfo);

                    #region TODO 审批，这里暂时直接通过
                    DateTime now = DateTime.Now;
                    int activity_status = mainInfo.activity_status;
                    if (now.Date > alterDate)
                        activity_status = 2;
                    object upObj = new
                    {
                        end_date = alterDate,
                        activity_status = activity_status
                    };
                    db.Update<daoben_activity_sales_perf>(upObj, a => a.id == id);
                    db.Update<daoben_activity_sales_perf_alter>(new { approve_status = 100 }, a => a.id == alterInfo.id);
                    if (activity_status < 0)
                        return "success";

                    List<daoben_activity_sales_perf> mainInfoList = new List<daoben_activity_sales_perf>();
                    mainInfoList.Add(mainInfo);
                    Statistics(db, mainInfoList, true);
                    #endregion
                    return "success";
                }
            }
            catch (Exception ex)
            {
                return "系统出错：" + ex.Message;
            }
        }

        public string Recall(string id)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (string.IsNullOrEmpty(id))
                return "信息错误，操作失败!";
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    daoben_activity_sales_perf recallInfo = db.Queryable<daoben_activity_sales_perf>().InSingle(id);
                    if (recallInfo == null)
                        return "信息错误：考核信息不存在!";
                    if (recallInfo.creator_job_history_id != LoginInfo.jobHistoryId)
                        return "操作失败：考核信息只能由发起人撤回!";
                    if (recallInfo.approve_status != 0)
                        return "操作失败：考核信息已审批!";

                    db.Delete<daoben_activity_sales_perf>(a => a.id == id);
                    //删除所有待办事项
                    db.Delete<daoben_sys_task>(a => a.main_id == id.ToString());
                    db.Delete<daoben_sys_notification>(a => a.main_id == id.ToString());
                    return "success";
                }
            }
            catch (Exception ex)
            {
                return "系统出错：" + ex.Message;
            }
        }
        /// <summary>
        /// //0分公司总经理1——1事业部总经理/助手2——2财务经理100
        /// </summary>
        /// <param name="approveInfo"></param>
        /// <returns></returns>
        public string Approve(daoben_activity_sales_perf_approve approveInfo)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            if (approveInfo == null || string.IsNullOrEmpty(approveInfo.main_id))
                return "信息错误，操作失败!";
            approveInfo.approve_id = LoginInfo.accountId;
            approveInfo.approve_name = LoginInfo.empName;
            approveInfo.approve_position_id = myPositionInfo.id;
            approveInfo.approve_position_name = myPositionInfo.name;
            approveInfo.approve_time = DateTime.Now;

            object upObj = null;
            SqlSugarClient db = null;
            try
            {
                using (db = SugarDao.GetInstance())
                {
                    daoben_activity_sales_perf mainInfo = db.Queryable<daoben_activity_sales_perf>().InSingle(approveInfo.main_id);
                    if (mainInfo == null)
                        return "操作失败：考核信息不存在";
                    if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER && approveInfo.status > 0)
                    {
                        approveInfo.status = 100; //以100作为审批完成的标志
                        if (mainInfo.start_date <= DateTime.Now)
                            upObj = new { approve_status = approveInfo.status, activity_status = 1 };
                        else
                            upObj = new { approve_status = approveInfo.status, activity_status = -1 };
                    }
                    else
                    {
                        if (approveInfo.status > 0)
                            approveInfo.status = mainInfo.approve_status + 1;
                        else
                            approveInfo.status = 0 - 1 - mainInfo.approve_status;
                        upObj = new { approve_status = approveInfo.status };
                    }
                    //清除之前待办事项
                    List<daoben_sys_task> origTask = db.Queryable<daoben_sys_task>().Where(a => a.main_id == approveInfo.main_id.ToString()).ToList();
                    object upObjForTask = new
                    {
                        status = 3,
                        finished_time = now
                    };
                    if (origTask.Count() > 0)
                        db.Update<daoben_sys_task>(upObjForTask, a => a.main_id == approveInfo.main_id.ToString());
                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    db.DisableInsertColumns = new string[] { "id" };
                    db.Insert(approveInfo);
                    db.Update<daoben_activity_sales_perf>(upObj, a => a.id == approveInfo.main_id);
                    db.CommitTran();

                    #region 待办事项
                    //List<daoben_sys_notification> newsTotal = new List<daoben_sys_notification>();//消息通知
                    //List<daoben_sys_task> taskTotal = new List<daoben_sys_task>();//待办事项
                    //string newsStr = null;
                    //List<string> newsIdList = null;
                    //string taskStr = null;
                    //List<string> taskIdList = null;
                    ////0分公司总经理1——1事业部总经理/助手2——2财务经理100
                    //daoben_activity_sales_perf origInfo = db.Queryable<daoben_activity_sales_perf>().InSingle(approveInfo.main_id);
                    //if (origInfo.approve_status == 1)
                    //    taskIdList = db.Queryable<daoben_hr_emp_job>()
                    //        .Where(a => a.position_type == ConstData.POSITION_GM1 && a.company_id == myCompanyInfo.parentId)
                    //        .Select<string>("id").ToList();
                    //else if (origInfo.approve_status == 2)
                    //    taskIdList = db.Queryable<daoben_hr_emp_job>()
                    //                .JoinTable<daoben_ms_account>((a, b) => a.id == b.employee_id)
                    //                .Where<daoben_ms_account>((a, b) => b.role_id == ConstData.ROLE_ID_FINANCIALMANAGER)
                    //                .Where(a => a.company_id == myCompanyInfo.id)
                    //                .Select<string>("a.id as id").ToList();
                    //else if (origInfo.approve_status != 0)
                    //    newsIdList = db.Queryable<daoben_hr_emp_job_history>()
                    //    .Where(a => a.id == origInfo.creator_job_history_id).Select<string>("emp_id as id").ToList();

                    //taskStr = "业务考核待审批";
                    ////待办事项 生成列表
                    //if (taskIdList != null && taskIdList.Count > 0)
                    //{
                    //    taskTotal = taskIdList
                    //        .Select(a1 => new daoben_sys_task
                    //        {
                    //            emp_id = a1,
                    //            category = 1,
                    //            main_id = origInfo.id.ToString(),
                    //            main_url = "/ActivityManage/SalesPerformance/Approve?id=" + origInfo.id,
                    //            title = taskStr,
                    //            content_abstract = origInfo.emp_name,
                    //            recipient_type = 1,
                    //            create_time = now,
                    //            status = 1
                    //        }).ToList();
                    //}
                    //if (newsIdList != null && newsIdList.Count > 0)
                    //{
                    //    if (origInfo.approve_status < 0)
                    //        newsStr = "业务考核审批没有通过";
                    //    else if (origInfo.approve_status == 100)
                    //        newsStr = "业务考核审批已通过";
                    //    //消息通知 生成列表
                    //    newsTotal = newsIdList
                    //            .Select(a1 => new daoben_sys_notification
                    //            {
                    //                emp_id = a1,
                    //                category = 2,
                    //                main_id = origInfo.id.ToString(),
                    //                main_url = "/ActivityManage/SalesPerformance/Show?id=" + origInfo.id,
                    //                title = newsStr,
                    //                content_abstract = origInfo.emp_name,
                    //                recipient_type = 1,
                    //                create_time = now,
                    //                status = 1
                    //            }).ToList();
                    //}
                    ////待办事项 and 消息通知 列表插入
                    //if (newsTotal != null && newsTotal.Count > 25)
                    //    db.SqlBulkCopy(newsTotal);
                    //else if (newsTotal != null && newsTotal.Count > 0)
                    //    db.InsertRange(newsTotal);
                    //if (taskTotal != null && taskTotal.Count > 25)
                    //    db.SqlBulkCopy(taskTotal);
                    //else if (taskTotal != null && taskTotal.Count > 0)
                    //    db.InsertRange(taskTotal);
                    #endregion 待办事项


                }
            }
            catch (Exception ex)
            {
                if (db != null)
                    db.RollbackTran();
                return "系统出错：" + ex.Message;
            }
            //Statistics(approveInfo.main_id);
            return "success";

        }

        public void ApproveTmp(string id)
        {
            using (var db = SugarDao.GetInstance())
            {
                daoben_activity_sales_perf origInfo = db.Queryable<daoben_activity_sales_perf>().SingleOrDefault(a => a.id == id);
                if (origInfo == null)
                    return;
                DateTime now = DateTime.Now;
                int activity_status = -1;
                if (now.Date > origInfo.end_date)
                    activity_status = 2;
                else if (now >= origInfo.start_date)
                    activity_status = 1;

                object upObj = new
                {
                    approve_status = 100,
                    activity_status = activity_status
                };
                db.Update<daoben_activity_sales_perf>(upObj, a => a.id == id);
                if (activity_status < 0)
                    return;
                List<daoben_activity_sales_perf> mainInfoList = new List<daoben_activity_sales_perf>();
                mainInfoList.Add(origInfo);
                Statistics(db, mainInfoList, true);
            }
        }

        public void Statistics(SqlSugarClient db, List<daoben_activity_sales_perf> mainInfoList, bool guidePerf = false)
        {
            if (mainInfoList == null || mainInfoList.Count < 1)
                return;
            //活动管理 业务考核
            DateTime now = DateTime.Now;
            StringBuilder sqlUpSb1 = new StringBuilder("insert into daoben_activity_sales_perf_emp (`id`, `total_count`, `total_amount`, `total_ratio`, `total_reward`) values ");
            StringBuilder sqlUpSb2 = new StringBuilder("insert into daoben_activity_sales_perf_product (`id`, `total_count`, `total_amount`) values ");
            StringBuilder sqlUpSb3 = new StringBuilder("insert into daoben_activity_sales_perf (`id`, `total_sale_count`, `total_sale_amount`, `total_reward`, `counting_time`) values ");

            bool toUp2 = false;
            List<daoben_activity_sales_perf_emp> empList = null;
            List<daoben_activity_sales_perf_product> productList = null;
            foreach (var mainInfo in mainInfoList)
            {
                if (mainInfo.category == 3)
                {
                    if (!guidePerf)
                        continue;
                    #region 导购人数考核 (TODO: 需重写 - JIANG 2019-02-20)
                    empList = db.Queryable<daoben_activity_sales_perf_emp>()
                            .Where(a => a.main_id == mainInfo.id).ToList();
                    int distriCount, guideCount;
                    if (empList != null && empList.Count > 0)
                    {
                        foreach (var emp in empList)
                        {
                            emp.total_count = 0;
                            emp.total_reward = 0;
                            emp.total_ratio = 0;
                            emp.total_amount = 0;
                            distriCount = 0;
                            guideCount = 0;
                            if (mainInfo.emp_category == 20)
                            {
                                //经销商数
                                distriCount = db.Queryable<daoben_distributor_info>()
                                    .Where(a => a.area_l2_id == emp.area_l2_id && a.inactive == false).ToList().Count();
                                //导购员人数
                                guideCount = db.Queryable<daoben_hr_emp_job>()
                                    .Where(a => a.area_l2_id == emp.area_l2_id && a.position_type == ConstData.POSITION_GUIDE)
                                    .Count();
                                emp.total_count = guideCount;
                                emp.total_amount = distriCount;
                            }
                            else
                            {
                                //经销商数
                                distriCount = db.Queryable<daoben_distributor_info>()
                                    .Where(a => a.area_l1_id == emp.area_l1_id && a.inactive == false).ToList().Count();
                                //导购员人数
                                guideCount = db.Queryable<daoben_hr_emp_job>()
                                    .Where(a => a.area_l1_id == emp.area_l1_id && a.position_type == ConstData.POSITION_GUIDE
                                    )
                                    .Count();
                                emp.total_count = guideCount;
                                emp.total_amount = distriCount;
                            }
                            //比例
                            emp.total_ratio = (guideCount == 0 || distriCount == 0) ? 0 : (decimal)(guideCount * 100) / distriCount;
                            if (mainInfo.target_mode == 1)
                            {
                                //按比例                                    
                                if (emp.total_ratio >= mainInfo.activity_target)
                                    emp.total_reward = emp.total_ratio * mainInfo.reward / 100;
                                else
                                {
                                    if (emp.total_ratio > 0)
                                        emp.total_reward = ((emp.total_ratio - mainInfo.activity_target) * mainInfo.penalty) / 100;
                                    else
                                        emp.total_reward = -mainInfo.penalty;
                                }
                            }
                            else if (mainInfo.target_mode == 2)
                            {
                                //按人数
                                if (emp.total_ratio >= mainInfo.activity_target)
                                    emp.total_reward = emp.total_count * mainInfo.reward;
                                else
                                {
                                    int temp = (int)(emp.total_amount * mainInfo.activity_target / 100);
                                    if ((emp.total_amount * mainInfo.activity_target / 100) > temp)
                                        temp += 1;
                                    emp.total_reward = (emp.total_count - temp) * mainInfo.penalty;
                                }
                            }
                        };
                    }
                    #endregion
                }
                int total_sale_count = 0;
                decimal total_sale_amount = 0, total_reward = 0;
                DateTime startDate = mainInfo.start_date.ToDate();
                DateTime endDate = mainInfo.end_date > now ? now : mainInfo.end_date.ToDate().AddDays(1);

                #region 月度考核/销量考核
                #region 员工销量
                List<daoben_activity_sales_perf_emp> fullEmpList = db.Queryable<daoben_activity_sales_perf_emp>()
                        .Where(e => e.main_id == mainInfo.id).Select("id, emp_id").ToList();
                if (fullEmpList == null || fullEmpList.Count < 1)
                    continue;
                var qable = db.Queryable<daoben_activity_sales_perf_emp>();
                if (mainInfo.target_content == 1)       // 按实销
                {
                    if (mainInfo.emp_category == 20)    // 业务员
                    {
                        qable.JoinTable<daoben_product_sn>((e, s) => s.sales_id == e.emp_id && s.sale_type > 0
                                && s.sale_time >= startDate && s.sale_time < endDate);
                    }
                    else                                // 业务经理
                    {
                        qable.JoinTable<daoben_product_sn>((e, s) => s.sales_m_id == e.emp_id && s.sale_type > 0
                                && s.sale_time >= startDate && s.sale_time < endDate);
                    }
                }
                else                                    // 按下货
                {
                    if (mainInfo.emp_category == 20)    // 业务员
                    {
                        qable.JoinTable<daoben_product_sn>((e, s) => s.out_sales_id == e.emp_id && s.sale_type > -1
                                && s.outstorage_time >= startDate && s.outstorage_time < endDate);
                    }
                    else                                // 业务经理
                    {
                        qable.JoinTable<daoben_product_sn>((e, s) => s.out_sales_m_id == e.emp_id && s.sale_type > -1
                                && s.outstorage_time >= startDate && s.outstorage_time < endDate);
                    }
                }
                if (mainInfo.target_product == 2)       // 指定机型
                {
                    qable.JoinTable<daoben_product_sn, daoben_activity_sales_perf_product>((e, s, p) => p.model == s.model && p.color == s.color)
                            .Where<daoben_activity_sales_perf_product>((e, p) => p.main_id == mainInfo.id);
                }
                empList = qable.Where(e => e.main_id == mainInfo.id).GroupBy("e.emp_id")
                        .Select<daoben_activity_sales_perf_emp>("e.id, e.emp_id, count(s.id) as total_count, sum(s.price_sale) as total_amount")
                        .ToList();

                var empResList = empList.Union(fullEmpList, new EmpCompare()).ToList(); // 合并LIST并保留empList的结果
                if (empResList != null && empResList.Count > 0) // empList只包含有销量的员工 
                {
                    foreach (daoben_activity_sales_perf_emp emp in empResList)
                    {
                        if (emp.total_count == 0)
                        {
                            emp.total_ratio = 0;
                            emp.total_reward = 0;
                            emp.total_reward = 0 - mainInfo.penalty;
                            total_reward += emp.total_reward;
                        }
                        else
                        {
                            if (mainInfo.target_mode == 1) //完成率
                            {
                                if (mainInfo.activity_target == 0)
                                    emp.total_ratio = 100;
                                else
                                    emp.total_ratio = (emp.total_count * 100) / (mainInfo.activity_target);

                                if (emp.total_count >= mainInfo.activity_target)
                                    emp.total_reward = emp.total_ratio * mainInfo.reward / 100;
                                else
                                {
                                    if (emp.total_ratio > 0)
                                        emp.total_reward = (emp.total_ratio - 100) * mainInfo.penalty / 100;
                                    else
                                        emp.total_reward = 0 - mainInfo.penalty;
                                }
                            }
                            else //销量
                            {
                                if (emp.total_count >= mainInfo.activity_target)
                                    emp.total_reward = emp.total_count * mainInfo.reward;
                                else
                                    emp.total_reward = (emp.total_count - mainInfo.activity_target) * mainInfo.penalty;
                            }
                            total_sale_count += emp.total_count;
                            total_sale_amount += emp.total_amount;
                            total_reward += emp.total_reward;
                        }
                        sqlUpSb1.AppendFormat("({0},{1},{2},{3},{4}),", emp.id, emp.total_count, emp.total_amount, emp.total_ratio, emp.total_reward);
                    }
                }
                #endregion
                #region 机型销量
                if (mainInfo.target_product == 2)      // 指定机型
                {
                    var qableP = db.Queryable<daoben_activity_sales_perf_product>();
                    if (mainInfo.target_content == 1)  // 按实销
                    {
                        qableP.JoinTable<daoben_product_sn>((p, s) => p.model == s.model && p.color == s.color && s.sale_type > 0
                                && s.sale_time >= startDate && s.sale_time < endDate);
                        if (mainInfo.emp_category == 20)    // 业务员
                        {
                            qableP.JoinTable<daoben_product_sn, daoben_activity_sales_perf_emp>((p, s, e) => e.main_id == mainInfo.id
                                    && s.sales_id == e.emp_id && s.sale_type > 0);
                        }
                        else                                // 业务经理
                        {
                            qableP.JoinTable<daoben_product_sn, daoben_activity_sales_perf_emp>((p, s, e) => e.main_id == mainInfo.id
                                    && s.sales_m_id == e.emp_id && s.sale_type > 0);
                        }
                    }
                    else                                // 按下货
                    {
                        qableP.JoinTable<daoben_product_sn>((p, s) => p.model == s.model && p.color == s.color && s.sale_type > -1
                                && s.outstorage_time >= startDate && s.outstorage_time < endDate);
                        if (mainInfo.emp_category == 20)    // 业务员
                        {
                            qableP.JoinTable<daoben_product_sn, daoben_activity_sales_perf_emp>((p, s, e) => e.main_id == mainInfo.id
                                    && s.out_sales_id == e.emp_id && s.sale_type > -1);
                        }
                        else                                // 业务经理
                        {
                            qableP.JoinTable<daoben_product_sn, daoben_activity_sales_perf_emp>((p, s, e) => e.main_id == mainInfo.id
                                    && s.out_sales_m_id == e.emp_id && s.sale_type > -1);
                        }
                    }
                    productList = qableP.Where<daoben_activity_sales_perf_emp>((p,e) => p.main_id == mainInfo.id
                            && e.main_id == mainInfo.id).GroupBy("p.id")
                            .Select<daoben_activity_sales_perf_product>("p.id, p.model, p.color, count(s.id) as total_count, sum(s.price_sale) as total_amount")
                            .ToList();
                    if (productList != null && productList.Count > 0)
                    {
                        foreach (var a in productList)
                            sqlUpSb2.AppendFormat("({0},{1},{2}),", a.id, a.total_count, a.total_amount);
                        toUp2 = true;
                    }
                }
                #endregion
                #endregion
                sqlUpSb3.AppendFormat("('{0}',{1},{2},{3},'{4}'),", mainInfo.id, total_sale_count, total_sale_amount, total_reward, endDate.AddDays(-1));
            }


            sqlUpSb1.Remove(sqlUpSb1.Length - 1, 1); // 最后一个逗号
            sqlUpSb1.Append("on duplicate key update total_count=values(total_count),total_amount=values(total_amount),total_ratio=values(total_ratio),total_reward=values(total_reward);");
            db.SqlQuery<int>(sqlUpSb1.ToString());
            if (toUp2)
            {
                sqlUpSb2.Remove(sqlUpSb2.Length - 1, 1); // 最后一个逗号
                sqlUpSb2.Append("on duplicate key update total_count=values(total_count),total_amount=values(total_amount);");
                db.SqlQuery<int>(sqlUpSb2.ToString());
            }
            sqlUpSb3.Remove(sqlUpSb3.Length - 1, 1); // 最后一个逗号
            sqlUpSb3.Append("on duplicate key update total_sale_count=values(total_sale_count),total_sale_amount=values(total_sale_amount),total_reward=values(total_reward),counting_time=values(counting_time);");
            db.SqlQuery<int>(sqlUpSb3.ToString());
        }
        public class EmpCompare : IEqualityComparer<daoben_activity_sales_perf_emp>
        {
            public bool Equals(daoben_activity_sales_perf_emp x, daoben_activity_sales_perf_emp y)
            {
                return x.id == y.id;
            }
            public int GetHashCode(daoben_activity_sales_perf_emp obj)
            {
                return obj.id.GetHashCode();
            }
        }
#if false
        public void Statistics(string id)
        {
            List<daoben_activity_sales_perf_emp> empList = null;
            List<daoben_activity_sales_perf_product> productList = null;
            object upObj = null;
            DateTime now = DateTime.Now;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_activity_sales_perf mainInfo = db.Queryable<daoben_activity_sales_perf>()
                        .SingleOrDefault(a => a.id == id);
                    if (mainInfo == null)
                        return;

                    if (mainInfo.category == 1 || mainInfo.category == 2)
                    {
        #region 月度考核/销量考核
        #region 员工销量
                        DateTime startDate = mainInfo.start_date.ToDate();
                        DateTime endDate = mainInfo.end_date > now ? now : mainInfo.end_date.ToDate().AddDays(1);

                        var qable = db.Queryable<daoben_product_sn>();
                        if (mainInfo.target_product == 2)       // 指定机型
                            qable.JoinTable<daoben_activity_sales_perf_product>((a, b) => a.model == b.model && a.color == b.color);
                        if (mainInfo.target_content == 1)       // 按实销
                        {
                            if (mainInfo.emp_category == 20)    // 业务员
                                qable.JoinTable<daoben_activity_sales_perf_emp>((a, c) => a.sales_id == c.emp_id);
                            else                                    // 业务经理
                                qable.JoinTable<daoben_activity_sales_perf_emp>((a, c) => a.sales_m_id == c.emp_id);
                            qable.Where(a => a.sale_time >= startDate && a.sale_time < endDate);
                        }
                        else                                        // 按下货
                        {
                            if (mainInfo.emp_category == 20)    // 业务员
                                qable.JoinTable<daoben_activity_sales_perf_emp>((a, c) => a.out_sales_id == c.emp_id);
                            else                                    // 业务经理
                                qable.JoinTable<daoben_activity_sales_perf_emp>((a, c) => a.out_sales_m_id == c.emp_id);
                            qable.Where(a => a.outstorage_time >= startDate && a.outstorage_time < endDate);
                        }
                        empList = qable.Where<daoben_activity_sales_perf_emp>((a, c) => c.main_id == mainInfo.id && a.sale_type > -1)
                                        .GroupBy("c.emp_id")
                                        .Select<daoben_activity_sales_perf_emp>("c.id, c.emp_id, count(*) as total_count, sum(a.price_sale) as total_amount").ToList();
                        if (empList != null && empList.Count > 0) // resList只包含有销量的员工 
                        {
                            int total_sale_count = 0;
                            decimal total_sale_amount = 0, total_reward = 0;

                            foreach (daoben_activity_sales_perf_emp emp in empList)
                            {
                                if (mainInfo.target_mode == 1) //完成率
                                {
                                    if (mainInfo.activity_target == 0)
                                        emp.total_ratio = 100;
                                    else
                                        emp.total_ratio = (emp.total_count * 100) / (mainInfo.activity_target);

                                    if (emp.total_count >= mainInfo.activity_target)
                                        emp.total_reward = emp.total_ratio * mainInfo.reward / 100;
                                    else
                                    {
                                        if (emp.total_ratio > 0)
                                            emp.total_reward = 1 - (emp.total_ratio * mainInfo.penalty) / 100;
                                        else
                                            emp.total_reward = 1 - mainInfo.penalty;
                                    }
                                }
                                else //销量
                                {
                                    if (emp.total_count >= mainInfo.activity_target)
                                        emp.total_reward = emp.total_count * mainInfo.reward;
                                    else
                                        emp.total_reward = 1 - (emp.total_count - mainInfo.activity_target) * mainInfo.penalty;
                                }
                                total_sale_count += emp.total_count;
                                total_sale_amount += emp.total_amount;
                                total_reward = emp.total_reward;
                            }
                            upObj = new
                            {
                                total_sale_count = total_sale_count,
                                total_sale_amount = total_sale_amount,
                                total_reward = total_reward,
                                counting_time = endDate,
                            };
                        }
        #endregion
        #region 机型销量
                        if (mainInfo.target_product == 2)       // 指定机型
                        {
                            var qableP = db.Queryable<daoben_product_sn>()
                                    .JoinTable<daoben_activity_sales_perf_product>((a, b) => a.model == b.model && a.color == b.color);
                            if (mainInfo.target_content == 1)      // 按实销
                                qableP.Where(a => a.sale_time >= startDate && a.sale_time < endDate);
                            else                                       // 按下货
                                qableP.Where(a => a.outstorage_time >= startDate && a.outstorage_time < endDate);
                            productList = qableP.Where<daoben_activity_sales_perf_product>((a, b) => b.main_id == mainInfo.id
                                    && a.sale_type > -1).GroupBy("b.id")
                                    .Select<daoben_activity_sales_perf_product>("b.id, a.model, a.color, count(*) as total_count, sum(price_sale) as total_amount")
                                    .ToList();
                        }
        #endregion
        #endregion
                    }
                    else
                    {
        #region 导购人数考核 (TODO: 需重写 - JIANG 2019-02-20)
                        empList = db.Queryable<daoben_activity_sales_perf_emp>()
                                .Where(a => a.main_id == id).ToList();
                        int distriCount, guideCount;
                        if (empList != null && empList.Count > 0)
                        {
                            foreach (var emp in empList)
                            {
                                emp.total_count = 0;
                                emp.total_reward = 0;
                                emp.total_ratio = 0;
                                emp.total_amount = 0;
                                distriCount = 0;
                                guideCount = 0;
                                if (mainInfo.emp_category == 20)
                                {
                                    //经销商数
                                    distriCount = db.Queryable<daoben_distributor_info>()
                                        .Where(a => a.area_l2_id == emp.area_l2_id && a.inactive == false).ToList().Count();
                                    //导购员人数
                                    guideCount = db.Queryable<daoben_hr_emp_job>()
                                        .Where(a => a.area_l2_id == emp.area_l2_id && a.position_type == ConstData.POSITION_GUIDE)
                                        .Count();
                                    emp.total_count = guideCount;
                                    emp.total_amount = distriCount;
                                }
                                else
                                {
                                    //经销商数
                                    distriCount = db.Queryable<daoben_distributor_info>()
                                        .Where(a => a.area_l1_id == emp.area_l1_id && a.inactive == false).ToList().Count();
                                    //导购员人数
                                    guideCount = db.Queryable<daoben_hr_emp_job>()
                                        .Where(a => a.area_l1_id == emp.area_l1_id && a.position_type == ConstData.POSITION_GUIDE
                                        )
                                        .Count();
                                    emp.total_count = guideCount;
                                    emp.total_amount = distriCount;
                                }
                                //比例
                                emp.total_ratio = (guideCount == 0 || distriCount == 0) ? 0 : (decimal)(guideCount * 100) / distriCount;
                                if (mainInfo.target_mode == 1)
                                {
                                    //按比例                                    
                                    if (emp.total_ratio >= mainInfo.activity_target)
                                        emp.total_reward = emp.total_ratio * mainInfo.reward / 100;
                                    else
                                    {
                                        if (emp.total_ratio > 0)
                                            emp.total_reward = ((emp.total_ratio - mainInfo.activity_target) * mainInfo.penalty) / 100;
                                        else
                                            emp.total_reward = -mainInfo.penalty;
                                    }
                                }
                                else if (mainInfo.target_mode == 2)
                                {
                                    //按人数
                                    if (emp.total_ratio >= mainInfo.activity_target)
                                        emp.total_reward = emp.total_count * mainInfo.reward;
                                    else
                                    {
                                        int temp = (int)(emp.total_amount * mainInfo.activity_target / 100);
                                        if ((emp.total_amount * mainInfo.activity_target / 100) > temp)
                                            temp += 1;
                                        emp.total_reward = (emp.total_count - temp) * mainInfo.penalty;
                                    }
                                }
                            };
                        }
        #endregion
                    }
                    //保存计算后的数据-员工
                    StringBuilder sqlUpSb1 = new StringBuilder("insert into daoben_activity_sales_perf_emp (`id`, `total_count`, `total_amount`, `total_ratio`, `total_reward`) values ");
                    if (empList != null && empList.Count > 0)
                    {
                        foreach (var a in empList)
                        {
                            sqlUpSb1.AppendFormat("({0},{1},{2},{3},{4}),", a.id, a.total_count, a.total_amount, a.total_ratio, a.total_reward);
                        }
                        sqlUpSb1.Remove(sqlUpSb1.Length - 1, 1); // 最后一个逗号
                        sqlUpSb1.Append("on duplicate key update total_count=values(total_count),total_amount=values(total_amount),total_ratio=values(total_ratio),total_reward=values(total_reward);");
                    }
                    //保存计算后的数据-机型
                    StringBuilder sqlUpSb2 = new StringBuilder("insert into daoben_activity_sales_perf_product (`id`, `total_count`, `total_amount`) values ");
                    if (productList != null && productList.Count > 0)
                    {
                        foreach (var a in productList)
                        {
                            sqlUpSb2.AppendFormat("({0},{1},{2}),", a.id, a.total_count, a.total_amount);
                        }
                        sqlUpSb2.Remove(sqlUpSb2.Length - 1, 1); // 最后一个逗号
                        sqlUpSb2.Append("on duplicate key update total_count=values(total_count),total_amount=values(total_amount);");
                    }
                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    if (empList != null && empList.Count > 0)
                        db.SqlQuery<int>(sqlUpSb1.ToString());
                    if (productList != null && productList.Count > 0)
                        db.SqlQuery<int>(sqlUpSb2.ToString());
                    if (upObj != null)
                        db.Update<daoben_activity_sales_perf>(upObj, a => a.id == id);
                    db.CommitTran();
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    ExceptionApp.WriteLog("SalesPerformanceApp(Statistics)：" + ex.Message);
                }
            }
        }
#endif
    }
}
