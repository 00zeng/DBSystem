using System;

namespace ProjectShare.Database
{
    public class daoben_activity_sales_perf_alter
    {
        /// <summary>
        /// 业务考核修改表（结束时间）
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 主表ID
        /// </summary>
        public string main_id { get; set; }
        /// <summary>
        /// 原始结束日期
        /// </summary>
        public DateTime? orig_end_date { get; set; }
        /// <summary>
        /// 更新的结束日期
        /// </summary>
        public DateTime? alter_end_date { get; set; }
        /// <summary>
        /// 审批状态：0-未审批；大于0通过；小于0不通过；以100作为审批完成的标志
        /// </summary>
        public int approve_status { get; set; }
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
        /// 创建时间
        /// </summary>
        public DateTime create_time { get; set; }
    }
}