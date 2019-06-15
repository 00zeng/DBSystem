using System;

namespace ProjectShare.Database
{
    public class daoben_distributor_recommendation_rebate
    {
        /// <summary>
        /// 主推返利规则表
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 主表ID，表daoben_distributor_recommendation
        /// </summary>
        public string main_id { get; set; }
        /// <summary>
        /// 完成率（*100，如10表示10%）/销量起始数目
        /// </summary>
        public int target_min { get; set; }
        /// <summary>
        /// 完成率（*100，如10表示10%）/销量价临界数目，-1表示”以上“
        /// </summary>
        public int target_max { get; set; }
        /// <summary>
        /// 返利金额
        /// </summary>
        public decimal rebate { get; set; }


    }
}