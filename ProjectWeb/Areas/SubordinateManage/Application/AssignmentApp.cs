using Base.Code;
using MySqlSugar;
using ProjectShare.Database;
using ProjectShare.Models;
using ProjectShare.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectWeb.Areas.DistributorManage.Application;
using ProjectWeb.Areas.HumanResource.Application;
using System.Text;

namespace ProjectWeb.Areas.SubordinateManage.Application
{
    public class AssignmentApp

    {
        //TODO delete
        public object GetList(Pagination pagination, daoben_hr_emp_job queryInfo, string name, QueryTime queryTime)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");

            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "entry_date" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;

            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_hr_emp_job>();
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (LoginInfo.roleId == ConstData.ROLE_ID_TERMINALMANAGER)
                    {
                        // 终端经理，查看所有培训师/培训经理/业务员/业务经理/导购员 TODO
                        qable.JoinTable<daoben_org_position>((a, c) => a.position_id == c.id && c.position_type != ConstData.POSITION_OFFICE_NORMAL)
                                .Where(a => (a.company_id_parent == myCompanyInfo.id || a.company_id == myCompanyInfo.id));
                    }
                    else if (LoginInfo.roleId == ConstData.ROLE_ID_SALESMANAGER)
                    {
                        // 业务经理（区域经理），查看本区域内的所有业务员/导购员
                        qable.Where(a => a.area_l1_id == myPositionInfo.areaL1Id && a.id != LoginInfo.empId);
                    }
                    else if (LoginInfo.roleId == ConstData.ROLE_ID_SALES)
                    {
                        // 业务员，查看挂勾的导购员
                    }
                    else if (LoginInfo.roleId == ConstData.ROLE_ID_TRAINER)
                    {
                        // 培训师，查看挂勾的导购员
                    }
                    else if (LoginInfo.roleId == ConstData.ROLE_ID_TRAINERMANAGER)
                    {
                        // 培训经理，查看下属培训师及培训师挂勾的导购员
                    }
                    else if (myPositionInfo.positionType == ConstData.POSITION_GM1
                                    || myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT1)
                    {   // 事业部
                        qable.Where(a =>( a.company_id_parent == myCompanyInfo.id || a.company_id == myCompanyInfo.id));
                    }
                    else if (myPositionInfo.positionType == ConstData.POSITION_GM2 || myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT2)   // 分公司
                        qable.Where(a => a.company_id == myCompanyInfo.id);
                    else if (myPositionInfo.positionType == ConstData.POSITION_DEPT_M)
                        qable.Where(a => a.dept_id == myPositionInfo.id);
                    else
                        qable.Where(a => a.supervisor_id == LoginInfo.empId);
                }

