using System;

namespace ProjectShare.Database
{
    public class daoben_product_sn
    {
        /// <summary>
        /// 串码表（出库->实销/买断）
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 串码
        /// </summary>
        public string phone_sn { get; set; }
        /// <summary>
        /// 型号
        /// </summary>
        public string model { get; set; }
        /// <summary>
        /// 颜色
        /// </summary>
        public string color { get; set; }
        /// <summary>
        /// 出库经销商ID
        /// </summary>
        public string out_distributor_id { get; set; }
        /// <summary>
        /// 出库经销商
        /// </summary>
        public string out_distributor { get; set; }
        /// <summary>
        /// 实销经销商ID
        /// </summary>
        public string sale_distributor_id { get; set; }
        /// <summary>
        /// 实销经销商
        /// </summary>
        public string sale_distributor { get; set; }
        /// <summary>
        /// 实销人ID，（可为导购员、店员或经销商，根据reporter_type分别对应不同的表）
        /// </summary>
        public string reporter_id { get; set; }
        /// <summary>
        /// 实销人，（可为导购员、店员或经销商，根据reporter_type分别对应不同的表）
        /// </summary>
        public string reporter { get; set; }
        /// <summary>
        /// 实销人类型：1-导购员；2-店员；3-经销商
        /// </summary>
        public int reporter_type { get; set; }
        /// <summary>
        /// 出库业务员ID
        /// </summary>
        public string out_sales_id { get; set; }
        /// <summary>
        /// 出库业务员
        /// </summary>
        public string out_sales { get; set; }
        /// <summary>
        /// 出库业务经理ID
        /// </summary>
        public string out_sales_m_id { get; set; }
        /// <summary>
        /// 出库业务经理
        /// </summary>
        public string out_sales_m { get; set; }
        /// <summary>
        /// 实销业务员ID
        /// </summary>
        public string sales_id { get; set; }
        /// <summary>
        /// 实销业务员
        /// </summary>
        public string sales { get; set; }
        /// <summary>
        /// 实销业务经理ID
        /// </summary>
        public string sales_m_id { get; set; }
        /// <summary>
        /// 实销业务经理
        /// </summary>
        public string sales_m { get; set; }
        /// <summary>
        /// 1-已出库；2-已实销；11-已买断(实销未导入)；12-已买断(实销已导入)；-101已退库
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 0-出库；1-正常销售；2-买断；3-包销； -1 退库
        /// </summary>
        public int sale_type { get; set; }
        /// <summary>
        /// 出库时间
        /// </summary>
        public DateTime? outstorage_time { get; set; }
        /// <summary>
        /// 实销时间
        /// </summary>
        public DateTime? sale_time { get; set; }
        /// <summary>
        /// 买断时间/包销时间
        /// </summary>
        public DateTime? buyout_time { get; set; }
        /// <summary>
        /// 最终批发价（有调价补差时该值为新批发价）
        /// </summary>
        public decimal price_wholesale { get; set; }
        /// <summary>
        /// 零售价
        /// </summary>
        public decimal price_retail { get; set; }
        /// <summary>
        /// 销售价（sale_type=1时为零售价，sale_type=2时为买断价，sale_type=3时为包销价）
        /// </summary>
        public decimal price_sale { get; set; }
        /// <summary>
        /// 调价补差总额（非买断补差）
        /// </summary>
        public decimal refund_amount { get; set; }
        /// <summary>
        /// 调价补差次数
        /// </summary>
        public int refund_count { get; set; }
        /// <summary>
        /// 0-非特价机；1-特价机
        /// </summary>
        public bool special_offer { get; set; }
        /// <summary>
        /// 0-非高端机；1-高端机
        /// </summary>
        public bool high_level { get; set; }
        /// <summary>
        /// 所属机构的上级机构ID，表daoben_org_company
        /// </summary>
        public int company_id_parent { get; set; }
        /// <summary>
        /// 所属机构ID，表daoben_org_company
        /// </summary>
        public int company_id { get; set; }
        /// <summary>
        /// 所属机构名称
        /// </summary>
        public string company_linkname { get; set; }
        /// <summary>
        /// 非数据库列，用于存放临时数据，
        /// db.IsIgnoreErrorColumns = true 必须设置【默认值】
        /// </summary>
        public int tmpInt { get; set; }
    }
}