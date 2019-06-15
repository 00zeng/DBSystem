using System;

namespace ProjectShare.Database
{
    public class daoben_activity_sales_perf
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 活动名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 参与员工数量
        /// </summary>
        public int emp_count { get; set; }
        /// <summary>
        /// 参与对象类型：20-业务员；21-业务经理
        /// </summary>
        public int emp_category { get; set; }

        /// <summary>
        /// 考核类型：1-月度考核；2-销量考核；3-导购人数考核
        /// </summary>
        public int category { get; set; }
        /// <summary>
        /// 活动状态：-2-审核未结束；-1未开始；1-进行中；2-已结束
        /// </summary>
        public int activity_status { get; set; }
        /// <summary>
        /// 审批状态：100表示审批完成且通过
        /// </summary>
        public int approve_status { get; set; }
        /// <summary>
        /// 考核开始日期；月度考核时，该值为考核月份
        /// </summary>
        public DateTime? start_date { get; set; }
        /// <summary>
        /// 考核结束日期；
        /// </summary>
        public DateTime? end_date { get; set; }
        /// <summary>
        /// 考核内容：1-按实销量；2-按下货量
        /// </summary>
        public int target_content { get; set; }
        /// <summary>
        /// 考核模式：1-按完成率/按比例（导购人数）；2-按销量/按人数（导购人数考核）；
        /// </summary>
        public int target_mode { get; set; }
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
        /// 目标台数/目标销量/考核任务“导购-经销商”占比（导购人数考核）
        /// </summary>
        public int activity_target { get; set; }
        /// <summary>
        /// 目标完成率(仅当考核模式为1按照比例)
        /// </summary>
        public decimal activity_ratio { get; set; }
        /// <summary>
        /// 考核机型（销量考核）：1-按所有机型；2-按指定机型
        /// </summary>
        public int target_product { get; set; }
        /// <summary>
        /// 考核机型的指定型号（销量考核）
        /// </summary>
        public string product_model { get; set; }
        /// <summary>
        /// 奖罚方式-奖励：0-无奖励；>0-奖励金额
        /// </summary>
        public decimal reward { get; set; }
        /// <summary>
        /// 奖罚方式-罚款：0-无罚款；>0-罚款金额
        /// </summary>
        public decimal penalty { get; set; }
        /// <summary>
        /// 累计完成销量（台）
        /// </summary>
        public int total_sale_count { get; set; }
        /// <summary>
        /// 累计完成销量总金额（元）
        /// </summary>
        public decimal total_sale_amount { get; set; }
        /// <summary>
        /// 累计奖金（元），小于0为罚款
        /// </summary>
        public decimal total_reward { get; set; }
        /// <summary>
        /// 统计时间（中途统计）
        /// </summary>
        public DateTime? counting_time { get; set; }
        /// <summary>
        /// 创建人信息记录ID；表daoben_hr_emp_job_history
        /// </summary>
        public string creator_job_history_id { get; set; }
        /// <summary>
        /// 创建人职位名
        /// </summary>
        public string creator_position_name { get; set; }
        /// <summary>
        /// 创建人名称
        /// </summary>
        public string creator_name { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime create_time { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string note { get; set; }
    }
}