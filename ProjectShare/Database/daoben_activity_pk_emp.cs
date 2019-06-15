using System;

namespace ProjectShare.Database
{
    public class daoben_activity_pk_emp
    {
        /// <summary>
        /// PK比赛参赛人员表
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 主表ID，表daoben_activity_pk
        /// </summary>
        public string main_id { get; set; }
        /// <summary>
        /// 组号，相同的两条数据为一组
        /// </summary>
        public int group_number { get; set; }
        /// <summary>
        /// 员工ID
        /// </summary>
        public string emp_id { get; set; }
        /// <summary>
        /// 员工姓名
        /// </summary>
        public string emp_name { get; set; }
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
        /// 业务片区
        /// </summary>
        public string area_l2_name { get; set; }
        /// <summary>
        /// 目标销量（台）
        /// </summary>
        public int activity_target { get; set; }
        /// <summary>
        /// 累计销量（台）
        /// </summary>
        public int total_count { get; set; }
        /// <summary>
        /// 累计销量总金额（元）
        /// </summary>
        public decimal total_amount { get; set; }
        /// <summary>
        /// 累计完成率（*100，如40表示40%）
        /// </summary>
        public decimal total_ratio { get; set; }
        /// <summary>
        /// 总奖金，小于0表示罚款
        /// </summary>
        public decimal total_reward { get; set; }
        /// <summary>
        /// 公司奖金
        /// </summary>
        public decimal company_reward { get; set; }
        /// <summary>
        /// 罚款
        /// </summary>
        public decimal total_penalty { get; set; }
        /// <summary>
        /// 0-输；1-赢
        /// </summary>
        public bool winner { get; set; }


    }
}