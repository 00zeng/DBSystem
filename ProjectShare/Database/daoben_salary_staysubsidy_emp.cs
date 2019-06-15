using System;

namespace ProjectShare.Database
{
    public class daoben_salary_staysubsidy_emp
    {
        /// <summary>
        /// id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 主表ID，表daoben_salary_staysubsidy
        /// </summary>
        public string main_id { get; set; }
        /// <summary>
        /// 员工ID
        /// </summary>
        public string emp_id { get; set; }
        /// <summary>
        /// 员工姓名
        /// </summary>
        public string emp_name { get; set; }
        /// <summary>
        /// 所属机构的上级机构ID，表daoben_org_company
        /// </summary>
        public int company_id_parent { get; set; }
        /// <summary>
        /// 所属机构ID，表daoben_org_company
        /// </summary>
        public int company_id { get; set; }
        /// <summary>
        /// 所属机构名称
        /// </summary>
        public string company_name { get; set; }
        /// <summary>
        /// 区域名称
        /// </summary>
        public string area_l2_name { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string dept_name { get; set; }
        /// <summary>
        /// 职位名称
        /// </summary>
        public string position_name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string display_info { get; set; }

    }
}