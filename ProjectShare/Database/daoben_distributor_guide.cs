using System;

namespace ProjectShare.Database
{
    public class daoben_distributor_guide
    {
        /// <summary>
        /// 导购员-经销商关系表
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 员工ID，表daoben_hr_emp_job
        /// </summary>
        public string guide_id { get; set; }
        /// <summary>
        /// 员工ID，表daoben_hr_emp_job
        /// </summary>
        public string guide_name { get; set; }
        /// <summary>
        /// 经销商ID
        /// </summary>
        public string distributor_id { get; set; }
        /// <summary>
        /// 经销商Name
        /// </summary>
        public string distributor_name { get; set; }
        /// <summary>
        /// 创建人信息记录ID；表daoben_hr_emp_job_history
        /// </summary>
        public string creator_job_history_id { get; set; }
        /// <summary>
        /// 创建人姓名（非账户名）
        /// </summary>
        public string creator_name { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime create_time { get; set; }
        /// <summary>
        /// 生效时间
        /// </summary>
        public DateTime? effect_date { get; set; }
        /// <summary>
        /// 0-有效；1-无效
        /// </summary>
        public bool inactive { get; set; }
        /// <summary>
        /// 失效操作人信息记录ID；表daoben_hr_emp_job_history
        /// </summary>
        public string inactive_job_history_id { get; set; }
        /// <summary>
        /// 失效操作时间
        /// </summary>
        public DateTime? inactive_time { get; set; }
    }
}