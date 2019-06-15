using System;

namespace ProjectShare.Database
{
    public class daoben_product_commission_detail
    {
        /// <summary>
        /// id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 型号
        /// </summary>
        public string model { get; set; }
        /// <summary>
        /// 颜色
        /// </summary>
        public string color { get; set; }
        /// <summary>
        /// 促销员/导购员提成
        /// </summary>
        public int guide_commission { get; set; }
        /// <summary>
        /// 业务提成
        /// </summary>
        public int sales_commission { get; set; }
        /// <summary>
        /// 包销返利
        /// </summary>
        public int exclusive_commission { get; set; }
        /// <summary>
        /// 是否核算底薪：0-否；1-是
        /// </summary>
        public bool salary_include { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string note { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? effect_date { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? expire_date { get; set; }
        /// <summary>
        /// 状态：-2-审核未结束；-1未生效；1有效；2-已失效
        /// </summary>
        public int effect_status { get; set; }
        /// <summary>
        /// 所属机构ID，表daoben_org_company
        /// </summary>
        public int company_id { get; set; }
        /// <summary>
        /// 所属事业部ID，表daoben_org_company
        /// </summary>
        public int company_id_parent { get; set; }
        /// <summary>
        /// 表daoben_product_price_approve
        /// </summary>
        public string main_id { get; set; }


    }
}