                if (!string.IsNullOrEmpty(name))
                    qable.Where(a => a.name.Contains(name));
                if (queryInfo != null)
                {
                    if (!string.IsNullOrEmpty(queryInfo.work_number))
                        qable.Where(a => a.work_number.Contains(queryInfo.work_number));
                    if (queryInfo.company_id != 0)
                        qable.Where(a => a.company_id == queryInfo.company_id);
                }
                if (queryTime != null)
                {
                    if (queryTime.startTime1 != null)
                        qable.Where(a => a.entry_date >= queryTime.startTime1);
                    if (queryTime.startTime2 != null)
                    {
                        queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                        qable.Where(a => a.entry_date < queryTime.startTime2);
                    }
                }

                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord) // 差售点
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        #region 导购员
        public object GetGuideList(Pagination pagination, string name, bool isAll)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");

            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.entry_date" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;

            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_hr_emp_job>()
                            .JoinTable<daoben_distributor_guide>((a, b) => b.guide_id == a.id && b.inactive == false)
                            .Where(a => a.position_type == ConstData.POSITION_GUIDE);
                if (myCompanyInfo.category == "事业部")
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                else if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                if (!string.IsNullOrEmpty(name))
                    qable.Where(a => a.name.Contains(name));
                if (isAll)
                {
                    qable.Select("SQL_CALC_FOUND_ROWS a.id,a.name,a.name_v2,a.area_l1_id,a.area_l1_name,a.area_l2_id,a.area_l2_name,a.company_id_parent,a.company_id,a.company_linkname,if(b.distributor_name is null, null,GROUP_CONCAT(b.distributor_name)) as d_list_name,1 as linkId")
                            .GroupBy("a.id");
                }
                else
                {
                    qable.Where<daoben_distributor_guide>((a, b) => b.distributor_name == null)  // 即不存在b表的记录
                        .Select("SQL_CALC_FOUND_ROWS a.id,a.name,a.name_v2,a.area_l1_id,a.area_l1_name,a.area_l2_id,a.area_l2_name,a.company_id_parent,a.company_id,a.company_linkname,null as d_list_name,0 as linkId");
                }

                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .ToJsonPage(pagination.page, pagination.rows);
                pagination.records = db.SqlQuery<int>("SELECT FOUND_ROWS() as count").FirstOrDefault();// records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }
        public string GuideAdd(string empId, int area_l2_id, List<daoben_distributor_info> distributorList, DateTime? effect_date)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;

            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    List<daoben_distributor_guide> guideLinkList = new List<daoben_distributor_guide>();
                    List<daoben_sys_notification> newsTotal = new List<daoben_sys_notification>();
                    daoben_org_area areaInfo = db.Queryable<daoben_org_area>().InSingle(area_l2_id);
                    if (areaInfo == null)
                        return "信息错误：指定的区域不存在！";
                    daoben_hr_emp_job empInfo = db.Queryable<daoben_hr_emp_job>().InSingle(empId);
                    if (empInfo == null || empInfo.position_type != ConstData.POSITION_GUIDE)
                        return "信息错误：指定的导购员不存在！";
                    if (distributorList == null || distributorList.Count < 1)
                        return "信息错误：至少指派一个经销商";
                    //找出原来的jxs list
                    List<daoben_distributor_info> origDistriList = db.Queryable<daoben_distributor_info>()
                        .JoinTable<daoben_distributor_guide>((a, b) => a.id == b.distributor_id && b.inactive == false)
                        .Where<daoben_distributor_guide>((a, b) => b.guide_id == empId && b.inactive == false).Select("a.*").GroupBy("a.id").ToList();//???
                    //不变动jxs                  
                    List<daoben_distributor_info> stayDistriList = origDistriList.Where(a => distributorList.Any(b => b.id == a.id)).ToList();

                    //解除jxs
                    List<daoben_distributor_info> inactiveDistriList = origDistriList.Where(a => stayDistriList.All(b => b.id != a.id)).ToList();
                    //注销解除jxs的数据 通知失效jxs

                    #region 通知解除关系的经销商
                    string InactiveDistriStr = "导购员" + empInfo.name + "已与您解除指派关系";
                    List<daoben_sys_notification> inactiveNewsList = inactiveDistriList
                        .Select(a1 => new daoben_sys_notification
                        {
                            emp_id = a1.id,
                            category = 3,
                            main_id = null,
                            main_url = "",
                            title = InactiveDistriStr,
                            content_abstract = InactiveDistriStr,
                            recipient_type = 1,
                            create_time = now,
                            status = 1
                        }).ToList();
                    newsTotal.AddRange(inactiveNewsList);
                    #endregion

                    //新增jxs
                    List<daoben_distributor_info> newDistriList = distributorList.Where(a => stayDistriList.All(b => b.id != a.id)).ToList();
                    //建立联系 通知新增的jxs 
                    foreach (var a in newDistriList)
                    {
                        daoben_distributor_guide tempGuideLink = new daoben_distributor_guide();
                        if (a.id == null || a.id == "")
                            return "信息错误：指派的经销商ID不能为空";
                        if (a.name == null || a.name == "")
                            return "信息错误：指派的经销商名称不能为空";
                        tempGuideLink.guide_id = empInfo.id;
                        tempGuideLink.guide_name = empInfo.name;
                        tempGuideLink.distributor_id = a.id;
                        tempGuideLink.distributor_name = a.name;
                        tempGuideLink.effect_date = effect_date;
                        tempGuideLink.creator_job_history_id = LoginInfo.jobHistoryId;
                        tempGuideLink.creator_name = LoginInfo.empName;
                        tempGuideLink.create_time = now;
                        tempGuideLink.inactive = false;
                        guideLinkList.Add(tempGuideLink);
                    }

                    #region 通知建立关系的经销商
                    string newDistriStr = "导购员" + empInfo.name + "已与您建立指派关系";
                    List<daoben_sys_notification> newsList = newDistriList
                        .Select(a1 => new daoben_sys_notification
                        {
                            emp_id = a1.id,
                            category = 3,
                            main_id = null,
                            main_url = "",
                            title = newDistriStr,
                            content_abstract = newDistriStr,
                            recipient_type = 1,
                            create_time = now,
                            status = 1
                        }).ToList();
                    newsTotal.AddRange(newsList);
                    #endregion

                    //判断是否有区域变动
                    bool isAreaL2Change = empInfo.area_l2_id != area_l2_id;
                    if (isAreaL2Change)
                    {

                        #region TODO 消息通知
                        //给导购员本人的通知
                        string MyNewsStr = "您已产生新的指派关系";
                        daoben_sys_notification myNews = new daoben_sys_notification()
                        {
                            emp_id = empId,
                            category = 3,
                            main_id = null,
                            main_url = "",
                            title = MyNewsStr,
                            content_abstract = MyNewsStr,
                            recipient_type = 1,
                            create_time = now,
                            status = 1
                        };
                        newsTotal.Add(myNews);
                        #endregion
                    }
                    if (empInfo.company_id > 0 && empInfo.company_id != areaInfo.company_id)
                    {
                        return "信息错误：不能指派至原分公司之外的区域";
                    }

                    empInfo.company_id_parent = areaInfo.company_id_parent;
                    empInfo.company_name = areaInfo.company_name;
                    empInfo.company_linkname = areaInfo.company_linkname;
                    empInfo.area_l1_id = areaInfo.parent_id;
                    empInfo.area_l1_name = areaInfo.parent_name;
                    empInfo.area_l2_id = areaInfo.id;
                    empInfo.area_l2_name = areaInfo.name;


                    List<daoben_distributor_guide> inactiveDistriLink = db.Queryable<daoben_distributor_guide>()
                        .Where(a => a.guide_id == empId && a.inactive == false).ToList();
                    inactiveDistriLink = inactiveDistriLink.Where(a => inactiveDistriList.Any(b => b.id == a.distributor_id)).ToList();
                    foreach (var a in inactiveDistriLink)
                    {
                        a.inactive = true;
                        a.inactive_job_history_id = LoginInfo.jobHistoryId;
                        a.inactive_time = now;
                    }
                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    db.UpdateRange(inactiveDistriLink);

                    //新建指派 
                    if (guideLinkList != null && guideLinkList.Count > 25)
                        db.SqlBulkCopy(guideLinkList);
                    else if (guideLinkList != null && guideLinkList.Count > 0)
                        db.InsertRange(guideLinkList);
                    if (isAreaL2Change)
                    {
                        //将前面的注销
                        object inactiveObj = new
                        {
                            inactive = true,
                            inactive_pisition_name = myPositionInfo.name,
                            inactive_job_history_id = LoginInfo.jobHistoryId,
                            inactive_time = now
                        };
                        db.Update<daoben_hr_emp_job_history>(inactiveObj, a => a.inactive == false && a.emp_id == empInfo.id);
                        //生成新的job_history
                        EmployeeManageApp employeeManager = new EmployeeManageApp();
                        daoben_hr_emp_job_history newHistory = employeeManager.NewHistoryInfo(empInfo, LoginInfo, effect_date);
                        //区域信息修改
                        object upObj = new
                        {
                            company_id = areaInfo.company_id,
                            company_id_parent = areaInfo.company_id_parent,
                            company_name = areaInfo.company_name,
                            company_linkname = areaInfo.company_linkname,
                            area_l1_id = areaInfo.parent_id,
                            area_l1_name = areaInfo.parent_name,
                            area_l2_id = areaInfo.id,
                            area_l2_name = areaInfo.name,
                            cur_job_history_id = newHistory.id
                        };
                        db.Update<daoben_hr_emp_job>(upObj, a => a.id == empId);
                        db.Insert(newHistory);
                    }
                    if (newsTotal != null && newsTotal.Count > 25)
                        db.SqlBulkCopy(newsTotal);
                    else if (newsTotal != null && newsTotal.Count > 0)
                        db.InsertRange(newsTotal);
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
        #endregion
        #region 业务员
        public object GetSalesList(Pagination pagination, string name, bool isAll)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");

            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "entry_date" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;

            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_hr_emp_job>()
                            .Where(a => a.position_type == ConstData.POSITION_SALES);
                if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                else if (myCompanyInfo.category == "事业部")
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                if (!isAll)
                    qable.Where(a => a.area_l2_id == 0);

                if (!string.IsNullOrEmpty(name))
                    qable.Where(a => a.name.Contains(name));
                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .Select("id, name, name_v2, area_l1_id, area_l1_name, area_l2_id, area_l2_name, company_linkname")
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="empId"></param>
        /// <param name="area_l2_id"></param>
        /// <param name="effect_date">上次指派日期 - 当前日期（预留了往后日期的功能）</param>
        /// <returns></returns>
        public string SalesAdd(string empId, int area_l2_id, DateTime? effect_date)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_hr_emp_job empInfo = db.Queryable<daoben_hr_emp_job>().InSingle(empId);
                    if (empInfo == null)
                        return "信息错误：指定的业务员不存在！";
                    daoben_org_area areaInfo = db.Queryable<daoben_org_area>().InSingle(area_l2_id);
                    if (areaInfo == null)
                        return "信息错误：指定的区域不存在！";
                    daoben_hr_emp_job origSales = db.Queryable<daoben_hr_emp_job>()
                        .SingleOrDefault(a => a.area_l2_id == areaInfo.id && a.position_type == ConstData.POSITION_SALES);
                    if (origSales != null)
                        return "信息错误：此业务片区已有业务员: " + origSales.name + " !请先解绑！";



                    #region 解除关系 消息通知 业务片区+业务经理
