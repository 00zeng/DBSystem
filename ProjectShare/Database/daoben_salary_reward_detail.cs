using System;

namespace ProjectShare.Database
{
    public class daoben_salary_reward_detail
    {
        /// <summary>
        /// 员工奖罚明细表
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 主表ID，daoben_salary_reward
        /// </summary>
        public string main_id { get; set; }
        /// <summary>
        /// 奖罚名称
        /// </summary>
        public string detail_name { get; set; }
        
        /// <summary>
        /// 金额（元）
        /// </summary>
        public decimal amount { get; set; }
        /// <summary>
        /// 评分说明
        /// </summary>
        public string note { get; set; }


    }
}