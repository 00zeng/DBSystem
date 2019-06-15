using Base.Code;
using MySqlSugar;
using ProjectShare.Database;
using ProjectShare.Process;
using System;
using System.Linq;
using System.Collections.Generic;
using ProjectShare.Models;

namespace ProjectWeb.Areas.SaleManage.Application
{
    public class BuyoutInfoApp
    {
        /// <summary>
        /// 查看全部（串码列表）
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryInfo"></param>
        /// <param name="requestInfo"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public object GetListAll(Pagination pagination, daoben_sale_buyout queryInfo, daoben_sale_buyout_request requestInfo)
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
            string selectStr = "b.category, b.distributor_name, b.guide_name, b.sales_name, b.area_name, b.company_name, b.create_time,"
                        + "a.id, a.main_id, a.model, a.color, a.phone_sn, a.price_wholesale, a.price_buyout, a.price_retail";
            using (var db = SugarDao.GetInstance())
            {
                //查看全部
                var qable = db.Queryable<daoben_sale_buyout>()
                        .JoinTable<daoben_sale_buyout_request>((a, b) => a.main_id == b.id)
                        .Where<daoben_sale_buyout_request>((a, b) => b.approve_status == 100);
                if (LoginInfo.roleId == ConstData.ROLE_ID_DISTRIBUTOR)  // 隐藏提成信息
                {
                    qable.Where<daoben_sale_buyout_request>((a, b) => b.distributor_id == LoginInfo.empId);
                    selectStr += ", a.buyout_refund";
                }
                if (myCompanyInfo.category == "分公司")
                {
                    if (LoginInfo.roleId == ConstData.ROLE_ID_SALESMANAGER)
                    {
                        qable.Where<daoben_sale_buyout_request>((a, b) => b.area_id == myPositionInfo.areaL2Id);//TODO区域 yajun
                        selectStr += ", a.buyout_refund, a.guide_commission, a.sales_commission";
                    }
                    else if (LoginInfo.roleId == ConstData.ROLE_ID_SALES)
                    {
                        qable.Where<daoben_sale_buyout_request>((a, b) => b.sales_id == LoginInfo.empId);
                        selectStr += ", a.buyout_refund, a.guide_commission, a.sales_commission";
                    }
                    else if (LoginInfo.roleId == ConstData.ROLE_ID_GUIDE)
                    {
                        qable.Where<daoben_sale_buyout_request>((a, b) => b.guide_id == LoginInfo.empId);
                        selectStr += ", a.buyout_refund";
                    }
                    else
                    {
                        qable.Where<daoben_sale_buyout_request>((a, b) => b.company_id == myCompanyInfo.id);
                        selectStr += ", a.buyout_refund, a.guide_commission, a.sales_commission";
                    }
                }
                else if (myCompanyInfo.category == "事业部")
                {
                    qable.Where<daoben_sale_buyout_request>((a, b) => b.company_id_parent == myCompanyInfo.id);
                    selectStr += ", a.buyout_refund, a.guide_commission, a.sales_commission";
                }
                else if (myCompanyInfo.category != "董事会" && LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                    return null;
                else
                    selectStr += ", a.buyout_refund, a.guide_commission, a.sales_commission";

                if (requestInfo != null)
                {
                    if (!string.IsNullOrEmpty(requestInfo.distributor_name))//经销商
                        qable.Where<daoben_sale_buyout_request>((a, b) => b.distributor_name.Contains(requestInfo.distributor_name));
                    if (!string.IsNullOrEmpty(requestInfo.sales_name))//业务员
                        qable.Where<daoben_sale_buyout_request>((a, b) => b.sales_name.Contains(requestInfo.sales_name));
                    if (!string.IsNullOrEmpty(requestInfo.area_name))//区域
                        qable.Where<daoben_sale_buyout_request>((a, b) => b.area_name.Contains(requestInfo.area_name));
                    if (requestInfo.category != 0)//买断类型
                        qable.Where<daoben_sale_buyout_request>((a, b) => b.category == requestInfo.category);
                    if (!string.IsNullOrEmpty(requestInfo.guide_name))//促销员（导购员）
                        qable.Where<daoben_sale_buyout_request>((a, b) => b.guide_name.Contains(requestInfo.guide_name));
                }
                if (queryInfo != null)
                {
                    if (!string.IsNullOrEmpty(queryInfo.model))//型号
                        qable.Where(a => a.model.Contains(queryInfo.model));
                    if (!string.IsNullOrEmpty(queryInfo.phone_sn))//串码
                        qable.Where(a => a.phone_sn.Contains(queryInfo.phone_sn));
                }

                string listStr = qable.Select(selectStr).OrderBy(pagination.sidx + " " + pagination.sord)
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }
        public object GetListRequest(Pagination pagination, string distributorName, string salesName, bool isMyRequest)
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
                //查看全部
                var qable = db.Queryable<daoben_sale_buyout_request>();
                if (isMyRequest)
                    qable.Where(a => a.creator_job_history_id == LoginInfo.jobHistoryId);
                else
                {
                    if (LoginInfo.roleId == ConstData.ROLE_ID_DISTRIBUTOR)
                        qable.Where(a => a.distributor_id == LoginInfo.empId);
                    if (myCompanyInfo.category == "分公司")
                    {
                        if (LoginInfo.roleId == ConstData.ROLE_ID_SALESMANAGER)
                            qable.Where(a => a.area_id == myPositionInfo.areaL2Id);//TODO 区域
                        else if (LoginInfo.roleId == ConstData.ROLE_ID_SALES)
                            qable.Where(a => a.sales_id == LoginInfo.empId);
                        else if (LoginInfo.roleId == ConstData.ROLE_ID_GUIDE)
                            qable.Where(a => a.guide_id == LoginInfo.empId);
                        else
                            qable.Where(a => a.company_id == myCompanyInfo.id);
                    }
                    else if (myCompanyInfo.category == "事业部")
                        qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                    else if (myCompanyInfo.category != "董事会" && LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                        return null;

                    if (!string.IsNullOrEmpty(distributorName))
                        qable.Where(a => a.distributor_name.Contains(distributorName));
                    if (!string.IsNullOrEmpty(salesName))
                        qable.Where(a => a.sales_name.Contains(salesName));
                }
                string listStr = qable.Select("id, category, distributor_id, distributor_name, guide_id, guide_name, sales_id, sales_name, approve_status, area_id, area_name, company_id, company_name, create_time")
                        .OrderBy(pagination.sidx + " " + pagination.sord)
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }
        public object GetListApprove(Pagination pagination, string distributorName, string salesName)
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
                //我的审批
                var qable = db.Queryable<daoben_sale_buyout_request>();
                //门店买断审批：
                //2-分公司助理确认，给出业务员提成，是否需要向上级请示（默认不通过）-没审批扣钱 
                //3-分公司经理审批（默认通过）TODO 怎么给他默认通过
                //4-总经理审批（默认通过）
                //5-财务经理确认

                //仓库买断审批：
                //2-分公司经理审批（默认不通过）TODO 怎么给他默认不通过
                if (myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT2)
                {
                    //2-分公司助理确认，给出业务员提成
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                    qable.Where(a => a.category == 2 && a.approve_status == 1);
                }
                else if (myPositionInfo.positionType == ConstData.POSITION_GM2)
                {
                    //3-分公司经理审批(门店)
                    //2-分公司经理审批(仓库)
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                    qable.Where(a => (a.category == 2 && a.approve_status == 2) || (a.category == 1 && a.approve_status == 1));
                }
                else if (myPositionInfo.positionType == ConstData.POSITION_GM1)
                {
                    //4-总经理审批
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                    qable.Where(a => (a.category == 2 && a.approve_status == 3));
                }
                else if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER || LoginInfo.roleId == ConstData.ROLE_ID_FINANCIAL)
                {
                    qable.Where(a => (a.category == 2 && a.approve_status == 4) || a.category == 1 && a.approve_status == 2);
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                }
                else
                    return null;