#if false
                    //给自己的通知
                    List<daoben_sys_notification> newsTotal = new List<daoben_sys_notification>();
                    string MyNewsStr = "您已产生新的指派关系";
                    daoben_sys_notification myNews = new daoben_sys_notification()
                    {
                        emp_id = empId,
                        category = 3,
                        main_id = null,
                        main_url = "",
                        title = MyNewsStr,
                        content_abstract = MyNewsStr,
                        recipient_type = 1,
                        create_time = now,
                        status = 1
                    };
                    newsTotal.Add(myNews);
                    //给解除经销商 的通知
                    string distributorNewStr = "业务员" + empJob.name + "已与您解除指派关系";
                    List<string> distributorIdList = db.Queryable<daoben_distributor_info>()
                        .Where(a => a.area_l2_id == empJob.area_l2_id)
                        .Select<string>("id").ToList();
                    List<daoben_sys_notification> distributorNewsList = distributorIdList
                        .Select(a1 => new daoben_sys_notification
                        {
                            emp_id = a1,
                            category = 3,
                            main_id = null,
                            main_url = "",
                            title = distributorNewStr,
                            content_abstract = distributorNewStr,
                            recipient_type = 1,
                            create_time = now,
                            status = 1
                        }).ToList();
                    newsTotal.AddRange(distributorNewsList);
#endif
                    #endregion
                    #region 建立指派关系 消息通知 业务片区+业务经理
#if false
                    //给新指派区域内的 经销商 的通知
                    string newDistriNewStr = "业务员" + empJob.name + "已与您建立指派关系";
                    List<string> distriIdList = db.Queryable<daoben_distributor_info>()
                        .Where(a => a.area_l2_id == empJob.area_l2_id)
                        .Select<string>("id").ToList();
                    List<daoben_sys_notification> distriNewsList = distriIdList
                        .Select(a1 => new daoben_sys_notification
                        {
                            emp_id = a1,
                            category = 3,
                            main_id = null,
                            main_url = "",
                            title = newDistriNewStr,
                            content_abstract = newDistriNewStr,
                            recipient_type = 1,
                            create_time = now,
                            status = 1
                        }).ToList();
                    newsTotal.AddRange(distriNewsList);
