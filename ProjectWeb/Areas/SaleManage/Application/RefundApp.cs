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

namespace ProjectWeb.Areas.SaleManage.Application
{
    public class RefundApp
    {

        public object GetList(Pagination pagination, daoben_sale_refund queryInfo, QueryTime queryTime)
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
                var qable = db.Queryable<daoben_sale_refund>()
                        .JoinTable<daoben_sale_refund_approve>((a, b) => a.import_file_id == b.id);
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER || LoginInfo.roleId == ConstData.ROLE_ID_FINANCIAL)
                        qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                    else
                        qable.Where(a => a.company_id == myCompanyInfo.id);
                }
                if (queryInfo != null)
                {
                    if (!string.IsNullOrEmpty(queryInfo.distributor_name))
                        qable.Where(a => a.distributor_name.Contains(queryInfo.distributor_name));
                    if (!string.IsNullOrEmpty(queryInfo.phone_sn))
                        qable.Where(a => a.phone_sn.Contains(queryInfo.phone_sn));
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
                        qable.Where(a => a.accur_time >= queryTime.startTime1);
                    if (queryTime.startTime2 != null)
                    {
                        queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                        qable.Where(a => a.accur_time < queryTime.startTime2);
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
                var qable = db.Queryable<daoben_sale_refund_approve>();//历史判定
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
                var qable = db.Queryable<daoben_sale_refund_approve>().Where(a => a.status == 0);

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


        public string Import(List<daoben_sale_refund> importList, daoben_sale_refund_approve importInfo, ref int delCount)
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
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            importInfo.creator_job_history_id = LoginInfo.jobHistoryId;
            importInfo.creator_name = LoginInfo.empName;
            importInfo.create_time = DateTime.Now;
            importInfo.status = 0;
            importInfo.company_id = myCompanyInfo.id;
            importInfo.company_name = myCompanyInfo.name;

            List<string> snStrList = importList.OrderBy(a => a.phone_sn).Select(a => a.phone_sn).ToList();
            DateTime maxDate = importList.Max(a => a.accur_time).ToDate();
            DateTime threeMAgo = maxDate.AddMonths(-3); // 只检查3个月内的数据
            using (var db = SugarDao.GetInstance())
            {
                // TODO 待办事项 --财务文员+财务经理 待测试
                #region 待办事项 财务
                //待办事项 收件人
                string tempStr = "调价补差信息" + "待审批";
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
                        main_url = "/SaleManage/Refund/Approve?id=" + importInfo.id,
                        title = tempStr,
                        content_abstract = importInfo.import_file,
                        recipient_type = 1,
                        create_time = now,
                        status = 1
                    }).ToList();
                #endregion
                List<daoben_sale_refund> retainList = new List<daoben_sale_refund>();
                db.CommandTimeOut = 300;
                try
                {
                    List<daoben_sale_refund> refundDupList = db.Queryable<daoben_sale_refund>()
                            .JoinTable<daoben_sale_refund_approve>((a, b) => a.import_file_id == b.id)
                            .Where<daoben_sale_refund_approve>((a, b) => b.status >= 0 &&
                            a.accur_time > threeMAgo && snStrList.Contains(a.phone_sn))
                            .Select("a.phone_sn,a.accur_time")
                            .ToList();
#if false
暂不检查
                    List<daoben_product_sn> snInfoList = db.Queryable<daoben_product_sn>()
                            .Where(a => snStrList.Contains(a.phone_sn)).OrderBy(a => a.phone_sn)
                            .Select("id, phone_sn, model,out_distributor_id,sale_distributor_id").ToList();
                    if (snInfoList == null || snInfoList.Count < 1)
                        return "信息错误：串码信息不存在，请确认已导入相应的出库信息";
#endif
                    foreach (daoben_sale_refund i in importList)
                    {
#if false
暂不检查
                        // 串码信息表
                        daoben_product_sn snInfo = snInfoList.Where(a => a.phone_sn == i.phone_sn).SingleOrDefault();
                        if (snInfo == null)
                            return "信息错误：串码【" + i.phone_sn + "】信息不存在，请确认已导入相应的出库信息";
                        if (snInfo.model != i.model)    // 经销商之间可能线下调机，故不检查经销商ID是否匹配
                            return "信息错误：串码【" + i.phone_sn + "】与出库信息不匹配";
#endif
                        // 重复导入检查
                        if (refundDupList != null && refundDupList.Count > 0)
                        {
                            if (refundDupList.Any(a => a.phone_sn == i.phone_sn && a.accur_time == i.accur_time))
                            {
                                delCount++;
                                continue;
                            }
                        }
                        retainList.Add(i);
                    }
                    if (retainList.Count == 0)
                        return "导入失败：重复导入数据";

                    db.BeginTran();
                    db.Insert(importInfo);
                    db.DisableInsertColumns = new string[] { "id" };
                    //if (importList.Count > 25)
                    //    db.SqlBulkCopy(importList);
                    //else
                    //    db.InsertRange(importList);
                    if (retainList.Count > 25)
                        db.SqlBulkCopy(retainList);
                    else
                        db.InsertRange(retainList);
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

        public string Approve(daoben_sale_refund_approve approveInfo)
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
                    daoben_sale_refund_approve origInfo = db.Queryable<daoben_sale_refund_approve>().InSingle(approveInfo.id);
                    if (origInfo == null)
                        return "信息错误：指定的申请信息不存在";

                    if (approveInfo.status > 0)
                        approveInfo.status = 100;   // 以100作为审批完成的标志
                    else
                        approveInfo.status = -100;
                    //TODO 通过之后？然后呢
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
                    tempStr = "调价补差导入审批" + (approveInfo.status == 100 ? "通过" : "不通过");
                    List<string> idList = db.Queryable<daoben_hr_emp_job_history>()
                        .Where(a => a.id == origInfo.creator_job_history_id).Select<string>("emp_id as id").ToList();
                    //消息通知 生成列表
                    List<daoben_sys_notification> newsList = idList
                        .Select(a1 => new daoben_sys_notification
                        {
                            emp_id = a1,
                            category = 2,
                            main_id = origInfo.id,
                            main_url = "/SaleManage/Refund/Show?id=" + origInfo.id,
                            title = tempStr + "\t",
                            content_abstract = origInfo.import_file,
                            recipient_type = 1,
                            create_time = now,
                            status = 1
                        }).ToList();
                    #endregion
                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    db.Update<daoben_sale_refund_approve>(upObj, a => a.id == approveInfo.id);
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
                if (approveInfo.status == 100)
                    ThreadPool.QueueUserWorkItem(t =>
                    {
                        Statistics(approveInfo.id);
                    });
            }
            return "success";
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
                try
                {
                    List<daoben_distributor_info> distributorList = db.Queryable<daoben_distributor_info>()
                            .Where(a => a.company_id_parent == myCompanyInfo.id).ToList();
                    return distributorList.ToJson();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
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
                    string mainInfoStr = db.Queryable<daoben_sale_refund_approve>()
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
                    var qable = db.Queryable<daoben_sale_refund>().Where(a => a.import_file_id == id);
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
                    daoben_sale_refund_approve mainInfo = db.Queryable<daoben_sale_refund_approve>().SingleOrDefault(a => a.id == id);
                    if (mainInfo == null || mainInfo.status != 0)
                        //财务经理审批过后，将不能撤回
                        return "撤回失败：指定的申请信息不存在或者已被审批";
                    else
                    {
                        db.CommandTimeOut = 30;
                        db.BeginTran();
                        db.Delete<daoben_sale_refund_approve>(a => a.id == id);
                        db.Delete<daoben_sale_refund>(a => a.import_file_id == id);
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

        public void Statistics(string importId)
        {
            // 串码信息表
            StringBuilder sqlSb = new StringBuilder("update daoben_product_sn a inner join ");
            sqlSb.Append("(select phone_sn, diff_price, new_price from daoben_sale_refund ");
            sqlSb.AppendFormat("where import_file_id='{0}') b on a.phone_sn=b.phone_sn ", importId);
            sqlSb.Append("set a.price_wholesale=if(b.new_price>0, b.new_price, a.price_wholesale),");
            sqlSb.Append("a.refund_amount=a.refund_amount+b.diff_price, a.refund_count=a.refund_count+1");

            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    db.SqlQuery<int>(sqlSb.ToString());
                }
            }
            catch (Exception ex)
            {
                ExceptionApp.WriteLog("ReturnApp(Statistic)：" + ex.Message);
            }
        }
    }
}
