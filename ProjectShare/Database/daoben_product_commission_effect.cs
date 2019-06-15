using System;

namespace ProjectShare.Database
{
    public class daoben_product_commission_effect
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