#endif
                    #endregion

                    int origL2Id = empInfo.area_l2_id;
                    if (empInfo.company_id > 0 && empInfo.company_id != areaInfo.company_id)
                    {
                        return "信息错误：不能指派至原分公司之外的区域";
                    }
                    empInfo.company_id_parent = areaInfo.company_id_parent;
                    empInfo.company_name = areaInfo.company_name;
                    empInfo.company_linkname = areaInfo.company_linkname;
                    empInfo.area_l1_id = areaInfo.parent_id;
                    empInfo.area_l1_name = areaInfo.parent_name;
                    empInfo.area_l2_id = areaInfo.id;
                    empInfo.area_l2_name = areaInfo.name;
                    EmployeeManageApp employeeManager = new EmployeeManageApp();
                    daoben_hr_emp_job_history newHistory = employeeManager.NewHistoryInfo(empInfo, LoginInfo, effect_date);
                    daoben_hr_emp_area_history areaHistory = employeeManager.NewAreaHistoryInfo(empInfo, LoginInfo, effect_date);
                    if (effect_date > now)
                    {
                        newHistory.inactive = true;
                        areaHistory.inactive = true;
                    }
                    db.CommandTimeOut = 30;
                    db.BeginTran();

                    //将前面的注销
                    if (areaHistory.inactive == false)
                    {
                        object inactiveObj = new
                        {
                            inactive = true,
                            inactive_pisition_name = myPositionInfo.name,
                            inactive_job_history_id = LoginInfo.jobHistoryId,
                            inactive_time = now
                        };
                        db.Update<daoben_hr_emp_job_history>(inactiveObj, a => a.emp_id == empId && a.inactive == false);
                        db.Delete<daoben_hr_emp_job_history>(a => a.emp_id == empId && a.effect_date > now);  // 以前设置过将来生效的信息
                        inactiveObj = new
                        {
                            inactive = true,
                            inactive_time = now
                        };
                        db.Update<daoben_hr_emp_area_history>(inactiveObj, a => a.emp_id == empId && a.inactive == false);
                        db.Delete<daoben_hr_emp_area_history>(a => a.emp_id == empId && a.effect_date > now); // 以前设置过将来生效的信息
                        object upObj = new
                        {
                            company_id = areaInfo.company_id,
                            company_id_parent = areaInfo.company_id_parent,
                            company_name = areaInfo.company_name,
                            company_linkname = areaInfo.company_linkname,
                            area_l1_id = areaInfo.parent_id,
                            area_l1_name = areaInfo.parent_name,
                            area_l2_id = areaInfo.id,
                            area_l2_name = areaInfo.name,
                            cur_job_history_id = newHistory.id
                        };
                        db.Update<daoben_hr_emp_job>(upObj, a => a.id == empId);
                        db.Update<daoben_distributor_info>(new { sales_id = 0, sales_name = "-" }, a => a.area_l2_id == origL2Id);
                        db.Update<daoben_distributor_info>(new { sales_id = empInfo.id, sales_name = empInfo.name }, a => a.area_l2_id == areaInfo.id);
                        //if (newsTotal != null && newsTotal.Count > 25)
                        //    db.SqlBulkCopy(newsTotal);
                        //else if (newsTotal != null && newsTotal.Count > 0)
                        //    db.InsertRange(newsTotal);
                    }
                    db.Insert(newHistory);
                    db.Insert(areaHistory);

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
        #endregion

        #region 业务经理
        public object GetSalesManager(Pagination pagination, string name, bool isAll)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "entry_date" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;

            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_hr_emp_job>()
                    .Where(a => a.position_type == ConstData.POSITION_SALESMANAGER);
                if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                else if (myCompanyInfo.category == "事业部")
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);

                if (!isAll)
                    qable.Where(a => a.area_l1_id == 0);
                if (!string.IsNullOrEmpty(name))
                    qable.Where(a => a.name.Contains(name));
                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .Select("id, name, name_v2, area_l1_id, area_l1_name, area_l2_id, area_l2_name, company_linkname")
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }
        public string SalesManageAdd(string empId, int area_l1_id, DateTime? effect_date)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_hr_emp_job empInfo = db.Queryable<daoben_hr_emp_job>().SingleOrDefault(a => a.id == empId);
                    if (empInfo == null || empInfo.position_type != ConstData.POSITION_SALESMANAGER)
                        return "信息错误：指定的业务经理不存在！";
                    daoben_org_area areaInfo = db.Queryable<daoben_org_area>().SingleOrDefault(a => a.id == area_l1_id);
                    if (areaInfo == null)
                        return "信息错误：指定的区域不存在！";
                    daoben_hr_emp_job origSales = db.Queryable<daoben_hr_emp_job>()
                        .SingleOrDefault(a => a.area_l1_id == areaInfo.id && a.position_type == ConstData.POSITION_SALESMANAGER);
                    if (origSales != null && origSales.id == empId)
                        return "success";
                    if (origSales != null)
                        return "信息错误：此经理片区已有业务经理:" + origSales.name + " !请先解绑！";

                    //区域信息修改
                    object upObj = new
                    {
                        company_id = areaInfo.company_id,
                        company_id_parent = areaInfo.company_id_parent,
                        company_name = areaInfo.company_name,
                        company_linkname = areaInfo.company_linkname,
                        area_l1_id = areaInfo.id,
                        area_l1_name = areaInfo.name,
                    };
                    #region 解除指派关系 消息通知 经理片区
