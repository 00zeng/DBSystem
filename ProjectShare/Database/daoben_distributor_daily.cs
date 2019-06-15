using System;

namespace ProjectShare.Database
{
    // TODO DELETE --JIANG
    public class daoben_distributor_daily
    {
        /// <summary>
        /// id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 1-实销；2-出库；3-门店买断；4-仓库买断；5-补差
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 统计日期
        /// </summary>
        public DateTime calculate_date { get; set; }
        /// <summary>
        /// 经销商id
        /// </summary>
        public string distributor_id { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int quatity { get; set; }
        /// <summary>
        /// 批发价总额
        /// </summary>
        public decimal wp_amount { get; set; }
        /// <summary>
        /// 零售价总额
        /// </summary>
        public decimal rp_amount { get; set; }
        /// <summary>
        /// 补差总额
        /// </summary>
        public decimal refund { get; set; }
        /// <summary>
        /// 所属分公司ID，表daoben_org_company
        /// </summary>
        public int company_id { get; set; }
        /// <summary>
        /// 所属机构名称链
        /// </summary>
        public string company_linkname { get; set; }
        /// <summary>
        /// 所属机构的上级机构ID，表daoben_org_company
        /// </summary>
        public int company_id_parent { get; set; }

    }
}