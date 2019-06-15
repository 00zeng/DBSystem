using System;

namespace ProjectShare.Database
{
    public class daoben_distributor_attaining_product_sec
    {
        /// <summary>
        /// 机型分段表
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 主表ID
        /// </summary>
        public string main_id { get; set; }
        /// <summary>
        /// 机型分段Index，同个返利不重复
        /// </summary>
        public int product_sec_i { get; set; }
        /// <summary>
        /// 活动机型：1-全部机型；2-指定型号（副表）
        /// </summary>
        public int product_scope { get; set; }
    }
}