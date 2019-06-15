using System;

namespace ProjectShare.Database
{
    public class daoben_hr_emp_resigned_job
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
        /// 所属区域ID，表daoben_org_dept_area
        /// </summary>
        public int area_id { get; set; }
        /// <summary>
        /// 所属区域名称
        /// </summary>
        public string area_name { get; set; }
        /// <summary>
        /// 介绍人ID（仅导购）
        /// </summary>
        public int introducer_id { get; set; }
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
        /// 所属机构名称
        /// </summary>
        public string company_name { get; set; }
        /// <summary>
        /// 上司ID
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
        public DateTime entry_date { get; set; }
        /// <summary>
        /// 离职时间
        /// </summary>
        public DateTime resign_date { get; set; }
        /// <summary>
        /// 离职类型
        /// </summary>
        public int resign_type { get; set; }
        /// <summary>
        /// 离职原因
        /// </summary>
        public string resign_reason { get; set; }
        /// <summary>
        /// 工作年限：年
        /// </summary>
        public int work_length_y { get; set; }
        /// <summary>
        /// 工作年限：月
        /// </summary>
        public int work_length_m { get; set; }
        /// <summary>
        /// 工作年限：日
        /// </summary>
        public int work_length_d { get; set; }
        /// <summary>
        /// 创建人ID
        /// </summary>
        public int creator_id { get; set; }
        /// <summary>
        /// 创建人姓名（非账户名）
        /// </summary>
        public string creator_name { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime create_time { get; set; }
    }
}