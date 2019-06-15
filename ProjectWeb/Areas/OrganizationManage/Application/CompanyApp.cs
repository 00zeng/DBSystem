using Base.Code;
using MySqlSugar;
using ProjectShare.Database;
using ProjectShare.Models;
using ProjectShare.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ProjectWeb.Areas.OrganizationManage.Application
{
    public class CompanyApp
    {
        public object GetList(Pagination pagination, daoben_org_company queryInfo)
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
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_org_company>();
                if (myCompanyInfo != null)
                {
                    if (myCompanyInfo.category == "事业部")
                        qable.Where(a => (a.id == myCompanyInfo.parentId || a.id == myCompanyInfo.id || a.parent_id == myCompanyInfo.id));
                    else if (myCompanyInfo.category == "分公司")
                        qable.Where(a => (a.id == myCompanyInfo.parentId || a.id == myCompanyInfo.id));
                }
                if (queryInfo != null)
                {
                    if (!string.IsNullOrEmpty(queryInfo.name))
                        qable.Where(a => a.name.Contains(queryInfo.name));
                    if (!string.IsNullOrEmpty(queryInfo.parent_name))
                        qable.Where(a => a.parent_name.Contains(queryInfo.parent_name));
                    if (!string.IsNullOrEmpty(queryInfo.category))
                        qable.Where(a => a.category == queryInfo.category);
                }

                string listStr = qable.Where(a => a.status == 1)
                                .OrderBy(pagination.sidx + " " + pagination.sord)
                                .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public daoben_org_company GetInfo(int id)
        {
            if (id < 1)
                return null;
            using (var db = SugarDao.GetInstance())
            {
                daoben_org_company companyInfo = db.Queryable<daoben_org_company>().InSingle(id);
                return companyInfo;
            }
        }

        public string Add(daoben_org_company addInfo, List<daoben_org_grade> gradeList)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            if (addInfo == null || string.IsNullOrEmpty(addInfo.name))
                return "信息错误，操作失败!";

            string category = addInfo.category;
            if (addInfo.parent_id < 1)
            {
                if (category != "董事会")
                    return "信息错误：无上级的机构应该为董事会";
                addInfo.parent_id = 0;
                addInfo.parent_name = null;
                addInfo.link_name = addInfo.name;
            }
            daoben_org_company_history historyInfo = new daoben_org_company_history();
            historyInfo.name = addInfo.name;
            historyInfo.status = 1;
            historyInfo.creator_job_history_id = LoginInfo.jobHistoryId;
            historyInfo.creator_name = LoginInfo.empName;
            historyInfo.effect_date = now;
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    bool isExist = db.Queryable<daoben_org_company>().Any(a => a.name == addInfo.name && a.status == 1);
                    if (isExist)
                        return "信息错误：机构“" + addInfo.name + "”已存在";

                    if (addInfo.parent_id > 0)
                    {
                        daoben_org_company companyParent = db.Queryable<daoben_org_company>().InSingle(addInfo.parent_id);
                        if (companyParent == null)
                            return "信息错误：指定上级机构不存在";
                        addInfo.parent_name = companyParent.name;
                        if (category == "事业部")
                            addInfo.link_name = addInfo.name; // 非董事会的机构名称链不包含董事会
                        else if (category == "分公司")
                            addInfo.link_name = addInfo.parent_name + "-" + addInfo.name;
                        else
                            return "信息错误：机构类型出错";
                    }
                    addInfo.status = 1;
                    db.CommandTimeOut = 30;
                    try
                    {
                        db.BeginTran();
                        db.DisableInsertColumns = new string[] { "id" };
                        historyInfo.company_id = db.Insert(addInfo).ToInt();
                        db.Insert(historyInfo);
                        if (gradeList != null && gradeList.Count > 0)
                        {
                            gradeList.ForEach(a => a.company_id = historyInfo.company_id);
                            db.InsertRange(gradeList);
                        }
                        db.CommitTran();
                    }
                    catch (Exception ex)
                    {
                        db.RollbackTran();
                        return "系统出错：" + ex.Message;
                    }
                    if (category == "分公司")
                    {   // 只有分公司才有公版薪资
                        ThreadPool.QueueUserWorkItem(t =>
                        {
                            setPosSalary(LoginInfo, historyInfo.company_id);
                        });
                    }
                    return "success";
                }
            }
            catch (Exception ex)
            {
                return "系统出错：" + ex.Message;
            }
        }

        public string Edit(daoben_org_company editInfo)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (editInfo == null || editInfo.id < 1)
                return "信息错误，操作失败!";

            string category = editInfo.category;
            if (editInfo.parent_id < 1)
            {
                if (category != "董事会")
                    return "信息错误：机构类型出错";
                editInfo.link_name = editInfo.name;
            }
            else if (editInfo.parent_id > 0)
            {
                if (category == "事业部")
                    editInfo.link_name = editInfo.name; // 非董事会的机构名称链不包含董事会
                else if (category == "分公司")
                    editInfo.link_name = editInfo.parent_name + "-" + editInfo.name;
                else
                    return "信息错误：机构类型出错";
            }
            object companyObj = new
            {
                name = editInfo.name,
                link_name = editInfo.link_name,
                city = editInfo.city,
                city_code = editInfo.city_code,
                address = editInfo.address,
                contact_phone = editInfo.contact_phone,
                note = editInfo.note,
            };
            object upObj = new
            {
                company_name = editInfo.name,
                company_linkname = editInfo.link_name,
            };
            using (var db = SugarDao.GetInstance())
            {
                daoben_org_company origInfo = db.Queryable<daoben_org_company>()
                            .SingleOrDefault(a => a.id == editInfo.id && a.status == 1);
                if (origInfo == null)
                    return "修改失败：机构不存在";
                // 修改名称需保留历史记录                   
                if (editInfo.name != origInfo.name)
                {
                    DateTime now = DateTime.Now;
                    daoben_org_company_history historyInfo = new daoben_org_company_history();
                    historyInfo.name = editInfo.name;
                    historyInfo.status = 1;
                    historyInfo.creator_job_history_id = LoginInfo.jobHistoryId;
                    historyInfo.effect_date = now;
                    //将原历史记录设为失效
                    daoben_org_company_history origHistoryInfo = db.Queryable<daoben_org_company_history>()
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
                            db.Update<daoben_org_company_history>(inactiveObj, a => a.id == origHistoryInfo.id);
                        }
                        //插入新的历史记录
                        db.Insert(historyInfo);

                        //更新区域
                        db.Update<daoben_org_area>(upObj, a => a.company_id == editInfo.id);
                        db.Update<daoben_org_area_history>(upObj, a => a.company_id == editInfo.id && a.inactive == false);
                        //更新经销商
                        db.Update<daoben_distributor_info>(upObj, a => a.company_id == editInfo.id);
                        db.Update<daoben_distributor_info_history>(new { company_linkname = editInfo.link_name },
                                    a => a.company_id == editInfo.id && a.inactive == false);
                        db.CommitTran();
                    }
                    catch (Exception ex)
                    {
                        db.RollbackTran();
                        return "系统出错：" + ex.Message;
                    }
                }
                //更新分公司信息
                db.Update<daoben_org_company>(companyObj, a => a.id == editInfo.id);
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
            List<daoben_org_company_history> historyList = new List<daoben_org_company_history>();
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    for (int i = 0; i < idArray.Length; i++)
                    {
                        int id = idArray[i];
                        if (id < 1)
                            return "删除失败：ID不正确!";
                        daoben_org_company delInfo = db.Queryable<daoben_org_company>()
                                    .SingleOrDefault(a => a.id == id && a.status == 1);
                        if (delInfo == null)
                            return "删除失败：机构不存在";
                        if (db.Queryable<daoben_org_dept>().Any(a => a.company_id == id))
                            return "删除失败：该机构存在下属部门";

                        if (delInfo.category == "董事会" || delInfo.category == "事业部")
                        {
                            if (db.Queryable<daoben_org_company>().Any(a => a.parent_id == id && a.status == 1))
                                return "删除失败：该机构存在下属机构";
                        }
                        else
                        {
                            if (db.Queryable<daoben_org_area>().Any(a => a.company_id == id && a.status == 1))
                                return "删除失败：该机构存在下属区域";
                        }
                        daoben_org_company_history historyInfo = new daoben_org_company_history();
                        historyInfo.company_id = id;
                        historyInfo.name = delInfo.name;
                        historyInfo.status = -1;
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
                        db.Update<daoben_org_company_history>(inactiveObj, a => idList.Contains(a.company_id) && a.inactive == false);
                        //插入新的历史纪录
                        db.InsertRange(historyList);
                        //更新company表状态
                        db.Update<daoben_org_company>(new { status = -1 }, a => idList.Contains(a.id));
                        //db.Delete<daoben_org_grade>(a => a.company_id == id); // 职等信息不删除
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
        /// 获取机构选择列表
        /// </summary>
        /// <param name="category"></param>
        /// <param name="paramType">
        ///     0: 获取类型为category的机构列表
        ///     1：获取类型为category的机构的上级机构列表
        /// </param>
        /// <returns></returns>
        public List<IdNamePair> GetIdNameList(string category, int paramType)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            List<IdNamePair> pairList = new List<IdNamePair>();
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            if (paramType == 1)
            {
                if (category == "分公司")
                    category = "事业部";
                else if (category == "事业部")
                    category = "董事会";
                else
                    return pairList;    // 董事会 无上级
            }
            if (myCompanyInfo.category == category)
            {
                pairList.Add(new IdNamePair { id = myCompanyInfo.id.ToString(), name = myCompanyInfo.name });
                return pairList;
            }
            using (var db = SugarDao.GetInstance())
            {
                // 获取类型为category的机构列表
                var qable = db.Queryable<daoben_org_company>().Where(a => a.category == category && a.status == 1);
                if (myCompanyInfo.category == "分公司")
                    qable.Where(a => a.id == myCompanyInfo.id);
                else if (LoginInfo.companyInfo.category == "事业部")
                    qable.Where(a => a.parent_id == myCompanyInfo.id);

                pairList = qable.Select<IdNamePair>("id,name").ToList();
                return pairList;
            }
        }

        public List<IdNamePair> GetCategoryList()
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");

            List<IdNamePair> pairList = ConstData.OrgCategory();
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            if (myCompanyInfo == null || myCompanyInfo.id == 0)
                return pairList;
            int i = 0, count = pairList.Count;
            for (; i < count; i++)
            {
                if (myCompanyInfo.category == pairList.First().name)
                    break;
            }
            if (i == count || i == 0)
                return pairList;
            return pairList.GetRange(i, count - i);
        }

        /// <summary>
        /// 获取机构选择列表(link_name)
        /// </summary>
        /// <returns></returns>
        public string GetIdNameCategoryList(string category)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo; // 用于控制权限，禁止访问上级及其他同级

            string pairListStr = null;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_org_company>();
                if (!string.IsNullOrEmpty(category))
                    qable.Where(a => a.category == category);
                if (myCompanyInfo == null || myCompanyInfo.id == 0 || myCompanyInfo.category == "董事会")
                {
                    pairListStr = qable.Select("id,link_name as name,category").ToJson();
                    return pairListStr;
                }
                pairListStr = qable.Where(a => (a.parent_id == myCompanyInfo.id || a.id == myCompanyInfo.id)
                            && a.status == 1)
                            .Select("id,link_name as name,category").ToJson();
                return pairListStr;
            }
        }

        public KeyValuePair<string, string> GetAddr(int id)
        {
            using (var db = SugarDao.GetInstance())
            {
                KeyValuePair<string, string> pairInfo = db.Queryable<daoben_org_company>()
                           .Where(a => a.id == id).Select<KeyValuePair<string, string>>("city, address").SingleOrDefault();
                return pairInfo;
            }
        }


        public void setPosSalary(OperatorModel LoginInfo, int company_id)
        {
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_org_company companyInfo = db.Queryable<daoben_org_company>().SingleOrDefault(a => a.id == company_id);
                    if (companyInfo == null || companyInfo.category != "分公司")
                        return;
                    daoben_org_company parentCompany = db.Queryable<daoben_org_company>().SingleOrDefault(t => t.id == companyInfo.parent_id);
                    if (parentCompany == null || parentCompany.category != "事业部")
                        return;
                    //公版且覆盖
                    List<daoben_salary_position> origPosSalary = db.Queryable<daoben_salary_position>()
                            .Where(t => t.company_id_parent == parentCompany.id && t.is_template == 1).ToList();

                    //生效状态 当前已生效
                    origPosSalary = origPosSalary.Where(t => t.effect_status == 1).OrderByDescending(t => t.effect_date).ToList();

                    //职等 + 工龄工资
                    daoben_salary_position origGrade = origPosSalary.FirstOrDefault(t => t.category == 4);
                    daoben_salary_position origSeniority = origPosSalary.FirstOrDefault(t => t.category == 3);

                    List<daoben_salary_position> insertPosList = new List<daoben_salary_position>();
                    List<daoben_salary_position_other> insertOtherList = new List<daoben_salary_position_other>();
                    List<daoben_salary_position_seniority> insertSeniorityList = new List<daoben_salary_position_seniority>();
                    List<daoben_salary_position_grade> insertGradeList = new List<daoben_salary_position_grade>();

                    if (origSeniority != null)
                    {
                        //仿照origSeniority 生成工龄信息                        
                        daoben_salary_position_other otherInfo = db.Queryable<daoben_salary_position_other>()
                                .SingleOrDefault(t => t.salary_position_id == origSeniority.id);
                        List<daoben_salary_position_seniority> seniorityList = db.Queryable<daoben_salary_position_seniority>()
                                .Where(t => t.salary_position_id == origSeniority.id).ToList();
                        origSeniority.id = Common.GuId();
                        origSeniority.company_id = companyInfo.id;
                        origSeniority.company_name = companyInfo.name;
                        origSeniority.effect_date = (origSeniority.effect_date > DateTime.Now ? origSeniority.effect_date : DateTime.Now);
                        origSeniority.create_time = DateTime.Now;
                        origSeniority.creator_job_history_id = LoginInfo.jobHistoryId;
                        origSeniority.creator_name = LoginInfo.empName;
                        origSeniority.creator_position_name = LoginInfo.positionInfo.name;
                        if (otherInfo != null)
                        {
                            otherInfo.salary_position_id = origSeniority.id;
                            insertOtherList.Add(otherInfo);
                        }
                        foreach (var a in seniorityList)
                        {
                            a.salary_position_id = origSeniority.id;
                        }
                        insertSeniorityList.AddRange(seniorityList);

                        insertPosList.Add(origSeniority);
                    }
                    if (origGrade != null)
                    {
                        //仿照origGrade 生成职等信息
                        List<daoben_salary_position_grade> gradeList = db.Queryable<daoben_salary_position_grade>()
                                .Where(t => t.salary_position_id == origGrade.id).ToList();
                        origGrade.id = Common.GuId();
                        origGrade.company_id = companyInfo.id;
                        origGrade.company_name = companyInfo.name;
                        origGrade.effect_date = DateTime.Now;
                        origGrade.create_time = DateTime.Now;
                        origGrade.creator_job_history_id = LoginInfo.jobHistoryId;
                        origGrade.creator_name = LoginInfo.empName;
                        origGrade.creator_position_name = LoginInfo.positionInfo.name;
                        foreach (var a in gradeList)
                        {
                            a.salary_position_id = origGrade.id;
                        }
                        insertGradeList.AddRange(gradeList);
                        insertPosList.Add(origGrade);
                    }
                    //将来才会生效的公版配置  TODO 需要吗？
                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    if (insertPosList.Count() > 25)
                        db.SqlBulkCopy(insertPosList);
                    else if (insertPosList.Count() > 0)
                        db.InsertRange(insertPosList);

                    db.DisableInsertColumns = new string[] { "id" };
                    if (insertGradeList.Count() > 25)
                        db.SqlBulkCopy(insertGradeList);
                    else if (insertGradeList.Count() > 0)
                        db.InsertRange(insertGradeList);

                    if (insertOtherList.Count() > 25)
                        db.SqlBulkCopy(insertOtherList);
                    else if (insertOtherList.Count() > 0)
                        db.InsertRange(insertOtherList);

                    if (insertSeniorityList.Count() > 25)
                        db.SqlBulkCopy(insertSeniorityList);
                    else if (insertSeniorityList.Count() > 0)
                        db.InsertRange(insertSeniorityList);
                    db.CommitTran();
                }

                catch (Exception ex)
                {
                    db.RollbackTran();
                }
            }
        }



        public string GetOrgDistriTree()
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
                    //从董事会开始
                    daoben_org_company firstCompanyInfo = new daoben_org_company();
                    if (myCompanyInfo.category == "事业部")
                    {
                        firstCompanyInfo = db.Queryable<daoben_org_company>().FirstOrDefault(t => t.id == myCompanyInfo.parentId);
                    }
                    if (firstCompanyInfo.category != "董事会")
                        return null;
                    //事业部
                    List<daoben_org_company> company1List = db.Queryable<daoben_org_company>()
                            .Where(t => t.parent_id == firstCompanyInfo.id)
                            .Select("id,name,parent_id")
                            .ToList();
                    //分公司
                    List<daoben_org_company> company2List = db.Queryable<daoben_org_company>()
                            .JoinTable<daoben_org_company>((a, b) => a.parent_id == b.id)
                            .Where<daoben_org_company>((a, b) => b.parent_id == firstCompanyInfo.id)
                            .Select("a.id,a.name,a.parent_id")
                            .ToList();
                    //区域 职位 TODO
                    //经理区域
                    List<daoben_org_area> area1List = db.Queryable<daoben_org_area>()
                            .JoinTable<daoben_org_company>((a, b) => a.company_id_parent == b.id)
                            .Where<daoben_org_company>((a, b) => b.parent_id == firstCompanyInfo.id && a.type == 1)
                            .Select("a.id,a.name,a.company_id,a.company_id_parent")
                            .ToList();
                    //业务片区
                    List<daoben_org_area> area2List = db.Queryable<daoben_org_area>()
                            .JoinTable<daoben_org_company>((a, b) => a.company_id_parent == b.id)
                            .Where<daoben_org_company>((a, b) => b.parent_id == firstCompanyInfo.id && a.type == 2)
                            .Select("a.id,a.name,a.company_id,a.company_id_parent,a.parent_id")
                            .ToList();
                    //经销商
                    List<daoben_distributor_info> distriList = db.Queryable<daoben_distributor_info>()
                            .JoinTable<daoben_org_company>((a, b) => a.company_id_parent == b.id)
                            .Where<daoben_org_company>((a, b) => b.parent_id == firstCompanyInfo.id)
                            .Select("a.id,a.name,a.company_id,a.company_id_parent,a.area_l1_id,a.area_l2_id")
                            .ToList();
                    organizationTree firstTree = new organizationTree()
                    {
                        name = firstCompanyInfo.name,
                        value = "董事会",
                    };
                    if (company1List.Count() > 0)
                        firstTree.children = new List<organizationTree>();
                    foreach (var company1 in company1List)
                    {
                        organizationTree company1Tree = new organizationTree()
                        {
                            name = company1.name,
                            value = "事业部",
                        };
                        List<daoben_org_company> tempCompany2List = company2List
                                .Where(t => t.parent_id == company1.id).ToList();
                        if (tempCompany2List.Count() > 0)
                            company1Tree.children = new List<organizationTree>();
                        foreach (var company2 in tempCompany2List)
                        {
                            #region 分公司
                            organizationTree company2Tree = new organizationTree()
                            {
                                name = company2.name,
                                value = "分公司",
                            };
                            List<daoben_org_area> tempArea1List = area1List
                                    .Where(t => t.company_id == company2.id).ToList();
                            if (tempArea1List.Count() > 0)
                                company2Tree.children = new List<organizationTree>();
                            foreach (var area1 in tempArea1List)
                            {
                                #region 经理片区
                                organizationTree area1Tree = new organizationTree()
                                {
                                    name = area1.name,
                                    value = "经理片区",
                                };
                                List<daoben_org_area> tempArea2List = area2List
                                        .Where(t => t.parent_id == area1.id).ToList();
                                if (tempArea2List.Count() > 0)
                                    area1Tree.children = new List<organizationTree>();
                                foreach (var area2 in tempArea2List)
                                {
                                    organizationTree area2Tree = new organizationTree()
                                    {
                                        name = area2.name,
                                        value = "业务片区",
                                    };
                                    List<daoben_distributor_info> tempDistriList = distriList
                                            .Where(t => t.area_l2_id == area2.id).ToList();
                                    if (tempDistriList.Count() > 0)
                                        area2Tree.children = new List<organizationTree>();
                                    foreach (var distri in tempDistriList)
                                    {
                                        organizationTree distriTree = new organizationTree()
                                        {
                                            name = distri.name,
                                            value = "门店",
                                        };
                                        area2Tree.children.Add(distriTree);
                                    }
                                    area1Tree.children.Add(area2Tree);
                                }
                                #endregion
                                company2Tree.children.Add(area1Tree);
                            }
                            #endregion
                            company1Tree.children.Add(company2Tree);
                        }
                        firstTree.children.Add(company1Tree);
                    }
                    return firstTree.ToJson();
                }
                catch (Exception ex)
                {
                    return "系统出错：" + ex.Message;
                }
            }
        }

        public string GetOrgDeptTree()
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
                    //从董事会开始
                    daoben_org_company firstCompanyInfo = new daoben_org_company();
                    if (myCompanyInfo.category == "事业部")
                    {
                        firstCompanyInfo = db.Queryable<daoben_org_company>().FirstOrDefault(t => t.id == myCompanyInfo.parentId);
                    }
                    if (firstCompanyInfo.category != "董事会")
                        return null;
                    //事业部
                    List<daoben_org_company> company1List = db.Queryable<daoben_org_company>()
                            .Where(t => t.parent_id == firstCompanyInfo.id)
                            .Select("id,name,parent_id")
                            .ToList();
                    //分公司
                    List<daoben_org_company> company2List = db.Queryable<daoben_org_company>()
                            .JoinTable<daoben_org_company>((a, b) => a.parent_id == b.id)
                            .Where<daoben_org_company>((a, b) => b.parent_id == firstCompanyInfo.id)
                            .Select("a.id,a.name,a.parent_id")
                            .ToList();
                    return null;//暂时不作处理
