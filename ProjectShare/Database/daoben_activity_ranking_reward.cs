using System;

namespace ProjectShare.Database
{
    public class daoben_activity_ranking_reward
    {
        /// <summary>
        /// id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 主表ID，表daoben_activity_ranking
        /// </summary>
        public string main_id { get; set; }
        /// <summary>
        /// 排名范围起点；或排名位置
        /// </summary>
        public uint place_min { get; set; }
        /// <summary>
        /// 排名范围终点
        /// </summary>
        public uint place_max { get; set; }
        /// <summary>
        /// 奖罚方式-奖励：0-无奖励；>0-奖励金额
        /// </summary>
        public decimal reward { get; set; }
        /// <summary>
        /// 获奖人数
        /// </summary>
        public uint emp_count { get; set; }
    }
}