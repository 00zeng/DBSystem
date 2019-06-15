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
    public class BuyoutImportApp
    {

        public object GetAllList(Pagination pagination, daoben_sale_buyout_import_temp queryInfo, QueryTime queryTime)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");

            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "b.create_time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_sale_buyout_import_temp>()
                        .JoinTable<daoben_sale_buyout_import_approve>((a, b) => a.import_file_id == b.id)
                        .Where<daoben_sale_buyout_import_approve>((a, b) => b.status == 100);
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER || LoginInfo.roleId == ConstData.ROLE_ID_FINANCIAL)
                        qable.Where<daoben_sale_buyout_import_approve>((a, b) => b.company_id == myCompanyInfo.id);
                    else
                        qable.Where(a => a.area_l2_id == myPositionInfo.areaL2Id);
                }
                if (queryInfo != null)
                {
                    if (!string.IsNullOrEmpty(queryInfo.phone_sn))
                        qable.Where(a => a.phone_sn.Contains(queryInfo.phone_sn));
                    if (!string.IsNullOrEmpty(queryInfo.distributor_name))
                        qable.Where(a => a.distributor_name.Contains(queryInfo.distributor_name));
                    if (!string.IsNullOrEmpty(queryInfo.model))
                        qable.Where(a => a.model.Contains(queryInfo.model));
                    if (queryInfo.area_l2_id > 0)
                        qable.Where(a => a.area_l2_id == queryInfo.area_l2_id || a.area_l1_id == queryInfo.area_l2_id); //区域搜索条件
                    if (!string.IsNullOrEmpty(queryInfo.import_file_id))
                        qable.Where<daoben_sale_buyout_import_approve>((a, b) => b.import_file.Contains(queryInfo.import_file_id));
                }
                if (queryTime != null)
                {
                    if (queryTime.startTime1 != null)
                        qable.Where<daoben_sale_buyout_import_approve>((a, b) => a.accur_time >= queryTime.startTime1);
                    if (queryTime.startTime2 != null)
                    {
                        queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                        qable.Where<daoben_sale_buyout_import_approve>((a, b) => a.accur_time < queryTime.startTime2);
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
                var qable = db.Queryable<daoben_sale_buyout_import_approve>();//历史判定
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
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_sale_buyout_import_approve>().Where(a => a.status == 0);
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER)
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


        public string Import(List<daoben_sale_buyout_import_temp> importList, daoben_sale_buyout_import_approve importInfo, ref int delCount)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (importList == null || importList.Count < 0 || importInfo == null || string.IsNullOrEmpty(importInfo.id))
                return "信息错误，操作失败!";

            List<string> dupList = importList.GroupBy(a => a.phone_sn).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            if (dupList != null && dupList.Count > 0)
                return "信息错误：导入表中串码重复" + dupList.ToJson();

            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            importInfo.creator_job_history_id = LoginInfo.jobHistoryId;
            importInfo.creator_name = LoginInfo.empName;
            importInfo.create_time = DateTime.Now;
            importInfo.create_time = DateTime.Now;
            importInfo.status = 0;
            importInfo.company_id = myCompanyInfo.id;
            importInfo.company_name = myCompanyInfo.name;

            List<string> snList = importList.Select(a => a.phone_sn).ToList();
            DateTime maxDate = importList.Max(a => a.accur_time).ToDate();
            DateTime threeMAgo = maxDate.AddMonths(-3); // 只检查3个月内的数据
            StringBuilder sqlUpSb = new StringBuilder("insert into daoben_sale_buyout_import_temp (`id`, `check_status`) values ");
            int errCount = 0;
            using (var db = SugarDao.GetInstance())
            {
                //待办事项 --财务文员+财务经理 待测试
                #region 待办事项 财务
                //待办事项 收件人
                string tempStr = "买断信息" + "待审批";
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
                        main_url = "/SaleManage/BuyoutImport/Approve?id=" + importInfo.id,
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
                    List<daoben_sale_buyout_import_temp> errList = db.Queryable<daoben_sale_buyout_import_temp>()
                            .Where(a => a.accur_time > threeMAgo && a.check_status == 1 && snList.Contains(a.phone_sn))
                            .ToList();
                    if (errList != null && errList.Count > 1)
                    {
                        int delCount1 = 0;
                        errList.ForEach(e =>
                        {
                            daoben_sale_buyout_import_temp data = importList.Where(a => a.phone_sn == e.phone_sn).SingleOrDefault();
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

        public string Approve(daoben_sale_buyout_import_approve approveInfo)
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
                    daoben_sale_buyout_import_approve origInfo = db.Queryable<daoben_sale_buyout_import_approve>().InSingle(approveInfo.id);
                    if (origInfo == null)
                        return "信息错误：指定的申请信息不存在";
                    bool errData = db.Queryable<daoben_sale_buyout_import_temp>().Any(a => a.import_file_id == approveInfo.id && a.check_status == 21);
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
                    //TODO 生效判定
                    //db.Update<daoben_sale_buyout>(new { effect_status = 0 }, a => a.import_file_id == approveInfo.id);
                    //TODO 消息通知——通知申请者 待测试
                    #region 消息通知 创建人
                    //消息通知 收件人id列表
                    string tempStr = null;
                    tempStr = "买断信息导入审批" + (approveInfo.status == 100 ? "通过" : "不通过");
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
                            main_url = "/SaleManage/BuyoutImport/Show?id=" + origInfo.id,
                            title = tempStr + "\t",
                            content_abstract = origInfo.import_file,
                            recipient_type = 1,
                            create_time = now,
                            status = 1
                        }).ToList();
                    #endregion
                    db.CommandTimeOut = 300;
                    db.BeginTran();
                    db.Update<daoben_sale_buyout_import_approve>(upObj, a => a.id == approveInfo.id);
                    if (approveInfo.status == -100)  // 将check_status也置为-100
                        db.Update<daoben_sale_buyout_import_temp>(new { check_status = -100 }, a => a.import_file_id == approveInfo.id);
                    //代办事项 置为 已处理
                    List<daoben_sys_task> origTask = db.Queryable<daoben_sys_task>().Where(a => a.main_id == approveInfo.id).ToList();
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
                    settlement.Rebate(approveInfo.id, 3);
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
                    string mainInfoStr = db.Queryable<daoben_sale_buyout_import_approve>()
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
                    var qable = db.Queryable<daoben_sale_buyout_import_temp>().Where(a => a.import_file_id == id);
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
        internal string Delete(string id)
        {
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_sale_buyout_import_approve mainInfo = db.Queryable<daoben_sale_buyout_import_approve>().SingleOrDefault(a => a.id == id);
                    if (mainInfo == null || mainInfo.status != 0)
                        //财务经理审批过后，将不能撤回
                        return "撤回失败：指定的申请信息不存在或者已被审批";
                    else
                    {
                        db.CommandTimeOut = 30;
                        db.BeginTran();
                        db.Delete<daoben_sale_buyout_import_temp>(a => a.import_file_id == id);
                        db.Delete<daoben_sale_buyout_import_approve>(a => a.id == id);
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
                StringBuilder sqlUpSb = new StringBuilder("UPDATE daoben_sale_buyout_temp SET check_status = check_status + ");
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
                    daoben_sale_buyout_import_temp origInfo = db.Queryable<daoben_sale_buyout_import_temp>()
                            .JoinTable<daoben_sale_buyout_import_approve>((a, b) => a.import_file_id == b.id)
                            .Where(a => a.id == id)
                            .Select("a.phone_sn, b.status as check_status").SingleOrDefault(); // 占用check_status作为审批状态
                    if (origInfo == null)
                        return "信息错误,指定的数据不存在!";
                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    db.Update<daoben_sale_buyout_import_temp>(new { check_status = 23 }, a => a.phone_sn == origInfo.phone_sn);
                    db.Update<daoben_sale_buyout_import_temp>(new { check_status = 1 }, a => a.id == id);
                    if (origInfo.check_status != 100)   // 保留的信息是未审批通过的，则要删除的信息可能已存在串码信息表
                    {
                        if (db.Queryable<daoben_product_sn>().Any(a => a.phone_sn == origInfo.phone_sn && a.status > 10))
                        {
                            StringBuilder sqlSb = new StringBuilder("update daoben_product_sn ");
                            sqlSb.Append("set status=status-10,buyout_time=null ");
                            sqlSb.AppendFormat("where phone_sn='{0}' and status>10 ", origInfo.phone_sn);
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
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    // Note: 业务经理信息只在实销导入时补充
                    // 买断提成
                    StringBuilder sqlSbOutlayDel = new StringBuilder("delete a from daoben_product_sn_outlay a ");
                    sqlSbOutlayDel.Append("inner join daoben_sale_buyout_import_temp b on a.phone_sn=b.phone_sn ");
                    sqlSbOutlayDel.AppendFormat("where b.import_file_id='{0}' and b.check_status=1 and a.category=31", importId);

                    StringBuilder sqlSbOutlayIns = new StringBuilder("insert into daoben_product_sn_outlay ");
                    sqlSbOutlayIns.Append("(category, phone_sn, outlay, outlay_type, time) ");
                    sqlSbOutlayIns.Append("select 31 as category,phone_sn,guide_commission as outlay,1 as outlay_type,buyout_time as time ");
                    sqlSbOutlayIns.AppendFormat("from daoben_sale_buyout_import_temp where import_file_id='{0}' and check_status=1", importId);

                    #region 串码表中已存在的
                    StringBuilder sqlSb = new StringBuilder("update daoben_product_sn a inner join ");
                    sqlSb.Append("(select distributor_id,distributor_name, phone_sn,accur_time,price_wholesale,price_retail,");
                    sqlSb.Append("price_buyout,sales_id,sales_name,guide_id,guide_name, ");
                    sqlSb.Append("company_id,company_id_parent,company_linkname from daoben_sale_buyout_import_temp ");
                    sqlSb.AppendFormat("where import_file_id='{0}' and check_status=1) b on a.phone_sn=b.phone_sn ", importId);
                    sqlSb.Append("set a.price_wholesale=if(b.price_wholesale>0,b.price_wholesale,a.price_wholesale),");
                    sqlSb.Append("a.price_retail=if(b.price_retail>0,b.price_retail,a.price_retail),");
                    sqlSb.Append("a.price_sale=b.price_buyout,a.sale_time=ifnull(a.sale_time,b.accur_time),");   //实销未导入时先保存sale_time，避免数据统计时不一致
                    sqlSb.Append("a.sales_id=ifnull(b.sales_id,a.sales_id),a.sales=ifnull(b.sales_name,a.sales),");
                    sqlSb.Append("a.reporter_id=ifnull(b.guide_id,a.reporter_id),a.reporter=ifnull(b.guide_name,a.reporter),");
                    sqlSb.Append("a.reporter_type=if(b.guide_id is null, a.reporter_type, 1),"); // 业务员、导购员以买断导入为准
                    sqlSb.Append("a.status=a.status+10,a.buyout_time=b.accur_time,");
                    sqlSb.Append("a.sale_distributor_id=b.distributor_id,a.sale_distributor=b.distributor_name,a.sale_type=2,");
                    sqlSb.Append("a.company_id=b.company_id,a.company_id_parent=b.company_id_parent,a.company_linkname=b.company_linkname");
                    #endregion

                    QContainer qInfo = db.Queryable<daoben_sale_buyout_import_temp>()
                                .Where(a => a.import_file_id == importId && a.check_status == 1)
                                .Select<QContainer>("MAX(accur_time) as endTime1, MIN(accur_time) as startTime1,company_id_parent as idInt")
                                .SingleOrDefault();

                    List<daoben_product_price> priceList = db.Queryable<daoben_product_price>()
                                .Where(a => a.company_id_parent == qInfo.idInt
                                && a.effect_date <= ((DateTime)qInfo.endTime1).Date
                                && a.expire_date >= ((DateTime)qInfo.startTime1).Date
                                && a.effect_status > 0)
                                .Select("id, model, color, price_wholesale, price_retail, effect_date, expire_date, effect_status, company_id")
                                .OrderBy(a => a.effect_date, OrderByType.Desc).ToList();   // 必须按生效时间倒序
                    #region 串码表中不存在的
                    List<daoben_product_sn> snNotExistList = db.Queryable<daoben_sale_buyout_import_temp>()
                                .JoinTable<daoben_product_sn>((a, b) => a.phone_sn == b.phone_sn && b.status > 0)
                                .Where<daoben_product_sn>((a, b) => a.import_file_id == importId && a.check_status == 1 && b.phone_sn == null)
                                .Select<daoben_product_sn>("a.phone_sn,a.model,a.color,a.distributor_id as sale_distributor_id,a.distributor_name as sale_distributor,a.sales_id,a.sales_name as sales,a.guide_id as reporter_id,a.guide_name as reporter,a.accur_time as buyout_time,a.price_wholesale,a.price_retail,a.price_buyout as price_sale,a.company_id,a.company_id_parent,a.company_linkname")
                                .ToList();
                    snNotExistList.ForEach(ne =>
                    {
                        ne.status = 11;
                        ne.outstorage_time = null;
                        ne.sale_time = ne.buyout_time;  //实销未导入时先保存sale_time，避免数据统计时不一致
                        ne.sale_type = 2;
                        if (!string.IsNullOrEmpty(ne.reporter_id))
                            ne.reporter_type = 1;
                        DateTime curDate = ((DateTime)ne.buyout_time).Date;
                        daoben_product_price priceInfo = priceList.Where(a => a.company_id == ne.company_id && a.model == ne.model && a.color == ne.color
                                && a.effect_date <= curDate && a.expire_date >= curDate).FirstOrDefault();
                        if (priceInfo != null)
                        {
                            ne.special_offer = priceInfo.special_offer;
                            ne.high_level = priceInfo.high_level;
                            if (ne.price_wholesale == 0)
                                ne.price_wholesale = priceInfo.price_wholesale;
                            if (ne.price_retail == 0)
                                ne.price_retail = priceInfo.price_retail;
                        }
                    });
                    #endregion
                    db.SqlQuery<int>(sqlSb.ToString());
                    db.SqlQuery<int>(sqlSbOutlayDel.ToString());
                    db.SqlQuery<int>(sqlSbOutlayIns.ToString());
                    db.DisableInsertColumns = new string[] { "id" };
                    if (snNotExistList.Count > 25)
                        db.SqlBulkCopy(snNotExistList);
                    else if (snNotExistList.Count > 0)
                        db.InsertRange(snNotExistList);
                }
            }
            catch (Exception ex)
            {
                ExceptionApp.WriteLog("BuyoutImportApp(Statistic)：" + ex.Message);
            }
        }

    }
}
