using System;

namespace ProjectShare.Database
{
    public class daoben_sale_buyout
    {
        /// <summary>
        /// 买断详情表.门店买断发起申请时直接插入该表信息；仓库买断时，数据等到excel表导入时再插入该表
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 申请表ID
        /// </summary>
        public string main_id { get; set; }
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
        /// 最低买断价
        /// </summary>
        public decimal price_buyout { get; set; }
        /// <summary>
        /// 零售价
        /// </summary>
        public decimal price_retail { get; set; }
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
        ///     11-重复数据（串码/经销商/型号/颜色/时间）未处理；12-重复数据已处理（线下）；13-重复数据已删除；
        ///     21-相同串码不同信息，未处理；22-相同串码不同信息，已处理（线下）；23-相同串码不同信息，已删除；
        ///     -100：审批不通过
        /// </summary>
        public int check_status { get; set; }
    }
}