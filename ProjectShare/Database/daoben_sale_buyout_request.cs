using System;

namespace ProjectShare.Database
{
    public class daoben_sale_buyout_request
    {
        /// <summary>
        /// 买断申请表（含仓库买断和门店买断）
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 买断类型：1-仓库买断；2-门店买断
        /// </summary>
        public int category { get; set; }
        /// <summary>
        /// 经销商
        /// </summary>
        public string distributor_id { get; set; }
        /// <summary>
        /// 经销商（导入时经销商名称一般含编码，故在创建经销商时需以此为经销商名称）
        /// </summary>
        public string distributor_name { get; set; }
        /// <summary>
        /// 导购员
        /// </summary>
        public string guide_id { get; set; }
        /// <summary>
        /// 导购员
        /// </summary>
        public string guide_name { get; set; }
        /// <summary>
        /// 业务员（业务片区）
        /// </summary>
        public string sales_id { get; set; }
        /// <summary>
        /// 业务员（业务片区）
        /// </summary>
        public string sales_name { get; set; }
        /// <summary>
        /// 业务经理（区域经理）
        /// </summary>
        public string salesmanager_id { get; set; }
        /// <summary>
        /// 业务经理（区域经理）
        /// </summary>
        public string salesmanager_name { get; set; }
        /// <summary>
        /// 备注说明
        /// </summary>
        public string note { get; set; }
        /// <summary>
        /// 审批状态：0-未审批；100表示审批完成且通过
        /// </summary>
        public int approve_status { get; set; }
        /// <summary>
        /// 0-人工审批；1-超时系统退审（仅在“分公司助理”审批节点使用，用于罚款）
        /// </summary>
        public bool system_approve { get; set; }
        /// <summary>
        /// 是否需要事业部总经理审批：0-不需要；1-需要
        /// </summary>
        public bool gm1_approve { get; set; }
        /// <summary>
        /// 是否需要分公司总经理审批：0-不需要；1-需要
        /// </summary>
        public bool gm2_approve { get; set; }
        /// <summary>
        /// 总买断金额（元）
        /// </summary>
        public decimal total_amount { get; set; }
        /// <summary>
        /// 总台数
        /// </summary>
        public int total_count { get; set; }
        /// <summary>
        /// 总补差
        /// </summary>
        public decimal total_refund { get; set; }
        /// <summary>
        /// 促销员/导购员总提成
        /// </summary>
        public int total_guide_commission { get; set; }
        /// <summary>
        /// 业务总提成
        /// </summary>
        public int total_sales_commission { get; set; }
        /// <summary>
        /// 区域
        /// </summary>
        public int area_id { get; set; }
        /// <summary>
        /// 区域
        /// </summary>
        public string area_name { get; set; }
        /// <summary>
        /// 所属机构的上级机构ID，表daoben_org_company
        /// </summary>
        public int company_id_parent { get; set; }
        /// <summary>
        /// 所属机构ID，表daoben_org_company
        /// </summary>
        public int company_id { get; set; }
        /// <summary>
        /// 所属机构名称
        /// </summary>
        public string company_name { get; set; }
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