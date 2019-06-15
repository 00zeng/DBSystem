using Base.Code;
using MySqlSugar;
using ProjectShare.Database;
using ProjectShare.Models;
using ProjectShare.Process;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectWeb.Areas.OrganizationManage.Application
{

    public class DeptApp
    {

        public object GetList(Pagination pagination, daoben_org_dept queryInfo)
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
            using (var db = SugarDao.GetInstance())
            {
                //获取数据
                var qable = db.Queryable<daoben_org_dept>();

                if (myCompanyInfo != null)
                {
                    if (myCompanyInfo.category == "事业部") // 查看本机构及下属机构的所有部门/区域
                        qable.Where(a => (a.company_id == myCompanyInfo.id || a.company_id_parent == myCompanyInfo.id));
                    else if (myCompanyInfo.category == "分公司") // 查看本机构下所有部门/区域
                        qable.Where(a => (a.company_id == myCompanyInfo.id));
                }

                if (queryInfo != null)
                {
                    if (!string.IsNullOrEmpty(queryInfo.name))
                        qable.Where(a => a.name.Contains(queryInfo.name));
                    if (queryInfo.company_id > 0)
                        qable.Where(a => a.company_id == queryInfo.company_id);
                }

                List<daoben_org_dept> list = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                                .ToPageList(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                return list;
            }
        }

        public daoben_org_dept GetInfo(int id)
        {
            if (id < 1)
                return null;
            using (var db = SugarDao.GetInstance())
            {
                daoben_org_dept deptInfo = db.Queryable<daoben_org_dept>().InSingle(id);
                return deptInfo;
            }
        }

        public string Add(daoben_org_dept addInfo, string positionName, daoben_org_position positionInfo)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");

            if (addInfo == null || string.IsNullOrEmpty(addInfo.name))
                return "信息错误，操作失败!";
            if (addInfo.company_id < 1)
                return "信息错误：无所属机构";
            addInfo.creator_job_history_id = LoginInfo.jobHistoryId;
            addInfo.creator_name = LoginInfo.empName;
            addInfo.create_time = DateTime.Now;
            addInfo.delible = true; // 初始为可删除，一旦被使用则不可删除

            if (addInfo.parent_id < 1)
            {
                addInfo.parent_id = 0;
                addInfo.parent_name = null;
            }

            object companyDelibleObj = null;
            object deptDelibleObj = null;
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    bool isExist = db.Queryable<daoben_org_dept>()
                            .Any(a => a.company_id == addInfo.company_id && a.name == addInfo.name && a.grade_category == addInfo.grade_category);
                    if (isExist)
                        return "信息错误：“" + addInfo.name + "”已存在";
                    daoben_org_company companyInfo = db.Queryable<daoben_org_company>().InSingle(addInfo.company_id);
                    if (companyInfo == null)
                        return "信息错误：指定所属机构不存在";
                    //if (companyInfo.delible)
                    //    companyDelibleObj = new { delible = false };

                    addInfo.company_linkname = companyInfo.link_name;
                    addInfo.company_name = companyInfo.name;
                    addInfo.company_id_parent = companyInfo.parent_id;
                    if (addInfo.grade_category < 0 && addInfo.grade_category > 4)
                        return "信息错误：部门类型不正确";
                    if (addInfo.parent_id < 1)
                    {
                        addInfo.parent_id = 0;
                        addInfo.parent_name = null;
                    }
                    else
                    {
                        daoben_org_dept deptParent = db.Queryable<daoben_org_dept>().InSingle(addInfo.parent_id);
                        if (deptParent == null)
                            return "信息错误：指定上级部门不存在";
                        addInfo.parent_name = deptParent.name;
                        if (deptParent.delible)
                            deptDelibleObj = new { delible = false };

                    }
                    daoben_org_position addPosition = null;
                    if (!string.IsNullOrEmpty(positionName))
                    {
                        //新增机构部门职位
                        addPosition = new daoben_org_position
                        {
                            name = positionName,
                            grade_level = positionInfo.grade_level,
                            grade_level_display = positionInfo.grade_level_display,
                            position_type = positionInfo.position_type,
                            company_id = addInfo.company_id,
                            company_linkname = addInfo.company_linkname,
                            company_id_parent = addInfo.company_id_parent,
                            company_name = addInfo.company_name,
                            company_category=companyInfo.category,
                            dept_name = addInfo.name,
                            creator_job_history_id = LoginInfo.jobHistoryId,
                            creator_name = LoginInfo.empName,
                            create_time = DateTime.Now,
                            delible = false,
                        };
                    }
                    db.CommandTimeOut = 30;
                    try
                    {
                        db.BeginTran();
                        db.DisableInsertColumns = new string[] { "id" };
                        addInfo.id = db.Insert(addInfo).ToInt();
                        if (companyDelibleObj != null)
                            db.Update<daoben_org_company>(companyDelibleObj, a => a.id == addInfo.company_id);
                        if (deptDelibleObj != null)
                            db.Update<daoben_org_dept>(deptDelibleObj, a => a.id == addInfo.parent_id);
                        if (addPosition != null)
                        {
                            addPosition.dept_id = addInfo.id;
                            db.Insert(addPosition);
                        }
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

        public string Edit(daoben_org_dept editInfo)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");

            if (editInfo == null || editInfo.id < 1)
                return "信息错误，操作失败!";
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    daoben_org_dept deptAreaOrig = db.Queryable<daoben_org_dept>().InSingle(editInfo.id);
                    if (deptAreaOrig == null)
                        return "修改失败：部门/区域不存在";
                    object obj = new
                    {
                        city = editInfo.city,
                        city_code = editInfo.city_code,
                        address = editInfo.address,
                        contact_phone = editInfo.contact_phone,
                        note = editInfo.note,
                    };
                    db.Update<daoben_org_dept>(obj, a => a.id == editInfo.id);
                    return "success";
                }
            }
            catch (Exception ex)
            {
                return "系统出错：" + ex.Message;
            }
        }

        public string Delete(int id)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (id < 1)
                return "信息错误，操作失败!";
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    daoben_org_dept delInfo = db.Queryable<daoben_org_dept>().InSingle(id);
                    if (delInfo == null)
                        return "删除失败：部门不存在";
                    if (!delInfo.delible)
                        return "删除失败：部门已使用，无法删除";
                    db.Delete<daoben_org_dept>(a => a.id == id);
                    db.Delete<daoben_org_position>(a => a.dept_id == id);
                    return "success";
                }
            }
            catch (Exception ex)
            {
                return "系统出错：" + ex.Message;
            }
        }

        /// <summary>
        /// 根据机构获取部门/区域选择列表
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="type">0: 部门；1：区域</param>
        /// <returns></returns>
        public List<IdNamePair> GetIdNameList(int companyId)
        {
            if (companyId < 0)
                return null;
            if (companyId == 0)
            {
                OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
                if (LoginInfo == null)
                    throw new Exception("用户登陆过期，请重新登录");
                companyId = LoginInfo.companyInfo.id;
            }
            using (var db = SugarDao.GetInstance())
            {

                List<IdNamePair> pairList = db.Queryable<daoben_org_dept>()
                            .Where(a => a.company_id == companyId)
                            .Select<IdNamePair>("id, name").ToList();

                return pairList;
            }
        }

        public string GetDeptAddrList(int companyId)
        {
            using (var db = SugarDao.GetInstance())
            {
                if (companyId < 1)
                    return null;
                string getListStr = db.Queryable<daoben_org_dept>()
                            .Where(a => a.company_id == companyId)
                            .Select("id, name,concat(city, ' ', address) as addr_str, grade_category").ToJson();

                return getListStr;
            }
        }
    }
}
