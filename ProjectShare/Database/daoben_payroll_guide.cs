using System;

namespace ProjectShare.Database
{
    public class daoben_payroll_guide
    {
        /// <summary>
        /// 导购工资结算表
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 底薪
        /// </summary>
        public decimal base_salary { get; set; }
        /// <summary>
        /// 导购员底薪类型：0-0底薪，1-达标底薪，2-星级制，3-浮动底薪，4-保底工资
        /// </summary>
        public int base_type { get; set; }
        /// <summary>
        /// 实销提成
        /// </summary>
        public decimal sale_commission { get; set; }
        /// <summary>
        /// 用于核算达标底薪的提成
        /// </summary>
        public decimal commission_basesalary { get; set; }        
        /// <summary>
        /// 当月实销
        /// </summary>
        public int sale_count { get; set; }
        /// <summary>
        /// 包销提成
        /// </summary>
        public decimal exclusive_commission { get; set; }

        /// <summary>
        /// 当月包销
        /// </summary>
        public int exclusive_count { get; set; }
        /// <summary>
        /// 买断提成
        /// </summary>
        public decimal buyout_commission { get; set; }
        /// <summary>
        /// 当月买断
        /// </summary>
        public int buyout_count { get; set; }
        /// <summary>
        /// 增员奖
        /// </summary>
        public decimal increase_reward { get; set; }
        /// <summary>
        /// 增员人数
        /// </summary>
        public int increase_count { get; set; }
        /// <summary>
        /// 增员提成
        /// </summary>
        public decimal increase_commission { get; set; }
        /// <summary>
        /// 增员销量
        /// </summary>
        public int increase_sale_count { get; set; }
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
        /// 风险金，一般为负值
        /// </summary>
        public int resign_deposit { get; set; }
        /// <summary>
        /// 社保，一般为负值
        /// </summary>
        public decimal insurance_fee { get; set; }
       
        /// <summary>
        /// 请假扣款，一般为负值
        /// </summary>
        public decimal leaving_deduction { get; set; }
        /// <summary>
        /// 累计风险金（不含本月），一般为负值
        /// </summary>
        public decimal deposit_total { get; set; }
        /// <summary>
        /// 实发工资
        /// </summary>
        public decimal actual_salary { get; set; }
        /// <summary>
        /// 岗位薪资标准
        /// </summary>
        public decimal position_standard_salary { get; set; }
        /// <summary>
        /// 月份，含年
        /// </summary>
        public DateTime month { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string note { get; set; }
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
        /// 工资支付状态：-2-审核未结束；-1未支付；2-已支付
        /// </summary>
        public int paid_status { get; set; }
        /// <summary>
        /// 审批状态：0-未审批；100表示审批完成且通过
        /// </summary>
        public int approve_status { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime create_time { get; set; }
        /// <summary>
        /// 创建人信息记录ID；表daoben_hr_emp_job_history
        /// </summary>
        public string creator_job_history_id { get; set; }
        /// <summary>
        /// 创建人姓名（非账户名）
        /// </summary>
        public string creator_name { get; set; }

    }
}