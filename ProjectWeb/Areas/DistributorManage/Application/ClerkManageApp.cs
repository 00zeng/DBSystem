using Base.Code;
using Base.Code.Security;
using MySqlSugar;
using ProjectShare.Database;
using ProjectShare.Models;
using ProjectShare.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectWeb.Areas.DistributorManage.Application
{
    public class ClerkManageApp
    {

        public object GetList(Pagination pagination, daoben_distributor_clerk queryInfo)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.create_time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_distributor_clerk>()
                        .JoinTable<daoben_ms_account>((a, b) => a.id == b.employee_id)
                        .Select("a.*,b.account,b.inactive,b.creator_id,b.creator_name");
                if (LoginInfo.roleId == ConstData.ROLE_ID_DISTRIBUTOR)
                {
                    qable.Where(a => a.distributor_id == LoginInfo.empId);
                }
                else if (LoginInfo.roleId == ConstData.ROLE_ID_CLERK)
                {
                    qable.Where(a => a.guide_id == LoginInfo.empId);
                }

                if (queryInfo != null)
                {
                    if (!string.IsNullOrEmpty(queryInfo.name))
                        qable.Where(a => a.name.Contains(queryInfo.name));
                    if (!string.IsNullOrEmpty(queryInfo.distributor_name))
                        qable.Where(a => a.distributor_name == queryInfo.distributor_name);
                    if (!string.IsNullOrEmpty(queryInfo.guide_name))
                        qable.Where(a => a.guide_name.Contains(queryInfo.guide_name));
                }
                string listStr = qable
                        .OrderBy(pagination.sidx + " " + pagination.sord)
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public object GetGridJsonResign(Pagination pagination, daoben_distributor_clerk_resign queryInfo)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "create_time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_distributor_clerk_resign>();
                if (queryInfo != null)
                {
                    if (!string.IsNullOrEmpty(queryInfo.name))
                        qable.Where(a => a.name.Contains(queryInfo.name));
                    if (!string.IsNullOrEmpty(queryInfo.distributor_name))
                        qable.Where(a => a.distributor_name == queryInfo.distributor_name);
                    if (!string.IsNullOrEmpty(queryInfo.guide_name))
                        qable.Where(a => a.guide_name.Contains(queryInfo.guide_name));
                }
                string listStr = qable
                        .OrderBy(pagination.sidx + " " + pagination.sord)
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }
        public string GetInfo(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;
            using (var db = SugarDao.GetInstance())
            {
                string getInfoStr = db.Queryable<daoben_distributor_clerk>()
                           .JoinTable<daoben_ms_account>((a, b) => a.id == b.employee_id)
                           .Where(a => a.id == id)
                           .Select("a.*, b.account, b.inactive as account_state").ToJson();
                return getInfoStr;
            }
        }
        public string GetResignInfo(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;
            using (var db = SugarDao.GetInstance())
            {
                string getInfoStr = db.Queryable<daoben_distributor_clerk_resign>()
                           .JoinTable<daoben_ms_account>((a, b) => a.id == b.employee_id)
                           .Where(a => a.id == id)
                           .Select("a.*, b.account, b.inactive as account_state").ToJson();
                return getInfoStr;
            }
        }
        public string Add(daoben_distributor_clerk addInfo, daoben_ms_account addAccountInfo)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            if (addInfo == null || string.IsNullOrEmpty(addInfo.name))
                return "信息错误，操作失败!";
            if (addAccountInfo == null || string.IsNullOrEmpty(addAccountInfo.account))
                return "信息错误，操作失败!";
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_distributor_info distributorInfo = db.Queryable<daoben_distributor_info>().InSingle(addInfo.distributor_id);
                    if (distributorInfo == null)
                        return "信息错误：指定所属经销商不存在";
                    daoben_ms_account accountOrig = db.Queryable<daoben_ms_account>()
                                .Where(a => a.account == addAccountInfo.account).FirstOrDefault();
                    if (accountOrig != null)
                        return "信息错误：指定账户名称已存在" + (accountOrig.inactive ? "(已注销)" : "") + "，操作失败！";

                    addInfo.distributor_name = distributorInfo.name;
                    addInfo.company_id = distributorInfo.company_id;
                    addInfo.company_name = distributorInfo.company_name;
                    addInfo.company_id_parent = distributorInfo.company_id_parent;
                    addInfo.id = Common.GuId();
                    addInfo.creator_job_history_id = LoginInfo.jobHistoryId;
                    addInfo.creator_name = LoginInfo.empName;
                    addInfo.create_time = DateTime.Now;
                    addInfo.create_time = addAccountInfo.reg_time = DateTime.Now;

                    addAccountInfo.employee_id = addInfo.id;
                    addAccountInfo.employee_name = addInfo.name;
                    addAccountInfo.role_id = ConstData.ROLE_ID_CLERK;
                    addAccountInfo.role_name = "店员";
                    if (addAccountInfo.inactive)
                    {
                        addAccountInfo.inactive_id = LoginInfo.accountId;
                        addAccountInfo.inactive_name = LoginInfo.empName;
                        addAccountInfo.inactive_time = DateTime.Now;
                    }
                    else
                    {
                        addAccountInfo.inactive_id = 0;
                        addAccountInfo.inactive_name = null;
                        addAccountInfo.inactive_time = null;
                    }
                    addAccountInfo.password = PasswordStorage.CreateHash(addAccountInfo.password);

                    db.CommandTimeOut = 30;

                    db.BeginTran();
                    db.Insert(addInfo);
                    db.Insert(addAccountInfo);
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

        public string Edit(daoben_distributor_clerk editInfo)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_distributor_clerk clerkOrig = db.Queryable<daoben_distributor_clerk>().InSingle(editInfo.id);
                    if (clerkOrig == null)
                        return "修改失败：店员不存在或已离职";
                    //信息修改
                    object obj = new
                    {
                        phone = editInfo.phone,
                        wechat = editInfo.wechat,
                        entry_date = editInfo.entry_date,
                    };
                    db.Update<daoben_distributor_info>(obj, a => a.id == editInfo.id);
                    return "success";
                }
                catch (Exception ex)
                {
                    return "系统出错：" + ex.Message;
                }
            }
        }
        public string AccountActive(string id)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (string.IsNullOrEmpty(id))
                return "信息错误，操作失败！";
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    daoben_ms_account accountInfo = db.Queryable<daoben_ms_account>().SingleOrDefault(a => a.employee_id == id);
                    if (accountInfo == null)
                        return "账户不存在，操作失败！";
                    object obj = new
                    {
                        inactive = !accountInfo.inactive,
                        inactive_id = LoginInfo.accountId,
                        inactive_name = LoginInfo.empName,
                        inactive_time = DateTime.Now
                    };
                    db.Update<daoben_ms_account>(obj, a => a.employee_id == id);
                }
                return "success";
            }
            catch (Exception ex)
            {
                return "系统出错：" + ex.Message;
            }
        }
        public string Delete(string id)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            if (string.IsNullOrEmpty(id))
                return "信息错误，操作失败！";

            using (var db = SugarDao.GetInstance())
            {
                daoben_distributor_clerk delInfo = db.Queryable<daoben_distributor_clerk>().InSingle(id);
                if (delInfo == null)
                    return "删除失败：店员不存在或者已经离职";
                db.CommandTimeOut = 30;
                try
                {
                    db.BeginTran();
                    db.Delete<daoben_ms_account>(a => a.employee_id == id);
                    db.Delete<daoben_distributor_clerk>(a => a.id == id);
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

        public string Resign(string id)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            if (string.IsNullOrEmpty(id))
                return "信息错误，操作失败！";

            using (var db = SugarDao.GetInstance())
            {
                daoben_distributor_clerk origInfo = db.Queryable<daoben_distributor_clerk>().InSingle(id);
                daoben_distributor_clerk_resign resignClerk = new daoben_distributor_clerk_resign();
                if (origInfo == null)
                    return "离职失败：店员不存在或者已经离职";
                resignClerk.id = origInfo.id;
                resignClerk.name = origInfo.name;
                resignClerk.phone = origInfo.phone;
                resignClerk.distributor_id = origInfo.distributor_id;
                resignClerk.distributor_name = origInfo.distributor_name;
                resignClerk.city = origInfo.city;
                resignClerk.city_code = origInfo.city_code;
                resignClerk.address = origInfo.address;
                resignClerk.guide_id = origInfo.guide_id;
                resignClerk.guide_name = origInfo.guide_name;
                resignClerk.entry_date = origInfo.entry_date;
                resignClerk.resign_date = DateTime.Now;
                resignClerk.operator_id = LoginInfo.accountId;
                resignClerk.operator_name = LoginInfo.empName;
                resignClerk.create_time = origInfo.create_time;
                resignClerk.creator_job_history_id = LoginInfo.jobHistoryId;
                resignClerk.creator_name = LoginInfo.empName;
                resignClerk.create_time = DateTime.Now;
                db.CommandTimeOut = 30;
                try
                {
                    db.BeginTran();
                    db.Insert(resignClerk);
                    db.Delete(origInfo);
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

    }
}
