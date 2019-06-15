using System;

namespace ProjectShare.Database
{
    public class daoben_activity_attaining_emp
    {
        /// <summary>
        /// 达量奖人员表
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 主表ID，表daoben_activity_attaining
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
        /// 累计销量（台）
        /// </summary>
        public int total_count { get; set; }
        /// <summary>
        /// 算钱的销量(台)
        /// </summary>
        public int total_normal_count { get; set; }
        /// <summary>
        /// 累计销量总金额（元）
        /// </summary>
        public decimal total_amount { get; set; }
        /// <summary>
        /// 累计完成率（*100，如40表示40%）
        /// </summary>
        public decimal total_ratio { get; set; }
        /// <summary>
        /// 奖金，小于0表示罚款
        /// </summary>
        public decimal total_reward { get; set; }

    }
}