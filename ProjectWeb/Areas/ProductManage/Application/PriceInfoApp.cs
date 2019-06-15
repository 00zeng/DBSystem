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
using System.IO;
using NPOI.HSSF.UserModel;

namespace ProjectWeb.Areas.ProductManage.Application
{
    public class PriceInfoApp
    {
        public object GetListEffect(Pagination pagination, int? price_wholesale_min,
                    int? price_wholesale_max, daoben_product_price queryInfo)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");

            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "company_id" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "asc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_product_price_effect>().Where(a => a.is_effect == true);
                if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                else if (myCompanyInfo.category == "事业部")
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                if (queryInfo != null)
                {
                    if (!string.IsNullOrEmpty(queryInfo.model))
                        qable.Where(a => a.model.Contains(queryInfo.model));
                    if (!string.IsNullOrEmpty(queryInfo.color))
                        qable.Where(a => a.color.Contains(queryInfo.color));
                    if (queryInfo.company_id > 0)
                        qable.Where(a => a.company_id == queryInfo.company_id);
                }
                if (price_wholesale_min != null)
                    qable.Where(a => a.price_wholesale >= price_wholesale_min);
                if (price_wholesale_max != null)
                    qable.Where(a => a.price_wholesale <= price_wholesale_max);
                object list = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .ToPageList(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                return list;
            }
        }
        public object GetListPriceHistory(Pagination pagination, daoben_product_price queryInfo, QueryTime queryTime)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");

            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.company_id" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "asc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_product_price>()
                            .JoinTable<daoben_product_price_approve>((a, b) => a.import_file_id == b.id)
                            .Where<daoben_product_price_approve>((a, b) => b.status == 100);

                if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                else if (myCompanyInfo.category == "事业部")
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                if (queryTime != null)
                {
                    if (queryTime.startTime1 != null)
                        qable.Where<daoben_product_price_approve>((a, b) => a.effect_date >= queryTime.startTime1);
                    if (queryTime.startTime2 != null)
                    {
                        queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                        qable.Where<daoben_product_price_approve>((a, b) => a.effect_date < queryTime.startTime2);
                    }
                    if (queryTime.endTime1 != null)
                        qable.Where<daoben_product_price_approve>((a, b) => a.expire_date >= queryTime.endTime1);
                    if (queryTime.endTime2 != null)
                    {
                        queryTime.endTime2 = queryTime.endTime2.ToDate().AddDays(1);
                        qable.Where<daoben_product_price_approve>((a, b) => a.expire_date < queryTime.endTime2);
                    }
                }
                if (queryInfo != null)
                {
                    if (!string.IsNullOrEmpty(queryInfo.model))
                        qable.Where(a => a.model.Contains(queryInfo.model));
                    if (!string.IsNullOrEmpty(queryInfo.color))
                        qable.Where(a => a.color.Contains(queryInfo.color));
                    if (queryInfo.company_id > 0)
                        qable.Where(a => a.company_id == queryInfo.company_id);
                    if (queryInfo.effect_status != 0)
                        qable.Where(a => a.effect_status == queryInfo.effect_status);
                    else
                        qable.Where(a => a.effect_status > -2);
                }
                else
                    qable.Where(a => a.effect_status > -2);

                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .Select("a.*, b.import_file,b.creator_name, b.create_time")
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
        public MemoryStream ExportExcel(Pagination pagination, int company_id, string model,
            int? price_wholesale_min, int? price_wholesale_max)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");

