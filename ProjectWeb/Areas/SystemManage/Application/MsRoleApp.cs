using Base.Code;
using MySqlSugar;
using ProjectShare.Database;
using ProjectShare.Models;
using ProjectShare.Process;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectWeb.Areas.SystemManage.Application
{
    public class MsRoleApp
    {
        #region 获取角色列表
        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <param ></param>
        /// <returns></returns>
        public object GetList(Pagination pagination, daoben_ms_role roleInfo)
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

            bool inactive = false;
            string name = null;
            if (roleInfo != null)
            {
                inactive = roleInfo.inactive;
                name = roleInfo.name;
            }
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_ms_role>().Where(a => a.inactive == inactive && a.id != ConstData.ROLE_ID_ADMIN);
                if (!string.IsNullOrEmpty(name))
                    qable.Where(a => a.name.Contains(name));
                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;//获取所有数量
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }
        #endregion  

        #region 新增角色
        public string AddRole(daoben_ms_role roleInfo)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (roleInfo == null || string.IsNullOrEmpty(roleInfo.name))
                return "信息错误，操作失败!";
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    daoben_ms_role roleOrig = db.Queryable<daoben_ms_role>()
                            .Where(a => a.name == roleInfo.name).FirstOrDefault();
                    if (roleOrig != null)
                    {
                        if (roleOrig.inactive)
                            return "该角色已存在(已注销)，操作失败！";
                        else
                            return "该角色已存在，操作失败！";
                    }
                    roleInfo.create_time = DateTime.Now;
                    roleInfo.creator_id = LoginInfo.accountId;
                    roleInfo.creator_name = LoginInfo.empName;
                    roleInfo.inactive = false;
                    roleInfo.inactive_id = 0;
                    roleInfo.inactive_name = null;
                    roleInfo.inactive_time = null;
                    roleInfo.delible = true;

                    db.DisableInsertColumns = new string[] { "id" };
                    db.Insert(roleInfo);
                    return "success";
                }
            }
            catch (Exception ex)
            {
                return "系统出错：" + ex.Message;
            }
        }
        #endregion

        #region 修改角色
        public string EditRole(daoben_ms_role roleInfo)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (roleInfo == null || roleInfo.id < 1 || string.IsNullOrEmpty(roleInfo.name))
                return "信息错误，操作失败!";
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    daoben_ms_role roleOrig = db.Queryable<daoben_ms_role>().InSingle(roleInfo.id);
                    if (roleOrig == null || roleOrig.inactive)
                        return "角色不存在或已注销，操作失败!";
                    if (roleOrig.name == roleInfo.name && roleOrig.role_desc == roleInfo.role_desc)
                        return "success";
                    db.Update<daoben_ms_role>(new
                    {
                        name = roleInfo.name,
                        role_desc = roleInfo.role_desc,
                    }, a => a.id == roleInfo.id);
                    return "success";
                }
            }
            catch (Exception ex)
            {
                return "系统出错：" + ex.Message;
            }
        }
        #endregion


        #region 注销角色
        /// <summary>
        /// 注销角色及其该角色下的所有账户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string ActiveRole(int id)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if(id == ConstData.ROLE_ID_ADMIN)
                return "操作失败：系统角色！";
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    daoben_ms_role roleInfo = db.Queryable<daoben_ms_role>().InSingle(id);
                    if (roleInfo == null)
                        return "角色不存在，操作失败！";
                    object obj = new
                    {
                        inactive = !roleInfo.inactive,
                        inactive_id = LoginInfo.accountId,
                        inactive_name = LoginInfo.empName,
                        inactive_time = DateTime.Now
                    };
                    if (roleInfo.inactive)
                    {
                        // 启用
                        db.Update<daoben_ms_role>(obj, a => a.id == id); //启用角色
                        return "success";
                    }
                    db.CommandTimeOut = 30000;
                    try
                    {
                        db.BeginTran();
                        db.Update<daoben_ms_role>(obj, a => a.id == id); //注销角色
                        db.Update<daoben_ms_account>(obj, b => b.role_id == id); //注销账户
                        db.CommitTran();
                        return "success";
                    }
                    catch (Exception ex)
                    {
                        db.RollbackTran();
                        return "系统出错：" + ex.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                return "系统出错：" + ex.Message;
            }
        }

        #endregion

        #region 删除角色
        public string DeleteRole(int id)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");

            if (id < 1)
                return "信息错误，操作失败！";
            if (id == ConstData.ROLE_ID_ADMIN)
                return "操作失败：系统角色！";
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    var delInfo = db.Queryable<daoben_ms_role>().InSingle(id);
                    if (delInfo == null)
                        return "角色不存在，操作失败!";
                    if (!delInfo.inactive)
                        return "该角色未注销，操作失败！";
                    if (!delInfo.delible)
                        return "删除失败：该角色不可删除";
                    int accountCount = db.Queryable<daoben_ms_account>()
                        .Where(a => a.role_id == id).Count();
                    if (accountCount != 0)
                        return "该角色下存在账户，操作失败！";

                    db.Delete<daoben_ms_role>(a => a.id == id);
                    return "success";
                }
            }
            catch (Exception ex)
            {
                return "系统出错：" + ex.Message;
            }
        }
        #endregion

        #region 获取下拉列表
        /// <summary>
        /// 绑定下拉
        /// </summary>
        /// <returns></returns>
        public List<IdNamePair> GetIdNameList()
        {
            using (var db = SugarDao.GetInstance())
            {
                return db.Queryable<daoben_ms_role>().Where(a => a.inactive == false && a.id != ConstData.ROLE_ID_ADMIN)
                            .Select<IdNamePair>("id,name").ToList();
            }
        }
        #endregion

        #region 查看账户
        /// <summary>
        /// 筛选出该角色下的所有账户
        /// </summary>      
        /// <returns></returns>
        //public object GetAccountList(Pagination pagination, int roleID)
        //{
        //    var LoginInfo = OperatorProvider.Provider.GetCurrent();
        //    if (LoginInfo == null)
        //        return null;
        //    if (roleID == 0)
        //        return null;
        //    int records = 0;
        //    if (pagination == null)
        //        pagination = new Pagination();
        //    pagination.page = pagination.page > 0 ? pagination.page : 1;
        //    pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
        //    pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "create_time" : pagination.sidx;
        //    pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
        //    using (var db = SugarDao.GetInstance())
        //    {
        //        string accountList = db.Queryable<daoben_ms_account>()
        //              .Where(a => a.role_id == roleID)
        //              .OrderBy(pagination.sidx + " " + pagination.sord)
        //              .ToJsonPage(pagination.page, pagination.rows, ref records);
        //        pagination.records = records;
        //        if (string.IsNullOrEmpty(accountList) || accountList == "[]")
        //            return null;
        //        return accountList.ToJson();
        //    }
        //}
        #endregion
    }
}
