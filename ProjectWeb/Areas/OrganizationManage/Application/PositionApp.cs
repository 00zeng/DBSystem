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
    public class PositionApp
    {
        public object GetList(Pagination pagination, daoben_org_position queryInfo)
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
                var qable = db.Queryable<daoben_org_position>();
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
                    if (queryInfo.dept_id > 0)
                        qable.Where(a => a.dept_id == queryInfo.dept_id);
                }
                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                                .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public daoben_org_position GetInfo(int id)
        {
            if (id < 1)
                return null;
            using (var db = SugarDao.GetInstance())
            {
                daoben_org_position positionInfo = db.Queryable<daoben_org_position>().InSingle(id);
                return positionInfo;
            }
        }

        public string Add(daoben_org_position addInfo)
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

            object companyDelibleObj = null;
            object deptDelibleObj = null;
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    daoben_org_company companyInfo = db.Queryable<daoben_org_company>().InSingle(addInfo.company_id);
                    if (companyInfo == null)
                        return "信息错误：指定所属机构不存在";
                    addInfo.company_linkname = companyInfo.link_name;
                    addInfo.company_id_parent = companyInfo.parent_id;
                    addInfo.company_category = companyInfo.category;
                    addInfo.company_name = companyInfo.name;
                    //if (companyInfo.delible)
                    //    companyDelibleObj = new { delible = false };

                    if (addInfo.dept_id < 1)
                    {
                        addInfo.dept_id = 0;
                        addInfo.dept_name = null;
                    }
                    else
                    {
                        daoben_org_dept deptInfo = db.Queryable<daoben_org_dept>().InSingle(addInfo.dept_id);
                        if (deptInfo == null)
                            return "信息错误：指定所属部门不存在";
                        addInfo.dept_name = deptInfo.name;
                        if (deptInfo.delible)
                            deptDelibleObj = new { delible = false };
                    }
                    bool isExist = db.Queryable<daoben_org_position>().Any(a => a.name == addInfo.name && a.company_id == addInfo.company_id && a.dept_id == addInfo.dept_id);
                    if (isExist)
                        return "信息错误：同部门下“" + addInfo.name + "”已存在";
                    db.CommandTimeOut = 30;
                    try
                    {
                        db.BeginTran();
                        db.DisableInsertColumns = new string[] { "id" };
                        db.Insert(addInfo);
                        if (companyDelibleObj != null)
                            db.Update<daoben_org_company>(companyDelibleObj, a => a.id == addInfo.company_id);
                        if (deptDelibleObj != null)
                            db.Update<daoben_org_dept>(deptDelibleObj, a => a.id == addInfo.dept_id);
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

        public string Edit(daoben_org_position editInfo)
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
                    daoben_org_position positionOrig = db.Queryable<daoben_org_position>().InSingle(editInfo.id);
                    if (positionOrig == null)
                        return "修改失败：职位不存在";
                    object obj = new
                    {
                        name = editInfo.name,
                        note = editInfo.note,
                    };
                    db.Update<daoben_org_position>(obj, a => a.id == editInfo.id);
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
                    daoben_org_position delInfo = db.Queryable<daoben_org_position>().InSingle(id);
                    if (delInfo == null)
                        return "删除失败：职位不存在";
                    if (!delInfo.delible)
                        return "删除失败：职位已使用，无法删除";
                    db.Delete<daoben_org_position>(a => a.id == id);
                    return "success";
                }
            }
            catch (Exception ex)
            {
                return "系统出错：" + ex.Message;
            }
        }

        /// <summary>
        /// 根据部门获取职位选择列表
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public List<IdNamePair> GetIdNameList(int companyId, bool subCompany, int deptId, bool office)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (companyId < 1)
                companyId = LoginInfo.companyInfo.id;   //不指定机构时获取登录人所属机构
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_org_position>();
                //if (deptId > 0)
                qable.Where(a => a.dept_id == deptId);//change by yajun

                if (subCompany) // 获取本及下属机构
                    qable.Where(a => (a.company_id_parent == companyId || a.company_id == companyId)).GroupBy(a => a.name);
                else // 只获取本机构
                    qable.Where(a => a.company_id == companyId);
                if (office) // 只获取行政人员的
                    qable.Where(a => a.position_type <= ConstData.POSITION_OFFICE_NORMAL);

                List<IdNamePair> pairList = qable.Select<IdNamePair>("id, name").ToList();
                return pairList;
            }
        }

        public string GetGradeList(int positionId, int grade_category)
        {
            using (var db = SugarDao.GetInstance())
            {
                if (positionId < 1)
                    return null;
                daoben_org_position positionInfo = db.Queryable<daoben_org_position>().InSingle(positionId);
                if (positionInfo == null)
                    return null;
                List<IdNamePair> pairList = new List<IdNamePair>();
                if (positionInfo.position_type == ConstData.POSITION_GUIDE)
                {
                    pairList.Add(new IdNamePair { id = "一星", name = "一星" });
                    pairList.Add(new IdNamePair { id = "二星", name = "二星" });
                    pairList.Add(new IdNamePair { id = "三星", name = "三星" });
                    pairList.Add(new IdNamePair { id = "四星", name = "四星" });
                    pairList.Add(new IdNamePair { id = "五星", name = "五星" });
                }
                else
                {
                    if (positionInfo.dept_id < 1) // 无所属部门
                        grade_category = 1;
                    pairList = db.Queryable<daoben_org_grade>()
                            .Where(a => a.company_id == positionInfo.company_id && a.grade_level == positionInfo.grade_level
                            && a.grade_category == grade_category)
                            .Select<IdNamePair>("id, grade as name").ToList();
                }

                object retInfo = new
                {
                    gradeList = pairList,
                    positionInfo = positionInfo,
                };
                return retInfo.ToJson();
            }
        }

    }
}
