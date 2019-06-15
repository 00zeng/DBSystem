using System;

namespace ProjectShare.Database
{
    public class daoben_distributor_attaining_time_sec
    {
        /// <summary>
        /// 时间分段表
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 主表ID
        /// </summary>
        public string main_id { get; set; }
        /// <summary>
        /// 机型分段Index
        /// </summary>
        public int product_sec_i { get; set; }
        /// <summary>
        /// 时间分段Index，同个返利不重复
        /// </summary>
        public int time_sec_i { get; set; }
        /// <summary>
        /// 分段开始日期；
        /// </summary>
        public DateTime? start_date { get; set; }
        /// <summary>
        /// 分段结束日期；
        /// </summary>
        public DateTime? end_date { get; set; }
        /// <summary>
        /// 奖励模式：3-按零售价；4-按型号；5-按批发价；6-无
        /// </summary>
        public int target_mode { get; set; }
        /// <summary>
        /// 金额计算方式：1-每台固定金额；2-每台批发价比例；3-每台零售价比例；4-固定总金额
        /// </summary>
        public int rebate_mode { get; set; }
    }
}