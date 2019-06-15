namespace ProjectShare.Database
{
    public class daoben_salary_position_trainermanager
    {
        /// <summary>
        /// 培训经理KPI设置
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 岗位薪资表daoben_salary_position(主表)ID
        /// </summary>
        public string salary_position_id { get; set; }
        /// <summary>
        /// 人均产值建议金额（元）
        /// </summary>
        public decimal average_standard_money { get; set; }
        /// <summary>
        /// 人均产值标准台数（台）
        /// </summary>
        public int average_standard_number { get; set; }
        /// <summary>
        /// 导购离职率标准金额（元）
        /// </summary>
        public decimal resign_standard_money { get; set; }
        /// <summary>
        /// 导购离职率考核标准（%）
        /// </summary>
        public int resign_standard_ratio { get; set; }
        /// <summary>
        /// 高端机占比标准金额（元）
        /// </summary>
        public decimal product_expensive_money { get; set; }
        /// <summary>
        /// 高端机占比标准比率（%）
        /// </summary>
        public int product_expensive_ratio { get; set; }
    }
}