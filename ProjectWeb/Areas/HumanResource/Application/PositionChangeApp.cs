using Base.Code;
using MySqlSugar;
using ProjectShare.Database;
using ProjectShare.Models;
using ProjectShare.Process;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectWeb.Areas.HumanResource.Application
{
    public class PositionChangeApp
    {

        /// <summary>
        /// 查看全部请假信息
        /// </summary>
        public object GetListAll(Pagination pagination, daoben_hr_position_change queryInfo, int? status, QueryTime queryTime)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "create_time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_hr_position_change>();
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (LoginInfo.roleId == ConstData.ROLE_ID_HR || LoginInfo.roleId == ConstData.ROLE_ID_HRMANAGER)
                        qable.Where(a => a.company_id == myCompanyInfo.id);
                    else
                    {
                        if (myPositionInfo.positionType == ConstData.POSITION_GM1)
                            qable.Where(a => a.company_id == myCompanyInfo.id);
                        else if (myPositionInfo.positionType == ConstData.POSITION_GM2)
                            qable.Where(a => a.company_id == myCompanyInfo.id);
                        else if (myPositionInfo.positionType == ConstData.POSITION_DEPT_M)
                            qable.Where(a => a.dept_id == myPositionInfo.deptId);
                        else qable.Where(a => a.emp_id == LoginInfo.empId);
                    }
                }

                //姓名 机构 部门 职位名 审批状态 入职时间 * 2
                if (!string.IsNullOrEmpty(queryInfo.emp_name))
                    qable.Where(a => a.emp_name.Contains(queryInfo.emp_name));
                if (!string.IsNullOrEmpty(queryInfo.work_number))
                    qable.Where(a => a.emp_name.Contains(queryInfo.work_number));
                if (queryInfo.company_id > 0)
                    qable.Where(a => a.company_id == queryInfo.company_id);
                if (queryInfo.dept_id > 0)
                    qable.Where(a => a.dept_id == queryInfo.dept_id);
                if (!string.IsNullOrEmpty(queryInfo.position_name))
                    qable.Where(a => a.position_name.Contains(queryInfo.position_name));
                if (queryTime != null)
                {
                    if (queryTime.startTime1 != null)
                        qable.Where(a => a.date_approve >= queryTime.startTime1);
                    if (queryTime.startTime2 != null)
                        qable.Where(a => a.date_approve <= queryTime.startTime2);
                }                

                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                       .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public object GetListApprove(Pagination pagination, daoben_hr_position_change queryInfo)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "create_time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_hr_position_change>();
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (LoginInfo.roleId == ConstData.ROLE_ID_HR || LoginInfo.roleId == ConstData.ROLE_ID_HRMANAGER)
                    {
                        //分公司第三步，事业部第四步，人事部普通成员第一步，分公司总经理第二步
                        qable.Where(a => (a.company_id == myCompanyInfo.id && a.approve_status == 3)
                                    || (a.company_id_parent == myCompanyInfo.id && a.approve_status == 3)
                                    || (a.position_type == ConstData.POSITION_GM2 && a.approve_status == 2)
                                    || (a.dept_id == myPositionInfo.deptId && a.approve_status == 0 && a.position_type == ConstData.POSITION_OFFICE_NORMAL));
                    }
                    else if (LoginInfo.roleId == ConstData.ROLE_ID_TRAINER)
                    {
                        qable.Where<daoben_hr_emp_job>((a, b) => b.position_type >= ConstData.POSITION_SALESMANAGER && a.emp_id == b.id && a.approve_status == 0 && a.company_id == myCompanyInfo.id);
                    }
                    else
                    {
                        if (myPositionInfo.positionType == ConstData.POSITION_GM1)
                            //事业部第三步+分公司总经理的第一步
                            qable.Where(a => (a.company_id == myCompanyInfo.id && a.approve_status == 2)
                                        || (a.company_id_parent == myCompanyInfo.id && a.position_type == ConstData.POSITION_GM2 && a.approve_status == 0));
                        else if (myPositionInfo.positionType == ConstData.POSITION_GM2)
                            //分公司部门经理第一步
                            qable.Where(a => a.company_id == myCompanyInfo.id && a.position_type == ConstData.POSITION_DEPT_M && a.approve_status == 0);
                        else if (myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT1)
                            //事业部部门经理第一步
                            qable.Where(a => a.company_id == myCompanyInfo.id && a.position_type == ConstData.POSITION_DEPT_M && a.approve_status == 0);
                        else if (myPositionInfo.positionType == ConstData.POSITION_DEPT_M)
                            //部门普通成员第一步
                            qable.Where(a => a.dept_id == myPositionInfo.deptId && a.position_type == ConstData.POSITION_OFFICE_NORMAL && a.approve_status == 0);
                        else return null;//下属
                    }
                }
                //姓名 机构 部门 职位名 审批状态 入职时间 * 2
                if (!string.IsNullOrEmpty(queryInfo.emp_name))
                    qable.Where(a => a.emp_name.Contains(queryInfo.emp_name));
                if (!string.IsNullOrEmpty(queryInfo.work_number))
                    qable.Where(a => a.emp_name.Contains(queryInfo.work_number));
                if (queryInfo.company_id > 0)
                    qable.Where(a => a.company_id == queryInfo.company_id);
                if (queryInfo.dept_id > 0)
                    qable.Where(a => a.dept_id == queryInfo.dept_id);
                if (!string.IsNullOrEmpty(queryInfo.position_name))
                    qable.Where(a => a.position_name.Contains(queryInfo.position_name));
                
                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                       .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public object GetMyList(Pagination pagination, daoben_hr_position_change queryInfo, int? status, QueryTime queryTime)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "create_time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            using (var db = SugarDao.GetInstance())
            {

                var qable = db.Queryable<daoben_hr_position_change>().Where(a => a.creator_job_history_id == LoginInfo.jobHistoryId);
                //queryInfo
                if (queryInfo != null)
                {
                    if (!string.IsNullOrEmpty(queryInfo.emp_name))
                        qable.Where(a => a.emp_name.Contains(queryInfo.emp_name));
                    if (queryInfo.type > 0)
                        qable.Where(a => a.type == queryInfo.type);
                    if (queryInfo.area_id > 0)
                        qable.Where(a => a.area_id == queryInfo.area_id);
                    if (queryInfo.dept_id > 0)
                        qable.Where(a => a.dept_id == queryInfo.dept_id);
                    //审批状态: 1-未审批 2-审批中 3-已审批 4-不通过
                    if (status != null)
                    {
                        if (status == 1)
                            qable.Where(a => a.approve_status == 0);
                        else if (status == 4)
                            qable.Where(a => a.approve_status < 0);
                        else if (status == 3)
                            qable.Where(a => a.approve_status == 100);
                        else if (status == 2)
                            qable.Where(a => a.approve_status > 0 && a.approve_status != 100);
                        else
                            throw new Exception("审批状态错误！");
                    }
                }
                if (queryTime != null)
                {
                    if (queryTime.startTime1 != null)
                        qable.Where(a => a.date_approve >= queryTime.startTime1);
                    if (queryTime.startTime2 != null)
                        qable.Where(a => a.date_approve <= queryTime.startTime2);
                }
                
                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
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
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            if (string.IsNullOrEmpty(id))
                return "信息错误：输入的信息为空";
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_hr_position_change mainInfo = db.Queryable<daoben_hr_position_change>().InSingle(id);
                    if (mainInfo == null)
                        return "信息错误：指定的信息不存在";
                    DateTime? entry_date = db.Queryable<daoben_hr_emp_job>().SingleOrDefault(a => a.id == mainInfo.emp_id).entry_date;
                    daoben_hr_emp_info empInfo = db.Queryable<daoben_hr_emp_info>().SingleOrDefault(a => a.id == mainInfo.emp_id);
                    List<daoben_hr_position_change_approve> approveInfoList = db.Queryable<daoben_hr_position_change_approve>().Where(a => a.main_id == id).ToList();
                    List<daoben_hr_position_change_file> fileInfoList = db.Queryable<daoben_hr_position_change_file>().Where(a => a.main_id == id).ToList();
                    List<daoben_hr_position_change_effect> effectInfoList = db.Queryable<daoben_hr_position_change_effect>().Where(a => a.main_id == id).ToList();
                    object resultObj = new
                    {
                        mainInfo = mainInfo,
                        empInfo = empInfo,
                        entry_date = entry_date,
                        approveInfoList = approveInfoList,
                        fileInfoList = fileInfoList,
                        effectInfoList = effectInfoList
                    };
                    return resultObj.ToJson();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        /// <summary>
        /// 岗位调整申请
        /// </summary>
        /// <param name="addInfo">emp_id,type,area_id_new,dept_id_new,company_id_new,change_date </param>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public string Add(daoben_hr_position_change addInfo, daoben_hr_position_change_file fileInfo)
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
                    daoben_hr_emp_job empInfo = db.Queryable<daoben_hr_emp_job>().SingleOrDefault(a => a.id == addInfo.emp_id);
                    if (empInfo == null)
                        return "信息错误：指定的员工不存在";
                    #region  新旧职位信息

                    daoben_org_position posInfo = db.Queryable<daoben_org_position>().InSingle(empInfo.position_id);
                    daoben_org_position posInfoNew = db.Queryable<daoben_org_position>().InSingle(addInfo.position_id_new);
                    daoben_org_dept_area deptInfo = db.Queryable<daoben_org_dept_area>().InSingle(empInfo.dept_id);
                    daoben_org_dept_area deptInfoNew = db.Queryable<daoben_org_dept_area>().InSingle(addInfo.dept_id_new);
                    daoben_org_dept_area areaInfo = db.Queryable<daoben_org_dept_area>().InSingle(empInfo.position_id);
                    daoben_org_dept_area areaInfoNew = db.Queryable<daoben_org_dept_area>().InSingle(addInfo.area_id_new);
                    daoben_org_company orgInfo = db.Queryable<daoben_org_company>().InSingle(empInfo.company_id);
                    daoben_org_company orgInfoNew = db.Queryable<daoben_org_company>().InSingle(addInfo.company_id_new);
                    if (posInfoNew == null)
                        return "信息错误：指定的新的职位信息不存在";
                    if (deptInfoNew == null)
                        return "信息错误：指定的新的部门信息不存在";
                    if (orgInfoNew == null)
                        return "信息错误：指定的新的机构信息不存在";
                    addInfo.grade = empInfo.grade;
                    addInfo.company_id = orgInfo.id;
                    addInfo.company_name = orgInfo.name;
                    if (orgInfo.parent_id != 0)
                        addInfo.company_id_parent = orgInfo.parent_id;
                    addInfo.company_id_new = orgInfoNew.id;
                    addInfo.company_name_new = orgInfoNew.name;
                    if (orgInfoNew.parent_id != 0)
                        addInfo.company_id_parent_new = orgInfoNew.parent_id;
                    if (areaInfo != null)
                    {
                        addInfo.area_id = areaInfo.id;
                        addInfo.area_name = areaInfo.name;
                    }
                    if (areaInfoNew != null)
                    {
                        addInfo.area_id_new = areaInfoNew.id;
                        addInfo.area_name_new = areaInfoNew.name;
                    }
                    addInfo.dept_id = deptInfo.id;
                    addInfo.dept_name = deptInfo.name;
                    addInfo.dept_id_new = deptInfoNew.id;
                    addInfo.dept_name_new = deptInfoNew.name;

                    addInfo.position_id = posInfo.id;
                    addInfo.position_name = posInfo.name;
                    addInfo.position_id_new = posInfoNew.id;
                    addInfo.position_name_new = posInfoNew.name;

                    addInfo.company_category = orgInfo.category;
                    addInfo.company_category_new = orgInfoNew.category;
                    addInfo.position_type = posInfo.position_type;
                    addInfo.position_type_new = posInfoNew.position_type;
                    #endregion

                    addInfo.work_number = empInfo.work_number;
                    addInfo.emp_name = empInfo.name;
                    addInfo.creator_job_history_id = LoginInfo.jobHistoryId;
                    addInfo.creator_name = LoginInfo.empName;
                    addInfo.create_time = DateTime.Now;


                    addInfo.id = Common.GuId();
                    addInfo.approve_status = 0;

                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    //TODO 插入文件信息 参考员工
                    if (fileInfo != null)
                        db.Insert(fileInfo);
                    db.Insert(addInfo);

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


        /// <summary>
        /// 提交审核
        /// </summary>
        /// <param name="approveInfo"> main_id,status</param>
        /// /// <param name="effectInfoList">id,date_approve</param>
        /// <param name="effectInfoList">all</param>
        /// <returns></returns> 
        public string Approve(daoben_hr_position_change_approve approveInfo, daoben_hr_position_change mainInfo, List<daoben_hr_position_change_effect> effectInfoList)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            object upObj = null;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    //申请表ID
                    daoben_hr_position_change origInfo = db.Queryable<daoben_hr_position_change>().InSingle(approveInfo.main_id);
                    if (origInfo == null)
                        return "信息错误：指定的申请信息不存在";
                    bool isOneDay = mainInfo.date_approve.ToDate() == origInfo.request_date.ToDate();//同一天
                    if (origInfo.approve_status == 0 && (mainInfo.date_approve == null || isOneDay))
                    {
                        mainInfo.date_approve = origInfo.request_date;
                        origInfo.approve_status += 1;
                    }
                    else
                    {
                        //TODO 消息通知
                    }
                    approveInfo.approve_id = LoginInfo.accountId;
                    approveInfo.approve_name = LoginInfo.empName;
                    approveInfo.approve_time = DateTime.Now;
                    approveInfo.approve_position_id = myPositionInfo.id;
                    approveInfo.approve_position_name = myPositionInfo.name;
                    //approveInfo.approve_note


                    if (LoginInfo.roleId == ConstData.ROLE_ID_HRMANAGER && approveInfo.status > 0)
                    {
                        //调岗effectInfoList TODO 定时任务
                        approveInfo.status = 100;
                    }

                    else if (approveInfo.status > 0)
                        approveInfo.status = 0 + 1 + origInfo.approve_status;
                    else
                        approveInfo.status = 0 - 1 - origInfo.approve_status;

                    if (origInfo.approve_status == 1 || origInfo.approve_status == 0)
                        upObj = new { approve_status = approveInfo.status, date_approve = mainInfo.date_approve };
                    else if (approveInfo.status == 100)
                    {

                        upObj = new
                        {
                            approve_status = 100,
                            execute_status = -1
                        };
                        //TODO 通过之后设置定时任务
                    }
                    else
                        upObj = new { approve_status = approveInfo.status };

                    db.CommandTimeOut = 30;
                    db.BeginTran();

                    db.Update<daoben_hr_position_change>(upObj, a => a.id == origInfo.id);
                    db.Insert(approveInfo);

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
        public string Confirm(daoben_hr_position_change confirmInfo)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            object upObj = null;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    //申请表ID
                    daoben_hr_position_change origInfo = db.Queryable<daoben_hr_position_change>().InSingle(confirmInfo.id);
                    if (origInfo == null || origInfo.approve_status != 1)
                        return "信息错误：指定的申请信息不存在或者无需确认";
                    confirmInfo.approve_status = 1;

                    upObj = new
                    {
                        approve_status = origInfo.approve_status + 1
                    };

                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    db.Update<daoben_hr_position_change>(upObj, a => a.id == confirmInfo.id);

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
        public string Delete(string id)
        {
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_hr_position_change mainInfo = db.Queryable<daoben_hr_position_change>().SingleOrDefault(a => a.id == id);
                    if (mainInfo == null || mainInfo.approve_status == 0)
                        //审批过后，将不能撤回
                        return "撤回失败：指定的申请信息不存在或者已被审批";
                    else
                    {
                        db.CommandTimeOut = 30;
                        db.BeginTran();

                        db.Delete<daoben_hr_position_change_file>(a => a.main_id == id);
                        db.Delete<daoben_hr_position_change>(a => a.id == id);

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
