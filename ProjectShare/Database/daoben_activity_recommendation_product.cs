namespace ProjectShare.Database
{
    public class daoben_activity_recommendation_product
    {
        /// <summary>
        /// 达量奖活动机型表
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 主表ID，表daoben_activity_attaining
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
        /// 总销量（台）
        /// </summary>
        public int total_count { get; set; }
        /// <summary>
        /// 正常机销量（台）
        /// </summary>
        public int total_normal_count { get; set; }
        /// <summary>
        /// 累计正常机销量总金额（元）
        /// </summary>
        public decimal total_normal_amount { get; set; }
        /// <summary>
        /// 奖罚金额，小于0表示罚款
        /// </summary>
        public decimal reward { get; set; }
    }
}