            if (pagination == null)
                pagination = new Pagination();
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "company_id" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "asc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_product_price_effect>().Where(a => a.is_effect == true);
                if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                else if (myCompanyInfo.category == "事业部")
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);

                if (!string.IsNullOrEmpty(model))
                    qable.Where(a => a.model.Contains(model));
                if (company_id > 0)
                    qable.Where(a => a.company_id == company_id);
                if (price_wholesale_min != null)
                    qable.Where(a => a.price_wholesale >= price_wholesale_min);
                if (price_wholesale_max != null)
                    qable.Where(a => a.price_wholesale <= price_wholesale_max);

                //价格信息-当前价格
                string str = "company_linkname,model,color,DATE_FORMAT(expire_date,'%Y-%m-%d'),IF(special_offer=1,'是','否'),IF(high_level=1,'是','否'),price_wholesale,price_retail,price_l2,price_l3,price_l4,ad_fee_show,price_internal,price_buyout,product_type";
                var listDt = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                   .Select(str)
                   .ToDataTable();
                int rowsCount = listDt.Rows.Count;
                string[] headerArr = new string[]
                {
                        "所属机构","型号","颜色","生效时间","特价机","高端机","批发价","零售价","二级价","事业部价格","代理价","广告费",
                        "内购价","最低买断价","属性",
                };
                int[] colWidthArr = new int[] { 25, 20, 12, 25, 10, 10, 10, 10, 10, 11, 10, 10, 10, 11, 12 };

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

                    ExcelApi.ExcelExport(book, lastName, headerArr, colWidthArr, listDt, rowsCount - lastCount, lastCount);
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
        public string GetListImport(Pagination pagination, string name, QueryTime queryTime, int? status, bool approve)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (LoginInfo.roleId != ConstData.ROLE_ID_FINANCIALMANAGER && approve)
            {
                object errObj = new
                {
                    status = ConstData.ERR_STATUS_AUTH,
                    message = ConstData.ERR_MSG_AUTH
                };
                return errObj.ToJson();    
            }
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
                var qable = db.Queryable<daoben_product_price_approve>();
                if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                else if (myCompanyInfo.category == "事业部")
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);

                if (!string.IsNullOrEmpty(name))
                    qable.Where(a => a.import_file.Contains(name));
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
                if (status == 1)
                    qable.Where(a => a.status == 100);
                else if (status == -1)
                    qable.Where(a => a.status < 0);
                else if (status == -2)
                    qable.Where(a => a.status > 0 && a.status < 100);
                else if (status == 0)
                    qable.Where(a => a.status == 0);
                if (approve)    // “我的审批”列表
                    qable.Where(a => a.status == 0);

                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;

                object resObj = new
                {
                    status = ConstData.OK_STATUS,
                    rows = listStr.ToJson(),
                    total = pagination.records,
                };
                return resObj.ToJson();               
            }
        }

        public string Import(List<daoben_product_price> importList, daoben_product_price_approve mainInfo,
                List<IntIdNamePair> companyList)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            if (mainInfo == null || companyList == null || companyList.Count < 1)
                return "信息错误，操作失败!";
            if (importList == null || importList.Count < 1)
                return "信息错误：详情列表不能为空!";
            if (myCompanyInfo.category != "事业部")
                return "信息错误：价格信息须由事业部导入!";

            IntIdNamePair companyInfo = companyList[0];
            DateTime now = DateTime.Now;
            int companyParentId = myCompanyInfo.id;
            string companyNamePrefix = myCompanyInfo.name + " - ";
            List<daoben_product_price> insertImportList = new List<daoben_product_price>();
            List<daoben_product_price_approve> insertMainList = new List<daoben_product_price_approve>();
            // 第一个分公司
            string linkName = companyNamePrefix + companyInfo.name;
            mainInfo.id = Common.GuId();
            mainInfo.creator_job_history_id = LoginInfo.jobHistoryId;
            mainInfo.creator_name = LoginInfo.empName;
            mainInfo.create_time = now;
            mainInfo.status = 0;
            mainInfo.company_id = companyInfo.id;
            mainInfo.company_linkname = linkName;
            mainInfo.company_id_parent = companyParentId;
            importList.ForEach(a =>
            {
                a.import_file_id = mainInfo.id; a.company_id = companyInfo.id; a.effect_status = -2;
                a.company_linkname = linkName; a.company_id_parent = companyParentId;
            });
            insertMainList.Add(mainInfo);
            if (companyList.Count == 1)
                insertImportList = importList;
            else
            {
                insertImportList.AddRange(importList);

                for (int i = 1; i < companyList.Count; i++)
                {
                    string guid = Common.GuId();
                    int companyId = companyList[i].id;
                    string linkName1 = companyNamePrefix + companyList[i].name;
                    daoben_product_price_approve tmpMainInfo = new daoben_product_price_approve()
                    {
                        id = guid,
                        import_file = mainInfo.import_file,
                        creator_job_history_id = LoginInfo.jobHistoryId,
                        creator_name = LoginInfo.empName,
                        create_time = now,
                        status = 0,
                        company_id = companyId,
                        company_linkname = linkName1,
                        company_id_parent = companyParentId,
                    };
                    insertMainList.Add(tmpMainInfo);
                    importList.ForEach(a =>
                    {
                        daoben_product_price info = new daoben_product_price()
                        {
                            model = a.model,
                            color = a.color,
                            price_l2 = a.price_l2,
                            price_l3 = a.price_l3,
                            price_l4 = a.price_l4,
                            ad_fee = a.ad_fee,
                            ad_fee_show = a.ad_fee_show,
                            price_internal = a.price_internal,
                            price_buyout = a.price_buyout,
                            price_wholesale = a.price_wholesale,
                            price_retail = a.price_retail,
                            product_type = a.product_type,
                            special_offer = a.special_offer,
                            high_level = a.high_level,
                            effect_date = a.effect_date,
                            expire_date = a.expire_date,
                            effect_status = a.effect_status,
                            import_file_id = guid,
                            company_id = companyId,
                            company_linkname = linkName1,
                            company_id_parent = companyParentId
                        };
                        insertImportList.Add(info);
                    });
                }
            }

            using (var db = SugarDao.GetInstance())
            {
                db.CommandTimeOut = 300;
                try
                {
                    #region 待办事项 财务经理
                    //待办事项 收件人
                    string taskStr = "价格信息待审批";
                    List<string> taskIdList = db.Queryable<daoben_hr_emp_job>()
                                      .JoinTable<daoben_ms_account>((a, b) => a.id == b.employee_id)
                                      .Where<daoben_ms_account>((a, b) => b.role_id == ConstData.ROLE_ID_FINANCIALMANAGER)
                                      .Where(a => a.company_id == myCompanyInfo.id)
                                      .Select<string>("a.id as id").ToList();
                    //待办事项 生成列表
                    List<daoben_sys_task> taskTotal = new List<daoben_sys_task>();
                    foreach (var a in insertMainList)
                    {
                        List<daoben_sys_task> taskList = taskIdList
                            .Select(a1 => new daoben_sys_task
                            {
                                emp_id = a1,
                                category = 1,
                                main_id = a.id,
                                main_url = "/ProductManage/PriceInfo/Approve?id=" + a.id,
                                title = taskStr,
                                content_abstract = a.import_file,
                                recipient_type = 1,
                                create_time = now,
                                status = 1
                            }).ToList();
                        taskTotal.AddRange(taskList);
                    }
                    #endregion


                    db.BeginTran();
                    db.InsertRange(insertMainList);
                    db.DisableInsertColumns = new string[] { "id" };
                    if (insertImportList.Count > 25)
                        db.SqlBulkCopy(insertImportList);
                    else
                        db.InsertRange(insertImportList);
                    //待办事项 列表插入                    
                    if (taskTotal != null && taskTotal.Count() > 25)
                        db.SqlBulkCopy(taskTotal);
                    else if (taskTotal != null && taskTotal.Count() > 0)
                        db.InsertRange(taskTotal);
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
        /// 设置价格信息状态，审批通过时调用，数据库定时任务保留相同函数
        /// </summary>
        public void SetStatus()
        {
            DateTime now = DateTime.Now;
            DateTime yesterday = now.AddDays(-1);   // 失效时间为当天24:00
            // 以下SQL操作需按顺序执行
            // 1、删除所有将被另一个长期价格替代的信息。
            StringBuilder sqlSb1 = new StringBuilder("delete a from daoben_product_price_effect a inner join ");
            sqlSb1.AppendFormat("(select model, color, company_id from daoben_product_price where effect_status=-1 and effect_date<='{0}' ", now);
            sqlSb1.Append("and expire_date='2099-01-01') b on a.model=b.model and a.color=b.color and a.company_id=b.company_id");
            // 2、删除所有过期信息
            string sqlStr2 = "delete from daoben_product_price_effect where expire_date<='" + yesterday.ToString() + "'";
            // 3、将所有信息置为“失效”状态
            string sqlStr3 = "update daoben_product_price_effect set is_effect=0 where is_effect=1";
            // 4、添加新生效的信息
            StringBuilder sqlSb4 = new StringBuilder("insert into daoben_product_price_effect(model, color, price_l2,");
            sqlSb4.Append("price_l3, price_l4,ad_fee,ad_fee_show, price_internal, price_buyout, price_wholesale, price_retail, ");
            sqlSb4.Append("product_type, special_offer, high_level, effect_date, expire_date, company_id, company_linkname, company_id_parent, is_effect) ");
            sqlSb4.Append("(select model, color, price_l2,");
            sqlSb4.Append("price_l3, price_l4,ad_fee,ad_fee_show, price_internal, price_buyout, price_wholesale, price_retail, ");
            sqlSb4.Append("product_type, special_offer, high_level, effect_date, expire_date, company_id, company_linkname, company_id_parent, 0 as is_effect ");
            sqlSb4.AppendFormat("from daoben_product_price where effect_status=-1 and effect_date<='{0}' and expire_date>='{1}' )", now, yesterday);
            // 5、排除重复的model-color-company后将剩余的设为生效
            StringBuilder sqlSb5 = new StringBuilder("update daoben_product_price_effect a inner join ");
            sqlSb5.Append("(select MAX(id) as id from daoben_product_price_effect group by model,color,company_id) b on a.id=b.id ");
            sqlSb5.Append("set a.is_effect=1");
            // 6、daoben_product_price：将被另一个长期价格替代的信息置为“失效”状态
            StringBuilder sqlSb6 = new StringBuilder("update daoben_product_price a inner join ");
            sqlSb6.AppendFormat("(select model, color, company_id from daoben_product_price where effect_status=-1 and effect_date<='{0}' ", now);
            sqlSb6.Append("and expire_date='2099-01-01') b on a.model=b.model and a.color=b.color and a.company_id=b.company_id and a.effect_status=1 ");
            sqlSb6.AppendFormat("set a.effect_status=2,a.expire_date='{0}'", now.Date);
            // 7、daoben_product_price：将所有过期信息置为“失效”状态
            string sqlStr7 = "update daoben_product_price set effect_status=2 where effect_status=1 and expire_date<='" + yesterday.ToString() + "'";
            // 8、将所有生效信息置为“生效”状态
            string sqlStr8 = "update daoben_product_price set effect_status=1 where effect_status=-1 and effect_date<='"
                    + now.ToString() + "' and expire_date>='" + yesterday.ToString() + "'";

            using (SqlSugarClient db = SugarDao.GetInstance())
            {
                try
                {
                    db.CommandTimeOut = 150;
                    db.BeginTran();

                    db.SqlQuery<int>(sqlSb1.ToString());
                    db.SqlQuery<int>(sqlStr2);
                    db.SqlQuery<int>(sqlStr3);
                    db.SqlQuery<int>(sqlSb4.ToString());
                    db.SqlQuery<int>(sqlSb5.ToString());

                    db.SqlQuery<int>(sqlSb6.ToString());
                    db.SqlQuery<int>(sqlStr7);
                    db.SqlQuery<int>(sqlStr8);

                    db.CommitTran();
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    ExceptionApp.WriteLog("PriceInfoApp(SetStatus)：" + ex.Message);
                }
            }
        }

        public string Approve(daoben_product_price_approve approveInfo)
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
                    daoben_product_price_approve origInfo = db.Queryable<daoben_product_price_approve>().InSingle(approveInfo.id);
                    if (origInfo == null || origInfo.status < 0 || origInfo.status == 100)
                        return "信息错误：指定的申请信息不存在或已审批";

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
                    //清除之前待办事项
                    List<daoben_sys_task> origTask = db.Queryable<daoben_sys_task>().Where(a => a.main_id == approveInfo.id).ToList();
                    object upObjForTask = new
                    {
                        status = 3,
                        finished_time = now
                    };
                    if (origTask.Count() > 0)
                        db.Update<daoben_sys_task>(upObjForTask, a => a.main_id == approveInfo.id);
                    #region 消息通知
                    List<daoben_sys_notification> newsTotal = new List<daoben_sys_notification>();//消息通知
                    string newsStr = null;
                    List<string> newsIdList = null;
                    daoben_product_price_approve mainInfo = db.Queryable<daoben_product_price_approve>().InSingle(approveInfo.id);
                    newsIdList = db.Queryable<daoben_hr_emp_job_history>()
                                .Where(a => a.id == mainInfo.creator_job_history_id).Select<string>("emp_id as id").ToList();

                    if (newsIdList != null && newsIdList.Count > 0)
                    {
                        if (approveInfo.status < 0)
                            newsStr = mainInfo.company_linkname + " 价格导入没有通过";
                        else if (approveInfo.status > 0)
                            newsStr = mainInfo.company_linkname + " 价格导入已通过";
                        //消息通知 生成列表
                        List<daoben_sys_notification> newsList = newsIdList
                                .Select(a1 => new daoben_sys_notification
                                {
                                    emp_id = a1,
                                    category = 2,
                                    main_id = mainInfo.id,
                                    main_url = "/ProductManage/PriceInfo/Show?id=" + mainInfo.id,
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
                    db.Update<daoben_product_price_approve>(upObj, a => a.id == approveInfo.id);
                    if (approveInfo.status == 100)
                    {
                        db.Update<daoben_product_price>(new { effect_status = 2 }, a => a.expire_date < DateTime.Now.Date && a.import_file_id == approveInfo.id);
                        db.Update<daoben_product_price>(new { effect_status = -1 }, a => a.import_file_id == approveInfo.id && a.effect_status == -2);
                    }
                    else if (approveInfo.status == -100)
                        db.Update<daoben_product_price>(new { effect_status = -1 }, a => a.import_file_id == approveInfo.id && a.effect_status == -2);
                }
                catch (Exception ex)
                {
                    return "系统出错：" + ex.Message;
                }
            }
            if (approveInfo.status == 100)
                System.Threading.ThreadPool.QueueUserWorkItem(t => { SetStatus(); });
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
                    string mainInfoStr = db.Queryable<daoben_product_price_approve>()
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
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "effect_date" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    var qable = db.Queryable<daoben_product_price>().Where(a => a.import_file_id == id);
                    if (!string.IsNullOrEmpty(queryStr))
                        qable.Where(a => a.model.Contains(queryStr));

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
        //public string GetProductTree()
        //{
        //    using (var db = SugarDao.GetInstance())
        //    {
        //        try
        //        {
        //            List<ProductKeyInfo> infoList = db.Queryable<daoben_product_price_effect>()
        //                    .Where(a => a.is_effect == true).GroupBy(a => a.model).OrderBy(a => a.id)
        //                    .Select<ProductKeyInfo>("id, model as name, product_type, price_buyout").ToList();

        //            List<ProductTypeTree> productTree = infoList.GroupBy(a => a.product_type)
        //                    .Select(b => new ProductTypeTree
        //                    {
        //                        product_type = b.Key,
        //                        product_list = b.ToList()
        //                    }).ToList();
        //            return productTree.ToJson();
        //        }
        //        catch (Exception ex)
        //        {
        //            return "系统出错：" + ex.Message;
        //        }
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId">分公司ID</param>
        /// <returns></returns>
        public string GetEffectIdNameList(int companyId)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            if (companyId == 0 && LoginInfo.companyInfo.category == "分公司")
                companyId = LoginInfo.companyInfo.id;
            if (companyId < 1)
                return null;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    string pairListStr = db.Queryable<daoben_product_price_effect>()
                            .Where(a => a.is_effect == true && a.company_id == companyId)
                            .OrderBy(a => a.id)
                            .Select("id, model, color, price_buyout, price_wholesale").ToJson();
                    return pairListStr;
                }
                catch (Exception ex)
                {
                    return "系统出错：" + ex.Message;
                }
            }
        }
        public string GetColorPriceInfo(string queryInfo, int type)
        {

            using (var db = SugarDao.GetInstance())
            {
                string mainInfo = null;
                if (type == 1)
                {
                    if (string.IsNullOrEmpty(queryInfo))
                        return "信息错误：需要查询的信息为空";
                    mainInfo = db.Queryable<daoben_product_price_effect>()
                           .JoinTable<daoben_sale_outstorage>((a, b) => a.model == b.model && a.color == b.color)
                           .JoinTable<daoben_sale_outstorage, daoben_sale_outstorage_approve>((a, b, c) => b.import_file_id == c.id)
                           .Where<daoben_sale_outstorage, daoben_sale_outstorage_approve>((a, b, c) => b.phone_sn == queryInfo && a.is_effect == true && c.status == 100)
                           .Select("a.model,a.color,a.price_buyout").ToJson();
                    return mainInfo;
                }
                if (type == 2)
                {
                    List<ProductKeyInfo> mainInfoList = db.Queryable<daoben_product_price_effect>()
                            .Where(a => a.is_effect == true)
                            .Select<ProductKeyInfo>("id,model as name,color,price_buyout").ToList();
                    List<ProductModelTree> productTree = mainInfoList.GroupBy(a => a.name)
                           .Select(b => new ProductModelTree
                           {
                               name = b.Key,
                               product_list = b.ToList()
                           }).ToList();
                    return productTree.ToJson();
                }
                return mainInfo;
            }
        }

        internal string Delete(string id)
        {
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_product_price_approve mainInfo = db.Queryable<daoben_product_price_approve>().SingleOrDefault(a => a.id == id);
                    if (mainInfo == null || mainInfo.status != 0) //审批过后，将不能撤回
                        return "撤回失败：指定的申请信息不存在或者已被审批";

                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    db.Delete<daoben_product_price>(a => a.import_file_id == id);
                    db.Delete<daoben_product_price_approve>(a => a.id == id);
                    db.CommitTran();
                    return "success";
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return "系统出错：" + ex.Message;
                }
            }
        }
    }
}
