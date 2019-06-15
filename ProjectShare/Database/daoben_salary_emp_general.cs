using System;

namespace ProjectShare.Database
{
    public class daoben_salary_emp_general
    {
        /// <summary>
        /// 薪资设置表-常规薪资
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 主表ID（表daoben_salary_emp）
        /// </summary>
        public string main_id { get; set; }
        /// <summary>
        /// 离职押金（元/月）
        /// </summary>
        public int resign_deposit { get; set; }
        /// <summary>
        /// 交通补贴
        /// </summary>
        public decimal traffic_subsidy { get; set; }
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
        /// 导购员底薪类型：0-0底薪，1-达标底薪，2-星级制，3-浮动底薪，4-保底工资
        /// </summary>
        public int guide_base_type { get; set; }
        /// <summary>
        /// 导购员保底工资
        /// </summary>
        public decimal guide_salary_base { get; set; }
        /// <summary>
        /// 导购员达标底薪
        /// </summary>
        public decimal guide_standard_salary { get; set; }
        /// <summary>
        /// 导购员达标提成
        /// </summary>
        public decimal guide_standard_commission { get; set; }
        /// <summary>
        /// 年终奖方案类型：1-按销量，2-按星级
        /// </summary>
        public int guide_annualbonus_type { get; set; }

    }
}