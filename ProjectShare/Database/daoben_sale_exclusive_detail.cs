using System;

namespace ProjectShare.Database
{
    public class daoben_sale_exclusive_detail
    {
        /// <summary>
        /// 包销导入子表
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 主表ID
        /// </summary>
        public string main_id { get; set; }
        /// <summary>
        /// 经销商（实销售点）
        /// </summary>
        public string distributor_id { get; set; }
        /// <summary>
        /// 经销商（实销售点）
        /// </summary>
        public string distributor_name { get; set; }
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
        /// 业务片区（三级地区）
        /// </summary>
        public string area_l2_name { get; set; }
        /// <summary>
        /// 所属机构名称
        /// </summary>
        public string company_linkname { get; set; }
        /// <summary>
        /// 所属机构ID，表daoben_org_company
        /// </summary>
        public int company_id { get; set; }
        /// <summary>
        /// 所属机构的上级机构ID，表daoben_org_company
        /// </summary>
        public int company_id_parent { get; set; }
        /// <summary>
        /// 型号
        /// </summary>
        public string model { get; set; }
        /// <summary>
        /// 颜色
        /// </summary>
        public string color { get; set; }
        /// <summary>
        /// 串码
        /// </summary>
        public string phone_sn { get; set; }
        /// <summary>
        /// 批发价
        /// </summary>
        public decimal price_wholesale { get; set; }
        /// <summary>
        /// 包销价
        /// </summary>
        public decimal price_exclusive { get; set; }
        /// <summary>
        /// 包销补差
        /// </summary>
        public decimal refund { get; set; }
        /// <summary>
        /// 促销员/导购员提成
        /// </summary>
        public decimal guide_commission { get; set; }
        /// <summary>
        /// 业务员（业务片区）
        /// </summary>
        public string sales_id { get; set; }
        /// <summary>
        /// 业务员（业务片区）
        /// </summary>
        public string sales_name { get; set; }
        /// <summary>
        /// 最后操作日期
        /// </summary>
        public DateTime? accur_time { get; set; }
        /// <summary>
        /// 上报规则
        /// </summary>
        public string report_rule { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string note { get; set; }
        /// <summary>
        /// 数据检查状态：0-未检查；1-检查通过；21-相同串码不同信息，未处理；23-相同串码不同信息，已删除；-100：审批不通过
        /// </summary>
        public int check_status { get; set; }



    }
}