using System;

namespace ProjectShare.Database
{
    public class daoben_distributor_recommendation
    {
        /// <summary>
        /// 经销商主推返利表
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
        /// 活动机型：1-全部机型；2-指定型号（副表）
        /// </summary>
        public int product_scope { get; set; }
        /// <summary>
        /// 计量方式：1-按实销量；2-按下货量
        /// </summary>
        public int target_content { get; set; }
        /// <summary>
        /// 考核模式：1-按完成率返利；2-按台数返利；3-按零售价返利；4-按型号返利
        /// </summary>
        public int target_mode { get; set; }
        /// <summary>
        /// 金额计算方式：1-每台固定金额；2-每台批发价比例；3-每台零售价比例；4-固定总金额
        /// </summary>
        public int rebate_mode { get; set; }
        /// <summary>
        /// 目标销量（台），仅当target_mode为“1-按完成率返利”有效
        /// </summary>
        public int activity_target { get; set; }
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
        /// 统计时间（中途统计）
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
        /// 创建人姓名（非账户名）
        /// </summary>
        public string creator_name { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? create_time { get; set; }


    }
}