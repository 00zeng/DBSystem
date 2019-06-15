namespace ProjectShare.Database
{
    public class daoben_activity_pk_product
    {
        /// <summary>
        /// PK比赛机型表
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 主表ID，表daoben_activity_pk
        /// </summary>
        public string main_id { get; set; }
        /// <summary>
        /// 型号
        /// </summary>
        public string model { get; set; }
        /// <summary>
        /// 颜色
        /// </summary>
        public string color { get; set; }
        /// <summary>
        /// 批发价
        /// </summary>
        public decimal price_wholesale { get; set; }
        /// <summary>
        /// 累计销量（台）
        /// </summary>
        public int total_count { get; set; }
        /// <summary>
        /// 累计销量总金额（元）
        /// </summary>
        public int total_amount { get; set; }

    }
}