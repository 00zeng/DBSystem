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
    /// <summary>
    /// 达量奖励
    /// </summary>
    public class AttainingApp
    {

        public object AllList(Pagination pagination, daoben_distributor_attaining queryInfo, int? status, QueryTime queryTime)
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
                var qable = db.Queryable<daoben_distributor_attaining>();
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
                    if (queryInfo.category > 0)
                        qable.Where(a => a.category == queryInfo.category);
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
                        qable.Where(a => a.end_date < queryTime.endTime2);
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

        public object ApproveList(Pagination pagination, daoben_distributor_attaining queryInfo)
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
                var qable = db.Queryable<daoben_distributor_attaining>();
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

        public string GetInfo(string id, bool forEdit = false)
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
                    daoben_distributor_attaining mainInfo = db.Queryable<daoben_distributor_attaining>().SingleOrDefault(a => a.id == id);
                    if (mainInfo == null)
                        return "信息错误：指定的信息不存在";
                    DateTime startDate = mainInfo.start_date.ToDate();
                    DateTime endDate = mainInfo.end_date.ToDate().AddDays(1);
                    List<daoben_distributor_attaining_approve> approveList = db.Queryable<daoben_distributor_attaining_approve>()
                                .Where(a => a.main_id == id).ToList();

                    List<daoben_distributor_attaining_distributor> distributorList = db.Queryable<daoben_distributor_attaining_distributor>()
                                .Where(a => a.main_id == id).ToList();
                    List<daoben_distributor_attaining_product_sec> productSecList = db.Queryable<daoben_distributor_attaining_product_sec>()
                                .Where(a => a.main_id == id).ToList();

                    List<daoben_distributor_attaining_time_sec> timeSecList = db.Queryable<daoben_distributor_attaining_time_sec>()
                                .Where(a => a.main_id == id).ToList();
                    List<daoben_distributor_attaining_rebate> rebateList = db.Queryable<daoben_distributor_attaining_rebate>()
                                .Where(a => a.main_id == id).ToList();
                    List<daoben_distributor_attaining_rebate_product> rebateProList = db.Queryable<daoben_distributor_attaining_rebate_product>()
                                .Where(a => a.main_id == id).ToList();
                    List<daoben_distributor_attaining_res> resList = db.Queryable<daoben_distributor_attaining_res>()
                                .Where(a => a.main_id == id).ToList();

                    List<daoben_distributor_attaining_product> productList = null;
                    List<daoben_distributor_attaining_product> productRestList = null;
                    if (forEdit)
                    {   // 修改页面使用
                        productRestList = db.Queryable<daoben_product_price_effect>()
                                .JoinTable<daoben_distributor_attaining_product>((a, b) => a.model == b.model && a.color == b.color
                                && b.main_id == id)
                                .Where(a => a.is_effect == true && a.company_id == mainInfo.company_id)
                                .Where("b.id is null").OrderBy(a => a.id)
                                .Select<daoben_distributor_attaining_product>("a.id, a.model, a.color, a.price_wholesale").ToList();
                        productList = db.Queryable<daoben_product_price_effect>()
                                .JoinTable<daoben_distributor_attaining_product>((a, b) => a.model == b.model && a.color == b.color
                                && b.main_id == id)
                                .Where(a => a.is_effect == true && a.company_id == mainInfo.company_id)
                                .Where("b.id is not null").OrderBy(a => a.id)
                                .Select<daoben_distributor_attaining_product>("a.id, b.main_id, b.product_sec_i, b.model, b.color, b.price_wholesale,b.rebate_advice,b.rebate").ToList();
                    }
                    else
                    {   // 查看
                        productList = db.Queryable<daoben_distributor_attaining_product>()
                                .Where(a => a.main_id == id).ToList();
                    }

                    object resultObj = new
                    {
                        mainInfo = mainInfo,
                        approveList = approveList,
                        distributorList = distributorList,
                        productSecList = productSecList,
                        productList = productList,
                        timeSecList = timeSecList,
                        rebateList = rebateList,
                        rebateProList = rebateProList,
                        resList = resList,
                        productRestList = productRestList,
                    };
                    return resultObj.ToJson();

                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        public object GetSaleList(Pagination pagination, daoben_product_sn queryInfo, daoben_distributor_attaining queryMainInfo)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            int records = 0;
            if (queryMainInfo == null)
                return "信息错误，获取销售详情失败";
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            DateTime startTime = queryMainInfo.start_date.ToDate();
            DateTime endTime = queryMainInfo.end_date.ToDate().AddDays(1);

            string sqlSelStr = "a.phone_sn, a.model, a.color, a.{0} as name, a.sale_type,  a.{1} as time, b.outlay,b.outlay_type,a.price_wholesale, a.price_sale,a.special_offer ";
            string whereStr = "  like '%{0}%' ";
            if (queryMainInfo.target_content == 1)       // 按实销
            {
                sqlSelStr = string.Format(sqlSelStr, "sale_distributor", "sale_time");
                whereStr = "sale_distributor" + whereStr;
            }
            else                                    // 按下货
            {
                sqlSelStr = string.Format(sqlSelStr, "out_distributor", "outstorage_time");
                whereStr = "out_distributor" + whereStr;
            }
            using (var db = SugarDao.GetInstance())
            {

                var qable = db.Queryable<daoben_product_sn>()
                            .JoinTable<daoben_product_sn_outlay>((a, b) => a.phone_sn == b.phone_sn && b.category == 51)
                            .Where<daoben_product_sn_outlay>((a, b) => b.main_id == queryMainInfo.id);

                if (queryInfo != null) //匹配查询参数
                {
                    if (!string.IsNullOrEmpty(queryInfo.phone_sn))
                        qable.Where(s => s.phone_sn.Contains(queryInfo.phone_sn));
                    if (!string.IsNullOrEmpty(queryInfo.model))
                        qable.Where(s => s.model.Contains(queryInfo.model));
                }
                if (!string.IsNullOrEmpty(queryMainInfo.name))
                    qable.Where(string.Format(whereStr, queryMainInfo.name));
                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                            .Select(sqlSelStr)
                            .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public string Add(daoben_distributor_attaining addInfo, List<daoben_distributor_attaining_distributor> distributorList,
                List<daoben_distributor_attaining_product_sec> productSecList, List<daoben_distributor_attaining_product> productList,
                List<daoben_distributor_attaining_time_sec> timeSecList, List<daoben_distributor_attaining_rebate> rebateList,
                List<daoben_distributor_attaining_rebate_product> rebateProList)
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
            //if (addInfo.target_mode < 3 || addInfo.target_mode > 6)
            //    return "信息错误：考核模式错误";
            if (distributorList == null || distributorList.Count < 1)
                return "信息错误，请至少选择一个经销商!";
            if (productSecList == null || productSecList.Count < 1)
                return "信息错误，机型分段不能为空!";
            if (timeSecList == null || timeSecList.Count < 1)
                return "信息错误，时间分段不能为空!";
            //if (addInfo.product_scope < 1 || addInfo.product_scope > 2)
            //    return "信息错误：活动机型类别有误!";
            //if (addInfo.product_scope == 2 && (productList == null || productList.Count < 1))
            //    return "信息错误，机型信息不能为空!";
            //if (addInfo.target_mode != 4 && (rebateList == null || rebateList.Count < 1))
            //    return "信息错误，返利规则不能为空!";
            if (rebateList == null || rebateList.Count < 1)
                return "信息错误：返利细则不能为空";
            //List<string> tempIdList = rebateList.Select(t => t.id).ToList();
            //foreach (var a in proRebateList)
            //{
            //    a.start_date = addInfo.start_date;
            //    a.end_date = addInfo.end_date;
            //    if (!tempIdList.Contains(a.rebate_id))
            //        return "信息错误：返利规则附表中没有对应的返利表id";
            //}

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
                    addInfo.creator_position_name = myPositionInfo.name;
                    addInfo.creator_name = LoginInfo.empName;
                    addInfo.create_time = DateTime.Now;

                    db.CommandTimeOut = 30;
                    db.BeginTran();

                    db.Insert(addInfo);
                    db.DisableInsertColumns = new string[] { "id" };
                    if (distributorList.Count > 25)
                        db.SqlBulkCopy(distributorList);
                    else
                        db.InsertRange(distributorList);
                    db.InsertRange(productSecList);
                    if (productList != null && productList.Count > 0)
                        db.SqlBulkCopy(productList);
                    db.InsertRange(timeSecList);
                    db.InsertRange(rebateList);
                    if (rebateProList != null && rebateProList.Count > 0)
                        db.SqlBulkCopy(rebateProList);

                    db.CommitTran();

                    //待办事项 TODO 待测试
                    #region 待办事项 分公司总经理
                    //待办事项 收件人
                    string taskStr = "达量返利申请待审批";
                    List<string> taskIdList = db.Queryable<daoben_hr_emp_job>()
                        .Where(a => a.company_id == myCompanyInfo.id && a.position_type == ConstData.POSITION_GM2).Select<string>("id").ToList();
                    //待办事项 生成列表
                    List<daoben_sys_task> taskList = taskIdList
                        .Select(a1 => new daoben_sys_task
                        {
                            emp_id = a1,
                            category = 1,
                            main_id = addInfo.id,
                            main_url = "/DistributorManage/Attaining/Approve?id=" + addInfo.id,
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

        public string Approve(daoben_distributor_attaining_approve approveInfo)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            object upObj = null;
            daoben_distributor_attaining mainInfo;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    mainInfo = db.Queryable<daoben_distributor_attaining>().SingleOrDefault(a => a.id == approveInfo.main_id);
                    if (mainInfo == null)
                        return "信息错误：指定的申请信息不存在";
                    if ((LoginInfo.roleId == ConstData.ROLE_ID_FINANCIAL || LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER) && approveInfo.status > 0)
                    {
                        approveInfo.status = mainInfo.approve_status = 100;
                        if (mainInfo.start_date <= DateTime.Now)
                            upObj = new { approve_status = approveInfo.status, activity_status = 1 };
                        else
                            upObj = new { approve_status = approveInfo.status, activity_status = -1 };
                    }
                    else
                    {
                        if (approveInfo.status > 0)
                        {
                            mainInfo.approve_status = 1 + mainInfo.approve_status;
                            approveInfo.status = mainInfo.approve_status;
                        }
                        else
                        {
                            mainInfo.approve_status = 0 - 1 - mainInfo.approve_status;
                            approveInfo.status = mainInfo.approve_status;
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

                    db.Update<daoben_distributor_attaining>(upObj, a => a.id == mainInfo.id);
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
                    daoben_distributor_attaining attainingInfo = db.Queryable<daoben_distributor_attaining>().SingleOrDefault(a => a.id == approveInfo.main_id);
                    if (attainingInfo.approve_status == 1)
                        // TODO 待办事项 --事业部总经理
                        taskIdList = db.Queryable<daoben_hr_emp_job>()
                            .Where(a => a.position_type == ConstData.POSITION_GM1 && a.company_id == attainingInfo.company_id_parent)
                            .Select<string>("id").ToList();
                    else if (attainingInfo.approve_status == 2)
                    {
                        //财务审批
                        taskIdList = db.Queryable<daoben_hr_emp_job>()
                                    .JoinTable<daoben_ms_account>((a, b) => a.id == b.employee_id)
                                    .Where<daoben_ms_account>((a, b) => b.role_id == ConstData.ROLE_ID_FINANCIALMANAGER)
                                    .Where(a => a.company_id == attainingInfo.company_id_parent)
                                    .Select<string>("a.id as id").ToList();
                    }
                    else if (attainingInfo.approve_status != 0)
                        newsIdList = db.Queryable<daoben_hr_emp_job_history>()
                                .Where(a => a.id == attainingInfo.creator_job_history_id).Select<string>("emp_id as id").ToList();

                    taskStr = "达量返利申请待审批";
                    //待办事项 生成列表
                    if (taskIdList != null && taskIdList.Count > 0)
                    {
                        taskTotal = taskIdList
                            .Select(a1 => new daoben_sys_task
                            {
                                emp_id = a1,
                                category = 1,
                                main_id = attainingInfo.id,
                                main_url = "/DistributorManage/Attaining/Approve?id=" + attainingInfo.id,
                                title = taskStr,
                                content_abstract = attainingInfo.note,
                                recipient_type = 1,
                                create_time = now,
                                status = 1
                            }).ToList();
                    }
                    if (newsIdList != null && newsIdList.Count > 0)
                    {
                        if (attainingInfo.approve_status < 0)
                            newsStr = "达量返利申请没有通过";
                        else if (attainingInfo.approve_status == 100)
                            newsStr = "达量返利申请已通过";
                        //消息通知 生成列表
                        List<daoben_sys_notification> newsList = newsIdList
                                .Select(a1 => new daoben_sys_notification
                                {
                                    emp_id = a1,
                                    category = 2,
                                    main_id = attainingInfo.id,
                                    main_url = "/DistributorManage/Attaining/Show?id=" + attainingInfo.id,
                                    title = newsStr,
                                    content_abstract = attainingInfo.note,
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
                List<daoben_distributor_attaining> mainInfoList = new List<daoben_distributor_attaining>();
                mainInfoList.Add(mainInfo);
                Statistics(db, mainInfoList);

                return "success";
            }
        }

        public string Alter(string id, DateTime alterDate)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (string.IsNullOrEmpty(id))
                return "信息错误，操作失败!";
            daoben_distributor_attaining_alter alterInfo = new daoben_distributor_attaining_alter
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
                    daoben_distributor_attaining mainInfo = db.Queryable<daoben_distributor_attaining>().SingleOrDefault(a => a.id == id);
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
                    DateTime now = DateTime.Now;
                    int activity_status = mainInfo.activity_status;
                    if (now.Date > alterDate)
                        activity_status = 2;
                    object upObj = new
                    {
                        end_date = alterDate,
                        activity_status = activity_status
                    };
                    db.Update<daoben_distributor_attaining>(upObj, a => a.id == id);
                    db.Update<daoben_distributor_attaining_alter>(new { approve_status = 100 }, a => a.id == alterInfo.id);
                    if (activity_status < 0)
                        return "success";
                    List<daoben_distributor_attaining> mainInfoList = new List<daoben_distributor_attaining>();
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

        public string EditFull(List<daoben_distributor_attaining_product_sec> productSecList, List<daoben_distributor_attaining_product> productList,
                List<daoben_distributor_attaining_time_sec> timeSecList, List<daoben_distributor_attaining_rebate> rebateList,
                List<daoben_distributor_attaining_rebate_product> rebateProList, string orig_id)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (productSecList == null || productSecList.Count < 1)
                return "信息错误，机型分段不能为空!";
            if (timeSecList == null || timeSecList.Count < 1)
                return "信息错误，时间分段不能为空!";
            if (rebateList == null || rebateList.Count < 1)
                return "信息错误：返利细则不能为空";
            daoben_distributor_attaining_alter alterInfo = new daoben_distributor_attaining_alter
            {
                id = Common.GuId(),
                main_id = orig_id,
                alter_main_id = productSecList[0].main_id,
                orig_end_date = null,
                alter_end_date = null,
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
                    daoben_distributor_attaining mainInfo = db.Queryable<daoben_distributor_attaining>()
                                .SingleOrDefault(a => a.id == orig_id);
                    if (mainInfo == null)
                        return "信息错误：活动信息不存在!";
                    if (mainInfo.activity_status == 2)
                        return "操作失败：活动已结束!";
                    if (mainInfo.activity_status == -2)
                        return "操作失败：活动未审批通过!";

                    db.CommandTimeOut = 30;
                    db.BeginTran();

                    db.Insert(alterInfo);
                    db.DisableInsertColumns = new string[] { "id" };
                    db.InsertRange(productSecList);
                    if (productList != null && productList.Count > 0)
                        db.SqlBulkCopy(productList);
                    db.InsertRange(timeSecList);
                    db.InsertRange(rebateList);
                    if (rebateProList != null && rebateProList.Count > 0)
                        db.SqlBulkCopy(rebateProList);

                    db.CommitTran();

                    #region TODO 审批，这里暂时直接通过
                    object upObjOld = new { main_id = alterInfo.alter_main_id };    // 旧的main_id与新的交换
                    object upObjNew = new { main_id = alterInfo.main_id };
                    object upObjTmp = new { main_id = alterInfo.id };    // 中间临时值，用于交换

                    // 以下Tmp-New-Old顺序不可调换
                    db.Update<daoben_distributor_attaining_product_sec>(upObjTmp, a => a.main_id == orig_id);
                    db.Update<daoben_distributor_attaining_product_sec>(upObjNew, a => a.main_id == alterInfo.alter_main_id);
                    db.Update<daoben_distributor_attaining_product_sec>(upObjOld, a => a.main_id == alterInfo.id);

                    db.Update<daoben_distributor_attaining_product>(upObjTmp, a => a.main_id == orig_id);
                    db.Update<daoben_distributor_attaining_product>(upObjNew, a => a.main_id == alterInfo.alter_main_id);
                    db.Update<daoben_distributor_attaining_product>(upObjOld, a => a.main_id == alterInfo.id);

                    db.Update<daoben_distributor_attaining_time_sec>(upObjTmp, a => a.main_id == orig_id);
                    db.Update<daoben_distributor_attaining_time_sec>(upObjNew, a => a.main_id == alterInfo.alter_main_id);
                    db.Update<daoben_distributor_attaining_time_sec>(upObjOld, a => a.main_id == alterInfo.id);

                    db.Update<daoben_distributor_attaining_rebate>(upObjTmp, a => a.main_id == orig_id);
                    db.Update<daoben_distributor_attaining_rebate>(upObjNew, a => a.main_id == alterInfo.alter_main_id);
                    db.Update<daoben_distributor_attaining_rebate>(upObjOld, a => a.main_id == alterInfo.id);

                    db.Update<daoben_distributor_attaining_rebate_product>(upObjTmp, a => a.main_id == orig_id);
                    db.Update<daoben_distributor_attaining_rebate_product>(upObjNew, a => a.main_id == alterInfo.alter_main_id);
                    db.Update<daoben_distributor_attaining_rebate_product>(upObjOld, a => a.main_id == alterInfo.id);
                    db.Update<daoben_distributor_attaining_alter>(new { approve_status = 100 }, a => a.id == alterInfo.id);

                    List<daoben_distributor_attaining> mainInfoList = new List<daoben_distributor_attaining>();
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

        public void Statistics(SqlSugarClient db, List<daoben_distributor_attaining> mainInfoList)
        {
            if (mainInfoList == null || mainInfoList.Count < 1)
                return;
            DateTime now = DateTime.Now;
            StringBuilder sqlUpSb1 = new StringBuilder("insert into daoben_distributor_attaining_distributor (`id`,");
            sqlUpSb1.Append("`total_count`, `total_normal_count`, `total_amount`,`total_ratio`,`total_rebate`) values ");

            // StringBuilder sqlUpSb2 = new StringBuilder("insert into daoben_activity_pk () values ");
            StringBuilder sqlUpSb3 = new StringBuilder("insert into daoben_distributor_attaining (`id`, `total_sale_count`,");
            sqlUpSb3.Append("`total_sale_amount`, `total_rebate`, `counting_time`) values ");
            bool toUp1 = false, toUp3 = false;
            
            List<daoben_product_sn_outlay> snRebateList = new List<daoben_product_sn_outlay>();
            // 下列SQL片段中s表示daoben_product_sn
            string sqlJoinD = " LEFT JOIN daoben_distributor_attaining_distributor d ON {0}=d.distributor_id AND {1}>='{2}' AND {1}<'{3}' AND s.sale_type>{4} ";
            string sqlJoinP = " LEFT JOIN daoben_distributor_attaining_product p ON s.model=p.model AND s.color=p.color ";
            string sqlSelD = " s.{0}sale_distributor_id ";
            string sqlSelT = " s.{0}sale_time ";
            string sqlSelA = " s.{0} as refund_amount "; // Amount, 占用补差总额字段存放批发价（按下货）/零售价（按实销）

            foreach (var mainInfo in mainInfoList)
            {
                bool toExclude = false;
                int outlayType = 0, totalCount = 0;
                string sqlStr = null;
                List<daoben_product_sn> fullSnList = null, secSnList = null;
                List<daoben_distributor_attaining_product_sec> productSecList = db.Queryable<daoben_distributor_attaining_product_sec>()
                                .Where(a => a.main_id == mainInfo.id).ToList();
                List<daoben_distributor_attaining_time_sec> timeSecList = db.Queryable<daoben_distributor_attaining_time_sec>()
                            .Where(a => a.main_id == mainInfo.id).ToList();
                List<daoben_distributor_attaining_distributor> distributorList = db.Queryable<daoben_distributor_attaining_distributor>()
                                .Where(a => a.main_id == mainInfo.id)
                                .Select("id, distributor_id").ToList(); // 返利金额重置为0
                List<daoben_distributor_attaining_product> productList = db.Queryable<daoben_distributor_attaining_product>()
                                .Where(a => a.main_id == mainInfo.id).ToList();
                List<daoben_distributor_attaining_rebate_product> rebateProList = db.Queryable<daoben_distributor_attaining_rebate_product>()
                                .Where(a => a.main_id == mainInfo.id).ToList();
                if (productSecList.Count < 1 || timeSecList.Count < 1)
                    continue;
                DateTime startDate = mainInfo.start_date.ToDate();
                DateTime endDate = mainInfo.end_date > now ? now : mainInfo.end_date.ToDate().AddDays(1);
                #region 预生成SQL片段，针对“按下货”/“按实销”
                string curSqlJoinD = null, curSqlSelD = null, curSqlSelT = null, curSqlSelA = null;
                if (mainInfo.target_content == 1)       // 按实销
                {
                    curSqlJoinD = string.Format(sqlJoinD, "s.sale_distributor_id", "s.sale_time", startDate, endDate, "0");
                    curSqlSelD = string.Format(sqlSelD, "");
                    curSqlSelT = string.Format(sqlSelT, "");
                    curSqlSelA = string.Format(sqlSelA, "price_sale");
                    outlayType = 1;
                }
                else                                    // 按下货
                {
                    curSqlJoinD = string.Format(sqlJoinD, "s.out_distributor_id", "s.outstorage_time", startDate, endDate, "-1");
                    curSqlSelD = string.Format(sqlSelD, "out_distributor_id as ");
                    curSqlSelT = string.Format(sqlSelT, "outstorage_time as ");
                    curSqlSelA = string.Format(sqlSelA, "price_wholesale");
                    outlayType = 2;
                }
                #endregion

                #region 获取所有串码与总数
                // 总销量
                if (productSecList[0].product_scope == 1)
                {   // 有“全部机型”的政策，总量为全部机型，返利金额再分开算
                    sqlStr = "select SQL_CALC_FOUND_ROWS s.phone_sn, s.model, s.color," + curSqlSelD
                                + ", s.sale_type, s.special_offer, " + curSqlSelT + ", s.price_wholesale, s.price_sale, " + curSqlSelA
                                + " from daoben_product_sn s "
                                + curSqlJoinD + string.Format("where d.main_id='{0}'", mainInfo.id);
                    fullSnList = db.SqlQuery<daoben_product_sn>(sqlStr).ToList();
                    totalCount = db.SqlQuery<int>("SELECT FOUND_ROWS() as count").FirstOrDefault();
                    if (productSecList.Count > 1)
                        toExclude = true;
                    if (totalCount == 0)
                    {
                        #region 无销售，重置所有结果并返回 (避免“指定机型”再次查询数据库)
                        foreach (var d in distributorList)
                        {
                            sqlUpSb1.AppendFormat("({0},{1},{2},{3},{4},{5}),", d.id, 0, 0, 0, 0, 0);
                            toUp1 = true;
                        }
                        sqlUpSb3.AppendFormat("('{0}',{1},{2},{3},'{4}'),", mainInfo.id, 0, 0, 0, endDate.AddDays(-1));
                        toUp3 = true;
                        continue;
                        #endregion
                    }
                }
                if (productSecList.Count > 1 || productSecList[0].product_scope == 2) // 有指定机型的情况存在
                {
                    // 有分段“指定机型”的政策，查出所有指定机型，tmpInt为分段index
                    sqlStr = "select SQL_CALC_FOUND_ROWS p.product_sec_i as tmpInt, s.phone_sn, s.model, s.color," + curSqlSelD
                                + ", s.sale_type, s.special_offer, " + curSqlSelT + ", s.price_wholesale, s.price_sale, " + curSqlSelA
                                + " from daoben_product_sn s " + curSqlJoinD + sqlJoinP
                                + string.Format("where d.main_id='{0}' AND p.main_id='{0}' ", mainInfo.id);
                    secSnList = db.SqlQuery<daoben_product_sn>(sqlStr).ToList();

                    if (totalCount == 0) // 没有“全部机型”的政策
                        totalCount = db.SqlQuery<int>("SELECT FOUND_ROWS() as count").FirstOrDefault();
                }
                if (totalCount == 0)
                {
                    #region 无销售，重置所有结果并返回
                    foreach (var d in distributorList)
                    {
                        sqlUpSb1.AppendFormat("({0},{1},{2},{3},{4},{5}),", d.id, 0, 0, 0, 0, 0);
                        toUp1 = true;
                    }
                    sqlUpSb3.AppendFormat("('{0}',{1},{2},{3},'{4}'),", mainInfo.id, 0, 0, 0, endDate.AddDays(-1));
                    toUp3 = true;
                    continue;
                    #endregion
                }

                #endregion

               

                if (mainInfo.pay_mode == 2)     // 按月发放
                {
                    //DateTime endDate1 = mainInfo.end_date.ToDate().AddDays(1);
                    //DateTime selEnd = mainInfo.start_date.ToDate();
                    //selEnd = selEnd.AddDays(1 - selEnd.Day).AddMonths(1); // 按月查询的结束日
                    //DateTime selStart = mainInfo.start_date.ToDate();  // 按月查询的起始日
                    //while (true)
                    //{
                    //    if (selStart >= now)
                    //        break;
                        //    #region 获取所有串码与总数
                        //    // 总销量
                        //    if (productSecList[0].product_scope == 1)
                        //    {   // 有“全部机型”的政策，总量为全部机型，返利金额再分开算
                        //        sqlStr = "select SQL_CALC_FOUND_ROWS s.phone_sn, s.model, s.color," + curSqlSelD
                        //                    + ", s.sale_type, " + curSqlSelT + ", s.price_wholesale, s.price_sale, " + curSqlSelA
                        //                    + " from daoben_product_sn s "
                        //                    + curSqlJoinD + string.Format("where d.main_id='{0}'", mainInfo.id);
                        //        fullSnList = db.SqlQuery<daoben_product_sn>(sqlStr).ToList();
                        //        totalCount = db.SqlQuery<int>("SELECT FOUND_ROWS() as count").FirstOrDefault();
                        //        if (productSecList.Count > 1)
                        //            toExclude = true;
                        //        if (totalCount == 0)
                        //        {
                        //            #region 无销售，重置所有结果并返回 (避免“指定机型”再次查询数据库)
                        //            foreach (var d in distributorList)
                        //            {
                        //                sqlUpSb1.AppendFormat("({0},{1},{2},{3},{4},{5}),", d.id, 0, 0, 0, 0, 0);
                        //                toUp1 = true;
                        //            }
                        //            sqlUpSb3.AppendFormat("('{0}',{1},{2},{3},'{4}'),", mainInfo.id, 0, 0, 0, endDate.AddDays(-1));
                        //            toUp3 = true;
                        //            continue;
                        //            #endregion
                        //        }
                        //    }
                        //    if (productSecList.Count > 1 || productSecList[0].product_scope == 2) // 有指定机型的情况存在
                        //    {
                        //        // 有分段“指定机型”的政策，查出所有指定机型，tmpInt为分段index
                        //        sqlStr = "select SQL_CALC_FOUND_ROWS p.product_sec_i as tmpInt, s.phone_sn, s.model, s.color," + curSqlSelD
                        //                    + ", s.sale_type, " + curSqlSelT + ", s.price_wholesale, s.price_sale, " + curSqlSelA
                        //                    + " from daoben_product_sn s " + curSqlJoinD + sqlJoinP
                        //                    + string.Format("where d.main_id='{0}' AND p.main_id='{0}' ", mainInfo.id);
                        //        secSnList = db.SqlQuery<daoben_product_sn>(sqlStr).ToList();

                        //        if (totalCount == 0) // 没有“全部机型”的政策
                        //            totalCount = db.SqlQuery<int>("SELECT FOUND_ROWS() as count").FirstOrDefault();
                        //    }
                        //    if (totalCount == 0)
                        //    {
                        //        #region 无销售，重置所有结果并返回
                        //        foreach (var d in distributorList)
                        //        {
                        //            sqlUpSb1.AppendFormat("({0},{1},{2},{3},{4},{5}),", d.id, 0, 0, 0, 0, 0);
                        //            toUp1 = true;
                        //        }
                        //        sqlUpSb3.AppendFormat("('{0}',{1},{2},{3},'{4}'),", mainInfo.id, 0, 0, 0, endDate.AddDays(-1));
                        //        toUp3 = true;
                        //        continue;
                        //        #endregion
                        //    }

                        //    #endregion
                        //    daoben_product_sn monthSnList =
                        //    daoben_distributor_image_res resInfo = db.Queryable<daoben_product_sn>()
                        //                .Where(a => a.sale_type > -1).Where(whereStr)
                        //                .Select<daoben_distributor_image_res>("count(*) as total_sale_count, sum(price_wholesale) as total_sale_amount")
                        //                .SingleOrDefault();
                        //    if (resInfo == null)
                        //        continue;
                        //    resInfo.main_id = image.id;
                        //    resInfo.month = selStart.AddDays(1 - selStart.Day);
                        //    resInfo.start_date = selStart;
                        //    resInfo.end_date = selEnd;
                        //    resInfo.rebate = resInfo.total_sale_count >= image.activity_target ? image.rebate : 0;
                        //    resList.Add(resInfo);
                    //    if (selEnd >= endDate1)
                    //        break;
                    //    selStart = selEnd;
                    //    selEnd = selEnd.AddMonths(1) > endDate1 ? endDate1 : selEnd.AddMonths(1);
                    //}
                }
                else    // 一次性发放
                {
                    List<daoben_product_sn> curSnList = null;
                    List<daoben_distributor_attaining_rebate_product> curRebProList = null;
                    // 完成率 / 按台数
                    int finTarget = (mainInfo.target_sale == 1) ? (totalCount * 100 / mainInfo.activity_target) : totalCount;
                    bool attaining = (mainInfo.target_sale == 1) ? true : (totalCount >= mainInfo.activity_target);
                    List<daoben_distributor_attaining_rebate> curRebateList = db.Queryable<daoben_distributor_attaining_rebate>()
                                .Where(a => a.target_min <= finTarget && (a.target_max >= finTarget || a.target_max == -1)
                                && a.main_id == mainInfo.id).OrderBy("time_sec_i asc").ToList();    // 当前对应的销量/完成率

                    int proSecI = 0;
                    for (int i = 0; i < curRebateList.Count; i++)
                    {
                        #region 当前对应的销量/完成率(每个信息对应一个时间分段)
                        var timeSecInfo = timeSecList.Where(a => a.time_sec_i == curRebateList[i].time_sec_i).SingleOrDefault();
                        if (timeSecInfo == null)
                            continue;
                        proSecI = timeSecInfo.product_sec_i;
                        if (proSecI > 1) // 之后不可能存在需要排除的情况
                            toExclude = false;

                        DateTime timeSecEndD = timeSecInfo.end_date.ToDate().AddDays(1);
                        if (proSecI == 1 && productSecList[0].product_scope == 1) // 全部机型
                            curSnList = fullSnList;
                        else
                            curSnList = secSnList;
                        if (timeSecInfo.rebate_mode == 4) // 固定总金额 且 返利模式“无”
                        {
                            decimal rebAmount = attaining ? curRebateList[i].rebate : 0;    // 不达量时返利为0
                            #region 每个串码的返利金额
                            List<daoben_product_sn_outlay> curSnRList = new List<daoben_product_sn_outlay>();
                            foreach (var snInfo in curSnList)
                            {
                                if (toExclude)
                                {
                                    if (productList.Exists(a => (a.model == snInfo.model && a.color == snInfo.color))
                                            || snInfo.sale_time < timeSecInfo.start_date || snInfo.sale_time >= timeSecEndD)
                                        continue;       // 需排除的型号或时间不在分段范围的
                                }
                                else if ((snInfo.tmpInt > 0 && snInfo.tmpInt != proSecI)
                                            || snInfo.sale_time < timeSecInfo.start_date || snInfo.sale_time >= timeSecEndD)
                                    continue;           // 不在该机型分段，或时间不在时间分段范围的
                                if (snInfo.sale_type > 1 || (snInfo.special_offer && mainInfo.money_included == 0)) // 买断/包销/特价
                                    continue;
                                curSnRList.Add(new daoben_product_sn_outlay()
                                {
                                    category = 51,
                                    main_id = mainInfo.id,
                                    phone_sn = snInfo.phone_sn,
                                    outlay_type = outlayType
                                });
                            }
                            decimal outlay = rebAmount / curSnList.Count;
                            curSnRList.ForEach(a => a.outlay = outlay);
                            snRebateList.AddRange(curSnRList);
                            #endregion
                            distributorList.ForEach(a => a.total_rebate += rebAmount);

                            continue;
                        }
                        curRebProList = rebateProList.Where(a => a.rebate_i == curRebateList[i].rebate_i)
                                    .OrderBy("target_min", OrderByType.Asc).ToList(); // 按最低值升序
                        foreach (var snInfo in curSnList)
                        {
                            #region 串码列表
                            if (toExclude)
                            {
                                if (productList.Exists(a => (a.model == snInfo.model && a.color == snInfo.color))
                                        || snInfo.sale_time < timeSecInfo.start_date || snInfo.sale_time >= timeSecEndD)
                                    continue;       // 需排除的型号或时间不在分段范围的
                            }
                            else if ((snInfo.tmpInt > 0 && snInfo.tmpInt != proSecI)
                                        || snInfo.sale_time < timeSecInfo.start_date || snInfo.sale_time >= timeSecEndD)
                                continue;           // 不在该机型分段，或时间不在时间分段范围的
                            if (snInfo.sale_type > 1 || (snInfo.special_offer && mainInfo.money_included == 0)) // 买断/包销/特价
                            {
                                snRebateList.Add(new daoben_product_sn_outlay()  // 0返利的串码也保存
                                {
                                    category = 51,
                                    main_id = mainInfo.id,
                                    phone_sn = snInfo.phone_sn,
                                    outlay = 0,
                                    outlay_type = outlayType,
                                });
                                continue;
                            }

                            daoben_distributor_attaining_distributor dInfo = distributorList
                                        .Where(a => a.distributor_id == snInfo.sale_distributor_id).SingleOrDefault();
                            decimal rebateFactor = 0, outlay = 0;   // 系数， rebate * rebateFactor = 返利值
                            if (attaining)   // 不达量时返利为0
                            {
                                if (timeSecInfo.rebate_mode == 1)       // 1-每台固定金额；
                                    rebateFactor = 1;
                                else if (timeSecInfo.rebate_mode == 2)  // 2-每台批发价比例；
                                    rebateFactor = snInfo.price_wholesale / 100;
                                else                                    // 3-每台零售价比例
                                    rebateFactor = snInfo.price_sale / 100;
                            }
                            if (timeSecInfo.target_mode == 6)    // 无，此时返利金额存放在RebateList，不在RebateProductList中
                            {
                                outlay = curRebateList[i].rebate * rebateFactor;
                                dInfo.total_rebate += outlay;
                                snRebateList.Add(new daoben_product_sn_outlay()
                                {
                                    category = 51,
                                    main_id = mainInfo.id,
                                    phone_sn = snInfo.phone_sn,
                                    outlay = outlay,
                                    outlay_type = outlayType,
                                });
                                continue;
                            }
                            foreach (var rebPro in curRebProList)
                            {
                                #region 返利方案列表
                                if (timeSecInfo.target_mode == 3) // 按零售价
                                {
                                    if (snInfo.price_sale >= rebPro.target_max && rebPro.target_max > 0)
                                        continue;
                                }
                                else if (timeSecInfo.target_mode == 5) // 按批发价
                                {
                                    if (snInfo.price_wholesale >= rebPro.target_max)
                                        continue;
                                }
                                else if (timeSecInfo.target_mode == 4) // 按型号
                                {
                                    if (snInfo.model != rebPro.model || snInfo.color != rebPro.color)
                                        continue;
                                } // else 无，无需处理
                                outlay = rebPro.rebate * rebateFactor;
                                dInfo.total_rebate += outlay;
                                snRebateList.Add(new daoben_product_sn_outlay()
                                {
                                    category = 51,
                                    main_id = mainInfo.id,
                                    phone_sn = snInfo.phone_sn,
                                    outlay = outlay,
                                    outlay_type = outlayType,
                                });
                                break;
                                #endregion
                            }
                            #endregion
                        }
                        #endregion
                    }
                    curSnList = fullSnList == null ? secSnList : fullSnList;    // 有“全部机型”的政策，总量为全部机型
                    foreach (var d in distributorList)
                    {
                        #region 数据库操作语句-经销商
                        var dSnList = curSnList.Where(a => a.sale_distributor_id == d.distributor_id).ToList();
                        d.total_count = dSnList.Count();
                        d.total_amount = dSnList.Sum(a => a.refund_amount);// 借用字段，“按下货”时为批发价，“按实销”时为零售价
                        d.total_ratio = d.total_count * 100 / mainInfo.activity_target;
                        if (mainInfo.money_included == 0)
                            d.total_normal_count = dSnList.Where(a => a.sale_type < 2 && a.special_offer == false).Count();
                        else
                            d.total_normal_count = dSnList.Where(a => a.sale_type < 2).Count();
                        sqlUpSb1.AppendFormat("({0},{1},{2},{3},{4},{5}),", d.id, d.total_count, d.total_normal_count, d.total_amount,
                                     d.total_ratio, d.total_rebate);
                        toUp1 = true;
                        #endregion
                    }
                    sqlUpSb3.AppendFormat("('{0}',{1},{2},{3},'{4}'),", mainInfo.id,
                                distributorList.Sum(a => a.total_count), distributorList.Sum(a => a.total_amount),
                                distributorList.Sum(a => a.total_rebate), endDate.AddDays(-1));
                    toUp3 = true;
                }
            }
            if (toUp1)
            {
                sqlUpSb1.Remove(sqlUpSb1.Length - 1, 1); // 最后一个逗号
                sqlUpSb1.Append("on duplicate key update total_count=values(total_count),total_normal_count=values(total_normal_count),");
                sqlUpSb1.Append("total_amount =values(total_amount),total_ratio=values(total_ratio),total_rebate=values(total_rebate);");
                db.SqlQuery<int>(sqlUpSb1.ToString());
            }
            if (toUp3)
            {
                sqlUpSb3.Remove(sqlUpSb3.Length - 1, 1); // 最后一个逗号
                sqlUpSb3.Append("on duplicate key update total_sale_count=values(total_sale_count),total_sale_amount=values(total_sale_amount),");
                sqlUpSb3.Append("total_rebate =values(total_rebate),counting_time=values(counting_time);");
                db.SqlQuery<int>(sqlUpSb3.ToString());
            }
            List<string> idList = mainInfoList.Select(a => a.id).ToList<string>();
            db.Delete<daoben_product_sn_outlay>(a => a.category == 51 && idList.Contains(a.main_id));
            if (snRebateList.Count > 0)
                db.SqlBulkCopy(snRebateList);
        }
        public void ApproveTmp(string id)
        {
            using (var db = SugarDao.GetInstance())
            {
                daoben_distributor_attaining mainInfo = db.Queryable<daoben_distributor_attaining>().SingleOrDefault(a => a.id == id);
                DateTime now = DateTime.Now;
                int activity_status = -1;
                if (now.Date > mainInfo.end_date)
                    activity_status = 2;
                else if (now >= mainInfo.start_date)
                    activity_status = 1;

                object upObj = new
                {
                    approve_status = 100,
                    activity_status = activity_status
                };
                db.Update<daoben_distributor_attaining>(upObj, a => a.id == id);
                if (activity_status < 0)
                    return;
                List<daoben_distributor_attaining> mainInfoList = new List<daoben_distributor_attaining>();
                mainInfoList.Add(mainInfo);
                Statistics(db, mainInfoList);
            }

        }

    }
}
