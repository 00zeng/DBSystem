namespace ProjectShare.Database
{
    public class daoben_salary_position_grade
    {
        /// <summary>
        /// 职等工资表
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
        /// 公司层面；部门层面
        /// </summary>
        public string grade_level_display { get; set; }
        /// <summary>
        /// 等级工资(除导购员)类型。事业部：1-行政管理、4-运营商中心；分公司：1-行政管理、2-市场销售、3-终端管理、4-运营商中心
        /// </summary>
        public int grade_category { get; set; }
        /// <summary>
        /// 行政管理、运营商中心、市场销售、终端管理
        /// </summary>
        public string grade_category_display { get; set; }
        /// <summary>
        /// 职位显示
        /// </summary>
        public string position_display { get; set; }
        /// <summary>
        /// 基本工资
        /// </summary>
        public int base_salary { get; set; }
        /// <summary>
        /// 岗位工资
        /// </summary>
        public int position_salary { get; set; }
        /// <summary>
        /// 住房补贴
        /// </summary>
        public int house_subsidy { get; set; }
        /// <summary>
        /// 全勤奖
        /// </summary>
        public int attendance_reward { get; set; }
        /// <summary>
        /// 工龄工资显示
        /// </summary>
        public string seniority_wage { get; set; }
        /// <summary>
        /// 车费补贴显示
        /// </summary>
        public string traffic_subsidy_display { get; set; }
        /// <summary>
        /// KPI参考值
        /// </summary>
        public string kpi_advice { get; set; }
        /// <summary>
        /// 合计
        /// </summary>
        public int total { get; set; }
    }
}