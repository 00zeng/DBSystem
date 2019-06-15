using Base.Code;
using MySqlSugar;
using ProjectShare.Database;
using ProjectShare.Models;
using ProjectShare.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ProjectWeb.Areas.OrganizationManage.Application
{
    public class AreaApp
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryInfo"></param>
        /// <param name="type">0: 部门；1：区域</param>
        /// <returns></returns>
        public object GetAreaList(Pagination pagination, daoben_org_area queryInfo)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");

            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "id" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            using (var db = SugarDao.GetInstance())
            {
                //获取数据
                var qable = db.Queryable<daoben_org_area>().Where(a => a.type == queryInfo.type);
                if (myCompanyInfo != null)
                {
                    if (myCompanyInfo.category == "事业部") // 查看本机构及下属机构的所有部门/区域
                        qable.Where(a => (a.company_id_parent == myCompanyInfo.id && a.status == 1));
                    else if (myCompanyInfo.category == "分公司") // 查看本机构下所有部门/区域
                        qable.Where(a => (a.company_id == myCompanyInfo.id && a.status == 1));
                }

                if (queryInfo != null)
                {
                    if (!string.IsNullOrEmpty(queryInfo.name))
                        qable.Where(a => a.name.Contains(queryInfo.name));
                    if (queryInfo.type > 0)
                        qable.Where(a => a.type == queryInfo.type);
                    if (queryInfo.company_id > 0)
                        qable.Where(a => a.company_id == queryInfo.company_id);
                }

                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                                .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public daoben_org_area GetInfo(int id)
        {
            if (id < 1)
                return null;
            using (var db = SugarDao.GetInstance())
            {
                daoben_org_area deptAreaInfo = db.Queryable<daoben_org_area>().InSingle(id);
                return deptAreaInfo;
            }
        }

        public string Add(daoben_org_area addInfo)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");

            if (addInfo == null || string.IsNullOrEmpty(addInfo.name))
                return "信息错误，操作失败!";
            if (addInfo.company_id < 1)
                return "信息错误：无所属机构";
            if (!(addInfo.type == 1 || addInfo.type == 2))
                return "信息错误：区域类型错误";

            //经理片区
            if (addInfo.parent_id < 1)
            {
                if (addInfo.type != 1)
                    return "信息错误：无上级区域的应为经理片区";
                addInfo.parent_id = 0;
                addInfo.parent_name = null;
            }
            daoben_org_area_history historyInfo = new daoben_org_area_history();
            historyInfo.name = addInfo.name;
            historyInfo.status = 1;
            historyInfo.type = addInfo.type;
            historyInfo.creator_job_history_id = LoginInfo.jobHistoryId;
            historyInfo.creator_name = LoginInfo.empName;
            historyInfo.effect_date = DateTime.Now;

            object companyDelibleObj = null;
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    bool isExist = db.Queryable<daoben_org_area>()
                            .Any(a => a.company_id == addInfo.company_id && a.name == addInfo.name && a.status == 1 && a.type == addInfo.type);
                    if (isExist)
                        return "信息错误：“" + addInfo.name + "”已存在";
                    daoben_org_company companyInfo = db.Queryable<daoben_org_company>().InSingle(addInfo.company_id);
                    if (companyInfo == null)
                        return "信息错误：指定所属机构不存在";

                    addInfo.company_name = companyInfo.name;
                    addInfo.company_linkname = companyInfo.link_name;
                    addInfo.company_id_parent = companyInfo.parent_id;
                    addInfo.status = 1;
                    //业务片区
                    if (addInfo.parent_id > 0)
                    {
                        daoben_org_area areaParent = db.Queryable<daoben_org_area>().InSingle(addInfo.parent_id);
                        if (areaParent == null)
                            return "信息错误：指定上级部门不存在";
                        addInfo.parent_name = areaParent.name;
                        historyInfo.parent_id = addInfo.parent_id;
                    }

                    db.CommandTimeOut = 30;
                    try
                    {
                        db.BeginTran();
                        db.DisableInsertColumns = new string[] { "id" };
                        addInfo.id = db.Insert(addInfo).ToInt();
                        historyInfo.company_id = addInfo.company_id;
                        historyInfo.area_id = addInfo.id;
                        db.Insert(historyInfo);
                        if (companyDelibleObj != null)
                            db.Update<daoben_org_company>(companyDelibleObj, a => a.id == addInfo.company_id);
                        //if (addPosition != null)
                        //{
                        //    addPosition.dept_id = addInfo.id;
                        //    db.Insert(addPosition);
                        //}
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
        public string Edit(daoben_org_area editInfo)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (editInfo == null || editInfo.id < 1)
                return "信息错误，操作失败!";

            object areaObj = new
            {
                name = editInfo.name,
                city = editInfo.city,
                city_code = editInfo.city_code,
                address = editInfo.address,
                note = editInfo.note,
            };

            using (var db = SugarDao.GetInstance())
            {
                daoben_org_area origInfo = db.Queryable<daoben_org_area>()
                            .SingleOrDefault(a => a.id == editInfo.id && a.status == 1);
                if (origInfo == null)
                    return "修改失败：区域不存在";
                // 修改区域名称需保留历史记录
                if (editInfo.name != origInfo.name)
                {
                    DateTime now = DateTime.Now;
                    daoben_org_area_history historyInfo = new daoben_org_area_history();
                    historyInfo.name = editInfo.name;
                    historyInfo.status = 1;
                    historyInfo.type = editInfo.type;
                    historyInfo.creator_job_history_id = LoginInfo.jobHistoryId;
                    historyInfo.creator_name = LoginInfo.empName;
                    historyInfo.effect_date = now;
                    //将原历史记录设为失效
                    daoben_org_area_history origHistoryInfo = db.Queryable<daoben_org_area_history>()
                                .Where(a => a.company_id == editInfo.id && a.inactive == false).SingleOrDefault();
                    db.CommandTimeOut = 30;
                    try
                    {
                        db.BeginTran();
                        if (origHistoryInfo != null)
                        {
                            object inactiveObj = new
                            {
                                inactive = true,
                                inactive_job_history_id = LoginInfo.jobHistoryId,
                                inactive_time = now
                            };
                            db.Update<daoben_org_area_history>(inactiveObj, a => a.id == origHistoryInfo.id);
                        }
                        //插入新的历史记录
                        db.Insert(historyInfo);

                        //更新经销商的区域名称
                        if (editInfo.type == 1)
                        {
                            db.Update<daoben_distributor_info>(new { area_l1_name = editInfo.name }, a => a.area_l1_id == editInfo.id);
                            db.Update<daoben_distributor_info_history>(new { area_l1_name = editInfo.name },
                                a => a.area_l1_id == editInfo.id && a.inactive == false);
                        }
                        else if (editInfo.type == 2)
                        {
                            db.Update<daoben_distributor_info>(new { area_l2_name = editInfo.name }, a => a.area_l2_id == editInfo.id);
                            db.Update<daoben_distributor_info_history>(new { area_l2_name = editInfo.name },
                                a => a.area_l2_id == editInfo.id && a.inactive == false);
                        }
                        db.CommitTran();
                    }
                    catch (Exception ex)
                    {
                        db.RollbackTran();
                        return "系统出错：" + ex.Message;
                    }
                }
                //更新区域信息
                db.Update<daoben_org_area>(areaObj, a => a.id == editInfo.id);
            }
            return "success";
        }
        public string Delete(int[] idArray, DateTime? effect_date = null)//批量删除
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (idArray == null || idArray.Length < 1)
                return "success";

            DateTime now = DateTime.Now;
            if (effect_date == null)
                effect_date = now;
            List<daoben_org_area_history> historyList = new List<daoben_org_area_history>();
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    for (int i = 0; i < idArray.Length; i++)
                    {
                        int id = idArray[i];
                        if (id < 1)
                            return "删除失败：ID不正确!";
                        daoben_org_area delInfo = db.Queryable<daoben_org_area>()
                                    .SingleOrDefault(a => a.id == id && a.status == 1);
                        if (delInfo == null)
                            return "删除失败：该区域不存在";

                        if (delInfo.type == 1) //经理片区
                        {
                            if (db.Queryable<daoben_org_area>().Any(a => a.parent_id == id && a.status == 1))
                                return "删除失败：该区域存在业务片区";
                        }
                        else if (delInfo.type == 2)//业务片区
                        {
                            if (db.Queryable<daoben_hr_emp_job>().Any(a => a.area_l2_id == id && a.status > -1))
                                return "删除失败：该业务片区存在业务员或业务经理";

                            if (db.Queryable<daoben_distributor_info>().Any(a => a.area_l2_id == id && a.inactive == false))
                                return "删除失败：该业务片区存在经销商";
                        }

                        daoben_org_area_history historyInfo = new daoben_org_area_history();
                        historyInfo.area_id = id;
                        historyInfo.company_id = delInfo.company_id;
                        historyInfo.parent_id = delInfo.parent_id;
                        historyInfo.name = delInfo.name;
                        historyInfo.status = -1;
                        historyInfo.type = delInfo.type;
                        historyInfo.creator_job_history_id = LoginInfo.jobHistoryId;
                        historyInfo.creator_name = LoginInfo.empName;
                        historyInfo.effect_date = effect_date.ToDate();
                        historyList.Add(historyInfo);
                    }
                    List<int> idList = idArray.ToList<int>();
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
                        db.Update<daoben_org_area_history>(inactiveObj, a => idList.Contains(a.area_id) && a.inactive == false);
                        //插入新的历史纪录
                        db.InsertRange(historyList);
                        //更新区域表状态
                        db.Update<daoben_org_area>(new { status = -1 }, a => idList.Contains(a.id));
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

        public string AjustL1(int[] idArray, int company_id, DateTime? effect_date = null)//批量调区-经理片区
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (idArray == null || idArray.Length < 1)
                return "success";

            DateTime now = DateTime.Now;
            if (effect_date == null)
                effect_date = now;
            List<daoben_org_area_history> historyList = new List<daoben_org_area_history>();
            try
            {
                using (var db = SugarDao.GetInstance())
                {

                    daoben_org_company companyInfo = db.Queryable<daoben_org_company>()
                                  .SingleOrDefault(a => a.id == company_id && a.status == 1);
                    if (companyInfo == null)
                        return "调区失败：该分公司不存在";
                    for (int i = 0; i < idArray.Length; i++)
                    {
                        int id = idArray[i];
                        if (id < 1)
                            return "调区失败：ID不正确!";
                        daoben_org_area ajustInfo = db.Queryable<daoben_org_area>()
                                                        .SingleOrDefault(a => a.id == id && a.status == 1);
                        if (ajustInfo == null)
                            return "调区失败：该经理片区不存在";
                        if (ajustInfo.company_id == company_id)
                            continue;
                        daoben_org_area_history historyInfo = new daoben_org_area_history();
                        historyInfo.area_id = id;
                        historyInfo.company_id = company_id;
                        historyInfo.parent_id = 0;
                        historyInfo.name = ajustInfo.name;
                        historyInfo.status = 1;
                        historyInfo.type = ajustInfo.type;
                        historyInfo.creator_job_history_id = LoginInfo.jobHistoryId;
                        historyInfo.creator_name = LoginInfo.empName;
                        historyInfo.effect_date = effect_date.ToDate();
                        historyList.Add(historyInfo);
                    }
                    List<int> idList = idArray.ToList<int>();
                    object inactiveObj = new
                    {
                        inactive = true,
                        inactive_job_history_id = LoginInfo.jobHistoryId,
                        inactive_time = now
                    };
                    object ajustObj = new
                    {
                        company_id = company_id,
                        company_name = companyInfo.name,
                        company_id_parent = companyInfo.parent_id,
                        company_linkname = companyInfo.link_name,
                    };
                    db.CommandTimeOut = 30;
                    try
                    {
                        db.BeginTran();
                        //将原历史记录设为失效
                        db.Update<daoben_org_area_history>(inactiveObj, a => idList.Contains(a.area_id) && a.inactive == false);
                        //插入新的历史纪录
                        db.InsertRange(historyList);
                        //更新区域表状态
                        db.Update<daoben_org_area>(ajustObj, a => idList.Contains(a.id));
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

        public string AjustL2(int[] idArray, int company_id, int area_l1_id, DateTime? effect_date = null)//批量调区-业务片区
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (idArray == null || idArray.Length < 1)
                return "success";

            DateTime now = DateTime.Now;
            if (effect_date == null)
                effect_date = now;
            List<daoben_org_area_history> historyList = new List<daoben_org_area_history>();
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    //判断经理片区和分公司是否存在
                    daoben_org_area areaInfo = db.Queryable<daoben_org_area>()
                                   .SingleOrDefault(a => a.id == area_l1_id && a.status == 1);
                    if (areaInfo == null)
                        return "调区失败：该经理片区不存在";
                    daoben_org_company companyInfo = db.Queryable<daoben_org_company>()
                                  .SingleOrDefault(a => a.id == company_id && a.status == 1);
                    if (companyInfo == null)
                        return "调区失败：该分公司不存在";

                    for (int i = 0; i < idArray.Length; i++)
                    {
                        int id = idArray[i];
                        if (id < 1)
                            return "调区失败：ID不正确!";
                        daoben_org_area ajustInfo = db.Queryable<daoben_org_area>()
                                                         .SingleOrDefault(a => a.id == id && a.status == 1);
                        if (ajustInfo == null)
                            return "调区失败：该业务片区不存在";
                        if (ajustInfo.company_id == company_id && ajustInfo.parent_id == area_l1_id)
                            continue;
                        daoben_org_area_history historyInfo = new daoben_org_area_history();
                        historyInfo.area_id = id;
                        historyInfo.company_id = company_id;
                        historyInfo.parent_id = area_l1_id;
                        historyInfo.name = ajustInfo.name;
                        historyInfo.status = 1;
                        historyInfo.type = ajustInfo.type;
                        historyInfo.creator_job_history_id = LoginInfo.jobHistoryId;
                        historyInfo.creator_name = LoginInfo.empName;
                        historyInfo.effect_date = effect_date.ToDate();
                        historyList.Add(historyInfo);
                    }
                    List<int> idList = idArray.ToList<int>();
                    object inactiveObj = new
                    {
                        inactive = true,
                        inactive_job_history_id = LoginInfo.jobHistoryId,
                        inactive_time = now
                    };
                    object ajustObj = new
                    {
                        company_id = company_id,
                        company_name = companyInfo.name,
                        company_id_parent = companyInfo.parent_id,
                        company_linkname = companyInfo.link_name,
                        parent_id = area_l1_id,
                        parent_name = areaInfo.name,
                    };
                    db.CommandTimeOut = 30;
                    try
                    {
                        db.BeginTran();
                        //将原历史记录设为失效
                        db.Update<daoben_org_area_history>(inactiveObj, a => idList.Contains(a.area_id) && a.inactive == false);
                        //插入新的历史纪录
                        db.InsertRange(historyList);
                        //更新区域表信息
                        db.Update<daoben_org_area>(ajustObj, a => idList.Contains(a.id));
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

        /// <summary>
        /// 根据机构获取区域选择列表
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public List<IdNamePair> GetIdNameList(int companyId)
        {
            if (companyId < 0)
                return null;
            else if (companyId == 0)
            {
                OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
                if (LoginInfo == null)
                    throw new Exception("用户登陆过期，请重新登录");
                companyId = LoginInfo.companyInfo.id;
            }
            using (var db = SugarDao.GetInstance())
            {

                List<IdNamePair> pairList = db.Queryable<daoben_org_area>()
                            .Where(a => a.company_id == companyId)
                            .Select<IdNamePair>("id, name").ToList();
                return pairList;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">所查找信息的上一级ID</param>
        /// <param name="isSalesArea">true: 业务片区；false：经理片区</param>
        /// <returns></returns>
        public List<IdNamePair> GetAreaIdNameList(int id, bool isSalesArea)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            List<IdNamePair> pairList = null;
            string whereStrId = null, whereStrCompany = null;

            if (myCompanyInfo.category == "分公司")
                whereStrCompany = string.Format(" company_id='{0}'", myCompanyInfo.id);
            else if (myCompanyInfo.category == "事业部")
                whereStrCompany = string.Format(" company_id_parent='{0}'", myCompanyInfo.id);
            else
                whereStrCompany = "";
            using (var db = SugarDao.GetInstance())
            {

                if (isSalesArea) //业务片区
                {
                    if (id <= 0)
                        id = myPositionInfo.areaL1Id;
                    if (id <= 0)    // 说明登陆人无经理片区
                        whereStrId = whereStrCompany;
                    else
                        whereStrId = " parent_id=" + id.ToString();
                    pairList = db.Queryable<daoben_org_area>()
                                .Where(whereStrId)
                                .Where(a => a.type == 2 && a.status==1)
                                .Select<IdNamePair>("id, name").ToList();
                }
                else  //经理片区
                {
                    var qable = db.Queryable<daoben_org_area>()
                        .Where(a => a.type == 1 && a.status == 1);                  
                    if (id <= 0)  
                        qable.Where(whereStrCompany);
                    else
                        qable.Where(a => a.company_id == id);
                    pairList = qable.Select<IdNamePair>("id, name").ToList();
                }
                return pairList;
            }
        }


        public object GetAddrStr(int id)
        {
            using (var db = SugarDao.GetInstance())
            {
                string addrStr = db.Queryable<daoben_org_area>()
                           .Where(a => a.id == id).Select<string>("concat(city, address) as addr_str").SingleOrDefault();
                return new { addr_str = addrStr };
            }
        }


    }
}
