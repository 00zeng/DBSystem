namespace ProjectShare.Database
{
    public class daoben_salary_position_traffic
    {
        /// <summary>
        /// 岗位薪资-交通补贴（导购/业务/部门等类型使用）
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 岗位薪资表daoben_salary_position(主表)ID
        /// </summary>
        public string salary_position_id { get; set; }
        /// <summary>
        /// 交通补贴
        /// </summary>
        public decimal traffic_subsidy { get; set; }
        /// <summary>
        /// 将所有个人设置重置为此方案：0-不重置；1-重置；（仅交通补贴）
        /// </summary>
        public bool reset_all { get; set; }
    }
}