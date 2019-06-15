using System;

namespace ProjectShare.Database
{
    public class daoben_distributor_attaining_rebate_product
    {
        /// <summary>
        /// 达量返利活动机型表
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 主表ID，表daoben_distributor_attaining
        /// </summary>
        public string main_id { get; set; }
        /// <summary>
        /// 表daoben_distributor_attaining_rebate Index
        /// </summary>
        public int rebate_i { get; set; }
        /// <summary>
        /// 型号
        /// </summary>
        public string model { get; set; }
        /// <summary>
        /// 颜色
        /// </summary>
        public string color { get; set; }
        /// <summary>
        /// 批发价/零售价起始数目
        /// </summary>
        public decimal target_min { get; set; }
        /// <summary>
        /// 批发价/零售价临界数目，-1表示”以上“
        /// </summary>
        public decimal target_max { get; set; }
        /// <summary>
        /// 返利金额/基数比例（*100，如10表示10%，主表返利模式为“按完成率”）
        /// </summary>
        public decimal rebate { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? start_date { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? end_date { get; set; }




    }
}