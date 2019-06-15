using System;

namespace ProjectShare.Database
{
    public class daoben_hr_emp_job
    {
        /// <summary>
        /// ID同个人信息表ID，表daoben_hr_emp_info
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 工号
        /// </summary>
        public string work_number { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// V2系统名称，导入数据使用
        /// </summary>
        public string name_v2 { get; set; }
        /// <summary>
        /// 当前职位记录ID，表daoben_hr_emp_job_history
        /// </summary>
        public string cur_job_history_id { get; set; }
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
        /// 所属部门ID，表daoben_org_dept_area
        /// </summary>
        public int dept_id { get; set; }
        /// <summary>
        /// 所属部门名称
        /// </summary>
        public string dept_name { get; set; }
        /// <summary>
        /// 经理片区，表daoben_org_area
        /// </summary>
        public int area_l1_id { get; set; }
        /// <summary>
        /// 经理片区名称
        /// </summary>
        public string area_l1_name { get; set; }
        /// <summary>
        /// 业务片区，表daoben_org_area
        /// </summary>
        public int area_l2_id { get; set; }
        /// <summary>
        /// 业务片区名称
        /// </summary>
        public string area_l2_name { get; set; }
        /// <summary>
        /// 介绍人ID（仅导购）
        /// </summary>
        public string introducer_id { get; set; }
        /// <summary>
        /// 介绍人名称（仅导购）
        /// </summary>
        public string introducer_name { get; set; }
        /// <summary>
        /// 所属业务员ID（仅导购）
        /// </summary>
        public int sales_id { get; set; }
        /// <summary>
        /// 所属业务员名称（仅导购）
        /// </summary>
        public string sales_name { get; set; }
        /// <summary>
        /// 所属培训师ID（仅导购）
        /// </summary>
        public int trainer_id { get; set; }
        /// <summary>
        /// 所属培训师名称（仅导购）
        /// </summary>
        public string trainer_name { get; set; }
        /// <summary>
        /// 导购员类型：正职、副职、挂职（仅导购）
        /// </summary>
        public string guide_category { get; set; }
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
        /// 上司ID（非账户ID）
        /// </summary>
        public string supervisor_id { get; set; }
        /// <summary>
        /// 上司姓名
        /// </summary>
        public string supervisor_name { get; set; }
        /// <summary>
        /// 雇员类别:实习生、劳务工、员工、职员
        /// </summary>
        public string emp_category { get; set; }
        /// <summary>
        /// 工作地点
        /// </summary>
        public string work_addr { get; set; }
        /// <summary>
        /// 纳税单位
        /// </summary>
        public string tax_unit { get; set; }
        /// <summary>
        /// 是否已购买社保？0-否；1-是；
        /// </summary>
        public bool insurance_paid { get; set; }
        /// <summary>
        /// 入职时间
        /// </summary>
        public DateTime? entry_date { get; set; }
        /// <summary>
        /// 当前合同签署时间
        /// </summary>
        public DateTime? cur_contract_sign { get; set; }
        /// <summary>
        /// 当前合同到期时间
        /// </summary>
        public DateTime? cur_contract_expire { get; set; }
        /// <summary>
        /// 员工状态：0-在职，1-休假 ,-100离职
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? create_time { get; set; }
        /// <summary>
        /// 创建人信息记录ID；表daoben_hr_emp_job_history
        /// </summary>
        public string creator_job_history_id { get; set; }
        /// <summary>
        /// 创建人名称
        /// </summary>
        public string creator_name { get; set; }
        /// <summary>
        /// 1-事业部总经理；2-事业部助理；3-分公司总经理；4-分公司助理；5-部门经理；6-部门主管；7-行政普通员工；
        /// 11-培训经理；12-培训师；21-业务经理；22-业务员；31-导购员
        /// </summary>
        public int position_type { get; set; }
        /// <summary>
        /// 新入职工资是否未设置：0-已设置；1-未设置
        /// </summary>
        public bool salary_blank { get; set; }
        /// <summary>
        /// 非数据库列，用于存放临时数据，
        /// db.IsIgnoreErrorColumns = true 必须设置
        /// </summary>
        public string distributor_name { get; set; }
    }
}