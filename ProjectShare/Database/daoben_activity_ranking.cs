using System;

namespace ProjectShare.Database
{
    public class daoben_activity_ranking
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
        public DateTime start_date { get; set; }
        /// <summary>
        /// 比赛结束日期；
        /// </summary>
        public DateTime end_date { get; set; }
        /// <summary>
        /// 参与员工范围：1-全体；2-指定人员；3-指定区域
        /// </summary>
        public int emp_scope { get; set; }
        /// <summary>
        /// 参与员工人数
        /// </summary>
        public int emp_count { get; set; }
        /// <summary>
        /// 考核内容：1-按实销总量；2-按下货总量；3-按实销总金额（零售价）；4-按下货总金额（零售价）
        /// </summary>
        public int ranking_content { get; set; }
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
        /// 实际参与总人数
        /// </summary>
        public int actual_emp_count { get; set; }
        /// <summary>
        /// 累计完成总销量
        /// </summary>
        public int finished_count { get; set; }
        /// <summary>
        /// 获奖人数
        /// </summary>
        public int reward_emp_count { get; set; }
        /// <summary>
        /// 累计奖金（元）
        /// </summary>
        public decimal reward_amount { get; set; }
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
        /// 创建人职位名称
        /// </summary>
        public string creator_position_name { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime create_time { get; set; }

    }
}