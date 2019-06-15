using Base.Code;
using MySqlSugar;
using ProjectShare.Database;
using ProjectShare.Models;
using ProjectShare.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using ProjectWeb.Areas.SystemManage.Application;
using System.Text;
using ProjectWeb.Application;

namespace ProjectWeb.Areas.FinancialAccounting.Application
{
    public class PositionSalaryApp
    {
        MsAccountApp msAccount = new MsAccountApp();

        /// <summary>
        /// 查看当前生效的：queryInfo.effect_status = 1;
        /// 查看历史：queryInfo.effect_status = 0;
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryInfo"></param>
        /// <returns></returns>
        public object GetList(Pagination pagination, daoben_salary_position queryInfo, QueryTime queryTime)
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
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "company_id" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "asc" : pagination.sord;

            AuthMenuButton authInfo = new SysAuthorityApp().GetAuthorityCache(LoginInfo.roleId);
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    var qable = db.Queryable<daoben_salary_position>();
                    if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)    // 跳过超级管理员，避免判断公司/职位信息是否存在
                    {
                        // todo 重写 2019-3-6
                        ////if (!(authInfo.menuList.Exists(a => a.id == ConstData.MENU_ID_POSITIONSALARY)))
                        ////    return "无权限，非法访问";
                        //if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIAL || LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER)
                        //    qable.Where(a => a.company_id_parent == myCompanyInfo.id || a.company_id == myCompanyInfo.id);
                        //else if (authInfo.buttonList.Exists(a => a.id == ConstData.BUTTON_ID_TRAINER_KPI_ADJUST))
                        //{   // 有培训KPI设置权限的仅可查看培训KPI（终端经理）
                        //    qable.Where(a => (a.category == 11 || a.category == 12) &&
                        //                (a.company_id_parent == myCompanyInfo.id || a.company_id == myCompanyInfo.id));
                        //}
                        //else if (authInfo.buttonList.Exists(a => a.id == ConstData.BUTTON_ID_DEPT_KPI_ADJUST))
                        //{   // 有部门KPI设置权限的仅可查看部门KPI
                        //    qable.Where(a => a.category == 13 && (a.company_id_parent == myCompanyInfo.id || a.company_id == myCompanyInfo.id));
                        //}
                        //else
                        if (myCompanyInfo.category == "分公司")
                        {   // 分公司人员，包括分公司总经理/分公司总经理助理仅可查看本机构的
                            qable.Where(a => a.company_id == myCompanyInfo.id);
                        }
                        else if (myCompanyInfo.category == "事业部")
                            qable.Where(a => (a.company_id_parent == myCompanyInfo.id || a.company_id == myCompanyInfo.id));
                    }
                    if (queryInfo != null)
                    {
                        if (queryInfo.effect_status == 1)     // 查看所有已生效的，即当前岗位薪资
                            qable.Where(a => a.effect_status == 1);

                        if (queryInfo.approve_status != 0)
                        {
                            if (queryInfo.approve_status == 100)
                                qable.Where(a => a.approve_status == 100);
                            else if (queryInfo.approve_status == -100)
                                qable.Where(a => a.approve_status < 0);
                            else if (queryInfo.approve_status == 1)
                                qable.Where(a => a.approve_status < 1 && a.approve_status > 0);
                            else if (queryInfo.approve_status == -1)
                                qable.Where(a => a.approve_status == 0);
                        }
                        if (queryInfo.category > 0)
                            qable.Where(a => a.category == queryInfo.category);
                        if (queryInfo.company_id > 0)
                            qable.Where(a => a.company_id == queryInfo.company_id);
                        if (queryInfo.dept_id > 0)
                            qable.Where(a => a.dept_id == queryInfo.dept_id);
                        if (queryInfo.is_template != 0)
                            qable.Where(a => a.is_template == queryInfo.is_template);
                        if (!string.IsNullOrEmpty(queryInfo.position_name))
                            qable.Where(a => a.position_name.Contains(queryInfo.position_name));
                        if (queryTime.startTime1 != null)
                            qable.Where(a => a.effect_date >= queryTime.startTime1);
                        if (queryTime.startTime2 != null)
                        {
                            DateTime startTime2 = queryTime.startTime2.ToDate().AddMonths(1);
                            qable.Where(a => a.effect_date < startTime2);
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
        public object GetListApprove(Pagination pagination, daoben_salary_position queryInfo)
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
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "create_time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;

            AuthMenuButton authInfo = new SysAuthorityApp().GetAuthorityCache(LoginInfo.roleId);
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    var qable = db.Queryable<daoben_salary_position>();
                    if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)    // 跳过超级管理员，避免判断公司/职位信息是否存在
                    {
                        if (myPositionInfo.positionType == ConstData.POSITION_GM1
                                    || myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT1)
                        {   // 事业部总经理/事业部总经理助理
                            qable.Where(a => a.approve_status == 0 && a.category > 10 &&
                                        (a.company_id_parent == myCompanyInfo.id || a.company_id == myCompanyInfo.id));
                        }
                        else if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER)
                        {   // 财务经理
                            qable.Where(a => (a.company_id_parent == myCompanyInfo.id || a.company_id == myCompanyInfo.id) &&
                                        ((a.approve_status == 0 && a.category < 10) || (a.approve_status == 1 && a.category > 10)));
                        }
                        else
                            return "无权限，非法访问";
                    }
                    if(queryInfo!=null)
                    {
                        if (queryInfo.company_id > 0)
                            qable.Where(a => a.company_id == queryInfo.company_id);
                        if (queryInfo.dept_id > 0)
                            qable.Where(a => a.dept_id == queryInfo.dept_id);
                        if (!string.IsNullOrEmpty(queryInfo.position_name))
                            qable.Where(a => a.position_name.Contains(queryInfo.position_name));
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
        /// 岗位薪资调整、培训KPI调整
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="company_id">筛选条件</param>
        /// <param name="type">0：岗位薪资调整；1：培训KPI调整</param>
        /// <returns></returns>
        public object GetListPosition(Pagination pagination, daoben_salary_position queryInfo, int type)
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
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "create_time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;

            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    var qable = db.Queryable<daoben_org_position>().Where(a => (a.company_id_parent == myCompanyInfo.id
                                    || a.company_id == myCompanyInfo.id));
                    if (queryInfo != null)
                    {
                        if (queryInfo.company_id >0)
                            qable.Where(a => a.company_id == queryInfo.company_id);
                        if (queryInfo.dept_id > 0)
                            qable.Where(a => a.dept_id == queryInfo.dept_id);
                        if (!string.IsNullOrEmpty(queryInfo.position_name))
                            qable.Where(a => a.name.Contains(queryInfo.position_name));
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

        // 部门KPI调整
        public object GetListDept(Pagination pagination, int company_id)
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
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "create_time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;

            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    var qable = db.Queryable<daoben_org_dept>().Where(a =>
                                    (a.company_id_parent == myCompanyInfo.id || a.company_id == myCompanyInfo.id));
                    if (company_id > 0)
                        qable.Where(a => a.company_id == company_id);

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

        #region 导购薪资
        public object GetCurrentGuide(int companyId)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            object retObj = null;
            using (var db = SugarDao.GetInstance())
            {
                daoben_salary_position getInfo = db.Queryable<daoben_salary_position>()
                        .Where(a => a.category == 1 && a.company_id_parent == myCompanyInfo.id && a.effect_status == 1
                        && (a.company_id == companyId || a.company_id == 0)).SingleOrDefault(); //company_id == 0 表示所有分公司
                if (getInfo == null)
                {
                    retObj = new
                    {
                        getInfo = getInfo,
                    };
                    return retObj;
                }
                daoben_salary_position_guide infoMain = db.Queryable<daoben_salary_position_guide>()
                            .Where(a => a.salary_position_id == getInfo.id).SingleOrDefault();
                List<daoben_salary_position_guide_sub> infoList = db.Queryable<daoben_salary_position_guide_sub>()
                            .Where(a => a.main_id == infoMain.id).ToList();
                daoben_salary_position_traffic trafficInfo = db.Queryable<daoben_salary_position_traffic>()
                    .SingleOrDefault(a => a.salary_position_id == infoMain.id);
                retObj = new
                {
                    getInfo = getInfo,
                    infoMain = infoMain,
                    infoList = infoList,
                    trafficInfo = trafficInfo
                };
                return retObj;
            }
        }
        public object GetGuide(string id)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            using (var db = SugarDao.GetInstance())
            {
                daoben_salary_position getInfo = db.Queryable<daoben_salary_position>().InSingle(id);
                List<daoben_salary_position_approve> approveList = db.Queryable<daoben_salary_position_approve>()
                            .Where(a => a.salary_position_id == id).ToList();
                daoben_salary_position_guide infoMain = db.Queryable<daoben_salary_position_guide>()
                            .Where(a => a.salary_position_id == id).SingleOrDefault();
                List<daoben_salary_position_guide_sub> infoList = new List<daoben_salary_position_guide_sub>();
                if (infoMain != null)
                    infoList = db.Queryable<daoben_salary_position_guide_sub>()
                                .Where(a => a.main_id == infoMain.id).ToList();
                daoben_salary_position_traffic trafficInfo = db.Queryable<daoben_salary_position_traffic>()
                    .SingleOrDefault(a => a.salary_position_id == id);
                object retObj = new
                {
                    getInfo = getInfo,
                    approveList = approveList,
                    infoMain = infoMain,
                    infoList = infoList,
                    trafficInfo = trafficInfo
                };
                return retObj;
            }
        }
        public string SetGuide(daoben_salary_position settingInfo, daoben_salary_position_guide infoMain,
                    List<daoben_salary_position_guide_sub> infoList, daoben_salary_position_traffic trafficInfo, List<IntIdNamePair> companyList)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (infoList == null || infoList.Count < 0 || infoMain == null || settingInfo == null)
                return "信息错误，操作失败!";
            settingInfo.id = Common.GuId();
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            settingInfo.creator_job_history_id = LoginInfo.jobHistoryId;
            settingInfo.creator_name = LoginInfo.empName;
            settingInfo.create_time = DateTime.Now;
            if (settingInfo.is_template == 0 && settingInfo.company_id != myCompanyInfo.id)
                return "信息错误：没有指定事业部ID";
            else if (settingInfo.is_template == 3 && companyList == null)
                return "信息错误：非公版需要指定分公司";
            else if (settingInfo.company_id != myCompanyInfo.id)
                return "信息错误：设为公版需要指定事业部ID";
            settingInfo.approve_status = 0;
            settingInfo.effect_status = -2;
            settingInfo.category = 1;
            settingInfo.category_display = "导购薪资";

