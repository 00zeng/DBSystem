using System;

namespace ProjectShare.Database
{
    public class daoben_sale_buyout_request_sub
    {
        /// <summary>
        /// 仓库买断详情表，发起申请时使用
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
        /// 买断价
        /// </summary>
        public int price_buyout { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int buyout_count { get; set; }
        /// <summary>
        /// 0-未导入；1-已导入
        /// </summary>
        public bool imported { get; set; }
        /// <summary>
        /// 表daoben_sales_buyout_import_approve
        /// </summary>
        public string import_file_id { get; set; }


    }
}