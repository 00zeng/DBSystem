using Base.Code;
using MySqlSugar;
using ProjectShare.Database;
using ProjectShare.Models;
using ProjectShare.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectWeb.Areas.SystemManage.Application;

namespace ProjectWeb.Areas.HumanResource.Application
{
    public class GradeChangeApp
    {
        MsAccountApp msAccount = new MsAccountApp();


        public object GetListAll(Pagination pagination, daoben_hr_emp_job queryInfo,int approve_status, QueryTime queryTime)
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
                var qable = db.Queryable<daoben_hr_grade_change>();
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
                if (!string.IsNullOrEmpty(queryInfo.name))
                    qable.Where(a => a.emp_name.Contains(queryInfo.name));
                if (queryInfo.company_id > 0)
                    qable.Where(a => a.company_id == queryInfo.company_id);
                if (queryInfo.dept_id > 0)
                    qable.Where(a => a.dept_id == queryInfo.dept_id);
                if (!string.IsNullOrEmpty(queryInfo.position_name))
                    qable.Where(a => a.position_name.Contains(queryInfo.position_name));

                if (approve_status != 0)  // 0表示查找全部
                {
                    int status = approve_status;
                    if (status == 100)   // 已审批
                        qable.Where(a => a.approve_status == 100);
                    else if (status == -100)    // 审批不通过
                        qable.Where(a => a.approve_status < 0);
                    else if (status == 1)    // 审批中
                        qable.Where(a => a.approve_status > 0 && a.approve_status < 100);
                    else if (status == -1)    // 未审批
                        qable.Where(a => a.approve_status == 0);
                }                
                
