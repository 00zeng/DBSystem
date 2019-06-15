using System;

namespace ProjectShare.Database
{
    public class daoben_payroll_template
    {
        /// <summary>
        /// 员工ID，同daoben_hr_emp_job
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 所属机构ID
        /// </summary>
        public int company_id { get; set; }
        /// <summary>
        /// 新入职工资是否未设置：0-已设置；1-未设置
        /// </summary>
        public bool salary_blank { get; set; }
        /// <summary>
        /// 0-无对应职等工资；1-无更新；2-当月职等工资有更新
        /// </summary>
        public int grade_salary_status { get; set; }
        /// <summary>
        /// 底薪
        /// </summary>
        public int base_salary { get; set; }
        /// <summary>
        /// 岗位工资
        /// </summary>
        public int position_salary { get; set; }
        /// <summary>
        /// 住房补贴；-1表示无此项
        /// </summary>
        public int house_subsidy { get; set; }
        /// <summary>
        /// 全勤奖；-1表示无此项
        /// </summary>
        public int attendance_reward { get; set; }
        /// <summary>
        /// 车补；-1表示无此项
        /// </summary>
        public int traffic_subsidy { get; set; }
        /// <summary>
        /// 工龄工资
        /// </summary>
        public int seniority_salary { get; set; }
        /// <summary>
        /// 工龄（月，含当月，当月16号前入职的计算工龄）
        /// </summary>
        public int seniority_month { get; set; }
        /// <summary>
        /// KPI参考值
        /// </summary>
        public string kpi_advice { get; set; }
        /// <summary>
        /// KPI基数(不含KPI补助)
        /// </summary>
        public decimal kpi_standard { get; set; }
        /// <summary>
        /// KPI补助基数
        /// </summary>
        public decimal kpi_subsidy { get; set; }
        /// <summary>
        /// KPI补助是否全额发放：1全额发放；0默认按照评分比例发放
        /// </summary>
        public bool kpi_subsidy_full { get; set; }
        /// <summary>
        /// 薪资补助（特聘补贴）
        /// </summary>
        public int salary_subsidy { get; set; }
        /// <summary>
        /// 社保，一般为负值
        /// </summary>
        public int insurance_fee { get; set; }
        /// <summary>
        /// 风险金，一般为负值
        /// </summary>
        public int resign_deposit { get; set; }
        /// <summary>
        /// 累计风险金（不含本月，每月更新），一般为负值
        /// </summary>
        public int deposit_total { get; set; }
        /// <summary>
        /// 入职时间
        /// </summary>
        public DateTime? entry_date { get; set; }
        /// <summary>
        /// 0-正式工；1-实习生
        /// </summary>
        public bool is_internship { get; set; }
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
    }
}