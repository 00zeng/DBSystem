using System;

namespace ProjectShare.Database
{
    public class daoben_distributor_image_res
    {
        /// <summary>
        /// 结果信息表
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 主表ID
        /// </summary>
        public string main_id { get; set; }
        /// <summary>
        /// 月份，按月返利时使用
        /// </summary>
        public DateTime? month { get; set; }
        /// <summary>
        /// 累计完成销量（台）
        /// </summary>
        public int total_sale_count { get; set; }
        /// <summary>
        /// 累计完成销量总金额（元）
        /// </summary>
        public decimal total_sale_amount { get; set; }
        /// <summary>
        /// 返利金额
        /// </summary>
        public decimal rebate { get; set; }
        /// <summary>
        /// 统计开始时间
        /// </summary>
        public DateTime? start_date { get; set; }
        /// <summary>
        /// 统计结束时间
        /// </summary>
        public DateTime? end_date { get; set; }
    }
}