#if false
                    //给自己的通知
                    List<daoben_sys_notification> newsTotal = new List<daoben_sys_notification>();
                    string MyNewsStr = "您已产生新的指派关系";
                    daoben_sys_notification myNews = new daoben_sys_notification()
                    {
                        emp_id = empId,
                        category = 3,
                        main_id = null,
                        main_url = "",
                        title = MyNewsStr,
                        content_abstract = MyNewsStr,
                        recipient_type = 1,
                        create_time = now,
                        status = 1
                    };
                    newsTotal.Add(myNews);
                    //给经销商 的通知
                    string distributorNewStr = "业务经理" + empJob.name + "已与您解除指派关系";
                    List<string> distributorIdList = db.Queryable<daoben_distributor_info>()
                        .Where(a => a.area_l1_id == empJob.area_l1_id)
                        .Select<string>("id").ToList();
                    List<daoben_sys_notification> distributorNewsList = distributorIdList
                        .Select(a1 => new daoben_sys_notification
                        {
                            emp_id = a1,
                            category = 3,
                            main_id = null,
                            main_url = "",
                            title = distributorNewStr,
                            content_abstract = distributorNewStr,
                            recipient_type = 1,
                            create_time = now,
                            status = 1
                        }).ToList();
                    newsTotal.AddRange(distributorNewsList);
#endif
                    #endregion
                    #region 新建指派关系 消息通知 经理片区
#if false
                    //给经销商 的通知
                    string distriNewStr = "业务经理" + empJob.name + "已与您建立指派关系";
                    List<string> distriIdList = db.Queryable<daoben_distributor_info>()
                        .Where(a => a.area_l1_id == empJob.area_l1_id)
                        .Select<string>("id").ToList();
                    List<daoben_sys_notification> distriNewsList = distriIdList
                        .Select(a1 => new daoben_sys_notification
                        {
                            emp_id = a1,
                            category = 3,
                            main_id = null,
                            main_url = "",
                            title = distriNewStr,
                            content_abstract = distriNewStr,
                            recipient_type = 1,
                            create_time = now,
                            status = 1
                        }).ToList();
                    newsTotal.AddRange(distriNewsList);
