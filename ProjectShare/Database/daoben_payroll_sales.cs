using System;

namespace ProjectShare.Database
{
    public class daoben_payroll_sales
    {
        /// <summary>
        /// 业务工资结算表
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 员工ID
        /// </summary>
        public string emp_id { get; set; }
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
        /// 岗位薪资标准
        /// </summary>
        public decimal position_standard_salary { get; set; }
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
        /// 上班天数
        /// </summary>
        public int onduty_day { get; set; }
        /// <summary>
        /// 工龄工资
        /// </summary>
        public decimal seniority_salary { get; set; }
        /// <summary>
        /// 车补；-1表示无此项
        /// </summary>
        public decimal traffic_subsidy { get; set; }        
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
        public int insurance_fee { get; set; }
        /// <summary>
        /// 风险金，一般为负值
        /// </summary>
        public int resign_deposit { get; set; }
        /// <summary>
        /// 累计风险金（不含本月），一般为负值
        /// </summary>
        public decimal deposit_total { get; set; }
        /// <summary>
        /// 请假扣款，一般为负值
        /// </summary>
        public int leaving_deduction { get; set; }
        /// <summary>
        /// 区域经理代管补助
        /// </summary>
        public int proxy_subsidy { get; set; }
        /// <summary>
        /// 实发工资
        /// </summary>
        public decimal actual_salary { get; set; }
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
        /// 姓名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 入职时间
        /// </summary>
        public DateTime entry_date { get; set; }
        /// <summary>
        /// 等级（含职等、星级）
        /// </summary>
        public string grade { get; set; }
        /// <summary>
        /// 职位ID，表daoben_org_position
        /// </summary>
        public int position_id { get; set; }
        /// <summary>
        /// 职位名称
        /// </summary>
        public string position_name { get; set; }
        
        /// <summary>
        /// 所属机构的上级机构ID，表daoben_org_company
        /// </summary>
        public int company_id_parent { get; set; }
        /// <summary>
        /// 所属机构ID，表daoben_org_company
        /// </summary>
        public int company_id { get; set; }
        /// <summary>
        /// 所属机构名称
        /// </summary>
        public string company_name { get; set; }
        /// <summary>
        /// 活动状态：-2-审核未结束；-1未开始；1-进行中；2-已结束
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