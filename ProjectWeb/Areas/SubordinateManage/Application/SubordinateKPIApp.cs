using Base.Code;
using MySqlSugar;
using ProjectShare.Database;
using ProjectShare.Models;
using ProjectShare.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectWeb.Areas.SubordinateManage.Application
{
    public class SubordinateKPIApp

    {

        public object GetList(Pagination pagination,string name,string work_number)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return null;

            int records = 0;
            if (pagination == null)
                pagination = new Pagination();
            pagination.page = pagination.page > 0 ? pagination.page : 1;
            pagination.rows = pagination.rows > 0 ? pagination.rows : 20;
            pagination.sidx = string.IsNullOrEmpty(pagination.sidx) ? "entry_date" : pagination.sidx;
            pagination.sord = string.IsNullOrEmpty(pagination.sord) ? "desc" : pagination.sord;
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            PositionInfo myPositionInfo = LoginInfo.positionInfo;
            using (var db = SugarDao.GetInstance())
            {

                var qable = db.Queryable<daoben_hr_emp_job>()
                         .Where(a => a.company_id == myCompanyInfo.id);
                if (!string.IsNullOrEmpty(name))
                    qable.Where(a => a.name.Contains(name));
                if (!string.IsNullOrEmpty(work_number))
                    qable.Where(a => a.work_number.Contains(work_number));
                string listStr = qable
                                .OrderBy(pagination.sidx + " " + pagination.sord)
                                .ToJsonPage(pagination.page, pagination.rows, ref records);
                pagination.records = records;//获取所有数量
                if (string.IsNullOrEmpty(listStr) || listStr == "[]")
                    return null;
                return listStr.ToJson();
            }
        }



        public string GetInfo()
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            using (var db = SugarDao.GetInstance())
            {

                daoben_ms_account accountInfo = db.Queryable<daoben_ms_account>().InSingle(LoginInfo.accountId);
                daoben_hr_emp_job employeeInfo = db.Queryable<daoben_hr_emp_job>().InSingle(accountInfo.employee_id);

                if (accountInfo == null || employeeInfo == null)
                    return "信息不存在";

                object resultObj = new
                {
                    employeeInfo = employeeInfo,
                    accountInfo = accountInfo

                };
                return resultObj.ToJson();
            }
        }



    }
}
