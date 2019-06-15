using System;

namespace ProjectShare.Database
{
    public class daoben_org_company_history
    {
        /// <summary>
        /// 调整记录表
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 机构ID
        /// </summary>
        public int company_id { get; set; }
        /// <summary>
        /// 机构名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 状态：1-正常；-1-已删除
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 创建人信息记录ID；表daoben_hr_emp_job_history
        /// </summary>
        public string creator_job_history_id { get; set; }
        /// <summary>
        /// 创建人姓名（非账户名
        /// </summary>
        public string creator_name { get; set; }
        /// <summary>
        /// 创建时间（生效时间）
        /// </summary>
        public DateTime create_time { get; set; }
        /// <summary>
        /// 生效时间
        /// </summary>
        public DateTime effect_date { get; set; }
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
        public daoben_org_company_history()
        {
            create_time = DateTime.Now;
            inactive = false;
            inactive_job_history_id = null;
            inactive_time = null;
        }      
    }
}