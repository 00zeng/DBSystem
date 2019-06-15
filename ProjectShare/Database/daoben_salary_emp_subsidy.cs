using System;

namespace ProjectShare.Database
{
    public class daoben_salary_emp_subsidy
    {
        /// <summary>
        /// 薪资设置表-其他补助表
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 主表ID（表daoben_salary_emp）
        /// </summary>
        public string main_id { get; set; }
        /// <summary>
        /// 其他补助
        /// </summary>
        public decimal subsidy { get; set; }
        /// <summary>
        /// 其他补助说明
        /// </summary>
        public string subsidy_note { get; set; }


    }
}