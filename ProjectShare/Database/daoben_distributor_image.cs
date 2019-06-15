using System;

namespace ProjectShare.Database
{
    public class daoben_distributor_image
    {
        /// <summary>
        /// 经销商形象返利表
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 经销商形象返利活动名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 经销商ID
        /// </summary>
        public string distributor_id { get; set; }
        /// <summary>
        /// 经销商姓名
        /// </summary>
        public string distributor_name { get; set; }
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
        /// 返利金额（元）
        /// </summary>
        public decimal rebate { get; set; }
        /// <summary>
        /// 返利模式：1-按时间返利；2-按销量返利；
        /// </summary>
        public int target_mode { get; set; }
        /// <summary>
        /// 奖励发放方式：1-一次性发放；2-按月发放
        /// </summary>
        public int pay_mode { get; set; }
        /// <summary>
        /// 活动开始日期；
        /// </summary>
        public DateTime? start_date { get; set; }
        /// <summary>
        /// 活动结束日期（仅当返利模式为“按时间返利”）
        /// </summary>
        public DateTime? end_date { get; set; }
        /// <summary>
        /// 活动天数
        /// </summary>
        public string day { get; set; }
        /// <summary>
        /// 计量方式（仅当返利模式为“按销量返利”）：1-按实销量；2-按下货量
        /// </summary>
        public int target_content { get; set; }
        /// <summary>
        /// 目标销量（台，仅当返利模式为“按销量返利”）
        /// </summary>
        public int activity_target { get; set; }
        /// <summary>
        /// 该门店前三个月月均销量台数（台，仅当返利模式为“按销量返利”）
        /// </summary>
        public int sale_avg_before { get; set; }
        /// <summary>
        /// 备注说明
        /// </summary>
        public string note { get; set; }
        /// <summary>
        /// 活动状态：-2-审核未结束；-1未开始；1-进行中；2-已结束
        /// </summary>
        public int activity_status { get; set; }
        /// <summary>
        /// 审批状态：0-未审批；100表示审批完成且通过
        /// </summary>
        public int approve_status { get; set; }
        // TODO delete
        /// <summary>
        /// 累计完成销量（台）
        /// </summary>
        public int total_sale_count { get; set; }
        /// <summary>
        /// 累计完成销量总金额（元）
        /// </summary>
        public decimal total_sale_amount { get; set; }
        /// <summary>
        /// 统计时间（中途统计）
        /// </summary>
        public DateTime? counting_time { get; set; }
        /// <summary>
        /// 返利金额(中途统计)
        /// </summary>
        public decimal counting_rebate { get; set; }
        // end todo
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
        /// 创建人信息记录ID；表daoben_hr_emp_job_history
        /// </summary>
        public string creator_job_history_id { get; set; }
        /// <summary>
        /// 创建人职位名称
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

    }
}