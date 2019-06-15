using System;

namespace ProjectShare.Database
{
    public class daoben_distributor_info_history
    {
        public int id { get; set; }
        /// <summary>
        /// 经销商ID
        /// </summary>
        public string main_id { get; set; }
        /// <summary>
        /// 经理片区，表daoben_org_area
        /// </summary>
        public int area_l1_id { get; set; }
        /// <summary>
        /// 经理片区名称
        /// </summary>
        public string area_l1_name { get; set; }
        /// <summary>
        /// 业务片区，表daoben_org_area
        /// </summary>
        public int area_l2_id { get; set; }
        /// <summary>
        /// 业务片区名称
        /// </summary>
        public string area_l2_name { get; set; }
        /// <summary>
        /// 所属机构ID，表daoben_org_company
        /// </summary>
        public int company_id { get; set; }
        /// <summary>
        /// 所属机构的上级机构ID，表daoben_org_company
        /// </summary>
        public int company_id_parent { get; set; }
        /// <summary>
        /// 所属机构完整名称链，表daoben_org_company
        /// </summary>
        public string company_linkname { get; set; }
        /// <summary>
        /// 状态：1-正常；-1-已删除
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 创建人信息记录ID；表daoben_hr_emp_job_history
        /// </summary>
        public string creator_job_history_id { get; set; }
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
        // 以下为非数据库列，用于存放临时数据，
        /// <summary>
        /// 经销商名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// V2系统报量名称
        /// </summary>
        public string name_v2 { get; set; }
        /// <summary>
        /// 快捷编码
        /// </summary>
        public string code { get; set; }

        public daoben_distributor_info_history()
        {
            create_time = DateTime.Now;
            inactive = false;
            inactive_job_history_id = null;
            inactive_time = null;
        }
    }
}