using Base.Code;
using MySqlSugar;
using ProjectShare.Database;
using ProjectShare.Models;
using ProjectShare.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectWeb.Areas.ActivityManage.Application
{
    public class PKApp
    {
        public object GetList(Pagination pagination, daoben_activity_pk queryInfo, QueryTime queryTime)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;

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
                    var qable = db.Queryable<daoben_activity_pk>();
                    if (myCompanyInfo.category == "事业部")
                        qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                    else if (myCompanyInfo.category == "分公司")
                        qable.Where(a => a.company_id == myCompanyInfo.id);

                    if (queryInfo != null)
                    {
                        if (!string.IsNullOrEmpty(queryInfo.name))
                            qable.Where(a => a.name.Contains(queryInfo.name));
                        if (queryInfo.company_id != 0)
                            qable.Where(a => a.company_id == queryInfo.company_id);
                        if (queryInfo.activity_status != 0)
                            qable.Where(a => a.activity_status == queryInfo.activity_status);
                        if (queryInfo.emp_category > 0)
                            qable.Where(a => a.emp_category == queryInfo.emp_category);
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

                        if (queryTime.startTime1 != null)
                        {
                            DateTime nextMonth = queryTime.startTime1.ToDate().AddMonths(1);
                            qable.Where(a => a.start_date >= queryTime.startTime1 && a.start_date < nextMonth);
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
            catch (Exception ex)
            {
                return "系统出错：" + ex.Message;
            }
        }
        public object GetListApprove(Pagination pagination, daoben_activity_pk queryInfo)
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
                    var qable = db.Queryable<daoben_activity_pk>();

                    if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)    // 跳过超级管理员，避免判断公司/职位信息是否存在
                    {
                        if (myPositionInfo.positionType == ConstData.POSITION_GM2)
                        {   // 分公司总经理
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
                        if (!string.IsNullOrEmpty(queryInfo.name))
                            qable.Where(a => a.name.Contains(queryInfo.name));
                        if (queryInfo.company_id > 0)
                            qable.Where(a => a.company_id == queryInfo.company_id);
                        if (queryInfo.emp_category > 0)
                            qable.Where(a => a.emp_category == queryInfo.emp_category);
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
        /// 从串码表获取销售详情 pk -实销
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryInfo"></param>
        /// <param name="queryMainInfo"></param>
        /// <returns></returns>
        public object GetSaleList(Pagination pagination, daoben_product_sn queryInfo, daoben_activity_pk queryMainInfo)
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
                if (queryMainInfo.emp_category == 3) //导购员
                {
                    qable.JoinTable<daoben_activity_pk_emp>((a, b) => a.reporter_id == b.emp_id)
                         .Where(a => a.reporter_type == 1);
                }
                else if (queryMainInfo.emp_category == 20) //业务员
                    qable.JoinTable<daoben_activity_pk_emp>((a, b) => a.sales_id == b.emp_id);
                else if (queryMainInfo.emp_category == 21) //业务经理
                    qable.JoinTable<daoben_activity_pk_emp>((a, b) => a.sales_m_id == b.emp_id);

                qable.Where<daoben_activity_pk_emp>((a, b) => b.main_id == queryMainInfo.id && a.sale_type > 0
                                                            && a.sale_time >= startTime && a.sale_time < endTime);
                string fieldStr = ("b.emp_name as name, a.phone_sn, a.model, a.color, a.price_wholesale, a.price_retail, a.sale_time as time");

                if (queryMainInfo.product_scope == 2 || queryMainInfo.product_scope == 3) //指定机型
                {
                    qable.JoinTable<daoben_activity_pk_product>((a, c) => a.model == c.model && a.color == c.color)
                         .Where<daoben_activity_pk_product>((a, c) => c.main_id == queryMainInfo.id);
                }

                if (queryInfo != null) //匹配查询参数
                {
                    if (!string.IsNullOrEmpty(queryInfo.phone_sn))
                        qable.Where(a => a.phone_sn.Contains(queryInfo.phone_sn));
                    if (!string.IsNullOrEmpty(queryInfo.model))
                        qable.Where(a => a.model.Contains(queryInfo.model));
                }
                if (!string.IsNullOrEmpty(queryMainInfo.name))
                    qable.Where<daoben_activity_pk_emp>((a, b) => b.emp_name.Contains(queryMainInfo.name));

                var listStr = qable.Select(fieldStr)
                                   .OrderBy(pagination.sidx + " " + pagination.sord)
                                   .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }

        }
        public string Add(daoben_activity_pk addInfo, List<daoben_activity_pk_product> productList,
                            List<daoben_activity_pk_emp> empList)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            if (addInfo == null)
                return "信息错误，操作失败!";
            if (addInfo.company_id < 1)
                return "信息错误：需指定分公司!";
            if (string.IsNullOrEmpty(addInfo.name))
                return "信息错误：活动名称不能为空!";
            if (empList == null || empList.Count < 2)
                return "信息错误：参与人数不能小于2!";
            if (addInfo.emp_category < 1 && addInfo.emp_category > 3)
                return "信息错误：参与对象类型不正确!";
            if (addInfo.start_date == null || addInfo.end_date == null)
                return "信息错误：起止时间不能为空!";
            //DateTime curMonth = DateTime.Now.Date.AddDays(0 - DateTime.Now.Day);
            //if (addInfo.start_date < curMonth)
            //    return "信息错误：时间不能早于本月!";
            addInfo.id = Common.GuId();
            if (addInfo.product_scope != 1)
            {
                foreach (var a in productList)
                {
                    a.main_id = addInfo.id;
                }
            }
            foreach (var a in empList)
            {
                a.main_id = addInfo.id;
            }
            if (string.IsNullOrEmpty(addInfo.id) || addInfo.id.Length != 36)
                return "信息错误：ID不正确!";
            if ((addInfo.product_scope == 2 || addInfo.product_scope == 3) && (productList == null || productList.Count < 1))
                return "信息错误：指定机型时机型列表不能为空!";

            addInfo.creator_job_history_id = LoginInfo.empType == 0 ? LoginInfo.jobHistoryId : LoginInfo.empId;
            addInfo.creator_name = LoginInfo.empName;
            addInfo.create_time = DateTime.Now;
            addInfo.activity_status = -2;
            addInfo.approve_status = 0;
            addInfo.pk_group_count = empList.Count / 2;
            addInfo.counting_time = null;
            SqlSugarClient db = null;
            try
            {
                using (db = SugarDao.GetInstance())
                {
                    daoben_org_company companyInfo = db.Queryable<daoben_org_company>().InSingle(addInfo.company_id);
                    if (companyInfo == null)
                        return "信息错误：指定分公司不存在";
                    addInfo.company_name = companyInfo.link_name;
                    addInfo.company_id_parent = companyInfo.parent_id;

                    #region 待办事项 分公司总经理
                    ////待办事项 收件人
                    //string tempStr = "PK活动待审批";
                    //List<string> idList = db.Queryable<daoben_hr_emp_job>()
                    //    .Where(a => a.company_id == addInfo.company_id && a.position_type == ConstData.POSITION_GM2).Select<string>("id").ToList();
                    ////待办事项 生成列表
                    //List<daoben_sys_task> taskList = idList
                    //    .Select(a1 => new daoben_sys_task
                    //    {
                    //        emp_id = a1,
                    //        category = 1,
                    //        main_id = addInfo.id,
                    //        main_url = "/ActivityManage/PK/Approve?id=" + addInfo.id,
                    //        title = tempStr,
                    //        content_abstract = addInfo.note,
                    //        recipient_type = 1,
                    //        create_time = now,
                    //        status = 1
                    //    }).ToList();
                    #endregion
                    db.CommandTimeOut = 150;
                    db.BeginTran();
                    db.Insert(addInfo);
                    db.DisableInsertColumns = new string[] { "id" };
                    if (productList != null && productList.Count > 0)
                        db.InsertRange(productList);
                    db.InsertRange(empList);
                    //待办事项 列表插入
                    //if (taskList != null && taskList.Count() >= 25)
                    //    db.SqlBulkCopy(taskList);
                    //else if (taskList != null && taskList.Count() > 0)
                    //    db.InsertRange(taskList);

                    db.CommitTran();
                }
            }
            catch (Exception ex)
            {
                if (db != null)
                    db.RollbackTran();
                return "系统出错：" + ex.Message;
            }
            //临时直接置为已审批 12.21            
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
            List<daoben_activity_pk_approve> approveList = new List<daoben_activity_pk_approve>();
            List<daoben_activity_pk_product> productList = new List<daoben_activity_pk_product>();

            using (var db = SugarDao.GetInstance())
            {
                daoben_activity_pk mainInfo = db.Queryable<daoben_activity_pk>().InSingle(id);
                if (mainInfo == null)
                    return null;
                List<daoben_activity_pk_emp> empList = db.Queryable<daoben_activity_pk_emp>().Where(a => a.main_id == id).ToList();
                empList = empList.OrderBy(t => t.group_number).ThenByDescending(t => t.total_count).ToList();
                if (mainInfo.product_scope == 2 || mainInfo.product_scope == 3)
                    productList = db.Queryable<daoben_activity_pk_product>().Where(a => a.main_id == id).ToList();
                if (mainInfo.approve_status != 0)
                    approveList = db.Queryable<daoben_activity_pk_approve>().Where(a => a.main_id == id).ToList();
                mainInfo.total_penalty = empList.Sum(t => t.total_penalty);
                mainInfo.total_company_reward = empList.Sum(t => t.company_reward);
                mainInfo.total_sale_count = empList.Sum(t => t.total_count);
                object retObj = new
                {
                    mainInfo = mainInfo,
                    rewardList = productList,
                    empList = empList,
                    approveList = approveList,
                };
                return retObj;
            }
        }

        /// <summary>
        /// 修改活动结束时间
        /// </summary>
        /// <param name="id"></param>
        /// <param name="alterDate"></param>
        /// <returns></returns>
        public string Alter(string id, DateTime alterDate)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (string.IsNullOrEmpty(id))
                return "信息错误，操作失败!";
            daoben_activity_pk_alter alterInfo = new daoben_activity_pk_alter
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
                    daoben_activity_pk mainInfo = db.Queryable<daoben_activity_pk>().SingleOrDefault(a => a.id == id);
                    if (mainInfo == null)
                        return "信息错误：活动信息不存在!";
                    if (mainInfo.activity_status == 2)
                        return "操作失败：活动已结束!";
                    if (mainInfo.activity_status == -2)
                        return "操作失败：活动未审批通过!";
                    if (alterDate < mainInfo.start_date)
                        return "信息错误：结束时间不能小于开始时间!";
                    alterInfo.orig_end_date = mainInfo.end_date;
                    db.Insert(alterInfo);

                    #region TODO 审批，这里暂时直接通过
                    //修改完时间后判断是否要修改活动状态
                    DateTime now = DateTime.Now;
                    int activity_status = mainInfo.activity_status;
                    if (now.Date > alterDate)
                        activity_status = 2;
                    object upObj = new
                    {
                        end_date = alterDate,
                        activity_status = activity_status
                    };
                    db.Update<daoben_activity_pk>(upObj, a => a.id == id);
                    db.Update<daoben_activity_pk_alter>(new { approve_status = 100 }, a => a.id == alterInfo.id);
                    if (activity_status < 0)
                        return "success";
                    List<daoben_activity_pk> mainInfoList = new List<daoben_activity_pk>();
                    mainInfoList.Add(mainInfo);
                    Statistics(db, mainInfoList);
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
                return null;
            SqlSugarClient db = null;
            try
            {
                using (db = SugarDao.GetInstance())
                {
                    daoben_activity_pk recallInfo = db.Queryable<daoben_activity_pk>().InSingle(id);
                    if (recallInfo == null)
                        return "信息错误：活动信息不存在!";
                    if (recallInfo.creator_job_history_id != LoginInfo.jobHistoryId)
                        return "操作失败：活动信息只能由发起人撤回!";
                    if (recallInfo.approve_status != 0)
                        return "操作失败：活动信息已审批!";
                    db.CommandTimeOut = 150;
                    db.BeginTran();
                    if (recallInfo.product_scope == 2 || recallInfo.product_scope == 3)
                        db.Delete<daoben_activity_pk_product>(a => a.main_id == id);
                    db.Delete<daoben_activity_pk_emp>(a => a.main_id == id);
                    db.Delete<daoben_activity_pk>(a => a.id == id);
                    //删除所有待办事项
                    db.Delete<daoben_sys_task>(a => a.main_id == id);
                    db.Delete<daoben_sys_notification>(a => a.main_id == id);
                    return "success";
                }
            }
            catch (Exception ex)
            {
                if (db != null)
                    db.RollbackTran();
                return "系统出错：" + ex.Message;
            }
        }
        /// <summary>
        /// //0分公司总经理1——1事业部总经理/助手2——2财务经理100
        /// </summary>
        /// <param name="approveInfo"></param>
        /// <returns></returns>
        public string Approve(daoben_activity_pk_approve approveInfo)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
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
                    daoben_activity_pk mainInfo = db.Queryable<daoben_activity_pk>().InSingle(approveInfo.main_id);
                    if (mainInfo == null)
                        return "操作失败：活动信息不存在";
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
                    //查询之前待办事项
                    List<daoben_sys_task> origTask = db.Queryable<daoben_sys_task>().Where(a => a.main_id == approveInfo.main_id).ToList();
                    object upObjForTask = new
                    {
                        status = 3,
                        finished_time = now
                    };
                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    //清除之前的待办事项
                    if (origTask.Count() > 0)
                        db.Update<daoben_sys_task>(upObjForTask, a => a.main_id == approveInfo.main_id);
                    db.DisableInsertColumns = new string[] { "id" };
                    db.Insert(approveInfo);
                    db.Update<daoben_activity_pk>(upObj, a => a.id == approveInfo.main_id);


                    db.CommitTran();

                    #region 待办事项
                    //List<daoben_sys_notification> newsTotal = new List<daoben_sys_notification>();//消息通知
                    //List<daoben_sys_task> taskTotal = new List<daoben_sys_task>();//待办事项
                    //string newsStr = null;
                    //List<string> newsIdList = null;
                    //string taskStr = null;
                    //List<string> taskIdList = null;
                    ////0分公司总经理1——1事业部总经理/助手2——2财务经理100
                    //daoben_activity_pk origInfo = db.Queryable<daoben_activity_pk>().InSingle(approveInfo.main_id);
                    //if (origInfo.approve_status == 1)
                    //    taskIdList = db.Queryable<daoben_hr_emp_job>()
                    //        .Where(a => a.position_type == ConstData.POSITION_GM1 && a.company_id == origInfo.company_id_parent)
                    //        .Select<string>("id").ToList();
                    //else if (origInfo.approve_status == 2)
                    //    taskIdList = db.Queryable<daoben_hr_emp_job>()
                    //                .JoinTable<daoben_ms_account>((a, b) => a.id == b.employee_id)
                    //                .Where<daoben_ms_account>((a, b) => b.role_id == ConstData.ROLE_ID_FINANCIALMANAGER)
                    //                .Where(a => a.company_id == origInfo.company_id_parent)
                    //                .Select<string>("a.id as id").ToList();
                    //else if (origInfo.approve_status != 0)
                    //    newsIdList = db.Queryable<daoben_distributor_info>()
                    //             .Where(a => a.id == origInfo.creator_job_history_id).Select<string>("id").ToList();

                    //taskStr = "PK活动待审批";
                    ////待办事项 生成列表
                    //if (taskIdList != null && taskIdList.Count > 0)
                    //{
                    //    taskTotal = taskIdList
                    //        .Select(a1 => new daoben_sys_task
                    //        {
                    //            emp_id = a1,
                    //            category = 1,
                    //            main_id = origInfo.id,
                    //            main_url = "/ActivityManage/PK/Approve?id=" + origInfo.id,
                    //            title = taskStr,
                    //            content_abstract = origInfo.note,
                    //            recipient_type = 1,
                    //            create_time = now,
                    //            status = 1
                    //        }).ToList();
                    //}
                    //if (newsIdList != null && newsIdList.Count > 0)
                    //{
                    //    if (origInfo.approve_status < 0)
                    //        newsStr = "PK活动审批通过";
                    //    else if (origInfo.approve_status == 100)
                    //        newsStr = "PK活动已审批通过";
                    //    //消息通知 生成列表
                    //    newsTotal = newsIdList
                    //            .Select(a1 => new daoben_sys_notification
                    //            {
                    //                emp_id = a1,
                    //                category = 2,
                    //                main_id = origInfo.id,
                    //                main_url = "/ActivityManage/PK/Show?id=" + origInfo.id,
                    //                title = newsStr,
                    //                content_abstract = origInfo.note,
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
                    return "success";
                }
            }
            catch (Exception ex)
            {
                if (db != null)
                    db.RollbackTran();
                return "系统出错：" + ex.Message;
            }
        }
        public void ApproveTmp(string id)
        {
            using (var db = SugarDao.GetInstance())
            {
                daoben_activity_pk origInfo = db.Queryable<daoben_activity_pk>().InSingle(id);
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
                db.Update<daoben_activity_pk>(upObj, a => a.id == id);
                if (activity_status < 0)
                    return;
                List<daoben_activity_pk> mainInfoList = new List<daoben_activity_pk>();
                mainInfoList.Add(origInfo);
                Statistics(db, mainInfoList);
            }
        }

        public void Statistics(SqlSugarClient db, List<daoben_activity_pk> mainInfoList)
        {
            if (mainInfoList == null || mainInfoList.Count < 1)
                return;
            DateTime now = DateTime.Now;
            StringBuilder sqlUpSb1 = new StringBuilder("insert into daoben_activity_pk_emp (`id`, `total_amount`, `total_count`, `total_ratio`,`total_reward`,`winner`,`total_penalty`,`company_reward`) values ");

            // StringBuilder sqlUpSb2 = new StringBuilder("insert into daoben_activity_pk () values ");
            StringBuilder sqlUpSb3 = new StringBuilder("insert into daoben_activity_pk (`id`, `total_sale_count`, `total_sale_amount`, `total_company_reward`, `total_penalty`, `counting_time`) values ");

            //  bool toUp2 = false;
            List<daoben_activity_pk_emp> empList = null;
            // List<daoben_activity_pk_product> productList = null;
            foreach (var mainInfo in mainInfoList)
            {

                DateTime startDate = mainInfo.start_date.ToDate();
                DateTime endDate = mainInfo.end_date > now ? now : mainInfo.end_date.ToDate().AddDays(1);
                List<daoben_activity_pk_emp> fullEmpList = db.Queryable<daoben_activity_pk_emp>()
                        .Where(e => e.main_id == mainInfo.id).Select("id, emp_id, group_number").ToList();
                #region 查询销量
                var qable = db.Queryable<daoben_activity_pk_emp>();
                // 按实销
                if (mainInfo.emp_category == 3)         //导购员
                {
                    qable.JoinTable<daoben_product_sn>((e, s) => s.reporter_id == e.emp_id && s.reporter_type == 1 && s.sale_type > 0
                           && s.sale_time >= startDate && s.sale_time < endDate);
                }
                else if (mainInfo.emp_category == 20)    // 业务员
                {
                    qable.JoinTable<daoben_product_sn>((e, s) => s.sales_id == e.emp_id && s.sale_type > 0
                            && s.sale_time >= startDate && s.sale_time < endDate);
                }
                else                                // 业务经理
                {
                    qable.JoinTable<daoben_product_sn>((e, s) => s.sales_m_id == e.emp_id && s.sale_type > 0
                            && s.sale_time >= startDate && s.sale_time < endDate);
                }

                if (mainInfo.product_scope > 1)       // 指定机型
                {
                    qable.JoinTable<daoben_product_sn, daoben_activity_pk_product>((e, s, p) => p.model == s.model && p.color == s.color)
                            .Where<daoben_activity_pk_product>((e, p) => p.main_id == mainInfo.id);
                }
                empList = qable.Where(e => e.main_id == mainInfo.id).GroupBy("e.emp_id").OrderBy("e.group_number")
                        .Select<daoben_activity_pk_emp>("e.id, e.emp_id, e.group_number, count(s.id) as total_count, sum(s.price_sale) as total_amount, e.activity_target")
                        .ToList();
                var empResList = empList.Union(fullEmpList, new EmpCompare())
                        .OrderBy(a => a.group_number).ToList(); // 合并LIST并保留empList的结果
                #endregion
                if (empResList != null && empResList.Count > 0) // empList只包含有销量的员工 
                {
                    int total_sale_count = 0;
                    decimal total_sale_amount = 0, total_company_reward = 0, total_penalty = 0;
                    for (int i = 0; i < empResList.Count; i++)
                    {
                        if (empList[i].total_count == 0)
                            empList[i].total_ratio = 0;
                        else if (empList[i].activity_target == 0)
                            empList[i].total_ratio = 100;
                        else
                            empList[i].total_ratio = (empList[i].total_count * 100) / (empList[i].activity_target);
                        if (i % 2 == 1) // else 新的一组，暂时不处理
                        {
                            #region 比赛结果计算
                            if (empList[i - 1].total_ratio == empList[i].total_ratio)
                            {   // 平手
                                empList[i - 1].winner = empList[i].winner = false;
                                empList[i - 1].total_reward = empList[i].total_reward = 0;
                                empList[i - 1].company_reward = empList[i].company_reward = 0;
                                if (empList[i].total_ratio < 100)
                                    empList[i - 1].total_penalty = empList[i].total_penalty = 0 - mainInfo.lose_penalty;
                            }
                            else
                            {
                                int winI = i, loseI = i - 1;
                                if (empList[i - 1].total_ratio > empList[i].total_ratio)
                                {
                                    winI = i - 1;
                                    loseI = i;
                                }
                                empList[winI].winner = true;
                                empList[loseI].winner = false;
                                if (empList[winI].total_ratio < 100)
                                {
                                    empList[winI].total_reward = mainInfo.win_lose - mainInfo.win_penalty;
                                    empList[winI].total_penalty = 0 - mainInfo.win_penalty;
                                }
                                else
                                {
                                    empList[winI].total_reward = mainInfo.win_lose;
                                    empList[winI].total_penalty = 0;
                                    if (mainInfo.win_company_condition == 2)
                                    {
                                        empList[winI].total_reward += mainInfo.win_company;
                                        empList[winI].company_reward = mainInfo.win_company;
                                    }
                                    else if (empList[loseI].total_ratio >= 100) // win_company_condition == 1
                                    {
                                        empList[winI].total_reward += mainInfo.win_company;
                                        empList[winI].company_reward = mainInfo.win_company;
                                    }
                                }
                                if (empList[loseI].total_ratio < 100)
                                {
                                    empList[loseI].total_reward = 0 - mainInfo.lose_win - mainInfo.lose_penalty;
                                    empList[loseI].total_penalty = 0 - mainInfo.lose_penalty;
                                }
                                else
                                    empList[loseI].total_reward = 0 - mainInfo.lose_win;
                            }
                            total_sale_count += empList[i].total_count + empList[i - 1].total_count;
                            total_sale_amount += empList[i].total_amount + empList[i - 1].total_amount;
                            total_company_reward += empList[i].total_reward + empList[i - 1].total_reward;
                            total_penalty += empList[i].total_penalty + empList[i - 1].total_penalty;
                            sqlUpSb1.AppendFormat("({0},{1},{2},{3},{4},{5},{6},{7}),", empList[i - 1].id, empList[i - 1].total_amount,
                                    empList[i - 1].total_count, empList[i - 1].total_ratio, empList[i - 1].total_reward,
                                    empList[i - 1].winner, empList[i - 1].total_penalty, empList[i - 1].company_reward);
                            sqlUpSb1.AppendFormat("({0},{1},{2},{3},{4},{5},{6},{7}),", empList[i].id, empList[i].total_amount,
                                    empList[i].total_count, empList[i].total_ratio, empList[i].total_reward, empList[i].winner,
                                    empList[i].total_penalty, empList[i].company_reward);
                            #endregion
                        }
                    }
                    sqlUpSb3.AppendFormat("('{0}',{1},{2},{3},{4},'{5}'),", mainInfo.id, total_sale_count, total_sale_amount, total_company_reward, total_penalty, endDate.AddDays(-1));
                }
            }
            sqlUpSb1.Remove(sqlUpSb1.Length - 1, 1); // 最后一个逗号
            sqlUpSb1.Append("on duplicate key update total_amount=values(total_amount),total_count=values(total_count),");
            sqlUpSb1.Append("total_ratio =values(total_ratio),total_reward=values(total_reward),winner=values(winner),");
            sqlUpSb1.Append("total_penalty =values(total_penalty),company_reward=values(company_reward);");
            db.SqlQuery<int>(sqlUpSb1.ToString());
            sqlUpSb3.Remove(sqlUpSb3.Length - 1, 1); // 最后一个逗号
            sqlUpSb3.Append("on duplicate key update total_sale_count=values(total_sale_count),total_sale_amount=values(total_sale_amount),");
            sqlUpSb3.Append("total_company_reward =values(total_company_reward),total_penalty=values(total_penalty),counting_time=values(counting_time);");
            db.SqlQuery<int>(sqlUpSb3.ToString());

        }
        public class EmpCompare : IEqualityComparer<daoben_activity_pk_emp>
        {
            public bool Equals(daoben_activity_pk_emp x, daoben_activity_pk_emp y)
            {
                return x.id == y.id;
            }
            public int GetHashCode(daoben_activity_pk_emp obj)
            {
                return obj.id.GetHashCode();
            }
        }

    }
}
