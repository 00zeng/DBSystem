using System;
using System.Linq;
using MySqlSugar;
using Base.Code;
using ProjectShare.Database;
using ProjectShare.Process;
using ProjectShare.Models;
using System.Collections.Generic;

namespace ProjectWeb.Areas.SystemManage.Application
{
    public class CommonApiApp
    {
        #region 导入匹配列表
        /// <summary>
        /// 
        /// </summary>
        /// <param name="guide">是否获取【导购员】列表</param>
        /// <param name="sales">是否获取【业务员】列表</param>
        /// <param name="distributor">是否获取【经销商】列表</param>
        /// <param name="company">是否获取【分公司】列表</param>
        /// <param name="area">是否获取【业务片区】列表</param>
        /// <returns></returns>
        public object GetListForMatching(bool guide = true, bool sales = true, bool distributor = true,
                    bool company = true, bool area = true)
        {
            var LoginInfo = OperatorProvider.Provider.GetCurrent();
            if (LoginInfo == null)
                return "登录超时，请重新登录";
            CompanyInfo myCompanyInfo = LoginInfo.companyInfo;
            string whereStr = " {0}='{1}' ";
            string whereStrCompany = " {0}='{1}' ";
            string whereStrDistory = null;
            string myIdStr = myCompanyInfo.id.ToString();
            if (myCompanyInfo.category == "事业部")
            {
                whereStr = string.Format(" company_id_parent='{0}' ", myIdStr);
                whereStrCompany = string.Format(" parent_id='{0}' ", myIdStr);
                whereStrDistory = string.Format(" b.company_id_parent='{0}' ",myIdStr);
            }
            else if (myCompanyInfo.category == "分公司")
            {
                whereStr = string.Format(" company_id='{0}' ", myIdStr);
                whereStrCompany = string.Format(" id='{0}' ", myIdStr);
                whereStrDistory = string.Format(" b.company_id='{0}' ", myIdStr);
            }
            else
                return null;
            List<CommonApiInfo> guideList = null, salesList = null;
            List<daoben_distributor_info_history> distributorList = null;
            List<CommonApiOrg> companyList = null, areaList = null;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    if (guide)
                        guideList = db.Queryable<daoben_hr_emp_job_history>()   // 只要当过导购的都查出来
                                .Where(a => a.position_type == ConstData.POSITION_GUIDE)
                                .Where(whereStr).Select<CommonApiInfo>("emp_id as id,name,name_v2")
                                .GroupBy(a => a.id).ToList();
                    if (sales)
                        salesList = db.Queryable<daoben_hr_emp_job_history>()   // 业务员
                                .Where(a => a.position_type == ConstData.POSITION_SALES)
                                .Where(whereStr).Select<CommonApiInfo>("emp_id as id,name,name_v2")
                                .GroupBy(a => a.id).ToList();
                    if (distributor)// 导入时分公司信息以经销商的信息为准，需匹配销售时间
                        distributorList = db.Queryable<daoben_distributor_info_history>()
                                .JoinTable<daoben_distributor_info>((a, b)=>a.main_id == b.id)
                                .Where(whereStrDistory)
                                .Select("b.name, b.name_v2, b.code, a.main_id,a.company_id,a.company_id_parent,a.company_linkname,a.effect_date,a.inactive,a.inactive_time")
                                .OrderBy("a.main_id asc").OrderBy("effect_date desc").ToList();
                    if (company)
                        companyList = db.Queryable<daoben_org_company>()           // 分公司
                                .Where(whereStrCompany).Select<CommonApiOrg>("id,name,link_name as company_linkname")
                                .ToList();
                    if (area)
                        areaList = db.Queryable<daoben_org_area>()           // 业务片区
                                .Where(whereStr).Select<CommonApiOrg>("id,name")
                                .ToList();
                    object retObj = new
                    {
                        guideList = guideList,
                        salesList = salesList,
                        distributorList = distributorList,
                        companyList = companyList,
                        areaList = areaList,
                    };
                    return retObj;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
        #endregion
    }
}
