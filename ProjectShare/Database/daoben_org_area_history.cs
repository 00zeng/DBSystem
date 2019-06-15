using System;

namespace ProjectShare.Database
{
    public class daoben_org_area_history
    {
        /// <summary>
        /// 调整记录表
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 区域ID
        /// </summary>
        public int area_id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 所属经理片区ID（仅业务片区）
        /// </summary>
        public int parent_id { get; set; }
        /// <summary>
        /// 1-经理片区；2-业务片区（预留功能）
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 所属机构ID，表daoben_org_company
        /// </summary>
        public int company_id { get; set; }
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
        public daoben_org_area_history()
        {
            create_time = DateTime.Now;
            inactive = false;
            inactive_job_history_id = null;
            inactive_time = null;
        }
    }
}