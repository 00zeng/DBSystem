using System;

namespace ProjectShare.Database
{
    public class daoben_product_price_effect
    {
        /// <summary>
        /// 当前生效的价格信息表
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
        /// 二级价(总部价)
        /// </summary>
        public decimal price_l2 { get; set; }
        /// <summary>
        /// 事业部价格
        /// </summary>
        public decimal price_l3 { get; set; }
        /// <summary>
        /// 代理价(分公司价)
        /// </summary>
        public decimal price_l4 { get; set; }
        /// <summary>
        /// 广告费，小于0表示用百分比
        /// </summary>
        public decimal ad_fee { get; set; }
        /// <summary>
        /// 广告费，用于显示，与导入表格中一样
        /// </summary>
        public string ad_fee_show { get; set; }
        /// <summary>
        /// 内购价
        /// </summary>
        public decimal price_internal { get; set; }
        /// <summary>
        /// 最低买断价
        /// </summary>
        public decimal price_buyout { get; set; }
        /// <summary>
        /// 批发价
        /// </summary>
        public decimal price_wholesale { get; set; }
        /// <summary>
        /// 零售价
        /// </summary>
        public decimal price_retail { get; set; }
        /// <summary>
        /// 属性：正常机型、高端机型、清仓机型、特价机型
        /// </summary>
        public string product_type { get; set; }
        /// <summary>
        /// 0-非特价机；1-特价机
        /// </summary>
        public bool special_offer { get; set; }
        /// <summary>
        /// 0-非高端机；1-高端机
        /// </summary>
        public bool high_level { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? effect_date { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? expire_date { get; set; }
        /// <summary>
        /// 所属机构ID，表daoben_org_company
        /// </summary>
        public int company_id { get; set; }
        /// <summary>
        /// 所属机构名称链
        /// </summary>
        public string company_linkname { get; set; }
        /// <summary>
        /// 所属机构的上级机构ID，表daoben_org_company
        /// </summary>
        public int company_id_parent { get; set; }
        /// <summary>
        /// 1-当前生效的；0-不生效（暂时被新的覆盖）
        /// </summary>
        public bool is_effect { get; set; }
    }
}