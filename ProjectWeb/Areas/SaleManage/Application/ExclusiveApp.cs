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
    public class ExclusiveApp
    {

        public object GetAllList(Pagination pagination, daoben_sale_exclusive_detail queryInfo, QueryTime queryTime)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");

            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "c.create_time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_sale_exclusive_detail>()
                        .JoinTable<daoben_sale_exclusive_approve>((a, b) => a.main_id == b.main_id)
                        .JoinTable<daoben_sale_exclusive>((a, c) => a.main_id == c.id)
                        .Where<daoben_sale_exclusive_approve>((a, b) => b.status == 100);
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER || LoginInfo.roleId == ConstData.ROLE_ID_FINANCIAL)
                        qable.Where<daoben_sale_exclusive>((a, c) => c.company_id == myCompanyInfo.id);
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
                    if (queryInfo.area_l2_id > 0)
                        qable.Where(a => a.area_l2_id == queryInfo.area_l2_id || a.area_l1_id == queryInfo.area_l2_id); //区域搜索条件
                    if (!string.IsNullOrEmpty(queryInfo.main_id))
                        qable.Where<daoben_sale_exclusive>((a, c) => c.import_file.Contains(queryInfo.main_id));//main_di存储文件名
                }
                if (queryTime != null)
                {
                    if (queryTime.startTime1 != null)
                        qable.Where<daoben_sale_exclusive>((a, c) => c.create_time >= queryTime.startTime1);
                    if (queryTime.startTime2 != null)
                    {
                        queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                        qable.Where<daoben_sale_exclusive>((a, c) => c.create_time < queryTime.startTime2);
                    }
                }



                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .Select("a.*,c.*")
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
                var qable = db.Queryable<daoben_sale_exclusive>();//历史判定
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER || LoginInfo.roleId == ConstData.ROLE_ID_FINANCIAL)
                        qable.Where(a => a.company_id == myCompanyInfo.id || a.company_id_parent == myCompanyInfo.id);
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
                var qable = db.Queryable<daoben_sale_exclusive>().Where(a => a.status == 0);
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


        public string Import(List<daoben_sale_exclusive_detail> importList, daoben_sale_exclusive importInfo, ref int delCount)
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
            importInfo.status = 0;
            importInfo.company_id = myCompanyInfo.id;
            importInfo.company_id_parent = myCompanyInfo.parentId;
            importInfo.company_linkname = myCompanyInfo.linkName;//TODO


            List<string> snList = importList.Select(a => a.phone_sn).ToList();
            DateTime maxDate = importList.Max(a => a.accur_time).ToDate();
            DateTime threeMAgo = maxDate.AddMonths(-3); // 只检查3个月内的数据
            StringBuilder sqlUpSb = new StringBuilder("insert into daoben_sale_exclusive_detail (`id`, `check_status`) values ");
            int errCount = 0;
            using (var db = SugarDao.GetInstance())
            {
                //待办事项 --财务文员+财务经理 待测试

                db.CommandTimeOut = 300;
                try
                {
                    List<daoben_sale_exclusive_detail> errList = db.Queryable<daoben_sale_exclusive_detail>()
                            .Where(a => a.accur_time > threeMAgo && a.check_status == 1 && snList.Contains(a.phone_sn))
                            .ToList();
                    if (errList != null && errList.Count > 1)
                    {
                        int delCount1 = 0;
                        errList.ForEach(e =>
                        {
                            daoben_sale_exclusive_detail data = importList.Where(a => a.phone_sn == e.phone_sn).SingleOrDefault();
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

        public string Approve(daoben_sale_exclusive_approve approveInfo)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            approveInfo.approve_job_history_id = LoginInfo.jobHistoryId;
            approveInfo.approve_name = LoginInfo.empName;
            approveInfo.approve_position_name = myPositionInfo.name;
            approveInfo.approve_time = now;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_sale_exclusive origInfo = db.Queryable<daoben_sale_exclusive>().SingleOrDefault(a => a.id == approveInfo.main_id);
                    if (origInfo == null)
                        return "信息错误：指定的申请信息不存在";
                    bool errData = db.Queryable<daoben_sale_exclusive_detail>().Any(a => a.main_id == approveInfo.main_id && a.check_status == 21);
                    if (errData)
                        return "操作失败：存在未处理的异常数据，请先处理";
                    if (approveInfo.status > 0)
                        approveInfo.status = 100;   // 以100作为审批完成的标志
                    else
                        approveInfo.status = -100;

                    object upObj = new
                    {
                        status = approveInfo.status,
                    };

                    db.CommandTimeOut = 30;
                    db.BeginTran();

                    db.Insert(approveInfo);
                    db.Update<daoben_sale_exclusive>(upObj, t => t.id == approveInfo.main_id);
                    if (approveInfo.status == -100)  // 将check_status也置为-100
                        db.Update<daoben_sale_exclusive_detail>(new { check_status = -100 }, a => a.main_id == approveInfo.main_id);


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
                    Statistics(approveInfo.main_id);
                    SettlementApp settlement = new SettlementApp();
                    settlement.Rebate(approveInfo.main_id, 5);
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
                    string mainInfoStr = db.Queryable<daoben_sale_exclusive>()
                            .JoinTable<daoben_hr_emp_job_history>((a, b) => a.creator_job_history_id == b.id)
                            .Where<daoben_hr_emp_job_history>((a, b) => a.id == id)
                            .Select("a.*, b.position_name as creator_position_name").ToJson();
                    List<daoben_sale_exclusive_approve> approveList = db.Queryable<daoben_sale_exclusive_approve>()
                        .Where(a => a.main_id == id).ToList();
                    object result = new
                    {
                        mainInfoStr = mainInfoStr.ToJson(),
                        approveList = approveList,
                    };
                    return result.ToJson();

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
                    var qable = db.Queryable<daoben_sale_exclusive_detail>().Where(a => a.main_id == id);
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
                    daoben_sale_exclusive mainInfo = db.Queryable<daoben_sale_exclusive>().SingleOrDefault(a => a.id == id);
                    if (mainInfo == null || mainInfo.status != 0)
                        //财务经理审批过后，将不能撤回
                        return "撤回失败：指定的申请信息不存在或者已被审批";
                    else
                    {
                        db.CommandTimeOut = 30;
                        db.BeginTran();
                        db.Delete<daoben_sale_exclusive>(a => a.id == id);
                        db.Delete<daoben_sale_exclusive_detail>(a => a.main_id == id);
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
                StringBuilder sqlUpSb = new StringBuilder("UPDATE daoben_sale_exclusive_detail SET check_status = check_status + ");
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
                    daoben_sale_exclusive_detail origInfo = db.Queryable<daoben_sale_exclusive_detail>()
                            .JoinTable<daoben_sale_exclusive>((a, b) => a.main_id == b.id)
                            .Where(a => a.id == id)
                            .Select("a.phone_sn, b.status as check_status").SingleOrDefault(); // 占用check_status作为审批状态
                    if (origInfo == null)
                        return "信息错误,指定的数据不存在!";
                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    db.Update<daoben_sale_exclusive_detail>(new { check_status = 23 }, a => a.phone_sn == origInfo.phone_sn);
                    db.Update<daoben_sale_exclusive_detail>(new { check_status = 1 }, a => a.id == id);
                    if (origInfo.check_status != 100)   // 保留的信息是未审批通过的，则要删除的信息可能已存在串码信息表
                    {
                        if (db.Queryable<daoben_product_sn>().Any(a => a.phone_sn == origInfo.phone_sn && a.status > 10))
                        {

                            //StringBuilder sqlSb = new StringBuilder("update daoben_product_sn ");
                            //sqlSb.Append("set status=status-10,buyout_time=null ");
                            //sqlSb.AppendFormat("where phone_sn='{0}' and status>10 ", origInfo.phone_sn);
                            //db.SqlQuery<int>(sqlSb.ToString());
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
                    sqlSbOutlayDel.Append("inner join daoben_sale_exclusive_detail b on a.phone_sn=b.phone_sn ");
                    sqlSbOutlayDel.AppendFormat("where b.main_id='{0}' and b.check_status=1 and a.category=31", importId);

                    StringBuilder sqlSbOutlayIns = new StringBuilder("insert into daoben_product_sn_outlay ");
                    sqlSbOutlayIns.Append("(category, phone_sn, outlay, outlay_type, time) ");
                    sqlSbOutlayIns.Append("select 31 as category,phone_sn,guide_commission as outlay,1 as outlay_type,buyout_time as time ");
                    sqlSbOutlayIns.AppendFormat("from daoben_sale_exclusive_detail where main_id='{0}' and check_status=1", importId);

                    // 串码信息表
                    #region 串码表中已存在的
                    StringBuilder sqlSb = new StringBuilder("update daoben_product_sn a inner join ");
                    sqlSb.Append("(select distributor_id,distributor_name, sales_id, sales_name, phone_sn,accur_time,price_wholesale,price_exclusive,");
                    sqlSb.Append("company_id,company_id_parent,company_linkname from daoben_sale_exclusive_detail ");
                    sqlSb.AppendFormat("where main_id='{0}' and check_status=1) b on a.phone_sn=b.phone_sn ", importId);
                    sqlSb.Append("set a.price_wholesale=if(b.price_wholesale>0,b.price_wholesale,a.price_wholesale),");
                    sqlSb.Append("a.price_sale=b.price_exclusive,a.sale_time=ifnull(a.sale_time,b.accur_time),");   //实销未导入时先保存sale_time，避免数据统计时不一致
                    sqlSb.Append("a.buyout_time=b.accur_time,a.sale_type=3,");
                    sqlSb.Append("a.sale_distributor_id=b.distributor_id,a.sale_distributor=b.distributor_name,a.sales_id=b.sales_id,a.sales=b.sales_name,");
                    sqlSb.Append("a.company_id=b.company_id,a.company_id_parent=b.company_id_parent,a.company_linkname=b.company_linkname");
                    #endregion

                    QContainer qInfo = db.Queryable<daoben_sale_exclusive_detail>()
                                .Where(a => a.main_id == importId && a.check_status == 1)
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
                    List<daoben_product_sn> snNotExistList = db.Queryable<daoben_sale_exclusive_detail>()
                                .JoinTable<daoben_product_sn>((a, b) => a.phone_sn == b.phone_sn && b.status > 0)
                                .Where<daoben_product_sn>((a, b) => a.main_id == importId && a.check_status == 1 && b.phone_sn == null)
                                .Select<daoben_product_sn>("a.phone_sn,a.model,a.color,a.distributor_id as sale_distributor_id,a.distributor_name as sale_distributor,a.accur_time as buyout_time,a.price_wholesale,a.price_exclusive as price_sale,a.sales_id,a.sales_name as sales,a.company_id,a.company_id_parent,a.company_linkname")
                                .ToList();
                    snNotExistList.ForEach(ne =>
                    {
                        ne.status = 11;
                        ne.outstorage_time = null;
                        ne.sale_time = ne.buyout_time;  //实销未导入时先保存sale_time，避免数据统计时不一致
                        ne.sale_type = 3;
                        DateTime curDate = ((DateTime)ne.buyout_time).Date;
                        daoben_product_price priceInfo = priceList.Where(a => a.company_id == ne.company_id && a.model == ne.model && a.color == ne.color
                                && a.effect_date <= curDate && a.expire_date >= curDate).FirstOrDefault();
                        if (priceInfo != null)
                        {
                            if (ne.price_wholesale == 0)
                                ne.price_wholesale = priceInfo.price_wholesale;
                            ne.special_offer = priceInfo.special_offer;
                            ne.high_level = priceInfo.high_level;
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
                ExceptionApp.WriteLog("ExclusiveApp(Statistic)：" + ex.Message);
            }
        }

    }
}
