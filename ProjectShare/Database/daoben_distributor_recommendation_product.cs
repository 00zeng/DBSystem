using System;

namespace ProjectShare.Database
{
    public class daoben_distributor_recommendation_product
    {
        /// <summary>
        /// 主推返利活动机型表
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 主表ID，表daoben_distributor_recommendation
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
        /// 批发价
        /// </summary>
        public decimal price_wholesale { get; set; }
        /// <summary>
        /// 累计销量（台）
        /// </summary>
        public int total_count { get; set; }
        /// <summary>
        /// 正常机累计销量（台）
        /// </summary>
        public int total_normal_count { get; set; }
        /// <summary>
        /// 正常机累计销量总金额（元）
        /// </summary>
        public decimal total_normal_amount { get; set; }
        /// <summary>
        /// 参考返利金额（元/台，主表返利模式为“按型号”）
        /// </summary>
        public int rebate_advice { get; set; }
        /// <summary>
        /// 返利金额（元/台，主表返利模式为“按型号”）
        /// </summary>
        public decimal rebate { get; set; }


    }
}