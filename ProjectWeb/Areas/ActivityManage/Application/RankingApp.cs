using Base.Code;
using MySqlSugar;
using NPOI.HSSF.UserModel;
using ProjectShare.Database;
using ProjectShare.Models;
using ProjectShare.Process;
using ProjectWeb.Areas.DistributorManage.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ProjectWeb.Areas.ActivityManage.Application
{
    public class RankingApp
    {
        public object GetList(Pagination pagination, daoben_activity_ranking queryInfo, QueryTime queryTime)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "start_date" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;

            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    var qable = db.Queryable<daoben_activity_ranking>();
                    if (myCompanyInfo.category == "事业部")
                        qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                    else if (myCompanyInfo.category == "分公司")
                        qable.Where(a => a.company_id == myCompanyInfo.id);

                    if (queryInfo != null)
                    {
                        if (!string.IsNullOrEmpty(queryInfo.name))
                            qable.Where(a => a.name.Contains(queryInfo.name));
                        if (queryInfo.company_id != 0)
                            qable.Where(a => a.company_id == queryInfo.company_id);
                        if (queryInfo.activity_status != 0)
                            qable.Where(a => a.activity_status == queryInfo.activity_status);
                        if (queryInfo.emp_category > 0)
                            qable.Where(a => a.emp_category == queryInfo.emp_category);
                        if (queryInfo.approve_status != 0)  // 0表示查找全部
                        {
                            int status = queryInfo.approve_status;
                            if (status == 100)   // 已审批
                                qable.Where(a => a.approve_status == 100);
                            else if (status == -100)    // 审批不通过
                                qable.Where(a => a.approve_status < 0);
                            else if (status == 1)    // 审批中
                                qable.Where(a => a.approve_status > 0 && a.approve_status < 100);
                            else if (status == -1)    // 未审批
                                qable.Where(a => a.approve_status == 0);
                        }
                    }
                    if (queryTime != null)
                    {
                        if (queryTime.startTime1 != null)
                        {
                            qable.Where(a => a.create_time >= queryTime.startTime1);
                        }
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
            catch (Exception ex)
            {
                return "系统出错：" + ex.Message;
            }
        }
        // 我的审批
        public object GetListApprove(Pagination pagination, daoben_activity_ranking queryInfo)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "start_date" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;

            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    var qable = db.Queryable<daoben_activity_ranking>();

                    if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)    // 跳过超级管理员，避免判断公司/职位信息是否存在
                    {
                        if (myPositionInfo.positionType == ConstData.POSITION_GM2)
                        {   // 分公司总经理
                            qable.Where(a => a.approve_status == 0 && a.company_id == myCompanyInfo.id);
                        }
                        else if (myPositionInfo.positionType == ConstData.POSITION_GM1
                                    || myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT1)
                        {   // 事业部总经理/事业部总经理助理
                            qable.Where(a => a.approve_status == 1 && a.company_id_parent == myCompanyInfo.id);
                        }
                        else if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER)
                        {   // 财务经理
                            qable.Where(a => a.approve_status == 2 && a.company_id_parent == myCompanyInfo.id);
                        }
                        else
                            return "无权限，非法访问";
                    }
                    if (queryInfo != null)
                    {
                        if (!string.IsNullOrEmpty(queryInfo.name))
                            qable.Where(a => a.name.Contains(queryInfo.name));
                        if (queryInfo.company_id > 0)
                            qable.Where(a => a.company_id == queryInfo.company_id);
                        if (queryInfo.emp_category > 0)
                            qable.Where(a => a.emp_category == queryInfo.emp_category);
                    }

                    string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                                .ToJsonPage(pagination.page, pagination.rows, ref records);
                    pagination.records = records;
                    if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                        return null;
                    return listStr.ToJson();
                }
            }
            catch (Exception ex)
            {
                return "系统出错：" + ex.Message;
            }
        }
        /// <summary>
        /// 从串码表获取销售详情
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryInfo"></param>
        /// <param name="queryMainInfo"></param>
        /// <returns></returns>
        public object GetSaleList(Pagination pagination, daoben_product_sn queryInfo, daoben_activity_ranking queryMainInfo)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (queryMainInfo == null)
                return "信息错误，获取销售详情失败";
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
                string fieldStr = ("b.emp_name as name, a.phone_sn, a.model, a.color, a.price_wholesale, a.price_retail,");
                if (queryMainInfo.ranking_content == 1 || queryMainInfo.ranking_content == 3) //实销
                {
                    if (queryMainInfo.emp_category == 3) //导购员
                    {
                        qable.JoinTable<daoben_activity_ranking_emp>((a, b) => a.reporter_id == b.emp_id)
                             .Where(a => a.reporter_type == 1);
                    }
                    else if (queryMainInfo.emp_category == 20) //业务员
                        qable.JoinTable<daoben_activity_ranking_emp>((a, b) => a.sales_id == b.emp_id);
                    else if (queryMainInfo.emp_category == 21) //业务经理
                        qable.JoinTable<daoben_activity_ranking_emp>((a, b) => a.sales_m_id == b.emp_id);

                    qable.Where<daoben_activity_ranking_emp>((a, b) => b.main_id == queryMainInfo.id && a.sale_type > 0
                                                                && a.sale_time >= startTime && a.sale_time < endTime);
                    fieldStr += ("a.sale_time as time");
                }
                else if (queryMainInfo.ranking_content == 2 || queryMainInfo.ranking_content == 4) //下货
                {
                    if (queryMainInfo.emp_category == 3) //导购员
                    {
                        qable.JoinTable<daoben_activity_ranking_emp>((a, b) => a.reporter_id == b.emp_id)
                             .Where(a => a.reporter_type == 1);
                    }
                    else if (queryMainInfo.emp_category == 20) //业务员
                        qable.JoinTable<daoben_activity_ranking_emp>((a, b) => a.out_sales_id == b.emp_id);
                    else if (queryMainInfo.emp_category == 21) //业务经理
                        qable.JoinTable<daoben_activity_ranking_emp>((a, b) => a.out_sales_m_id == b.emp_id);

                    qable.Where<daoben_activity_ranking_emp>((a, b) => b.main_id == queryMainInfo.id && a.sale_type == 0
                                                                && a.outstorage_time >= startTime && a.outstorage_time < endTime);
                    fieldStr += ("a.outstorage_time as time");
                }

                if (queryInfo != null) //匹配查询参数
                {
                    if (!string.IsNullOrEmpty(queryInfo.phone_sn))
                        qable.Where(a => a.phone_sn.Contains(queryInfo.phone_sn));
                    if (!string.IsNullOrEmpty(queryInfo.model))
                        qable.Where(a => a.model.Contains(queryInfo.model));
                }
                if (!string.IsNullOrEmpty(queryMainInfo.name))
                    qable.Where<daoben_activity_ranking_emp>((a, b) => b.emp_name.Contains(queryMainInfo.name));

                var listStr = qable.Select(fieldStr)
                                   .OrderBy(pagination.sidx + " " + pagination.sord)
                                   .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }

        }

        public string Add(daoben_activity_ranking addInfo, List<daoben_activity_ranking_reward> rewardList,
                    List<daoben_activity_ranking_emp> empList)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            if (addInfo == null)
                return "信息错误，操作失败!";
            if (addInfo.company_id < 1)
                return "信息错误：需指定分公司!";
            if (string.IsNullOrEmpty(addInfo.name))
                return "信息错误：活动名称不能为空!";
            if (rewardList == null || rewardList.Count < 0)
                return "信息错误：奖励信息不能为空!";
            if (empList == null || empList.Count < 1)
                return "信息错误：参与人数不能小于1!";
            if (addInfo.emp_category < 1 && addInfo.emp_category > 3)
                return "信息错误：参与对象类型不正确!";
            if (addInfo.start_date == null || addInfo.end_date == null)
                return "信息错误：比赛起止时间不能为空!";
            //DateTime curMonth = DateTime.Now.Date.AddDays(0 - DateTime.Now.Day);
            //if (addInfo.start_date < curMonth)
            //    return "信息错误：比赛时间不能早于本月!";
            if (string.IsNullOrEmpty(addInfo.id) || addInfo.id.Length != 36)
                return "信息错误：ID不正确!";

            addInfo.creator_job_history_id = LoginInfo.jobHistoryId;
            addInfo.creator_name = LoginInfo.empName;
            addInfo.creator_position_name = myPositionInfo.name;
            addInfo.create_time = DateTime.Now;
            addInfo.activity_status = -2;
            addInfo.approve_status = 0;
            addInfo.actual_emp_count = empList.Count;
            SqlSugarClient db = null;
            try
            {
                using (db = SugarDao.GetInstance())
                {
                    daoben_org_company companyInfo = db.Queryable<daoben_org_company>().InSingle(addInfo.company_id);
                    if (companyInfo == null)
                        return "信息错误：指定分公司不存在";
                    addInfo.company_name = companyInfo.link_name;
                    addInfo.company_id_parent = companyInfo.parent_id;

                    #region 待办事项 分公司总经理
                    //待办事项 收件人
                    string tempStr = "排名比赛待审批";
                    List<string> idList = db.Queryable<daoben_hr_emp_job>()
                        .Where(a => a.company_id == addInfo.company_id && a.position_type == ConstData.POSITION_GM2).Select<string>("id").ToList();
                    //待办事项 生成列表
                    List<daoben_sys_task> taskList = idList
                        .Select(a1 => new daoben_sys_task
                        {
                            emp_id = a1,
                            category = 1,
                            main_id = addInfo.id,
                            main_url = "/ActivityManage/Ranking/Approve?id=" + addInfo.id,
                            title = tempStr,
                            content_abstract = addInfo.note,
                            recipient_type = 1,
                            create_time = now,
                            status = 1
                        }).ToList();
                    #endregion

                    db.CommandTimeOut = 150;
                    db.BeginTran();
                    db.Insert(addInfo);
                    db.DisableInsertColumns = new string[] { "id" };
                    db.InsertRange(rewardList);
                    db.InsertRange(empList);
                    //待办事项 列表插入
                    if (taskList != null && taskList.Count() >= 25)
                        db.SqlBulkCopy(taskList);
                    else if (taskList != null && taskList.Count() > 0)
                        db.InsertRange(taskList);

                    db.CommitTran();

                }
            }
            catch (Exception ex)
            {
                if (db != null)
                    db.RollbackTran();
                return "系统出错：" + ex.Message;
            }
            StatisticsRanking(addInfo.id);
            //临时直接置为已审批 12.21
            ApproveTmp(addInfo.id);
            return "success";
        }


        public object GetInfo(string id)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (string.IsNullOrEmpty(id))
                return null;
            List<daoben_activity_ranking_approve> approveList = new List<daoben_activity_ranking_approve>();
            using (var db = SugarDao.GetInstance())
            {
                daoben_activity_ranking ranking = db.Queryable<daoben_activity_ranking>().InSingle(id);
                if (ranking == null)
                    return null;
                List<daoben_activity_ranking_reward> rewardList = db.Queryable<daoben_activity_ranking_reward>()
                            .Where(a => a.main_id == id).ToList();
                List<daoben_activity_ranking_emp> empList = db.Queryable<daoben_activity_ranking_emp>()
                            .Where(a => a.main_id == id).ToList();
                empList = empList.OrderBy(t => t.counting_palce).ToList();
                if (ranking.approve_status != 0)
                    approveList = db.Queryable<daoben_activity_ranking_approve>().Where(a => a.main_id == id).ToList();
                #region 统计
                List<daoben_product_sn> snList = new List<daoben_product_sn>();
                DateTime startDate = ranking.start_date.ToDate();
                DateTime endDate = ranking.end_date.ToDate().AddDays(1);
                List<daoben_distributor_info> distriList = db.Queryable<daoben_distributor_info>()
                    .Where(a => a.company_id == ranking.company_id || a.company_id_parent == ranking.company_id).ToList();
                //是否指定机型，实销or下货 业务员，业务经理，导购员
                //是否指定机型，实销or下货 业务员，业务经理，导购员
                if (ranking.emp_category == 21)//业务经理
                {
                    if (ranking.ranking_content == 2 || ranking.ranking_content == 4)
                    {
                        snList = db.Queryable<daoben_product_sn>()
                                    .JoinTable<daoben_activity_ranking_emp>((a, c) => c.emp_id == a.out_sales_m_id)
                                    .Where<daoben_activity_ranking_emp>((a, c) => c.main_id == ranking.id && a.status > 0
                                        && a.outstorage_time >= startDate && a.outstorage_time < endDate)
                                    .Select("a.*").ToList();

                    }
                    else if (ranking.ranking_content == 1 || ranking.ranking_content == 3)
                    {
                        snList = db.Queryable<daoben_product_sn>()
                                    .JoinTable<daoben_activity_ranking_emp>((a, c) => c.emp_id == a.sales_m_id)
                                    .Where<daoben_activity_ranking_emp>((a, c) => c.main_id == ranking.id && a.status > 0
                                        && a.sale_time >= startDate && a.sale_time < endDate)
                                    .Select("a.*").ToList();
                    }
                }
                else if (ranking.emp_category == 20)//业务员
                {
                    if (ranking.ranking_content == 2 || ranking.ranking_content == 4)
                    {
                        snList = db.Queryable<daoben_product_sn>()
                                    .JoinTable<daoben_activity_ranking_emp>((a, c) => a.out_sales_id == c.emp_id)
                                    .Where<daoben_activity_ranking_emp>((a, c) => c.main_id == ranking.id && a.status > 0
                                        && a.outstorage_time >= startDate && a.outstorage_time < endDate)
                                    .Select("a.*").ToList();

                    }
                    else if (ranking.ranking_content == 1 || ranking.ranking_content == 3)
                    {
                        snList = db.Queryable<daoben_product_sn>()
                                    .JoinTable<daoben_activity_ranking_emp>((a, c) => a.sales_id == c.emp_id)
                                    .Where<daoben_activity_ranking_emp>((a, c) => c.main_id == ranking.id && a.status > 0
                                        && a.sale_time >= startDate && a.sale_time < endDate)
                                    .Select("a.*").ToList();
                    }
                }
                else if (ranking.emp_category == 3)//导购员  
                {
                    if (ranking.ranking_content == 2 || ranking.ranking_content == 4)
                    {
                        //snList = db.Queryable<daoben_product_sn>()
                        //            .JoinTable<daoben_activity_ranking_emp>((a, b) => a.out_guide_id == b.emp_id)
                        //            .Where<daoben_activity_ranking_emp>((a, b) => b.main_id == ranking.id && a.status > 0
                        //                && a.sale_time >= startDate && a.sale_time < endDate)
                        //            .Select("a.*").ToList();
                    }
                    else if (ranking.ranking_content == 1 || ranking.ranking_content == 3)
                    {
                        List< daoben_activity_ranking_emp> tmpList = db.Queryable<daoben_product_sn>()
                                        .JoinTable<daoben_activity_ranking_emp>((a, c) => c.emp_id == a.reporter_id)
                                        .Where<daoben_activity_ranking_emp>((a, c) => c.main_id == ranking.id && a.sale_type > -1
                                        && a.sale_time >= startDate && a.sale_time < endDate)
                                        .GroupBy("c.emp_id")
                                        .Select<daoben_activity_ranking_emp>("count(*) as counting_count").ToList();
                        snList = db.Queryable<daoben_product_sn>()
                                    .JoinTable<daoben_activity_ranking_emp>((a, b) => a.reporter_id == b.emp_id)
                                    .Where<daoben_activity_ranking_emp>((a, b) => b.main_id == ranking.id && a.status > 0
                                        && a.sale_time >= startDate && a.sale_time < endDate)
                                    .Select("a.*").ToList();
                    }
                }
                object statisticsTime;
                if (snList.Count > 1)
                {
                    if (ranking.ranking_content == 1 || ranking.ranking_content == 3)
                    {
                        statisticsTime = new
                        {
                            end_time = snList.OrderByDescending(t => t.sale_time).First().sale_time,
                            start_time = snList.OrderBy(t => t.sale_time).First().sale_time,

                        };
                    }
                    else
                    {
                        statisticsTime = new
                        {
                            end_time = snList.OrderByDescending(t => t.outstorage_time).First().outstorage_time,
                            start_time = snList.OrderBy(t => t.outstorage_time).First().outstorage_time,
                        };
                    }
                }
                else
                {
                    statisticsTime = new
                    {
                        start_time = ranking.start_date,
                        end_time = ranking.end_date == null ? DateTime.Now : ranking.end_date,
                    };
                }
                #endregion
                object retObj = new
                {
                    mainInfo = ranking,
                    rewardList = rewardList,
                    empList = empList,
                    approveList = approveList,
                    statisticsTime = statisticsTime
                };
                return retObj;
            }
        }
        public string Alter(string id, DateTime alterDate)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (string.IsNullOrEmpty(id))
                return "信息错误，操作失败!";
            daoben_activity_ranking_alter alterInfo = new daoben_activity_ranking_alter
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
                    daoben_activity_ranking mainInfo = db.Queryable<daoben_activity_ranking>().SingleOrDefault(a => a.id == id);
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
                    //修改完时间后判断是否要修改活动状态
                    DateTime now = DateTime.Now;
                    int activity_status = mainInfo.activity_status;
                    if (now.Date > alterDate)
                        activity_status = 2;
                    object upObj = new
                    {
                        end_date = alterDate,
                        activity_status = activity_status
                    };
                    db.Update<daoben_activity_ranking>(upObj, a => a.id == id);
                    db.Update<daoben_activity_ranking_alter>(new { approve_status = 100 }, a => a.id == alterInfo.id);
                    if (activity_status < 0)
                        return "success";
                    StatisticsRanking(id);
                    #endregion
                    return "success";
                }
            }
            catch (Exception ex)
            {
                return "系统出错：" + ex.Message;
            }
        }
        public string Recall(string id)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (string.IsNullOrEmpty(id))
                return null;
            SqlSugarClient db = null;
            try
            {
                using (db = SugarDao.GetInstance())
                {

                    daoben_activity_ranking recallInfo = db.Queryable<daoben_activity_ranking>().InSingle(id);
                    if (recallInfo == null)
                        return "信息错误：活动信息不存在!";
                    if (recallInfo.creator_job_history_id != LoginInfo.jobHistoryId)
                        return "操作失败：活动信息只能由发起人撤回!";
                    if (recallInfo.approve_status != 0)
                        return "操作失败：活动信息已审批!";
                    db.CommandTimeOut = 150;
                    db.BeginTran();
                    db.Delete<daoben_activity_ranking_reward>(a => a.main_id == id);
                    db.Delete<daoben_activity_ranking_emp>(a => a.main_id == id);
                    db.Delete<daoben_activity_ranking>(a => a.id == id);
                    //删除所有待办事项
                    db.Delete<daoben_sys_task>(a => a.main_id == id);
                    db.Delete<daoben_sys_notification>(a => a.main_id == id);
                    return "success";
                }
            }
            catch (Exception ex)
            {
                if (db != null)
                    db.RollbackTran();
                return "系统出错：" + ex.Message;
            }
        }
        /// <summary>
        /// //0分公司总经理1——1事业部总经理/助手2——2财务经理100
        /// </summary>
        /// <param name="approveInfo"></param>
        /// <returns></returns>
        public string Approve(daoben_activity_ranking_approve approveInfo)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            if (approveInfo == null || string.IsNullOrEmpty(approveInfo.main_id))
                return "信息错误，操作失败!";
            approveInfo.approve_id = LoginInfo.accountId;
            approveInfo.approve_name = LoginInfo.empName;
            approveInfo.approve_position_id = myPositionInfo.id;
            approveInfo.approve_position_name = myPositionInfo.name;
            approveInfo.approve_time = DateTime.Now;
            object upObj = null;
            SqlSugarClient db = null;
            try
            {
                using (db = SugarDao.GetInstance())
                {
                    daoben_activity_ranking mainInfo = db.Queryable<daoben_activity_ranking>().InSingle(approveInfo.main_id);
                    if (mainInfo == null)
                        return "操作失败：活动信息不存在";
                    if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER && approveInfo.status > 0)
                    {
                        approveInfo.status = 100; //以100作为审批完成的标志
                        if (mainInfo.start_date <= DateTime.Now)
                            upObj = new { approve_status = approveInfo.status, activity_status = 1 };
                        else
                            upObj = new { approve_status = approveInfo.status, activity_status = -1 };
                    }
                    else
                    {
                        if (approveInfo.status > 0)
                            approveInfo.status = mainInfo.approve_status + 1;
                        else
                            approveInfo.status = 0 - 1 - mainInfo.approve_status;
                        upObj = new { approve_status = approveInfo.status };
                    }
                    //查询之前待办事项
                    List<daoben_sys_task> origTask = db.Queryable<daoben_sys_task>().Where(a => a.main_id == approveInfo.main_id).ToList();
                    object upObjForTask = new
                    {
                        status = 3,
                        finished_time = now
                    };
                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    db.DisableInsertColumns = new string[] { "id" };
                    db.Insert(approveInfo);
                    db.Update<daoben_activity_ranking>(upObj, a => a.id == approveInfo.main_id);
                    //清除之前的待办事项
                    if (origTask.Count() > 0)
                        db.Update<daoben_sys_task>(upObjForTask, a => a.main_id == approveInfo.main_id);


                    db.CommitTran();

                    #region 待办事项
                    List<daoben_sys_notification> newsTotal = new List<daoben_sys_notification>();//消息通知
                    List<daoben_sys_task> taskTotal = new List<daoben_sys_task>();//待办事项
                    string newsStr = null;
                    List<string> newsIdList = null;
                    string taskStr = null;
                    List<string> taskIdList = null;
                    //0分公司总经理1——1事业部总经理/助手2——2财务经理100
                    daoben_activity_ranking origInfo = db.Queryable<daoben_activity_ranking>().InSingle(approveInfo.main_id);
                    if (origInfo.approve_status == 1)
                        taskIdList = db.Queryable<daoben_hr_emp_job>()
                            .Where(a => a.position_type == ConstData.POSITION_GM1 && a.company_id == origInfo.company_id_parent)
                            .Select<string>("id").ToList();
                    else if (origInfo.approve_status == 2)
                        taskIdList = db.Queryable<daoben_hr_emp_job>()
                                    .JoinTable<daoben_ms_account>((a, b) => a.id == b.employee_id)
                                    .Where<daoben_ms_account>((a, b) => b.role_id == ConstData.ROLE_ID_FINANCIALMANAGER)
                                    .Where(a => a.company_id == origInfo.company_id_parent)
                                    .Select<string>("a.id as id").ToList();
                    else if (origInfo.approve_status != 0)
                        newsIdList = db.Queryable<daoben_hr_emp_job_history>()
                        .Where(a => a.id == origInfo.creator_job_history_id).Select<string>("emp_id as id").ToList();

                    taskStr = "排名比赛待审批";
                    //待办事项 生成列表
                    if (taskIdList != null && taskIdList.Count > 0)
                    {
                        taskTotal = taskIdList
                            .Select(a1 => new daoben_sys_task
                            {
                                emp_id = a1,
                                category = 1,
                                main_id = origInfo.id,
                                main_url = "/ActivityManage/Ranking/Approve?id=" + origInfo.id,
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
                            newsStr = "排名比赛没有审批通过";
                        else if (origInfo.approve_status == 100)
                            newsStr = "排名比赛已审批通过";
                        //消息通知 生成列表
                        newsTotal = newsIdList
                                .Select(a1 => new daoben_sys_notification
                                {
                                    emp_id = a1,
                                    category = 2,
                                    main_id = origInfo.id,
                                    main_url = "/ActivityManage/Ranking/Show?id=" + origInfo.id,
                                    title = newsStr,
                                    content_abstract = origInfo.note,
                                    recipient_type = 1,
                                    create_time = now,
                                    status = 1
                                }).ToList();
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
            }
            catch (Exception ex)
            {
                if (db != null)
                    db.RollbackTran();
                return "系统出错：" + ex.Message;
            }
        }

        public void ApproveTmp(string id)
        {
            using (var db = SugarDao.GetInstance())
            {
                daoben_activity_ranking origInfo = db.Queryable<daoben_activity_ranking>().InSingle(id);
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
                db.Update<daoben_activity_ranking>(upObj, a => a.id == id);
            }
        }

        public void StatisticsRanking(string id)
        {
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    DateTime now = DateTime.Now;

                    List<daoben_activity_ranking> ranking = db.Queryable<daoben_activity_ranking>().Where(a => a.id == id).ToList(); ;

                    //分别对上述活动进行统计
                    //排名
                    SettlementApp app = new SettlementApp();
                    string sqlUpSb = app.EmpRankReward(db, ranking);

                    if (!string.IsNullOrEmpty(sqlUpSb))
                        db.SqlQuery<int>(sqlUpSb);

                }
                catch (Exception ex)
                {

                    //TODO 
                }
            }

        }

        public MemoryStream ExportExcelRanking(Pagination pagination, QueryTime queryTime, int? company_id, string emp_name)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            if (pagination == null)
                pagination = new Pagination();
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "c.name" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            DateTime firstDayOfThisYear = new DateTime(now.Year, 1, 1);
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_activity_ranking_emp>()
                    .JoinTable<daoben_hr_emp_job>((a, b) => a.emp_id == b.id)
                    .JoinTable<daoben_activity_ranking>((a, c) => a.main_id == c.id);
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER)
                        qable.Where<daoben_hr_emp_job>((a, b) => b.company_id == myCompanyInfo.id || b.company_id_parent == myCompanyInfo.id);
                    else if (myPositionInfo.positionType <= ConstData.POSITION_OFFICE_NORMAL)
                        qable.Where<daoben_hr_emp_job>((a, b) => b.company_id == myCompanyInfo.id || b.company_id_parent == myCompanyInfo.id);
                    else return null;
                }
                if (company_id > 0)
                    qable.Where<daoben_hr_emp_job>((a, b) => b.company_id == company_id || b.company_id_parent == company_id);
                if (!string.IsNullOrEmpty(emp_name))
                    qable.Where(a => a.emp_name.Contains(emp_name));
                if (queryTime.startTime1 == null)
                    qable.Where<daoben_activity_ranking>((a, c) => c.end_date >= firstDayOfThisYear);
                else
                    qable.Where<daoben_activity_ranking>((a, c) => c.end_date >= queryTime.startTime1);
                if (queryTime.startTime2 != null)
                {
                    queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                    qable.Where<daoben_activity_ranking>((a, c) => c.end_date < queryTime.startTime2);
                }
                    


                string selStr = "b.company_linkname,b.name as name,(CASE WHEN c.emp_category=3 THEN '导购员' WHEN c.emp_category=20 THEN '业务员' WHEN c.emp_category=21 THEN '业务经理' ELSE '-' END),c.name as activity_name,(CASE WHEN c.ranking_content=1 THEN '按实销总量' WHEN c.ranking_content=2 THEN '按下货总量' WHEN c.ranking_content=3 THEN '按实销总金额（零售价）' WHEN c.ranking_content=4 THEN '按下货总金额（零售价）ELSE '-' END),DATE_FORMAT(c.start_date,'%Y-%m-%d'),DATE_FORMAT(c.end_date,'%Y-%m-%d'),a.reward";
                var listDt = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .Select(selStr).ToDataTable();
                int rowsCount = listDt.Rows.Count;

                string[] headerArr = new string[] { "所属机构", "员工名称", "员工类型", "活动名称", "考核内容", "活动开始时间", "活动结束时间", "奖励金额" };
                int[] colWidthArr = new int[] { 18, 20, 15, 18, 18, 20, 20, 18 };

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
    }
}
