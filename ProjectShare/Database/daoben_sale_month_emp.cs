using System;

namespace ProjectShare.Database
{
    public class daoben_sale_month_emp
    {
        /// <summary>
        /// 员工月度销售额，工资结算时提交
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
        /// 11-培训经理；12-培训师；21-业务经理；22-业务员；31-导购员
        /// </summary>
        public int position_type { get; set; }
        /// <summary>
        /// 考核月份
        /// </summary>
        public DateTime month { get; set; }
        /// <summary>
        /// 开始日期；
        /// </summary>
        public DateTime start_date { get; set; }
        /// <summary>
        /// 结束日期；
        /// </summary>
        public DateTime end_date { get; set; }
        /// <summary>
        /// 审批状态：0-未审批；大于0通过；小于0不通过；以100作为审批完成的标志
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 正常机型销量
        /// </summary>
        public int normal_total_count { get; set; }
        /// <summary>
        /// 正常机批发价总额
        /// </summary>
        public decimal normal_wholesale_amount { get; set; }
        /// <summary>
        /// 正常机零售价总额
        /// </summary>
        public decimal normal_retail_amount { get; set; }
        /// <summary>
        /// 买断机型销量
        /// </summary>
        public int buyout_total_count { get; set; }
        /// <summary>
        /// 买断机批发价总额
        /// </summary>
        public decimal buyout_wholesale_amount { get; set; }
        /// <summary>
        /// 买断价总额
        /// </summary>
        public decimal buyout_amount { get; set; }
        /// <summary>
        /// 包销机型销量
        /// </summary>
        public int exclusive_total_count { get; set; }
        /// <summary>
        /// 包销机批发价总额
        /// </summary>
        public decimal exclusive_wholesale_amount { get; set; }
        /// <summary>
        /// 包销价总额
        /// </summary>
        public decimal exclusive_amount { get; set; }
        /// <summary>
        /// 特价机型销量
        /// </summary>
        public int special_total_count { get; set; }
        /// <summary>
        /// 特价机批发价总额
        /// </summary>
        public decimal special_wholesale_amount { get; set; }
        /// <summary>
        /// 特价机零售价总额
        /// </summary>
        public decimal special_retail_amount { get; set; }
        /// <summary>
        /// 高端机型销量
        /// </summary>
        public int high_level_total_count { get; set; }
        /// <summary>
        /// 高端机批发价总额
        /// </summary>
        public decimal high_level_wholesale_amount { get; set; }
        /// <summary>
        /// 高端机零售价总额
        /// </summary>
        public decimal high_level_retail_amount { get; set; }
        /// <summary>
        /// 总销量：正常机+买断机+包销机
        /// </summary>
        public int total_count { get; set; }
        /// <summary>
        /// 批发价总额
        /// </summary>
        public decimal total_wholesale_amount { get; set; }
        /// <summary>
        /// 批发价总额
        /// </summary>
        public decimal total_retail_amount { get; set; }
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