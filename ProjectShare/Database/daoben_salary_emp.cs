using System;

namespace ProjectShare.Database
{
    public class daoben_salary_emp
    {
        /// <summary>
        /// 薪资设置表-主表
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 员工ID，同表daoben_hr_emp_job
        /// </summary>
        public string emp_id { get; set; }
        /// <summary>
        /// 1-普通薪资设置（表daoben_salary_emp_general）；2-KPI补助（表daoben_salary_emp_kpi_subsidy）；3-薪资补助（daoben_salary_emp_subsidy）；
        /// 4-业务薪资 5-培训薪资
        /// </summary>
        public int category { get; set; }
        /// <summary>
        /// 生效时间
        /// </summary>
        public DateTime? effect_date { get; set; }
        /// <summary>
        /// 1-立即生效
        /// </summary>
        public bool effect_now { get; set; }
        /// <summary>
        /// -2-审核未结束；-1未生效；1有效；2-已失效
        /// </summary>
        public int effect_status { get; set; }
        /// <summary>
        /// 审批状态：0-未审批；100表示审批完成且通过
        /// </summary>
        public int approve_status { get; set; }
        /// <summary>
        /// 失效时间
        /// </summary>
        public DateTime? invalid_date { get; set; }
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