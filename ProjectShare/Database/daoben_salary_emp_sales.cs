using System;

namespace ProjectShare.Database
{
    public class daoben_salary_emp_sales
    {
        /// <summary>
        /// 员工薪资-业务薪资附表
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 员工薪资表daoben_salary_emp(主表)ID
        /// </summary>
        public string salary_position_id { get; set; }
        /// <summary>
        /// 实习工资结算方式：1-按照总工资比例；2-按固定金额
        /// </summary>
        public int intern_salary_type { get; set; }
        /// <summary>
        /// 实习期工资：按固定金额时的工资值
        /// </summary>
        public decimal intern_fix_salary { get; set; }
        /// <summary>
        /// 实习期工资：比例*100；
        /// </summary>
        public int intern_ratio_salary { get; set; }
        /// <summary>
        /// 离职押金（元/月）
        /// </summary>
        public int resign_deposit { get; set; }
        /// <summary>
        /// 交通补贴
        /// </summary>
        public decimal traffic_subsidy { get; set; }
        /// <summary>
        /// 计量方式：1-按实销量；2-按下货量
        /// </summary>
        public int target_content { get; set; }
        /// <summary>
        /// 奖励模式：1-按完成率；2-按台数；3-按零售价；4-按型号；5-按批发价
        /// </summary>
        public int target_mode { get; set; }
        /// <summary>
        /// 正常机：金额计算方式：1-每台固定金额；2-每台批发价比例；3-每台零售价比例；4-固定总金额
        /// </summary>
        public int normal_rebate_mode { get; set; }
        /// <summary>
        /// 买断机：金额计算方式：1-每台固定金额；2-每台批发价比例；3-每台买断价比例；4-固定总金额
        /// </summary>
        public int buyout_rebate_mode { get; set; }
        /// <summary>
        /// 目标销量（台），仅当target_mode为“1-按完成率返利”有效
        /// </summary>
        public int activity_target { get; set; }
        /// <summary>
        /// 累计完成销量（台）
        /// </summary>
        public int total_sale_count { get; set; }
        /// <summary>
        /// 累计完成销量总金额（元）
        /// </summary>
        public decimal total_sale_amount { get; set; }
        /// <summary>
        /// 累计奖金（元）
        /// </summary>
        public decimal total_reward { get; set; }
        /// <summary>
        /// 累计罚款（元）
        /// </summary>
        public decimal total_penalty { get; set; }
        /// <summary>
        /// 备注说明
        /// </summary>
        public string note { get; set; }
    }
}