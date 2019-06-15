using System;

namespace ProjectShare.Database
{
    public class daoben_salary_position_dept
    {
        /// <summary>
        /// 行政部门职等KPI设置
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 岗位薪资表daoben_salary_position(主表)ID
        /// </summary>
        public string salary_position_id { get; set; }
        /// <summary>
        /// 职等
        /// </summary>
        public string grade { get; set; }
        /// <summary>
        /// 职层：1-公司层面；2-部门层面
        /// </summary>
        public int grade_level { get; set; }
        /// <summary>
        /// 等级工资(除导购员)类型。事业部：1-行政管理、4-运营商中心；分公司：1-行政管理、2-市场销售、3-终端管理、4-运营商中心
        /// </summary>
        public int grade_category { get; set; }
        /// <summary>
        /// KPI建议金额（元）
        /// </summary>
        public string kpi_advice { get; set; }
        /// <summary>
        /// KPI标准金额（元）
        /// </summary>
        public decimal kpi_standard { get; set; }

    }
}