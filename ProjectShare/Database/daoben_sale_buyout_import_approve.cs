using System;

namespace ProjectShare.Database
{
    public class daoben_sale_buyout_import_approve
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 导入文件名
        /// </summary>
        public string import_file { get; set; }
        /// <summary>
        /// 审批状态：0-未审批；大于0通过；小于0不通过；以100作为审批完成的标志
        /// </summary>
        public int status { get; set; }
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
        /// <summary>
        /// 所属机构ID，表daoben_org_company
        /// </summary>
        public int company_id { get; set; }
        /// <summary>
        /// 所属机构名称
        /// </summary>
        public string company_name { get; set; }
        
        /// <summary>
        /// 审批人ID
        /// </summary>
        public int approve_id { get; set; }
        /// <summary>
        /// 审批人姓名（非账户名）
        /// </summary>
        public string approve_name { get; set; }
        /// <summary>
        /// 审批时间
        /// </summary>
        public DateTime? approve_time { get; set; }
        /// <summary>
        /// 审批职位ID
        /// </summary>
        public int approve_position_id { get; set; }
        /// <summary>
        /// 审批职位名称
        /// </summary>
        public string approve_position_name { get; set; }
        /// <summary>
        /// 审批备注
        /// </summary>
        public string approve_note { get; set; }

    }
}