using Base.Code;
using MySqlSugar;
using ProjectShare.Database;
using ProjectShare.Process;
using System;
using System.Linq;
using System.Collections.Generic;
using ProjectShare.Models;

namespace ProjectWeb.Areas.HumanResource.Application
{
    public class AttendanceApp
    {
        public object GetList(Pagination pagination, string queryName, QueryTime queryTime)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");

            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.month" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_hr_attendance>()
                        .JoinTable<daoben_hr_emp_job>((a, b) => a.emp_id == b.id)
                        .JoinTable<daoben_hr_attendance_approve>((a, c) => a.import_file_id == c.id);
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    //hr+总经理+财务
                    if (LoginInfo.roleId == ConstData.ROLE_ID_HR || LoginInfo.roleId == ConstData.ROLE_ID_HRMANAGER)
                        qable.Where<daoben_hr_emp_job>((a, b) => b.company_id == myCompanyInfo.id);
                    else if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIAL || LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER)
                        qable.Where<daoben_hr_emp_job>((a, b) => b.company_id == myCompanyInfo.id);
                    else if (myPositionInfo.positionType == ConstData.POSITION_GM1 || myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT1)
                        qable.Where<daoben_hr_emp_job>((a, b) => b.company_id == myCompanyInfo.id);
                    else if (myPositionInfo.positionType == ConstData.POSITION_GM2 || myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT2)
                        qable.Where<daoben_hr_emp_job>((a, b) => b.company_id == myCompanyInfo.id);
                    else return null;
                }

                if (!string.IsNullOrEmpty(queryName))
                    qable.Where(a => a.emp_name.Contains(queryName));
                if (queryTime != null)
                {
                    if (queryTime.startTime1 != null)
                        qable.Where(a => a.month >= queryTime.startTime1);
                    if (queryTime.startTime2 != null)
                    {
                        queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                        qable.Where(a => a.month < queryTime.startTime2);
                    }
                }
               

                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .Select("a.month,a.emp_name,b.position_name,b.dept_name,b.company_name,a.work_days,a.attendance,a.attendance_rate,c.import_file as import_file,c.creator_name as creator_name,c.create_time as create_time")
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }
        public object GetListHistory(Pagination pagination, QueryTime queryTime)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");

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
                var qable = db.Queryable<daoben_hr_attendance_approve>();
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    //hr+总经理+财务
                    if (LoginInfo.roleId == ConstData.ROLE_ID_HR || LoginInfo.roleId == ConstData.ROLE_ID_HRMANAGER)
                        qable.Where(a => a.company_id == myCompanyInfo.id);
                    else if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIAL || LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER)
                        qable.Where(a => a.company_id == myCompanyInfo.id);
                    else if (myPositionInfo.positionType == ConstData.POSITION_GM1 || myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT1)
                        qable.Where(a => a.company_id == myCompanyInfo.id);
                    else if (myPositionInfo.positionType == ConstData.POSITION_GM2 || myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT2)
                        qable.Where(a => a.company_id == myCompanyInfo.id);
                    else return null;
                }

                //if (!string.IsNullOrEmpty(queryName))
                //    qable.Where(a => a..Contains(queryName));                
                if (queryTime != null)
                {
                    if (queryTime.startTime1 != null)
                        qable.Where(a => a.create_time >= queryTime.startTime1);
                    if (queryTime.startTime2 != null)
                    {
                        queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                        qable.Where(a => a.create_time < queryTime.startTime2);
                    }
                }
                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        /// <summary>
        /// 保留我的审批
        /// </summary>
        //public object GetListApprove(Pagination pagination, string queryName)
        //{
        //    OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
        //    if (LoginInfo == null)
        //        throw new Exception("用户登陆过期，请重新登录");

        //    int records = 0;
        //    if (pagination == null)
        //        pagination = new Pagination();
        //    pagination.page = pagination.page > 0 ? pagination.page : 1;
        //    pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
        //    pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "company_id" : pagination.sidx;
        //    pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
        //    CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
        //    PositionInfo myPositionInfo = LoginInfo.positionInfo;

        //    using (var db = SugarDao.GetInstance())
        //    {
        //        var qable = db.Queryable<daoben_activity_attaining_approve>().Where(a => a.status == 0);

        //        string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
        //                .ToJsonPage(pagination.page, pagination.rows, ref records);
        //        pagination.records = records;
        //        if (string.IsNullOrEmpty(listStr) || listStr == "[]")
        //            return null;
        //        return listStr.ToJson();
        //    }
        //}
        public string Import(List<daoben_hr_attendance> importList, daoben_hr_attendance_approve importInfo)
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

            importInfo.creator_job_history_id = LoginInfo.jobHistoryId;
            importInfo.creator_name = LoginInfo.empName;
            importInfo.create_time = DateTime.Now;
            //立即生效
            importInfo.status = 100;
            importInfo.company_id = myCompanyInfo.id;
            importInfo.company_name = myCompanyInfo.name;

            using (var db = SugarDao.GetInstance())
            {
                db.CommandTimeOut = 300;
                try
                {
                    db.BeginTran();
                    db.Insert(importInfo);
                    db.DisableInsertColumns = new string[] { "id" };
                    if (importList.Count > 25)
                        db.SqlBulkCopy(importList);
                    else
                        db.InsertRange(importList);
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
        /// <summary>
        /// 保留审批功能
        /// </summary>
        //public string Approve(daoben_hr_attendance_approve approveInfo)
        //{
        //    var LoginInfo = OperatorProvider.Provider.GetCurrent();
        //    if (LoginInfo == null)
        //        return "登录超时，请重新登录";
        //    CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
        //    PositionInfo myPositionInfo = LoginInfo.positionInfo;
        //    using (var db = SugarDao.GetInstance())
        //    {
        //        try
        //        {
        //            daoben_hr_attendance_approve origInfo = db.Queryable<daoben_hr_attendance_approve>().InSingle(approveInfo.id);
        //            if (origInfo == null)
        //                return "信息错误：指定的申请信息不存在";

        //            if (approveInfo.status > 0)
        //                approveInfo.status = 100;   // 以100作为审批完成的标志
        //            else
        //                approveInfo.status = -100;
        //            object upObj = new
        //            {
        //                status = approveInfo.status,
        //                approve_note = approveInfo.approve_note,
        //                approve_id = LoginInfo.accountId,
        //                approve_name = LoginInfo.empName,
        //                approve_position_id = myPositionInfo.id,
        //                approve_position_name = myPositionInfo.name,
        //                approve_time = DateTime.Now
        //            };

        //            db.Update<daoben_hr_attendance_approve>(upObj, a => a.id == approveInfo.id);
        //            //db.Update<daoben_sale_outstorage>(new { status = origInfo.status }, a => a.import_file_id == approveInfo.id);
        //            return "success";
        //        }
        //        catch (Exception ex)
        //        {
        //            return "系统出错：" + ex.Message;
        //        }
        //    }
        //}

        public string GetInfoMain(string id)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    string mainInfoStr = db.Queryable<daoben_hr_attendance_approve>()
                            .JoinTable<daoben_hr_emp_job_history>((a, b) => a.creator_job_history_id == b.id)
                            .Where<daoben_hr_emp_job_history>((a, b) => a.id == id)
                            .Select("a.*, b.position_name as creator_position_name").ToJson();
                    return mainInfoStr;

                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
        public object GetInfoPage(Pagination pagination, string id)
        {
            if (string.IsNullOrEmpty(id))
                return "信息错误：ID不能为空";

            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    string listStr = db.Queryable<daoben_hr_attendance>().Where(a => a.import_file_id == id)
                            .OrderBy("id asc").ToJsonPage(pagination.page, pagination.rows, ref records);
                    pagination.records = records;
                    if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                        return null;
                    return listStr.ToJson();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
        //internal string Delete(string id)
        //{
        //    using (var db = SugarDao.GetInstance())
        //    {
        //        try
        //        {
        //            daoben_hr_attendance_approve mainInfo = db.Queryable<daoben_hr_attendance_approve>().SingleOrDefault(a => a.id == id);
        //            if (mainInfo == null || mainInfo.status != 0)
        //                //财务经理审批过后，将不能撤回
        //                return "撤回失败：指定的申请信息不存在或者已被审批";
        //            else
        //            {
        //                db.CommandTimeOut = 30;
        //                db.BeginTran();
        //                db.Delete<daoben_hr_attendance_approve>(a => a.id == id);
        //                db.Delete<daoben_hr_attendance>(a => a.import_file_id == id);
        //                db.CommitTran();
        //                return "success";
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            db.RollbackTran();
        //            return "系统出错：" + ex.Message;
        //        }
        //    }
        //}
    }
}
