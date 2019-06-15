using System;

namespace ProjectShare.Database
{
    public class daoben_distributor_recommendation_distributor
    {
        /// <summary>
        /// 主推返利经销商表
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 主表ID，表daoben_distributor_recommendation
        /// </summary>
        public string main_id { get; set; }
        /// <summary>
        /// 经销商ID
        /// </summary>
        public string distributor_id { get; set; }
        /// <summary>
        /// 经销商姓名
        /// </summary>
        public string distributor_name { get; set; }
        /// <summary>
        /// 经理片区，表daoben_org_area
        /// </summary>
        public int area_l1_id { get; set; }
        /// <summary>
        /// 经理片区名称
        /// </summary>
        public string area_l1_name { get; set; }
        /// <summary>
        /// 业务片区，表daoben_org_area
        /// </summary>
        public int area_l2_id { get; set; }
        /// <summary>
        /// 业务片区名称
        /// </summary>
        public string area_l2_name { get; set; }
        /// <summary>
        /// 累计销量（台）
        /// </summary>
        public int total_count { get; set; }
        /// <summary>
        /// 算钱机型销量(台)
        /// </summary>
        public int total_normal_count { get; set; }
        /// <summary>
        /// 累计销量总金额（元）
        /// </summary>
        public decimal total_amount { get; set; }
        /// <summary>
        /// 累计完成率（*100，如40表示40%）
        /// </summary>
        public decimal total_ratio { get; set; }
        /// <summary>
        /// 返利金额
        /// </summary>
        public decimal total_rebate { get; set; }


    }
}