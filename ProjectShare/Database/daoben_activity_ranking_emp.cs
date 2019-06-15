using System;

namespace ProjectShare.Database
{
    public class daoben_activity_ranking_emp
    {
        /// <summary>
        /// 排名比赛参赛人员表
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 主表ID，表daoben_activity_ranking
        /// </summary>
        public string main_id { get; set; }
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
        /// 最终排名
        /// </summary>
        public int final_place { get; set; }
        /// <summary>
        /// 统计排名
        /// </summary>
        public int counting_palce { get; set; }
        /// <summary>
        /// 统计时间
        /// </summary>
        public DateTime? counting_time { get; set; }
        /// <summary>
        /// 统计销量 实销/下货
        /// </summary>
        public int counting_count { get; set; }
        /// <summary>
        /// 统计金额 实销/下货
        /// </summary>
        public decimal counting_amount { get; set; }
        /// <summary>
        /// 奖励金额
        /// </summary>
        public decimal reward { get; set; }
        /// <summary>
        /// 0-奖励未发放；1-已发放
        /// </summary>
        public bool reward_paid { get; set; }

    }
}