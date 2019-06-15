using System;

namespace ProjectShare.Database
{
    public class daoben_kpi_sales_sn_temp
    {
        /// <summary>
        /// 业务月度考核串码提成表（计算工资填入）
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 主表ID，表daoben_payroll_sales
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
        /// 型号
        /// </summary>
        public string model { get; set; }
        /// <summary>
        /// 颜色
        /// </summary>
        public string color { get; set; }
        /// <summary>
        /// 0-出库；1-正常销售；2-买断；3-包销； -1 退库
        /// </summary>
        public int sale_type { get; set; }
        /// <summary>
        /// 最终批发价（有调价补差时该值为新批发价）
        /// </summary>
        public decimal price_wholesale { get; set; }
        /// <summary>
        /// 销售价（sale_type=1时为零售价，sale_type=2时为买断价，sale_type=3时为包销价）
        /// </summary>
        public decimal price_sale { get; set; }
        /// <summary>
        /// 实销/出库时间
        /// </summary>
        public DateTime? time { get; set; }
    }
}