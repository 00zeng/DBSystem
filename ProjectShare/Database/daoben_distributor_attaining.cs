using System;

namespace ProjectShare.Database
{
    public class daoben_distributor_attaining
    {
        /// <summary>
        /// 经销商达量返利表
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 活动名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 活动开始日期；
        /// </summary>
        public DateTime? start_date { get; set; }
        /// <summary>
        /// 活动结束日期；
        /// </summary>
        public DateTime? end_date { get; set; }
        /// <summary>
        /// 参与经销商数
        /// </summary>
        public int distributor_count { get; set; }
        /// <summary>
        /// 特价机算钱（预留扩展）：0-特价机算量不算钱；1-特价机算量算钱
        /// </summary>
        public int money_included { get; set; }
        /// <summary>
        /// 返利类型：1、达量返利、2、主推返利、3、订货返利、4、无促保卡返利
        /// </summary>
        public int category { get; set; }
        /// <summary>
        /// 奖励发放方式：1-一次性发放；2-按月发放
        /// </summary>
        public int pay_mode { get; set; }
        /// <summary>
        /// 计量方式：1-按实销量；2-按下货量
        /// </summary>
        public int target_content { get; set; }
        /// <summary>
        /// 销量阶梯：1-按完成率；2-按照台数
        /// </summary>
        public int target_sale { get; set; }
        /// <summary>
        /// 目标销量（台），仅当target_mode为“1-按完成率返利”有效
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
        /// <summary>
        /// 累计完成销量（台）
        /// </summary>
        public int total_sale_count { get; set; }
        /// <summary>
        /// 累计完成销量总金额（元）
        /// </summary>
        public int total_sale_amount { get; set; }
        /// <summary>
        /// 累计返利（元）
        /// </summary>
        public int total_rebate { get; set; }
        /// <summary>
        /// 统计时间
        /// </summary>
        public DateTime? counting_time { get; set; }
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
        /// 创建人姓名（非账户名）
        /// </summary>
        public string creator_name { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? create_time { get; set; }
    }
}