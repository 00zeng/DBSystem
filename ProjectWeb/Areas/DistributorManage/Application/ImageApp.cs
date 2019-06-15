using Base.Code;
using Base.Code.Security;
using MySqlSugar;
using ProjectShare.Database;
using ProjectShare.Models;
using ProjectShare.Process;
using ProjectWeb.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ProjectWeb.Areas.DistributorManage.Application
{
    public class ImageApp
    {
        public object AllList(Pagination pagination, daoben_distributor_image queryInfo, int? status, QueryTime queryTime)
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
                var qable = db.Queryable<daoben_distributor_image>();
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
                    if (queryInfo.distributor_name != null)
                        qable.Where(a => a.distributor_name.Contains(queryInfo.distributor_name));
                    if (queryInfo.activity_status != 0)
                        qable.Where(a => a.activity_status == queryInfo.activity_status);
                    if (queryInfo.target_mode != 0)
                        qable.Where(a => a.target_mode == queryInfo.target_mode);
                    if (queryInfo.company_id != 0)
                        qable.Where(a => a.company_id == queryInfo.company_id);
                    //审批状态
                    //if (status != null)                   
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
                        qable.Where(a => a.create_time >= queryTime.endTime1);
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
        public object ApproveList(Pagination pagination, daoben_distributor_image queryInfo)
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
                var qable = db.Queryable<daoben_distributor_image>();
                //我的审批
                //分公司助理-分公司总经理-总经理-财务
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (myPositionInfo.positionType == ConstData.POSITION_GM2)//分公司总经理
                    {
                        qable.Where(a => a.company_id == myCompanyInfo.id);
                        qable.Where(a => a.approve_status == 0);
                    }
                    else if (myPositionInfo.positionType == ConstData.POSITION_GM1)//总经理
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
                    if (queryInfo.distributor_name != null)
                        qable.Where(a => a.distributor_name.Contains(queryInfo.distributor_name));
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

        /// <summary>
        /// 从串码表获取销售详情
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryInfo"></param>
        /// <param name="queryMainInfo"></param>
        /// <returns></returns>
        public object GetSaleList(Pagination pagination, daoben_product_sn queryInfo, daoben_distributor_image queryMainInfo, QueryTime queryTime)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (queryMainInfo == null)
                return "信息错误，获取销售详情失败";
            if (queryMainInfo.target_mode == 1)
                return "按时间返利，无需获取销售详情";
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;

            using (var db = SugarDao.GetInstance())
            {
                DateTime startTime = queryMainInfo.start_date.ToDate();
                DateTime endTime = queryMainInfo.end_date.ToDate().AddDays(1);

                var qable = db.Queryable<daoben_product_sn>();                                                        
                string fieldStr = ("a.phone_sn, a.model, a.color, a.price_wholesale, a.price_retail,");
                if (queryMainInfo.target_content==1) //实销
                {
                     qable.JoinTable<daoben_distributor_image>((a, b) => a.sale_distributor_id == b.distributor_id)
                          .Where<daoben_distributor_image>((a, b) => b.id == queryMainInfo.id && a.sale_type > 0
                                                          && a.sale_time >= startTime && a.sale_time < endTime);
                    fieldStr += ("a.sale_time as time");
                    if (queryTime.startTime1 != null) //匹配查询时间
                        qable.Where(a => a.sale_time >= queryTime.startTime1);
                    if (queryTime.endTime1 != null)
                    {
                        queryTime.endTime1 = queryTime.endTime1.ToDate().AddDays(1);
                        qable.Where(a => a.sale_time < queryTime.endTime1);
                    }
                        
                }
                else if (queryMainInfo.target_content==2) //下货
                {
                    qable.JoinTable<daoben_distributor_image>((a, b) => a.out_distributor_id == b.distributor_id)
                         .Where<daoben_distributor_image>((a, b) => b.id == queryMainInfo.id && a.sale_type == 0
                                                          && a.outstorage_time >= startTime && a.outstorage_time < endTime);
                    fieldStr += ("a.outstorage_time as time");
                    if (queryTime.startTime1 != null)
                        qable.Where(a => a.outstorage_time >= queryTime.startTime1);
                    if (queryTime.endTime1 != null)
                    {
                        queryTime.endTime1 = queryTime.endTime1.ToDate().AddDays(1);
                        qable.Where(a => a.outstorage_time < queryTime.endTime1);
                    }                   
                }

                if (queryInfo != null) //匹配查询参数
                {
                    if (!string.IsNullOrEmpty(queryInfo.phone_sn))
                        qable.Where(a => a.phone_sn.Contains(queryInfo.phone_sn));
                    if (!string.IsNullOrEmpty(queryInfo.model))
                        qable.Where(a => a.model.Contains(queryInfo.model));
              
                }
                var listStr = qable.Select(fieldStr)
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
                    daoben_distributor_image mainInfo = db.Queryable<daoben_distributor_image>().SingleOrDefault(a => a.id == id);
                    if (mainInfo == null)
                        return "信息错误：指定的形象返利信息不存在";

                    List<daoben_distributor_image_approve> approveList = db.Queryable<daoben_distributor_image_approve>()
                                .Where(a => a.main_id == id).ToList();

                    List<daoben_distributor_image_file> fileList = db.Queryable<daoben_distributor_image_file>()
                                .Where(a => a.main_id == id).ToList();
                    List<daoben_distributor_image_res> resList = db.Queryable<daoben_distributor_image_res>()
                                .Where(a => a.main_id == id && a.end_date <= DateTime.Now).ToList();

                    object resultObj = new
                    {
                        mainInfo = mainInfo,
                        approveList = approveList,
                        fileList = fileList,
                        resList = resList,
                    };
                    return resultObj.ToJson();

                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
       
        public string Add(daoben_distributor_image addInfo, List<daoben_distributor_image_file> fileList)
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
                    daoben_distributor_info distributorInfo = db.Queryable<daoben_distributor_info>()
                                .SingleOrDefault(a => a.id == addInfo.distributor_id);
                    if (distributorInfo == null)
                        return "信息错误：指定所属经销商不存在";

                    addInfo.distributor_name = distributorInfo.name;
                    addInfo.area_l1_id = distributorInfo.area_l1_id;
                    addInfo.area_l1_name = distributorInfo.area_l1_name;
                    addInfo.area_l2_id = distributorInfo.area_l2_id;
                    addInfo.area_l2_name = distributorInfo.area_l2_name;
                    addInfo.company_id = distributorInfo.company_id;
                    addInfo.company_name = distributorInfo.company_name;
                    addInfo.company_id_parent = distributorInfo.company_id_parent;

                    addInfo.activity_status = -2;

                    addInfo.creator_job_history_id = LoginInfo.jobHistoryId;
                    addInfo.creator_position_name = myPositionInfo.name;
                    addInfo.creator_name = LoginInfo.empName;
                    addInfo.create_time = now;
                    addInfo.id = Common.GuId();
                    if (fileList != null && fileList.Count > 0)
                    {
                        fileList.ForEach(a =>
                        {
                            a.main_id = addInfo.id;
                            a.creator_job_history_id = LoginInfo.jobHistoryId;
                            a.creator_name = LoginInfo.empName;
                            a.create_time = now;
                        });
                    }

                    db.CommandTimeOut = 30;

                    db.BeginTran();
                    db.Insert(addInfo);
                    if (fileList != null && fileList.Count > 0)
                        db.InsertRange(fileList);
                    db.CommitTran();
                    #region 待办事项 分公司总经理
                    //待办事项 收件人
                    string taskStr = "形象返利申请待审批";
                    List<string> taskIdList = db.Queryable<daoben_hr_emp_job>()
                        .Where(a => a.company_id == myCompanyInfo.id && a.position_type == ConstData.POSITION_GM2).Select<string>("id").ToList();
                    //待办事项 生成列表
                    List<daoben_sys_task> taskList = taskIdList
                        .Select(a1 => new daoben_sys_task
                        {
                            emp_id = a1,
                            category = 1,
                            main_id = addInfo.id,
                            main_url = "/DistributorManage/Image/Approve?id=" + addInfo.id + "&name=" + addInfo.distributor_name,
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
                    else if (taskList != null && taskList.Count() >= 25)
                        db.SqlBulkCopy(taskList);

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
        public string Alter(string id, DateTime alterDate)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (string.IsNullOrEmpty(id))
                return "信息错误，操作失败!";
            daoben_distributor_image_alter alterInfo = new daoben_distributor_image_alter
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
                    daoben_distributor_image mainInfo = db.Queryable<daoben_distributor_image>().SingleOrDefault(a => a.id == id);
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
                    db.Update<daoben_distributor_image>(upObj, a => a.id == id);
                    db.Update<daoben_distributor_image_alter>(new { approve_status = 100 }, a => a.id == alterInfo.id);

                    if (activity_status < 0)
                        return "success";
                    ImageRebateCalculate(id);
                    #endregion
                    return "success";
                }
            }
            catch (Exception ex)
            {
                return "系统出错：" + ex.Message;
            }
        }
        public string Approve(daoben_distributor_image_approve approveInfo)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            Object upObj = null;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_distributor_image mainInfo = db.Queryable<daoben_distributor_image>().SingleOrDefault(a => a.id == approveInfo.main_id);
                    if (mainInfo == null)
                        return "信息错误：指定的申请信息不存在";
                    if ((LoginInfo.roleId == ConstData.ROLE_ID_FINANCIAL || LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER) && approveInfo.status > 0)
                    {
                        approveInfo.status = mainInfo.approve_status = 100;
                        if (mainInfo.start_date <= DateTime.Now)
                            upObj = new { approve_status = 100, activity_status = 1 };
                        else
                            upObj = new { approve_status = 100, activity_status = -1 };
                    }
                    else
                    {
                        if (approveInfo.status > 0)
                        {
                            mainInfo.approve_status = 0 + 1 + mainInfo.approve_status;
                            approveInfo.status = mainInfo.approve_status;
                        }
                        else
                        {
                            mainInfo.approve_status = 0 - 1 - mainInfo.approve_status;
                            approveInfo.status = mainInfo.approve_status;
                        }
                        upObj = new { approve_status = mainInfo.approve_status };
                    }
                    approveInfo.approve_id = LoginInfo.accountId;
                    approveInfo.approve_name = LoginInfo.empName;
                    approveInfo.approve_time = DateTime.Now;
                    approveInfo.approve_position_id = myPositionInfo.id;
                    approveInfo.approve_position_name = myPositionInfo.name;

                    db.CommandTimeOut = 30;
                    db.BeginTran();

                    db.Update<daoben_distributor_image>(upObj, a => a.id == mainInfo.id);
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

                    #region 添加下一步待办事项
                    List<daoben_sys_notification> newsTotal = new List<daoben_sys_notification>();//消息通知
                    List<daoben_sys_task> taskTotal = new List<daoben_sys_task>();//待办事项
                    string newsStr = null;
                    List<string> newsIdList = null;
                    string taskStr = null;
                    List<string> taskIdList = null;
                    //分公司助理(申请)-0分公司总经理1-1总经理2-2财务100
                    daoben_distributor_image origInfo = db.Queryable<daoben_distributor_image>().SingleOrDefault(a => a.id == approveInfo.main_id);
                    if (origInfo.approve_status == 1)
                        // TODO 待办事项 --事业部总经理
                        taskIdList = db.Queryable<daoben_hr_emp_job>()
                            .Where(a => a.position_type == ConstData.POSITION_GM1 && a.company_id == origInfo.company_id_parent)
                            .Select<string>("id").ToList();
                    else if (origInfo.approve_status == 2)
                    {
                        //财务审批
                        taskIdList = db.Queryable<daoben_hr_emp_job>()
                                    .JoinTable<daoben_ms_account>((a, b) => a.id == b.employee_id)
                                    .Where<daoben_ms_account>((a, b) => b.role_id == ConstData.ROLE_ID_FINANCIALMANAGER)
                                    .Where(a => a.company_id == origInfo.company_id_parent)
                                    .Select<string>("a.id as id").ToList();
                    }
                    else if (origInfo.approve_status != 0)
                        newsIdList = db.Queryable<daoben_hr_emp_job_history>()
                                .Where(a => a.id == origInfo.creator_job_history_id).Select<string>("emp_id as id").ToList();

                    taskStr = "形象返利申请待审批";
                    //待办事项 生成列表
                    if (taskIdList != null && taskIdList.Count > 0)
                    {
                        taskTotal = taskIdList
                            .Select(a1 => new daoben_sys_task
                            {
                                emp_id = a1,
                                category = 1,
                                main_id = origInfo.id,
                                main_url = "/DistributorManage/Image/Approve?id=" + origInfo.id + "&name=" + origInfo.distributor_name,
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
                            newsStr = "形象返利申请没有通过";
                        else if (origInfo.approve_status == 100)
                            newsStr = "形象返利申请已通过";
                        //消息通知 生成列表
                        List<daoben_sys_notification> newsList = newsIdList
                                .Select(a1 => new daoben_sys_notification
                                {
                                    emp_id = a1,
                                    category = 2,
                                    main_id = origInfo.id,
                                    main_url = "/DistributorManage/Image/Show?id=" + origInfo.id + "&name=" + origInfo.distributor_name,
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


                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return ex.Message;
                }
                ImageRebateCalculate(approveInfo.main_id);
                return "success";
            }
        }
        public void ApproveTmp(string id)
        {
            using (var db = SugarDao.GetInstance())
            {
                daoben_distributor_image origInfo = db.Queryable<daoben_distributor_image>().SingleOrDefault(a => a.id == id);
                if (origInfo == null)
                    return;
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
                db.Update<daoben_distributor_image>(upObj, a => a.id == id);
            }
            ImageRebateCalculate(id);
        }

        public void ImageRebateCalculate(string id)
        {
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    DateTime now = DateTime.Now;
                    daoben_distributor_image image = db.Queryable<daoben_distributor_image>().SingleOrDefault(a => a.id == id);
                    if (image == null)
                        return;
                    List<daoben_distributor_image_res> resList = new List<daoben_distributor_image_res>();
                    if (image.target_mode == 1) // 未开始的也先生成，查询/汇总时过滤掉
                    {
                        #region 按时间返利计算
                        if (image.pay_mode == 2)     // 按月发放
                        {
                            DateTime endDate1 = image.end_date.ToDate().AddDays(1);
                            DateTime selEnd = image.start_date.ToDate();
                            selEnd = selEnd.AddDays(1 - selEnd.Day).AddMonths(1); //按月查询的结束日
                            DateTime selStart = image.start_date.ToDate();  // 按月查询的起始日
                            while (true)
                            {
                                daoben_distributor_image_res resInfo = new daoben_distributor_image_res();
                                resInfo.main_id = image.id;
                                resInfo.month = selStart.AddDays(1 - selStart.Day);
                                resInfo.start_date = selStart;
                                resInfo.end_date = selEnd;
                                resInfo.rebate = image.rebate;
                                resList.Add(resInfo);
                                if (selEnd >= endDate1)
                                    break;
                                selStart = selEnd;
                                selEnd = selEnd.AddMonths(1) > endDate1 ? endDate1 : selEnd.AddMonths(1);
                            }
                        }
                        else    // 一次性发放
                        {
                            daoben_distributor_image_res resInfo = new daoben_distributor_image_res();
                            resInfo.main_id = image.id;
                            resInfo.rebate = image.rebate;
                            resInfo.start_date = image.start_date;
                            resInfo.end_date = image.end_date;
                            resList.Add(resInfo);
                        }
                        #endregion
                    }
                    else if(image.activity_status > 0)  // 只统计已开始/已结束的
                    {
                        #region 按销量返利计算
                        string whereStrF = null, whereStr = null; ;
                        if (image.target_content == 1)
                            whereStrF = "(sale_distributor_id='" + image.distributor_id + "' and sale_time>='{0}' and sale_time<'{1}')";
                        else
                            whereStrF = "(out_distributor_id='" + image.distributor_id + "' and outstorage_time>='{0}' and outstorage_time<'{1}')";
                        if (image.pay_mode == 2)     // 按月发放
                        {
                            DateTime endDate1 = image.end_date.ToDate().AddDays(1);
                            DateTime selEnd = image.start_date.ToDate();
                            selEnd = selEnd.AddDays(1 - selEnd.Day).AddMonths(1); // 按月查询的结束日
                            DateTime selStart = image.start_date.ToDate();  // 按月查询的起始日
                            while (true)
                            {
                                if (selStart >= now)
                                    break;
                                whereStr = string.Format(whereStrF, selStart, selEnd);
                                daoben_distributor_image_res resInfo = db.Queryable<daoben_product_sn>()
                                            .Where(a => a.sale_type > -1).Where(whereStr)
                                            .Select<daoben_distributor_image_res>("count(*) as total_sale_count, sum(price_wholesale) as total_sale_amount")
                                            .SingleOrDefault();
                                if (resInfo == null)
                                    continue;
                                resInfo.main_id = image.id;
                                resInfo.month = selStart.AddDays(1 - selStart.Day);
                                resInfo.start_date = selStart;
                                resInfo.end_date = selEnd;
                                resInfo.rebate = resInfo.total_sale_count >= image.activity_target ? image.rebate : 0;
                                resList.Add(resInfo);
                                if (selEnd >= endDate1)
                                    break;
                                selStart = selEnd;
                                selEnd = selEnd.AddMonths(1) > endDate1 ? endDate1 : selEnd.AddMonths(1);
                            }
                        }
                        else    // 一次性发放
                        {
                            if (image.end_date > now)
                                whereStr = string.Format(whereStrF, image.start_date, now);
                            else
                                whereStr = string.Format(whereStrF, image.start_date, image.end_date.ToDate().AddDays(1));
                            daoben_distributor_image_res resInfo = db.Queryable<daoben_product_sn>()
                                    .Where(a => a.sale_type > -1).Where(whereStr)
                                    .Select<daoben_distributor_image_res>("count(*) as total_sale_count, sum(price_wholesale) as total_sale_amount")
                                    .SingleOrDefault();
                            if (resInfo == null)
                                return;
                            resInfo.main_id = image.id;
                            resInfo.start_date = image.start_date;
                            resInfo.end_date = image.end_date > now ? now : image.end_date;
                            resInfo.rebate = resInfo.total_sale_count >= image.activity_target ? image.rebate : 0;
                            resList.Add(resInfo);
                        }
                        #endregion
                    }
                    if (resList.Count < 1)
                        return;
                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    db.Delete<daoben_distributor_image_res>(a => a.main_id == image.id);
                    db.DisableInsertColumns = new string[] { "id" };
                    db.InsertRange(resList);
                    db.CommitTran();
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    ExceptionApp.WriteLog("ImageApp(ImageRebateCalculate)：" + ex.Message);
                }
            }

        }


    }
}
