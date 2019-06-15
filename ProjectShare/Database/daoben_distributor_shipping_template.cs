using System;

namespace ProjectShare.Database
{
    public class daoben_distributor_shipping_template
    {
        /// <summary>
        /// 经销商运费规则表
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 主表ID，表daoben_distributor_shipping_template_approve
        /// </summary>
        public string main_id { get; set; }
        /// <summary>
        /// 台数起点
        /// </summary>
        public int count_min { get; set; }
        /// <summary>
        /// 台数终点；-1-表示以上
        /// </summary>
        public int count_max { get; set; }
        /// <summary>
        /// 运费(元/台)
        /// </summary>
        public int shipping_fee { get; set; }

    }
}