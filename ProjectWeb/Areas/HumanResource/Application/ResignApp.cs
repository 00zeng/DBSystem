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
    public class ResignApp
    {
        MsAccountApp msAccount = new MsAccountApp();


        public object GetListAll(Pagination pagination, daoben_hr_resign queryInfo, int? status, QueryTime queryTime)
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
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.create_time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_hr_resign>()
                    .JoinTable<daoben_hr_emp_info>((a, b) => a.emp_id == b.id);
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
                        else qable.Where(a => a.emp_id == LoginInfo.empId || a.creator_job_history_id == LoginInfo.jobHistoryId);
                    }
                }

                //queryInfo
                if (queryInfo != null)
                {
                    if (!string.IsNullOrEmpty(queryInfo.emp_name))
                        qable.Where(a => a.emp_name.Contains(queryInfo.emp_name));
                    if (queryInfo.type > 0)
                        qable.Where(a => a.type == queryInfo.type);
                    //if (queryInfo.area_id > 0)
                    //    qable.Where(a => a.area_id == queryInfo.area_id);
                    //if (queryInfo.dept_id > 0)
                    //    qable.Where(a => a.dept_id == queryInfo.dept_id);
                    if (queryInfo.company_id > 0)
                        qable.Where(a => a.company_id == queryInfo.company_id);
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
                       .Select("a.*,b.gender as gender,b.birthdate,b.education as edu")
                       .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public object GetListApprove(Pagination pagination, daoben_hr_resign queryInfo, int? status, QueryTime queryTime)
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
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.create_time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_hr_resign>()
                    .JoinTable<daoben_hr_emp_info>((a, b) => a.emp_id == b.id);
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (LoginInfo.roleId == ConstData.ROLE_ID_HR || LoginInfo.roleId == ConstData.ROLE_ID_HRMANAGER)
                    {
                        qable.Where(a => a.company_id == myCompanyInfo.id || a.company_id_parent == myCompanyInfo.id);
                        qable.Where(a => a.approve_status == 3);
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
                //queryInfo
                if (queryInfo != null)
                {
                    if (!string.IsNullOrEmpty(queryInfo.emp_name))
                        qable.Where(a => a.emp_name.Contains(queryInfo.emp_name));
                    if (queryInfo.type > 0)
                        qable.Where(a => a.type == queryInfo.type);
                    //if (queryInfo.area_id > 0)
                    //    qable.Where(a => a.area_id == queryInfo.area_id);
                    //if (queryInfo.dept_id > 0)
                    //    qable.Where(a => a.dept_id == queryInfo.dept_id);
                    if (queryInfo.company_id > 0)
                        qable.Where(a => a.company_id == queryInfo.company_id);
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
                       .Select("a.*,b.gender as gender,b.birthdate,b.education as edu")
                       .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public object GetMyList(Pagination pagination, daoben_hr_resign queryInfo, int? status, QueryTime queryTime)
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
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.create_time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_hr_resign>()
                    .JoinTable<daoben_hr_emp_info>((a, b) => a.emp_id == b.id)
                    .Where(a => a.emp_id == LoginInfo.empId);
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
                       .Select("a.*,b.gender as gender,b.birthdate,b.education as edu")
                       .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }


        /// <summary>
        /// 根据离职信息表ID获取信息
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
                    daoben_hr_resign mainInfo = db.Queryable<daoben_hr_resign>().InSingle(id);
                    if (mainInfo == null)
                        return "信息错误：指定的信息不存在";
                    daoben_hr_emp_job jobInfo = db.Queryable<daoben_hr_emp_job>().InSingle(mainInfo.emp_id);
                    daoben_hr_emp_info empInfo = db.Queryable<daoben_hr_emp_info>().InSingle(mainInfo.emp_id);
                    List<daoben_hr_resign_approve> approveInfoList = db.Queryable<daoben_hr_resign_approve>().Where(a => a.main_id == id).ToList();
                    List<daoben_hr_resign_file> fileInfoList = db.Queryable<daoben_hr_resign_file>().Where(a => a.main_id == id).ToList();
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

        public string Approve(daoben_hr_resign_approve approveInfo, daoben_hr_resign mainInfo, List<daoben_hr_resign_effect> effectInfoList)
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
                    daoben_hr_resign origInfo = db.Queryable<daoben_hr_resign>().SingleOrDefault(a => a.id == approveInfo.main_id);
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
                        //人事经理需要调整离职者的下属继任
                        //if (effectInfoList.Count < 1 || effectInfoList == null)
                        //    return "信息错误：人事经理需要调整离职者的下属继任";
                        approveInfo.status = 100;
                    }

                    else if (approveInfo.status > 0)
                        approveInfo.status = 0 + 1 + origInfo.approve_status;
                    else
                        approveInfo.status = 0 - 1 - origInfo.approve_status;

                    if (origInfo.approve_status == 0)
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
                        //TODO 通过之后设置定时任务
                    }
                    else
                        upObj = new
                        {
                            approve_status = approveInfo.status
                        };
                    db.CommandTimeOut = 30;
                    db.BeginTran();

                    db.Update<daoben_hr_resign>(upObj, a => a.id == origInfo.id);
                    db.Insert(approveInfo);
                    if (effectInfoList != null)
                        db.InsertRange(effectInfoList);
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
                    daoben_hr_resign origInfo = db.Queryable<daoben_hr_resign>().InSingle(id);
                    if (origInfo == null || origInfo.approve_status != 1)
                        return "信息错误：指定的申请信息不存在或者无需确认";

                    upObj = new
                    {
                        approve_status = origInfo.approve_status + 1
                    };

                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    db.Update<daoben_hr_resign>(upObj, a => a.id == id);

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
                    daoben_hr_resign mainInfo = db.Queryable<daoben_hr_resign>().SingleOrDefault(a => a.id == id);
                    if (mainInfo == null || mainInfo.approve_status == 0)
                        //审批过后，将不能撤回
                        return "撤回失败：指定的申请信息不存在或者已被审批";
                    else
                    {
                        db.CommandTimeOut = 30;
                        db.BeginTran();
                        db.Delete<daoben_hr_resign>(a => a.id == id);
                        db.Delete<daoben_hr_resign_file>(a => a.main_id == id);
                        db.Delete<daoben_hr_resign_effect>(a => a.main_id == id);
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

        public string Add(daoben_hr_resign addInfo, List<daoben_hr_resign_file> fileInfo)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    //if (addInfo.emp_id == LoginInfo.empId)
                    //    addInfo.type = 3;
                    //else
                    //    addInfo.type = 2;
                    //TO DO 自动离职的判断条件 正常离职和自动离职 有什么区别
                    daoben_hr_emp_job jobInfo = db.Queryable<daoben_hr_emp_job>().InSingle(addInfo.emp_id);
                    daoben_hr_emp_info empInfo = db.Queryable<daoben_hr_emp_info>().InSingle(addInfo.emp_id);
                    if (jobInfo == null || empInfo == null)
                        return "信息错误：指定的员工信息不存在";
                    addInfo.entry_date = jobInfo.entry_date;

                    daoben_org_company orgInfo = db.Queryable<daoben_org_company>().InSingle(jobInfo.company_id);
                    if (orgInfo == null)
                        return "信息错误：指定的信息不存在";
                    addInfo.company_id = orgInfo.id;
                    addInfo.company_name = orgInfo.name;
                    addInfo.company_id_parent = orgInfo.parent_id;

                    addInfo.dept_id = jobInfo.dept_id;
                    addInfo.dept_name = jobInfo.dept_name;
                    addInfo.area_id = jobInfo.area_l1_id;
                    addInfo.area_name = jobInfo.area_l1_name;
                    addInfo.position_id = jobInfo.position_id;
                    addInfo.position_name = jobInfo.position_name;
                    addInfo.gender = empInfo.gender;
                    addInfo.education = empInfo.education;
                    addInfo.grade = jobInfo.grade;
                    addInfo.work_number = jobInfo.work_number;
                    addInfo.emp_name = jobInfo.name;
                    addInfo.creator_job_history_id = LoginInfo.jobHistoryId;
                    addInfo.creator_name = LoginInfo.empName;
                    addInfo.create_time = DateTime.Now;
                    addInfo.id = Common.GuId();
                    addInfo.approve_status = 0;
                    addInfo.execute_status = 0;
                    if (fileInfo != null && fileInfo.Count > 0)
                    {
                        fileInfo.ForEach(a =>
                        {
                            a.main_id = addInfo.id;
                            //TODO 
                        });
                    }
                    db.CommandTimeOut = 30;
                    db.BeginTran();

                    db.Insert(addInfo);
                    db.DisableInsertColumns = new string[] { "id" };
                    if (fileInfo != null && fileInfo.Count > 0)
                        db.InsertRange(fileInfo);

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