                if (queryTime != null)
                {
                    if (queryTime.startTime1 != null)
                        qable.Where(a => a.date_approve >= queryTime.startTime1);
                    if (queryTime.startTime2 != null)
                    {
                        queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                        qable.Where(a => a.date_approve < queryTime.startTime2);
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

        public object GetListApprove(Pagination pagination, daoben_hr_emp_job queryInfo)
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
                var qable = db.Queryable<daoben_hr_grade_change>();
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (LoginInfo.roleId == ConstData.ROLE_ID_HR || LoginInfo.roleId == ConstData.ROLE_ID_HRMANAGER)
                    {
                        qable.Where(a => ((a.company_id == myCompanyInfo.id || a.company_id_parent == myCompanyInfo.id) && a.approve_status == 3)
                                    || (a.dept_id == myPositionInfo.deptId && a.approve_status == 0));
                    }
                    else
                    {
                        if (myPositionInfo.positionType == ConstData.POSITION_GM1)
                            qable.Where(a => a.company_id == myCompanyInfo.id && a.approve_status == 2);
                        else if (myPositionInfo.positionType == ConstData.POSITION_GM2)
                            qable.Where(a => a.company_id == myCompanyInfo.id && a.approve_status == 2);
                        else if (myPositionInfo.positionType == ConstData.POSITION_DEPT_M)
                            qable.Where(a => a.dept_id == myPositionInfo.deptId && a.approve_status == 0);
                        else return null;//下属
                    }
                }
                //姓名 机构 部门 职位名 审批状态 入职时间 * 2
                if (!string.IsNullOrEmpty(queryInfo.name))
                    qable.Where(a => a.emp_name.Contains(queryInfo.name));
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

        public object GetMyList(Pagination pagination, daoben_hr_emp_job queryInfo, int approve_status, QueryTime queryTime)
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

                var qable = db.Queryable<daoben_hr_grade_change>()
                    .Where(a => a.emp_id == LoginInfo.empId);
                if (approve_status != 0)  // 0表示查找全部
                {
                    int status = approve_status;
                    if (status == 100)   // 已审批
                        qable.Where(a => a.approve_status == 100);
                    else if (status == -100)    // 审批不通过
                        qable.Where(a => a.approve_status < 0);
                    else if (status == 1)    // 审批中
                        qable.Where(a => a.approve_status > 0 && a.approve_status < 100);
                    else if (status == -1)    // 未审批
                        qable.Where(a => a.approve_status == 0);
                }

                if (queryTime != null)
                {
                    if (queryTime.startTime1 != null)
                        qable.Where(a => a.date_approve >= queryTime.startTime1);
                    if (queryTime.startTime2 != null)
                    {
                        queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                        qable.Where(a => a.date_approve < queryTime.startTime2);
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
        /// 根据申请表ID获取信息
        /// </summary>
        /// <returns></returns>
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
                    daoben_hr_grade_change mainInfo = db.Queryable<daoben_hr_grade_change>().InSingle(id);
                    if (mainInfo == null)
                        return "信息错误：指定的信息不存在";
                    daoben_hr_emp_job jobInfo = db.Queryable<daoben_hr_emp_job>().InSingle(mainInfo.emp_id);
                    List<daoben_hr_grade_change_approve> approveInfoList = db.Queryable<daoben_hr_grade_change_approve>().Where(a => a.main_id == id).ToList();
                    List<daoben_hr_grade_change_file> fileInfoList = db.Queryable<daoben_hr_grade_change_file>().Where(a => a.main_id == id).ToList();
                    object resultObj = new
                    {
                        mainInfo = mainInfo,
                        jobInfo = jobInfo,
                        approveInfoList = approveInfoList,
                        fileInfoList = fileInfoList
                    };
                    return resultObj.ToJson();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }


        public string Approve(daoben_hr_grade_change_approve approveInfo, daoben_hr_grade_change mainInfo)
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
                    daoben_hr_grade_change origInfo = db.Queryable<daoben_hr_grade_change>().InSingle(approveInfo.main_id);
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
                        //晋升降级effectInfoList TODO 定时任务,申请成功之后呢？
                        approveInfo.status = 100;
                    }

                    else if (approveInfo.status > 0)
                        approveInfo.status = 0 + 1 + origInfo.approve_status;
                    else
                        approveInfo.status = 0 - 1 - origInfo.approve_status;
                    if (origInfo.approve_status == 1 || origInfo.approve_status==0)
                        upObj = new
                        {
                            approve_status = approveInfo.status,
                            date_approve = mainInfo.date_approve,
                            execute_status = -2
                        };
                    else if (approveInfo.status == 100)
                    {

                        upObj = new
                        {
                            approve_status = 100,
                            execute_status = -1
                        };
                    }
                    else
                        upObj = new
                        {
                            approve_status = approveInfo.status
                        };
                    db.CommandTimeOut = 30;
                    db.BeginTran();

                    db.Update<daoben_hr_grade_change>(upObj, a => a.id == origInfo.id);
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
        public string Confirm(string id)
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
                    daoben_hr_grade_change origInfo = db.Queryable<daoben_hr_grade_change>().InSingle(id);
                    if (origInfo == null || origInfo.approve_status != 1)
                        return "信息错误：指定的申请信息不存在或者无需确认";

                    upObj = new
                    {
                        approve_status = origInfo.approve_status + 1
                    };

                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    db.Update<daoben_hr_grade_change>(upObj, a => a.id == id);

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
                    daoben_hr_grade_change mainInfo = db.Queryable<daoben_hr_grade_change>().SingleOrDefault(a => a.id == id);
                    if (mainInfo == null || mainInfo.approve_status == 0)
                        //审批过后，将不能撤回
                        return "撤回失败：指定的申请信息不存在或者已被审批";
                    else
                    {
                        db.CommandTimeOut = 30;
                        db.BeginTran();
                        db.Delete<daoben_hr_grade_change>(a => a.id == id);
                        db.Delete<daoben_hr_grade_change_file>(a => a.main_id == id);
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



        public string Add(daoben_hr_grade_change addInfo, List<daoben_hr_grade_change_file> fileInfo)
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
                    #region  新旧职位信息+职等

                    daoben_org_position posInfo = db.Queryable<daoben_org_position>().InSingle(empInfo.position_id);
                    daoben_org_position posInfoNew = db.Queryable<daoben_org_position>().InSingle(addInfo.position_id_new);
                    daoben_org_dept_area deptInfo = db.Queryable<daoben_org_dept_area>().InSingle(empInfo.dept_id);
                    daoben_org_dept_area deptInfoNew = db.Queryable<daoben_org_dept_area>().InSingle(addInfo.dept_id_new);
                    daoben_org_dept_area areaInfo = db.Queryable<daoben_org_dept_area>().InSingle(empInfo.position_id);
                    daoben_org_dept_area areaInfoNew = db.Queryable<daoben_org_dept_area>().InSingle(addInfo.area_id_new);

                    if (posInfoNew == null)
                        return "信息错误：指定的新的职位信息不存在";
                    if (deptInfoNew == null)
                        return "信息错误：指定的新的部门信息不存在";
                    
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
                    addInfo.company_id = posInfo.company_id;
                    addInfo.company_id_parent = posInfo.company_id_parent;
                    addInfo.company_name = posInfo.company_linkname;
                    addInfo.dept_id = deptInfo.id;
                    addInfo.dept_name = deptInfo.name;
                    addInfo.dept_id_new = deptInfoNew.id;
                    addInfo.dept_name = deptInfoNew.name;

                    addInfo.position_id = posInfo.id;
                    addInfo.position_name = posInfo.name;
                    addInfo.position_id_new = posInfoNew.id;
                    addInfo.position_name_new = posInfoNew.name;

                    addInfo.grade = empInfo.grade;

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
                    addInfo.execute_status = -2;

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
    }
}
