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
    public class LeavingCancelApp
    {
        MsAccountApp msAccount = new MsAccountApp();
        public object GetListAll(Pagination pagination, daoben_hr_leaving queryInfo, int? status, QueryTime queryTime)
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
                var qable = db.Queryable<daoben_hr_leaving_cancel>()
                    .JoinTable<daoben_hr_leaving>((a, b) => a.leaving_id == b.id)
                    .JoinTable<daoben_hr_leaving, daoben_hr_emp_job>((a, b, c) => b.emp_id == c.id);

                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (LoginInfo.roleId == ConstData.ROLE_ID_HR || LoginInfo.roleId == ConstData.ROLE_ID_HRMANAGER)
                        qable.Where<daoben_hr_leaving>((a, b) => b.company_id == myCompanyInfo.id);
                    else
                    {
                        if (myPositionInfo.positionType == ConstData.POSITION_GM1)
                            qable.Where<daoben_hr_leaving>((a, b) => b.company_id == myCompanyInfo.id);
                        else if (myPositionInfo.positionType == ConstData.POSITION_GM2)
                            qable.Where<daoben_hr_leaving>((a, b) => b.company_id == myCompanyInfo.id);
                        else if (myPositionInfo.positionType == ConstData.POSITION_DEPT_M)
                            qable.Where<daoben_hr_leaving>((a, b) => b.dept_id == myPositionInfo.deptId);
                        else qable.Where<daoben_hr_leaving>((a, b) => b.emp_id == LoginInfo.empId);
                    }
                }
                //queryInfo
                if (queryInfo != null)
                {
                    //if (!string.IsNullOrEmpty(queryInfo.creator_name))
                    //    qable.Where<daoben_hr_leaving>((a, b) => b.emp_name.Contains(queryInfo.creator_name));
                    if (queryInfo.leaving_type > 0)
                        qable.Where<daoben_hr_leaving>((a, b) => b.leaving_type == queryInfo.leaving_type);
                    if (queryInfo.area_id > 0)
                        qable.Where<daoben_hr_leaving>((a, b) => b.area_id == queryInfo.area_id);
                    if (queryInfo.dept_id > 0)
                        qable.Where<daoben_hr_leaving>((a, b) => b.dept_id == queryInfo.dept_id);

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
                        qable.Where<daoben_hr_leaving>((a,b) => b.begin_time >= queryTime.startTime1);
                    if (queryTime.startTime2 != null)
                    {
                        queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                        qable.Where<daoben_hr_leaving>((a, b) => b.begin_time < queryTime.startTime2);
                    }
                }
                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                       .Select("a.*,b.begin_time,b.end_time,b.days,b.content_reason,c.name as emp_name,c.position_name,c.dept_name,c.company_name,c.area_name,c.work_number")
                       .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public object GetListApprove(Pagination pagination, daoben_hr_leaving queryInfo, int? status, QueryTime queryTime)
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
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "cancel_begin_time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_hr_leaving_cancel>()
                    .JoinTable<daoben_hr_leaving>((a, b) => a.leaving_id == b.id)
                    .JoinTable<daoben_hr_leaving, daoben_hr_emp_job>((a, b, c) => b.emp_id == c.id);


                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT1)
                    {
                        qable.Where(a => a.approve_status == 0);
                        qable.Where<daoben_hr_leaving>((a, b) => (b.position_type == ConstData.POSITION_DEPT_M && b.company_id == myCompanyInfo.id)
                                || (b.position_type == ConstData.POSITION_GM2 && b.company_id_parent == myCompanyInfo.id));//事业部助理：事业部部门经理+分公司经理
                    }
                    else if (myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT2)
                    {
                        qable.Where(a => a.approve_status == 0);
                        qable.Where<daoben_hr_leaving>((a, b) => b.position_type == ConstData.POSITION_DEPT_M && b.company_id == myCompanyInfo.id);
                    }
                    else if (myPositionInfo.positionType == ConstData.POSITION_DEPT_M)

                        if (LoginInfo.roleId == ConstData.ROLE_ID_HRMANAGER)
                        {
                            qable.Where<daoben_hr_leaving>((a, b) => ((b.company_id == myCompanyInfo.id || b.company_id_parent == myCompanyInfo.id) && a.approve_status == 2)
                                        || (b.dept_id == myPositionInfo.deptId && b.approve_status == 0 && b.emp_id != LoginInfo.empId));
                        }
                        else
                        {
                            qable.Where<daoben_hr_leaving>((a, b) => b.dept_id == myPositionInfo.deptId && a.approve_status == 0);
                            qable.Where<daoben_hr_leaving>((a, b) => b.emp_id != LoginInfo.empId);
                        }
                    else if (myPositionInfo.positionType == ConstData.POSITION_GM1)
                        qable.Where<daoben_hr_leaving>((a, b) => b.company_id == myCompanyInfo.id && a.approve_status == 1);
                    else if (myPositionInfo.positionType == ConstData.POSITION_GM2)
                    {
                        qable.Where<daoben_hr_leaving>((a, b) => b.company_id == myCompanyInfo.id && a.approve_status == 1);
                        qable.Where<daoben_hr_leaving>((a, b) => b.emp_id != LoginInfo.empId);
                    }
                    else return null;
                }
                //queryInfo
                if (queryInfo != null)
                {
                    //if (!string.IsNullOrEmpty(queryInfo.creator_name))
                    //    qable.Where<daoben_hr_leaving>((a, b) => b.emp_name.Contains(queryInfo.creator_name));
                    if (queryInfo.leaving_type > 0)
                        qable.Where<daoben_hr_leaving>((a, b) => b.leaving_type == queryInfo.leaving_type);
                    if (queryInfo.area_id > 0)
                        qable.Where<daoben_hr_leaving>((a, b) => b.area_id == queryInfo.area_id);
                    if (queryInfo.dept_id > 0)
                        qable.Where<daoben_hr_leaving>((a, b) => b.dept_id == queryInfo.dept_id);
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
                        qable.Where<daoben_hr_leaving>((a, b) => b.begin_time >= queryTime.startTime1);
                    if (queryTime.startTime2 != null)
                    {
                        queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                        qable.Where<daoben_hr_leaving>((a, b) => b.begin_time < queryTime.startTime2);
                    }
                }
                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                      .Select("a.*,b.begin_time,b.end_time,b.days,b.content_reason,c.name as emp_name,c.position_name,c.dept_name,c.company_name,c.area_name,c.work_number")
                      .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public object GetMyList(Pagination pagination, daoben_hr_leaving queryInfo, int? status, QueryTime queryTime)
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
                var qable = db.Queryable<daoben_hr_leaving_cancel>()
                    .JoinTable<daoben_hr_leaving>((a, b) => a.leaving_id == b.id)
                    .JoinTable<daoben_hr_leaving, daoben_hr_emp_job>((a, b, c) => b.emp_id == c.id)
                    .Where<daoben_hr_leaving>((a, b) => b.emp_id == LoginInfo.empId);
                //queryInfo
                if (queryInfo != null)
                {
                    //if (!string.IsNullOrEmpty(queryInfo.creator_name))
                    //    qable.Where<daoben_hr_leaving>((a, b) => b.emp_name.Contains(queryInfo.creator_name));
                    if (queryInfo.leaving_type > 0)
                        qable.Where<daoben_hr_leaving>((a, b) => b.leaving_type == queryInfo.leaving_type);
                    if (queryInfo.area_id > 0)
                        qable.Where<daoben_hr_leaving>((a, b) => b.area_id == queryInfo.area_id);
                    if (queryInfo.dept_id > 0)
                        qable.Where<daoben_hr_leaving>((a, b) => b.dept_id == queryInfo.dept_id);
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
                        qable.Where<daoben_hr_leaving>((a, b) => b.begin_time >= queryTime.startTime1);
                    if (queryTime.startTime2 != null)
                    {
                        queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                        qable.Where<daoben_hr_leaving>((a, b) => b.begin_time < queryTime.startTime2);
                    }
                }
                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                       .Select("a.*,b.begin_time,b.end_time,b.days,b.content_reason,c.name as emp_name,c.position_name,c.dept_name,c.company_name,c.area_name,c.work_number")
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
                    daoben_hr_leaving_cancel mainInfo = db.Queryable<daoben_hr_leaving_cancel>().InSingle(id);
                    if (mainInfo == null)
                        return "信息错误：指定的信息不存在";
                    List<daoben_hr_leaving_cancel_approve> approveInfoList = db.Queryable<daoben_hr_leaving_cancel_approve>().Where(a => a.main_id == id).ToList();
                    daoben_hr_leaving leavingInfo = db.Queryable<daoben_hr_leaving>().SingleOrDefault(a => a.id == mainInfo.leaving_id);
                    object resultObj = new
                    {
                        mainInfo = mainInfo,
                        leavingInfo = leavingInfo,
                        approveInfoList = approveInfoList
                    };
                    return resultObj.ToJson();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }


        public string Add(daoben_hr_leaving_cancel addInfo)
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
                    //只能自己销假
                    daoben_hr_emp_job empInfo = db.Queryable<daoben_hr_emp_job>().SingleOrDefault(a => a.id == LoginInfo.empId);

                    //请假信息核实
                    bool isExist = db.Queryable<daoben_hr_leaving>().Any(a => a.id == addInfo.leaving_id);
                    if (!isExist)
                        return "申请失败：请假信息不存在";
                    addInfo.creator_job_history_id = LoginInfo.jobHistoryId;
                    addInfo.creator_name = LoginInfo.empName;
                    addInfo.create_time = DateTime.Now;

                    addInfo.approve_status = 0;
                    addInfo.id = Common.GuId();

                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    //TODO 删除事务
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
        public string Approve(daoben_hr_leaving_cancel_approve approveInfo)
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
                    //申请表ID
                    daoben_hr_leaving_cancel mainInfo = db.Queryable<daoben_hr_leaving_cancel>().InSingle(approveInfo.main_id);
                    if (mainInfo == null)
                        return "信息错误：指定的申请信息不存在";
                    approveInfo.approve_id = LoginInfo.accountId;
                    approveInfo.approve_name = LoginInfo.empName;
                    approveInfo.approve_time = DateTime.Now;
                    approveInfo.approve_position_id = myPositionInfo.id;
                    approveInfo.approve_position_name = myPositionInfo.name;
                    //approveInfo.approve_note


                    if (LoginInfo.roleId == ConstData.ROLE_ID_HRMANAGER && approveInfo.status > 0)
                        approveInfo.status = 100;
                    else if (approveInfo.status > 0)
                        approveInfo.status += 1;
                    else
                        approveInfo.status = 0 - 1 - approveInfo.status;
                    if (approveInfo.status == 100)
                    {
                        //TODO 设置销假成功和起始时间

                    }

                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    if (approveInfo.status != 100)
                        db.Update<daoben_hr_leaving_cancel>(new { approve_status = approveInfo.status }, a => a.id == approveInfo.main_id);
                    else
                    {

                    }
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

        public string Delete(string id)
        {
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_hr_leaving_cancel mainInfo = db.Queryable<daoben_hr_leaving_cancel>().SingleOrDefault(a => a.id == id);
                    if (mainInfo == null || mainInfo.approve_status != 0)
                        //审批过后，将不能撤回
                        return "撤回失败：指定的申请信息不存在或者已被审批";
                    else
                    {
                        db.CommandTimeOut = 30;
                        db.BeginTran();
                        db.Delete<daoben_hr_leaving_cancel>(a => a.id == id);
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
