using Base.Code;
using Base.Code.Security;
using MySqlSugar;
using ProjectShare.Database;
using ProjectShare.Models;
using ProjectShare.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ProjectWeb.Areas.DistributorManage.Application
{

    public class RecommendationApp
    {

        public object AllList(Pagination pagination, daoben_distributor_recommendation queryInfo, int? status, QueryTime queryTime)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "create_time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_distributor_recommendation>();
                //查看所有
                //分公司助理-分公司总经理-总经理-财务
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER)
                        qable.Where(a => a.company_id == myCompanyInfo.id || a.company_id_parent == myCompanyInfo.id);
                    else if (myPositionInfo.positionType < ConstData.POSITION_OFFICE_NORMAL)
                        qable.Where(a => a.company_id == myCompanyInfo.id || a.company_id_parent == myCompanyInfo.id);
                    else return null;
                }
                if (queryInfo != null)
                {
                    if (queryInfo.name != null)
                        qable.Where(a => a.name.Contains(queryInfo.name));
                    if (queryInfo.activity_status != 0)
                        qable.Where(a => a.activity_status == queryInfo.activity_status);
                    if (queryInfo.company_id != 0)
                        qable.Where(a => a.company_id == queryInfo.company_id);
                }
                if (queryTime != null)
                {
                    if (queryTime.startTime1 != null)
                        qable.Where(a => a.start_date >= queryTime.startTime1);
                    if (queryTime.startTime2 != null)
                    {
                        queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                        qable.Where(a => a.start_date < queryTime.startTime2);
                    }
                    if (queryTime.endTime1 != null)
                        qable.Where(a => a.end_date >= queryTime.endTime1);
                    if (queryTime.endTime2 != null)
                    {
                        queryTime.endTime2 = queryTime.endTime2.ToDate().AddDays(1);
                        qable.Where(a => a.create_time < queryTime.endTime2);
                    }
                }
                string listStr = qable
                        .OrderBy(pagination.sidx + " " + pagination.sord)
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public object ApproveList(Pagination pagination, daoben_distributor_recommendation queryInfo)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "create_time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_distributor_recommendation>();
                //我的审批
                //分公司助理-分公司总经理-总经理-财务
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (myPositionInfo.positionType == ConstData.POSITION_GM2)//分公司总经理
                    {
                        qable.Where(a => a.company_id == myCompanyInfo.id);
                        qable.Where(a => a.approve_status == 0);
                    }
                    else if (myPositionInfo.positionType == ConstData.POSITION_GM1)//事业部总经理
                    {
                        qable.Where(a => a.company_id == myCompanyInfo.id || a.company_id_parent == myCompanyInfo.id);
                        qable.Where(a => a.approve_status == 1);
                    }
                    else if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER)//财务经理
                    {
                        qable.Where(a => a.company_id == myCompanyInfo.id || a.company_id_parent == myCompanyInfo.id);
                        qable.Where(a => a.approve_status == 2);
                    }
                    else return null;
                }
                if (queryInfo != null)
                {
                    if (queryInfo.name != null)
                        qable.Where(a => a.name.Contains(queryInfo.name));
                    if (queryInfo.company_id != 0)
                        qable.Where(a => a.company_id == queryInfo.company_id);
                }
                string listStr = qable
                        .OrderBy(pagination.sidx + " " + pagination.sord)
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public object DistributorList(Pagination pagination, string queryName)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "create_time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_distributor_info>().Where(a => a.inactive == false);
                //分公司助理-分公司总经理-总经理-财务（only 分公司助理）
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT2)
                        qable.Where(a => a.company_id == myCompanyInfo.id);
                    else return null;
                }
                if (queryName != null)
                    qable.Where(a => a.name.Contains(queryName));
                string listStr = qable
                        .OrderBy(pagination.sidx + " " + pagination.sord)
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public string GetInfo(string id)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            if (string.IsNullOrEmpty(id))
                return "信息错误：ID不能为空";
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_distributor_recommendation mainInfo = db.Queryable<daoben_distributor_recommendation>().SingleOrDefault(a => a.id == id);
                    if (mainInfo == null)
                        return "信息错误：指定的信息不存在";
                    List<daoben_distributor_recommendation_approve> approveList = db.Queryable<daoben_distributor_recommendation_approve>().Where(a => a.main_id == id).ToList();
                    List<daoben_distributor_recommendation_product> productList = db.Queryable<daoben_distributor_recommendation_product>().Where(a => a.main_id == id).ToList();
                    string creator_position_name = db.Queryable<daoben_hr_emp_job_history>()
                        .Where(a => a.id == mainInfo.creator_job_history_id).Select<string>("position_name").SingleOrDefault();
                    DateTime startDate = mainInfo.start_date.ToDate();
                    DateTime endDate = mainInfo.end_date.ToDate().AddDays(1);
                    List<daoben_distributor_recommendation_distributor> distributorList = db.Queryable<daoben_distributor_recommendation_distributor>().Where(a => a.main_id == id).ToList();
                    List<daoben_product_sn> snScopeList = new List<daoben_product_sn>();
                    if (mainInfo.product_scope == 1)
                        snScopeList = db.Queryable<daoben_product_sn>()
                            .JoinTable<daoben_distributor_recommendation_distributor>((a, b) => a.sale_distributor_id == b.distributor_id)
                            .Where<daoben_distributor_recommendation_distributor>((a, b) => b.main_id == mainInfo.id && a.sale_time >= startDate && a.sale_time < endDate && a.status > 1).ToList();
                    else
                        snScopeList = db.Queryable<daoben_product_sn>()
                            .JoinTable<daoben_distributor_recommendation_distributor>((a, b) => a.sale_distributor_id == b.distributor_id)
                            .JoinTable<daoben_distributor_recommendation_product>((a, c) => a.model == c.model && a.color == c.color)
                            .Where<daoben_distributor_recommendation_distributor, daoben_distributor_recommendation_product>((a, b, c) => b.main_id == mainInfo.id && c.main_id == mainInfo.id && a.sale_time >= startDate && a.sale_time < endDate && a.status > 1).ToList();
                    List<daoben_distributor_recommendation_rebate> rebateList = db.Queryable<daoben_distributor_recommendation_rebate>()
                        .Where(a => a.main_id == mainInfo.id).ToList();

                    //foreach (var i in distributorList)
                    //{
                    //    i.total_amount = 0;
                    //    List<daoben_product_sn> snList = snScopeList.Where(a => a.sale_distributor_id == i.distributor_id).ToList();
                    //    i.total_count = snList.Count;//统计销量
                    //    foreach (var y in snList)
                    //    {
                    //        i.total_amount += y.price_retail;
                    //    }
                    //}

                    //foreach (var i in distributorList)
                    //{
                    //    daoben_distributor_recommendation_rebate rebateModel = rebateList.Where(b => i.total_count >= b.target_min).OrderByDescending(t => t.target_min).First();
                    //    i.total_rebate = rebateModel.rebate * i.total_count;//计算返利金额 每台固定金额*台数
                    //};
                    foreach (var p in productList)
                    {
                        List<daoben_product_sn> snList = snScopeList.Where(a => a.model == p.model && a.color == p.color).ToList();
                        p.total_count = snList.Count;
                        snList = snList.Where(t => t.sale_type == 1).ToList();
                        if (p.total_count > 0)
                            p.total_normal_amount = p.total_count * (snList.First().price_retail);//零售价 * 数量  
                        else
                            p.total_normal_amount = 0;
                    }
                    object statisticsTime;
                    if (snScopeList.Count > 1)
                        statisticsTime = new
                        {
                            end_time = snScopeList.OrderByDescending(t => t.sale_time).First().sale_time,
                            start_time = snScopeList.OrderBy(t => t.sale_time).First().sale_time,
                        };
                    else
                        statisticsTime = new
                        {
                            start_time = mainInfo.start_date,
                            end_time = mainInfo.end_date == null ? DateTime.Now : mainInfo.end_date,
                        };

                    #region 连表查询 已注释
                    //var qable = db.Queryable<daoben_product_sn>()
                    //    .JoinTable<daoben_distributor_info>((a, b) => a.distributor_id == b.id)
                    //    .JoinTable<daoben_distributor_info, daoben_distributor_recommendation_distributor>((a, b, c) => b.id == c.distributor_id)
                    //    .Where<daoben_distributor_info, daoben_distributor_recommendation_distributor>((a, b, c) => c.main_id == id);
                    //if (mainInfo.target_mode == 1)
                    //{
                    //    DateTime endDate = mainInfo.end_date.ToDate().AddDays(1);
                    //    if (mainInfo.target_content == 1)
                    //        qable.Where(a => a.sale_time >= mainInfo.start_date && a.sale_time <= endDate && a.status >= 2);
                    //    else if (mainInfo.target_content == 2)
                    //        qable.Where(a => a.outstorage_time >= mainInfo.start_date && a.outstorage_time <= endDate && a.status >= 1);
                    //}
                    //else if (mainInfo.target_mode == 2)
                    //{
                    //    if (mainInfo.target_content == 1)
                    //        qable.Where(a => a.sale_time >= mainInfo.start_date && a.status >= 2);
                    //    else if (mainInfo.target_content == 2)
                    //        qable.Where(a => a.outstorage_time >= mainInfo.start_date && a.status >= 1);
                    //}
                    //string calculateInfo = qable.Select("b.id as distribuotr_id,b.name as distributor_name,count(*) as quantity,SUM(a.price_retail) as amount_price_retail, SUM(a.price_wholesale) as amount_price_wholesale ").GroupBy("name").OrderBy("name").ToJson();
                    //calculateInfo = calculateInfo.ToJson(),
                    #endregion
                    object resultObj = new
                    {
                        mainInfo = mainInfo,
                        approveList = approveList,
                        distributorList = distributorList,
                        productList = productList,
                        rebateList = rebateList,
                        creator_position_name = creator_position_name,
                        statisticsTime = statisticsTime
                    };
                    return resultObj.ToJson();

                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }


        public string GetDistributorList()
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            using (var db = SugarDao.GetInstance())
            {
                //只能是分公司助手发起
                List<daoben_distributor_info> distributorList = db.Queryable<daoben_distributor_info>()
                            .Where(a => a.company_id == myCompanyInfo.id).ToList();
                object resultObj = new
                {
                    distributorList = distributorList
                };
                return resultObj.ToJson();
            }
        }


        public string Add(daoben_distributor_recommendation addInfo, List<daoben_distributor_recommendation_distributor> distributorList,
            List<daoben_distributor_recommendation_product> productList, List<daoben_distributor_recommendation_rebate> rebateList)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            if (addInfo == null)
                return "信息错误：考核信息不能为空";
            if (string.IsNullOrEmpty(addInfo.id) || addInfo.id.Length != 36)
                return "信息错误：ID不正确!";
            if (addInfo.target_mode < 1 || addInfo.target_mode > 4)
                return "信息错误：考核模式错误";
            if (distributorList == null || distributorList.Count < 1)
                return "信息错误，请至少选择一个经销商!";
            if (addInfo.product_scope < 1 || addInfo.product_scope > 2)
                return "信息错误：活动机型类别有误!";
            if (addInfo.product_scope == 2 && (productList == null || productList.Count < 1))
                return "信息错误，机型信息不能为空!";
            if (addInfo.target_mode != 4 && (rebateList == null || rebateList.Count < 1))
                return "信息错误，返利规则不能为空!";

            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    if (myCompanyInfo.category == "分公司")
                        addInfo.company_id_parent = myCompanyInfo.parentId;
                    else
                        addInfo.company_id_parent = myCompanyInfo.id;//发起人为事业部

                    addInfo.activity_status = -2;
                    addInfo.approve_status = 0;
                    addInfo.counting_time = null;
                    addInfo.creator_job_history_id = LoginInfo.jobHistoryId;
                    addInfo.creator_name = LoginInfo.empName;
                    addInfo.create_time = DateTime.Now;

                    db.CommandTimeOut = 30;
                    db.BeginTran();

                    db.Insert(addInfo);
                    db.DisableInsertColumns = new string[] { "id" };
                    db.InsertRange(distributorList);
                    if (productList != null)
                        db.InsertRange(productList);
                    if (rebateList != null)
                        db.InsertRange(rebateList);

                    db.CommitTran();

                    //待办事项 TODO 待测试
                    #region 待办事项 分公司总经理
                    //待办事项 收件人
                    string taskStr = "主推返利申请待审批";
                    List<string> taskIdList = db.Queryable<daoben_hr_emp_job>()
                        .Where(a => a.company_id == myCompanyInfo.id && a.position_type == ConstData.POSITION_GM2).Select<string>("id").ToList();
                    //待办事项 生成列表
                    List<daoben_sys_task> taskList = taskIdList
                        .Select(a1 => new daoben_sys_task
                        {
                            emp_id = a1,
                            category = 1,
                            main_id = addInfo.id,
                            main_url = "/DistributorManage/Recommendation/Approve?id=" + addInfo.id,
                            title = taskStr,
                            content_abstract = addInfo.note,
                            recipient_type = 1,
                            create_time = now,
                            status = 1
                        }).ToList();
                    #endregion
                    //待办事项 列表插入
                    if (taskList != null && taskList.Count() >= 25)
                        db.SqlBulkCopy(taskList);
                    else if
                        (taskList != null && taskList.Count() > 0)
                        db.InsertRange(taskList);
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return "系统出错：" + ex.Message;
                }

                //12.14 默认活动已审批通过 TODO
                ApproveTmp(addInfo.id);

                return "success";
            }
        }

        public string Approve(daoben_distributor_recommendation_approve approveInfo)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            object upObj = null;

            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_distributor_recommendation origInfo = db.Queryable<daoben_distributor_recommendation>().SingleOrDefault(a => a.id == approveInfo.main_id);
                    if (origInfo == null)
                        return "信息错误：指定的申请信息不存在";
                    if ((LoginInfo.roleId == ConstData.ROLE_ID_FINANCIAL || LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER) && approveInfo.status > 0)
                    {
                        approveInfo.status = origInfo.approve_status = 100;
                        if (origInfo.start_date <= DateTime.Now)
                            upObj = new { approve_status = approveInfo.status, activity_status = 1 };
                        else
                            upObj = new { approve_status = approveInfo.status, activity_status = -1 };
                    }
                    else
                    {
                        if (approveInfo.status > 0)
                        {
                            origInfo.approve_status = 0 + 1 + origInfo.approve_status;
                            approveInfo.status = origInfo.approve_status;
                        }
                        else
                        {
                            origInfo.approve_status = 0 - 1 - origInfo.approve_status;
                            approveInfo.status = origInfo.approve_status;
                        }
                        upObj = new { approve_status = approveInfo.status };
                    }
                    approveInfo.approve_id = LoginInfo.accountId;
                    approveInfo.approve_name = LoginInfo.empName;
                    approveInfo.approve_time = DateTime.Now;
                    approveInfo.approve_position_id = myPositionInfo.id;
                    approveInfo.approve_position_name = myPositionInfo.name;

                    // 消息通知
                    db.CommandTimeOut = 30;
                    db.BeginTran();

                    db.Update<daoben_distributor_recommendation>(upObj, a => a.id == origInfo.id);
                    db.Insert(approveInfo);

                    //清除之前待办事项
                    List<daoben_sys_task> origTask = db.Queryable<daoben_sys_task>().Where(a => a.main_id == approveInfo.main_id).ToList();
                    object upObjForTask = new
                    {
                        status = 3,
                        finished_time = now
                    };
                    if (origTask != null && origTask.Count() > 0)
                        db.Update<daoben_sys_task>(upObjForTask, a => a.main_id == approveInfo.main_id);
                    db.CommitTran();
                    #region 添加下一步待办事项+消息通知
                    List<daoben_sys_notification> newsTotal = new List<daoben_sys_notification>();//消息通知
                    List<daoben_sys_task> taskTotal = new List<daoben_sys_task>();//待办事项
                    string newsStr = null;
                    List<string> newsIdList = null;
                    string taskStr = null;
                    List<string> taskIdList = null;
                    //分公司助理(申请)-0分公司总经理1-1总经理2-2财务100
                    daoben_distributor_recommendation recommendationInfo = db.Queryable<daoben_distributor_recommendation>().SingleOrDefault(a => a.id == approveInfo.main_id);
                    if (recommendationInfo.approve_status == 1)
                        // TODO 待办事项 --事业部总经理
                        taskIdList = db.Queryable<daoben_hr_emp_job>()
                            .Where(a => a.position_type == ConstData.POSITION_GM1 && a.company_id == recommendationInfo.company_id_parent)
                            .Select<string>("id").ToList();
                    else if (recommendationInfo.approve_status == 2)
                    {
                        //财务审批
                        taskIdList = db.Queryable<daoben_hr_emp_job>()
                                    .JoinTable<daoben_ms_account>((a, b) => a.id == b.employee_id)
                                    .Where<daoben_ms_account>((a, b) => b.role_id == ConstData.ROLE_ID_FINANCIALMANAGER)
                                    .Where(a => a.company_id == recommendationInfo.company_id_parent)
                                    .Select<string>("a.id as id").ToList();
                    }
                    else if (recommendationInfo.approve_status != 0)
                        newsIdList = db.Queryable<daoben_hr_emp_job_history>()
                                .Where(a => a.id == recommendationInfo.creator_job_history_id).Select<string>("emp_id as id").ToList();

                    taskStr = "主推返利申请待审批";
                    //待办事项 生成列表
                    if (taskIdList != null && taskIdList.Count > 0)
                    {
                        taskTotal = taskIdList
                            .Select(a1 => new daoben_sys_task
                            {
                                emp_id = a1,
                                category = 1,
                                main_id = recommendationInfo.id,
                                main_url = "/DistributorManage/Recommendation/Approve?id=" + recommendationInfo.id,
                                title = taskStr,
                                content_abstract = recommendationInfo.note,
                                recipient_type = 1,
                                create_time = now,
                                status = 1
                            }).ToList();
                    }
                    if (newsIdList != null && newsIdList.Count > 0)
                    {
                        if (recommendationInfo.approve_status < 0)
                            newsStr = "主推返利申请没有通过";
                        else if (recommendationInfo.approve_status == 100)
                            newsStr = "主推返利申请已通过";
                        //消息通知 生成列表
                        List<daoben_sys_notification> newsList = newsIdList
                                .Select(a1 => new daoben_sys_notification
                                {
                                    emp_id = a1,
                                    category = 2,
                                    main_id = recommendationInfo.id,
                                    main_url = "/DistributorManage/Recommendation/Show?id=" + recommendationInfo.id,
                                    title = newsStr,
                                    content_abstract = recommendationInfo.note,
                                    recipient_type = 1,
                                    create_time = now,
                                    status = 1
                                }).ToList();
                        newsTotal.AddRange(newsList);
                    }
                    //待办事项 and 消息通知 列表插入
                    if (newsTotal != null && newsTotal.Count > 0)
                        db.InsertRange(newsTotal);
                    else if (newsTotal != null && newsTotal.Count > 25)
                        db.SqlBulkCopy(newsTotal);
                    if (taskTotal != null && taskTotal.Count > 0)
                        db.InsertRange(taskTotal);
                    else if (taskTotal != null && taskTotal.Count > 25)
                        db.SqlBulkCopy(taskTotal);
                    #endregion 待办事项



                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return ex.Message;
                }

                RecomRebate(approveInfo.main_id);
                return "success";
            }
        }
        public void RecomRebate(string id)
        {

            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    DateTime now = DateTime.Now;
                    List<daoben_distributor_recommendation> recomList = db.Queryable<daoben_distributor_recommendation>()
                        .Where(a => a.id == id).ToList();
                    SettlementApp setApp = new SettlementApp();
                    string sqlUpSb = setApp.DisRecomRebate(db, recomList);
                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    if (!string.IsNullOrEmpty(sqlUpSb))
                        db.SqlQuery<int>(sqlUpSb);
                    db.CommitTran();
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                }
            }

        }
        public void ApproveTmp(string id)
        {
            using (var db = SugarDao.GetInstance())
            {
                daoben_distributor_recommendation origInfo = db.Queryable<daoben_distributor_recommendation>().InSingle(id);
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
                db.Update<daoben_distributor_recommendation>(upObj, a => a.id == id);
            }
            RecomRebate(id);
        }

        //public string GetDetailInfo(string id)
        //{
        //    var LoginInfo = OperatorProvider.Provider.GetCurrent();
        //    if (LoginInfo == null)
        //        return "登录超时，请重新登录";
        //    if (string.IsNullOrEmpty(id))
        //        return "信息错误：ID不能为空";
        //    using (var db = SugarDao.GetInstance())
        //    {
        //        try
        //        {
        //            daoben_distributor_recommendation mainInfo = db.Queryable<daoben_distributor_recommendation>().SingleOrDefault(a => a.id == id);
        //            if (mainInfo == null)
        //                return "信息错误：指定的信息不存在";
        //            DateTime startDate = mainInfo.start_date.ToDate();
        //            List<daoben_distributor_recommendation_distributor> distributorList = db.Queryable<daoben_distributor_recommendation_distributor>().Where(a => a.main_id == id).ToList();
        //            List<daoben_product_sn> snScopeList = db.Queryable<daoben_product_sn>()
        //                .JoinTable<daoben_distributor_recommendation_distributor>((a, b) => a.distributor_id == b.distributor_id)
        //                .Where<daoben_distributor_recommendation_distributor>((a, b) => b.main_id == mainInfo.id && a.sale_time >= startDate).ToList();
        //            List<daoben_distributor_recommendation_rebate> rebateList = db.Queryable<daoben_distributor_recommendation_rebate>()
        //                .Where(a => a.main_id == mainInfo.id).ToList();

        //            foreach (var i in distributorList)
        //            {
        //                List<daoben_product_sn> snList = snScopeList.Where(a => a.distributor_id == i.distributor_id).ToList();
        //                i.total_count = snList.Count;//统计销量
        //            }

        //            foreach (var i in distributorList)
        //            {
        //                daoben_distributor_recommendation_rebate rebateModel = rebateList.Where(b => i.total_count >= b.target_min).OrderByDescending(t => t.target_min).First();
        //                i.total_rebate = rebateModel.rebate * i.total_count;//计算返利金额 每天固定金额*台数
        //            };
        //            return distributorList.ToJson();

        //        }
        //        catch (Exception ex)
        //        {
        //            return ex.Message;
        //        }
        //    }
        //}
    }
}
