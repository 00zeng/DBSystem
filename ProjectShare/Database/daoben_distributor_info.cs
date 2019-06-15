using System;

namespace ProjectShare.Database
{
    public class daoben_distributor_info
    {
        /// <summary>
        /// 经销商ID（录入/导入/自动生成）
        /// </summary>
        public string id { get; set; }
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
        /// <summary>
        /// 所属代理商
        /// </summary>
        public string agent { get; set; }
        /// <summary>
        /// 连锁体系
        /// </summary>
        public string system_chain { get; set; }
        /// <summary>
        /// TOP层级客户
        /// </summary>
        public string top_customers { get; set; }
        /// <summary>
        /// 体系调拨
        /// </summary>
        public string system_allocation { get; set; }
        /// <summary>
        /// 地区属性
        /// </summary>
        public string locale_attribute { get; set; }
        /// <summary>
        /// 客户类别
        /// </summary>
        public string customer_category { get; set; }
        /// <summary>
        /// 客户级别
        /// </summary>
        public string customer_level { get; set; }
        /// <summary>
        /// 经销商属性：非自营；自营
        /// </summary>
        public string distributor_attribute { get; set; }
        /// <summary>
        /// 1-直营（自营）；2-加盟店（非自营）
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 运营商属性：移动；联通；电信
        /// </summary>
        public string sp_attribute { get; set; }
        /// <summary>
        /// 潜在售点：是；否
        /// </summary>
        public string potential_salepoint { get; set; }
        /// <summary>
        /// 经营品牌
        /// </summary>
        public string brand { get; set; }
        /// <summary>
        /// 年销售规模
        /// </summary>
        public string annual_sales_volume { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string phone { get; set; }
        /// <summary>
        /// 业务员
        /// </summary>
        public string salesman { get; set; }
        /// <summary>
        /// 传真
        /// </summary>
        public string fax { get; set; }
        /// <summary>
        /// 所在省市，包含省
        /// </summary>
        public string city { get; set; }
        /// <summary>
        /// 所在省市代码
        /// </summary>
        public string city_code { get; set; }
        /// <summary>
        /// 收货地址
        /// </summary>
        public string address { get; set; }
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string contact_name { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string contact_phone { get; set; }
        /// <summary>
        /// 经营模式
        /// </summary>
        public string operation_mode { get; set; }
        /// <summary>
        /// 价格级别
        /// </summary>
        public string price_level { get; set; }
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
        /// 所属机构名称
        /// </summary>
        public string company_name { get; set; }
        /// <summary>
        /// 所属机构的上级机构ID，表daoben_org_company
        /// </summary>
        public int company_id_parent { get; set; }
        /// <summary>
        /// 所属机构完整名称链，表daoben_org_company
        /// </summary>
        public string company_linkname { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string note { get; set; }
        /// <summary>
        /// 0-正常；1-注销
        /// </summary>
        public bool inactive { get; set; }
        /// <summary>
        /// 业务员员工ID
        /// </summary>
        public string sales_id { get; set; }
        /// <summary>
        /// 业务员名称
        /// </summary>
        public string sales_name { get; set; }
        ///// <summary>
        ///// 创建人信息记录ID；表daoben_hr_emp_job_history
        ///// </summary>
        //public string creator_job_history_id { get; set; }
        ///// <summary>
        ///// 创建人名称
        ///// </summary>
        //public string creator_name { get; set; }
        ///// <summary>
        ///// 创建时间
        ///// </summary>
        //public DateTime? create_time { get; set; }


    }
}