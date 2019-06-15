using System;

namespace ProjectShare.Database
{
    public class daoben_distributor_settlement_shipping
    {
        /// <summary>
        /// id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 经销商（导入时赋值）
        /// </summary>
        public string distributor_id { get; set; }
        /// <summary>
        /// 经销商
        /// </summary>
        public string distributor_name { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int total_count { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        public int total_amount { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime? shipping_date { get; set; }
        /// <summary>
        /// 所属分公司ID
        /// </summary>
        public int company_id { get; set; }
        /// <summary>
        /// 所属机构名称链
        /// </summary>
        public string company_linkname { get; set; }
        /// <summary>
        /// 所属事业部ID
        /// </summary>
        public int company_id_parent { get; set; }

    }
}