                if (!string.IsNullOrEmpty(distributorName))
                    qable.Where(a => a.distributor_name.Contains(distributorName));
                if (!string.IsNullOrEmpty(salesName))
                    qable.Where(a => a.sales_name.Contains(salesName));


                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }
        public object GetListConfirm(Pagination pagination, string queryName)
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
                //我的确认
                var qable = db.Queryable<daoben_sale_buyout_request>()
                         .Where(a => a.sales_id == LoginInfo.empId && a.approve_status == 0);

                if (!string.IsNullOrEmpty(queryName))
                    qable.Where(a => a.distributor_name.Contains(queryName));

                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        #region 门店买断
        /// <summary>
        /// 门店买断
        /// </summary>
        /// <param name="addInfo"></param>
        /// <param name="buyoutList"></param>
        /// <returns></returns>
        public string StoreAdd(daoben_sale_buyout_request addInfo, List<daoben_sale_buyout> buyoutList)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            if (addInfo == null)
                return "信息错误，操作失败!";
            if (buyoutList == null || buyoutList.Count < 1)
                return "信息错误：没有买断机型!";
            if (string.IsNullOrEmpty(addInfo.id) || addInfo.id.Length != 36)
                return "信息错误：ID不正确!";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    //登陆者经销商信息 
                    // TODO：申请人可为其他角色
                    daoben_distributor_info distributorInfo = db.Queryable<daoben_distributor_info>().InSingle(LoginInfo.empId);
                    addInfo.approve_status = 0;

