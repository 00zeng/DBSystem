namespace ProjectShare.Database
{
    public class daoben_salary_position_guide_sub
    {
        /// <summary>
        /// id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 主表ID（表daoben_salary_position_guide）
        /// </summary>
        public string main_id { get; set; }
        /// <summary>
        /// 类型：1-星级制底薪；2-浮动底薪；3-按销量年终奖；4-按星级年终奖
        /// </summary>
        public int category { get; set; }
        /// <summary>
        /// 底薪/奖金/星级工资（元/月，元/台）
        /// </summary>
        public decimal amount { get; set; }
        /// <summary>
        /// 星级：一星；二星...
        /// </summary>
        public string level { get; set; }
        /// <summary>
        /// 月销量指标（台）起始数目
        /// </summary>
        public int target_min { get; set; }
        /// <summary>
        /// 月销量指标（台）临界数目，-1表示”以上“
        /// </summary>
        public int target_max { get; set; }

    }
}