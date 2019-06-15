using System;

namespace ProjectShare.Database
{
    public class daoben_salary_benefit
    {
        /// <summary>
        /// 员工福利设置表(主表)
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 不享受福利员工：1-未转正不享受福利；0-不含此条件
        /// </summary>
        public bool exclude_emp_status { get; set; }
        /// <summary>
        /// 不享受福利员工：入职时间晚于此时；null表示不含此条件
        /// </summary>
        public DateTime? exclude_entry_date { get; set; }
        /// <summary>
        /// 发放人数
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// 发放月份
        /// </summary>
        public DateTime? month { get; set; }
        /// <summary>
        /// 审批状态
        /// </summary>
        public int approve_status { get; set; }
        /// <summary>
        /// 所属机构ID，表daoben_org_company
        /// </summary>
        public int company_id { get; set; }
        /// <summary>
        /// 所属机构名称
        /// </summary>
        public string company_name { get; set; }
        /// <summary>
        /// 所属机构的上级机构ID，表daoben_org_company
        /// </summary>
        public int company_id_parent { get; set; }
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