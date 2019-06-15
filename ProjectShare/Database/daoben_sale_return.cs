using System;

namespace ProjectShare.Database
{
    public class daoben_sale_return
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
        /// 颜色
        /// </summary>
        public string color { get; set; }
        /// <summary>
        /// 最终批发价（有调价补差时该值为新批发价）
        /// </summary>
        public decimal price_wholesale { get; set; }
        
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
        /// 日期
        /// </summary>
        public DateTime accur_time { get; set; }
        /// <summary>
        /// 表daoben_sales_outstorage_approve
        /// </summary>
        public string import_file_id { get; set; }

    }
}