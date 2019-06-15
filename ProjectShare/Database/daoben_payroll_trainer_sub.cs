using System;

namespace ProjectShare.Database
{
    public class daoben_payroll_trainer_sub
    {
        /// <summary>
        /// 培训工资副表
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 主表ID，表daoben_payroll_trainer
        /// </summary>
        public string main_id { get; set; }
        /// <summary>
        /// 11-奖罚；12-福利（以工资形式发放的）；21-公司借款-购机；22-公司借款-贷款；23-公司借款-其他；31-其他款
        /// </summary>
        public int category { get; set; }
        /// <summary>
        /// 不同类型对应不同的详情主表ID
        /// </summary>
        public string category_id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string sub_name { get; set; }
        /// <summary>
        /// 备注等信息
        /// </summary>
        public string sub_note { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal amount { get; set; }
    }
}