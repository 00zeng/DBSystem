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
using Base.Code.Security;
using System.Text;
using System.Threading;
using System.IO;
using NPOI.HSSF.UserModel;

namespace ProjectWeb.Areas.HumanResource.Application
{
    public class EmployeeManageApp
    {
        MsAccountApp msAccount = new MsAccountApp();
        public object GetList(Pagination pagination, daoben_hr_emp_job queryInfo, string name, QueryTime queryTime)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");

            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.company_id" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "asc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_hr_emp_job>().JoinTable<daoben_hr_emp_info>((a, b) => a.id == b.id)
                    .JoinTable<daoben_ms_account>((a, c) => a.id == c.employee_id)
                    .Where(a => a.position_id > 0);

                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIAL || LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER)
                    {   // TODO：临时用于ly-cw测试，放开权限
                        qable.Where(a => (a.company_id_parent == myCompanyInfo.id || a.company_id == myCompanyInfo.id));
                    }
                    else if (myPositionInfo.positionType == ConstData.POSITION_GM1
                                    || myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT1
                                    || LoginInfo.roleId == ConstData.ROLE_ID_HR
                                    || LoginInfo.roleId == ConstData.ROLE_ID_HRMANAGER)
                    {   // 事业部
                        qable.Where(a => (a.company_id_parent == myCompanyInfo.id || a.company_id == myCompanyInfo.id));
                    }
                    else if (myPositionInfo.positionType == ConstData.POSITION_GM2 || myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT2)   // 分公司
                        qable.Where(a => a.company_id == myCompanyInfo.id);
                    else if (myPositionInfo.positionType == ConstData.POSITION_DEPT_M)
                        qable.Where(a => a.dept_id == myPositionInfo.deptId);
                    else
                        qable.Where(a => a.supervisor_id == LoginInfo.empId);
                }

                if (!string.IsNullOrEmpty(name))
                    qable.Where(a => a.name.Contains(name));
                if (queryInfo != null)
                {
                    if (!string.IsNullOrEmpty(queryInfo.work_number))
                        qable.Where(a => a.work_number.Contains(queryInfo.work_number));
                    if (!string.IsNullOrEmpty(queryInfo.position_name))
                        qable.Where(a => a.position_name.Contains(queryInfo.position_name));
                    if (queryInfo.company_id > 0)
                        qable.Where(a => a.company_id == queryInfo.company_id);
                    if (queryInfo.area_l1_id > 0)
                        qable.Where(a => a.area_l1_id == queryInfo.area_l1_id);
                    if (queryInfo.dept_id > 0)
                        qable.Where(a => a.dept_id == queryInfo.dept_id);
                }
                if (queryTime != null)
                {
                    if (queryTime.startTime1 != null)
                        qable.Where(a => a.entry_date >= queryTime.startTime1);
                    if (queryTime.startTime2 != null)
                    {
                        queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                        qable.Where(a => a.entry_date < queryTime.startTime2);
                    }
                }

                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .Select("a.id, a.position_id, a.position_name, a.grade, a.dept_id, a.dept_name, a.introducer_id, a.introducer_name, a.company_id, a.company_linkname, a.emp_category, a.work_addr, a.entry_date, a.status, a.work_number, a.name,a.name_v2, b.phone, b.emergency_contact, b.emergency_contact_phone, b.identity, b.birthdate, b.birthday, b.birthday_type, b.native, b.address, b.education, b.bank_account,c.inactive as account_status")
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public MemoryStream ExportExcel(Pagination pagination, daoben_hr_emp_job queryInfo, string name, QueryTime queryTime)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (pagination == null)
                pagination = new Pagination();
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.company_id" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "asc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_hr_emp_job>()
                    .JoinTable<daoben_hr_emp_info>((a, b) => a.id == b.id)
                    .JoinTable<daoben_ms_account>((a, c) => a.id == c.employee_id)
                    .Where(a => a.position_id > 0);

                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (LoginInfo.roleId == ConstData.ROLE_ID_FINANCIAL || LoginInfo.roleId == ConstData.ROLE_ID_FINANCIALMANAGER)
                    {   // TODO：临时用于ly-cw测试，放开权限
                        qable.Where(a => (a.company_id_parent == myCompanyInfo.id || a.company_id == myCompanyInfo.id));
                    }
                    else if (myPositionInfo.positionType == ConstData.POSITION_GM1
                                    || myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT1
                                    || LoginInfo.roleId == ConstData.ROLE_ID_HR
                                    || LoginInfo.roleId == ConstData.ROLE_ID_HRMANAGER)
                    {   // 事业部
                        qable.Where(a => (a.company_id_parent == myCompanyInfo.id || a.company_id == myCompanyInfo.id));
                    }
                    else if (myPositionInfo.positionType == ConstData.POSITION_GM2 || myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT2)   // 分公司
                        qable.Where(a => a.company_id == myCompanyInfo.id);
                    else if (myPositionInfo.positionType == ConstData.POSITION_DEPT_M)
                        qable.Where(a => a.dept_id == myPositionInfo.deptId);
                    else
                        qable.Where(a => a.supervisor_id == LoginInfo.empId);
                }

                if (!string.IsNullOrEmpty(name))
                    qable.Where(a => a.name.Contains(name));
                if (queryInfo != null)
                {
                    if (!string.IsNullOrEmpty(queryInfo.work_number))
                        qable.Where(a => a.work_number.Contains(queryInfo.work_number));
                    if (!string.IsNullOrEmpty(queryInfo.position_name))
                        qable.Where(a => a.position_name.Contains(queryInfo.position_name));
                    if (queryInfo.company_id > 0)
                        qable.Where(a => a.company_id == queryInfo.company_id);
                    if (queryInfo.area_l1_id > 0)
                        qable.Where(a => a.area_l1_id == queryInfo.area_l1_id);
                    if (queryInfo.dept_id > 0)
                        qable.Where(a => a.dept_id == queryInfo.dept_id);
                }
                if (queryTime != null)
                {
                    if (queryTime.startTime1 != null)
                        qable.Where(a => a.entry_date >= queryTime.startTime1);
                    if (queryTime.startTime2 != null)
                    {
                        queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                        qable.Where(a => a.entry_date < queryTime.startTime2);
                    }
                }

                //人力资源-在职员工
                string str = "a.name,a.name_v2,a.work_number,a.position_name,a.grade,a.emp_category,a.dept_name,a.company_linkname,(CASE WHEN a.status=1 THEN '休假' WHEN a.status=0 THEN '在职' ELSE '-' END),(CASE WHEN c.inactive=1 THEN '已注销' WHEN c.inactive=0 THEN '正常' ELSE '-' END) as account_status,b.phone,b.education,b.bank_account,DATE_FORMAT(b.birthdate,'%Y-%m-%d'),b.age,(CASE WHEN b.birthday_type=1 THEN '农历' WHEN b.birthday_type=2 THEN '阳历' ELSE '-' END),DATE_FORMAT(b.identity,'%Y-%m-%d'),a.work_addr,DATE_FORMAT(a.entry_date,'%Y-%m-%d'),b.address,b.native,b.emergency_contact,a.introducer_name";
                var listDt = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                    .Select(str)
                    .ToDataTable();
                int rowsCount = listDt.Rows.Count;
                string[] headerArr = new string[]
                {"姓名","v2姓名","工号","职位","等级","雇员类别", "所属部门","所属机构","在职状态","账户状态","联系电话", "学历"
            ,"银行卡号码","出生日期","年龄","农历/阳历","身份证号码","工作地点","入职日期","家庭地址","籍贯","紧急联系人","介绍人",

                };
                int[] colWidthArr = new int[] { 15, 15, 20, 15, 15, 15, 15, 25, 15, 15, 15, 15, 25, 18, 15, 15, 25, 25, 20, 25, 15, 25, 15 };

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
        public object GetListResign(Pagination pagination, daoben_hr_emp_resigned_job queryInfo, string name, QueryTime queryTime)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");

            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.resign_date" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_hr_emp_resigned_job>().JoinTable<daoben_hr_emp_resigned_info>((a, b) => a.id == b.id);
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                {
                    if (myPositionInfo.positionType == ConstData.POSITION_GM1
                                    || myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT1
                                    || LoginInfo.roleId == ConstData.ROLE_ID_HR
                                    || LoginInfo.roleId == ConstData.ROLE_ID_HRMANAGER)
                    {   // 事业部
                        qable.Where(a =>( a.company_id_parent == myCompanyInfo.id || a.company_id == myCompanyInfo.id));
                    }
                    else if (myPositionInfo.positionType == ConstData.POSITION_GM2 || myPositionInfo.positionType == ConstData.POSITION_GM_ASSISTANT2)   // 分公司
                        qable.Where(a => a.company_id == myCompanyInfo.id);
                    else if (myPositionInfo.positionType == ConstData.POSITION_DEPT_M)
                        qable.Where(a => a.dept_id == myPositionInfo.id);
                    else
                        qable.Where(a => a.supervisor_id == LoginInfo.empId);
                }
                if (queryInfo != null)
                {                   
                    if (!string.IsNullOrEmpty(queryInfo.position_name))
                        qable.Where(a => a.position_name.Contains(queryInfo.position_name));
                    if (!string.IsNullOrEmpty(queryInfo.work_number))
                        qable.Where(a => a.work_number.Contains(queryInfo.work_number));
                    if (queryInfo.company_id != 0)
                        qable.Where(a => a.company_id == queryInfo.company_id);
                }                    
                if (queryTime != null)
                {
                    if (queryTime.startTime1 != null)
                        qable.Where(a => a.entry_date >= queryTime.startTime1);
                    if (queryTime.startTime2 != null)
                    {
                        queryTime.startTime2 = queryTime.startTime2.ToDate().AddDays(1);
                        qable.Where(a => a.entry_date < queryTime.startTime2);
                    }
                }
                if (!string.IsNullOrEmpty(name))
                    qable.Where(a => a.name.Contains(name));

                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                    .Select("a.id, a.position_id, a.position_name, a.grade, a.dept_id, a.dept_name, a.introducer_id, a.introducer_name, a.company_id, a.company_name, a.emp_category, a.work_addr, a.entry_date, a.work_number, a.name,a.name_v2, b.phone, b.emergency_contact, b.emergency_contact_phone, b.identity, b.birthdate, b.birthday, b.birthday_type, b.native, b.address, b.education, b.bank_account")
                    .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }

        public object GetListNew(Pagination pagination, string name)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");

            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "a.entry_date" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_hr_emp_job>().JoinTable<daoben_hr_emp_info>((a, b) => a.id == b.id)
                        .Where(a => a.position_id == 0);
                if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                    qable.Where(a => a.company_id == myCompanyInfo.id);

                if (!string.IsNullOrEmpty(name))
                    qable.Where(a => a.name.Contains(name));

                string listStr = qable.OrderBy(pagination.sidx + " " + pagination.sord)
                        .Select("a.id, a.entry_date, a.work_number, a.name, b.submit_time")
                        .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }
        /// <summary>
        /// 规定：新增员工信息时，如果是“员工自行完善信息”的，则这里只提交姓名、工号、账号、密码，其他信息不录入
        /// </summary>
        /// <param name="addInfo"></param>
        /// <param name="addJobInfo"></param>
        /// <param name="addAccountInfo"></param>
        /// <returns></returns>
        public string Add(daoben_hr_emp_info addInfo, daoben_hr_emp_job addJobInfo,
                    daoben_ms_account addAccountInfo, List<daoben_distributor_guide> distributorList, List<daoben_hr_emp_file> imageList)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (addJobInfo == null || addAccountInfo == null)
                return "信息错误，操作失败!";
            if (string.IsNullOrEmpty(addJobInfo.name) || string.IsNullOrEmpty(addAccountInfo.account)
                        || string.IsNullOrEmpty(addAccountInfo.password))
            {
                return "信息错误，操作失败!";
            }
            daoben_hr_emp_job_history jobHistory = null;
            daoben_hr_emp_area_history areaHistory = null;

            DateTime now = DateTime.Now;
            addJobInfo.id = addAccountInfo.employee_id = Common.GuId();
            if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN)
                addJobInfo.creator_job_history_id = LoginInfo.jobHistoryId;
            else
                addJobInfo.creator_job_history_id = "admin";
            addJobInfo.creator_name = LoginInfo.empName;
            addJobInfo.create_time = DateTime.Now;
            addJobInfo.salary_blank = true;    // 新增员工必须后续配置工资信息
            addAccountInfo.employee_name = addJobInfo.name;
            addAccountInfo.inactive = false;
            addAccountInfo.inactive_id = 0;
            addAccountInfo.inactive_name = null;
            addAccountInfo.inactive_time = null;
            addAccountInfo.reg_time = DateTime.Now;
            addAccountInfo.password = PasswordStorage.CreateHash(addAccountInfo.password);
            if (imageList != null && imageList.Count > 0)
            {
                imageList.ForEach(a =>
                {
                    a.main_id = addJobInfo.id;
                    a.creator_job_history_id = LoginInfo.jobHistoryId;
                    a.creator_name = LoginInfo.empName;
                    a.create_time = DateTime.Now;
                });
            }
            daoben_payroll_template payrollTemp = new daoben_payroll_template();
            payrollTemp.id = addJobInfo.id;
            payrollTemp.salary_blank = true;    // 新增员工必须后续配置工资信息
            if (distributorList != null && distributorList.Count > 0)
            {
                distributorList.ForEach(a =>
                {
                    a.guide_id = addJobInfo.id;
                    a.guide_name = addJobInfo.name;
                    a.effect_date = addJobInfo.entry_date;
                    a.creator_job_history_id = LoginInfo.jobHistoryId;
                    a.creator_name = LoginInfo.empName;
                    a.create_time = DateTime.Now;
                    a.inactive_time = null;
                });
            }
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    //身份证不能与数据表中已有的相同
                    bool isExist = db.Queryable<daoben_hr_emp_info>().Any(a => a.identity == addInfo.identity);
                    if (isExist)
                        return "信息错误：此身份证信息与已有员工身份证重复";
                    if (addJobInfo.entry_date == null)
                    {
                        addAccountInfo.role_id = ConstData.ROLE_ID_NEW_EMP;
                        addAccountInfo.role_name = "新员工";
                        addJobInfo.company_id = LoginInfo.companyInfo.id;
                    }
                    else
                    {
                        if (addInfo == null)
                            return "信息错误，操作失败!";
                        addInfo.id = addJobInfo.id;
                        //if (addJobInfo.entry_date > now || addJobInfo.entry_date < now.AddDays(-7))
                        //    return "信息错误：入职时间有误!";// TODO 暂时放开
                        #region 判断信息完整性
                        if (addAccountInfo.role_id < 1 || addJobInfo.company_id < 1 || addJobInfo.position_id < 1)
                            return "信息错误，操作失败!";
                        daoben_ms_role roleInfo = db.Queryable<daoben_ms_role>().InSingle(addAccountInfo.role_id);
                        if (roleInfo == null)
                            return "信息错误：指定角色不存在";
                        addAccountInfo.role_name = roleInfo.name;
                        daoben_org_position positionInfo = db.Queryable<daoben_org_position>().InSingle(addJobInfo.position_id);
                        if (positionInfo == null)
                            return "信息错误：指定职位不存在";
                        addJobInfo.position_name = positionInfo.name;
                        addJobInfo.position_type = positionInfo.position_type;
                        addJobInfo.company_id = positionInfo.company_id;
                        addJobInfo.company_name = positionInfo.company_name;
                        addJobInfo.company_id_parent = positionInfo.company_id_parent;
                        addJobInfo.company_linkname = positionInfo.company_linkname;
                        addJobInfo.dept_id = positionInfo.dept_id;
                        addJobInfo.dept_name = positionInfo.dept_name;
                        if (addJobInfo.area_l1_id < 1 && addJobInfo.area_l2_id > 0)
                            return "信息错误：指定业务片区需要同时指定经理片区";
                        if (addJobInfo.area_l2_id < 1)
                        {
                            addJobInfo.area_l2_id = 0;
                            addJobInfo.area_l2_name = null;
                        }
                        if (addJobInfo.area_l1_id < 1)
                        {
                            addJobInfo.area_l1_id = 0;
                            addJobInfo.area_l1_name = null;
                        }
                        #endregion

                        addInfo.submitter_id = LoginInfo.accountId;
                        addInfo.submitter_name = LoginInfo.empName;
                        addInfo.submit_time = now;

                        if (addJobInfo.name_v2 == null)
                            addJobInfo.name_v2 = addJobInfo.name;
                        jobHistory = NewHistoryInfo(addJobInfo, LoginInfo);
                        addJobInfo.cur_job_history_id = jobHistory.id;
                        #region 业务员/业务经理添加员工区域变更表信息
                        if (addJobInfo.position_type == ConstData.POSITION_SALES || addJobInfo.position_type == ConstData.POSITION_SALESMANAGER)
                            areaHistory = NewAreaHistoryInfo(addJobInfo, LoginInfo);
                        #endregion
                    }

                    // Check name and work_number
                    bool empExist = db.Queryable<daoben_hr_emp_job>()
                           .Where(a => a.name == addJobInfo.name || a.work_number == addJobInfo.work_number).Any();
                    if (empExist)
                        return "信息错误：存在相同姓名或工号的员工";

                    // Check account
                    bool accountExist = db.Queryable<daoben_ms_account>().Where(a => a.account == addAccountInfo.account).Any();
                    if (accountExist)
                        return "该账户名称已存在，操作失败！";


                    db.CommandTimeOut = 30;
                    try
                    {
                        db.BeginTran();
                        db.Insert(addJobInfo);
                        db.Insert(payrollTemp);
                        if (addJobInfo.entry_date != null)
                        {
                            db.Insert(addInfo);
                            db.Insert(jobHistory);
                            if (areaHistory != null)
                                db.Insert(areaHistory);
                            db.DisableInsertColumns = new string[] { "id" };
                            if (imageList != null && imageList.Count > 0)
                                db.InsertRange(imageList);
                        }
                        db.DisableInsertColumns = new string[] { "id" };
                        if (distributorList != null && distributorList.Count > 0)
                            db.InsertRange(distributorList);
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
            catch (Exception ex)
            {
                return "系统出错：" + ex.Message;
            }
        }
        /// <summary>
        /// 修改个人信息
        /// </summary>
        /// <param name="editInfo"></param>
        /// <returns></returns>
        public string EditPersonalInfo(daoben_hr_emp_info editInfo)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            if (editInfo == null || string.IsNullOrEmpty(editInfo.id))
                return "信息错误，操作失败!";

            daoben_hr_emp_info empInfo = new daoben_hr_emp_info
            {
                #region 复制信息
                id = editInfo.id,
                phone = editInfo.phone,
                wechat = editInfo.wechat,
                emergency_contact = editInfo.emergency_contact,
                emergency_contact_phone = editInfo.emergency_contact_phone,
                identity_type = editInfo.identity_type,
                identity = editInfo.identity,
                identity_address = editInfo.identity_address,
                identity_issue = editInfo.identity_issue,
                identity_effect = editInfo.identity_effect,
                identity_expire = editInfo.identity_expire,
                gender = editInfo.gender,
                political = editInfo.political,
                birthdate = editInfo.birthdate,
                age = editInfo.age,
                birthday = editInfo.birthday,
                birthday_type = editInfo.birthday_type,
                marriage = editInfo.marriage,
                native = editInfo.native,
                nation = editInfo.nation,
                child_count = editInfo.child_count,
                country = editInfo.country,
                address = editInfo.address,
                health_start = editInfo.health_start,
                health_expire = editInfo.health_expire,
                education = editInfo.education,
                profession = editInfo.profession,
                graduation_school = editInfo.graduation_school,
                graduation_date = editInfo.graduation_date,
                note = editInfo.note,
                bank_type = editInfo.bank_type,
                bank_name = editInfo.bank_name,
                bank_account = editInfo.bank_account,
                parents_bank = editInfo.parents_bank,
                parents_bankaccount = editInfo.parents_bankaccount,
                parents_phone = editInfo.parents_phone
                #endregion
            };
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    bool isExist = db.Queryable<daoben_hr_emp_info>().Any(a => a.id == editInfo.id);
                    if (!isExist)
                        return "信息错误，指定员工信息不存在!";
                    db.Update(empInfo);
                    return "success";
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return "系统出错：" + ex.Message;
                }
            }
        }
        /// <summary>
        /// 修改个人职位信息
        /// </summary>
        /// <param name="editInfo"></param>
        /// <returns></returns>
        public string EditJobInfo(daoben_hr_emp_job editInfo)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            if (editInfo == null || string.IsNullOrEmpty(editInfo.id))
                return "信息错误，操作失败!";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            bool upHistory = false;
            daoben_hr_emp_area_history areaHistory = null;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_hr_emp_job origJobInfo = db.Queryable<daoben_hr_emp_job>().Where(a => a.id == editInfo.id).SingleOrDefault();
                    if (origJobInfo == null)
                        return "信息错误，指定员工信息不存在!";
                    #region 复制信息
                    origJobInfo.guide_category = editInfo.guide_category;
                    origJobInfo.work_addr = editInfo.work_addr;
                    origJobInfo.tax_unit = editInfo.tax_unit;
                    origJobInfo.insurance_paid = editInfo.insurance_paid;
                    origJobInfo.entry_date = editInfo.entry_date;
                    origJobInfo.cur_contract_sign = editInfo.cur_contract_sign;
                    origJobInfo.cur_contract_expire = editInfo.cur_contract_expire;
                    origJobInfo.status = editInfo.status;
                    origJobInfo.grade = editInfo.grade;
                    if (editInfo.position_id != origJobInfo.position_id)
                    {
                        upHistory = true;
                        #region 职位信息
                        daoben_org_position positionInfo = db.Queryable<daoben_org_position>()
                                    .Where(a => a.id == editInfo.position_id).SingleOrDefault();
                        if (positionInfo == null)
                            return "信息错误：指定职位不存在";
                        origJobInfo.position_id = positionInfo.id;
                        origJobInfo.position_name = positionInfo.name;
                        origJobInfo.position_type = positionInfo.position_type;
                        origJobInfo.company_id = positionInfo.company_id;
                        origJobInfo.company_name = positionInfo.company_name;
                        origJobInfo.company_id_parent = positionInfo.company_id_parent;
                        origJobInfo.company_linkname = positionInfo.company_linkname;
                        origJobInfo.dept_id = positionInfo.dept_id;
                        origJobInfo.dept_name = positionInfo.dept_name;
                        #endregion
                    }
                    if (editInfo.name_v2 != origJobInfo.name_v2)
                    {
                        upHistory = true;
                        origJobInfo.name_v2 = editInfo.name_v2;
                    }
                    if (editInfo.area_l2_id != origJobInfo.area_l2_id)
                    {
                        upHistory = true;
                        origJobInfo.area_l2_id = editInfo.area_l2_id;
                        origJobInfo.area_l2_name = editInfo.area_l2_name;
                        if (editInfo.position_type == ConstData.POSITION_SALES)
                            areaHistory = NewAreaHistoryInfo(origJobInfo, LoginInfo, now.Date);
                    }
                    if (editInfo.area_l1_id != origJobInfo.area_l1_id)
                    {
                        upHistory = true;
                        origJobInfo.area_l1_id = editInfo.area_l1_id;
                        origJobInfo.area_l1_name = editInfo.area_l1_name;
                        if (editInfo.position_type == ConstData.POSITION_SALESMANAGER)
                            areaHistory = NewAreaHistoryInfo(origJobInfo, LoginInfo, now.Date);
                    }
                    if (editInfo.emp_category != origJobInfo.emp_category)
                    {
                        upHistory = true;
                        origJobInfo.emp_category = editInfo.emp_category;
                    }
                    if (editInfo.supervisor_name != origJobInfo.supervisor_name)
                    {
                        origJobInfo.supervisor_name = editInfo.supervisor_name;
                        origJobInfo.supervisor_id = editInfo.supervisor_id;
                    }
                    if (editInfo.introducer_name != origJobInfo.introducer_name)
                    {
                        origJobInfo.introducer_name = editInfo.introducer_name;
                        origJobInfo.introducer_id = editInfo.introducer_id;
                    }
                    #endregion
                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    if (upHistory)
                    {
                        #region 职位历史
                        daoben_hr_emp_job_history historyInfo = NewHistoryInfo(origJobInfo, LoginInfo, now.Date);
                        origJobInfo.cur_job_history_id = historyInfo.id;
                        object inactiveObj = new
                        {
                            inactive = true,
                            inactive_pisition_name = myPositionInfo.name,
                            inactive_job_history_id = LoginInfo.jobHistoryId,
                            inactive_time = now
                        };
                        db.Update<daoben_hr_emp_job_history>(inactiveObj, a => a.emp_id == origJobInfo.id && a.inactive == false);
                        db.Insert(historyInfo);
                        #endregion
                        if (areaHistory != null) // 业务员/业务经理
                        {
                            // 以前设置过将来生效的信息，仅在指派业务员/业务经理时产生
                            db.Delete<daoben_hr_emp_job_history>(a => a.emp_id == origJobInfo.id && a.effect_date > now);
                            inactiveObj = new
                            {
                                inactive = true,
                                inactive_time = historyInfo.create_time
                            };
                            db.Update<daoben_hr_emp_area_history>(inactiveObj, a => a.emp_id == origJobInfo.id && a.inactive == false);
                            db.Delete<daoben_hr_emp_area_history>(a => a.emp_id == origJobInfo.id && a.effect_date > now); // 以前设置过将来生效的信息
                            db.Insert(areaHistory);
                        }
                    }
                    db.Update(origJobInfo);
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

        /// <summary>
        /// 员工离职
        /// </summary>
        public string EmpResign(string id, DateTime resignTime)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            if (string.IsNullOrEmpty(id))
                return "信息错误，操作失败！";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            DateTime now = DateTime.Now;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_hr_emp_job jobInfo = db.Queryable<daoben_hr_emp_job>().SingleOrDefault(a => a.id == id);
                    if (jobInfo == null || jobInfo.status == -100)
                        return "信息错误，指定员工不存在或者已离职";

                    jobInfo.status = -100;
                    daoben_hr_emp_job_history jobHistoryInfo = NewHistoryInfo(jobInfo, LoginInfo, resignTime);
                    jobInfo.cur_job_history_id = jobHistoryInfo.id;
                    object obj = new
                    {
                        inactive = true,
                        inactive_pisition_name = myPositionInfo.name,
                        inactive_job_history_id = LoginInfo.jobHistoryId,
                        inactive_time = resignTime
                    };

                    #region 更新表
                    db.CommandTimeOut = 30;
                    db.BeginTran();

                    db.Update<daoben_hr_emp_job_history>(obj, a => a.emp_id == jobInfo.id && a.inactive == false);
                    db.Insert(jobHistoryInfo);

                    db.Update<daoben_hr_emp_job>(new { status = -100 }, a => a.id == id);
                    if(jobInfo.position_type == ConstData.POSITION_SALES || jobInfo.position_type == ConstData.POSITION_SALESMANAGER)
                    {   // 业务员/业务经理
                        db.Update<daoben_hr_emp_area_history>(new { inactive = true, inactive_time = resignTime },
                                a => a.emp_id == jobInfo.id && a.inactive == false);
                    }
                    else if(jobInfo.position_type == ConstData.POSITION_GUIDE)
                    {   // 导购员
                        object guideObj = new
                        {
                            inactive = true,
                            inactive_job_history_id = LoginInfo.jobHistoryId,
                            inactive_time = resignTime
                        };
                        db.Update<daoben_distributor_guide>(guideObj, a => a.guide_id == jobInfo.id && a.inactive == false);

                    }
                    db.CommitTran();
                    return "success";
                    #endregion
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return "系统出错：" + ex.Message;
                }
            }
        }



        /// <summary>
        /// TODO 暂时不用
        /// </summary>
        /// <param name="editInfo"></param>
        /// <param name="editJobInfo"></param>
        /// <param name="addImageList"></param>
        /// <param name="delImageList"></param>
        /// <returns></returns>
        public string Edit(daoben_hr_emp_info editInfo, daoben_hr_emp_job editJobInfo,
                List<daoben_hr_emp_file> addImageList, List<int> delImageList)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            if (editInfo == null && editJobInfo == null)
                return "信息错误，操作失败!";
            string id = null;
            DateTime now = DateTime.Now;
            object empInfo = null;
            object empJobInfo = null;
            daoben_payroll_template payrollTemp = null;

            if (editJobInfo != null)
            {
                if (!string.IsNullOrEmpty(editJobInfo.id))
                    id = editJobInfo.id;
                //职位信息修改
                empJobInfo = new
                {
                    name_v2 = editJobInfo.name_v2 == null ? editJobInfo.name : editJobInfo.name_v2,
                    entry_date = editJobInfo.entry_date,
                    work_addr = editJobInfo.work_addr,
                    insurance_paid = editJobInfo.insurance_paid,
                    tax_unit = editJobInfo.tax_unit
                };
            }
            if (editInfo != null)
            {
                if (id == null && !string.IsNullOrEmpty(editInfo.id))
                    id = editInfo.id;
                //个人信息修改
                empInfo = new
                {
                    phone = editInfo.phone,
                    wechat = editInfo.wechat,
                    emergency_contact = editInfo.emergency_contact,
                    emergency_contact_phone = editInfo.emergency_contact_phone,
                    political = editInfo.political,
                    birthday = editInfo.birthday,
                    birthday_type = editInfo.birthday_type,
                    marriage = editInfo.marriage,
                    native = editInfo.native,
                    child_count = editInfo.child_count,
                    country = editInfo.country,
                    address = editInfo.address,
                    religion = editInfo.religion,
                    note = editInfo.note,
                    bank_type = editInfo.bank_type,
                    bank_name = editInfo.bank_name,
                    bank_account = editInfo.bank_account,
                    parents_bank = editInfo.parents_bank,
                    parents_bankaccount = editInfo.parents_bankaccount,
                    parents_phone = editInfo.parents_phone
                };
            }

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
                    a.main_id = id;
                    a.creator_job_history_id = LoginInfo.jobHistoryId;
                    a.creator_name = LoginInfo.empName;
                    a.create_time = DateTime.Now;
                });
            }
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_hr_emp_job origJobInfo = db.Queryable<daoben_hr_emp_job>().Where(a => a.id == id).SingleOrDefault();
                    if (origJobInfo == null)
                        return "信息错误，指定员工信息不存在!";
                    daoben_hr_emp_job_history jobHistory = null;
                    if (origJobInfo.entry_date == null && editJobInfo == null)
                    {
                        // 员工自行更新信息，往前倒推7天入职
                        empJobInfo = new { entry_date = now.AddDays(-7) };
                    }
                    else if (origJobInfo.position_id == 0 && editJobInfo != null)
                    {
                        #region 人事部完善职位信息
                        daoben_org_position positionInfo = db.Queryable<daoben_org_position>().InSingle(editJobInfo.position_id);
                        if (positionInfo == null)
                            return "信息错误：指定职位不存在";
                        editJobInfo.name = origJobInfo.name;
                        editJobInfo.work_number = origJobInfo.work_number;
                        editJobInfo.position_name = positionInfo.name;
                        editJobInfo.position_type = positionInfo.position_type;
                        editJobInfo.company_id = positionInfo.company_id;
                        editJobInfo.company_name = positionInfo.company_name;
                        editJobInfo.company_id_parent = positionInfo.company_id_parent;
                        editJobInfo.company_linkname = positionInfo.company_linkname;
                        editJobInfo.dept_id = positionInfo.dept_id;
                        editJobInfo.dept_name = positionInfo.dept_name;
                        if (editJobInfo.area_l1_id < 1 && editJobInfo.area_l2_id > 0)
                            return "信息错误：指定业务片区需要同时指定经理片区";
                        if (editJobInfo.area_l2_id < 1)
                        {
                            editJobInfo.area_l2_id = 0;
                            editJobInfo.area_l2_name = null;
                        }
                        if (editJobInfo.area_l1_id < 1)
                        {
                            editJobInfo.area_l1_id = 0;
                            editJobInfo.area_l1_name = null;
                        }
                        empJobInfo = editJobInfo;
                        payrollTemp = new daoben_payroll_template();
                        payrollTemp.id = id;
                        payrollTemp.salary_blank = true;
                        // InitSalaryTemp(payrollTemp, db, positionInfo, editJobInfo);

                        jobHistory = NewHistoryInfo(editJobInfo, LoginInfo);//历史表
                        #endregion
                    }
                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    if (empInfo != null)
                        db.Update<daoben_hr_emp_info>(empInfo, a => a.id == id);
                    if (empJobInfo != null)
                    {
                        db.Update<daoben_hr_emp_job>(empJobInfo, a => a.id == id);
                        if (jobHistory != null)
                            db.Insert(jobHistory);
                    }
                    if (payrollTemp != null)
                        db.Update<daoben_payroll_template>(payrollTemp, a => a.id == id);
                    if (delImageList != null && delImageList.Count > 0)
                        db.Update<daoben_hr_emp_file>(delObj, a => delImageList.Contains(a.id));
                    if (addImageList != null && addImageList.Count > 0)
                    {
                        db.DisableInsertColumns = new string[] { "id" };
                        db.InsertRange(addImageList);
                    }
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

        /// <summary>
        /// 账户停用/启用
        /// </summary>
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
        public string GetInfo(string id)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_hr_emp_job empJobInfo = db.Queryable<daoben_hr_emp_job>().InSingle(id);
                    if (empJobInfo == null)
                        return "信息错误：指定的职位信息不存在";
                    daoben_hr_emp_info empInfo = db.Queryable<daoben_hr_emp_info>().Where(a => a.id == id).SingleOrDefault();
                    daoben_ms_account accountInfo = db.Queryable<daoben_ms_account>().Where(a => a.employee_id == id)
                           .Select("account,role_id, role_name, inactive").SingleOrDefault();
                    List<daoben_hr_emp_file> imageList = db.Queryable<daoben_hr_emp_file>()
                                .Where(a => a.main_id == id && a.is_del == false)
                                .OrderBy(a => a.type, OrderByType.Asc).ToList();
                    object resultObj = new
                    {
                        empInfo = empInfo,
                        empJobInfo = empJobInfo,
                        accountInfo = accountInfo,
                        imageList = imageList,

                    };
                    return resultObj.ToJson();

                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }


        public string GetEmpByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            using (var db = SugarDao.GetInstance())
            {
                string empInfo = db.Queryable<daoben_hr_emp_job>()
                        .Where(a => a.name == name).Select("id, name").SingleOrDefault().ToJson();
                return empInfo;
            }
        }

        public string GetAreaEmpCountList(int companyId, int empType)
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
                var qable = db.Queryable<daoben_hr_emp_job>().Where(a => a.area_l1_id > 0 && a.company_id == companyId);
                switch (empType)
                {
                    case 0: // 全部
                        break;
                    case 20: // 业务员
                        qable.Where(a => a.position_type == ConstData.POSITION_SALES && a.position_type == 0);
                        break;
                    case 21: // 业务经理
                        qable.Where(a => a.position_type == ConstData.POSITION_SALESMANAGER && a.position_type == 5);
                        break;
                    case 3: // 导购员
                        qable.Where(a => a.position_type == ConstData.POSITION_GUIDE);
                        break;
                    default:
                        return null;
                }
                string pairListStr = qable.GroupBy(a => a.area_l1_id).Select("count(*) as emp_count, area_l1_id, area_l1_name").ToJson();
                return pairListStr;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId">分公司ID</param>
        /// <param name="areaId"></param>
        /// <param name="empType"></param>
        /// <returns></returns>
        public string GetEmpTree(int companyId, int empType, string guide_category = null)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;

            using (var db = SugarDao.GetInstance())
            {

                var qable = db.Queryable<daoben_hr_emp_job>();
                if (empType != 1)
                {
                    //.Where(a => a.area_l1_id > 0);
                    if (companyId == 0)  // 获取登录人所在机构下
                        qable.Where(a => a.company_id == myCompanyInfo.id);
                    else if (companyId > 0)
                        qable.Where(a => a.company_id == companyId);
                    else
                        return null;
                }
                else
                {
                    if (companyId == 0)  // 获取登录人所在机构下
                        qable.Where(a => a.company_id == myCompanyInfo.id || a.company_id_parent == myCompanyInfo.id);
                    else if (companyId > 0)
                        qable.Where(a => a.company_id == companyId);
                }
                switch (empType)
                {
                    case 0: // 全部
                        break;
                    case 20: // 业务员
                        qable.Where(a => a.position_type == ConstData.POSITION_SALES);
                        break;
                    case 21: // 业务经理
                        qable.Where(a => a.position_type == ConstData.POSITION_SALESMANAGER);
                        break;
                    case 3: // 导购员
                        qable.Where(a => a.position_type == ConstData.POSITION_GUIDE);
                        break;
                    case 12://培训师
                        qable.Where(a => a.position_type == ConstData.POSITION_TRAINER);
                        break;
                    case 13://培训经理
                        qable.Where(a => a.position_type == ConstData.POSITION_TRAINERMANAGER);
                        break;
                    case 1:
                        qable.Where(a => a.position_type <= ConstData.POSITION_OFFICE_NORMAL);
                        break;
                    default:
                        return null;
                }
                if ((!string.IsNullOrEmpty(guide_category)) && (empType == 3))
                    qable.Where(a => a.guide_category == guide_category);
                List<EmpKeyInfo> empList = new List<EmpKeyInfo>();
                if (empType != 1)
                    empList = qable.Select<EmpKeyInfo>("id, name,area_l1_id,area_l1_name, area_l2_id, area_l2_name,company_name,company_id,company_linkname,emp_category,grade, CONCAT(name,' (', IFNULL(area_l1_name,''),'-', IFNULL(area_l2_name,''),')')  as display_info").ToList();
                else
                    empList = qable.Select<EmpKeyInfo>("id, name,area_l1_id,area_l1_name, area_l2_id, area_l2_name,company_name,company_id,company_linkname,emp_category,grade, CONCAT(name,' (', IFNULL(company_name,''),'-', IFNULL(dept_name,''),')')  as display_info").ToList();
                string empTreeStr = empList.GroupBy(a => a.area_l2_id).Select(a1 => new AreaL2Tree
                {
                    area_l2_id = a1.Key,
                    area_l2_name = a1.First().area_l2_name,
                    key_list = a1.ToList()
                }).ToJson();
                return empTreeStr;
            }
        }


        public object GetEmpCount(int companyId, int empType)
        {
            if (companyId < 0)
                return new { emp_count = 0 };
            if (companyId == 0)
            {
                OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
                if (LoginInfo == null)
                    throw new Exception("用户登陆过期，请重新登录");
                companyId = LoginInfo.companyInfo.id;
            }
            using (var db = SugarDao.GetInstance())
            {
                var qable = db.Queryable<daoben_hr_emp_job>().Where(a => a.company_id == companyId); ;
                switch (empType)
                {
                    case 0: // 全部
                        break;
                    case 20: // 业务员
                        qable.Where(a => a.position_type == ConstData.POSITION_SALES && a.position_type == 0);
                        break;
                    case 21: // 业务经理
                        qable.Where(a => a.position_type == ConstData.POSITION_SALESMANAGER && a.position_type == 5);
                        break;
                    case 3: // 导购员
                        qable.Where(a => a.position_type == ConstData.POSITION_GUIDE);
                        break;
                    default:
                        return new { emp_count = 0 };
                }
                int empCount = qable.Count();
                return new { emp_count = empCount };
            }
        }
        /// <summary>
        /// TODO 应改用GetEmpTree
        /// </summary>
        /// <param name="companyId">分公司ID</param>
        /// <param name="areaId">经理片区</param>
        /// <param name="empType"></param>
        /// <returns></returns>
        public string GetAreaEmpList(int companyId, int areaId, int empType)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;

            if (areaId < 0)
                return null;
            using (var db = SugarDao.GetInstance())
            {

                var qable = db.Queryable<daoben_hr_emp_job>();
                if (areaId > 0)
                    qable.Where(a => a.area_l1_id == areaId);
                else
                {
                    qable.Where(a => a.area_l1_id > 0);
                    if (companyId == 0)  // 获取登录人所在机构下
                        qable.Where(a => a.company_id == myCompanyInfo.id);
                    else if (companyId > 0)
                        qable.Where(a => a.company_id == companyId);
                    else
                        return null;
                }
                switch (empType)
                {
                    case 0: // 全部
                        break;
                    case 20: // 业务员
                        qable.Where(a => a.position_type == ConstData.POSITION_SALES && a.position_type == 0);
                        break;
                    case 21: // 业务经理
                        qable.Where(a => a.position_type == ConstData.POSITION_SALESMANAGER && a.position_type == 5);
                        break;
                    case 3: // 导购员
                        qable.Where(a => a.position_type == ConstData.POSITION_GUIDE);
                        break;
                    default:
                        return null;
                }
                string pairListStr = qable.Select("id, name, area_l1_id, area_l1_name, CONCAT(name,' (', area_l2_name, ')') as name_area").ToJson();
                return pairListStr;
            }
        }

        public string GetAreaOrDeptEmpList()
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            using (var db = SugarDao.GetInstance())
            {
                // 事业部
                List<EmpKeyInfo> empList1 = db.Queryable<daoben_hr_emp_job>().Where(a => a.company_id == myCompanyInfo.id)
                           .Select<EmpKeyInfo>("id, name, dept_id as area_id, dept_name as area_name, company_id, company_linkname,emp_category,grade, CONCAT(name,' (',company_name,' - ', dept_name, ')') as display_info").ToList();
                // 分公司
                List<EmpKeyInfo> empList2 = db.Queryable<daoben_hr_emp_job>().Where(a => a.company_id_parent == myCompanyInfo.id)
                          .Select<EmpKeyInfo>("id, name, area_l1_id as area_id, area_l1_name as area_name, company_id, company_linkname,emp_category,grade, CONCAT(name,' (',company_name,' - ', area_l1_name, ')') as display_info").ToList();
                empList1.AddRange(empList2);
                return empList1.ToJson();

            }
        }

        public string GetMyCompanyEmpList(bool exclude_emp_status, DateTime? exclude_entry_date)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            string selectStr = null;
            using (var db = SugarDao.GetInstance())
            {
                if (myCompanyInfo.category == "分公司")
                    selectStr = "id, name,position_name,dept_name,area_l2_name, CONCAT(name,' (',position_name,' - ', IFNULL(area_l1_name,'无所属区域'), ')') as display_info";
                else
                    selectStr = "id, name,position_name,dept_name,area_l2_name, CONCAT(name,' (',position_name,' - ', IFNULL(dept_name,'无所属部门'), ')') as display_info";
                var qable = db.Queryable<daoben_hr_emp_job>().Where(a => a.company_id == myCompanyInfo.id);
                if (exclude_emp_status)
                    qable.Where(a => a.emp_category != "实习生");
                if (exclude_entry_date != null)
                    qable.Where(a => a.entry_date <= exclude_entry_date);
                string empListStr = qable.Select(selectStr).OrderBy("entry_date desc").ToJson();
                return empListStr;
            }
        }
        public string GetEmpListByCompanyId(bool exclude_emp_status, DateTime? exclude_entry_date, int company_id)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            string selectStr = null;
            if (company_id == 0)
                company_id = myCompanyInfo.id;
            using (var db = SugarDao.GetInstance())
            {
                daoben_org_company companyInfo = db.Queryable<daoben_org_company>().InSingle(company_id);
                if (companyInfo == null)
                    return null;
                if (companyInfo.category == "分公司")
                    selectStr = "id, name,position_name,dept_name,area_l2_name, CONCAT(name,' (',position_name,' - ', IFNULL(area_l1_name,'无所属区域'), ')') as display_info";
                else
                    selectStr = "id, name,position_name,dept_name,area_l2_name, CONCAT(name,' (',position_name,' - ', IFNULL(dept_name,'无所属部门'), ')') as display_info";
                var qable = db.Queryable<daoben_hr_emp_job>().Where(a => a.company_id == company_id);
                if (exclude_emp_status)
                    qable.Where(a => a.emp_category != "实习生");
                if (exclude_entry_date != null)
                    qable.Where(a => a.entry_date <= exclude_entry_date);
                string empListStr = qable.Select(selectStr).OrderBy("entry_date desc").ToJson();
                return empListStr;
            }
        }
        public string GetAreaDeptPosEmpList()
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    if (myCompanyInfo == null)
                        return null;
                    List<daoben_org_dept_area> areaDeptList = null;
                    if (myCompanyInfo.category == "事业部")
                        areaDeptList = db.Queryable<daoben_org_dept_area>()
                        .Where(a => a.company_id == myCompanyInfo.id).Where(a => a.type == 0).ToList();
                    else if (myCompanyInfo.category == "分公司")
                        areaDeptList = db.Queryable<daoben_org_dept_area>()
                        .Where(a => a.company_id == myCompanyInfo.id).Where(a => a.type == 1 || a.name == "公司直属").ToList();

                    List<daoben_org_position> posList = db.Queryable<daoben_org_position>()
                        .Where(a => a.company_id == myCompanyInfo.id).ToList();
                    List<daoben_hr_emp_job> empList = db.Queryable<daoben_hr_emp_job>()
                        .Where(a => a.company_id == myCompanyInfo.id).ToList();
                    object resultObj = new
                    {
                        areaDeptList = areaDeptList,
                        posList = posList,
                        empList = empList,
                    };
                    return resultObj.ToJson();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
        /// <summary>
        /// 雇员类别信息修改，仅仅支持实习转员工
        /// </summary>
        public string EmpCategoryChange(string id)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            if (string.IsNullOrEmpty(id))
                return "信息错误，操作失败!";
            object empjob = new
            {
                emp_category = "员工",
            };
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    daoben_hr_emp_job mainInfo = db.Queryable<daoben_hr_emp_job>().SingleOrDefault(a => a.id == id);
                    if (mainInfo == null || mainInfo.emp_category != "实习生")
                        return "信息错误，指定员工信息不存在或者已经转正";

                    db.CommandTimeOut = 30;
                    db.BeginTran();
                    db.Update<daoben_hr_emp_job>(empjob, a => a.id == id);
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


        #region  Private Functions
        private void InitSalaryTemp(daoben_payroll_template payrollTemp, SqlSugarClient db,
                    daoben_org_position positionInfo, daoben_hr_emp_job addJobInfo)
        {
            // 工资模板
            payrollTemp.company_id = positionInfo.company_id;
            DateTime now = DateTime.Now;
            if (addJobInfo.entry_date != null)
            {
                DateTime entryDate = (DateTime)addJobInfo.entry_date;
                payrollTemp.seniority_month = (now.Year - entryDate.Year) * 12 + (now.Month - entryDate.Month);
                if (entryDate.Day < 16)   // 15号以后入职的不计当月工龄
                    payrollTemp.seniority_month--;
                if (addJobInfo.emp_category == "实习生")
                    payrollTemp.is_internship = true;
                payrollTemp.entry_date = entryDate;
            }
            if (positionInfo.position_type != ConstData.POSITION_GUIDE)
            {
                string salaryId = db.Queryable<daoben_salary_position>().Where(a => a.company_id == payrollTemp.company_id && a.category == 1
                        && a.effect_status == 1).Select<string>("id").SingleOrDefault();
                if (string.IsNullOrEmpty(salaryId))  // 查找公版
                    salaryId = db.Queryable<daoben_salary_position>().Where(a => a.company_id == positionInfo.company_id_parent && a.category == 1
                        && a.is_template < 3 && a.effect_status == 1).Select<string>("id").SingleOrDefault();
                //var ss =
                //        .JoinTable<daoben_salary_position_guide>
            }
            if (positionInfo.company_category != "董事会" && positionInfo.position_type != ConstData.POSITION_GUIDE) // 非导购工资
            {
                int gradeCagetory = db.Queryable<daoben_org_dept_area>()
                            .Where(a => a.id == positionInfo.dept_id).Select<int>("grade_category").SingleOrDefault();
                daoben_salary_position_grade salaryInfo = db.Queryable<daoben_salary_position_grade>()
                            .JoinTable<daoben_salary_position>((a, b) => a.salary_position_id == b.id)
                            .Where<daoben_salary_position>((a, b) => b.category == 4 && b.effect_status == 1
                            && b.company_id == positionInfo.company_id && a.grade_category == gradeCagetory && a.grade == addJobInfo.grade)
                            .Select("base_salary, position_salary, house_subsidy, attendance_reward, kpi_advice").SingleOrDefault();
                if (salaryInfo != null)
                {
                    payrollTemp.grade_salary_status = 1;
                    payrollTemp.base_salary = salaryInfo.base_salary;
                    payrollTemp.position_salary = salaryInfo.position_salary;
                    payrollTemp.house_subsidy = salaryInfo.house_subsidy;
                    payrollTemp.attendance_reward = salaryInfo.attendance_reward;
                    payrollTemp.kpi_advice = salaryInfo.kpi_advice;
                }
                // 工龄工资
                payrollTemp.seniority_salary = db.Queryable<daoben_salary_position_seniority>()
                            .JoinTable<daoben_salary_position>((a, b) => a.salary_position_id == b.id)
                            .Where<daoben_salary_position>((a, b) => b.category == 3 && b.effect_status == 1
                            && b.company_id == positionInfo.company_id && a.year_min == 0)
                            .Select<int>("salary").SingleOrDefault();

            }
        }
        #endregion

        /// <summary>
        /// 由 addJobInfo,loginInfo 生成 NewHistoryInfo
        /// </summary>
        /// <param name="addJobInfo">daoben_hr_emp_job 职位信息表</param>
        /// <returns>daoben_hr_emp_job_history 历史表</returns>
        public daoben_hr_emp_job_history NewHistoryInfo(daoben_hr_emp_job addJobInfo, OperatorModel loginInfo, DateTime? effectDate = null)
        {
            DateTime now = DateTime.Now;
            if (addJobInfo.name_v2 == null)
                addJobInfo.name_v2 = addJobInfo.name;
            daoben_hr_emp_job_history jobHistory = new daoben_hr_emp_job_history
            {
                id = Common.GuId(),
                emp_id = addJobInfo.id,
                name = addJobInfo.name,
                name_v2 = addJobInfo.name_v2,
                position_id = addJobInfo.position_id,
                position_name = addJobInfo.position_name,
                grade = addJobInfo.grade,
                dept_id = addJobInfo.dept_id,
                dept_name = addJobInfo.dept_name,
                area_l1_id = addJobInfo.area_l1_id,
                area_l1_name = addJobInfo.area_l1_name,
                area_l2_id = addJobInfo.area_l2_id,
                area_l2_name = addJobInfo.area_l2_name,
                company_id = addJobInfo.company_id,
                company_name = addJobInfo.company_name,
                company_id_parent = addJobInfo.company_id_parent,
                company_linkname = addJobInfo.company_linkname,
                position_type = addJobInfo.position_type,
                status = addJobInfo.status,
                creator_job_history_id = loginInfo.jobHistoryId,
                creator_name = loginInfo.empName,
                create_time = now,
                effect_date = effectDate == null ? (DateTime)addJobInfo.entry_date : (DateTime)effectDate,
                inactive = false,
                inactive_time = null,

            };
            return jobHistory;
        }

        /// <summary>
        /// 由 addJobInfo,loginInfo 生成 NewHistoryInfo
        /// </summary>
        /// <param name="addJobInfo">daoben_hr_emp_job 职位信息表</param>
        /// <returns>daoben_hr_emp_job_history 历史表</returns>
        public daoben_hr_emp_area_history NewAreaHistoryInfo(daoben_hr_emp_job jobInfo, OperatorModel loginInfo, DateTime? effectDate = null)
        {
            DateTime now = DateTime.Now;
            if (jobInfo.name_v2 == null)
                jobInfo.name_v2 = jobInfo.name;
            daoben_hr_emp_area_history areaHistory = new daoben_hr_emp_area_history
            {
                id = Common.GuId(),
                emp_id = jobInfo.id,
                name = jobInfo.name,
                name_v2 = jobInfo.name_v2,
                area_l1_id = jobInfo.area_l1_id,
                area_l1_name = jobInfo.area_l1_name,
                area_l2_id = jobInfo.area_l2_id,
                area_l2_name = jobInfo.area_l2_name,
                company_id = jobInfo.company_id,
                company_id_parent = jobInfo.company_id_parent,
                company_name = jobInfo.company_name,
                effect_date = effectDate == null ? (DateTime)jobInfo.entry_date : (DateTime)effectDate,
                creator_job_history_id = loginInfo.jobHistoryId,
                creator_name = loginInfo.empName,
                create_time = now,
                inactive = false,
                inactive_time = null,
            };
            return areaHistory;
        }

        public string Import(List<daoben_hr_emp_info> empInfoList, List<daoben_hr_emp_job> empJobList)
        {
            OperatorModel LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                throw new Exception("用户登陆过期，请重新登录");
            if (empInfoList == null || empInfoList.Count < 1)
                return "信息错误，操作失败!";
            if (empJobList == null || empJobList.Count < 1)
                return "信息错误，操作失败!";
            DateTime now = DateTime.Now;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;

            List<string> dupList = empJobList.GroupBy(a => a.id).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            if (dupList != null && dupList.Count > 0)
                return "信息错误：导入表中员工工号重复" + dupList.ToJson();

            List<daoben_ms_account> accountList = new List<daoben_ms_account>();
            daoben_ms_account tempAccountInfo = new daoben_ms_account();
            List<daoben_hr_emp_job_history> jobHistoryList = new List<daoben_hr_emp_job_history>();
            List<daoben_hr_emp_area_history> areaHistoryList = new List<daoben_hr_emp_area_history>();
            List<daoben_distributor_guide> distributorGuideList = new List<daoben_distributor_guide>();
            List<daoben_distributor_info> distributorList = new List<daoben_distributor_info>();

            string password = PasswordStorage.CreateHash("123456");

            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    if (empJobList.Exists(a => a.position_type == ConstData.POSITION_GUIDE))
                    {
                        var qable = db.Queryable<daoben_distributor_info>();
                        if (myCompanyInfo.category == "事业部")
                            qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                        else if (myCompanyInfo.category == "分公司")
                            qable.Where(a => a.company_id == myCompanyInfo.id);
                        distributorList = qable.Select("id, name").ToList();
                    }
                    foreach (var a in empJobList)
                    {
                        if (string.IsNullOrEmpty(a.name))
                            return "员工姓名不能为空";
                        if (string.IsNullOrEmpty(a.work_number))
                            return "员工“" + a.name + "”工号不能为空";//将工号作为账户名

                        if (a.position_type == 0 || a.position_id == 0)
                            return "员工职位、职位类型不能为空";


                        a.creator_job_history_id = LoginInfo.jobHistoryId;
                        a.creator_name = LoginInfo.empName;
                        if (a.create_time == null)
                            a.create_time = now;
                        #region 账户信息
                        tempAccountInfo.role_id = ConstData.ROLE_ID_NEW_EMP;
                        tempAccountInfo.role_name = "新员工";//默认为新员工 TODO 根据position_type确定role_id
                        tempAccountInfo = new daoben_ms_account();
                        tempAccountInfo.account = a.id;
                        tempAccountInfo.employee_id = a.id;
                        tempAccountInfo.employee_name = a.name;
                        tempAccountInfo.employee_type = 0;
                        tempAccountInfo.password = password;
                        tempAccountInfo.creator_id = LoginInfo.accountId;
                        tempAccountInfo.creator_name = LoginInfo.empName;
                        tempAccountInfo.reg_time = now;
                        accountList.Add(tempAccountInfo);
                        #endregion
                        //生成job_history
                        daoben_hr_emp_job_history tempJobHistory = NewHistoryInfo(a, LoginInfo);
                        jobHistoryList.Add(tempJobHistory);
                        a.cur_job_history_id = tempJobHistory.id;//将工号作为id
                        // 业务员/业务经理添加员工区域变更表信息
                        if (a.position_type == ConstData.POSITION_SALES || a.position_type == ConstData.POSITION_SALESMANAGER)
                            areaHistoryList.Add(NewAreaHistoryInfo(a, LoginInfo));
                        #region 导购-经销商
                        if (a.position_type == ConstData.POSITION_GUIDE && !string.IsNullOrEmpty(a.distributor_name))
                        {
                            string[] dNameList = a.distributor_name.Split('|');
                            for (int i = 0; i < dNameList.Length; i++)
                            {
                                daoben_distributor_info dInfo = distributorList.Where(d => d.name == dNameList[i]).FirstOrDefault();
                                if (dInfo != null)
                                {
                                    distributorGuideList.Add(new daoben_distributor_guide
                                    {
                                        guide_id = a.id,
                                        guide_name = a.name,
                                        distributor_id = dInfo.id,
                                        distributor_name = dInfo.name,
                                        effect_date = a.entry_date,
                                        creator_job_history_id = LoginInfo.jobHistoryId,
                                        creator_name = LoginInfo.empName,
                                        create_time = now,
                                        inactive = false,
                                        inactive_time = null,
                                    });
                                }
                            }
                        }
                        #endregion
                    }
                    db.CommandTimeOut = 300;
                    db.BeginTran();
                    if (empInfoList.Count > 25)
                        db.SqlBulkCopy(empInfoList);
                    else
                        db.InsertRange(empInfoList);
                    if (empJobList.Count > 25)
                        db.SqlBulkCopy(empJobList);
                    else
                        db.InsertRange(empJobList);
                    if (accountList.Count > 25)
                        db.SqlBulkCopy(accountList);
                    else
                        db.InsertRange(accountList);
                    if (jobHistoryList.Count > 25)
                        db.SqlBulkCopy(jobHistoryList);
                    else
                        db.InsertRange(jobHistoryList);
                    if (areaHistoryList.Count > 25)
                        db.SqlBulkCopy(areaHistoryList);
                    else if (areaHistoryList.Count > 0)
                        db.InsertRange(areaHistoryList);
                    if (distributorGuideList.Count > 25)
                        db.SqlBulkCopy(distributorGuideList);
                    else if (distributorGuideList.Count > 0)
                        db.InsertRange(distributorGuideList);
                    db.CommitTran();
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    return "系统出错：" + ex.Message;
                }

                ThreadPool.QueueUserWorkItem(t =>
                {
                    supervisorIdCheck();//TODO 检查上级数据
                });
                return "success";

            }
        }//后续匹配上级主管，介绍人姓名等信息

        /// <summary>
        /// 职位列表（用于员工导入）
        /// </summary>
        /// <param name="companyId">事业部ID</param>
        /// <returns></returns>
        public string GetPosList(int companyId)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN && LoginInfo.roleId != ConstData.ROLE_ID_HRMANAGER && LoginInfo.roleId != ConstData.ROLE_ID_FINANCIALMANAGER)
                return "权限不足";
            companyId = (companyId == 0 ? myCompanyInfo.id : companyId);
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    //TODO 换成 CompanyPosTree                   
                    List<daoben_org_position> posList = db.Queryable<daoben_org_position>()
                        .Where(a => a.company_id == companyId || a.company_id_parent == companyId)
                        .Select("id ,name ,company_id_parent,company_id,company_linkname,company_name,company_category,dept_id,dept_name,position_type")
                        .ToList();
                    return posList.ToJson();
                }
                catch (Exception ex)
                {
                    return "系统出错：" + ex.Message;
                }
            }
        }

        /// <summary>
        /// 区域列表（用于员工导入）
        /// </summary>
        /// <param name="companyId">事业部ID</param>
        /// <returns></returns>
        public string GetAreaList(int companyId)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            if (LoginInfo.roleId != ConstData.ROLE_ID_ADMIN && LoginInfo.roleId != ConstData.ROLE_ID_HRMANAGER && LoginInfo.roleId != ConstData.ROLE_ID_FINANCIALMANAGER)
                return "权限不足";
            companyId = (companyId == 0 ? myCompanyInfo.id : companyId);
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    //TODO 换成 CompanyPosTree                   
                    List<daoben_org_area> posList = db.Queryable<daoben_org_area>()
                        .Where(a => a.company_id == companyId || a.company_id_parent == companyId)
                        .Select("id ,name ,parent_id,parent_name")
                        .ToList();
                    return posList.ToJson();
                }
                catch (Exception ex)
                {
                    return "系统出错：" + ex.Message;
                }
            }
        }



        private void supervisorIdCheck()
        {
            using (var db = SugarDao.GetInstance())
            {

                StringBuilder sqlUpSb1 = new StringBuilder("UPDATE daoben_hr_emp_job AS a,daoben_hr_emp_job AS b ");
                sqlUpSb1.Append("SET a.supervisor_id = b.id ");
                sqlUpSb1.Append("WHERE a.supervisor_name = b.name ;");
                StringBuilder sqlUpSb2 = new StringBuilder("UPDATE daoben_hr_emp_job AS a,daoben_hr_emp_job AS b ");
                sqlUpSb2.Append("SET a.introducer_id = b.id ");
                sqlUpSb2.Append("WHERE a.introducer_name = b.name ;");
                db.SqlQuery<int>(sqlUpSb1.ToString());
                db.SqlQuery<int>(sqlUpSb2.ToString());

            }
        }

        public string GetSalesList()
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    var qable = db.Queryable<daoben_hr_emp_job_history>()
                        .Where(a => a.position_type == ConstData.POSITION_SALES);
                    if (myCompanyInfo.category == "事业部")
                        qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                    else if (myCompanyInfo.category == "分公司")
                        qable.Where(a => a.company_id == myCompanyInfo.id);

                    string empListStr = qable.Select("emp_id as id,name,name_v2")
                            .GroupBy(a => a.emp_id).ToJson();

                    return empListStr;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
        public string GetGuideList()
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    var qable = db.Queryable<daoben_hr_emp_job_history>()
                        .Where(a => a.position_type == ConstData.POSITION_GUIDE);
                    if (myCompanyInfo.category == "事业部")
                        qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                    else if (myCompanyInfo.category == "分公司")
                        qable.Where(a => a.company_id == myCompanyInfo.id);

                    string empListStr = qable.Select("emp_id as id,name,name_v2")
                            .GroupBy(a=>a.emp_id).ToJson();                  

                    return empListStr;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }


        public string GetAllEmpId()
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;

            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    var qable = db.Queryable<daoben_hr_emp_job>();
                    if (myCompanyInfo.category == "分公司")
                        qable.Where(a => a.company_id == myCompanyInfo.id);
                    else if (myCompanyInfo.category == "事业部")
                        qable.Where(a => a.company_id_parent == myCompanyInfo.id);
                    List<string> empId = qable.Select<string>("id").ToList();
                    return empId.ToJson();
                }
                catch (Exception ex)
                {
                    return "系统出错：" + ex.Message;
                }
            }
        }
    }
}