                    addInfo.area_id = myPositionInfo.areaL2Id;
                    addInfo.area_name = myPositionInfo.areaL2Name;
                    addInfo.company_id = myCompanyInfo.id;
                    addInfo.company_name = myCompanyInfo.name;
                    addInfo.company_id_parent = myCompanyInfo.parentId;
                    addInfo.creator_job_history_id = LoginInfo.empType == 0 ? LoginInfo.jobHistoryId : LoginInfo.empId;
                    addInfo.creator_name = LoginInfo.empName;
                    addInfo.create_time = DateTime.Now;

                    addInfo.distributor_id = distributorInfo.id;
                    addInfo.distributor_name = distributorInfo.name;
                    addInfo.category = 2;
                    addInfo.sales_id = distributorInfo.sales_id;
                    addInfo.sales_name = distributorInfo.sales_name;

                    db.CommandTimeOut = 60;
                    db.BeginTran();
                    db.Insert(addInfo);
                    if (buyoutList.Count > 25)
                        db.SqlBulkCopy(buyoutList);
                    else
                        db.InsertRange(buyoutList);
                    db.CommitTran();

                    //待办事项 TODO 待测试
                    #region 待办事项 业务员
                    //待办事项 收件人
                    string taskStr = "买断申请待确认";
                    List<string> taskIdList = db.Queryable<daoben_hr_emp_job>()
                        .Where(a => a.id == addInfo.sales_id).Select<string>("id").ToList();
                    //待办事项 生成列表
                    List<daoben_sys_task> taskList = taskIdList
                        .Select(a1 => new daoben_sys_task
                        {
                            emp_id = a1,
                            category = 1,
                            main_id = addInfo.id,
                            main_url = "/SaleManage/BuyoutInfo/Confirm?id=" + addInfo.id,
                            title = taskStr,
                            content_abstract = addInfo.note,
                            recipient_type = 1,
                            create_time = now,
                            status = 1
                        }).ToList();
                    #endregion
                    //待办事项 列表插入
                    if (taskList != null && taskList.Count() < 25)
                        db.InsertRange(taskList);
                    else if (taskList != null && taskList.Count() > 0)
                        db.SqlBulkCopy(taskList);
                    return "success";
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return ex.Message;
                }
            }
        }
        public string StoreApprove(daoben_sale_buyout_request_approve approveInfo, daoben_sale_buyout_request requestInfo, List<daoben_sale_buyout> buyoutList)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            object upObj = null;
            DateTime now = DateTime.Now;
            //门店买断审批：
            //1-业务员确认，给出导购员提成
            //2-分公司助理确认，给出业务员提成，是否需要向上级请示（默认不通过）-没审批扣钱
            //3-分公司经理审批（默认通过）TODO 一天之后，怎么给他默认通过
            //4-总经理审批（默认通过）
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_sale_buyout_request mainInfo = db.Queryable<daoben_sale_buyout_request>().InSingle(approveInfo.main_id);
                    if (mainInfo == null)
                        return "信息错误：指定的申请信息不存在";
                    int approve_status = mainInfo.approve_status;
                    if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER || LoginInfo.roleId == ConstData.ROLE_ID_FINANCIAL)
                    {
                        if (mainInfo.approve_status == 100 || mainInfo.approve_status == -100)
                            return "操作失败：审批流程不正确";
                        approveInfo.status = (approveInfo.status > 0 ? 100 : -100);
                    }
                    else if (myPositionInfo.positionType == ConstData.POSITION_SALES)
                    {
                        if (mainInfo.approve_status != 0)
                            return "操作失败：审批流程不正确";
                        approveInfo.status = (approveInfo.status > 0 ? 1 : -1);
                    }
                    else if (myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT2)
                    {
                        if (mainInfo.approve_status != 1)
                            return "操作失败：审批流程不正确";
                        if (approveInfo.status > 0)
                        {
                            approveInfo.status = 2;
                            // TODO 流程处理，（客户新需求 2018-11-24）
                            if (!requestInfo.gm2_approve)
                                mainInfo.approve_status = requestInfo.gm1_approve ? 3 : 4;
                            else
                                mainInfo.approve_status = approveInfo.status;
                            upObj = new
                            {
                                approve_status = mainInfo.approve_status,
                                gm2_approve = requestInfo.gm2_approve,
                                gm1_approve = requestInfo.gm1_approve
                            };
                        }
                        else
                            approveInfo.status = -2;
                    }
                    else if (myPositionInfo.positionType == ConstData.POSITION_GM2)
                    {
                        if (mainInfo.approve_status != 2)
                            return "操作失败：审批流程不正确";
                        if (approveInfo.status > 0)
                        {
                            approveInfo.status = 3;
                            upObj = new { approve_status = mainInfo.gm1_approve ? 3 : 4 };
                        }
                        else
                            approveInfo.status = -3;
                    }
                    else if (myPositionInfo.positionType == ConstData.POSITION_GM1)
                    {
                        if (mainInfo.approve_status != 3)
                            return "操作失败：审批流程不正确";
                        approveInfo.status = (approveInfo.status > 0 ? 4 : -4);
                    }
                    if (upObj == null)
                        upObj = new { approve_status = approveInfo.status };

                    approveInfo.approve_id = LoginInfo.accountId;
                    approveInfo.approve_name = LoginInfo.empName;
                    approveInfo.approve_time = DateTime.Now;
                    approveInfo.approve_position_id = myPositionInfo.id;
                    approveInfo.approve_position_name = myPositionInfo.name;

                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    db.DisableInsertColumns = new string[] { "id" };
                    db.Insert(approveInfo);
                    db.DisableInsertColumns = null;
                    db.Update<daoben_sale_buyout_request>(upObj, a => a.id == mainInfo.id);
                    if (approve_status == 0 || approve_status == 1)
                    {
                        db.DisableUpdateColumns = new string[] { "main_id",
                        "bill_name", "model", "color", "phone_sn", "price_wholesale", "price_buyout", "price_retail",
                        "buyout_refund", "guide_id", "guide_name", "out_distributor_id", "distributor_name", "sales_id", "sales_name", "area_l1_id",
                        "area_l1_name","area_l2_id","area_l2_name", "sale_status", "accur_time", "import_file_id"};
                        db.UpdateRange(buyoutList);
                        db.DisableUpdateColumns = null;
                    }
                    //else if (approve_status == 1)
                    //{
                    //    db.DisableUpdateColumns = new string[] { "main_id",
                    //    "bill_name", "model", "color", "phone_sn", "price_wholesale", "price_buyout", "price_retail", 
                    //    "buyout_refund", "guide_id", "guide_name", "distributor_id", "distributor_name", "sales_id", "sales_name", "area_id",
                    //    "area_name", "sale_status", "accur_time", "import_file_id"};
                    //    db.UpdateRange(buyoutList);
                    //    db.DisableUpdateColumns = null;
                    //}
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
                    #region 添加下一步待办事项
                    List<daoben_sys_notification> newsTotal = new List<daoben_sys_notification>();//消息通知
                    List<daoben_sys_task> taskTotal = new List<daoben_sys_task>();//待办事项
                    string newsStr = null;
                    List<string> newsIdList = null;
                    string taskStr = null;
                    List<string> taskIdList = null;
                    daoben_sale_buyout_request origInfo = db.Queryable<daoben_sale_buyout_request>().InSingle(approveInfo.main_id);
                    if (origInfo.approve_status == 1)
                        // TODO 待办事项 --分公司助理 
                        taskIdList = db.Queryable<daoben_hr_emp_job>()
                            .Where(a => a.position_type == ConstData.POSITION_GM_ASSISTANT2 && a.company_id == origInfo.company_id)
                            .Select<string>("id").ToList();
                    else if (origInfo.approve_status == 2)
                    {//分总
                        taskIdList = db.Queryable<daoben_hr_emp_job>()
                                .Where(a => a.position_type == ConstData.POSITION_GM2 && a.company_id == origInfo.company_id)
                                .Select<string>("id").ToList();
                    }
                    else if (origInfo.approve_status == 3)
                    {
                        taskIdList = db.Queryable<daoben_hr_emp_job>()
                               .Where(a => a.position_type == ConstData.POSITION_GM1 && a.company_id == origInfo.company_id_parent)
                               .Select<string>("id").ToList();
                    }
                    else if (origInfo.approve_status == 4)
                        taskIdList = db.Queryable<daoben_hr_emp_job>()
                                       .JoinTable<daoben_ms_account>((a, b) => a.id == b.employee_id)
                                       .Where<daoben_ms_account>((a, b) => b.role_id == ConstData.ROLE_ID_FINANCIALMANAGER)
                                       .Where(a => a.company_id == origInfo.company_id_parent)
                                       .Select<string>("a.id as id").ToList();
                    else if (origInfo.approve_status != 0)
                        newsIdList = db.Queryable<daoben_distributor_info>()
                                .Where(a => a.id == mainInfo.creator_job_history_id).Select<string>("id").ToList();

                    taskStr = "买断申请待审批";
                    //待办事项 生成列表
                    if (taskIdList != null && taskIdList.Count > 0)
                    {
                        taskTotal = taskIdList
                            .Select(a1 => new daoben_sys_task
                            {
                                emp_id = a1,
                                category = 1,
                                main_id = origInfo.id,
                                main_url = "/SaleManage/BuyoutInfo/Approve?id=" + origInfo.id,
                                title = taskStr,
                                content_abstract = origInfo.note,
                                recipient_type = 1,
                                create_time = now,
                                status = 1
                            }).ToList();
                    }
                    if (newsIdList != null && newsIdList.Count > 0)
                    {
                        if (origInfo.approve_status < 0)
                            newsStr = "门店买断审批不通过";
                        else if (origInfo.approve_status == 100)
                            newsStr = "门店买断审批通过";
                        //消息通知 生成列表
                        List<daoben_sys_notification> newsList = newsIdList
                                .Select(a1 => new daoben_sys_notification
                                {
                                    emp_id = a1,
                                    category = 2,
                                    main_id = origInfo.id,
                                    main_url = "/SaleManage/BuyoutInfo/Myrequest?id=" + origInfo.id,
                                    title = newsStr,
                                    content_abstract = origInfo.note,
                                    recipient_type = 1,
                                    create_time = now,
                                    status = 1
                                }).ToList();
                        newsTotal.AddRange(newsList);
                    }
                    //待办事项 and 消息通知 列表插入
                    if (newsTotal != null && newsTotal.Count > 25)
                        db.SqlBulkCopy(newsTotal);
                    else if (newsTotal != null && newsTotal.Count > 0)
                        db.InsertRange(newsTotal);
                    if (taskTotal != null && taskTotal.Count > 25)
                        db.SqlBulkCopy(taskTotal);
                    else if (taskTotal != null && taskTotal.Count > 0)
                        db.InsertRange(taskTotal);
                    #endregion 待办事项
                    return "success";
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return "系统出错：" + ex.Message;
                }
            }
        }
        #endregion

        #region 仓库买断
        public string StorageAdd(daoben_sale_buyout_request addInfo, List<daoben_sale_buyout_request_sub> buyoutList)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            if (addInfo == null)
                return "信息错误，操作失败!";
            if (buyoutList == null || buyoutList.Count < 1)
                return "信息错误：没有买断机型!";
            if (string.IsNullOrEmpty(addInfo.id) || addInfo.id.Length != 36)
                return "信息错误：ID不正确!";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    //登陆者经销商信息
                    // TODO：申请人可为其他角色
                    daoben_distributor_info distributorInfo = db.Queryable<daoben_distributor_info>().InSingle(LoginInfo.empId);

                    addInfo.approve_status = 0;

                    addInfo.area_id = myPositionInfo.areaL2Id;
                    addInfo.area_name = myPositionInfo.areaL2Name;
                    addInfo.company_id = myCompanyInfo.id;
                    addInfo.company_name = myCompanyInfo.name;
                    addInfo.company_id_parent = myCompanyInfo.parentId;

                    addInfo.creator_job_history_id = LoginInfo.empType == 0 ? LoginInfo.jobHistoryId : LoginInfo.empId;
                    addInfo.creator_name = LoginInfo.empName;
                    addInfo.create_time = DateTime.Now;

                    addInfo.distributor_id = distributorInfo.id;
                    addInfo.distributor_name = distributorInfo.name;
                    addInfo.category = 1;
                    addInfo.sales_id = distributorInfo.sales_id;
                    addInfo.sales_name = distributorInfo.sales_name;
                    //待办事项 TODO 待测试
                    #region 待办事项 业务员
                    //待办事项 收件人
                    string tempStr = "买断申请待确认";
                    List<string> idList = db.Queryable<daoben_hr_emp_job>()
                        .Where(a => a.id == addInfo.sales_id).Select<string>("id").ToList();
                    //待办事项 生成列表
                    List<daoben_sys_task> taskList = idList
                        .Select(a1 => new daoben_sys_task
                        {
                            emp_id = a1,
                            category = 1,
                            main_id = addInfo.id,
                            main_url = "/SaleManage/BuyoutInfo/Confirm?id=" + addInfo.id,
                            title = tempStr,
                            content_abstract = addInfo.note,
                            recipient_type = 1,
                            create_time = now,
                            status = 1
                        }).ToList();

                    #endregion
                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    db.Insert(addInfo);
                    if (buyoutList != null && buyoutList.Count > 0)
                    {
                        if (buyoutList.Count > 25)
                            db.SqlBulkCopy(buyoutList);
                        else
                            db.InsertRange(buyoutList);
                    }
                    //待办事项 列表插入
                    if (taskList != null && taskList.Count() >= 25)
                        db.SqlBulkCopy(taskList);
                    else if (taskList != null && taskList.Count() > 0)
                        db.InsertRange(taskList);
                    db.CommitTran();
                    return "success";
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return ex.Message;
                }
            }
        }
        public string StorageApprove(daoben_sale_buyout_request_approve approveInfo)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            //仓库买断：
            //0-经销商发起申请（型号，数量，价格）;
            //1-业务员确认;价格符合最低买断价则无需 -2- 审批
            //2-分公司经理审批， 次日默认不通过
            //3-财务经理确认
            //4-仓库导入出库单,财务文员？THEN 审批此出库单
            //5-门店实销，实销售表导入
            //6-系统匹配串码一一计算
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_sale_buyout_request mainInfo = db.Queryable<daoben_sale_buyout_request>().InSingle(approveInfo.main_id);
                    if (mainInfo == null)
                        return "信息错误：指定的申请信息不存在";
                    if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER || LoginInfo.roleId == ConstData.ROLE_ID_FINANCIAL)
                    {
                        if (mainInfo.approve_status == 100 || mainInfo.approve_status == -100)
                            return "操作失败：审批流程不正确";
                        approveInfo.status = (approveInfo.status > 0 ? 100 : -100);
                    }
                    else if (myPositionInfo.positionType == ConstData.POSITION_SALES)
                    {
                        if (mainInfo.approve_status != 0)
                            return "操作失败：审批流程不正确";
                        approveInfo.status = (approveInfo.status > 0 ? 1 : -1);
                    }
                    else if (myPositionInfo.positionType == ConstData.POSITION_GM2)
                    {
                        if (mainInfo.approve_status != 1)
                            return "操作失败：审批流程不正确";
                        approveInfo.status = (approveInfo.status > 0 ? 2 : -2);
                    }
                    else
                        return "操作失败：无审批权限";

                    approveInfo.approve_id = LoginInfo.accountId;
                    approveInfo.approve_name = LoginInfo.empName;
                    approveInfo.approve_time = DateTime.Now;
                    approveInfo.approve_position_id = myPositionInfo.id;
                    approveInfo.approve_position_name = myPositionInfo.name;
                    //清除之前待办事项
                    List<daoben_sys_task> origTask = db.Queryable<daoben_sys_task>().Where(a => a.main_id == approveInfo.main_id).ToList();
                    object upObjForTask = new
                    {
                        status = 3,
                        finished_time = now
                    };
                    if (origTask.Count() > 0)
                        db.Update<daoben_sys_task>(upObjForTask, a => a.main_id == approveInfo.main_id);
                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    db.DisableInsertColumns = new string[] { "id" };
                    db.Insert(approveInfo);
                    db.Update<daoben_sale_buyout_request>(new { approve_status = approveInfo.status }, a => a.id == mainInfo.id);
                    db.CommitTran();

                    #region 待办事项
                    List<daoben_sys_notification> newsTotal = new List<daoben_sys_notification>();//消息通知
                    List<daoben_sys_task> taskTotal = new List<daoben_sys_task>();//待办事项
                    string newsStr = null;
                    List<string> newsIdList = null;
                    string taskStr = null;
                    List<string> taskIdList = null;
                    daoben_sale_buyout_request origInfo = db.Queryable<daoben_sale_buyout_request>().InSingle(approveInfo.main_id);
                    if (origInfo.approve_status == 1)
                        // TODO 待办事项 --分公司助理 
                        taskIdList = db.Queryable<daoben_hr_emp_job>()
                            .Where(a => a.position_type == ConstData.POSITION_GM2 && a.company_id == origInfo.company_id)
                            .Select<string>("id").ToList();
                    else if (origInfo.approve_status == 2)
                        taskIdList = db.Queryable<daoben_hr_emp_job>()
                                    .JoinTable<daoben_ms_account>((a, b) => a.id == b.employee_id)
                                    .Where<daoben_ms_account>((a, b) => b.role_id == ConstData.ROLE_ID_FINANCIALMANAGER)
                                    .Where(a => a.company_id == origInfo.company_id_parent)
                                    .Select<string>("a.id as id").ToList();
                    else if (origInfo.approve_status != 0)
                        newsIdList = db.Queryable<daoben_distributor_info>()
                                .Where(a => a.id == origInfo.creator_job_history_id).Select<string>("id").ToList();

                    taskStr = "买断申请待审批";
                    //待办事项 生成列表
                    if (taskIdList != null && taskIdList.Count > 0)
                    {
                        taskTotal = taskIdList
                            .Select(a1 => new daoben_sys_task
                            {
                                emp_id = a1,
                                category = 1,
                                main_id = origInfo.id,
                                main_url = "/SaleManage/BuyoutInfo/Approve?id=" + origInfo.id,
                                title = taskStr,
                                content_abstract = origInfo.note,
                                recipient_type = 1,
                                create_time = now,
                                status = 1
                            }).ToList();
                    }
                    if (newsIdList != null && newsIdList.Count > 0)
                    {
                        if (origInfo.approve_status < 0)
                            newsStr = "仓库买断审批不通过";
                        else if (origInfo.approve_status == 100)
                            newsStr = "仓库买断审批通过";
                        //消息通知 生成列表
                        List<daoben_sys_notification> newsList = newsIdList
                                .Select(a1 => new daoben_sys_notification
                                {
                                    emp_id = a1,
                                    category = 2,
                                    main_id = origInfo.id,
                                    main_url = "/SaleManage/BuyoutInfo/Myrequest?id=" + origInfo.id,
                                    title = newsStr,
                                    content_abstract = origInfo.note,
                                    recipient_type = 1,
                                    create_time = now,
                                    status = 1
                                }).ToList();
                        newsTotal.AddRange(newsList);
                    }
                    //待办事项 and 消息通知 列表插入
                    if (newsTotal != null && newsTotal.Count > 25)
                        db.SqlBulkCopy(newsTotal);
                    else if (newsTotal != null && newsTotal.Count > 0)
                        db.InsertRange(newsTotal);
                    if (taskTotal != null && taskTotal.Count > 25)
                        db.SqlBulkCopy(taskTotal);
                    else if (taskTotal != null && taskTotal.Count > 0)
                        db.InsertRange(taskTotal);
                    #endregion 待办事项
                    return "success";
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return "系统出错：" + ex.Message;
                }
            }
        }
        #endregion

        public string GetBuyoutInfo(string id)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_sale_buyout_request mainInfo = db.Queryable<daoben_sale_buyout_request>().SingleOrDefault(a => a.id == id);
                    if (mainInfo == null)
                        return "信息错误：指定的申请信息不存在";
                    List<daoben_sale_buyout> buyoutInfoList = db.Queryable<daoben_sale_buyout>()
                        .Where(a => a.main_id == mainInfo.id).ToList();
                    List<daoben_sale_buyout_request_approve> approveInfoList = db.Queryable<daoben_sale_buyout_request_approve>()
                        .Where(a => a.main_id == mainInfo.id).ToList();
                    List<daoben_sale_buyout_request_sub> subInfoList = db.Queryable<daoben_sale_buyout_request_sub>()
                        .Where(a => a.main_id == mainInfo.id).ToList();
                    daoben_hr_emp_job_history tempInfo = db.Queryable<daoben_hr_emp_job_history>().SingleOrDefault(a => a.id == mainInfo.creator_job_history_id);
                    string creator_position_name = null;
                    if (tempInfo != null)
                        creator_position_name = tempInfo.position_name;
                    object resultObj = new
                    {
                        mainInfo = mainInfo,
                        buyoutInfoList = buyoutInfoList,
                        approveInfoList = approveInfoList,
                        subInfoList = subInfoList,
                        creator_position_name = creator_position_name
                    };
                    return resultObj.ToJson();
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
                    daoben_sale_buyout_request mainInfo = db.Queryable<daoben_sale_buyout_request>().SingleOrDefault(a => a.id == id);
                    if (mainInfo == null || mainInfo.approve_status != 0)
                        //财务经理审批过后，将不能撤回
                        return "撤回失败：指定的申请信息不存在或者已被审批";
                    else
                    {
                        db.CommandTimeOut = 30;
                        db.BeginTran();
                        db.Delete<daoben_sale_buyout_request>(a => a.id == id);
                        db.Delete<daoben_sale_buyout>(a => a.import_file_id == id);
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

        /// <summary>
        /// 获取业务员和导购员列表
        /// </summary>
        /// <param name="distributorId"></param>
        /// <returns></returns>
        public string GetSalesInfo(string distributorId = null)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            if (distributorId == null)
                distributorId = LoginInfo.empId;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_distributor_info mainInfo = db.Queryable<daoben_distributor_info>()
                            .Where(a => a.id == distributorId).Select("id, name, sales_id, sales_name").SingleOrDefault();
                    List<IdNamePair> guideList = db.Queryable<daoben_distributor_guide>()
                            .Where(a => a.distributor_id == distributorId && a.inactive == false)
                            .Select<IdNamePair>("guide_id as id, guide_name as name").ToList();
                    object resultObj = new
                    {
                        id = mainInfo.id,
                        name = mainInfo.name,
                        sales_id = mainInfo.sales_id,
                        sales_name = mainInfo.sales_name,
                        guideList = guideList
                    };
                    return resultObj.ToJson();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
    }
}
