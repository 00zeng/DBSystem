using System;

namespace ProjectShare.Database
{
    public class daoben_kpi_trainer_detail
    {
        /// <summary>
        /// 详情表
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 表daoben_product_price_approve
        /// </summary>
        public string main_id { get; set; }
        /// <summary>
        /// 培训师: 1-V雪球活跃人数；2-V雪球转化率；3-导购人均产值；4-导购离职率；5-高端机占比；6-培训场次；7-形象执行效率；8-形象罚款；9-终端经理打分
        /// 培训经理：51-导购人均产值；52-导购离职率；53-高端机占比
        /// </summary>
        public int kpi_type { get; set; }
        /// <summary>
        /// 实际活跃人数、实际转化率、实销高端机占比、实际场次、品牌部打分、不合格数量
        /// </summary>
        public decimal kpi_score { get; set; }
        /// <summary>
        /// 月底人数、态度
        /// </summary>
        public int emp_count { get; set; }
        /// <summary>
        /// 离职人数、效率
        /// </summary>
        public int resign_emp_count { get; set; }
        /// <summary>
        /// 导购员销量 kpi_type=3
        /// </summary>
        public decimal guide_amount { get; set; }
        /// <summary>
        /// 导购员数量 kpi_type=3
        /// </summary>
        public int guide_count { get; set; }
        /// <summary>
        /// kpi结果金额
        /// </summary>
        public decimal kpi_result { get; set; }
    }
}