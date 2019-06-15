using System;

namespace ProjectShare.Database
{
    public class daoben_sale_buyout_import_temp
    {
        /// <summary>
        /// 买断导入临时表。仓库买断导入时先存放在该表，导入审批通过后再复制信息到daoben_sale_buyout
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 申请表ID
        /// </summary>
        public string main_id { get; set; }
        /// <summary>
        /// 单据名
        /// </summary>
        public string bill_name { get; set; }
        /// <summary>
        /// 经销商（实销售点）
        /// </summary>
        public string distributor_id { get; set; }
        /// <summary>
        /// 经销商（实销售点）
        /// </summary>
        public string distributor_name { get; set; }
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
        public string company_linkname { get; set; }
        /// <summary>
        /// 经销商编号
        /// </summary>
        public string distributor_code { get; set; }
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
        /// 业务片区
        /// </summary>
        public string area_l2_name { get; set; }
       
        /// <summary>
        /// 串码
        /// </summary>
        public string phone_sn { get; set; }
        /// <summary>
        /// 状态（终端售出等）
        /// </summary>
        public string sale_status { get; set; }
        /// <summary>
        /// 型号
        /// </summary>
        public string model { get; set; }
        /// <summary>
        /// 颜色
        /// </summary>
        public string color { get; set; }
        /// <summary>
        /// 最后操作日期
        /// </summary>
        public DateTime? accur_time { get; set; }
        /// <summary>
        /// 批发价
        /// </summary>
        public decimal price_wholesale { get; set; }
        /// <summary>
        /// 买断价
        /// </summary>
        public decimal price_buyout { get; set; }
        /// <summary>
        /// 零售价
        /// </summary>
        public decimal price_retail { get; set; }
        /// <summary>
        /// 导购员
        /// </summary>
        public string guide_id { get; set; }
        /// <summary>
        /// 导购员（促销员）
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
        /// 买断补差
        /// </summary>
        public decimal buyout_refund { get; set; }
        /// <summary>
        /// 促销员/导购员提成
        /// </summary>
        public decimal guide_commission { get; set; }
        /// <summary>
        /// 业务提成
        /// </summary>
        public decimal sales_commission { get; set; }
        /// <summary>
        /// 表daoben_sales_buyout_import_approve
        /// </summary>
        public string import_file_id { get; set; }
        /// <summary>
        /// 数据检查状态：0-未检查；1-检查通过；
        ///     21-相同串码不同信息，未处理；23-相同串码不同信息，已删除；
        ///     -100：审批不通过
        /// </summary>
        public int check_status { get; set; }
    }
}