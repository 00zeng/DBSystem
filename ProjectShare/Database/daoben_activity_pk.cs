using System;

namespace ProjectShare.Database
{
    public class daoben_activity_pk
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 比赛名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 参与对象类型：3-导购员；20-业务员；21-业务经理
        /// </summary>
        public int emp_category { get; set; }
        /// <summary>
        /// 比赛开始日期；
        /// </summary>
        public DateTime? start_date { get; set; }
        /// <summary>
        /// 比赛结束日期；
        /// </summary>
        public DateTime end_date { get; set; }
        /// <summary>
        /// PK组数
        /// </summary>
        public int pk_group_count { get; set; }
        /// <summary>
        /// 活动机型：1-全部机型；2-指定单一型号（副表）；3-指定多个型号（副表）
        /// </summary>
        public int product_scope { get; set; }
        /// <summary>
        /// 赢取对方金额（元）
        /// </summary>
        public int win_lose { get; set; }
        /// <summary>
        /// 公司奖励获胜方金额（元）。0-不勾选
        /// </summary>
        public int win_company { get; set; }
        /// <summary>
        /// 公司奖励条件：1-仅当双方完成率都达到100%时；2-只要获胜方完成率达到100%
        /// </summary>
        public int win_company_condition { get; set; }
        /// <summary>
        /// 获胜方完成率低于100%，罚款金额（元）。0-不勾选
        /// </summary>
        public int win_penalty { get; set; }
        /// <summary>
        /// 输给对方金额（元）
        /// </summary>
        public int lose_win { get; set; }
        /// <summary>
        /// 失败方完成率低于100%，罚款金额。0-不勾选
        /// </summary>
        public int lose_penalty { get; set; }
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
        public decimal total_sale_amount { get; set; }
        /// <summary>
        /// 累计公司奖金（元）
        /// </summary>
        public decimal total_company_reward { get; set; }
        /// <summary>
        /// 累计罚款（元）
        /// </summary>
        public decimal total_penalty { get; set; }
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
        public DateTime create_time { get; set; }
    }
}