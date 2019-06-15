using System;

namespace ProjectShare.Database
{
    public class daoben_activity_attaining_product_reward
    {
        /// <summary>
        /// 达量奖励活动机型表
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 主表ID，表daoben_activity_attaining
        /// </summary>
        public string reward_id { get; set; }
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
        ///  奖罚金额，小于0表示罚款
        /// </summary>
        public decimal reward { get; set; }
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