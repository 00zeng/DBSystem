using System;

namespace ProjectShare.Database
{
    public class daoben_org_position
    {
        /// <summary>
        /// 职位
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 职位名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 所属机构的上级机构ID，表daoben_org_company
        /// </summary>
        public int company_id_parent { get; set; }
        /// <summary>
        /// 所属机构ID，表daoben_org_company
        /// </summary>
        public int company_id { get; set; }
        /// <summary>
        /// 所属机构完整名称链，表daoben_org_company
        /// </summary>
        public string company_linkname { get; set; }
        /// <summary>
        /// 所属机构名称(仅自身名称)
        /// </summary>
        public string company_name { get; set; }
        /// <summary>
        /// “董事会”、“事业部”、“分公司”
        /// </summary>
        public string company_category { get; set; }
        /// <summary>
        /// 所属部门ID，表daoben_org_dept_area
        /// </summary>
        public int dept_id { get; set; }
        /// <summary>
        /// 所属部门名称
        /// </summary>
        public string dept_name { get; set; }        
        /// <summary>
        /// 职层：1-公司层面；2-部门层面
        /// </summary>
        public int grade_level { get; set; }
        /// <summary>
        /// 公司层面；部门层面
        /// </summary>
        public string grade_level_display { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string note { get; set; }
        /// <summary>
        /// 创建人信息记录ID；表daoben_hr_emp_job_history
        /// </summary>
        public string creator_job_history_id { get; set; }
        /// <summary>
        /// 创建人名称
        /// </summary>
        public string creator_name { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime create_time { get; set; }
        /// <summary>
        /// 1-事业部总经理；2-事业部助理；3-分公司总经理；4-分公司助理；5-部门经理；6-部门主管；7-行政普通员工；11-培训经理；12-培训师；21-业务经理；22-业务员；31-导购员
        /// </summary>
        public int position_type { get; set; }
        /// <summary>
        /// 是否能被删除：0-不可删除，1-可删除
        /// </summary>
        public bool delible { get; set; }

    }
}