#if false
                    ////董事会职位
                    //List<daoben_org_position> posList = db.Queryable<daoben_org_position>()
                    //        .Where(t => t.company_id == firstCompanyInfo.id).ToList();
                    ////事业部职位
                    //List<daoben_org_position> pos1List = db.Queryable<daoben_org_position>()
                    //        .JoinTable<daoben_org_company>((a, b) => a.company_id == b.id)
                    //        .Where<daoben_org_company>((a, b) => b.parent_id == firstCompanyInfo.id).ToList();
                    ////分公司职位
                    //List<daoben_org_position> pos2List = db.Queryable<daoben_org_position>()
                    //        .JoinTable<daoben_org_company>((a, b) => a.company_id_parent == b.id)
                    //        .Where<daoben_org_company>((a, b) => b.parent_id == firstCompanyInfo.id).ToList();
                    //organizationTree firstTree = new organizationTree()
                    //{
                    //    name = firstCompanyInfo.name,
                    //    value = 1,
                    //};
                    //if (company1List.Count() > 0 || posList.Count()>0)
                    //    firstTree.children = new List<organizationTree>();
                    //foreach (var pos in posList)
                    //{
                    //    organizationTree posTree = new organizationTree()
                    //    {
                    //        name = pos.name,
                    //        value = 11,
                    //    };
                    //}
                    //foreach (var company1 in company1List)
                    //{
                    //    organizationTree company1Tree = new organizationTree()
                    //    {
                    //        name = company1.name,
                    //        value = 2,
                    //    };
                    //    List<daoben_org_company> tempCompany2List = company2List
                    //            .Where(t => t.parent_id == company1.id).ToList();
                    //    if (tempCompany2List.Count() > 0 || pos)
                    //        company1Tree.children = new List<organizationTree>();
                    //    foreach (var company2 in tempCompany2List)
                    //    {
                    //        #region 分公司
                    //        organizationTree company2Tree = new organizationTree()
                    //        {
                    //            name = company2.name,
                    //            value = 3,
                    //        };
                    //        List<daoben_org_area> tempArea1List = area1List
                    //                .Where(t => t.company_id == company2.id).ToList();
                    //        if (tempArea1List.Count() > 0)
                    //            company2Tree.children = new List<organizationTree>();
                    //        foreach (var area1 in tempArea1List)
                    //        {
                    //            #region 经理片区
                    //            organizationTree area1Tree = new organizationTree()
                    //            {
                    //                name = area1.name,
                    //                value = 12,
                    //            };
                    //            List<daoben_org_area> tempArea2List = area2List
                    //                    .Where(t => t.parent_id == area1.id).ToList();
                    //            if (tempArea2List.Count() > 0)
                    //                area1Tree.children = new List<organizationTree>();
                    //            foreach (var area2 in tempArea2List)
                    //            {
                    //                organizationTree area2Tree = new organizationTree()
                    //                {
                    //                    name = area2.name,
                    //                    value = 13,
                    //                };
                    //                List<daoben_distributor_info> tempDistriList = distriList
                    //                        .Where(t => t.area_l2_id == area2.id).ToList();
                    //                if (tempDistriList.Count() > 0)
                    //                    area2Tree.children = new List<organizationTree>();
                    //                foreach (var distri in tempDistriList)
                    //                {
                    //                    organizationTree distriTree = new organizationTree()
                    //                    {
                    //                        name = distri.name,
                    //                        value = 21,
                    //                    };
                    //                    area2Tree.children.Add(distriTree);
                    //                }
                    //                area1Tree.children.Add(area2Tree);
                    //            }
                    //            #endregion
                    //            company2Tree.children.Add(area1Tree);
                    //        }
                    //        #endregion
                    //        company1Tree.children.Add(company2Tree);
                    //    }
                    //    firstTree.children.Add(company1Tree);
                    //}
                    //return firstTree.ToJson();
#endif
                }
                catch (Exception ex)
                {
                    return "系统出错：" + ex.Message;
                }
            }
        }
    }
}
