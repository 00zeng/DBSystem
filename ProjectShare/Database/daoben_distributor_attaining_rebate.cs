using System;

namespace ProjectShare.Database
{
    public class daoben_distributor_attaining_rebate
    {
        /// <summary>
        /// 达量返利规则表
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 主表ID，表daoben_distributor_attaining
        /// </summary>
        public string main_id { get; set; }
        /// <summary>
        /// 时间分段Index，表daoben_distributor_attaining_time_sec
        /// </summary>
        public int time_sec_i { get; set; }
        /// <summary>
        /// rebate index
        /// </summary>
        public int rebate_i { get; set; }
        /// <summary>
        /// 完成率（*100，如10表示10%）/销量起始数目
        /// </summary>
        public int target_min { get; set; }
        /// <summary>
        /// 完成率（*100，如10表示10%）/销量临界数目，-1表示”以上“
        /// </summary>
        public int target_max { get; set; }
        /// <summary>
        /// 返利金额/基数比例（*100，如10表示10%，主表返利模式为“按完成率”）
        /// </summary>
        public decimal rebate { get; set; }
    }
}