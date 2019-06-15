using Base.Code;
using Base.Code.Security;
using MySqlSugar;
using NPOI.HSSF.UserModel;
using ProjectShare.Database;
using ProjectShare.Models;
using ProjectShare.Process;
using ProjectWeb.Areas.SalaryCalculate.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ProjectWeb.Areas.DistributorManage.Application
{
    public class DistributorManageApp
    {
        public object GetList(Pagination pagination, daoben_distributor_info queryInfo)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.name" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_distributor_info>()
                            .JoinTable<daoben_ms_account>((a, b) => a.id == b.employee_id);
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (myPositionInfo.positionType == ConstData.POSITION_GM1 || myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT1)   // 事业部
                        qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                    else if (myPositionInfo.positionType == ConstData.POSITION_GM2 || myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT2)   // 分公司
                        qable.Where(a => a.company_id == myCompanyInfo.id);
                    else if (myPositionInfo.positionType == ConstData.POSITION_SALESMANAGER)
                        qable.Where(a => a.area_l1_id == myPositionInfo.areaL1Id); //业务经理
                    else if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER || LoginInfo.roleId == ConstData.ROLE_ID_HRMANAGER)
                        qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                    else
                        return null;
                }
                if (queryInfo != null)
                {
                    if (!string.IsNullOrEmpty(queryInfo.name))
                        qable.Where(a => a.name.Contains(queryInfo.name));
                    if (!string.IsNullOrEmpty(queryInfo.sales_name))
                        qable.Where(a => a.sales_name.Contains(queryInfo.sales_name));
                    if (queryInfo.company_id > 0)
                        qable.Where(a => a.company_id == queryInfo.company_id);

                    if (queryInfo.area_l2_id > 0)
                        qable.Where(a => a.area_l2_id == queryInfo.area_l2_id); //区域搜索条件
                }
                string listStr = qable.Where(a => a.inactive == false)
                        .OrderBy(pagination.sidx + " " + pagination.sord)
                        .Select("a.id, a.name,a.name_v2,a.code,a.company_linkname, a.distributor_attribute, a.sp_attribute, a.city, a.address, a.area_l1_name,a.area_l2_name, a.company_name, b.account, b.inactive as account_status")
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public object GetListClosed(Pagination pagination, daoben_distributor_info queryInfo)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "name" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_distributor_info>();
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (LoginInfo.roleId != ConstData.ROLE_ID_FINANCIAL || LoginInfo.roleId != ConstData.ROLE_ID_FINANCIALMANAGER)
                    {
                        qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                    }
                    else
                    {
                        if (myPositionInfo.positionType == ConstData.POSITION_GM1 || myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT1)   // 事业部
                            qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                        else if (myPositionInfo.positionType == ConstData.POSITION_GM2 || myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT2)   // 分公司
                            qable.Where(a => a.company_id == myCompanyInfo.id);
                        else if (myPositionInfo.positionType == ConstData.POSITION_SALESMANAGER)
                            qable.Where(a => a.area_l1_id == myPositionInfo.areaL1Id); // 业务经理
                        else
                            return null;
                    }
                }
                if (queryInfo != null)
                {
                    if (!string.IsNullOrEmpty(queryInfo.name))
                        qable.Where(a => a.name.Contains(queryInfo.name));
                    if (queryInfo.company_id > 0)
                        qable.Where(a => a.company_id == queryInfo.company_id);
                    if (queryInfo.area_l1_id > 0)
                        qable.Where(a => a.area_l1_id == queryInfo.area_l1_id);
                }
                string listStr = qable.Where(a => a.inactive == true)
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
                daoben_distributor_info mainInfo = db.Queryable<daoben_distributor_info>().Where(a => a.id == id).SingleOrDefault();
                daoben_ms_account accountInfo = db.Queryable<daoben_ms_account>().Where(a => a.employee_id == id)
                            .Select("account, inactive").SingleOrDefault();
                List<daoben_distributor_info_file> imageList = db.Queryable<daoben_distributor_info_file>()
                            .Where(a => a.main_id == id && a.is_del == false)
                            .OrderBy(a => a.type, OrderByType.Asc).ToList();
                object resultObj = new
                {
                    mainInfo = mainInfo,
                    imageList = imageList,
                    accountInfo = accountInfo,
                };
                return resultObj.ToJson();
            }
        }

        public string Add(daoben_distributor_info addInfo, daoben_ms_account addAccountInfo, List<daoben_distributor_info_file> imageList)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            if (addInfo == null || string.IsNullOrEmpty(addInfo.name))
                return "信息错误，操作失败!";
            if (addAccountInfo == null || string.IsNullOrEmpty(addAccountInfo.account))
                return "信息错误，操作失败!";

            DateTime now = DateTime.Now;
            addInfo.id = Common.GuId();
            addInfo.inactive = false;            
            addAccountInfo.creator_id = LoginInfo.accountId;
            addAccountInfo.employee_id = addInfo.id;
            addAccountInfo.employee_name = addInfo.name;
            addAccountInfo.role_id = ConstData.ROLE_ID_DISTRIBUTOR;
            addAccountInfo.role_name = "经销商";
            addAccountInfo.inactive = !addAccountInfo.inactive;//勾选启用，修复测试bug

            daoben_distributor_info_history historyInfo = new daoben_distributor_info_history();
            historyInfo.main_id = addInfo.id;
            historyInfo.company_id = addInfo.company_id;
            historyInfo.company_id_parent = addInfo.company_id_parent;
            historyInfo.company_linkname = addInfo.company_linkname;
            historyInfo.area_l1_id = addInfo.area_l1_id;
            historyInfo.area_l1_name = addInfo.area_l1_name;
            historyInfo.area_l2_id = addInfo.area_l2_id;
            historyInfo.area_l2_name = addInfo.area_l2_name;
            historyInfo.status = 1;
            historyInfo.creator_job_history_id = LoginInfo.jobHistoryId;
            historyInfo.create_time = historyInfo.effect_date=addAccountInfo.reg_time = now;
         
            if (addAccountInfo.inactive)
            {
                addAccountInfo.inactive_id = LoginInfo.accountId;
                addAccountInfo.inactive_name = LoginInfo.empName;
                addAccountInfo.inactive_time = now;
            }
            else
            {
                addAccountInfo.inactive_id = 0;
                addAccountInfo.inactive_name = null;
                addAccountInfo.inactive_time = null;
            }
            addAccountInfo.password = PasswordStorage.CreateHash(addAccountInfo.password);
            if (imageList != null && imageList.Count > 0)
            {
                imageList.ForEach(a =>
                {
                    a.main_id = addInfo.id;
                    a.creator_job_history_id = LoginInfo.jobHistoryId;
                    a.creator_name = LoginInfo.empName;
                    a.create_time = now;
                });
            }
            using (var db = SugarDao.GetInstance())
            {
                bool isExist = db.Queryable<daoben_distributor_info>()
                            .Any(a => a.name == addInfo.name || a.code == addInfo.code);
                if (isExist)
                    return "信息错误：经销商名称或快捷编码已存在";
                daoben_org_company companyInfo = db.Queryable<daoben_org_company>().InSingle(addInfo.company_id);
                if (companyInfo == null)
                    return "信息错误：指定所属机构不存在";
                daoben_ms_account accountOrig = db.Queryable<daoben_ms_account>()
                           .Where(a => a.account == addAccountInfo.account).FirstOrDefault();
                if (accountOrig != null)
                    return "信息错误：指定账户名称已存在" + (accountOrig.inactive ? "(已注销)" : "") + "，操作失败！";

                addInfo.company_name = companyInfo.name;
                addInfo.company_linkname = companyInfo.link_name;
                addInfo.company_id_parent = companyInfo.parent_id;

                db.CommandTimeOut = 30;
                try
                {
                    db.BeginTran();
                    db.Insert(addInfo);
                    db.Insert(addAccountInfo);
                    db.Insert(historyInfo);
                    if (imageList != null && imageList.Count > 0)
                        db.InsertRange(imageList);
                    db.CommitTran();
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return "系统出错：" + ex.Message;
                }
                // 删除缓存
                CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
                string cacheSuffix = myCompanyInfo.category == "分公司" ? myCompanyInfo.parentId.ToString() : myCompanyInfo.id.ToString();
                CacheFactory.Cache().RemoveCache(CachePrefix.DISTRIBUTORTREE.ToString() + cacheSuffix);
                return "success";
            }
        }

        public string Edit(daoben_distributor_info editInfo,
                List<daoben_distributor_info_file> addImageList, List<int> delImageList)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            DateTime now = DateTime.Now;
            object delObj = new
            {
                is_del = true,
                del_account_id = LoginInfo.accountId,
                del_time = now
            };
            if (addImageList != null && addImageList.Count > 0)
            {
                addImageList.ForEach(a =>
                {
                    a.main_id = editInfo.id;
                    a.creator_job_history_id = LoginInfo.jobHistoryId;
                    a.creator_name = LoginInfo.empName;
                    a.create_time = DateTime.Now;
                });
            }
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_distributor_info distributorOrig = db.Queryable<daoben_distributor_info>().InSingle(editInfo.id);
                    if (distributorOrig == null || distributorOrig.inactive)
                        return "修改失败：经销商不存在或已停业";

                    //信息修改
                    object obj = new
                    {
                        name_v2 = editInfo.name_v2,
                        system_chain = editInfo.system_chain,
                        top_customers = editInfo.top_customers,
                        system_allocation = editInfo.system_allocation,
                        locale_attribute = editInfo.locale_attribute,
                        customer_category = editInfo.customer_category,
                        customer_level = editInfo.customer_level,
                        distributor_attribute = editInfo.distributor_attribute,
                        sp_attribute = editInfo.sp_attribute,
                        potential_salepoint = editInfo.potential_salepoint,
                        brand = editInfo.brand,
                        annual_sales_volume = editInfo.annual_sales_volume,
                        phone = editInfo.phone,
                        fax = editInfo.fax,
                        city = editInfo.city,
                        city_code = editInfo.city_code,
                        address = editInfo.address,
                        contact_name = editInfo.contact_name,
                        contact_phone = editInfo.contact_phone,
                        operation_mode = editInfo.operation_mode,
                        price_level = editInfo.price_level,
                        note = editInfo.note,
                    };

                    db.Update<daoben_distributor_info>(obj, a => a.id == editInfo.id);
                    if (delImageList != null && delImageList.Count > 0)
                        db.Update<daoben_distributor_info_file>(delObj, a => delImageList.Contains(a.id));
                    if (addImageList != null && addImageList.Count > 0)
                    {
                        db.DisableInsertColumns = new string[] { "id" };
                        db.InsertRange(addImageList);
                    }
                    return "success";
                }
                catch (Exception ex)
                {
                    return "系统出错：" + ex.Message;
                }
            }
        }

        public string Delete(string[] idArray, DateTime? effect_date = null)//批量注销
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (idArray == null || idArray.Length < 1)
                return "success";

            DateTime now = DateTime.Now;
            if (effect_date == null)
                effect_date = now;
            List<daoben_distributor_info_history> historyList = new List<daoben_distributor_info_history>();
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    for (int i = 0; i < idArray.Length; i++)
                    {
                        string id = idArray[i];
                        if (string.IsNullOrEmpty(id))
                            return "删除失败：经销商ID错误!";
                        daoben_distributor_info delInfo = db.Queryable<daoben_distributor_info>()
                                    .SingleOrDefault(a => a.id == id && a.inactive==false);
                        if (delInfo == null)
                            return "删除失败：该经销商不存在";

                        daoben_distributor_info_history historyInfo = new daoben_distributor_info_history();
                        historyInfo.main_id = id;
                        historyInfo.company_id = delInfo.company_id;
                        historyInfo.company_id_parent = delInfo.company_id_parent;
                        historyInfo.company_linkname = delInfo.company_linkname;
                        historyInfo.area_l1_id = delInfo.area_l1_id;
                        historyInfo.area_l1_name = delInfo.area_l1_name;
                        historyInfo.area_l2_id = delInfo.area_l2_id;
                        historyInfo.area_l2_name = delInfo.area_l2_name;                        
                        historyInfo.status = -1;                       
                        historyInfo.creator_job_history_id = LoginInfo.jobHistoryId;                       
                        historyInfo.effect_date = effect_date.ToDate();
                        historyList.Add(historyInfo);
                    }
                    List<string> idList = idArray.ToList<string>();
                    object inactiveObj = new
                    {
                        inactive = true,
                        inactive_job_history_id = LoginInfo.jobHistoryId,
                        inactive_time = now
                    };
                    db.CommandTimeOut = 30;
                    try
                    {
                        db.BeginTran();
                        //将原历史记录设为失效
                        db.Update<daoben_distributor_info_history>(inactiveObj, a => idList.Contains(a.main_id) && a.inactive == false);
                        //插入新的历史纪录
                        db.InsertRange(historyList);
                        //更新经销商表状态
                        db.Update<daoben_distributor_info>(new {inactive=1}, a => idList.Contains(a.id));
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
            catch (Exception ex)
            {
                return "系统出错：" + ex.Message;
            }
        }

        public string Ajust(string[] idArray, int company_id, int area_l1_id,int area_l2_id, DateTime? effect_date = null)//经销商批量调区
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (idArray == null || idArray.Length < 1)
                return "success";

            DateTime now = DateTime.Now;
            if (effect_date == null)
                effect_date = now;
            List<daoben_distributor_info_history> historyList = new List<daoben_distributor_info_history>();
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    //分公司/经理片区/业务片区是否存在
                   
                    //daoben_org_company companyInfo = db.Queryable<daoben_org_company>()
                    //              .SingleOrDefault(a => a.id == company_id && a.status == 1);
                    //if (companyInfo == null)
                    //    return "调区失败：该分公司不存在";

                    //daoben_org_area areaInfo1 = db.Queryable<daoben_org_area>()
                    //              .SingleOrDefault(a => a.id == area_l1_id &&a.type==1&& a.status == 1);
                    //if (areaInfo1 == null)
                    //    return "调区失败：该经理片区不存在";

                    daoben_org_area areaInfo2 = db.Queryable<daoben_org_area>()
                                 .SingleOrDefault(a => a.id == area_l2_id && a.type == 2 && a.status == 1);
                    if (areaInfo2 == null)
                        return "调区失败：该业务片区不存在";
                   
                    for (int i = 0; i < idArray.Length; i++)
                    {
                        string id = idArray[i];
                        if (string.IsNullOrEmpty(id))
                            return "调区失败：经销商ID错误!";
                        daoben_distributor_info ajustInfo = db.Queryable<daoben_distributor_info>()
                                                         .SingleOrDefault(a => a.id == id && a.inactive == false);
                        if (ajustInfo == null)
                            return "调区失败：该经销商不存在";
                        if (ajustInfo.company_id == company_id &&ajustInfo.area_l1_id==area_l1_id&&ajustInfo.area_l2_id==area_l2_id)
                            continue;//区域与原来完全相同
                        daoben_distributor_info_history historyInfo = new daoben_distributor_info_history();
                        historyInfo.main_id = id;
                        historyInfo.company_id = company_id;
                        historyInfo.company_id_parent = areaInfo2.company_id_parent;
                        historyInfo.company_linkname = areaInfo2.company_linkname;
                        historyInfo.area_l1_id = area_l1_id;
                        historyInfo.area_l1_name = areaInfo2.parent_name;
                        historyInfo.area_l2_id = area_l2_id;
                        historyInfo.area_l2_name = areaInfo2.name;                        
                        historyInfo.status = 1;                       
                        historyInfo.creator_job_history_id = LoginInfo.jobHistoryId;                       
                        historyInfo.effect_date = effect_date.ToDate();
                        historyList.Add(historyInfo);
                    }
                    List<string> idList = idArray.ToList<string>();
                    object inactiveObj = new
                    {
                        inactive = true,
                        inactive_job_history_id = LoginInfo.jobHistoryId,
                        inactive_time = now
                    };
                    object ajustObj = new
                    {
                        company_id = company_id,
                        company_name = areaInfo2.company_name,
                        company_id_parent = areaInfo2.company_id_parent,
                        company_linkname = areaInfo2.company_linkname,
                        area_l1_id = area_l1_id,
                        area_l1_name = areaInfo2.parent_name,
                        area_l2_id = area_l2_id,
                        area_l2_name = areaInfo2.name,
                        inactive = false,

                    };
                    db.CommandTimeOut = 30;
                    try
                    {
                        db.BeginTran();
                        //将原历史记录设为失效
                        db.Update<daoben_distributor_info_history>(inactiveObj, a => idList.Contains(a.main_id) && a.inactive == false);
                        //插入新的历史纪录
                        db.InsertRange(historyList);
                        //更新经销商表信息
                        db.Update<daoben_distributor_info>(ajustObj, a => idList.Contains(a.id));
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
            catch (Exception ex)
            {
                return "系统出错：" + ex.Message;
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
       

        //public string Delete(string id)
        //{
        //    var LoginInfo = OperatorProvider.Provider.GetCurrent();
        //    if (LoginInfo == null)
        //        return "登录超时，请重新登录";
        //    if (string.IsNullOrEmpty(id))
        //        return "信息错误，操作失败！";
        //    using (var db = SugarDao.GetInstance())
        //    {
        //        daoben_distributor_info delInfo = db.Queryable<daoben_distributor_info>().InSingle(id);
        //        if (delInfo == null)
        //            return "删除失败：经销商不存在";
        //        db.CommandTimeOut = 30;
        //        try
        //        {
        //            db.BeginTran();
        //            db.Delete<daoben_ms_account>(a => a.employee_id == id);
        //            db.Delete<daoben_distributor_info>(a => a.id == id);
        //            db.CommitTran();
        //        }
        //        catch (Exception ex)
        //        {
        //            db.RollbackTran();
        //            return "系统出错：" + ex.Message;
        //        }
        //        // 删除缓存
        //        CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
        //        string cacheSuffix = myCompanyInfo.category == "分公司" ? myCompanyInfo.parentId.ToString() : myCompanyInfo.id.ToString();
        //        CacheFactory.Cache().RemoveCache(CachePrefix.DISTRIBUTORTREE.ToString() + cacheSuffix);
        //        return "success";
        //    }
        //}
        /// <summary>
        /// 经销商列表（用于销售管理）
        /// </summary>
        /// <param name="companyId">分公司ID</param>
        /// <param name="companyParentId">事业部ID</param>
        /// <returns></returns>
        public string GetDistributorTree(int companyId, int companyParentId)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            if (myCompanyInfo.category == "分公司")
            {
                if (companyId > 0 && companyId != myCompanyInfo.id)
                    return "权限不足";
                companyId = myCompanyInfo.id;
                companyParentId = myCompanyInfo.parentId;
            }
            else if (myCompanyInfo.category == "事业部")
            {
                if (companyParentId > 0 && companyParentId != myCompanyInfo.id)
                    return "权限不足";
                companyParentId = myCompanyInfo.id;
            }
            if (companyParentId < 1)
                return "请指定分公司或事业部";
            string cacheSuffix = companyParentId.ToString();
            //List<CompanyTree> companyTree = CacheFactory.Cache()
            //            .GetCache<List<CompanyTree>>(CachePrefix.DISTRIBUTORTREE.ToString() + cacheSuffix);
            //if (companyTree != null)
            //{
            //    if (companyId > 0)
            //        return companyTree.Where(a => a.company_id == companyId).ToJson();
            //    return companyTree.ToJson();
            //}
            List<CompanyTree> companyTree = null; // 暂时不用缓存
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    // 以事业部为根节点
                    List<EmpKeyInfo> distributorList = db.Queryable<daoben_distributor_info>()
                                .Where(a => a.inactive == false && a.company_id_parent == companyParentId)
                                .Select<EmpKeyInfo>("id,code,name,name_v2,company_id,company_name,company_linkname,area_l1_id,area_l1_name,area_l2_id,area_l2_name,code AS grade,system_allocation AS emp_category ,CONCAT(name, ' (',company_name, ' - ', area_l1_name, ')') AS display_info")
                                .ToList();
                    if (distributorList == null && distributorList.Count() < 1)
                        return null;
                    companyTree = distributorList.GroupBy(c => c.company_id).Select(c1 => new CompanyTree
                    {
                        company_id = c1.Key,
                        company_name = c1.First().company_name,
                        company_linkname = c1.First().company_linkname,
                        area_l2_list = (c1.GroupBy(a => a.area_l2_id).Select(a1 => new AreaL2Tree
                        {
                            area_l2_id = a1.Key,
                            area_l2_name = a1.First().area_l2_name,
                            key_list = a1.ToList()
                        })).ToList()
                    }).ToList();
                    //CacheFactory.Cache().WriteCache(companyTree,
                    //            CachePrefix.DISTRIBUTORTREE.ToString() + cacheSuffix, DateTime.Now.AddDays(1));
                    if (companyId > 0)
                        return companyTree.Where(a => a.company_id == companyId).ToJson();
                    return companyTree.ToJson();
                }
                catch (Exception ex)
                {
                    return "系统出错：" + ex.Message;
                }
            }
        }

        /// <summary>
        /// 获取经销商选择列表
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public string GetIdNameList(int companyId, int areaL1Id, int areaL2Id)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            int companyParentId = 0;
            if (areaL2Id == 0 && myPositionInfo.areaL2Id > 0)
                areaL2Id = myPositionInfo.areaL2Id; // 登录人有所属区域的，未指定区域时只能获取所属区域的经销商
            if (areaL1Id == 0 && myPositionInfo.areaL1Id > 0)
                areaL1Id = myPositionInfo.areaL1Id; // 登录人有所属区域的，未指定区域时只能获取所属区域的经销商
            if (areaL2Id == 0 && areaL1Id == 0 && companyId == 0)
            {
                if (myCompanyInfo.category == "分公司")
                    companyId = myCompanyInfo.id;
                else if (myCompanyInfo.category == "事业部")
                    companyParentId = myCompanyInfo.id;
            }
            using (var db = SugarDao.GetInstance())
            {
                if (areaL2Id > 0)
                {
                    return db.Queryable<daoben_distributor_info>().Where(a => a.area_l2_id == areaL2Id && a.inactive == false)
                                .Select("id, name, CONCAT(name, ' (', area_l1_name, ')') as display_info").ToJson();
                }
                if (areaL1Id > 0)
                {
                    return db.Queryable<daoben_distributor_info>().Where(a => a.area_l1_id == areaL1Id && a.inactive == false)
                                .Select("id, name, CONCAT(name, ' (', area_l1_name, ')') as display_info").ToJson();
                }
                if (companyId > 0)
                {
                    return db.Queryable<daoben_distributor_info>().Where(a => a.company_id == companyId && a.inactive == false)
                                .Select("id, name, CONCAT(name, ' (', area_l1_name, ')') as display_info").ToJson();
                }
                if (companyParentId > 0)
                {
                    return db.Queryable<daoben_distributor_info>().Where(a => a.company_id_parent == companyParentId && a.inactive == false)
                               .Select("id, name, CONCAT(name, ' (',company_name, ' - ', area_l1_name, ')') as display_info").ToJson();
                }
                return null;
            }
        }

        /// <summary>
        /// 导出
        /// </summary>
        public MemoryStream ExportExcel(Pagination pagination, daoben_distributor_info queryInfo)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;

            if (pagination == null)
                pagination = new Pagination();
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.create_time" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_distributor_info>()
                            .JoinTable<daoben_ms_account>((a, b) => a.id == b.employee_id);
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (myPositionInfo.positionType == ConstData.POSITION_GM1 || myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT1)   // 事业部
                        qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                    else if (myPositionInfo.positionType == ConstData.POSITION_GM2 || myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT2)   // 分公司
                        qable.Where(a => a.company_id == myCompanyInfo.id);
                    else if (myPositionInfo.positionType == ConstData.POSITION_SALESMANAGER)
                        qable.Where(a => a.area_l1_id == myPositionInfo.areaL1Id); //业务经理
                    else if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER || LoginInfo.roleId == ConstData.ROLE_ID_HRMANAGER)
                        qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                    else
                        return null;
                }
                if (queryInfo != null)
                {
                    if (!string.IsNullOrEmpty(queryInfo.name))
                        qable.Where(a => a.name.Contains(queryInfo.name));
                    if (!string.IsNullOrEmpty(queryInfo.sales_name))
                        qable.Where(a => a.sales_name.Contains(queryInfo.sales_name));
                    if (queryInfo.company_id > 0)
                        qable.Where(a => a.company_id == queryInfo.company_id);
                    if (queryInfo.area_l2_id > 0)
                        qable.Where(a => a.area_l2_id == queryInfo.area_l2_id); //区域搜索条件
                }
                string str = "a.name,a.name_v2,a.code,b.account,(CASE WHEN b.inactive=1 THEN '已注销' WHEN b.inactive=0 THEN '正常' ELSE '-' END) as account_status,a.area_l2_name,a.company_linkname,a.city,a.address,a.distributor_attribute,a.sp_attribute,a.creator_name,DATE_FORMAT(a.create_time,'%Y-%m-%d')";
                var listDt = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                   .Select(str)
                   .ToDataTable();
                int rowsCount = listDt.Rows.Count;
                string[] headerArr = new string[]
                { "经销商名称","V2报量名称","快捷编码","账户名称","账户状态","所属区域","所属机构","所在省市","详细地址","属性","运营商属性","创建人","创建时间",
                };
                int[] colWidthArr = new int[] { 25, 12, 12, 12, 10, 15, 18, 25, 23, 10, 12, 15, 15, 20 };

                //建立空白工作簿
                HSSFWorkbook book = new HSSFWorkbook();
                if (rowsCount < ConstData.EXPORT_SHEET_LEN)
                    ExcelApi.ExcelExport(book, "sheet1", headerArr, colWidthArr, listDt, 0, rowsCount);
                else
                {
                    int page = rowsCount / ConstData.EXPORT_SHEET_LEN;
                    for (int i = 0; i < page; i++)
                    {
                        int start = i * ConstData.EXPORT_SHEET_LEN;
                        int end = (i * ConstData.EXPORT_SHEET_LEN) + ConstData.EXPORT_SHEET_LEN - 1;
                        string sheetName = "sheet" + (i + 1).ToString();

                        ExcelApi.ExcelExport(book, sheetName, headerArr, colWidthArr, listDt, start, end);
                    }
                    int lastCount = rowsCount % ConstData.EXPORT_SHEET_LEN;
                    string lastName = "sheet" + (page + 1).ToString();

                    ExcelApi.ExcelExport(book, lastName, headerArr, colWidthArr, listDt, rowsCount - lastCount, lastCount);
                }

                // 写入到客户端 
                MemoryStream ms = new MemoryStream();
                {
                    book.Write(ms);
                    ms.Seek(0, SeekOrigin.Begin);
                    return ms;
                }
            }
        }

        public string Import(List<daoben_distributor_info> distributorList)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (distributorList == null || distributorList.Count < 1)
                return "信息错误，操作失败!";
            DateTime now = DateTime.Now;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            //System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            //stopwatch.Start(); //  开始监视代码 delete
            List<string> dupList = distributorList.GroupBy(a => a.id).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            if (dupList != null && dupList.Count > 0)
                return "信息错误：导入表中经销商ID重复" + dupList.ToJson();
            
            daoben_ms_account accountInfo = new daoben_ms_account();
            List<daoben_ms_account> accountList = new List<daoben_ms_account>();
            daoben_distributor_info_history historyInfo = new daoben_distributor_info_history();
            List<daoben_distributor_info_history> historyList = new List<daoben_distributor_info_history>();

            string password = PasswordStorage.CreateHash("123456");
            foreach (var a in distributorList)
            {
                if (string.IsNullOrEmpty(a.id))
                    return "经销商“" + a.name + "”ID不能为空";               
                string id = a.id.Trim();
                
                accountInfo = new daoben_ms_account();
                accountInfo.account = id;
                accountInfo.employee_id = id;
                accountInfo.employee_name = a.name;
                accountInfo.employee_type = 1;
                accountInfo.role_id = ConstData.ROLE_ID_DISTRIBUTOR;
                accountInfo.role_name = "经销商";
                accountInfo.password = password;
                accountInfo.creator_id = LoginInfo.accountId;
                accountInfo.creator_name = LoginInfo.empName;
                accountInfo.reg_time = now;
                accountList.Add(accountInfo);

                historyInfo.main_id = id;
                historyInfo.company_id = a.company_id;
                historyInfo.company_id_parent = a.company_id_parent;
                historyInfo.company_linkname = a.company_linkname;
                historyInfo.area_l1_id = a.area_l1_id;
                historyInfo.area_l1_name = a.area_l1_name;
                historyInfo.area_l2_id = a.area_l2_id;
                historyInfo.area_l2_name = a.area_l2_name;
                historyInfo.status = 1;
                historyInfo.creator_job_history_id = LoginInfo.jobHistoryId;
                historyInfo.create_time = historyInfo.effect_date = now;
                historyList.Add(historyInfo);
            }

            //stopwatch.Stop(); //TODO delete  停止监视
            //TimeSpan timeSpan = stopwatch.Elapsed; //  获取总时间
            //double seconds = timeSpan.TotalSeconds;  //  秒数 delete

            using (var db = SugarDao.GetInstance())
            {
                db.CommandTimeOut = 300;
                try
                {
                    db.BeginTran();
                    if (distributorList != null && distributorList.Count > 25)
                        db.SqlBulkCopy(distributorList);
                    else if (distributorList != null && distributorList.Count > 0)
                        db.InsertRange(distributorList);

                    if (accountList.Count > 25)
                        db.SqlBulkCopy(accountList);
                    else
                        db.InsertRange(accountList);

                    if (historyList.Count > 25)
                        db.SqlBulkCopy(historyList);
                    else
                        db.InsertRange(historyList);
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
        /// 区域列表
        /// </summary>
        /// <returns></returns>
        public string GetAreaTree()
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
                    List<Area2Info> area2List = db.Queryable<daoben_org_area>()
                                .Where(a => a.type == 2 && (a.company_id_parent == myCompanyInfo.id || a.company_id == myCompanyInfo.id))
                                .Select<Area2Info>("id as area_l2_id, name as area_l2_name,parent_id as area_l1_id,parent_name as area_l1_name, company_id, company_name, company_id_parent, company_linkname, CONCAT(name, ' (',company_linkname, ' - ', parent_name, ')') as display_info").ToList();
                    if (area2List == null && area2List.Count() < 1)
                        return null;
                    List<CompanyAreaTree> companyAreaTree = area2List.GroupBy(c => c.company_id)
                            .Select(c1 => new CompanyAreaTree
                            {
                                company_id = c1.Key,
                                company_name = c1.First().company_name,
                                company_linkname = c1.First().company_linkname,
                                area_l1_list = (c1.GroupBy(a => a.area_l1_id).Select(a1 => new AreaL1Info
                                {
                                    area_l1_id = a1.Key,
                                    area_l1_name = a1.First().area_l1_name,
                                    area_l2_list = a1.ToList()
                                })).ToList()
                            }).ToList();

                    return companyAreaTree.ToJson();
                }
                catch (Exception ex)
                {
                    return "系统出错：" + ex.Message;
                }
            }
        }

        public string GetAllDistriId()
        {
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    List<string> distriId = db.Queryable<daoben_distributor_info>()
                            .Select<string>("id").ToList();
                    return distriId.ToJson();
                }
                catch (Exception ex)
                {
                    return "系统出错：" + ex.Message;
                }
            }
        }


        public decimal GetAvgBefore(string idStr)
        {
            if (string.IsNullOrEmpty(idStr))
                return 0;
            if (idStr.Last() == '|')
                idStr = idStr.Remove(idStr.Length - 1, 1);
            List<string> idList = idStr.Split('|').ToList();
            if (idList.Count < 1)
                return 0;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    DateTime now = DateTime.Now;
                    DateTime endTime = now.Date.AddDays(1 - now.Day);
                    DateTime startTime = endTime.AddMonths(-3);

                    int totalCount = db.Queryable<daoben_product_sn>()
                            .Where(a => idList.Contains(a.sale_distributor_id) && a.sale_type > 0 && a.sale_time < endTime && a.sale_time > startTime)
                            .Select<int>("COUNT(*)").SingleOrDefault();
                    return (decimal.Round(totalCount / 3, 2));
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
        }

    }
}
