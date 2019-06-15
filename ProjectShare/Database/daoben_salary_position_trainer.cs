namespace ProjectShare.Database
{
    public class daoben_salary_position_trainer
    {
        /// <summary>
        /// 培训师KPI设置
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 岗位薪资表daoben_salary_position(主表)ID
        /// </summary>
        public string salary_position_id { get; set; }
        /// <summary>
        /// V雪球活跃人数建议金额（元）
        /// </summary>
        public decimal snowball_number_advice { get; set; }
        /// <summary>
        /// V雪球活跃人数标准金额（元/人）
        /// </summary>
        public decimal snowball_number_standard { get; set; }
        /// <summary>
        /// V雪球活跃人数考核标准（人）
        /// </summary>
        public int snowball_number_assess { get; set; }
        /// <summary>
        /// V雪球转化率建议金额（元）
        /// </summary>
        public decimal snowball_ratio_advice { get; set; }
        /// <summary>
        /// V雪球转化率标准金额（元）
        /// </summary>
        public decimal snowball_ratio_standard { get; set; }
        /// <summary>
        /// V雪球转化率考核标准（%）
        /// </summary>
        public int snowball_ratio_assess { get; set; }
        /// <summary>
        /// 导购服务人均产值建议金额（元）
        /// </summary>
        public decimal shopguide_average_advice { get; set; }
        /// <summary>
        /// 导购服务人均产值标准金额（元）
        /// </summary>
        public decimal shopguide_average_standard { get; set; }
        /// <summary>
        /// 导购服务人均产值考核标准（台）
        /// </summary>
        public decimal shopguide_average_assess { get; set; }
        /// <summary>
        /// 导购服务导购离职率建议金额（元）
        /// </summary>
        public decimal shopguide_resign_advice { get; set; }
        /// <summary>
        /// 导购服务导购离职率标准金额（元）
        /// </summary>
        public decimal shopguide_resign_standard { get; set; }
        /// <summary>
        /// 导购服务导购离职率考核标准（%）
        /// </summary>
        public int shopguide_resign_assess { get; set; }
        /// <summary>
        /// 产品培推高端机占比建议金额（元）
        /// </summary>
        public decimal product_expensive_advice { get; set; }
        /// <summary>
        /// 产品培推高端机占比标准金额（元）
        /// </summary>
        public decimal product_expensive_standard { get; set; }
        /// <summary>
        /// 产品培推高端机占比考核标准（%）
        /// </summary>
        public int product_expensive_assess { get; set; }
        /// <summary>
        /// 产品培推培训场次建议金额（元）
        /// </summary>
        public decimal product_train_advice { get; set; }
        /// <summary>
        /// 产品培推培训场次标准金额（元/场）
        /// </summary>
        public decimal product_train_standard { get; set; }
        /// <summary>
        /// 产品培推培训场次考核标准（场）
        /// </summary>
        public int product_train_assess { get; set; }
        /// <summary>
        /// 形象执行效率建议金额（元）
        /// </summary>
        public decimal image_efficiency_advice { get; set; }
        /// <summary>
        /// 形象执行效率标准金额（元）
        /// </summary>
        public decimal image_efficiency_standard { get; set; }
        /// <summary>
        /// 形象罚款建议金额（元）
        /// </summary>
        public decimal image_fine_advice { get; set; }
        /// <summary>
        /// 形象罚款标准金额（元/家）
        /// </summary>
        public decimal image_fine_standard { get; set; }
        /// <summary>
        /// 形象罚款考核标准（家）
        /// </summary>
        public int image_fine_number { get; set; }
        /// <summary>
        /// 终端经理打分建议金额（元）
        /// </summary>
        public decimal manager_scoring_advice { get; set; }
        /// <summary>
        /// 终端经理打分标准金额（元）
        /// </summary>
        public decimal manager_scoring_standard { get; set; }
    }
}