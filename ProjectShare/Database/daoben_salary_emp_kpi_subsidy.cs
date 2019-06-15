using System;

namespace ProjectShare.Database
{
    public class daoben_salary_emp_kpi_subsidy
    {
        /// <summary>
        /// 薪资设置表-KPI补助表
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 主表ID（表daoben_salary_emp）
        /// </summary>
        public string main_id { get; set; }
        /// <summary>
        /// KPI补助
        /// </summary>
        public decimal kpi_subsidy { get; set; }
        /// <summary>
        /// 是否全额发放：1全额发放；0默认按照评分比例发放
        /// </summary>
        public bool kpi_subsidy_full { get; set; }
        /// <summary>
        /// KPI补助说明
        /// </summary>
        public string kpi_subsidy_note { get; set; }


    }
}