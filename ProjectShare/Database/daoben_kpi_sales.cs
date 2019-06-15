using System;

namespace ProjectShare.Database
{
    public class daoben_kpi_sales
    {
        /// <summary>
        /// 业务月度考核
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
        /// 姓名
        /// </summary>
        public string emp_name { get; set; }
        /// <summary>
        /// 21-业务经理；22-业务员；
        /// </summary>
        public int position_type { get; set; }
        /// <summary>
        /// 考核月份
        /// </summary>
        public DateTime? month { get; set; }
        /// <summary>
        /// 审批状态：0-未审批；大于0通过；小于0不通过；以100作为审批完成的标志
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 0-无人工调整；1-有人工调整
        /// </summary>
        public bool is_adjust { get; set; }
        /// <summary>
        /// 正常机型销量（含包销机）
        /// </summary>
        public int normal_count { get; set; }
        /// <summary>
        /// 正常机批发价总额
        /// </summary>
        public decimal normal_amount_w { get; set; }
        /// <summary>
        /// 正常机零售价总额
        /// </summary>
        public decimal normal_amount_r { get; set; }
        /// <summary>
        /// 买断机型销量
        /// </summary>
        public int buyout_count { get; set; }
        /// <summary>
        /// 买断机批发价总额
        /// </summary>
        public decimal buyout_amount_w { get; set; }
        /// <summary>
        /// 买断价总额
        /// </summary>
        public decimal buyout_amount { get; set; }
        /// <summary>
        /// 总销量：正常机+买断机
        /// </summary>
        public int total_count { get; set; }
        /// <summary>
        /// 批发价总额
        /// </summary>
        public decimal total_amount_w { get; set; }
        /// <summary>
        /// 零售价总额
        /// </summary>
        public decimal total_amount_r { get; set; }
        /// <summary>
        /// KPI总金额
        /// </summary>
        public decimal kpi_total { get; set; }
        /// <summary>
        /// KPI总金额（人工调整）
        /// </summary>
        public decimal kpi_total_a { get; set; }
        /// <summary>
        /// 累计完成率（*100，如40表示40%）
        /// </summary>
        public decimal total_ratio { get; set; }
        /// <summary>
        /// 人工调整说明
        /// </summary>
        public string note { get; set; }
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
        /// <summary>
        /// 0-正常；1-已删除（重新评分）
        /// </summary>
        public bool is_del { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? create_time { get; set; }
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

    }
}