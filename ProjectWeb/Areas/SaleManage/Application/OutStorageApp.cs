using Base.Code;
using MySqlSugar;
using ProjectShare.Database;
using ProjectShare.Process;
using System;
using System.Linq;
using ProjectWeb.Application;
using System.Collections.Generic;
using ProjectShare.Models;
using System.Text;
using System.Threading;
using ProjectWeb.Areas.DistributorManage.Application;

namespace ProjectWeb.Areas.SaleManage.Application
{
    public class OutStorageApp
    {
        public object GetList(Pagination pagination, daoben_sale_outstorage queryInfo, QueryTime queryTime)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");

            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "b.create_time" : pagination.sidx;//改用时间
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_sale_outstorage>()
                        .JoinTable<daoben_sale_outstorage_approve>((a, b) => a.import_file_id == b.id);
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER || LoginInfo.roleId == ConstData.ROLE_ID_FINANCIAL)
                        qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                    else
                        qable.Where(a => a.company_id == myCompanyInfo.id);
                }
                if (queryInfo != null)
                {
                    if (!string.IsNullOrEmpty(queryInfo.phone_sn))
                        qable.Where(a => a.phone_sn.Contains(queryInfo.phone_sn));
                    if (!string.IsNullOrEmpty(queryInfo.distributor_name))
                        qable.Where(a => a.distributor_name.Contains(queryInfo.distributor_name));
                    if (!string.IsNullOrEmpty(queryInfo.model))
                        qable.Where(a => a.model.Contains(queryInfo.model));
                    if (queryInfo.company_id > 0)
                        qable.Where(a => a.company_id == queryInfo.company_id);
                    if (queryInfo.area_l2_id > 0)
                        qable.Where(a => a.area_l2_id == queryInfo.area_l2_id || a.area_l1_id == queryInfo.area_l2_id); //区域搜索条件
                    if (!string.IsNullOrEmpty(queryInfo.import_file_id))
                        qable.Where<daoben_sale_outstorage_approve>((a, b) => b.import_file.Contains(queryInfo.import_file_id));
                }
                if (queryTime != null)
                {
                    if (queryTime.startTime1 != null)
                        qable.Where<daoben_sale_outstorage_approve>((a, b) => a.accur_time >= queryTime.startTime1);
                    if (queryTime.startTime2 != null)
                    {
                        queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                        qable.Where<daoben_sale_outstorage_approve>((a, b) => a.accur_time < queryTime.startTime2);
                    }
                }

                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .Select("a.*,b.creator_name,b.import_file,b.create_time")
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }
        public object GetListHistory(Pagination pagination, string queryName, QueryTime queryTime)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");

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
                var qable = db.Queryable<daoben_sale_outstorage_approve>();//历史判定
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (myCompanyInfo.category == "事业部")
                        qable.Where(a => a.company_id == myCompanyInfo.id);
                    else if (myCompanyInfo.category == "分公司")
                        qable.Where(a => a.company_id == myCompanyInfo.parentId);
                }
                if (!string.IsNullOrEmpty(queryName))
                    qable.Where(a => a.import_file.Contains(queryName));
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

                object list = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .ToPageList(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                return list;
            }
        }

        public object GetListApprove(Pagination pagination, string queryName)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");

            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "create_time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_sale_outstorage_approve>().Where(a => a.status == 0);
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIAL || LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER)
                        qable.Where(a => a.company_id == myCompanyInfo.id);
                    else return null;
                }

                if (!string.IsNullOrEmpty(queryName))
                    qable.Where(a => a.import_file.Contains(queryName));
                object list = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .ToPageList(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                return list;
            }
        }
        public string Import(List<daoben_sale_outstorage> importList, daoben_sale_outstorage_approve importInfo, ref int delCount)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (importInfo == null)
                return "信息错误，操作失败!";
            if (importList == null || importList.Count < 1)
                return "信息错误：详情列表不能为空!";
            if (string.IsNullOrEmpty(importInfo.id) || importInfo.id.Length != 36)
                return "信息错误：ID不正确!";

            List<string> dupList = importList.GroupBy(a => a.phone_sn).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            if (dupList != null && dupList.Count > 0)
                return "信息错误：导入表中串码重复：" + dupList.ToJson();

            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            DateTime now = DateTime.Now;
            importInfo.creator_job_history_id = LoginInfo.jobHistoryId;
            importInfo.creator_name = LoginInfo.empName;
            importInfo.create_time = DateTime.Now;
            importInfo.status = 0;
            importInfo.company_id = myCompanyInfo.id;
            importInfo.company_name = myCompanyInfo.name;

            List<string> snList = importList.Select(a => a.phone_sn).ToList();
            DateTime maxDate = importList.Max(a => a.accur_time).ToDate();
            DateTime threeMAgo = maxDate.AddMonths(-3); // 只检查3个月内的数据
            StringBuilder sqlUpSb = new StringBuilder("insert into daoben_sale_outstorage (`id`, `check_status`) values ");
            int errCount = 0;
            using (var db = SugarDao.GetInstance())
            {
                // TODO 待办事项 --财务文员+财务经理 待测试
                #region 待办事项 财务
                //待办事项 收件人
                string tempStr = "出库信息" + "待审批";
                List<string> idList = db.Queryable<daoben_hr_emp_job>()
                    .JoinTable<daoben_ms_account>((a, b) => a.id == b.employee_id)
                    .Where<daoben_ms_account>((a, b) => b.role_id == ConstData.ROLE_ID_FINANCIALMANAGER)
                    .Where(a => a.company_id == importInfo.company_id)
                    .Select<string>("a.id as id").ToList();
                //待办事项 生成列表
                List<daoben_sys_task> taskList = idList
                    .Select(a1 => new daoben_sys_task
                    {
                        emp_id = a1,
                        category = 1,
                        main_id = importInfo.id,
                        main_url = "/SaleManage/OutStorage/Approve?id=" + importInfo.id,
                        title = tempStr,
                        content_abstract = importInfo.import_file,
                        recipient_type = 1,
                        create_time = now,
                        status = 1
                    }).ToList();
                #endregion

                db.CommandTimeOut = 300;
                try
                {
                    #region 数据状态
                    List<daoben_sale_outstorage> errList = db.Queryable<daoben_sale_outstorage>()
                            .Where(a => a.accur_time > threeMAgo && a.check_status == 1 && snList.Contains(a.phone_sn))
                            .ToList();
                    if (errList != null && errList.Count > 1)
                    {
                        int delCount1 = 0;
                        errList.ForEach(e =>
                        {
                            daoben_sale_outstorage data = importList.Where(a => a.phone_sn == e.phone_sn).SingleOrDefault();
                            if (data != null)
                            {
                                if (data.model == e.model && data.color == e.color
                                            && data.distributor_id == e.distributor_id && data.accur_time == e.accur_time)
                                {
                                    importList.Remove(data);
                                    delCount1++;
                                }
                                else
                                {
                                    sqlUpSb.AppendFormat("({0},{1}),", e.id, 21);   // 21-相同串码不同信息
                                    data.check_status = 21;
                                    errCount++;
                                }
                            }
                        });
                        delCount = delCount1;
                        if (errCount > 0)
                        {
                            sqlUpSb.Remove(sqlUpSb.Length - 1, 1); // 最后一个逗号
                            sqlUpSb.Append("on duplicate key update check_status=values(check_status);");
                        }
                        if (importList.Count == 0)
                            return "导入失败：重复导入数据";
                    }
                    #endregion

                    db.BeginTran();
                    if (errCount > 0)  // 异常数据
                        db.SqlQuery<int>(sqlUpSb.ToString());
                    db.Insert(importInfo);
                    db.DisableInsertColumns = new string[] { "id" };
                    if (importList.Count > 25)
                        db.SqlBulkCopy(importList);
                    else
                        db.InsertRange(importList);

                    if (taskList != null && taskList.Count > 25)
                        db.SqlBulkCopy(taskList);
                    else if (taskList != null && taskList.Count > 0)
                        db.InsertRange(taskList);
                    db.CommitTran();
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return "系统出错：" + ex.Message;
                }
            }
            return "success";
        }

        public string Approve(daoben_sale_outstorage_approve approveInfo)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            daoben_sys_cron cronInfo = null;

            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_sale_outstorage_approve origInfo = db.Queryable<daoben_sale_outstorage_approve>().InSingle(approveInfo.id);
                    if (origInfo == null)
                        return "信息错误：指定的申请信息不存在";
                    bool errData = db.Queryable<daoben_sale_outstorage>().Any(a => a.import_file_id == approveInfo.id && a.check_status == 21);
                    if (errData)
                        return "操作失败：存在未处理的异常数据，请先处理";
                    if (approveInfo.status > 0)
                        approveInfo.status = 100;   // 以100作为审批完成的标志
                    else
                        approveInfo.status = -100;
                    object upObj = new
                    {
                        status = approveInfo.status,
                        approve_note = approveInfo.approve_note,
                        approve_id = LoginInfo.accountId,
                        approve_name = LoginInfo.empName,
                        approve_position_id = myPositionInfo.id,
                        approve_position_name = myPositionInfo.name,
                        approve_time = DateTime.Now
                    };
                    //TODO 消息通知——通知申请者 待测试
                    #region 消息通知 创建人
                    //消息通知 收件人id列表
                    string tempStr = null;
                    tempStr = "出库信息导入审批" + (approveInfo.status == 100 ? "通过" : "不通过");
                    List<string> idList = db.Queryable<daoben_hr_emp_job_history>()
                        .Where(a => a.company_id == myCompanyInfo.id)
                        .Where(a => a.id == origInfo.creator_job_history_id).Select<string>("emp_id as id").ToList();
                    //消息通知 生成列表
                    List<daoben_sys_notification> newsList = idList
                        .Select(a1 => new daoben_sys_notification
                        {
                            emp_id = a1,
                            category = 2,
                            main_id = origInfo.id,
                            main_url = "/SaleManage/OutStorage/Show?id=" + origInfo.id,
                            title = tempStr + "\t",
                            content_abstract = origInfo.import_file,
                            recipient_type = 1,
                            create_time = now,
                            status = 1
                        }).ToList();
                    #endregion
                    db.CommandTimeOut = 300;
                    db.BeginTran();
                    db.Update<daoben_sale_outstorage_approve>(upObj, a => a.id == approveInfo.id);
                    if (approveInfo.status == -100)  // 将check_status也置为-100
                        db.Update<daoben_sale_outstorage>(new { check_status = -100 }, a => a.import_file_id == approveInfo.id);
                    db.DisableInsertColumns = new string[] { "id" };
                    if (cronInfo != null)
                        db.Insert(cronInfo);
                    //代办事项 置为 已处理
                    List<daoben_sys_task> origTask = db.Queryable<daoben_sys_task>().Where(a => a.emp_id == LoginInfo.empId && a.main_id == approveInfo.id).ToList();
                    object upObjForTask = new
                    {
                        status = 3,
                        finished_time = now
                    };
                    if (origTask != null)
                        db.Update<daoben_sys_task>(upObjForTask, a => a.main_id == approveInfo.id);
                    if (newsList != null && newsList.Count > 25)
                        db.SqlBulkCopy(newsList);
                    else if (newsList != null && newsList.Count > 0)
                        db.InsertRange(newsList);

                    db.CommitTran();
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return "系统出错：" + ex.Message;
                }
            }
            if (approveInfo.status == 100)
                ThreadPool.QueueUserWorkItem(t =>
                {
                    Statistics(approveInfo.id);
                    SettlementApp settlement = new SettlementApp();
                    settlement.Rebate(approveInfo.id, 1);
                });
            return "success";
        }
        public string GetInfoMain(string id)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    string mainInfoStr = db.Queryable<daoben_sale_outstorage_approve>()
                            .JoinTable<daoben_hr_emp_job_history>((a, b) => a.creator_job_history_id == b.id)
                            .Where<daoben_hr_emp_job_history>((a, b) => a.id == id)
                            .Select("a.*, b.position_name as creator_position_name").ToJson();
                    return mainInfoStr;

                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        public object GetInfoPage(Pagination pagination, string id, string queryStr)
        {
            if (string.IsNullOrEmpty(id))
                return "信息错误：ID不能为空";

            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "accur_time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    var qable = db.Queryable<daoben_sale_outstorage>().Where(a => a.import_file_id == id);
                    if (!string.IsNullOrEmpty(queryStr))
                        qable.Where(a => a.distributor_name.Contains(queryStr) || a.model.Contains(queryStr) || a.phone_sn.Contains(queryStr));

                    object list = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                            .ToPageList(pagination.page, pagination.rows, ref records);
                    pagination.records = records;
                    return list;
                }
                catch (Exception ex)
                {
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
                    daoben_sale_outstorage_approve mainInfo = db.Queryable<daoben_sale_outstorage_approve>().SingleOrDefault(a => a.id == id);
                    if (mainInfo == null || mainInfo.status != 0)
                        //财务经理审批过后，将不能撤回
                        return "撤回失败：指定的申请信息不存在或者已被审批";
                    else
                    {
                        db.CommandTimeOut = 30;
                        db.BeginTran();
                        db.Delete<daoben_sale_outstorage_approve>(a => a.id == id);
                        db.Delete<daoben_sale_outstorage>(a => a.import_file_id == id);
                        //删除所有待办事项
                        db.Delete<daoben_sys_task>(a => a.main_id == id);
                        db.Delete<daoben_sys_notification>(a => a.main_id == id);
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


        // UpdateCheckStatus TODO 作废
        public string UpdateCheckStatus(List<int> idList, int status)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (idList == null || idList.Count < 1)
                return "信息错误,指定的数据不存在!";
            if (status != 1 && status != 2)
                return "信息错误,操作类型错误!";
            using (var db = SugarDao.GetInstance())
            {
                StringBuilder sqlUpSb = new StringBuilder("UPDATE daoben_sale_outstorage SET check_status = check_status + ");
                sqlUpSb.AppendFormat("{0} WHERE ID IN(", status.ToString());
                foreach (var a in idList)
                {
                    sqlUpSb.Append("'" + a.ToString() + "',");
                }
                sqlUpSb.Remove(sqlUpSb.Length - 1, 1);
                sqlUpSb.Append(");");
                db.SqlQuery<int>(sqlUpSb.ToString());
            }
            return "success";
        }

        public string Keeping(int id)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");

            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_sale_outstorage origInfo = db.Queryable<daoben_sale_outstorage>()
                            .JoinTable<daoben_sale_outstorage_approve>((a, b) => a.import_file_id == b.id)
                            .Where(a => a.id == id)
                            .Select("a.phone_sn, b.status as check_status").SingleOrDefault(); // 占用check_status作为审批状态
                    if (origInfo == null)
                        return "信息错误,指定的数据不存在!";

                    db.CommandTimeOut = 30;
                    db.BeginTran();

                    db.Update<daoben_sale_outstorage>(new { check_status = 23 }, a => a.phone_sn == origInfo.phone_sn);
                    db.Update<daoben_sale_outstorage>(new { check_status = 1 }, a => a.id == id);
                    if (origInfo.check_status != 100)   // 保留的信息是未审批通过的，则要删除的信息可能已存在串码信息表
                        db.Delete<daoben_product_sn>(a => a.phone_sn == origInfo.phone_sn);
                    db.CommitTran();
                    return "success";
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return "系统错误：" + ex.Message;
                }
            }
        }

        public void Statistics(string importId)
        {
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    List<daoben_hr_emp_job_history> salesMList = db.Queryable<daoben_hr_emp_job_history>()
                                .JoinTable<daoben_sale_outstorage>((a, b) => b.area_l1_id == a.area_l1_id && a.effect_date <= b.accur_time
                                && (a.inactive == false || a.inactive_time > b.accur_time))
                                .Where<daoben_sale_outstorage>((a, b) => b.import_file_id == importId && b.check_status == 1 &&
                                a.position_type == ConstData.POSITION_SALESMANAGER)
                                .Select("a.emp_id, a.name, a.area_l1_id").GroupBy(a => a.emp_id).ToList();
                    bool checkSalesM = false;
                    if (salesMList != null && salesMList.Count > 0)
                        checkSalesM = true;
                    QContainer qInfo = db.Queryable<daoben_sale_outstorage>()
                            .Where(a => a.import_file_id == importId && a.check_status == 1)
                            .Select<QContainer>("MAX(accur_time) as endTime1, MIN(accur_time) as startTime1,company_id_parent as idInt")
                            .SingleOrDefault();

                    List<daoben_product_price> priceList = db.Queryable<daoben_product_price>()
                                .Where(a => a.company_id_parent == qInfo.idInt
                                && a.effect_date <= ((DateTime)qInfo.endTime1).Date
                                && a.expire_date >= ((DateTime)qInfo.startTime1).Date
                                && a.effect_status > 0)
                                .Select("id, model, color, price_wholesale, price_retail, special_offer, high_level, effect_date, expire_date, effect_status, company_id")
                                .OrderBy(a => a.id, OrderByType.Desc).ToList();   // 必须按ID倒序
                    #region 串码表中已存在的
                    // 实销/买断已导入的串码（不更新company_id等信息）
                    List<daoben_product_sn> snExistList = db.Queryable<daoben_sale_outstorage>()
                                .JoinTable<daoben_product_sn>((a, b) => a.phone_sn == b.phone_sn && b.status > 0)
                                .Where<daoben_product_sn>((a, b) => a.import_file_id == importId && a.check_status == 1
                                && b.phone_sn != null)
                                .Select<daoben_product_sn>("a.color, a.model,b.id, b.phone_sn, b.price_wholesale, b.price_retail, b.special_offer, b.high_level, a.sales_id as out_sales_id, a.sales_name as out_sales, a.distributor_id as out_distributor_id, a.distributor_name as out_distributor, a.accur_time as outstorage_time, a.company_id, a.area_l1_id as tmpInt")
                                .ToList();
                    StringBuilder sqlUpSb = new StringBuilder("insert into daoben_product_sn (`id`,`out_distributor_id`,`outstorage_time`,`price_wholesale`,`price_retail`,`out_sales_m_id`,`out_sales_id`,`special_offer`,`high_level`,`out_distributor`,`out_sales_m`,`out_sales`) values ");
                    foreach (var e in snExistList)
                    {
                        if (checkSalesM)
                        {
                            daoben_hr_emp_job_history saleM = salesMList.Where(a => a.area_l1_id == e.tmpInt).FirstOrDefault();
                            e.out_sales_m_id = saleM.emp_id;
                            e.out_sales_m = saleM.name;
                        }
                        if (e.price_wholesale == 0 || e.price_retail == 0)
                        {
                            DateTime curDate = ((DateTime)e.outstorage_time).Date;
                            daoben_product_price priceInfo = priceList.Where(a => a.company_id == e.company_id && a.model == e.model && a.color == e.color
                                        && a.effect_date <= curDate && a.expire_date >= curDate).FirstOrDefault();  // p.id存放company_id
                            if (priceInfo != null)
                            {
                                if (e.price_wholesale == 0)
                                    e.price_wholesale = priceInfo.price_wholesale;
                                if (e.price_retail == 0)
                                    e.price_retail = priceInfo.price_retail;
                                e.special_offer = priceInfo.special_offer;
                                e.high_level = priceInfo.high_level;
                            }
                        }
                        sqlUpSb.AppendFormat("({0},'{1}','{2}',{3},{4},'{5}','{6}',{7},{8},'{9}','{10}','{11}'),", e.id, e.out_distributor_id, e.outstorage_time,
                                e.price_wholesale, e.price_retail, e.out_sales_m_id, e.out_sales_id, e.special_offer, e.high_level,
                                e.out_distributor, e.out_sales_m, e.out_sales);
                    }
                    sqlUpSb.Remove(sqlUpSb.Length - 1, 1); // 最后一个逗号
                    sqlUpSb.Append(" on duplicate key update out_distributor_id=values(out_distributor_id),outstorage_time=values(outstorage_time),");
                    sqlUpSb.Append(" price_wholesale=values(price_wholesale),price_retail=values(price_retail),out_sales_m_id=values(out_sales_m_id),");
                    sqlUpSb.Append(" out_sales_id=values(out_sales_id),special_offer=values(special_offer),high_level=values(high_level),");
                    sqlUpSb.Append(" out_distributor=values(out_distributor),out_sales_m=values(out_sales_m),out_sales=values(out_sales); ");

                    #endregion
                    #region 串码表中不存在的
                    List<daoben_product_sn> snNotExistList = db.Queryable<daoben_sale_outstorage>()
                                .JoinTable<daoben_product_sn>((a, b) => a.phone_sn == b.phone_sn && b.status > 0)
                                .Where<daoben_product_sn>((a, b) => a.import_file_id == importId && a.check_status == 1
                                && b.phone_sn == null)
                                .Select<daoben_product_sn>("a.phone_sn,a.model,a.color,a.distributor_id as out_distributor_id,a.distributor_name as out_distributor,a.accur_time as outstorage_time,a.sales_id as out_sales_id,a.sales_name as out_sales,a.company_id,a.company_id_parent,a.company_linkname, a.area_l1_id as tmpInt")
                                .ToList();
                    snNotExistList.ForEach(ne =>
                    {
                        ne.status = 1;
                        ne.sale_time = null;
                        ne.buyout_time = null;
                        ne.sale_type = 0;
                        if (checkSalesM)
                        {
                            daoben_hr_emp_job_history saleM = salesMList.Where(a => a.area_l1_id == ne.tmpInt).FirstOrDefault();
                            ne.out_sales_m_id = saleM.emp_id;
                            ne.out_sales_m = saleM.name;
                        }

                        DateTime curDate = ((DateTime)ne.outstorage_time).Date;
                        daoben_product_price priceInfo = priceList.Where(a => a.company_id == ne.company_id && a.model == ne.model && a.color == ne.color
                                    && a.effect_date <= curDate && a.expire_date >= curDate).FirstOrDefault();
                        if (priceInfo != null)
                        {
                            ne.price_wholesale = priceInfo.price_wholesale;
                            ne.price_retail = priceInfo.price_retail;
                            ne.special_offer = priceInfo.special_offer;
                            ne.high_level = priceInfo.high_level;
                        }
                    });
                    #endregion
                    if (snExistList != null && snExistList.Count > 0)
                        db.SqlQuery<int>(sqlUpSb.ToString());
                    db.DisableInsertColumns = new string[] { "id" };
                    if (snNotExistList.Count > 25)
                        db.SqlBulkCopy(snNotExistList);
                    else if (snNotExistList.Count > 0)
                        db.InsertRange(snNotExistList);
                }
            }
            catch (Exception ex)
            {
                ExceptionApp.WriteLog("OutStorageApp(Statistic)：" + ex.Message);
            }
        }

    }
}
