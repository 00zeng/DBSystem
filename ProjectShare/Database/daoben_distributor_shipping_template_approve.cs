using System;

namespace ProjectShare.Database
{
    public class daoben_distributor_shipping_template_approve
    {
        /// <summary>
        /// 运费，预留审批
        /// </summary>
        public string id { get; set; }
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
        /// 最低运费
        /// </summary>
        public int minimum_fee { get; set; }
        /// <summary>
        /// 免运费台数
        /// </summary>
        public int free_count { get; set; }
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