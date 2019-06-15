namespace ProjectShare.Database
{
    public class daoben_salary_position_seniority
    {
        /// <summary>
        /// 工龄工资
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 岗位薪资表daoben_salary_position(主表)ID
        /// </summary>
        public string salary_position_id { get; set; }
        /// <summary>
        /// 工龄终点
        /// </summary>
        public int year_max { get; set; }
        /// <summary>
        /// 工龄起点
        /// </summary>
        public int year_min { get; set; }
        /// <summary>
        /// 工龄工资（元/月）
        /// </summary>
        public decimal salary { get; set; }
    }
}