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
    public class LeavingApp
    {


        /// <summary>
        /// 查看全部请假信息
        /// </summary>
        public object GetListAll(Pagination pagination, daoben_hr_emp_job queryInfo, int approve_status, QueryTime queryTime)
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
                var qable = db.Queryable<daoben_hr_leaving>();
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
                        qable.Where(a => a.begin_time >= queryTime.startTime1);
                    if (queryTime.startTime2 != null)
                    {
                        queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                        qable.Where(a => a.begin_time < queryTime.startTime2);
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
                var qable = db.Queryable<daoben_hr_leaving>();

                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT1)
                    {
                        qable.Where(a => a.approve_status == 0);
                        qable.Where(a => (a.position_type == ConstData.POSITION_DEPT_M && a.company_id == myCompanyInfo.id)
                                || (a.position_type == ConstData.POSITION_GM2 && a.company_id_parent == myCompanyInfo.id));//事业部助理：事业部部门经理+分公司经理
                    } //事业部助理判断要先于部门经理判断
                    else if (myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT2)
                    {
                        qable.Where(a => a.approve_status == 0);
                        qable.Where(a => a.position_type == ConstData.POSITION_DEPT_M && a.company_id == myCompanyInfo.id);//事业部助理：事业部部门经理+分公司经理
                    }
                    else if (myPositionInfo.positionType == ConstData.POSITION_DEPT_M)
                    {
                        if (LoginInfo.roleId == ConstData.ROLE_ID_HRMANAGER)
                        {
                            qable.Where(a => ((a.company_id == myCompanyInfo.id || a.company_id_parent == myCompanyInfo.id) && a.approve_status == 2)
                                    || (a.dept_id == myPositionInfo.deptId && a.approve_status == 0 && a.emp_id != LoginInfo.empId));
                        }
                        else
                            qable.Where(a => a.dept_id == myPositionInfo.deptId && a.approve_status == 0 && a.emp_id != LoginInfo.empId);
                    }
                    else if (myPositionInfo.positionType == ConstData.POSITION_GM2)
                        qable.Where(a => a.company_id == myCompanyInfo.id && a.approve_status == 1 && a.emp_id != LoginInfo.empId);
                    else if (myPositionInfo.positionType == ConstData.POSITION_GM1)
                    {
                        qable.Where(a => a.approve_status == 1);
                        qable.Where(a => (a.company_id == myCompanyInfo.id) || (a.company_id_parent == myCompanyInfo.id && a.position_type == ConstData.POSITION_GM2));
                    }
                    else return null;//下属
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
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.create_time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            using (var db = SugarDao.GetInstance())
            {

                var qable = db.Queryable<daoben_hr_leaving>()
                    .JoinTable<daoben_hr_emp_job>((a, b) => a.emp_id == b.id)
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
                        qable.Where(a => a.begin_time >= queryTime.startTime1);
                    if (queryTime.startTime2 != null)
                    {
                        queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                        qable.Where(a => a.begin_time < queryTime.startTime2);
                    }
                }
                
                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                       .Select("a.*,b.work_number")
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
                    daoben_hr_leaving mainInfo = db.Queryable<daoben_hr_leaving>().InSingle(id);
                    if (mainInfo == null)
                        return "信息错误：指定的信息不存在";
                    daoben_hr_emp_job jobInfo = db.Queryable<daoben_hr_emp_job>().InSingle(mainInfo.emp_id);
                    List<daoben_hr_leaving_approve> approveInfoList = db.Queryable<daoben_hr_leaving_approve>().Where(a => a.main_id == id).ToList();
                    List<daoben_hr_leaving_file> fileInfoList = db.Queryable<daoben_hr_leaving_file>().Where(a => a.main_id == id).ToList();
                    object resultObj = new
                    {
                        mainInfo = mainInfo,
                        jobInfo = jobInfo,
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


        public string Add(daoben_hr_leaving addInfo, List<daoben_hr_leaving_file> imageList)
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
                    //只能自己请假
                    daoben_hr_emp_job empInfo = db.Queryable<daoben_hr_emp_job>().SingleOrDefault(a => a.id == LoginInfo.empId);
                    addInfo.position_type = empInfo.position_type;
                    addInfo.leaving_cancel = false;
                    bool isSuperiorExist = db.Queryable<daoben_hr_emp_job>().Any(a => a.dept_id == empInfo.dept_id && a.position_type == ConstData.POSITION_OFFICE_NORMAL);
                    if (empInfo.position_type == ConstData.POSITION_OFFICE_NORMAL)//普通员工+无部门经理 跳过第一段审批
                        if (isSuperiorExist)
                        {
                            addInfo.isnosuperior = 0;
                            addInfo.approve_status = 0;
                        }
                        else
                        {
                            addInfo.isnosuperior = 1;
                            addInfo.approve_status = 1;
                        }
                    addInfo.leaving_status = -2;
                    addInfo.id = Common.GuId();
                    addInfo.emp_id = empInfo.id;
                    addInfo.work_number = empInfo.work_number;
                    addInfo.emp_name = empInfo.name;
                    addInfo.position_id = empInfo.position_id;
                    addInfo.position_name = empInfo.position_name;

                    addInfo.dept_id = empInfo.dept_id;
                    addInfo.dept_name = empInfo.dept_name;
                    addInfo.area_id = empInfo.area_l1_id;
                    addInfo.area_name = empInfo.area_l1_name;
                    addInfo.company_id = empInfo.company_id;
                    addInfo.company_name = empInfo.company_name;
                    addInfo.company_id_parent = empInfo.company_id_parent;

                    addInfo.creator_job_history_id = LoginInfo.jobHistoryId;
                    addInfo.creator_name = LoginInfo.empName;
                    addInfo.create_time = DateTime.Now;

                    if (imageList != null && imageList.Count > 0)
                    {
                        imageList.ForEach(a =>
                        {
                            a.main_id = addInfo.id;
                            a.type = addInfo.leaving_type;
                        });
                    }
                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    db.Insert(addInfo);
                    db.DisableInsertColumns = new string[] { "id" };
                    if (imageList != null)
                        db.InsertRange(imageList);
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
        public string Approve(daoben_hr_leaving_approve approveInfo)
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
                    daoben_hr_leaving mainInfo = db.Queryable<daoben_hr_leaving>().InSingle(approveInfo.main_id);
                    if (mainInfo == null)
                        return "信息错误：指定的申请信息不存在";
                    approveInfo.approve_id = LoginInfo.accountId;
                    approveInfo.approve_name = LoginInfo.empName;
                    approveInfo.approve_time = DateTime.Now;
                    approveInfo.approve_position_id = myPositionInfo.id;
                    approveInfo.approve_position_name = myPositionInfo.name;
                    //approveInfo.approve_note


                    if (LoginInfo.roleId == ConstData.ROLE_ID_HRMANAGER && approveInfo.status > 0)
                        //已通过，消息通知
                        //leaving_status 是否开始休假，休假状态；需要根据当前时间和请假时间相对比
                        approveInfo.status = 100;
                    else if (approveInfo.status > 0)
                        approveInfo.status = 0 + 1 + mainInfo.approve_status;
                    else
                        approveInfo.status = 0 - 1 - mainInfo.approve_status;

                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    db.Update<daoben_hr_leaving>(new { approve_status = approveInfo.status }, a => a.id == approveInfo.main_id);
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
                    daoben_hr_leaving mainInfo = db.Queryable<daoben_hr_leaving>().SingleOrDefault(a => a.id == id);
                    if (mainInfo == null || (mainInfo.approve_status - mainInfo.isnosuperior) == 0)
                        //审批过后，将不能撤回
                        return "撤回失败：指定的申请信息不存在或者已被审批";
                    else
                    {
                        db.CommandTimeOut = 30;
                        db.BeginTran();
                        db.Delete<daoben_hr_leaving>(a => a.id == id);
                        db.Delete<daoben_hr_leaving_file>(a => a.main_id == id);
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
