using System;

namespace ProjectShare.Database
{
    public class daoben_hr_emp_job_history
    {
        /// <summary>
        /// 员工职务历史记录表
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 员工ID，表daoben_hr_emp_job
        /// </summary>
        public string emp_id { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// V2系统名称，导入数据使用
        /// </summary>
        public string name_v2 { get; set; }
        /// <summary>
        /// 职位ID，表daoben_org_position
        /// </summary>
        public int position_id { get; set; }
        /// <summary>
        /// 职位名称
        /// </summary>
        public string position_name { get; set; }
        /// <summary>
        /// 等级（含职等、星级）
        /// </summary>
        public string grade { get; set; }
        /// <summary>
        /// 所属部门ID，表daoben_org_dept
        /// </summary>
        public int dept_id { get; set; }
        /// <summary>
        /// 所属部门名称
        /// </summary>
        public string dept_name { get; set; }
        /// <summary>
        /// 经理片区ID，表daoben_org_area
        /// </summary>
        public int area_l1_id { get; set; }
        /// <summary>
        /// 经理片区
        /// </summary>
        public string area_l1_name { get; set; }
        /// <summary>
        /// 业务片区，表daoben_org_area
        /// </summary>
        public int area_l2_id { get; set; }
        /// <summary>
        /// 业务片区
        /// </summary>
        public string area_l2_name { get; set; }
        /// <summary>
        /// 所属机构ID，表daoben_org_company
        /// </summary>
        public int company_id { get; set; }
        /// <summary>
        /// 所属机构名称
        /// </summary>
        public string company_name { get; set; }
        /// <summary>
        /// 所属机构的上级机构ID，表daoben_org_company
        /// </summary>
        public int company_id_parent { get; set; }
        /// <summary>
        /// 所属机构完整名称链，表daoben_org_company
        /// </summary>
        public string company_linkname { get; set; }
        /// <summary>
        /// 雇员类别:实习生、劳务工、员工、职员
        /// </summary>
        public string emp_category { get; set; }
        /// <summary>
        /// 1-事业部总经理；2-事业部助理；3-分公司总经理；4-分公司助理；5-部门经理；6-部门主管；7-行政普通员工；11-培训经理；12-培训师；21-业务经理；22-业务员；31-导购员
        /// </summary>
        public int position_type { get; set; }
        /// <summary>
        /// 创建人信息记录ID；表daoben_hr_emp_job_history
        /// </summary>
        public string creator_job_history_id { get; set; }
        /// <summary>
        /// 创建人姓名（非账户名）
        /// </summary>
        public string creator_name { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? create_time { get; set; }
        /// <summary>
        /// 生效时间
        /// </summary>
        public DateTime? effect_date { get; set; }
        /// <summary>
        /// 员工状态：0-在职，1-休假 ,-100离职
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 0-有效；1-无效
        /// </summary>
        public bool inactive { get; set; }
        /// <summary>
        /// 失效操作人职位信息
        /// </summary>
        public string inactive_pisition_name { get; set; }
        /// <summary>
        /// 失效操作人信息记录ID；表daoben_hr_emp_job_history
        /// </summary>
        public string inactive_job_history_id { get; set; }
        /// <summary>
        /// 失效操作时间
        /// </summary>
        public DateTime? inactive_time { get; set; }

    }
}