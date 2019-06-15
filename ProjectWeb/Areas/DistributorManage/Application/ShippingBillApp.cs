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
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Drawing;
using NPOI.HSSF.Util;

namespace ProjectWeb.Areas.DistributorManage.Application
{
    public class ShippingBillApp
    {
        /// <summary>
        /// 发货明细
        /// </summary>
        public object GetEffectList(Pagination pagination, QueryTime queryTime, string shipping_bill, string distributor_name,
            int? company_id, int? total_count_min = null, int? total_count_max = null)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");

            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.bill_date" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_distributor_shipping_detail>()
                    .JoinTable<daoben_distributor_shipping>((a, b) => b.status == 100)
                    .Select("a.*,b.creator_name,b.import_file,b.create_time");
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (myCompanyInfo.category == "事业部")
                        qable.Where<daoben_distributor_shipping>((a, b) => b.company_id_parent == myCompanyInfo.id || b.company_id == myCompanyInfo.id);
                    else if (myCompanyInfo.category == "分公司")
                        qable.Where<daoben_distributor_shipping>((a, b) => b.company_id == myCompanyInfo.id);
                    else return null;
                }

                if (company_id > 0)
                    qable.Where(a => a.company_id == company_id);
                if (shipping_bill != null)
                    qable.Where(a => a.shipping_bill.Contains(shipping_bill));
                if (distributor_name != null)
                    qable.Where(a => a.distributor_name.Contains(distributor_name));
                if (total_count_min != null)
                    qable.Where(a => a.quantity >= total_count_min);
                if (total_count_max != null)
                    qable.Where(a => a.quantity <= total_count_max);
                if (queryTime != null)
                {
                    if (queryTime.startTime1 != null)
                        qable.Where(a => a.bill_date >= queryTime.startTime1);
                    if (queryTime.startTime2 != null)
                    {
                        queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                        qable.Where(a => a.bill_date < queryTime.startTime2);
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
        public object GetList(Pagination pagination, string queryName, QueryTime queryTime)
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
                var qable = db.Queryable<daoben_distributor_shipping>();
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (myCompanyInfo.category == "事业部")
                        qable.Where(a => a.company_id == myCompanyInfo.id);
                    else if (myCompanyInfo.category == "分公司")
                        qable.Where(a => a.company_id == myCompanyInfo.parentId);
                    else return null;
                }

                //no queryInfo
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

        /// <summary>
        /// 我的审批
        /// </summary>
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
                // TODO：目前必须由事业部的人导入，如果由分公司导入则可能出错 JIANG 2019-3-11
                var qable = db.Queryable<daoben_distributor_shipping>().Where(a => a.status == 0 && a.company_id == myCompanyInfo.id);

                if (queryName != null)
                    qable.Where(a => a.import_file.Contains(queryName));
                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        /// <summary>
        /// 导出
        /// </summary>
        public MemoryStream ExportExcel(Pagination pagination, QueryTime queryTime, string shipping_bill, string distributor_name,
            int? company_id, int? total_count_min = null, int? total_count_max = null)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (pagination == null)
                pagination = new Pagination();
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.bill_date" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_distributor_shipping_detail>()
                    .JoinTable<daoben_distributor_shipping>((a, b) => b.status == 100);
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (myCompanyInfo.category == "事业部")
                        qable.Where<daoben_distributor_shipping>((a, b) => b.company_id_parent == myCompanyInfo.id || b.company_id == myCompanyInfo.id);
                    else if (myCompanyInfo.category == "分公司")
                        qable.Where<daoben_distributor_shipping>((a, b) => b.company_id == myCompanyInfo.id);
                    else return null;
                }