            if (settingInfo.effect_now)
                settingInfo.effect_date = now;
            List<daoben_salary_position> totalPosition = new List<daoben_salary_position>();
            List<daoben_salary_position_guide> totalGuide = new List<daoben_salary_position_guide>();
            List<daoben_salary_position_guide_sub> totalGuideSub = new List<daoben_salary_position_guide_sub>();
            List<daoben_salary_position_traffic> totalTraffic = new List<daoben_salary_position_traffic>();
            string errHint = "";
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    if (settingInfo.is_template == 1 || settingInfo.is_template == 2)
                    {
                        companyList = db.Queryable<daoben_org_company>()
                              .Where(a => a.parent_id == myCompanyInfo.id)
                              .Select<IntIdNamePair>("id as id ,name as  name").ToList();
                        if (settingInfo.is_template == 2)
                        {
                            List<int> delIdList = db.Queryable<daoben_salary_position>()
                                .Where(a => a.company_id_parent == myCompanyInfo.id && a.effect_status == 1)
                                .Where(a => a.category == 1)
                                .Select<int>("company_id")
                                .ToList();
                            companyList = companyList.Where(t => delIdList.Contains(t.id) == false).ToList();
                        }
                    }
                    for (int i = 0; i < companyList.Count; i++)
                    {

                        int companyId = companyList[i].id;
                        string companyName = companyList[i].name;
                        List<daoben_org_position> positionList = db.Queryable<daoben_org_position>()
                            .Where(a => a.company_id == companyId && a.position_type == ConstData.POSITION_GUIDE).ToList();
                        if (positionList == null || positionList.Count < 1)
                        {
                            errHint += "【" + companyName + "】";
                            continue;
                        }
                        #region
                        foreach (var pos in positionList)
                        {
                            string guid = Common.GuId();
                            daoben_salary_position tempPosSalary = new daoben_salary_position()
                            {
                                id = guid,
                                effect_date = settingInfo.effect_date,
                                effect_now = settingInfo.effect_now,
                                effect_status = settingInfo.effect_status,
                                file_name = settingInfo.file_name,
                                position_id = pos.id,
                                position_name = pos.name,
                                dept_id = pos.dept_id,
                                dept_name = pos.dept_name,
                                company_id = pos.company_id,
                                company_id_parent = pos.company_id_parent,
                                company_name = pos.company_name,
                                category = 1,
                                category_display = "导购薪资",
                                approve_status = 0,
                                create_time = now,
                                creator_job_history_id = LoginInfo.jobHistoryId,
                                creator_name = LoginInfo.empName,
                                creator_position_name = myPositionInfo.name,
                                is_template = settingInfo.is_template,
                            };
                            daoben_salary_position_traffic tempTraffic = new daoben_salary_position_traffic()
                            {
                                salary_position_id = guid,
                                reset_all = trafficInfo.reset_all,
                                traffic_subsidy = trafficInfo.traffic_subsidy
                            };
                            totalTraffic.Add(tempTraffic);
                            totalPosition.Add(tempPosSalary);
                            daoben_salary_position_guide tempGuide = new daoben_salary_position_guide()
                            {
                                id = Common.GuId(),
                                salary_position_id = guid,
                                guide_annualbonus_type = infoMain.guide_annualbonus_type,
                                guide_base_type = infoMain.guide_base_type,
                                guide_salary_base = infoMain.guide_salary_base,
                                reset_all_annualbonus = infoMain.reset_all_annualbonus,
                                reset_all_base = infoMain.reset_all_base,
                                standard_salary = infoMain.standard_salary,
                                standard_commission = infoMain.standard_commission,
                                increase_award_status = infoMain.increase_award_status,
                                increase_month = infoMain.increase_month,
                                increase_protect = infoMain.increase_protect,
                                increase_salary = infoMain.increase_salary,
                                increase_commission = infoMain.increase_commission,
                                increase_effect_time = infoMain.increase_effect_time,
                            };
                            totalGuide.Add(tempGuide);
                            List<daoben_salary_position_guide_sub> tempSubList = new List<daoben_salary_position_guide_sub>();
                            foreach (var a in infoList)
                            {
                                daoben_salary_position_guide_sub tempSub = new daoben_salary_position_guide_sub()
                                {
                                    main_id = tempGuide.id,
                                    category = a.category,
                                    amount = a.amount,
                                    level = a.level,
                                    target_min = a.target_min,
                                    target_max = a.target_max,
                                };
                                tempSubList.Add(tempSub);
                            }
                            totalGuideSub.AddRange(tempSubList);
                        }
                        #endregion
                    }
                    if (errHint != "")
                    {
                        errHint += "不存在导购岗位，请先添加职位后再设置岗位薪资";
                        return errHint;
                    }



                    db.CommandTimeOut = 60;
                    db.BeginTran();

                    //object upObj = new
                    //{
                    //    effect_status = 2,
                    //};
                    List<int> companyIdList = totalPosition.Select(t => t.company_id).ToList();
                    //db.Update<daoben_salary_position>(upObj, a => companyIdList.Contains(a.company_id) && a.category == 1);

