using System;

namespace ProjectShare.Database
{
    public class daoben_salary_position_other
    {
        /// <summary>
        /// 岗位薪资-其他
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 岗位薪资表daoben_salary_position(主表)ID
        /// </summary>
        public string salary_position_id { get; set; }
        
        /// <summary>
        /// 离职押金（元/月）
        /// </summary>
        public int resign_deposit { get; set; }
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
        /// 将所有个人设置重置为此方案：0-不重置；1-重置；（仅离职押金 及实习工资）
        /// </summary>
        public bool reset_all { get; set; }

    }
}