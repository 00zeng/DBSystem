using Base.Code;
using MySqlSugar;
using ProjectShare.Database;
using ProjectShare.Process;
using System;
using System.Linq;
using ProjectShare.Models;
using System.Collections.Generic;
using System.Text;
using ProjectWeb.Areas.DistributorManage.Application;
using NPOI.HSSF.UserModel;
using System.IO;

namespace ProjectWeb.Areas.ActivityManage.Application
{
    /// <summary>
    /// 主推奖励活动
    /// </summary>
    public class RecommendationApp
    {
        public object GetList(Pagination pagination, daoben_activity_recommendation queryInfo, QueryTime queryTime)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;

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
                    var qable = db.Queryable<daoben_activity_recommendation>();
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
                            qable.Where(a => a.start_date >= queryTime.startTime1);
                        }
                        if (queryTime.startTime2 != null)
                        {
                            queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                            qable.Where(a => a.start_date < queryTime.startTime2);
                        }
                        if (queryTime.endTime1 != null)
                        {
                            qable.Where(a => a.end_date >= queryTime.endTime1);
                        }
                        if (queryTime.endTime2 != null)
                        {
                            queryTime.endTime2 = queryTime.endTime2.ToDate().AddDays(1);
                            qable.Where(a => a.end_date < queryTime.endTime2);
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

        public object GetListApprove(Pagination pagination, daoben_activity_recommendation queryInfo)
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
                    var qable = db.Queryable<daoben_activity_recommendation>();

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

        public string Add(daoben_activity_recommendation addInfo, List<daoben_activity_recommendation_product> productList,
                            List<daoben_activity_recommendation_emp> empList, List<daoben_activity_recommendation_reward> rewardList)
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
            if (empList == null || empList.Count < 1)
                return "信息错误：参与人数不能小于1!";
            if (addInfo.emp_category != 3 && addInfo.emp_category != 21 && addInfo.emp_category != 20)
                return "信息错误：参与对象类型不正确!";
            if (addInfo.start_date == null || addInfo.end_date == null)
                return "信息错误：起止时间不能为空!";
            //DateTime curMonth = DateTime.Now.Date.AddDays(0 - DateTime.Now.Day);
            //if (addInfo.start_date < curMonth)
            //    return "信息错误：时间不能早于本月!";
            if (string.IsNullOrEmpty(addInfo.id) || addInfo.id.Length != 36)
                return "信息错误：ID不正确!";
            if ((addInfo.product_scope == 2 || addInfo.product_scope == 3) && (productList == null || productList.Count < 1))
                return "信息错误：指定机型时机型列表不能为空!";
            //if (addInfo.target_mode != 4 && (rewardList == null || rewardList.Count < 1))
            //    return "信息错误，返利规则不能为空!";

            addInfo.creator_job_history_id = LoginInfo.empType == 0 ? LoginInfo.jobHistoryId : LoginInfo.empId;
            addInfo.creator_name = LoginInfo.empName;
            addInfo.create_time = DateTime.Now;
            addInfo.creator_position_name = myPositionInfo.name;
            addInfo.activity_status = -2;
            addInfo.approve_status = 0;
            addInfo.emp_count = empList.Count;
            addInfo.counting_time = null;
            SqlSugarClient db = null;
            using (db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_org_company companyInfo = db.Queryable<daoben_org_company>().InSingle(addInfo.company_id);
                    if (companyInfo == null)
                        return "信息错误：指定分公司不存在";
                    addInfo.company_name = companyInfo.link_name;
                    addInfo.company_id_parent = companyInfo.parent_id;

                    #region 待办事项 分公司总经理
                    //待办事项 收件人
                    string tempStr = "主推奖励活动待审批";
                    List<string> idList = db.Queryable<daoben_hr_emp_job>()
                        .Where(a => a.company_id == addInfo.company_id && a.position_type == ConstData.POSITION_GM2).Select<string>("id").ToList();
                    //待办事项 生成列表
                    List<daoben_sys_task> taskList = idList
                        .Select(a1 => new daoben_sys_task
                        {
                            emp_id = a1,
                            category = 1,
                            main_id = addInfo.id,
                            main_url = "/ActivityManage/Attaining/Approve?id=" + addInfo.id,
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
                    if (productList != null && productList.Count > 0)
                        db.InsertRange(productList);
                    if (rewardList != null)
                        db.InsertRange(rewardList);
                    db.InsertRange(empList);
                    //待办事项 列表插入
                    if (taskList != null && taskList.Count() >= 25)
                        db.SqlBulkCopy(taskList);
                    else if (taskList != null && taskList.Count() > 0)
                        db.InsertRange(taskList);

                    db.CommitTran();
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return "系统出错：" + ex.Message;
                }
            }
            StatisticsRecom(addInfo.id);
            //临时直接置为已审批 12.21
            ApproveTmp(addInfo.id);
            return "success";
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
                    daoben_activity_recommendation recom = db.Queryable<daoben_activity_recommendation>().SingleOrDefault(a => a.id == id);
                    if (recom == null)
                        return "信息错误：指定的信息不存在";
                    List<daoben_activity_recommendation_approve> approveList = db.Queryable<daoben_activity_recommendation_approve>().Where(a => a.main_id == id).ToList();
                    List<daoben_activity_recommendation_emp> empList = db.Queryable<daoben_activity_recommendation_emp>().Where(a => a.main_id == id).ToList();
                    List<daoben_activity_recommendation_product> productList = db.Queryable<daoben_activity_recommendation_product>().Where(a => a.main_id == id).ToList();
                    List<daoben_activity_recommendation_reward> rewardList = db.Queryable<daoben_activity_recommendation_reward>().Where(a => a.main_id == id).ToList();
                    string creator_position_name = db.Queryable<daoben_hr_emp_job_history>()
                        .Where(a => a.id == recom.creator_job_history_id).Select<string>("position_name").SingleOrDefault();
                    #region 信息统计
                    //是否指定机型，实销or下货 业务员，业务经理，导购员
                    DateTime startDate = recom.start_date.ToDate();
                    DateTime endDate = recom.end_date.ToDate().AddDays(1);
                    List<daoben_product_sn> snList = new List<daoben_product_sn>();
                    if (recom.emp_category == 21)//业务经理
                    {
                        if (recom.product_scope > 1 && recom.target_content == 2)
                        {
                            snList = db.Queryable<daoben_product_sn>()
                                        .JoinTable<daoben_activity_recommendation_product>((a, b) => a.model == b.model && a.color == b.color)
                                        .JoinTable<daoben_activity_recommendation_emp>((a, d) => a.out_sales_m_id == d.emp_id)
                                        .Where<daoben_activity_recommendation_product, daoben_activity_recommendation_emp>((a, b, d) => b.main_id == recom.id && a.status > 0 &&
                                            d.main_id == recom.id && a.outstorage_time >= startDate && a.outstorage_time < endDate)
                                        .Select("a.*").ToList();
                        }
                        else if (recom.product_scope > 1 && recom.target_content == 1)
                        {
                            snList = db.Queryable<daoben_product_sn>()
                                        .JoinTable<daoben_activity_recommendation_product>((a, b) => a.model == b.model && a.color == b.color)
                                        .JoinTable<daoben_activity_recommendation_emp>((a, d) => a.sales_m_id == d.emp_id)
                                        .Where<daoben_activity_recommendation_product, daoben_activity_recommendation_emp>((a, b, d) => b.main_id == recom.id && a.status > 1 &&
                                            d.main_id == recom.id && a.sale_time >= startDate && a.sale_time < endDate)
                                        .Select("a.*").ToList();
                        }
                        else if (recom.product_scope == 1 && recom.target_content == 1)
                        {
                            snList = db.Queryable<daoben_product_sn>()
                                        .JoinTable<daoben_activity_recommendation_emp>((a, c) => c.emp_id == a.sales_m_id)
                                        .Where<daoben_activity_recommendation_emp>((a, c) => c.main_id == recom.id && a.status > 1
                                            && a.sale_time >= startDate && a.sale_time < endDate)
                                        .Select("a.*").ToList();
                        }
                        else if (recom.product_scope == 1 && recom.target_content == 2)
                        {
                            snList = db.Queryable<daoben_product_sn>()
                                        .JoinTable<daoben_activity_recommendation_emp>((a, c) => c.emp_id == a.out_sales_m_id)
                                        .Where<daoben_activity_recommendation_emp>((a, c) => c.main_id == recom.id && a.status > 0
                                            && a.outstorage_time >= startDate && a.outstorage_time < endDate)
                                        .Select("a.*").ToList();
                        }
                    }
                    else if (recom.emp_category == 20)//业务员
                    {
                        if (recom.product_scope > 1 && recom.target_content == 2)
                        {
                            snList = db.Queryable<daoben_product_sn>()
                                        .JoinTable<daoben_activity_recommendation_product>((a, b) => a.model == b.model && a.color == b.color)
                                        .JoinTable<daoben_activity_recommendation_emp>((a, d) => a.out_sales_id == d.emp_id)
                                        .Where<daoben_activity_recommendation_product, daoben_activity_recommendation_emp>((a, b, d) => b.main_id == recom.id && a.status > 0 &&
                                            d.main_id == recom.id && a.outstorage_time >= startDate && a.outstorage_time < endDate)
                                        .Select("a.*").ToList();
                        }
                        else if (recom.product_scope > 1 && recom.target_content == 1)
                        {
                            snList = db.Queryable<daoben_product_sn>()
                                        .JoinTable<daoben_activity_recommendation_product>((a, b) => a.model == b.model && a.color == b.color)
                                        .JoinTable<daoben_activity_recommendation_emp>((a, d) => a.sales_id == d.emp_id)
                                        .Where<daoben_activity_recommendation_product, daoben_activity_recommendation_emp>((a, b, d) => b.main_id == recom.id && a.status > 1 &&
                                            d.main_id == recom.id && a.sale_time >= startDate && a.sale_time < endDate)
                                        .Select("a.*").ToList();
                        }
                        else if (recom.product_scope == 1 && recom.target_content == 1)
                        {
                            snList = db.Queryable<daoben_product_sn>()
                                        .JoinTable<daoben_activity_recommendation_emp>((a, c) => c.emp_id == a.sales_id)
                                        .Where<daoben_activity_recommendation_emp>((a, c) => c.main_id == recom.id && a.status > 1
                                            && a.sale_time >= startDate && a.sale_time < endDate)
                                        .Select("a.*").ToList();
                        }
                        else if (recom.product_scope == 1 && recom.target_content == 2)
                        {
                            snList = db.Queryable<daoben_product_sn>()
                                        .JoinTable<daoben_activity_recommendation_emp>((a, c) => c.emp_id == a.out_sales_id)
                                        .Where<daoben_activity_recommendation_emp>((a, c) => c.main_id == recom.id && a.status > 0
                                            && a.outstorage_time >= startDate && a.outstorage_time < endDate)
                                        .Select("a.*").ToList();
                        }
                    }
                    else if (recom.emp_category == 3)//导购员
                    {
                        if (recom.product_scope > 1 && recom.target_content == 2)
                        {
                            //snList = db.Queryable<daoben_product_sn>()
                            //            .JoinTable<daoben_activity_recommendation_product>((a, b) => a.model == b.model && a.color == b.color)
                            //            .JoinTable<daoben_activity_recommendation_emp>((a, c) => a.out_guide_id == c.emp_id)
                            //            .Where<daoben_activity_recommendation_product, daoben_activity_recommendation_emp>((a, b, c) => b.main_id == recom.id && a.status > 0 &&
                            //                c.main_id == recom.id && a.outstorage_time >= startDate && a.outstorage_time < endDate)
                            //            .Select("a.*").ToList();
                        }
                        else if (recom.product_scope > 1 && recom.target_content == 1)
                        {
                            snList = db.Queryable<daoben_product_sn>()
                                        .JoinTable<daoben_activity_recommendation_product>((a, b) => a.model == b.model && a.color == b.color)
                                        .JoinTable<daoben_activity_recommendation_emp>((a, c) => a.reporter_id == c.emp_id)
                                        .Where<daoben_activity_recommendation_product, daoben_activity_recommendation_emp>((a, b, c) => b.main_id == recom.id && a.status > 1 &&
                                            c.main_id == recom.id && a.sale_time >= startDate && a.sale_time < endDate)
                                        .Select("a.*").ToList();
                        }
                        else if (recom.product_scope == 1 && recom.target_content == 1)
                        {
                            snList = db.Queryable<daoben_product_sn>()
                                        .JoinTable<daoben_activity_recommendation_emp>((a, b) => a.reporter_id == b.emp_id)
                                        .Where<daoben_activity_recommendation_emp>((a, b) => b.main_id == recom.id && a.status > 1
                                            && a.sale_time >= startDate && a.sale_time < endDate)
                                        .Select("a.*").ToList();
                        }
                        else if (recom.product_scope == 1 && recom.target_content == 2)
                        {
                            //snList = db.Queryable<daoben_product_sn>()
                            //            .JoinTable<daoben_activity_recommendation_emp>((a, b) => a.out_guide_id == b.emp_id)
                            //            .Where<daoben_activity_recommendation_emp>((a, b) => b.main_id == recom.id && a.status > 0
                            //                && a.outstorage_time >= startDate && a.outstorage_time < endDate)
                            //            .Select("a.*").ToList();
                        }
                    }

                    foreach (var p in productList)
                    {
                        //TODO 全部机型没有productList 是否需要展示详细数据
                        List<daoben_product_sn> snTempList = snList.Where(a => a.model == p.model && a.color == p.color).ToList();
                        p.total_count = snTempList.Count;
                        snTempList = snTempList.Where(t => t.sale_type == 1).ToList();
                        p.total_normal_count = snTempList.Count();
                        if (p.total_count > 0)
                            p.total_normal_amount = p.total_count * (snTempList.First().price_retail);//零售价 * 数量
                        else
                            p.total_normal_amount = 0;
                    }
                    object statisticsTime;
                    if (snList.Count > 1)
                    {
                        if (recom.target_content == 1)
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
                            start_time = recom.start_date,
                            end_time = recom.end_date == null ? DateTime.Now : recom.end_date,
                        };
                    }
                    #endregion
                    object resultObj = new
                    {
                        mainInfo = recom,
                        approveList = approveList,
                        empList = empList,
                        productList = productList,
                        rewardList = rewardList,
                        statisticsTime = statisticsTime,
                        creator_position_name = creator_position_name,
                    };
                    return resultObj.ToJson();

                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
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
                    daoben_activity_recommendation recallInfo = db.Queryable<daoben_activity_recommendation>().InSingle(id);
                    if (recallInfo == null)
                        return "信息错误：活动信息不存在!";
                    if (recallInfo.creator_job_history_id != LoginInfo.jobHistoryId)
                        return "操作失败：活动信息只能由发起人撤回!";
                    if (recallInfo.approve_status != 0)
                        return "操作失败：活动信息已审批!";
                    db.CommandTimeOut = 150;
                    db.BeginTran();
                    if (recallInfo.product_scope == 2 || recallInfo.product_scope == 3)
                        db.Delete<daoben_activity_recommendation_product>(a => a.main_id == id);
                    db.Delete<daoben_activity_recommendation_emp>(a => a.main_id == id);
                    db.Delete<daoben_activity_recommendation>(a => a.id == id);
                    db.Delete<daoben_sys_task>(a => a.main_id == id);
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
        public string Approve(daoben_activity_recommendation_approve approveInfo)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            if (approveInfo == null || string.IsNullOrEmpty(approveInfo.main_id))
                return "信息错误，操作失败!";
            approveInfo.approve_name = LoginInfo.empName;
            approveInfo.approve_position_id = myPositionInfo.id;
            approveInfo.approve_position_name = myPositionInfo.name;
            approveInfo.approve_time = DateTime.Now;
            approveInfo.approve_id = LoginInfo.accountId;
            object upObj = null;
            SqlSugarClient db = null;
            try
            {
                using (db = SugarDao.GetInstance())
                {
                    daoben_activity_recommendation mainInfo = db.Queryable<daoben_activity_recommendation>().InSingle(approveInfo.main_id);
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
                        {
                            mainInfo.approve_status = mainInfo.approve_status + 1;
                            approveInfo.status = mainInfo.approve_status;
                        }
                        else
                        {
                            mainInfo.approve_status = 0 - 1 - mainInfo.approve_status;
                            approveInfo.status = mainInfo.approve_status;
                        }
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
                    //清除之前的待办事项
                    if (origTask.Count() > 0)
                        db.Update<daoben_sys_task>(upObjForTask, a => a.main_id == approveInfo.main_id);
                    db.DisableInsertColumns = new string[] { "id" };
                    db.Insert(approveInfo);
                    db.Update<daoben_activity_recommendation>(upObj, a => a.id == approveInfo.main_id);



                    db.CommitTran();

                    #region 待办事项
                    List<daoben_sys_notification> newsTotal = new List<daoben_sys_notification>();//消息通知
                    List<daoben_sys_task> taskTotal = new List<daoben_sys_task>();//待办事项
                    string newsStr = null;
                    List<string> newsIdList = null;
                    string taskStr = null;
                    List<string> taskIdList = null;
                    //0分公司总经理1——1事业部总经理/助手2——2财务经理100
                    daoben_activity_recommendation origInfo = db.Queryable<daoben_activity_recommendation>().InSingle(approveInfo.main_id);
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
                        newsIdList = db.Queryable<daoben_distributor_info>()
                                 .Where(a => a.id == origInfo.creator_job_history_id).Select<string>("id").ToList();

                    taskStr = "主推奖励活动待审批";
                    //待办事项 生成列表
                    if (taskIdList != null && taskIdList.Count > 0)
                    {
                        taskTotal = taskIdList
                            .Select(a1 => new daoben_sys_task
                            {
                                emp_id = a1,
                                category = 1,
                                main_id = origInfo.id,
                                main_url = "/ActivityManage/Attaining/Approve?id=" + origInfo.id,
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
                            newsStr = "主推奖励活动审批不通过";
                        else if (origInfo.approve_status == 100)
                            newsStr = "主推奖励活动审批通过";
                        //消息通知 生成列表
                        newsTotal = newsIdList
                                .Select(a1 => new daoben_sys_notification
                                {
                                    emp_id = a1,
                                    category = 2,
                                    main_id = origInfo.id,
                                    main_url = "/ActivityManage/Attaining/Show?id=" + origInfo.id,
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
                daoben_activity_recommendation origInfo = db.Queryable<daoben_activity_recommendation>().InSingle(id);
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
                db.Update<daoben_activity_recommendation>(upObj, a => a.id == id);
            }
        }

        public void StatisticsRecom(string id)
        {
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    DateTime now = DateTime.Now;

                    List<daoben_activity_recommendation> recomList = db.Queryable<daoben_activity_recommendation>()
                        .Where(a => a.id == id).ToList();

                    //分别对上述活动进行统计
                    //主推
                    SettlementApp setApp = new SettlementApp();
                    string upDateSql = setApp.EmpRecomReward(db, recomList);
                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    if (!string.IsNullOrEmpty(upDateSql))
                        db.SqlQuery<int>(upDateSql);
                    db.CommitTran();
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    //TODO 
                }
            }

        }

    }
}
