using System;

namespace ProjectShare.Database
{
    public class daoben_payroll_office
    {
        /// <summary>
        /// 行政工资结算表
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 底薪
        /// </summary>
        public int base_salary { get; set; }
        /// <summary>
        /// 实习工资结算方式：1-按照总工资比例；2-按固定金额
        /// </summary>
        public int intern_salary_type { get; set; }
        /// <summary>
        /// 实习工资
        /// </summary>
        public decimal intern_salary { get; set; }
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
        /// 上班天数
        /// </summary>
        public decimal work_days { get; set; }
        /// <summary>
        /// 实际上班天数
        /// </summary>
        public decimal onduty_day { get; set; }
        /// <summary>
        /// 工龄工资
        /// </summary>
        public decimal seniority_salary { get; set; }
        /// <summary>
        /// 车补；-1表示无此项
        /// </summary>
        public decimal traffic_subsidy { get; set; }
        /// <summary>
        /// 评分后的总KPI工资(含KPI补助)
        /// </summary>
        public decimal kpi { get; set; }
        /// <summary>
        /// 基本KPI的基础金额（元）
        /// </summary>
        public decimal kpi_standard { get; set; }
        /// <summary>
        /// 基本KPI评分
        /// </summary>
        public decimal kpi_standard_score { get; set; }
        /// <summary>
        /// KPI补助的基础金额（元）
        /// </summary>
        public decimal kpi_subsidy { get; set; }
        /// <summary>
        /// KPI补助评分；-1表示全额发放
        /// </summary>
        public decimal kpi_subsidy_score { get; set; }
        /// <summary>
        /// 薪资补助（特聘补贴）
        /// </summary>
        public decimal salary_subsidy { get; set; }
        /// <summary>
        /// 应发工资
        /// </summary>
        public decimal salary { get; set; }
        /// <summary>
        /// 留守补助-公司
        /// </summary>
        public int stay_subsidy_company { get; set; }
        /// <summary>
        /// 留守补助-个人
        /// </summary>
        public int stay_subsidy_personal { get; set; }
        /// <summary>
        /// 社保，一般为负值
        /// </summary>
        public decimal insurance_fee { get; set; }
        /// <summary>
        /// 风险金，一般为负值
        /// </summary>
        public decimal resign_deposit { get; set; }
        /// <summary>
        /// 请假扣款，一般为负值
        /// </summary>
        public decimal leaving_deduction { get; set; }
        /// <summary>
        /// 实发工资
        /// </summary>
        public decimal actual_salary { get; set; }
        /// <summary>
        /// 岗位薪资标准
        /// </summary>
        public decimal position_standard_salary { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string note { get; set; }
        /// <summary>
        /// 月份，含年
        /// </summary>
        public DateTime month { get; set; }
        /// <summary>
        /// 起始日期
        /// </summary>
        public DateTime start_date { get; set; }
        /// <summary>
        /// 截止日期
        /// </summary>
        public DateTime end_date { get; set; }
        
        /// <summary>
        /// 员工ID
        /// </summary>
        public string emp_id { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 员工信息记录ID；表daoben_hr_emp_job_history
        /// </summary>
        public string job_history_id { get; set; }
        /// <summary>
        /// ///工资支付状态：-2-审核未结束；-1未支付；2-已支付
        /// </summary>
        public int paid_status { get; set; }
        /// <summary>
        /// 审批状态：0-未审批；100表示审批完成且通过
        /// </summary>
        public int approve_status { get; set; }
        /// <summary>
        /// 创建人信息记录ID；表daoben_hr_emp_job_history
        /// </summary>
        public string creator_job_history_id { get; set; }
        /// <summary>
        /// 创建人名称
        /// </summary>
        public string creator_name { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime create_time { get; set; }
    }
}