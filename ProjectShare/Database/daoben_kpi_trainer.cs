using System;

namespace ProjectShare.Database
{
    public class daoben_kpi_trainer
    {
        /// <summary>
        /// 培训KPI表-主表
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 员工ID，同表daoben_hr_emp_job
        /// </summary>
        public string emp_id { get; set; }
        /// <summary>
        /// 员工信息记录ID；表daoben_hr_emp_job_history
        /// </summary>
        public string emp_job_history_id { get; set; }
        /// <summary>
        /// 11-培训经理；12-培训师
        /// </summary>
        public int position_type { get; set; }
        /// <summary>
        /// KPI总金额
        /// </summary>
        public decimal kpi_total { get; set; }
        /// <summary>
        /// 评分说明
        /// </summary>
        public string note { get; set; }
        /// <summary>
        /// 考核月份
        /// </summary>
        public DateTime month { get; set; }
        /// <summary>
        /// 审批状态：0-未审批；大于0通过；小于0不通过；以100作为审批完成的标志
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 0-正常；1-已删除（重新评分）
        /// </summary>
        public bool is_del { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime create_time { get; set; }
        /// <summary>
        /// 创建人信息记录ID；表daoben_hr_emp_job_history
        /// </summary>
        public string creator_job_history_id { get; set; }
        /// <summary>
        /// 创建人职位名称
        /// </summary>
        public string creator_position_name { get; set; }
        /// <summary>
        /// 创建人姓名（非账户名）
        /// </summary>
        public string creator_name { get; set; }
        /// <summary>
        /// 所属分公司ID
        /// </summary>
        public int company_id { get; set; }
        /// <summary>
        /// 所属机构名称链
        /// </summary>
        public string company_linkname { get; set; }
        /// <summary>
        /// 所属事业部ID
        /// </summary>
        public int company_id_parent { get; set; }
    }
}