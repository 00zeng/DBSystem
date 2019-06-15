using System;

namespace ProjectShare.Database
{
    public class daoben_hr_leaving_cancel
    {
        /// <summary>
        /// 员工销假信息表
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 对应请假信息表ID
        /// </summary>
        public string leaving_id { get; set; }
        /// <summary>
        /// 销假开始时间
        /// </summary>
        public DateTime? cancel_begin_time { get; set; }
        /// <summary>
        /// 销假结束时间
        /// </summary>
        public DateTime? cancel_end_time { get; set; }
        /// <summary>
        /// 实际请假天数
        /// </summary>
        public decimal actual_days { get; set; }
        /// <summary>
        /// 销假种类：1-取消休假；2-提前回岗
        /// </summary>
        public int cancel_type { get; set; }
        /// <summary>
        /// 审批状态：0-未审批；100表示审批完成且通过
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