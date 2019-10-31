﻿using System;

namespace ProjectShare.Database
{
    public class daoben_activity_sales_perf_product
    {
        /// <summary>
        /// 业务提成考核机型列表
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 主表ID，表daoben_activity_sales_perf
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
        /// <summary>
        /// 参考返利金额（元/台，主表返利模式为“按型号”）
        /// </summary>
        public int rebate_advice { get; set; }
        /// <summary>
        /// 返利金额（元/台，主表返利模式为“按型号”）
        /// </summary>
        public decimal rebate { get; set; }

    }
}