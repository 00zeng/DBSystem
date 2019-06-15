using System;

namespace ProjectShare.Database
{
    public class daoben_hr_guide_link
    {
        /// <summary>
        /// 导购员关系表
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 导购员ID
        /// </summary>
        public string guide_id { get; set; }
        /// <summary>
        /// 业务员或者培训师ID表daoben_hr_emp_info
        /// </summary>
        public string emp_id { get; set; }
        /// <summary>
        /// 员工类型：1-业务员,2-培训师
        /// </summary>
        public int emp_type { get; set; }
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