using System;

namespace ProjectShare.Database
{
    public class daoben_sale_refund
    {
        /// <summary>
        /// id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 单据名
        /// </summary>
        public string bill_name { get; set; }
        /// <summary>
        /// 型号
        /// </summary>
        public string model { get; set; }
        /// <summary>
        /// 串码
        /// </summary>
        public string phone_sn { get; set; }
        /// <summary>
        /// 经销商
        /// </summary>
        public string distributor_id { get; set; }
        /// <summary>
        /// 经销商（导入时经销商名称一般含编码，故在创建经销商时需以此为经销商名称）
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
        /// 业务片区
        /// </summary>
        public string area_l2_name { get; set; }
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
        /// 所属机构名称
        /// </summary>
        public string company_linkname { get; set; }
        /// <summary>
        /// 原批发价
        /// </summary>
        public decimal orig_price { get; set; }
        /// <summary>
        /// 新批发价
        /// </summary>
        public decimal new_price { get; set; }
        /// <summary>
        /// 补差金额
        /// </summary>
        public decimal diff_price { get; set; }
        /// <summary>
        /// 调价日期
        /// </summary>
        public DateTime? accur_time { get; set; }
        /// <summary>
        /// 表daoben_sales_outstorage_approve
        /// </summary>
        public string import_file_id { get; set; }
    }
}