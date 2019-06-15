using System;

namespace ProjectShare.Database
{
    public class daoben_product_sn_outlay
    {
        /// <summary>
        /// 串码提成/返利表
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 1-业务员KPI提成；2-业务员销量考核提成；11-业务经理KPI提成；22-业务经理销量考核提成；
        /// 31-导购提成（main_id=null）;
        /// 51-经销商返利；
        /// </summary>
        public int category { get; set; }
        /// <summary>
        /// 主表ID，不同category对应不同主表
        /// </summary>
        public string main_id { get; set; }
        /// <summary>
        /// 串码
        /// </summary>
        public string phone_sn { get; set; }
        /// <summary>
        /// 提成金额（元）
        /// </summary>
        public decimal outlay { get; set; }
        /// <summary>
        /// 支出类型：1-实销，2-出库
        /// </summary>
        public int outlay_type { get; set; }
        /// <summary>
        /// 实销/出库时间
        /// </summary>
        public DateTime? time { get; set; }
        // 以下为非数据库列，用于存放临时数据，
        /// <summary>
        /// 型号
        /// </summary>
        public string model { get; set; }
        /// <summary>
        /// 颜色
        /// </summary>
        public string color { get; set; }
    }
}