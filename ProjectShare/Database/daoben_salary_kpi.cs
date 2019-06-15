using System;

namespace ProjectShare.Database
{

    public class daoben_salary_kpi
    {
        /// <summary>
        /// 薪资设置表-主表
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 员工ID，同表daoben_hr_emp_job
        /// </summary>
        public string emp_id { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string emp_name { get; set; }
        /// <summary>
        /// 工号
        /// </summary>
        public string work_number { get; set; }
        /// <summary>
        /// 雇员类别:实习生、劳务工、员工、职员
        /// </summary>
        public string emp_category { get; set; }
        /// <summary>
        /// 所属职位ID，表daoben_org_dept_area
        /// </summary>
        public int position_id { get; set; }
        /// <summary>
        /// 所属职位名称
        /// </summary>
        public string position_name { get; set; }
        /// <summary>
        /// 0-普通职位；1-事业部总经理；2-事业部助理；3-分公司总经理；4-分公司助理；5-部门/区域经理
        /// </summary>
        public int position_type { get; set; }
        /// <summary>
        /// 等级
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
        /// KPI评分类型：1-行政人员；2-培训师；3-培训经理
        /// </summary>
        public int category { get; set; }
        /// <summary>
        /// KPI分数
        /// </summary>
        public decimal kpi { get; set; }
        /// <summary>
        /// KPI补贴分数（仅行政人员）
        /// </summary>
        public decimal kpi_subsidy { get; set; }
        /// <summary>
        /// 评分说明
        /// </summary>
        public string note { get; set; }
        /// <summary>
        /// 考核周期起始日
        /// </summary>
        public DateTime? start_date { get; set; }
        /// <summary>
        /// 考核周期结束日
        /// </summary>
        public DateTime? end_date { get; set; }
        /// <summary>
        /// 考核月份
        /// </summary>
        public DateTime? month { get; set; }
        /// <summary>
        /// 当前审批状态（流程）
        /// </summary>
        public int approve_status { get; set; }
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


    }
}