                if (company_id > 0)
                    qable.Where(a => a.company_id == company_id);
                if (shipping_bill != null)
                    qable.Where(a => a.shipping_bill.Contains(shipping_bill));
                if (distributor_name != null)
                    qable.Where(a => a.distributor_name.Contains(distributor_name));
                if (total_count_min != null)
                    qable.Where(a => a.quantity >= total_count_min);
                if (total_count_max != null)
                    qable.Where(a => a.quantity <= total_count_max);
                if (queryTime != null)
                {
                    if (queryTime.startTime1 != null)
                        qable.Where(a => a.bill_date >= queryTime.startTime1);
                    if (queryTime.startTime2 != null)
                    {
                        queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                        qable.Where(a => a.bill_date < queryTime.startTime2);
                    }
                }
                string selStr = "a.shipping_bill,a.company_linkname, a.distributor_name, a.quantity, a.amount,a.product_detail,DATE_FORMAT(a.bill_date,'%Y-%m-%d'),a.is_received, a.note,a.operator_name,a.is_printed,a.extend_note,a.schedule_note,a.schedule_bill,b.import_file,b.creator_name,DATE_FORMAT(b.create_time,'%Y-%m-%d')";
                var listDt = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .Select(selStr).ToDataTable();
                int rowsCount = listDt.Rows.Count;

                string[] headerArr = new string[] { "单号", "所属机构","经销商","数量","金额","明细","制单时间","是否收货",
                        "备注","经手人","是否打印","扩展备注","计划单备注","计划单自定义单号","导入文件名", "导入人","导入时间"};
                int[] colWidthArr = new int[] { 17, 18, 15, 10, 10, 45, 17, 10, 18, 10, 10, 18, 18, 20, 20, 15, 17 };

                //建立空白工作簿
                HSSFWorkbook book = new HSSFWorkbook();
                if (rowsCount < ConstData.EXPORT_SHEET_LEN)
                    ExcelApi.ExcelExport(book, "sheet1", headerArr, colWidthArr, listDt, 0, rowsCount);
                else
                {
                    int page = rowsCount / ConstData.EXPORT_SHEET_LEN;
                    for (int i = 0; i < page; i++)
                    {
                        int start = i * ConstData.EXPORT_SHEET_LEN;
                        int end = (i * ConstData.EXPORT_SHEET_LEN) + ConstData.EXPORT_SHEET_LEN - 1;
                        string sheetName = "sheet" + (i + 1).ToString();

                        ExcelApi.ExcelExport(book, sheetName, headerArr, colWidthArr, listDt, start, end);
                    }
                    int lastCount = rowsCount % ConstData.EXPORT_SHEET_LEN;
                    string lastName = "sheet" + (page + 1).ToString();

                    ExcelApi.ExcelExport(book, lastName, headerArr, colWidthArr, listDt, rowsCount- lastCount, lastCount);
                }