                    if (infoMain.reset_all_base || infoMain.reset_all_annualbonus)
                    {
                        //找出符合筛选条件的所有 daoben_salary_emp_general id
                        List<int> generalIdList = db.Queryable<daoben_salary_emp_general>()
                                .JoinTable<daoben_salary_emp>((a, b) => b.id == a.main_id)
                                .JoinTable<daoben_salary_emp, daoben_hr_emp_job>((a, b, c) => b.emp_id == c.id)
                                .Where<daoben_salary_emp, daoben_hr_emp_job>((a, b, c) => b.effect_status == 1 && companyIdList.Contains(c.company_id))
                                .Where<daoben_salary_emp, daoben_hr_emp_job>((a, b, c) => c.position_type == ConstData.POSITION_GUIDE)
                                .Select<int>("a.id")
                                .ToList();
                        if (infoMain.reset_all_base)
                        {
                            object resetBaseObj = new
                            {
                                guide_base_type = infoMain.guide_base_type,
                                guide_salary_base = infoMain.guide_salary_base,
                                guide_standard_commission = infoMain.standard_commission,
                                guide_standard_salary = infoMain.standard_salary,
                            };
                            db.Update<daoben_salary_emp_general>(resetBaseObj, t => generalIdList.Contains(t.id));
                        }
                        if (infoMain.reset_all_annualbonus)
                        {
                            object resetAnnualObj = new
                            {
                                guide_annualbonus_type = infoMain.guide_annualbonus_type
                            };
                            db.Update<daoben_salary_emp_general>(resetAnnualObj, t => generalIdList.Contains(t.id));
                        }
                    }
                    if (totalPosition.Count == 1)
                    {
                        db.Insert(totalPosition[0]);
                        db.Insert(totalGuide[0]);
                        db.Insert(totalTraffic[0]);
                    }
                    else
                    {
                        db.InsertRange(totalPosition);
                        db.InsertRange(totalGuide);
                        db.InsertRange(totalTraffic);
                    }
                    if (totalGuideSub != null)
                    {
                        db.DisableInsertColumns = new string[] { "id" };
                        if (totalGuideSub.Count == 1)
                            db.Insert(totalGuideSub);
                        else if (totalGuideSub.Count > 25)
                            db.SqlBulkCopy(totalGuideSub);
                        else
                            db.InsertRange(totalGuideSub);
                    }
                    db.CommitTran();
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return "系统出错：" + ex.Message;
                }
                //12.25临时审批
                List<string> idList = totalPosition.Select(t => t.id).ToList();
                approveTemp(idList);
                return "success";
            }
        }
        #endregion

        #region 业务薪资
        //public object GetCurrentSales(int companyId)
        //{
        //    OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
        //    if (LoginInfo == null)
        //        throw new Exception("用户登陆过期，请重新登录");
        //    CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
        //    object retObj = null;
        //    using (var db = SugarDao.GetInstance())
        //    {
        //        daoben_salary_position getInfo = db.Queryable<daoben_salary_position>()
        //                .Where(a => a.category == 2 && a.company_id_parent == myCompanyInfo.id && a.effect_status == 1
        //                && (a.company_id == companyId || a.company_id == 0)).SingleOrDefault(); //company_id == 0 表示所有分公司
        //        if (getInfo == null)
        //        {
        //            retObj = new
        //            {
        //                getInfo = getInfo,
        //            };
        //            return retObj;
        //        }
        //        List<daoben_salary_position_sales> infoList = db.Queryable<daoben_salary_position_sales>()
        //                    .Where(a => a.salary_position_id == getInfo.id).ToList();
        //        daoben_salary_position_traffic trafficInfo = db.Queryable<daoben_salary_position_traffic>()
        //            .Where(a => a.salary_position_id == getInfo.id).SingleOrDefault();
        //        retObj = new
        //        {
        //            getInfo = getInfo,
        //            infoList = infoList,
        //            trafficInfo = trafficInfo,
        //        };
        //        return retObj;
        //    }
        //}
        public object GetSales(string id)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            using (var db = SugarDao.GetInstance())
            {
                daoben_salary_position getInfo = db.Queryable<daoben_salary_position>().SingleOrDefault(a => a.id == id);
                if (getInfo == null)
                    return "信息不存在";
                List<daoben_salary_position_approve> approveList = db.Queryable<daoben_salary_position_approve>()
                            .Where(a => a.salary_position_id == id).ToList();
                List<daoben_salary_position_sales> infoList = db.Queryable<daoben_salary_position_sales>()
                            .Where(a => a.salary_position_id == id).ToList();
                List<daoben_salary_position_sales_sub> subList = db.Queryable<daoben_salary_position_sales_sub>()
                    .JoinTable<daoben_salary_position_sales>((a, b) => a.main_id == b.id)
                    .Where<daoben_salary_position_sales>((a, b) => b.salary_position_id == id)
                    .Select<daoben_salary_position_sales_sub>("a.*").ToList();
                daoben_salary_position_traffic trafficInfo = db.Queryable<daoben_salary_position_traffic>()
                   .Where(a => a.salary_position_id == getInfo.id).SingleOrDefault();
                object retObj = new
                {
                    getInfo = getInfo,
                    approveList = approveList,
                    infoList = infoList,
                    trafficInfo = trafficInfo,
                    subList = subList,
                };
                return retObj;
            }
        }

        public string SetSales(daoben_salary_position settingInfo, daoben_salary_position_sales salesInfo,
             List<daoben_salary_position_sales_sub> subList, daoben_salary_position_traffic trafficInfo, List<IntIdNamePair> companyList)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (subList == null || subList.Count < 0 || settingInfo == null || salesInfo == null)
                return "信息错误，操作失败!";
            if(settingInfo.category == 21)
                settingInfo.category_display = "业务经理KPI";
            else if(settingInfo.category == 22)
                settingInfo.category_display = "业务员KPI";
            else 
                return "信息错误，类型不正确!";

            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            if (settingInfo.is_template == 0 && settingInfo.company_id != myCompanyInfo.id)
                return "信息错误：没有指定事业部ID";
            else if (settingInfo.is_template == 3 && companyList == null)
                return "信息错误：非公版需要指定分公司";
            else if (settingInfo.company_id != myCompanyInfo.id)
                return "信息错误：设为公版需要指定事业部ID";
            settingInfo.creator_job_history_id = LoginInfo.jobHistoryId;
            settingInfo.creator_name = LoginInfo.empName;
            settingInfo.create_time = DateTime.Now;
            settingInfo.approve_status = 0;
            settingInfo.effect_status = -2;

            List<daoben_salary_position> totalPosition = new List<daoben_salary_position>();
            List<daoben_salary_position_sales> totalSales = new List<daoben_salary_position_sales>();
            List<daoben_salary_position_sales_sub> totalSalesSub = new List<daoben_salary_position_sales_sub>();
            List<daoben_salary_position_traffic> totalTraffic = new List<daoben_salary_position_traffic>();
            //List<daoben_salary_position_sales_product> totalProduct = new List<daoben_salary_position_sales_product>();
            using (var db = SugarDao.GetInstance())
            {
                if (settingInfo.is_template == 1 || settingInfo.is_template == 2)
                {
                    companyList = db.Queryable<daoben_org_company>()
                          .Where(a => a.parent_id == myCompanyInfo.id)
                          .Select<IntIdNamePair>("id as id ,name as  name").ToList();
                    if (settingInfo.is_template == 2)
                    {
                        List<int> delIdList = db.Queryable<daoben_salary_position>()
                            .Where(a => a.company_id_parent == myCompanyInfo.id && a.effect_status == 1)
                            .Where(a => a.category == settingInfo.category)
                            .Select<int>("company_id")
                            .ToList();
                        companyList = companyList.Where(t => delIdList.Contains(t.id) == false).ToList();
                    }
                }
                string errHint = "";
                try
                {
                    for (int i = 0; i < companyList.Count; i++)
                    {
                        int companyId = companyList[i].id;
                        string companyName = companyList[i].name;
                        List<daoben_org_position> positionList = db.Queryable<daoben_org_position>()
                            .Where(a => a.company_id == companyId && a.position_type == settingInfo.category).ToList();// category 与 jobInfo.position_type 相对应（仅业务员/业务经理）
                        if (positionList == null || positionList.Count < 1)
                        {
                            errHint += "【" + companyName + "】";
                            continue;
                        }
                        #region 
                        foreach (var pos in positionList)
                        {
                            string guid = Common.GuId();
                            daoben_salary_position tempPosSalary = new daoben_salary_position()
                            {
                                id = guid,
                                effect_date = settingInfo.effect_date,
                                effect_now = settingInfo.effect_now,
                                effect_status = settingInfo.effect_status,
                                file_name = settingInfo.file_name,
                                position_id = pos.id,
                                position_name = pos.name,
                                dept_id = pos.dept_id,
                                dept_name = pos.dept_name,
                                company_id = pos.company_id,
                                company_id_parent = pos.company_id_parent,
                                company_name = pos.company_name,
                                category = settingInfo.category,
                                category_display = settingInfo.category_display,
                                approve_status = 0,
                                create_time = now,
                                creator_job_history_id = LoginInfo.jobHistoryId,
                                creator_name = LoginInfo.empName,
                                creator_position_name = myPositionInfo.name,
                                is_template = settingInfo.is_template,
                            };
                            daoben_salary_position_traffic tempTraffic = new daoben_salary_position_traffic()
                            {
                                salary_position_id = guid,
                                traffic_subsidy = trafficInfo.traffic_subsidy,
                                reset_all = trafficInfo.reset_all,
                            };
                            totalTraffic.Add(tempTraffic);

                            totalPosition.Add(tempPosSalary);
                            daoben_salary_position_sales tempSales = new daoben_salary_position_sales()
                            {
                                id = Common.GuId(),
                                salary_position_id = guid,
                                note = salesInfo.note,
                                activity_status = salesInfo.activity_status,
                                activity_target = salesInfo.activity_target,
                                approve_status = salesInfo.approve_status,
                                buyout_rebate_mode = salesInfo.buyout_rebate_mode,
                                normal_rebate_mode = salesInfo.normal_rebate_mode,
                                total_sale_amount = salesInfo.total_sale_amount,
                                target_content = salesInfo.target_content,
                                target_mode = salesInfo.target_mode,
                                total_penalty = salesInfo.total_penalty,
                                total_reward = salesInfo.total_reward,
                                total_sale_count = salesInfo.total_sale_count,
                                reset_all_perf = salesInfo.reset_all_perf,
                            };
                            //if (salesInfo.target_mode == 4)
                            //{
                            //    List<daoben_salary_position_sales_product> tempProList = new List<daoben_salary_position_sales_product>();
                            //    foreach (var a in productList)
                            //    {
                            //        daoben_salary_position_sales_product tempPro = new daoben_salary_position_sales_product()
                            //        {
                            //            color = a.color,
                            //            main_id = tempSales.id,
                            //            model = a.model,
                            //            price_wholesale = a.price_wholesale,
                            //            reward = a.reward,
                            //            total_amount = a.total_amount,
                            //            total_count = a.total_count,
                            //        };
                            //        tempProList.Add(tempPro);
                            //    }
                            //    totalProduct.AddRange(tempProList);
                            //}
                            totalSales.Add(tempSales);
                            List<daoben_salary_position_sales_sub> tempSubList = new List<daoben_salary_position_sales_sub>();
                            foreach (var a in subList)
                            {
                                daoben_salary_position_sales_sub tempSub = new daoben_salary_position_sales_sub()
                                {
                                    main_id = tempSales.id,
                                    buyout_rebate = a.buyout_rebate,
                                    sale_rebate = a.sale_rebate,
                                    target_min = a.target_min,
                                    target_max = a.target_max,
                                };
                                tempSubList.Add(tempSub);
                            }
                            totalSalesSub.AddRange(tempSubList);
                        }
                        #endregion
                    }
                    if (errHint != "")
                    {
                        errHint += "不存在业务岗位，请先添加职位后再设置岗位薪资";
                        return errHint;
                    }

                    object upObj = new
                    {
                        effect_status = 2,
                    };
                    List<int> companyIdList = totalPosition.Select(t => t.company_id).ToList();
                    db.Update<daoben_salary_position>(upObj, a => companyIdList.Contains(a.company_id) && a.category == settingInfo.category);
                    List<int> posIdList = totalPosition.Select(t => t.position_id).ToList();


                    List<string> empIdList = null;
                    List<string> origEmpIdList = null;
                    List<string> origEmpSalesIdList = null;
                    List<daoben_salary_emp_sales_sub> totalEmpSubList = null;
                    object resetObj = null;
                    if (salesInfo.reset_all_perf)
                    {
                        //找到涉及到的员工
                        //部分更新 不能删除
                        List<daoben_hr_emp_job> empList = db.Queryable<daoben_hr_emp_job>()
                            .Where(a => companyIdList.Contains(a.company_id))
                            .Where(a => a.position_type == ConstData.POSITION_SALES || a.position_type == ConstData.POSITION_SALESMANAGER)
                            .ToList();
                        empIdList = empList.Select(t => t.id).ToList();
                        origEmpIdList = db.Queryable<daoben_salary_emp>()
                            .Where(a => empIdList.Contains(a.emp_id))
                            .Where(a => a.effect_status == 1)
                            .Select<string>("id")
                            .ToList();
                        origEmpSalesIdList = db.Queryable<daoben_salary_emp_sales>()
                            .Where(a => origEmpIdList.Contains(a.salary_position_id))
                            .Select<string>("id")
                            .ToList();
                        totalEmpSubList = new List<daoben_salary_emp_sales_sub>();
                        //插入新的配置信息
                        resetObj = new
                        {
                            target_content = salesInfo.target_content,
                            target_mode = salesInfo.target_mode,
                            normal_rebate_mode = salesInfo.normal_rebate_mode,
                            buyout_rebate_mode = salesInfo.buyout_rebate_mode,
                            activity_target = salesInfo.activity_target,
                            total_sale_count = salesInfo.total_sale_count,
                            total_sale_amount = salesInfo.total_sale_amount,
                            total_penalty = salesInfo.total_penalty,
                            total_reward = salesInfo.total_reward,
                            //配置信息 更新到员工设置表中
                        };
                        foreach (var empSalesId in origEmpSalesIdList)
                        {
                            foreach (var tempSub in subList)
                            {
                                daoben_salary_emp_sales_sub tempSubEmp = new daoben_salary_emp_sales_sub
                                {
                                    main_id = empSalesId,
                                    buyout_rebate = tempSub.buyout_rebate,
                                    sale_rebate = tempSub.sale_rebate,
                                    target_max = tempSub.target_max,
                                    target_min = tempSub.target_min,
                                };
                                totalEmpSubList.Add(tempSubEmp);
                            }
                        }
                    }



                    db.CommandTimeOut = 60;
                    db.BeginTran();

                    if (salesInfo.reset_all_perf)
                    {
                        db.Update<daoben_salary_emp_sales>(resetObj, t => origEmpSalesIdList.Contains(t.id));
                        db.Delete<daoben_salary_emp_sales_sub>(t => origEmpSalesIdList.Contains(t.main_id));
                        if (totalEmpSubList != null && totalEmpSubList.Count() > 25)
                            db.SqlBulkCopy(totalEmpSubList);
                        else if (totalEmpSubList != null && totalEmpSubList.Count() > 0)
                            db.InsertRange(totalEmpSubList);
                    }

                    if (trafficInfo.reset_all)
                    {
                        //找出符合筛选条件的所有 daoben_salary_emp_sales id
                        List<string> generalIdList = db.Queryable<daoben_salary_emp_sales>()
                            .JoinTable<daoben_salary_emp>((a, b) => b.id == a.salary_position_id)
                            .JoinTable<daoben_salary_emp, daoben_hr_emp_job>((a, b, c) => b.emp_id == c.id)
                            .Where<daoben_salary_emp, daoben_hr_emp_job>((a, b, c) => b.effect_status == 1 && companyIdList.Contains(c.company_id))
                            .Where<daoben_salary_emp, daoben_hr_emp_job>((a, b, c) => c.position_type == ConstData.POSITION_SALES || c.position_type == ConstData.POSITION_SALESMANAGER)
                            .Select<string>("a.id")
                            .ToList();
                        object resetObj2 = new
                        {
                            traffic_subsidy = trafficInfo.traffic_subsidy,
                        };
                        //更新 交通补贴
                        db.Update<daoben_salary_emp_sales>(resetObj2, t => generalIdList.Contains(t.id));
                    }

                    if (totalPosition != null && totalPosition.Count() > 25)
                        db.SqlBulkCopy(totalPosition);
                    else if (totalPosition != null && totalPosition.Count() > 0)
                        db.InsertRange(totalPosition);
                    if (totalSales != null && totalSales.Count() > 25)
                        db.SqlBulkCopy(totalSales);
                    else if (totalSales != null && totalSales.Count() > 0)
                        db.InsertRange(totalSales);
                    db.DisableInsertColumns = new string[] { "id" };
                    if (totalSalesSub != null && totalSalesSub.Count() > 25)
                        db.SqlBulkCopy(totalSalesSub);
                    else if (totalSalesSub != null && totalSalesSub.Count() > 0)
                        db.InsertRange(totalSalesSub);
                    if (totalTraffic != null && totalTraffic.Count() > 25)
                        db.SqlBulkCopy(totalTraffic);
                    else if (totalTraffic != null && totalTraffic.Count() > 0)
                        db.InsertRange(totalTraffic);
                    //if (totalProduct != null && totalProduct.Count() > 25)
                    //    db.SqlBulkCopy(totalProduct);
                    //else if (totalProduct != null && totalProduct.Count() > 0)
                    //    db.InsertRange(totalProduct);
                    db.CommitTran();
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return "系统出错：" + ex.Message;
                }
                //12.25临时置为已审批
                List<string> idList = totalPosition.Select(t => t.id).ToList();
                approveTemp(idList);
                return "success";
            }
        }
        #endregion

        #region 培训师薪资
        public object GetCurrentTrainer(int companyId)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            object retObj = null;
            using (var db = SugarDao.GetInstance())
            {
                daoben_salary_position getInfo = db.Queryable<daoben_salary_position>()
                        .Where(a => a.category == 11 && a.company_id_parent == myCompanyInfo.id && a.effect_status == 1
                        && a.company_id == companyId).SingleOrDefault();
                if (getInfo == null)
                {
                    retObj = new
                    {
                        getInfo = getInfo,
                    };
                    return retObj;
                }
                daoben_salary_position_trainer infoMain = db.Queryable<daoben_salary_position_trainer>()
                            .Where(a => a.salary_position_id == getInfo.id).SingleOrDefault();
                daoben_salary_position_traffic trafficInfo = db.Queryable<daoben_salary_position_traffic>()
                            .Where(t => t.salary_position_id == getInfo.id).SingleOrDefault();
                retObj = new
                {
                    getInfo = getInfo,
                    infoMain = infoMain,
                    trafficInfo = trafficInfo
                };
                return retObj;
            }
        }
        public object GetTrainer(string id)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            using (var db = SugarDao.GetInstance())
            {
                daoben_salary_position getInfo = db.Queryable<daoben_salary_position>().InSingle(id);
                List<daoben_salary_position_approve> approveList = db.Queryable<daoben_salary_position_approve>()
                            .Where(a => a.salary_position_id == id).ToList();
                daoben_salary_position_trainer infoMain = db.Queryable<daoben_salary_position_trainer>()
                            .Where(a => a.salary_position_id == id).SingleOrDefault();
                daoben_salary_position_traffic trafficInfo = db.Queryable<daoben_salary_position_traffic>()
                            .Where(t => t.salary_position_id == getInfo.id).SingleOrDefault();
                object retObj = new
                {
                    getInfo = getInfo,
                    approveList = approveList,
                    infoMain = infoMain,
                    trafficInfo = trafficInfo
                };
                return retObj;
            }
        }

        public string SetTrainer(daoben_salary_position settingInfo, daoben_salary_position_trainer infoMain,
                     daoben_salary_position_traffic trafficInfo, List<IntIdNamePair> companyList)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (infoMain == null || settingInfo == null)
                return "信息错误，操作失败!";
            settingInfo.id = Common.GuId();
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            settingInfo.creator_job_history_id = LoginInfo.jobHistoryId;
            settingInfo.creator_name = LoginInfo.empName;
            settingInfo.create_time = DateTime.Now;
            if (settingInfo.is_template == 0 && settingInfo.company_id != myCompanyInfo.id)
                return "信息错误：没有指定事业部ID";
            else if (settingInfo.is_template == 3 && companyList == null)
                return "信息错误：非公版需要指定分公司";
            else if (settingInfo.company_id != myCompanyInfo.id)
                return "信息错误：设为公版需要指定事业部ID";
            settingInfo.approve_status = 0;
            settingInfo.effect_status = -2;
            settingInfo.category = 11;
            settingInfo.category_display = "培训薪资";

            if (settingInfo.effect_now)
                settingInfo.effect_date = now;
            List<daoben_salary_position> totalPosition = new List<daoben_salary_position>();
            List<daoben_salary_position_trainer> totalTrainer = new List<daoben_salary_position_trainer>();
            List<daoben_salary_position_traffic> totalTraffic = new List<daoben_salary_position_traffic>();
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    if (settingInfo.is_template == 1 || settingInfo.is_template == 2)
                    {
                        companyList = db.Queryable<daoben_org_company>()
                              .Where(a => a.parent_id == myCompanyInfo.id)
                              .Select<IntIdNamePair>("id as id ,name as  name").ToList();
                        if (settingInfo.is_template == 2)
                        {
                            List<int> delIdList = db.Queryable<daoben_salary_position>()
                                .Where(a => a.company_id_parent == myCompanyInfo.id && a.effect_status == 1)
                                .Where(a => a.category == 1)
                                .Select<int>("company_id")
                                .ToList();
                            companyList = companyList.Where(t => delIdList.Contains(t.id) == false).ToList();
                        }
                    }
                    string errHint = "";
                    for (int i = 0; i < companyList.Count; i++)
                    {
                        int companyId = companyList[i].id;
                        string companyName = companyList[i].name;
                        List<daoben_org_position> positionList = db.Queryable<daoben_org_position>()
                            .Where(a => a.company_id == companyId && a.position_type == ConstData.POSITION_TRAINER).ToList();
                        if (positionList == null || positionList.Count < 1)
                        {
                            errHint += "【" + companyName + "】";
                            continue;
                        }
                        foreach (var pos in positionList)
                        {
                            string guid = Common.GuId();
                            daoben_salary_position tempPosSalary = new daoben_salary_position()
                            {
                                id = guid,
                                effect_date = settingInfo.effect_date,
                                effect_now = settingInfo.effect_now,
                                effect_status = settingInfo.effect_status,
                                file_name = settingInfo.file_name,
                                position_id = pos.id,
                                position_name = pos.name,
                                dept_id = pos.dept_id,
                                dept_name = pos.dept_name,
                                company_id = pos.company_id,
                                company_id_parent = pos.company_id_parent,
                                company_name = pos.company_name,
                                category = 11,
                                category_display = "培训薪资",
                                approve_status = 0,
                                create_time = now,
                                creator_job_history_id = LoginInfo.jobHistoryId,
                                creator_name = LoginInfo.empName,
                                creator_position_name = myPositionInfo.name,
                                is_template = settingInfo.is_template,
                            };
                            daoben_salary_position_traffic tempTraffic = new daoben_salary_position_traffic()
                            {
                                salary_position_id = guid,
                                reset_all = trafficInfo.reset_all,
                                traffic_subsidy = trafficInfo.traffic_subsidy
                            };
                            totalTraffic.Add(tempTraffic);
                            totalPosition.Add(tempPosSalary);
                            daoben_salary_position_trainer tempTrainer = new daoben_salary_position_trainer()
                            {
                                salary_position_id = guid,
                                image_efficiency_advice = infoMain.image_efficiency_advice,
                                image_efficiency_standard = infoMain.image_efficiency_standard,
                                image_fine_advice = infoMain.image_fine_advice,
                                image_fine_number = infoMain.image_fine_number,
                                image_fine_standard = infoMain.image_fine_standard,
                                manager_scoring_advice = infoMain.manager_scoring_advice,
                                manager_scoring_standard = infoMain.manager_scoring_standard,
                                product_expensive_advice = infoMain.product_expensive_advice,
                                product_expensive_assess = infoMain.product_expensive_assess,
                                product_expensive_standard = infoMain.product_expensive_standard,
                                product_train_advice = infoMain.product_train_advice,
                                product_train_assess = infoMain.product_train_assess,
                                product_train_standard = infoMain.product_train_standard,
                                shopguide_average_advice = infoMain.shopguide_average_advice,
                                shopguide_average_assess = infoMain.shopguide_average_assess,
                                shopguide_average_standard = infoMain.shopguide_average_standard,
                                shopguide_resign_advice = infoMain.shopguide_resign_advice,
                                shopguide_resign_assess = infoMain.shopguide_resign_assess,
                                shopguide_resign_standard = infoMain.shopguide_resign_standard,
                                snowball_number_advice = infoMain.snowball_number_advice,
                                snowball_number_assess = infoMain.snowball_number_assess,
                                snowball_number_standard = infoMain.snowball_number_standard,
                                snowball_ratio_advice = infoMain.snowball_ratio_advice,
                                snowball_ratio_assess = infoMain.snowball_ratio_assess,
                                snowball_ratio_standard = infoMain.snowball_ratio_standard,
                            };
                            totalTrainer.Add(tempTrainer);
                        }
                    }
                    if (errHint != "")
                    {
                        errHint += "不存在培训师岗位，请先添加职位后再设置岗位薪资";
                        return errHint;
                    }
                    db.CommandTimeOut = 60;
                    db.BeginTran();
                    object upObj = new
                    {
                        effect_status = 2,
                    };
                    List<int> companyIdList = totalPosition.Select(t => t.company_id).ToList();
                    db.Update<daoben_salary_position>(upObj, a => companyIdList.Contains(a.company_id) && a.category == 11);

                    if (trafficInfo.reset_all)
                    {
                        //找出符合筛选条件的所有 daoben_salary_emp_general id
                        List<int> generalIdList = db.Queryable<daoben_salary_emp_general>()
                            .JoinTable<daoben_salary_emp>((a, b) => b.id == a.main_id)
                            .JoinTable<daoben_salary_emp, daoben_hr_emp_job>((a, b, c) => b.emp_id == c.id)
                            .Where<daoben_salary_emp, daoben_hr_emp_job>((a, b, c) => b.effect_status == 1 && c.position_type == ConstData.POSITION_TRAINER && companyIdList.Contains(c.company_id))
                            .Select<int>("a.id")
                            .ToList();
                        object resetObj = new
                        {
                            traffic_subsidy = trafficInfo.traffic_subsidy,
                        };
                        //更新 交通补贴
                        db.Update<daoben_salary_emp_general>(resetObj, t => generalIdList.Contains(t.id));
                    }

                    if (totalPosition != null && totalPosition.Count() > 25)
                        db.SqlBulkCopy(totalPosition);
                    else if (totalPosition != null && totalPosition.Count() > 0)
                        db.InsertRange(totalPosition);
                    db.DisableInsertColumns = new string[] { "id" };
                    if (totalTrainer != null && totalTrainer.Count() > 25)
                        db.SqlBulkCopy(totalTrainer);
                    else if (totalTrainer != null && totalTrainer.Count() > 0)
                        db.InsertRange(totalTrainer);
                    if (totalTraffic != null && totalTraffic.Count() > 25)
                        db.SqlBulkCopy(totalTraffic);
                    else if (totalTraffic != null && totalTraffic.Count() > 0)
                        db.InsertRange(totalTraffic);
                    db.CommitTran();
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return "系统出错：" + ex.Message;
                }
                //12.25临时审批
                List<string> idList = totalPosition.Select(t => t.id).ToList();
                approveTemp(idList);
                return "success";
            }
        }
        #endregion

        #region 培训经理薪资
        public object GetCurrentTrainerManager(int companyId)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            object retObj = null;
            using (var db = SugarDao.GetInstance())
            {
                daoben_salary_position getInfo = db.Queryable<daoben_salary_position>()
                        .Where(a => a.category == 11 && a.company_id_parent == myCompanyInfo.id && a.effect_status == 1
                        && a.company_id == companyId).SingleOrDefault();
                if (getInfo == null)
                {
                    retObj = new
                    {
                        getInfo = getInfo,
                    };
                    return retObj;
                }
                daoben_salary_position_trainermanager infoMain = db.Queryable<daoben_salary_position_trainermanager>()
                            .Where(a => a.salary_position_id == getInfo.id).SingleOrDefault();
                daoben_salary_position_traffic trafficInfo = db.Queryable<daoben_salary_position_traffic>()
                            .Where(t => t.salary_position_id == getInfo.id).SingleOrDefault();
                retObj = new
                {
                    getInfo = getInfo,
                    infoMain = infoMain,
                    trafficInfo = trafficInfo
                };
                return retObj;
            }
        }
        public object GetTrainerManager(string id)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            using (var db = SugarDao.GetInstance())
            {
                daoben_salary_position getInfo = db.Queryable<daoben_salary_position>().InSingle(id);
                List<daoben_salary_position_approve> approveList = db.Queryable<daoben_salary_position_approve>()
                            .Where(a => a.salary_position_id == id).ToList();
                daoben_salary_position_trainermanager infoMain = db.Queryable<daoben_salary_position_trainermanager>()
                            .Where(a => a.salary_position_id == id).SingleOrDefault();
                daoben_salary_position_traffic trafficInfo = db.Queryable<daoben_salary_position_traffic>()
                            .Where(t => t.salary_position_id == getInfo.id).SingleOrDefault();
                object retObj = new
                {
                    getInfo = getInfo,
                    approveList = approveList,
                    infoMain = infoMain,
                    trafficInfo = trafficInfo
                };
                return retObj;
            }
        }

        public string SetTrainerManager(daoben_salary_position settingInfo, daoben_salary_position_trainermanager infoMain,
                    daoben_salary_position_traffic trafficInfo, List<IntIdNamePair> companyList)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (infoMain == null || settingInfo == null)
                return "信息错误，操作失败!";
            settingInfo.id = Common.GuId();
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            settingInfo.creator_job_history_id = LoginInfo.jobHistoryId;
            settingInfo.creator_name = LoginInfo.empName;
            settingInfo.create_time = DateTime.Now;
            if (settingInfo.is_template == 0 && settingInfo.company_id != myCompanyInfo.id)
                return "信息错误：没有指定事业部ID";
            else if (settingInfo.is_template == 3 && companyList == null)
                return "信息错误：非公版需要指定分公司";
            else if (settingInfo.company_id != myCompanyInfo.id)
                return "信息错误：设为公版需要指定事业部ID";
            settingInfo.approve_status = 0;
            settingInfo.effect_status = -2;
            settingInfo.category = 12;
            settingInfo.category_display = "培训经理薪资";

            if (settingInfo.effect_now)
                settingInfo.effect_date = now;
            List<daoben_salary_position> totalPosition = new List<daoben_salary_position>();
            List<daoben_salary_position_trainermanager> totalTMer = new List<daoben_salary_position_trainermanager>();
            List<daoben_salary_position_traffic> totalTraffic = new List<daoben_salary_position_traffic>();
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    if (settingInfo.is_template == 1 || settingInfo.is_template == 2)
                    {
                        companyList = db.Queryable<daoben_org_company>()
                              .Where(a => a.parent_id == myCompanyInfo.id)
                              .Select<IntIdNamePair>("id as id ,name as  name").ToList();
                        if (settingInfo.is_template == 2)
                        {
                            List<int> delIdList = db.Queryable<daoben_salary_position>()
                                .Where(a => a.company_id_parent == myCompanyInfo.id && a.effect_status == 1)
                                .Where(a => a.category == 1)
                                .Select<int>("company_id")
                                .ToList();
                            companyList = companyList.Where(t => delIdList.Contains(t.id) == false).ToList();
                        }
                    }
                    string errHint = "";
                    for (int i = 0; i < companyList.Count; i++)
                    {

                        int companyId = companyList[i].id;
                        string companyName = companyList[i].name;
                        List<daoben_org_position> positionList = db.Queryable<daoben_org_position>()
                            .Where(a => a.company_id == companyId && a.position_type == ConstData.POSITION_TRAINERMANAGER).ToList();
                        if (positionList == null || positionList.Count < 1)
                        {
                            errHint += "【" + companyName + "】";
                            continue;
                        }
                        foreach (var pos in positionList)
                        {
                            string guid = Common.GuId();
                            daoben_salary_position tempPosSalary = new daoben_salary_position()
                            {
                                id = guid,
                                effect_date = settingInfo.effect_date,
                                effect_now = settingInfo.effect_now,
                                effect_status = settingInfo.effect_status,
                                file_name = settingInfo.file_name,
                                position_id = pos.id,
                                position_name = pos.name,
                                dept_id = pos.dept_id,
                                dept_name = pos.dept_name,
                                company_id = pos.company_id,
                                company_id_parent = pos.company_id_parent,
                                company_name = pos.company_name,
                                category = 12,
                                category_display = "培训经理薪资",
                                approve_status = 0,
                                create_time = now,
                                creator_job_history_id = LoginInfo.jobHistoryId,
                                creator_name = LoginInfo.empName,
                                creator_position_name = myPositionInfo.name,
                                is_template = settingInfo.is_template,
                            };
                            daoben_salary_position_traffic tempTraffic = new daoben_salary_position_traffic()
                            {
                                salary_position_id = guid,
                                reset_all = trafficInfo.reset_all,
                                traffic_subsidy = trafficInfo.traffic_subsidy
                            };
                            totalTraffic.Add(tempTraffic);
                            totalPosition.Add(tempPosSalary);
                            daoben_salary_position_trainermanager tempTMer = new daoben_salary_position_trainermanager()
                            {
                                salary_position_id = guid,
                                average_standard_money = infoMain.average_standard_money,
                                average_standard_number = infoMain.average_standard_number,
                                product_expensive_money = infoMain.product_expensive_money,
                                product_expensive_ratio = infoMain.product_expensive_ratio,
                                resign_standard_money = infoMain.resign_standard_money,
                                resign_standard_ratio = infoMain.resign_standard_ratio,

                            };
                            totalTMer.Add(tempTMer);
                        }
                    }
                    if (errHint != "")
                    {
                        errHint += "不存在培训经理岗位，请先添加职位后再设置岗位薪资";
                        return errHint;
                    }
                    db.CommandTimeOut = 60;
                    db.BeginTran();
                    object upObj = new
                    {
                        effect_status = 2,
                    };
                    List<int> companyIdList = totalPosition.Select(t => t.company_id).ToList();
                    db.Update<daoben_salary_position>(upObj, a => companyIdList.Contains(a.company_id) && a.category == 12);

                    if (trafficInfo.reset_all)
                    {
                        //找出符合筛选条件的所有 daoben_salary_emp_general id
                        List<int> generalIdList = db.Queryable<daoben_salary_emp_general>()
                            .JoinTable<daoben_salary_emp>((a, b) => b.id == a.main_id)
                            .JoinTable<daoben_salary_emp, daoben_hr_emp_job>((a, b, c) => b.emp_id == c.id)
                            .Where<daoben_salary_emp, daoben_hr_emp_job>((a, b, c) => b.effect_status == 1 && c.position_type == ConstData.POSITION_TRAINERMANAGER && companyIdList.Contains(c.company_id))
                            .Select<int>("a.id")
                            .ToList();
                        object resetObj = new
                        {
                            traffic_subsidy = trafficInfo.traffic_subsidy,
                        };
                        //更新 交通补贴
                        db.Update<daoben_salary_emp_general>(resetObj, t => generalIdList.Contains(t.id));
                    }

                    if (totalPosition != null && totalPosition.Count() > 25)
                        db.SqlBulkCopy(totalPosition);
                    else if (totalPosition != null && totalPosition.Count() > 0)
                        db.InsertRange(totalPosition);
                    db.DisableInsertColumns = new string[] { "id" };
                    if (totalTMer != null && totalTMer.Count() > 25)
                        db.SqlBulkCopy(totalTMer);
                    else if (totalTMer != null && totalTMer.Count() > 0)
                        db.InsertRange(totalTMer);
                    if (totalTraffic != null && totalTraffic.Count() > 25)
                        db.SqlBulkCopy(totalTraffic);
                    else if (totalTraffic != null && totalTraffic.Count() > 0)
                        db.InsertRange(totalTraffic);
                    db.CommitTran();
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return "系统出错：" + ex.Message;
                }
                //12.25临时审批
                List<string> idList = totalPosition.Select(t => t.id).ToList();
                approveTemp(idList);
                return "success";
            }
        }

        #endregion

        #region 部门薪资
        public object GetCurrentDept(int deptId, int companyId)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            object retObj = null;
            using (var db = SugarDao.GetInstance())
            {
                List<daoben_salary_position_grade> adviceInfoList = new List<daoben_salary_position_grade>();
                if (companyId == 0)
                {
                    return "信息错误：指定的机构不存在";
                }
                else
                {
                    if (!db.Queryable<daoben_org_company>().Any(a => a.id == companyId))
                        return "信息错误：指定的机构不存在";
                    if (deptId == 0)
                    {
                        adviceInfoList = db.Queryable<daoben_salary_position_grade>()
                            .JoinTable<daoben_salary_position>((a, b) => a.salary_position_id == b.id)
                            .Where<daoben_salary_position>((a, b) => b.category == 4 && b.company_id == companyId && b.effect_status == 1)
                            .Select("a.*").ToList();
                    }
                    else
                    {
                        daoben_org_dept deptInfo = db.Queryable<daoben_org_dept>().SingleOrDefault(a => a.id == deptId);
                        if (deptInfo == null)
                            return new { err_msg = "系统出错：指定的部门不存在" };
                        adviceInfoList = db.Queryable<daoben_salary_position_grade>()
                            .JoinTable<daoben_salary_position>((a, b) => a.salary_position_id == b.id)
                            .Where<daoben_salary_position>((a, b) => b.category == 4 && b.company_id == companyId
                                     && b.effect_status == 1)
                            .Where(a => a.grade_category == deptInfo.grade_category)
                            .Select("a.*").ToList();
                    }
                }
                if (deptId != 0)
                {
                    daoben_org_dept deptInfo = db.Queryable<daoben_org_dept>().InSingle(deptId);
                    if (deptInfo == null)
                        return new { err_msg = "系统出错：部门信息不存在" };
                    companyId = deptInfo.company_id;
                }

                if (adviceInfoList == null || adviceInfoList.Count < 1)
                    return new { err_msg = "职等工资信息不存在，请联系财务人员。" };

                daoben_salary_position getInfo = db.Queryable<daoben_salary_position>()
                        .Where(a => a.category == 13 && a.dept_id == deptId && a.effect_status == 1).SingleOrDefault();

                if (getInfo == null)
                {
                    retObj = new
                    {
                        getInfo = getInfo,
                        adviceInfoList = adviceInfoList,
                    };
                    return retObj;
                }
                List<daoben_salary_position_dept> infoList = db.Queryable<daoben_salary_position_dept>()
                            .Where(a => a.salary_position_id == getInfo.id).ToList();
                daoben_salary_position_traffic trafficInfo = db.Queryable<daoben_salary_position_traffic>()
                        .SingleOrDefault(a => a.salary_position_id == getInfo.id);
                retObj = new
                {
                    getInfo = getInfo,
                    adviceInfoList = adviceInfoList,
                    infoList = infoList,
                    trafficInfo = trafficInfo,
                    err_msg = ""
                };
                return retObj;
            }
        }
        public object GetDept(string id)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            using (var db = SugarDao.GetInstance())
            {
                daoben_salary_position getInfo = db.Queryable<daoben_salary_position>().InSingle(id);
                List<daoben_salary_position_approve> approveList = db.Queryable<daoben_salary_position_approve>()
                            .Where(a => a.salary_position_id == id).ToList();
                List<daoben_salary_position_dept> infoList = db.Queryable<daoben_salary_position_dept>()
                            .Where(a => a.salary_position_id == id).ToList();
                daoben_salary_position_traffic trafficInfo = db.Queryable<daoben_salary_position_traffic>()
                    .SingleOrDefault(a => a.salary_position_id == getInfo.id);
                object retObj = new
                {
                    getInfo = getInfo,
                    approveList = approveList,
                    infoList = infoList,
                    trafficInfo = trafficInfo
                };
                return retObj;
            }
        }
        public string SetDept(daoben_salary_position settingInfo, List<daoben_salary_position_dept> infoList, daoben_salary_position_traffic trafficInfo)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (infoList == null || infoList.Count < 0 || settingInfo == null)
                return "信息错误，操作失败!";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            settingInfo.id = Common.GuId();
            foreach (var a in infoList)
            {
                a.salary_position_id = settingInfo.id;
            }
            if (settingInfo.effect_now)
            {
                settingInfo.effect_date = DateTime.Now;
            }
            trafficInfo.salary_position_id = settingInfo.id;
            settingInfo.creator_job_history_id = LoginInfo.jobHistoryId;
            settingInfo.creator_name = LoginInfo.empName;
            settingInfo.create_time = DateTime.Now;
            settingInfo.creator_position_name = myPositionInfo.name;
            settingInfo.approve_status = 0;
            settingInfo.effect_status = -2;
            settingInfo.category = 13;
            settingInfo.category_display = "部门薪资";
            if (settingInfo.company_id == myCompanyInfo.id)
                settingInfo.company_id_parent = myCompanyInfo.parentId;
            else
                settingInfo.company_id_parent = myCompanyInfo.id;

            using (var db = SugarDao.GetInstance())
            {

                try
                {
                    if (settingInfo.dept_id > 0)
                    {
                        daoben_org_dept deptInfo = db.Queryable<daoben_org_dept>().SingleOrDefault(a => a.id == settingInfo.dept_id);
                        if (deptInfo == null)
                            return "信息错误：指定的部门不存在";
                        settingInfo.position_name = deptInfo.name + "下属职位";
                        settingInfo.dept_name = deptInfo.name;
                        settingInfo.company_name = deptInfo.company_name;
                    }
                    else
                    {
                        daoben_org_company companyInfo = db.Queryable<daoben_org_company>().SingleOrDefault(a => a.id == settingInfo.company_id);
                        if (companyInfo == null)
                            return "信息错误：指定的机构不存在";
                        settingInfo.position_name = "无部门下属职位";
                        settingInfo.dept_name = "--无部门--";
                        settingInfo.company_name = companyInfo.name;
                    }
                    db.CommandTimeOut = 60;
                    db.BeginTran();


                    if (trafficInfo.reset_all)
                    {
                        //找出符合筛选条件的所有 daoben_salary_emp_general
                        List<int> generalIdList = db.Queryable<daoben_salary_emp_general>()
                            .JoinTable<daoben_salary_emp>((a, b) => b.id == a.main_id)
                            .JoinTable<daoben_salary_emp, daoben_hr_emp_job>((a, b, c) => b.emp_id == c.id)
                            .Where<daoben_salary_emp, daoben_hr_emp_job>((a, b, c) => b.effect_status == 1 && c.dept_id == settingInfo.dept_id)
                            .Select<int>("a.id")
                            .ToList();
                        object resetObj = new
                        {
                            traffic_subsidy = trafficInfo.traffic_subsidy,
                        };
                        //更新 交通补贴
                        db.Update<daoben_salary_emp_general>(resetObj, t => generalIdList.Contains(t.id));
                    }
                    db.Insert(settingInfo);
                    db.DisableInsertColumns = new string[] { "id" };
                    if (infoList != null && infoList.Count() > 25)
                        db.SqlBulkCopy(infoList);
                    else if (infoList != null && infoList.Count() > 0)
                        db.InsertRange(infoList);
                    db.Insert(trafficInfo);
                    db.CommitTran();
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return "系统出错：" + ex.Message;
                }
                //1.25临时置为已审批
                approveTempDept(settingInfo.id);
                return "success";
            }
        }

        #endregion

        #region 工龄工资
        public object GetCurrentSeniority()
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            object retObj = null;
            using (var db = SugarDao.GetInstance())
            {
                daoben_salary_position getInfo = db.Queryable<daoben_salary_position>()
                        .Where(a => a.category == 3 && a.company_id == myCompanyInfo.id && a.effect_status == 1).SingleOrDefault();
                if (getInfo == null)
                {
                    getInfo = new daoben_salary_position();
                    getInfo.company_id = myCompanyInfo.id;
                    getInfo.company_name = myCompanyInfo.name;
                    retObj = new
                    {
                        getInfo = getInfo,
                    };
                    return retObj;
                }
                List<daoben_salary_position_seniority> infoList = db.Queryable<daoben_salary_position_seniority>()
                        .Where(a => a.salary_position_id == getInfo.id).ToList();
                daoben_salary_position_other otherInfo = db.Queryable<daoben_salary_position_other>()
                    .SingleOrDefault(a => a.salary_position_id == getInfo.id);
                retObj = new
                {
                    getInfo = getInfo,
                    infoList = infoList,
                    otherInfo = otherInfo
                };
                return retObj;
            }
        }
        public object GetSeniority(string id)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            using (var db = SugarDao.GetInstance())
            {
                daoben_salary_position getInfo = db.Queryable<daoben_salary_position>().InSingle(id);
                List<daoben_salary_position_approve> approveList = db.Queryable<daoben_salary_position_approve>()
                            .Where(a => a.salary_position_id == id).ToList();
                List<daoben_salary_position_seniority> infoList = db.Queryable<daoben_salary_position_seniority>()
                        .Where(a => a.salary_position_id == id).ToList();
                daoben_salary_position_other otherInfo = db.Queryable<daoben_salary_position_other>()
                    .SingleOrDefault(a => a.salary_position_id == id);
                object retObj = new
                {
                    getInfo = getInfo,
                    approveList = approveList,
                    infoList = infoList,
                    otherInfo = otherInfo
                };
                return retObj;
            }
        }
        public string SetSeniority(List<daoben_salary_position_seniority> infoList, daoben_salary_position_other otherInfo,
            daoben_salary_position settingInfo, List<IntIdNamePair> companyList)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (infoList == null || infoList.Count < 0 || settingInfo == null)
                return "信息错误，操作失败!";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            if (settingInfo.is_template == 0 && settingInfo.company_id != myCompanyInfo.id)
                return "信息错误：没有指定事业部ID";
            else if (settingInfo.is_template == 3 && companyList == null)
                return "信息错误：非公版需要指定分公司";
            else if (settingInfo.company_id != myCompanyInfo.id)
                return "信息错误：设为公版需要指定事业部ID";
            settingInfo.id = Common.GuId();
            foreach (var a in infoList)
            {
                a.salary_position_id = settingInfo.id;
            }
            otherInfo.salary_position_id = settingInfo.id;
            settingInfo.creator_job_history_id = LoginInfo.jobHistoryId;
            settingInfo.creator_name = LoginInfo.empName;
            settingInfo.create_time = DateTime.Now;
            settingInfo.creator_position_name = myPositionInfo.name;
            settingInfo.approve_status = 0;
            settingInfo.effect_status = -2;
            settingInfo.category = 3;
            settingInfo.category_display = "工龄工资及其他薪资设置";
            settingInfo.position_name = "所有职位";
            settingInfo.company_id = myCompanyInfo.id;
            settingInfo.company_id_parent = myCompanyInfo.parentId;
            settingInfo.company_name = myCompanyInfo.name + "及其下属分公司";
            List<daoben_salary_position> insertMainList = new List<daoben_salary_position>();
            using (var db = SugarDao.GetInstance())
            {

                try
                {
                    List<daoben_salary_position_seniority> insertList = new List<daoben_salary_position_seniority>();
                    List<daoben_salary_position_other> totalOther = new List<daoben_salary_position_other>();
                    if (settingInfo.is_template == 0)
                    {
                        if (settingInfo.company_id != myCompanyInfo.id)
                            return "信息错误：只有事业部员工能够配置此选项";
                        settingInfo.company_name = myCompanyInfo.name;
                        settingInfo.company_id_parent = myCompanyInfo.parentId;
                        insertMainList.Add(settingInfo);
                        insertList.AddRange(infoList);
                        totalOther.Add(otherInfo);
                    }
                    else
                    {
                        if (settingInfo.is_template == 1 || settingInfo.is_template == 2)
                        {
                            companyList = db.Queryable<daoben_org_company>()
                                    .Where(a => a.parent_id == myCompanyInfo.id)
                                    .Select<IntIdNamePair>("id as id ,name as  name").ToList();
                            if (settingInfo.is_template == 2)
                            {
                                List<int> delIdList = db.Queryable<daoben_salary_position>()
                                    .Where(a => a.company_id_parent == myCompanyInfo.id && a.effect_status == 1)
                                    .Where(a => a.category == 3)
                                    .Select<int>("company_id")
                                    .ToList();
                                companyList = companyList.Where(t => delIdList.Contains(t.id) == false).ToList();
                            }
                        }

                        int companyIdParent = myCompanyInfo.id;
                        DateTime effectTime;
                        if (settingInfo.effect_now)
                            effectTime = DateTime.Now;
                        else
                            effectTime = (DateTime)settingInfo.effect_date;
                        for (int i = 0; i < companyList.Count; i++)
                        {
                            string guid = Common.GuId();
                            int companyId = companyList[i].id;
                            string companyName = companyList[i].name;

                            daoben_salary_position tmpMainInfo = new daoben_salary_position()
                            {
                                id = guid,
                                effect_date = effectTime,
                                effect_now = settingInfo.effect_now,
                                effect_status = -2,
                                file_name = settingInfo.file_name,
                                //pos,dept*4
                                company_id = companyId,
                                company_name = companyName,
                                company_id_parent = companyIdParent,
                                category = 3,
                                category_display = "工龄工资及其他薪资设置",
                                is_template = settingInfo.is_template,
                                approve_status = 0,
                                creator_job_history_id = LoginInfo.jobHistoryId,
                                creator_name = LoginInfo.empName,
                                creator_position_name = myPositionInfo.name,
                                create_time = now,
                            };
                            insertMainList.Add(tmpMainInfo);
                            daoben_salary_position_other tempOther = new daoben_salary_position_other()
                            {
                                salary_position_id = guid,
                                resign_deposit = otherInfo.resign_deposit,
                                intern_fix_salary = otherInfo.intern_fix_salary,
                                intern_salary_type = otherInfo.intern_salary_type,
                                intern_ratio_salary = otherInfo.intern_ratio_salary,
                                reset_all = otherInfo.reset_all,
                            };
                            totalOther.Add(tempOther);
                            infoList.ForEach(a =>
                            {
                                daoben_salary_position_seniority info = new daoben_salary_position_seniority()
                                {
                                    salary_position_id = guid,
                                    year_max = a.year_max,
                                    year_min = a.year_min,
                                    salary = a.salary,
                                };
                                insertList.Add(info);
                            });
                        }
                    }

                    db.CommandTimeOut = 30;
                    db.BeginTran();

                    object upObj = new
                    {
                        effect_status = 2,
                    };
                    List<int> companyIdList = insertMainList.Select(t => t.company_id).ToList();
                    db.Update<daoben_salary_position>(upObj, a => companyIdList.Contains(a.company_id) && a.category == 3);

                    if (otherInfo.reset_all)
                    {
                        //实习分为 general 和 业务 只能更新
                        //查找出需要更新的全部的 daoben_salary_emp_general 和 daoben_salary_emp_sales 的 id
                        List<int> generalIdList = db.Queryable<daoben_salary_emp_general>()
                            .JoinTable<daoben_salary_emp>((a, b) => b.id == a.main_id)
                            .JoinTable<daoben_salary_emp, daoben_hr_emp_job>((a, b, c) => b.emp_id == c.id)
                            .Where<daoben_salary_emp, daoben_hr_emp_job>((a, b, c) => companyIdList.Contains(c.company_id) && b.effect_status == 1)
                            .Select<int>("a.id").ToList();
                        List<string> empSalesIdList = db.Queryable<daoben_salary_emp_sales>()
                            .JoinTable<daoben_salary_emp>((a, b) => b.id == a.salary_position_id)
                            .JoinTable<daoben_salary_emp, daoben_hr_emp_job>((a, b, c) => b.emp_id == c.id)
                            .Where<daoben_salary_emp, daoben_hr_emp_job>((a, b, c) => companyIdList.Contains(c.company_id) && b.effect_status == 1)
                            .Select<string>("a.id").ToList();
                        object resetObj = new
                        {
                            resign_deposit = otherInfo.resign_deposit,
                            intern_fix_salary = otherInfo.intern_fix_salary,
                            intern_ratio_salary = otherInfo.intern_ratio_salary,
                            intern_salary_type = otherInfo.intern_salary_type,
                        };
                        db.Update<daoben_salary_emp_general>(resetObj, t => generalIdList.Contains(t.id));
                        db.Update<daoben_salary_emp_sales>(resetObj, t => empSalesIdList.Contains(t.id));
                    }

                    if (insertMainList != null && insertMainList.Count() > 25)
                        db.SqlBulkCopy(insertMainList);
                    else if (insertMainList != null && insertMainList.Count() > 0)
                        db.InsertRange(insertMainList);
                    if (insertList != null && insertList.Count() > 25)
                        db.SqlBulkCopy(insertList);
                    else if (insertList != null && insertList.Count() > 0)
                        db.InsertRange(insertList);
                    if (totalOther != null && totalOther.Count() > 25)
                        db.SqlBulkCopy(totalOther);
                    else if (totalOther != null && totalOther.Count() > 0)
                        db.InsertRange(totalOther);
                    db.CommitTran();
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return "系统出错：" + ex.Message;
                }
                //12.25临时置为已审批
                List<string> idList = insertMainList.Select(t => t.id).ToList();
                idList.Add(settingInfo.id);
                approveTemp(idList);
                return "success";
            }
        }
        #endregion

        #region 职等薪资
        public object GetImport(string id)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_salary_position getInfo = db.Queryable<daoben_salary_position>().InSingle(id);
                    List<daoben_salary_position_approve> approveList = db.Queryable<daoben_salary_position_approve>()
                            .Where(a => a.salary_position_id == id).ToList();
                    List<daoben_salary_position_grade> infoList = db.Queryable<daoben_salary_position_grade>()
                            .Where(a => a.salary_position_id == id).ToList();
                    object retObj = new
                    {
                        getInfo = getInfo,
                        approveList = approveList,
                        infoList = infoList
                    };
                    return retObj;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
        public string Import(List<daoben_salary_position_grade> importList, daoben_salary_position importInfo, List<IntIdNamePair> companyList)
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

            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            if (importInfo.is_template == 0 && importInfo.company_id != myCompanyInfo.id)
                return "信息错误：没有指定事业部ID";
            else if (importInfo.is_template == 3 && companyList == null)
                return "信息错误：非公版需要指定分公司";
            else if (importInfo.company_id != myCompanyInfo.id)
                return "信息错误：设为公版需要指定事业部ID";
            importInfo.creator_job_history_id = LoginInfo.jobHistoryId;
            importInfo.creator_name = LoginInfo.empName;
            importInfo.creator_position_name = myPositionInfo.name;
            importInfo.create_time = DateTime.Now;
            importInfo.approve_status = 0;
            importInfo.effect_status = -2;
            importInfo.category = 4;
            importInfo.category_display = "职等薪资导入";
            List<daoben_salary_position> insertMainList = new List<daoben_salary_position>();
            List<daoben_salary_position_grade> insertImportList = new List<daoben_salary_position_grade>();
            using (var db = SugarDao.GetInstance())
            {
                if (importInfo.is_template == 0 && importInfo.company_id == myCompanyInfo.id)
                {
                    importInfo.company_name = myCompanyInfo.name;
                    importInfo.company_id_parent = myCompanyInfo.parentId;
                    insertMainList.Add(importInfo);
                    insertImportList.AddRange(importList);
                }
                else
                {
                    if (importInfo.is_template == 1 || importInfo.is_template == 2)
                    {
                        companyList = db.Queryable<daoben_org_company>()
                                        .Where(a => a.parent_id == myCompanyInfo.id)
                                        .Select<IntIdNamePair>("id as id ,name as  name").ToList();
                    }

                    int companyIdParent = myCompanyInfo.id;
                    DateTime effectTime;
                    if (importInfo.effect_now)
                        effectTime = DateTime.Now;
                    else
                        effectTime = (DateTime)importInfo.effect_date;
                    for (int i = 0; i < companyList.Count; i++)
                    {
                        string guid = Common.GuId();
                        int companyId = companyList[i].id;
                        string companyName = companyList[i].name;

                        daoben_salary_position tmpMainInfo = new daoben_salary_position()
                        {
                            id = guid,
                            effect_date = effectTime,
                            effect_now = importInfo.effect_now,
                            effect_status = -2,
                            file_name = importInfo.file_name,
                            //pos,dept*4
                            company_id = companyId,
                            company_name = companyName,
                            company_id_parent = companyIdParent,
                            category = 4,
                            category_display = "职等薪资导入",
                            is_template = 3,
                            approve_status = 0,
                            creator_job_history_id = LoginInfo.jobHistoryId,
                            creator_name = LoginInfo.empName,
                            creator_position_name = myPositionInfo.name,
                            create_time = now,
                        };
                        insertMainList.Add(tmpMainInfo);
                        importList.ForEach(a =>
                        {
                            daoben_salary_position_grade info = new daoben_salary_position_grade()
                            {
                                salary_position_id = guid,
                                grade = a.grade,
                                grade_level = a.grade_level,
                                grade_level_display = a.grade_level_display,
                                grade_category = a.grade_category,
                                grade_category_display = a.grade_category_display,
                                position_display = a.position_display,
                                base_salary = a.base_salary,
                                position_salary = a.position_salary,
                                house_subsidy = a.house_subsidy,
                                attendance_reward = a.attendance_reward,
                                seniority_wage = a.seniority_wage,
                                traffic_subsidy_display = a.traffic_subsidy_display,
                                kpi_advice = a.kpi_advice,
                                total = a.total
                            };
                            insertImportList.Add(info);
                        });
                    }

                }

                db.CommandTimeOut = 30;
                try
                {
                    db.BeginTran();

                    object upObj = new
                    {
                        effect_status = 2,
                    };
                    List<int> companyIdList = insertMainList.Select(t => t.company_id).ToList();
                    db.Update<daoben_salary_position>(upObj, a => companyIdList.Contains(a.company_id) && a.category == 4);


                    if (insertMainList.Count > 25)
                        db.SqlBulkCopy(insertMainList);
                    else
                        db.InsertRange(insertMainList);
                    db.DisableInsertColumns = new string[] { "id" };
                    if (insertImportList.Count > 25)
                        db.SqlBulkCopy(insertImportList);
                    else
                        db.InsertRange(insertImportList);
                    db.CommitTran();
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return "系统出错：" + ex.Message;
                }
                List<string> idList = insertMainList.Select(a => a.id).ToList();
                //12.23 默认审批通过
                approveTemp(idList);
                return "success";
            }
        }

        #endregion
        public string Approve(daoben_salary_position_approve approveInfo)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            if (approveInfo == null || string.IsNullOrEmpty(approveInfo.salary_position_id))
                return "信息错误，操作失败!";
            approveInfo.approve_id = LoginInfo.accountId;
            approveInfo.approve_name = LoginInfo.empName;
            approveInfo.approve_position_id = myPositionInfo.id;
            approveInfo.approve_position_name = myPositionInfo.name;
            approveInfo.approve_time = DateTime.Now;
            object upObj = null;
            object inactiveObj = null;

            using (var db = SugarDao.GetInstance())
            {
                daoben_salary_position mainInfo = db.Queryable<daoben_salary_position>().InSingle(approveInfo.salary_position_id);
                if (mainInfo == null)
                    return "操作失败：薪资设置信息不存在";

                if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER && approveInfo.status > 0)
                {
                    approveInfo.status = 100; //以100作为审批完成的标志
                    if (mainInfo.effect_now || mainInfo.effect_date <= DateTime.Now)
                    {
                        upObj = new { approve_status = 100, effect_date = DateTime.Now, effect_status = 1 };
                        inactiveObj = new { effect_status = 2 }; //设置失效
                    }
                    else
                        upObj = new { approve_status = 100, effect_status = -1 };
                }
                else if (approveInfo.status > 0)
                    approveInfo.status = 1 + mainInfo.approve_status;
                else
                    approveInfo.status = 0 - 1 - mainInfo.approve_status;
                if (upObj == null)
                    upObj = new { approve_status = approveInfo.status };
                // TODO 消息通知
                db.CommandTimeOut = 30;
                try
                {
                    db.BeginTran();
                    db.DisableInsertColumns = new string[] { "id" };
                    db.Insert(approveInfo);
                    if (inactiveObj != null)
                    {
                        db.Update<daoben_salary_position>(inactiveObj, a => a.effect_status == 1
                                && a.category == mainInfo.category && a.company_id == mainInfo.company_id);
                    }
                    db.Update<daoben_salary_position>(upObj, a => a.id == approveInfo.salary_position_id);
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

        private void approveTemp(List<string> idList)
        {
            using (var db = SugarDao.GetInstance())
            {
                //object upObj = new
                //{
                //    effect_status = 1,
                //    approve_status = 100,
                //    effect_date = DateTime.Now,
                //};
                //object upObj2 = new
                //{
                //    effect_status = 1,
                //    approve_status = 100,
                //};
                //db.Update<daoben_salary_position>(upObj, a => idList.Contains(a.id) && a.effect_now == true);
                //db.Update<daoben_salary_position>(upObj2, a => idList.Contains(a.id) && a.effect_now == false);

                List<daoben_salary_position> posSalaryList = db.Queryable<daoben_salary_position>()
                    .Where(t => idList.Contains(t.id)).ToList();
                List<int> companyIdList = posSalaryList.Select(t => t.company_id).ToList();
                string firstId = idList.FirstOrDefault();
                //非空判断
                daoben_salary_position tempPos = db.Queryable<daoben_salary_position>().InSingle(firstId);
                //非空判断

                if (tempPos.effect_now || tempPos.effect_date <= DateTime.Now)
                {
                    object upObjTemp = new
                    {
                        effect_status = 2,
                    };
                    db.Update<daoben_salary_position>(upObjTemp, a => companyIdList.Contains(a.company_id) && a.category == tempPos.category);
                }
                object upObj = new
                {
                    effect_status = 1,
                    approve_status = 100,
                    effect_date = DateTime.Now.Date,
                };
                object upObj2 = new
                {
                    effect_status = 1,
                    approve_status = 100,
                };
                object upObj3 = new
                {
                    effect_status = -1,
                    approve_status = 100,
                };
                db.Update<daoben_salary_position>(upObj2, a => idList.Contains(a.id) && a.effect_date <= DateTime.Now);
                db.Update<daoben_salary_position>(upObj, a => idList.Contains(a.id) && a.effect_now == true);
                db.Update<daoben_salary_position>(upObj3, a => idList.Contains(a.id) && a.effect_date > DateTime.Now);
            }
        }


        private void approveTempDept(string id)
        {
            using (var db = SugarDao.GetInstance())
            {


                daoben_salary_position deptSalaryPos = db.Queryable<daoben_salary_position>().InSingle(id);
                //非空判断

                if (deptSalaryPos.effect_now || deptSalaryPos.effect_date <= DateTime.Now)
                {
                    object upObjTemp = new
                    {
                        effect_status = 2,
                    };
                    db.Update<daoben_salary_position>(upObjTemp, a => a.company_id == deptSalaryPos.company_id && a.dept_id == deptSalaryPos.dept_id && a.category == deptSalaryPos.category);
                }
                object upObj2 = new
                {
                    effect_status = 1,
                    approve_status = 100,
                };
                object upObj3 = new
                {
                    effect_status = -1,
                    approve_status = 100,
                };
                if (deptSalaryPos.effect_date <= DateTime.Now)
                    db.Update<daoben_salary_position>(upObj2, a => a.id == id);
                else
                    db.Update<daoben_salary_position>(upObj3, a => a.id == id);
            }
        }
        internal string Delete(string id)
        {
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_salary_position mainInfo = db.Queryable<daoben_salary_position>().SingleOrDefault(a => a.id == id);
                    if (mainInfo == null || mainInfo.approve_status != 0)
                        //审批过后，将不能撤回
                        return "撤回失败：指定的申请信息不存在或者已被审批";
                    else
                    {
                        db.CommandTimeOut = 30;
                        db.BeginTran();
                        if (mainInfo.category == 1)
                        {
                            db.Delete<daoben_salary_position_guide_sub>(a => a.main_id == id);
                            db.Delete<daoben_salary_position_guide>(a => a.salary_position_id == id);
                        }
                        else if (mainInfo.category == 2)
                            db.Delete<daoben_salary_position_sales>(a => a.salary_position_id == id);
                        else if (mainInfo.category == 3)
                            db.Delete<daoben_salary_position_seniority>(a => a.salary_position_id == id);
                        else if (mainInfo.category == 4)
                            db.Delete<daoben_salary_position_grade>(a => a.salary_position_id == id);
                        else if (mainInfo.category == 11)
                            db.Delete<daoben_salary_position_trainer>(a => a.salary_position_id == id);
                        else if (mainInfo.category == 12)
                            db.Delete<daoben_salary_position_trainermanager>(a => a.salary_position_id == id);
                        else if (mainInfo.category == 13)
                            db.Delete<daoben_salary_position_dept>(a => a.salary_position_id == id);

                        db.Delete<daoben_salary_position>(a => a.id == id);
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

    }
}
