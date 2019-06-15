using System;

namespace ProjectShare.Database
{
    public class daoben_payroll_setting
    {
        /// <summary>
        /// 工资结算时间设置
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 月份，含年
        /// </summary>
        public DateTime month { get; set; }
        /// <summary>
        /// 起始日期
        /// </summary>
        public DateTime start_date { get; set; }
        /// <summary>
        /// 截止日期
        /// </summary>
        public DateTime? end_date { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string note { get; set; }
        /// <summary>
        /// 所属机构ID，表daoben_org_company
        /// </summary>
        public int company_id { get; set; }
        /// <summary>
        /// 所属机构名称
        /// </summary>
        public string company_name { get; set; }
        /// <summary>
        /// 创建人信息记录ID；表daoben_hr_emp_job_history
        /// </summary>
        public string creator_job_history_id { get; set; }
        /// <summary>
        /// 创建人职位名称
        /// </summary>
        public string creator_position_name { get; set; }
        /// <summary>
        /// 设置人姓名（非账户名）
        /// </summary>
        public string creator_name { get; set; }
        /// <summary>
        /// 设置时间
        /// </summary>
        public DateTime? create_time { get; set; }
        /// <summary>
        /// 审批状态：0-未审批；大于0通过；小于0不通过；以100作为审批完成的标志
        /// </summary>
        public int approve_status { get; set; }
        /// <summary>
        /// 审批人信息记录ID；表daoben_hr_emp_job_history
        /// </summary>
        public string approve_job_history_id { get; set; }
        /// <summary>
        /// 审批人姓名（非账户名）
        /// </summary>
        public string approve_name { get; set; }
        /// <summary>
        /// 审批时间
        /// </summary>
        public DateTime? approve_time { get; set; }
        /// <summary>
        /// 审批职位名称
        /// </summary>
        public string approve_position_name { get; set; }
        /// <summary>
        /// 审批备注
        /// </summary>
        public string approve_note { get; set; }
    }
}