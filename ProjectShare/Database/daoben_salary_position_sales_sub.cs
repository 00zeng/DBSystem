namespace ProjectShare.Database
{
    public class daoben_salary_position_sales_sub
    {
        /// <summary>
        /// 岗位薪资-业务薪资副表
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 主表ID (daoben_salary_position_sales) 
        /// </summary>
        public string main_id { get; set; }
        /// <summary>
        /// 正常机提成(百分比整数部分或“元/台”或“元/月”)
        /// </summary>
        public decimal sale_rebate { get; set; }
        /// <summary>
        /// 买断提成(百分比整数部分或“元/台”或“元/月”)
        /// </summary>
        public decimal buyout_rebate { get; set; }
        /// <summary>
        /// 完成率（*100，如10表示10%）/批发价起始数目
        /// </summary>
        public int target_min { get; set; }
        /// <summary>
        /// 完成率（*100，如10表示10%）/批发价临界数目，-1表示”以上“
        /// </summary>
        public int target_max { get; set; }

    }
}