#endif
                    #endregion

                    if (empInfo.company_id > 0 && empInfo.company_id != areaInfo.company_id)
                    {
                        return "信息错误：不能指派至原分公司之外的区域";
                    }
                    empInfo.company_id_parent = areaInfo.company_id_parent;
                    empInfo.company_name = areaInfo.company_name;
                    empInfo.company_linkname = areaInfo.company_linkname;
                    empInfo.area_l1_id = areaInfo.id;
                    empInfo.area_l1_name = areaInfo.name;
                    EmployeeManageApp employeeManager = new EmployeeManageApp();
                    daoben_hr_emp_job_history newHistory = employeeManager.NewHistoryInfo(empInfo, LoginInfo, effect_date);
                    daoben_hr_emp_area_history areaHistory = employeeManager.NewAreaHistoryInfo(empInfo, LoginInfo, effect_date);
                    if (effect_date > now)
                    {
                        newHistory.inactive = true;
                        areaHistory.inactive = true;
                    }
                    db.CommandTimeOut = 30;
                    db.BeginTran();

                    //将前面的注销
                    if (areaHistory.inactive == false)
                    {
                        object inactiveObj = new
                        {
                            inactive = true,
                            inactive_pisition_name = myPositionInfo.name,
                            inactive_job_history_id = LoginInfo.jobHistoryId,
                            inactive_time = now
                        };
                        db.Update<daoben_hr_emp_job_history>(inactiveObj, a => a.emp_id == empId && a.inactive == false);
                        db.Delete<daoben_hr_emp_job_history>(a => a.emp_id == empId && a.effect_date > now);  // 以前设置过将来生效的信息
                        inactiveObj = new
                        {
                            inactive = true,
                            inactive_time = now
                        };
                        db.Update<daoben_hr_emp_area_history>(inactiveObj, a => a.emp_id == empId && a.inactive == false);
                        db.Delete<daoben_hr_emp_area_history>(a => a.emp_id == empId && a.effect_date > now); // 以前设置过将来生效的信息
                        upObj = new
                        {
                            company_id = areaInfo.company_id,
                            company_id_parent = areaInfo.company_id_parent,
                            company_name = areaInfo.company_name,
                            company_linkname = areaInfo.company_linkname,
                            area_l1_id = areaInfo.id,
                            area_l1_name = areaInfo.name,
                            cur_job_history_id = newHistory.id
                        };
                        db.Update<daoben_hr_emp_job>(upObj, a => a.id == empId);
                        //// 将串码表中的 sales_m_id/out_sales_m_id置为该业务经理 [error code]
                        //db.Update<daoben_product_sn>(new { sales_m_id = empId }, a => a.sale_time >= effect_date);
                        //db.Update<daoben_product_sn>(new { out_sales_m_id = empId }, a => a.outstorage_time >= effect_date);

                        //if (newsTotal != null && newsTotal.Count > 25)
                        //    db.SqlBulkCopy(newsTotal);
                        //else if (newsTotal != null && newsTotal.Count > 0)
                        //    db.InsertRange(newsTotal);
                    }
                    db.Insert(newHistory);
                    db.Insert(areaHistory);
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
        #endregion

        #region 培训师
        public object GetTrainer(Pagination pagination, string name, bool isAll)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "entry_date" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;

            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_hr_emp_job>()
                    .Where(a => a.position_type == ConstData.POSITION_TRAINER);
                if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                else if (myCompanyInfo.category == "事业部")
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);

                if (!isAll)
                    qable.Where(a => a.area_l1_id == 0);
                if (!string.IsNullOrEmpty(name))
                    qable.Where(a => a.name.Contains(name));
                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .Select("id, name, name_v2, area_l1_id, area_l1_name, area_l2_id, area_l2_name, company_linkname")
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }
        public string TrainerAdd(string empId, int area_l1_id)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_hr_emp_job empJob = db.Queryable<daoben_hr_emp_job>().InSingle(empId);
                    if (empJob == null || empJob.position_type != ConstData.POSITION_TRAINER)
                        return "信息错误：指定的培训师不存在！";
                    daoben_org_area areaInfo = db.Queryable<daoben_org_area>().InSingle(area_l1_id);
                    if (areaInfo == null)
                        return "信息错误：指定的区域不存在！";
                    daoben_hr_emp_job origTrainer = db.Queryable<daoben_hr_emp_job>()
                        .SingleOrDefault(a => a.area_l1_id == areaInfo.id && a.position_type == ConstData.POSITION_TRAINER);
                    if (origTrainer != null)
                        return "信息错误：此经理片区已有培训师:" + origTrainer.name + " !请先解绑！";

                    empJob.company_id = areaInfo.company_id;
                    empJob.company_id_parent = areaInfo.company_id_parent;
                    empJob.company_name = areaInfo.company_name;
                    empJob.company_linkname = areaInfo.company_linkname;
                    empJob.area_l1_id = areaInfo.id;
                    empJob.area_l1_name = areaInfo.name;
                    //区域信息修改
                    object upObj = new
                    {
                        company_id = areaInfo.company_id,
                        company_id_parent = areaInfo.company_id_parent,
                        company_name = areaInfo.company_name,
                        company_linkname = areaInfo.company_linkname,
                        area_l1_id = areaInfo.id,
                        area_l1_name = areaInfo.name,
                    };

                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    //将前面的注销
                    object inactiveObj = new
                    {
                        inactive = true,
                        inactive_pisition_name = myPositionInfo.name,
                        inactive_job_history_id = LoginInfo.jobHistoryId,
                        inactive_time = now
                    };
                    db.Update<daoben_hr_emp_job_history>(inactiveObj, a => a.inactive == false && a.emp_id == empJob.id);
                    //生成新的job_history
                    EmployeeManageApp employeeManager = new EmployeeManageApp();
                    daoben_hr_emp_job_history newHistory = employeeManager.NewHistoryInfo(empJob, LoginInfo, DateTime.Now.Date);

                    db.Update<daoben_hr_emp_job>(upObj, a => a.id == empId);
                    db.Insert(newHistory);
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
        #endregion

        #region 培训经理
        public object GetTrainerManager(Pagination pagination, string name, bool isAll)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "entry_date" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;

            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_hr_emp_job>()
                    .Where(a => a.position_type == ConstData.POSITION_TRAINERMANAGER);
                if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.company_id == myCompanyInfo.id);
                else if (myCompanyInfo.category == "事业部")
                    qable.Where(a => a.company_id_parent == myCompanyInfo.id);

                if (!isAll)
                    qable.Where(a => a.company_id == 0);
                if (!string.IsNullOrEmpty(name))
                    qable.Where(a => a.name.Contains(name));
                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .Select("id, name, name_v2, area_l1_id, area_l1_name, area_l2_id, area_l2_name, company_linkname")
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }
        public string TrainerManagerAdd(string empId, int company_id)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_hr_emp_job empJob = db.Queryable<daoben_hr_emp_job>().InSingle(empId);
                    if (empJob == null || empJob.position_type != ConstData.POSITION_TRAINERMANAGER)
                        return "信息错误：指定的培训经理不存在！";
                    daoben_org_company companyInfo = db.Queryable<daoben_org_company>().InSingle(company_id);
                    if (companyInfo == null || companyInfo.category != "分公司")
                        return "信息错误：指定的分公司不存在！";
                    daoben_hr_emp_job origEmp = db.Queryable<daoben_hr_emp_job>()
                        .SingleOrDefault(a => a.company_id == companyInfo.id && a.position_type == ConstData.POSITION_TRAINERMANAGER);
                    if (origEmp != null)
                        return "信息错误：此经理片区已有培训经理经理:" + origEmp.name + " !请先解绑！";

                    empJob.company_id = companyInfo.id;
                    empJob.company_id_parent = companyInfo.parent_id;
                    empJob.company_name = companyInfo.name;
                    empJob.company_linkname = companyInfo.link_name;

                    //信息修改
                    object upObj = new
                    {
                        company_id = companyInfo.id,
                        company_id_parent = companyInfo.parent_id,
                        company_name = companyInfo.name,
                        company_linkname = companyInfo.link_name,
                    };

                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    //将前面的注销
                    object inactiveObj = new
                    {
                        inactive = true,
                        inactive_pisition_name = myPositionInfo.name,
                        inactive_job_history_id = LoginInfo.jobHistoryId,
                        inactive_time = now
                    };
                    db.Update<daoben_hr_emp_job_history>(inactiveObj, a => a.inactive == false && a.emp_id == empJob.id);
                    //生成新的job_history
                    EmployeeManageApp employeeManager = new EmployeeManageApp();
                    daoben_hr_emp_job_history newHistory = employeeManager.NewHistoryInfo(empJob, LoginInfo, DateTime.Now.Date);

                    db.Update<daoben_hr_emp_job>(upObj, a => a.id == empId);
                    db.Insert(newHistory);


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
        #endregion
        /// <summary>
        /// 解绑 0: 业务经理；1：业务员；2：导购员 ； 3：培训师；4：培训经理
        /// </summary>
        /// <param name="empId"></param>
        /// <param name="type">0: 业务经理；1：业务员；2：导购员 ； 3：培训师；4：培训经理</param>
        /// <returns></returns>
        public string Remove(string empId, int type)
        {
            if (type > 4 || type < 0)
                return null;
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_hr_emp_job empInfo = db.Queryable<daoben_hr_emp_job>().InSingle(empId);
                    if (empInfo == null)
                        return "信息错误：指定的员工信息不存在";
                    int origL2Id = empInfo.area_l2_id;
                    empInfo.area_l1_id = 0;
                    empInfo.area_l1_name = "-";
                    empInfo.area_l2_id = 0;
                    empInfo.area_l2_name = "-";
                    //区域信息修改
                    object upObj = new
                    {
                        area_l2_id = 0,
                        area_l2_name = "-",
                        area_l1_id = 0,
                        area_l1_name = "-",
                    };

                    EmployeeManageApp employeeManager = new EmployeeManageApp();
                    daoben_hr_emp_job_history newHistory = employeeManager.NewHistoryInfo(empInfo, LoginInfo, now.Date);
                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    //将前面的注销
                    object inactiveObj = new
                    {
                        inactive = true,
                        inactive_pisition_name = myPositionInfo.name,
                        inactive_job_history_id = LoginInfo.jobHistoryId,
                        inactive_time = now
                    };
                    db.Update<daoben_hr_emp_job_history>(inactiveObj, a => a.emp_id == empId && a.inactive == false);
                    db.Delete<daoben_hr_emp_job_history>(a => a.emp_id == empId && a.effect_date > now);  // 以前设置过将来生效的信息
                    db.Insert(newHistory);
                    if (type == 0 || type == 1)
                    {
                        daoben_hr_emp_area_history areaHistory = employeeManager.NewAreaHistoryInfo(empInfo, LoginInfo, now.Date);
                        inactiveObj = new
                        {
                            inactive = true,
                            inactive_time = now
                        };
                        db.Update<daoben_hr_emp_area_history>(inactiveObj, a => a.emp_id == empId && a.inactive == false);
                        db.Delete<daoben_hr_emp_area_history>(a => a.emp_id == empId && a.effect_date > now); // 以前设置过将来生效的信息
                        db.Insert(areaHistory);
                    }

                    if (type == 2)//解绑导购员 1-1业务员 1-N经销商 区域信息
                    {
                        #region 消息通知 导购员，经销商，业务员,业务经理
                        //给自己的通知
                        List<daoben_sys_notification> newsTotal = new List<daoben_sys_notification>();
                        string MyNewsStr = "您已解除指派关系";
                        daoben_sys_notification myNews = new daoben_sys_notification()
                        {
                            emp_id = empId,
                            category = 3,
                            main_id = null,
                            main_url = "",
                            title = MyNewsStr,
                            content_abstract = MyNewsStr,
                            recipient_type = 1,
                            create_time = now,
                            status = 1
                        };
                        newsTotal.Add(myNews);
                        //给经销商的通知
                        string distributorNewStr = "导购员" + empInfo.name + "已与您解除指派关系";
                        List<string> distributorIdList = db.Queryable<daoben_distributor_guide>()
                            .Where(a => a.guide_id == empId)
                            .Select<string>("distributor_id as id").ToList();
                        List<daoben_sys_notification> distributorNewsList = distributorIdList
                            .Select(a1 => new daoben_sys_notification
                            {
                                emp_id = a1,
                                category = 3,
                                main_id = null,
                                main_url = "",
                                title = distributorNewStr,
                                content_abstract = distributorNewStr,
                                recipient_type = 1,
                                create_time = now,
                                status = 1
                            }).ToList();
                        newsTotal.AddRange(distributorNewsList);
                        if (newsTotal != null && newsTotal.Count > 25)
                            db.SqlBulkCopy(newsTotal);
                        else if (newsTotal != null && newsTotal.Count > 0)
                            db.InsertRange(newsTotal);
                        #endregion

                        db.Delete<daoben_distributor_guide>(a => a.guide_id == empId);
                        db.Update<daoben_hr_emp_job>(upObj, a => a.id == empId);
                    }
                    else if (type == 1)//解绑业务员 1-N导购员 1-N经销商 区域信息
                    {
                        #region 消息通知 业务片区+业务经理
                        //给自己的通知
                        List<daoben_sys_notification> newsTotal = new List<daoben_sys_notification>();
                        string MyNewsStr = "您已解除指派关系";
                        daoben_sys_notification myNews = new daoben_sys_notification()
                        {
                            emp_id = empId,
                            category = 3,
                            main_id = null,
                            main_url = "",
                            title = MyNewsStr,
                            content_abstract = MyNewsStr,
                            recipient_type = 1,
                            create_time = now,
                            status = 1
                        };
                        newsTotal.Add(myNews);
                        //给经销商 的通知
                        string distributorNewStr = "业务员" + empInfo.name + "已与您解除指派关系";
                        List<string> distributorIdList = db.Queryable<daoben_distributor_info>()
                            .Where(a => a.area_l2_id == empInfo.area_l2_id)
                            .Select<string>("id").ToList();
                        List<daoben_sys_notification> distributorNewsList = distributorIdList
                            .Select(a1 => new daoben_sys_notification
                            {
                                emp_id = a1,
                                category = 3,
                                main_id = null,
                                main_url = "",
                                title = distributorNewStr,
                                content_abstract = distributorNewStr,
                                recipient_type = 1,
                                create_time = now,
                                status = 1
                            }).ToList();
                        newsTotal.AddRange(distributorNewsList);

                        if (newsTotal != null && newsTotal.Count > 25)
                            db.SqlBulkCopy(newsTotal);
                        else if (newsTotal != null && newsTotal.Count > 0)
                            db.InsertRange(newsTotal);
                        #endregion
                        db.Update<daoben_distributor_info>(new { sales_id = 0, sales_name = "-" }, a => a.area_l2_id == origL2Id);
                        db.Update<daoben_hr_emp_job>(upObj, a => a.id == empId);
                    }
                    else if (type == 0)//解绑业务经理 区域信息
                    {
                        #region 消息通知 经理片区
                        //给自己的通知
                        List<daoben_sys_notification> newsTotal = new List<daoben_sys_notification>();
                        string MyNewsStr = "您已解除指派关系";
                        daoben_sys_notification myNews = new daoben_sys_notification()
                        {
                            emp_id = empId,
                            category = 3,
                            main_id = null,
                            main_url = "",
                            title = MyNewsStr,
                            content_abstract = MyNewsStr,
                            recipient_type = 1,
                            create_time = now,
                            status = 1
                        };
                        newsTotal.Add(myNews);
                        //给经销商 的通知
                        string distributorNewStr = "业务经理" + empInfo.name + "已与您解除指派关系";
                        List<string> distributorIdList = db.Queryable<daoben_distributor_info>()
                            .Where(a => a.area_l1_id == empInfo.area_l1_id)
                            .Select<string>("id").ToList();
                        List<daoben_sys_notification> distributorNewsList = distributorIdList
                            .Select(a1 => new daoben_sys_notification
                            {
                                emp_id = a1,
                                category = 3,
                                main_id = null,
                                main_url = "",
                                title = distributorNewStr,
                                content_abstract = distributorNewStr,
                                recipient_type = 1,
                                create_time = now,
                                status = 1
                            }).ToList();
                        newsTotal.AddRange(distributorNewsList);

                        if (newsTotal != null && newsTotal.Count > 25)
                            db.SqlBulkCopy(newsTotal);
                        else if (newsTotal != null && newsTotal.Count > 0)
                            db.InsertRange(newsTotal);
                        #endregion
                        db.Update<daoben_hr_emp_job>(upObj, a => a.id == empId);
                    }
#if false
                    else if (type == 3)//解绑培训师
                    {
                        db.Update<daoben_hr_emp_job>(upObj, a => a.id == empId);
                    }
                    else if (type == 4)
                    {
                        object upObjTemp = new
                        {
                            company_id = 0,
                            company_name = "-",
                            company_id_parent = 0,
                            company_linkname = "-",
                        };
                        db.Update<daoben_hr_emp_job>(upObjTemp, a => a.id == empId);
                    }
#endif
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

        public string GetGuideInfo(string id)
        {
            DateTime? entryDate = null;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_hr_emp_job jobInfo = db.Queryable<daoben_hr_emp_job>().SingleOrDefault(a => a.id == id);
                    if (jobInfo == null)
                        return null;
                    DistributorManageApp distriApp = new DistributorManageApp();
                    StringBuilder distriNameStr = new StringBuilder();
                    List<daoben_distributor_guide> distributorList = db.Queryable<daoben_distributor_guide>()
                                .Where(a => a.guide_id == id && a.inactive == false)
                                .OrderBy(a => a.distributor_name).ToList();
                    if (distributorList == null || distributorList.Count < 1)
                        entryDate = jobInfo.entry_date;
                    else
                    {
                        foreach (var a in distributorList)
                        {
                            distriNameStr.Append(" " + a.distributor_name + ",");
                        }
                        entryDate = distributorList.Max(a => a.effect_date);
                    }
                    if (distriNameStr.Length > 1)
                        distriNameStr.Remove(distriNameStr.Length - 1, 1); // 最后一个逗号
                    object resultObj = new
                    {
                        company_id = jobInfo.company_id,
                        company_name = jobInfo.company_linkname,
                        area_l1_name = jobInfo.area_l1_name,
                        area_l2_name = jobInfo.area_l2_name,
                        effect_date = entryDate,
                        distributor_name = distriNameStr.ToString(),
                    };
                    return resultObj.ToJson();

                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        public string GetSalesInfo(string id)
        {
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_hr_emp_area_history areaHistory = db.Queryable<daoben_hr_emp_area_history>()
                                .Where(a => a.emp_id == id && a.effect_date < DateTime.Now)
                                .OrderBy(a => a.effect_date, OrderByType.Desc).FirstOrDefault();
                    if (areaHistory == null)
                    {
                        areaHistory = db.Queryable<daoben_hr_emp_job>()
                            .Where(a => a.id == id).Select<daoben_hr_emp_area_history>("company_id,company_name,entry_date as effect_date")
                            .SingleOrDefault();
                    }
                    return areaHistory.ToJson();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
    }
}