                // 写入到客户端 
                MemoryStream ms = new MemoryStream();
                {
                    book.Write(ms);
                    ms.Seek(0, SeekOrigin.Begin);
                    return ms;
                }
            }
        }
        public string Import(List<daoben_distributor_shipping_detail> importList, daoben_distributor_shipping mainInfo)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (mainInfo == null)
                return "信息错误，操作失败!";
            if (importList == null || importList.Count < 1)
                return "信息错误：详情列表不能为空!";
            List<string> dupList = importList.GroupBy(a => a.shipping_bill).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            if (dupList != null && dupList.Count > 0)
                return "信息错误：导入表中单号重复" + dupList.ToJson();

            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            // TODO：目前必须由事业部的人导入，如果由分公司导入则可能出错 JIANG 2019-3-11
            int company_id_parent = (myCompanyInfo.category == "事业部" ? myCompanyInfo.id : myCompanyInfo.parentId);
            string company_name_parent = (myCompanyInfo.category == "事业部" ? myCompanyInfo.name : myCompanyInfo.parentName);

            using (var db = SugarDao.GetInstance())
            {
                
                try
                {
                    //查找运费模版
                    daoben_distributor_shipping_template_approve shipTempMain = db.Queryable<daoben_distributor_shipping_template_approve>()
                               .SingleOrDefault(a => a.company_id == company_id_parent && a.effect_status == 1);
                    if (shipTempMain == null)
                        return "信息错误：机构（" + company_name_parent + "）暂未配置有效的运费模版";

                    mainInfo.id = Common.GuId();
                    //匹配经销商名称ID
                    List<daoben_distributor_info> distriList = db.Queryable<daoben_distributor_info>()
                        .Where(a => (a.company_id_parent == myCompanyInfo.id || a.company_id == myCompanyInfo.id)).ToList();
                    foreach (var a in importList)
                    {
                        daoben_distributor_info distriInfo = distriList.SingleOrDefault(b => b.name == a.distributor_name && b.company_name == a.company_name);
                        if (distriInfo == null)
                            return "信息错误：无法匹配到【" + a.company_name + "】机构下的【" + a.distributor_name + "】经销商信息";
                        else
                        {
                            a.main_id = mainInfo.id;
                            a.distributor_id = distriInfo.id;
                            a.company_id = distriInfo.company_id;
                            a.company_linkname = distriInfo.company_linkname;
                        }
                    }

                    //                     
                    mainInfo.creator_job_history_id = LoginInfo.jobHistoryId;
                    mainInfo.creator_name = LoginInfo.empName;
                    mainInfo.creator_position_name = myPositionInfo.name;
                    mainInfo.create_time = now;
                    mainInfo.status = 0;
                    mainInfo.company_id = myCompanyInfo.id;
                    mainInfo.company_linkname = myCompanyInfo.linkName;
                    mainInfo.company_id_parent = myCompanyInfo.parentId;
                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    db.SqlQuery<int>("insert into `daoben_distributor_shipping` (`id`,`import_file`,`status`,`create_time`,`creator_job_history_id`,`creator_position_name`,`creator_name`,`company_id`,`company_linkname`,`company_id_parent`) values(@id, @import_file,@status,@create_time,@creator_job_history_id,@creator_position_name,@creator_name,@company_id,@company_linkname,@company_id_parent)",
                               mainInfo);
                   // db.Insert(mainInfo);
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
        /// <summary>
        /// 审批功能
        /// </summary>        
        public string Approve(daoben_distributor_shipping_approve approveInfo)
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
                    daoben_distributor_shipping mainInfo = db.Queryable<daoben_distributor_shipping>()
                                .SingleOrDefault(a => a.id == approveInfo.main_id);
                    if (mainInfo == null || mainInfo.status < 0 || mainInfo.status == 100)
                        return "信息错误：指定的申请信息不存在或已审批";
                    //查找运费模版
                    // TODO：目前必须由事业部的人导入，如果由分公司导入则可能出错 JIANG 2019-3-11
                    daoben_distributor_shipping_template_approve shipTempMain = db.Queryable<daoben_distributor_shipping_template_approve>()
                               .SingleOrDefault(a => a.company_id == mainInfo.company_id && a.effect_status == 1);
                    if (shipTempMain == null)
                        return "信息错误：机构（" + mainInfo.company_linkname + "）暂未配置有效的运费模版";

                    if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIAL || LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER)
                    {
                        approveInfo.status = mainInfo.status = (approveInfo.status > 0) ? 100 : -100;
                    }
                    else
                    {
                        if (approveInfo.status > 0)
                        {
                            mainInfo.status = 0 + 1 + mainInfo.status;
                            approveInfo.status = mainInfo.status;
                        }
                        else
                        {
                            mainInfo.status = 0 - 1 - mainInfo.status;
                            approveInfo.status = mainInfo.status;
                        }
                    }
                    object upObj = new { status = approveInfo.status };


                    approveInfo.creator_job_history_id = LoginInfo.jobHistoryId;
                    approveInfo.approve_name = LoginInfo.empName;
                    approveInfo.approve_position_name = myPositionInfo.name;
                    approveInfo.approve_time = DateTime.Now;

                    //清除之前待办事项
                    List<daoben_sys_task> origTask = db.Queryable<daoben_sys_task>().Where(a => a.main_id == approveInfo.main_id).ToList();
                    object upObjForTask = new
                    {
                        status = 3,
                        finished_time = now
                    };
                    if (origTask.Count() > 0)
                        db.Update<daoben_sys_task>(upObjForTask, a => a.main_id == approveInfo.main_id);
                    #region 消息通知
                    List<daoben_sys_notification> newsTotal = new List<daoben_sys_notification>();//消息通知
                    string newsStr = null;
                    List<string> newsIdList = null;
                    newsIdList = db.Queryable<daoben_hr_emp_job_history>()
                                .Where(a => a.id == mainInfo.creator_job_history_id).Select<string>("emp_id as id").ToList();

                    if (newsIdList != null && newsIdList.Count > 0)
                    {
                        if (mainInfo.status < 0)
                            newsStr = mainInfo.company_linkname + " 运单信息导入没有通过";
                        else if (mainInfo.status > 0)
                            newsStr = mainInfo.company_linkname + " 运单信息导入已通过";
                        //消息通知 生成列表
                        List<daoben_sys_notification> newsList = newsIdList
                                .Select(a1 => new daoben_sys_notification
                                {
                                    emp_id = a1,
                                    category = 2,
                                    main_id = mainInfo.id,
                                    main_url = "/FinancialAccounting/ShippingBill/Show?id=" + mainInfo.id,
                                    title = newsStr,
                                    content_abstract = mainInfo.import_file,
                                    recipient_type = 1,
                                    create_time = now,
                                    status = 1
                                }).ToList();
                        newsTotal.AddRange(newsList);
                    }
                    //消息通知 列表插入
                    if (newsTotal != null && newsTotal.Count > 25)
                        db.SqlBulkCopy(newsTotal);
                    else if (newsTotal != null && newsTotal.Count > 0)
                        db.InsertRange(newsTotal);
                    #endregion 待办事项
                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    db.Insert(approveInfo);
                    db.Update<daoben_distributor_shipping>(upObj, a => a.id == approveInfo.main_id);
                    if(approveInfo.status == 100)
                    {
                        // 审批通过后，汇总
                        //数量相加
                        int company_id_parent = mainInfo.company_id_parent;
                        List<daoben_distributor_settlement_shipping> settlementList = new List<daoben_distributor_settlement_shipping>();
                        //List<daoben_distributor_shipping_detail> importList = db.Queryable<daoben_distributor_shipping_detail>()
                        //            .Where(a => a.main_id == mainInfo.id).ToList();
                        
                        List<daoben_distributor_shipping_template> shipTempList = db.Queryable<daoben_distributor_shipping_template>()
                            .Where(a => a.main_id == shipTempMain.id).ToList();
                        var totalQuantity = db.Queryable<daoben_distributor_shipping_detail>()
                                    .JoinTable<daoben_distributor_info>((a, b) => a.distributor_id == b.id)
                                    .Where(a => a.main_id == mainInfo.id)
                                    .GroupBy("distributor_id, shipping_date")
                                    .Select<daoben_distributor_settlement_shipping>("a.distributor_id,a.distributor_name, b.company_id,b.company_id_parent,b.company_linkname, SUM(a.quantity) as total_count,DATE(a.bill_date) as shipping_date")
                                    .ToList();
                       
                        //计算运费
                        foreach (var a in totalQuantity)
                        {
                            a.total_amount = shipTempList.Where(b => a.total_count >= b.count_min).OrderByDescending(t => t.count_min).First().shipping_fee * a.total_count;
                            if (a.total_amount <= shipTempMain.minimum_fee)
                                a.total_amount = shipTempMain.minimum_fee;
                            settlementList.Add(a);
                        }
                        if (settlementList.Count > 25)
                            db.SqlBulkCopy(settlementList);
                        else
                            db.InsertRange(settlementList);
                        db.CommitTran();
                    }

                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return "系统出错：" + ex.Message;
                }
            }
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
                    daoben_distributor_shipping mainInfo = db.Queryable<daoben_distributor_shipping>().SingleOrDefault(a => a.id == id);
                    if (mainInfo == null)
                        return "信息错误：指定的信息不存在";
                    List<daoben_distributor_shipping_approve> approveList = db.Queryable<daoben_distributor_shipping_approve>().Where(a => a.main_id == id).ToList();

                    object resultObj = new
                    {
                        mainInfo = mainInfo,
                        approveList = approveList,
                    };
                    return resultObj.ToJson();

                }
                catch (Exception ex)
                {
                    return "系统错误：" + ex.Message;
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
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "bill_date" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    var qable = db.Queryable<daoben_distributor_shipping_detail>().Where(a => a.main_id == id);
                    if (!string.IsNullOrEmpty(queryStr))
                        qable.Where(a => a.shipping_bill.Contains(queryStr) || a.distributor_name.Contains(queryStr));

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
                    daoben_distributor_shipping mainInfo = db.Queryable<daoben_distributor_shipping>().SingleOrDefault(a => a.id == id);
                    if (mainInfo == null || mainInfo.status != 0)
                        //财务经理审批过后，将不能撤回
                        return "撤回失败：指定的申请信息不存在或者已被审批";
                    else
                    {
                        db.CommandTimeOut = 30;
                        db.BeginTran();
                        db.Delete<daoben_distributor_shipping_detail>(a => a.main_id == id);
                        db.Delete<daoben_distributor_shipping>(a => a.id == id);
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

        public string GetEffectInfo()
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            string warning = null;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    List<daoben_distributor_shipping_template> shippingFeeList = new List<daoben_distributor_shipping_template>();
                    daoben_distributor_shipping_template_approve mainInfo = db.Queryable<daoben_distributor_shipping_template_approve>()
                        .SingleOrDefault(a => a.company_id == myCompanyInfo.id && a.effect_status == 1);
                    if (mainInfo != null)
                        shippingFeeList = db.Queryable<daoben_distributor_shipping_template>()
                            .Where(a => a.main_id == mainInfo.id).GroupBy("count_min").ToList();
                    else
                        warning = "需要提前配置生效的运费模版！";
                    object resultObj = new
                    {
                        mainInfo = mainInfo,
                        shippingFeeList = shippingFeeList,
                        warning = warning

                    };
                    return resultObj.ToJson();

                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        //private void DataCheck(string mainId)
        //{
        //    DateTime halfYAgo = DateTime.Now.AddMonths(-6); // 只检查半年内的数据
        //    using (var db = SugarDao.GetInstance())
        //    {
        //        List<daoben_distributor_shipping> importList = db.Queryable<daoben_distributor_shipping>()
        //                    .Where(a => a.import_file_id == mainId).Select("id,no,shipping_bill,sub_bill")
        //                    .ToList();      // 导入数据ID为0，故重新查询数据库
        //        List<string> snList = importList.Select(a => a.no).ToList();
        //        List<daoben_distributor_shipping> errList = db.Queryable<daoben_distributor_shipping>()
        //                    .Where(a => a.accur_time > halfYAgo && a.import_file_id != mainId
        //                    && a.check_status < 2 && snList.Contains(a.no))
        //                    .ToList();
        //        if (errList != null && errList.Count > 1)
        //        {
        //            StringBuilder sqlUpSb = new StringBuilder("insert into daoben_distributor_shipping (`id`, `check_status`) values ");
        //            errList.ForEach(e =>
        //            {
        //                daoben_distributor_shipping data = importList.Where(a => a.no == e.no).SingleOrDefault();
        //                if (data != null)
        //                {
        //                    if (data.shipping_bill == e.shipping_bill && data.sub_bill == e.sub_bill)
        //                        sqlUpSb.AppendFormat("({0},{1}),", data.id, 11);   // 11-重复数据 
        //                    else
        //                        sqlUpSb.AppendFormat("({0},{1}),", data.id, 21);   // 21-相同串码不同信息
        //                }
        //            });
        //            sqlUpSb.Remove(sqlUpSb.Length - 1, 1); // 最后一个逗号
        //            sqlUpSb.Append("on duplicate key update check_status=values(check_status);");
        //            db.SqlQuery<int>(sqlUpSb.ToString());
        //        }
        //    }
        //}


        //public string UpdateCheckStatus(List<int> idList, int status)
        //{
        //    OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
        //    if (LoginInfo == null)
        //        throw new Exception("用户登陆过期，请重新登录");
        //    if (idList == null || idList.Count < 1)
        //        return "信息错误,指定的数据不存在!";
        //    if (status != 1 && status != 2)
        //        return "信息错误,操作类型错误!";
        //    using (var db = SugarDao.GetInstance())
        //    {
        //        StringBuilder sqlUpSb = new StringBuilder("UPDATE daoben_distributor_shipping SET check_status = check_status + ");
        //        sqlUpSb.AppendFormat("{0} WHERE ID IN(", status.ToString());
        //        foreach (var a in idList)
        //        {
        //            sqlUpSb.Append("'" + a.ToString() + "',");
        //        }
        //        sqlUpSb.Remove(sqlUpSb.Length - 1, 1);
        //        sqlUpSb.Append(");");
        //        db.SqlQuery<int>(sqlUpSb.ToString());
        //    }
        //    return "success";
        //}
    }
}
