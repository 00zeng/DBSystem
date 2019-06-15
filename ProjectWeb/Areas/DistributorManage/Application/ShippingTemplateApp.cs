using Base.Code;
using MySqlSugar;
using ProjectShare.Database;
using ProjectShare.Process;
using System;
using System.Linq;
using ProjectWeb.Application;
using System.Collections.Generic;
using ProjectShare.Models;
using System.Text;

namespace ProjectWeb.Areas.DistributorManage.Application
{
    public class ShippingTemplateApp
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
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "create_time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_distributor_shipping_template_approve>();
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (myCompanyInfo.category == "事业部")
                        qable.Where(a => a.company_id == myCompanyInfo.id);
                    else if (myCompanyInfo.category == "分公司")
                        qable.Where(a => a.company_id == myCompanyInfo.parentId);
                    else return null;
                }

                //no queryInfo
                //if (!string.IsNullOrEmpty(queryName))
                //    qable.Where(a => a.name.Contains(queryName));

                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
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
        //    pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "create_time" : pagination.sidx;
        //    pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
        //    CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
        //    PositionInfo myPositionInfo = LoginInfo.positionInfo;

        //    using (var db = SugarDao.GetInstance())
        //    {
        //        var qable = db.Queryable<daoben_distributor_shipping_template_approve>().Where(a => a.approve_status == 0);

        //        string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
        //                .ToJsonPage(pagination.page, pagination.rows, ref records);
        //        pagination.records = records;
        //        if (string.IsNullOrEmpty(listStr) || listStr == "[]")
        //            return null;
        //        return listStr.ToJson();
        //    }
        //}
        public string Import(List<daoben_distributor_shipping_template> importList, daoben_distributor_shipping_template_approve importInfo)
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

            importInfo.creator_job_history_id = LoginInfo.jobHistoryId;
            importInfo.creator_name = LoginInfo.empName;
            importInfo.create_time = DateTime.Now;
            importInfo.create_time = DateTime.Now;
            //立即生效
            if (importInfo.effect_now || importInfo.effect_date < now)
            {
                importInfo.effect_status = 1;
                if (importInfo.effect_now)
                    importInfo.effect_date = now;
            }
            else
            {
                //TODO 添加定时任务
            }
            importInfo.company_id = myCompanyInfo.id;
            importInfo.company_name = myCompanyInfo.name;

            using (var db = SugarDao.GetInstance())
            {
                db.CommandTimeOut = 300;
                try
                {
                    db.BeginTran();
                    db.Update<daoben_distributor_shipping_template_approve>(new { effect_status = 2 }, a => a.effect_status == 1);
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
        //public string Approve(daoben_distributor_shipping_template_approve approveInfo)
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
        //            daoben_distributor_shipping_template_approve origInfo = db.Queryable<daoben_distributor_shipping_template_approve>().InSingle(approveInfo.id);
        //            if (origInfo == null)
        //                return "信息错误：指定的申请信息不存在";

        //            if (approveInfo.approve_status > 0)
        //                approveInfo.approve_status = 100;   // 以100作为审批完成的标志
        //            else
        //                approveInfo.approve_status = -100;
        //            object upObj = new
        //            {
        //                status = approveInfo.approve_status,
        //                approve_note = approveInfo.approve_note,
        //                approve_id = LoginInfo.accountId,
        //                approve_name = LoginInfo.empName,
        //                approve_position_id = myPositionInfo.id,
        //                approve_position_name = myPositionInfo.name,
        //                approve_time = DateTime.Now
        //            };

        //            db.Update<daoben_sale_outstorage_approve>(upObj, a => a.id == approveInfo.id);
        //            //db.Update<daoben_sale_outstorage>(new { status = origInfo.status }, a => a.import_file_id == approveInfo.id);
        //            return "success";
        //        }
        //        catch (Exception ex)
        //        {
        //            return "系统出错：" + ex.Message;
        //        }
        //    }
        //}
        public string GetInfo(string id, bool isEffect)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_distributor_shipping_template_approve mainInfo = null;
                    List<daoben_distributor_shipping_template> feeList = null;

                    if (isEffect)
                    {
                        mainInfo = db.Queryable<daoben_distributor_shipping_template_approve>().SingleOrDefault(a => a.effect_status == 1 && a.company_id == myCompanyInfo.id);
                        if (mainInfo != null)
                            feeList = db.Queryable<daoben_distributor_shipping_template>().Where(a => a.main_id == mainInfo.id).ToList();
                    }
                    else
                    {
                        mainInfo = db.Queryable<daoben_distributor_shipping_template_approve>().SingleOrDefault(a => a.id == id);
                        if (mainInfo != null)
                            feeList = db.Queryable<daoben_distributor_shipping_template>().Where(a => a.main_id == mainInfo.id).ToList();
                    }
                    object resultObj = new
                    {
                        mainInfo = mainInfo,
                        feeList = feeList
                    };
                    return resultObj.ToJson();

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

        public string GetInfoMain(string id)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    string mainInfoStr = db.Queryable<daoben_distributor_shipping_template_approve>()
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
                    string listStr = db.Queryable<daoben_distributor_shipping_template>().Where(a => a.main_id == id)
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
    }
}
