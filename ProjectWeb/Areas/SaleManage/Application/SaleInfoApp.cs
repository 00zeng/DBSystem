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
    public class SaleInfoApp
    {
        public object GetList(Pagination pagination, daoben_sale_salesinfo queryInfo, QueryTime queryTime)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");

            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.accur_time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_sale_salesinfo>()
                        .JoinTable<daoben_sale_salesinfo_approve>((a, b) => a.import_file_id == b.id);
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
                        qable.Where<daoben_sale_salesinfo_approve>((a, b) => b.import_file.Contains(queryInfo.import_file_id));
                }
                if (queryTime != null)
                {
                    if (queryTime.startTime1 != null)
                        qable.Where<daoben_sale_salesinfo_approve>((a, b) => a.accur_time >= queryTime.startTime1);
                    if (queryTime.startTime2 != null)
                    {
                        queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                        qable.Where<daoben_sale_salesinfo_approve>((a, b) => a.accur_time < queryTime.startTime2);
                    }
                }


                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .Select("a.*,b.creator_name,b.import_file as import_file,b.create_time as create_time")
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
                var qable = db.Queryable<daoben_sale_salesinfo_approve>();//历史判定
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER || LoginInfo.roleId == ConstData.ROLE_ID_FINANCIAL)
                        qable.Where(a => a.company_id == myCompanyInfo.id);
                    else
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


                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
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
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_sale_salesinfo_approve>().Where(a => a.status == 0);
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER)
                        qable.Where(a => a.company_id == myCompanyInfo.id);
                    else return null;
                }

                if (!string.IsNullOrEmpty(queryName))
                    qable.Where(a => a.import_file.Contains(queryName));
                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public string Import(List<daoben_sale_salesinfo> importList, daoben_sale_salesinfo_approve importInfo, ref int delCount)
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
                return "信息错误：导入表中串码重复" + dupList.ToJson();
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
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
            StringBuilder sqlUpSb = new StringBuilder("insert into daoben_sale_salesinfo (`id`, `check_status`) values ");
            int errCount = 0;
            using (var db = SugarDao.GetInstance())
            {
                #region 待办事项 财务
                //待办事项 收件人
                string tempStr = "实销信息" + "待审批";
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
                        main_url = "/SaleManage/SaleInfo/Approve?id=" + importInfo.id,
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
                    List<daoben_sale_salesinfo> errList = db.Queryable<daoben_sale_salesinfo>()
                            .Where(a => a.accur_time > threeMAgo && a.check_status == 1 && snList.Contains(a.phone_sn))
                            .ToList();
                    if (errList != null && errList.Count > 1)
                    {
                        int delCount1 = 0;
                        errList.ForEach(e =>
                        {
                            daoben_sale_salesinfo data = importList.Where(a => a.phone_sn == e.phone_sn).SingleOrDefault();
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
                return "success";
            }
        }

        public string Approve(daoben_sale_salesinfo_approve approveInfo)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_sale_salesinfo_approve origInfo = db.Queryable<daoben_sale_salesinfo_approve>().InSingle(approveInfo.id);
                    if (origInfo == null)
                        return "信息错误：指定的申请信息不存在";
                    bool errData = db.Queryable<daoben_sale_salesinfo>().Any(a => a.import_file_id == approveInfo.id && a.check_status == 21);
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
                    tempStr = "实销信息导入审批" + (approveInfo.status == 100 ? "通过" : "不通过");
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
                            main_url = "/SaleManage/SaleInfo/Show?id=" + origInfo.id,
                            title = tempStr + "\t",
                            content_abstract = origInfo.import_file,
                            recipient_type = 1,
                            create_time = now,
                            status = 1
                        }).ToList();
                    #endregion
                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    db.Update<daoben_sale_salesinfo_approve>(upObj, a => a.id == approveInfo.id);
                    if (approveInfo.status == -100)  // 将check_status也置为-100
                        db.Update<daoben_sale_salesinfo>(new { check_status = -100 }, a => a.import_file_id == approveInfo.id);
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
                    settlement.Rebate(approveInfo.id, 2);
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
                    string mainInfoStr = db.Queryable<daoben_sale_salesinfo_approve>()
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
                    var qable = db.Queryable<daoben_sale_salesinfo>().Where(a => a.import_file_id == id);
                    if (!string.IsNullOrEmpty(queryStr))
                        qable.Where(a => a.distributor_name.Contains(queryStr) || a.model.Contains(queryStr) || a.phone_sn.Contains(queryStr));

                    string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                            .ToJsonPage(pagination.page, pagination.rows, ref records);
                    pagination.records = records;
                    if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                        return null;
                    return listStr.ToJson();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
        internal string Delete(string id)
        {
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_sale_salesinfo_approve mainInfo = db.Queryable<daoben_sale_salesinfo_approve>().SingleOrDefault(a => a.id == id);
                    if (mainInfo == null || mainInfo.status != 0)
                        //财务经理审批过后，将不能撤回
                        return "撤回失败：指定的申请信息不存在或者已被审批";
                    else
                    {
                        db.CommandTimeOut = 30;
                        db.BeginTran();
                        db.Delete<daoben_sale_salesinfo_approve>(a => a.id == id);
                        db.Delete<daoben_sale_salesinfo>(a => a.import_file_id == id);
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
                StringBuilder sqlUpSb = new StringBuilder("UPDATE daoben_sale_salesinfo SET check_status = check_status + ");
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
                    daoben_sale_salesinfo origInfo = db.Queryable<daoben_sale_salesinfo>()
                            .JoinTable<daoben_sale_salesinfo_approve>((a, b) => a.import_file_id == b.id)
                            .Where(a => a.id == id)
                            .Select("a.phone_sn, b.status as check_status").SingleOrDefault(); // 占用check_status作为审批状态
                    if (origInfo == null)
                        return "信息错误,指定的数据不存在!";
                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    db.Update<daoben_sale_salesinfo>(new { check_status = 23 }, a => a.phone_sn == origInfo.phone_sn);
                    db.Update<daoben_sale_salesinfo>(new { check_status = 1 }, a => a.id == id);
                    if (origInfo.check_status != 100)   // 保留的信息是未审批通过的，则要删除的信息可能已存在串码信息表
                    {
                        int[] intArr = new[] { 2, 12 };
                        if (db.Queryable<daoben_product_sn>().Any(a => a.phone_sn == origInfo.phone_sn && intArr.Contains(a.status)))
                        {
                            StringBuilder sqlSb = new StringBuilder("update daoben_product_sn ");
                            sqlSb.Append("set reporter_id=null, reporter_type=0,sales_id=null,status=status-1,sale_time=null ");
                            sqlSb.AppendFormat("where phone_sn='{0}' and status in (2,12) ", origInfo.phone_sn);
                            db.SqlQuery<int>(sqlSb.ToString());
                        }
                    }
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
            List<daoben_product_sn_outlay> outlayList = new List<daoben_product_sn_outlay>();
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    List<daoben_hr_emp_job_history> salesMList = db.Queryable<daoben_hr_emp_job_history>()
                                .JoinTable<daoben_sale_salesinfo>((a, b) => b.area_l1_id == a.area_l1_id && a.effect_date <= b.accur_time
                                && (a.inactive == false || a.inactive_time > b.accur_time))
                                .Where<daoben_sale_salesinfo>((a, b) => b.import_file_id == importId && b.check_status == 1 &&
                                a.position_type == ConstData.POSITION_SALESMANAGER)
                                .Select("a.emp_id, a.name, a.area_l1_id").GroupBy(a => a.emp_id).ToList();
                    bool checkSalesM = false;
                    if (salesMList != null && salesMList.Count > 0)
                        checkSalesM = true;
                    // 串码信息表
                    QContainer qInfo = db.Queryable<daoben_sale_salesinfo>()
                                .Where(a => a.import_file_id == importId && a.check_status == 1)
                                .Select<QContainer>("MAX(accur_time) as endTime1, MIN(accur_time) as startTime1,company_id_parent as idInt")
                                .SingleOrDefault();

                    List<daoben_product_price> priceList = db.Queryable<daoben_product_price>()
                                .Where(a => a.company_id_parent == qInfo.idInt
                                && a.effect_date <= ((DateTime)qInfo.endTime1).Date
                                && a.expire_date >= ((DateTime)qInfo.startTime1).Date
                                && a.effect_status > 0)
                                .Select("id, model, color, price_wholesale, price_retail, effect_date, expire_date, effect_status,special_offer,high_level, company_id")
                                .OrderBy(a => a.effect_date, OrderByType.Desc).ToList();   // 必须按生效时间倒序
                    //提成规则
                    List<daoben_product_commission_detail> commissionList = db.Queryable<daoben_product_commission_detail>()
                                .Where(a => a.company_id_parent == qInfo.idInt && a.effect_date <= ((DateTime)qInfo.endTime1).Date
                                && a.expire_date >= ((DateTime)qInfo.startTime1).Date && a.effect_status > 0)
                                .Select("id, model, color, exclusive_commission,guide_commission, effect_date, expire_date,salary_include, company_id")
                                .OrderBy(a => a.effect_date, OrderByType.Desc).ToList();
                    #region 串码表中已存在的
                    List<daoben_product_sn> snExistList = db.Queryable<daoben_sale_salesinfo>()
                                .JoinTable<daoben_product_sn>((a, b) => a.phone_sn == b.phone_sn && b.status > 0)
                                .Where<daoben_product_sn>((a, b) => a.import_file_id == importId && a.check_status == 1
                                && b.phone_sn != null)
                                .Select<daoben_product_sn>("a.color, a.model,b.id,b.phone_sn,b.status,b.sale_type,b.price_wholesale,b.price_sale,b.price_retail,b.guide_commission,b.special_offer,b.high_level,a.reporter_id,a.reporter_name as reporter,a.reporter_type,a.sales_id,a.sales_name as sales,a.distributor_id as sale_distributor_id,a.distributor_name as sale_distributor,a.accur_time as sale_time,a.company_id,a.company_id_parent,a.company_linkname, a.area_l1_id as tmpInt")
                                .ToList();
                    StringBuilder sqlUpSb = new StringBuilder("insert into daoben_product_sn (`id`,`sale_distributor_id`,`sale_time`,`sales_m_id`,`sales_id`,");
                    sqlUpSb.Append(" `reporter_id`,`reporter_type`,`sale_type`,`status`,`price_wholesale`,`price_retail`,`special_offer`,`high_level`,");
                    sqlUpSb.Append(" `sale_distributor`,`sales_m`,`sales`,`reporter`,`price_sale`,`company_id`,`company_id_parent`,`company_linkname`) values ");

                    foreach (var e in snExistList)
                    {
                        if (e.sale_type == 0)
                            e.sale_type = 1;
                        e.status++;
                        if (checkSalesM)
                        {
                            daoben_hr_emp_job_history saleM = salesMList.Where(a => a.area_l1_id == e.tmpInt).FirstOrDefault();
                            e.sales_m_id = saleM.emp_id;
                            e.sales_m = saleM.name;
                        }
                        if (e.price_wholesale == 0 || e.price_retail == 0)
                        {
                            DateTime curDate = ((DateTime)e.sale_time).Date;
                            daoben_product_price priceInfo = priceList.Where(a => a.company_id == e.company_id && a.model == e.model && a.color == e.color
                                        && a.effect_date <= curDate && a.expire_date >= curDate).FirstOrDefault();
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
                        if (e.sale_type == 1)   // 只更新正常销售的，买断/包销不会进入此段代码
                        {
                            DateTime saleDate = ((DateTime)e.sale_time).Date;
                            daoben_product_commission_detail commissionInfo = commissionList
                                    .Where(c => c.company_id == e.company_id && c.model == e.model && c.color == e.color
                                    && c.effect_date <= saleDate && c.expire_date >= saleDate)
                                    .FirstOrDefault();
                            if (commissionInfo != null)
                            {
                                outlayList.Add(new daoben_product_sn_outlay()
                                {
                                    category = 31,
                                    phone_sn = e.phone_sn,
                                    outlay = commissionInfo.guide_commission,
                                    outlay_type = 1,
                                    time = e.sale_time
                                });
                            }
                            e.price_sale = e.price_retail;
                        }
                        sqlUpSb.AppendFormat("({0},'{1}','{2}','{3}','{4}','{5}',{6},{7},{8},{9},{10},{12},{13},'{14}','{15}','{16}','{17}',{18},{19},{20},'{21}'),",
                                e.id, e.sale_distributor_id, e.sale_time, e.sales_m_id, e.sales_id, e.reporter_id, e.reporter_type,
                                e.sale_type, e.status, e.price_wholesale, e.price_retail, e.special_offer, e.high_level,
                                e.sale_distributor, e.sales_m, e.sales, e.reporter, e.price_sale, e.company_id, e.company_id_parent, e.company_linkname);
                    }
                    sqlUpSb.Remove(sqlUpSb.Length - 1, 1); // 最后一个逗号
                    sqlUpSb.Append(" on duplicate key update sale_distributor_id=values(sale_distributor_id),sale_time=values(sale_time),");
                    sqlUpSb.Append(" sales_m_id=values(sales_m_id),sales_id=values(sales_id),reporter_id=values(reporter_id),");
                    sqlUpSb.Append(" reporter_type=values(reporter_type),sale_type=values(sale_type),status=values(status),");
                    sqlUpSb.Append(" price_wholesale=values(price_wholesale),price_retail=values(price_retail),");
                    sqlUpSb.Append(" special_offer=values(special_offer),high_level=values(high_level),sale_distributor=values(sale_distributor),");
                    sqlUpSb.Append(" sales_m=values(sales_m),sales=values(sales),reporter=values(reporter),price_sale=values(price_sale),");
                    sqlUpSb.Append(" company_id=values(company_id),company_id_parent=values(company_id_parent),company_linkname=values(company_linkname);");
                    #endregion

                    #region 串码表中不存在的
                    List<daoben_product_sn> snNotExistList = db.Queryable<daoben_sale_salesinfo>()
                                .JoinTable<daoben_product_sn>((a, b) => a.phone_sn == b.phone_sn && b.status > 0)
                                .Where<daoben_product_sn>((a, b) => a.import_file_id == importId && a.check_status == 1 && b.phone_sn == null)
                                .Select<daoben_product_sn>("a.phone_sn,a.model,a.color,a.distributor_id as sale_distributor_id,a.distributor_name as sale_distributor,a.accur_time as sale_time,a.sales_id,a.sales_name as sales,a.reporter_id,a.reporter_name as reporter,a.reporter_type,a.company_id,a.company_id_parent,a.company_linkname, a.area_l1_id as tmpInt")
                                .ToList();
                    snNotExistList.ForEach(ne =>
                    {
                        ne.status = 2;
                        ne.outstorage_time = null;
                        ne.buyout_time = null;
                        ne.sale_type = 1;
                        if (checkSalesM)
                        {
                            daoben_hr_emp_job_history saleM = salesMList.Where(a => a.area_l1_id == ne.tmpInt).FirstOrDefault();
                            ne.sales_m_id = saleM.emp_id;
                            ne.sales_m = saleM.name;
                        }

                        DateTime curDate = ((DateTime)ne.sale_time).Date;
                        daoben_product_price priceInfo = priceList.Where(a => a.company_id == ne.company_id && a.model == ne.model && a.color == ne.color
                                    && a.effect_date <= curDate && a.expire_date >= curDate).FirstOrDefault();
                        if (priceInfo != null)
                        {
                            ne.price_wholesale = priceInfo.price_wholesale;
                            ne.price_sale = ne.price_retail = priceInfo.price_retail;
                            ne.special_offer = priceInfo.special_offer;
                            ne.high_level = priceInfo.high_level;
                        }
                        DateTime saleDate = ((DateTime)ne.sale_time).Date;
                        daoben_product_commission_detail commissionInfo = commissionList
                                .Where(c => c.company_id == ne.company_id && c.model == ne.model && c.color == ne.color
                                && c.effect_date <= saleDate && c.expire_date >= saleDate)
                                .FirstOrDefault();
                        if (commissionInfo != null)
                        {
                            outlayList.Add(new daoben_product_sn_outlay()
                            {
                                category = 31,
                                phone_sn = ne.phone_sn,
                                outlay = commissionInfo.guide_commission,
                                outlay_type = 1,
                                time = ne.sale_time
                            });
                        }
                    });
                    #endregion
                    if (snExistList != null && snExistList.Count > 0)
                        db.SqlQuery<int>(sqlUpSb.ToString());
                    db.DisableInsertColumns = new string[] { "id" };
                    if (snNotExistList.Count > 0)
                        db.SqlBulkCopy(snNotExistList);
                    if (outlayList.Count > 0)
                        db.SqlBulkCopy(outlayList);
                }
            }
            catch (Exception ex)
            {
                ExceptionApp.WriteLog("SaleInfoApp(Statistic)：" + ex.Message);
            }
        }
    }
}
