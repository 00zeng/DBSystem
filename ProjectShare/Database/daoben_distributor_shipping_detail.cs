using System;
namespace ProjectShare.Database
{
    public class daoben_distributor_shipping_detail
    {
        /// <summary>
        /// id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 主表ID
        /// </summary>
        public string main_id { get; set; }
        /// <summary>
        /// 单号
        /// </summary>
        public string shipping_bill { get; set; }
        /// <summary>
        /// 分公司ID，表daoben_org_company
        /// </summary>
        public int company_id { get; set; }
        /// <summary>
        /// 分公司名称
        /// </summary>
        public string company_name { get; set; }
        /// <summary>
        /// 经销商对应机构的linkname
        /// </summary>
        public string company_linkname { get; set; }
        /// <summary>
        /// 经销商（导入时赋值）
        /// </summary>
        public string distributor_id { get; set; }
        /// <summary>
        /// 经销商
        /// </summary>
        public string distributor_name { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int quantity { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public int amount { get; set; }
        /// <summary>
        /// 明细
        /// </summary>
        public string product_detail { get; set; }
        /// <summary>
        /// 制单时间
        /// </summary>
        public DateTime bill_date { get; set; }
        /// <summary>
        /// 是否收货
        /// </summary>
        public string is_received { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string note { get; set; }
        /// <summary>
        /// 经手人
        /// </summary>
        public string operator_name { get; set; }
        /// <summary>
        /// 是否打印
        /// </summary>
        public string is_printed { get; set; }
        /// <summary>
        /// 扩展备注
        /// </summary>
        public string extend_note { get; set; }
        /// <summary>
        /// 计划单备注
        /// </summary>
        public string schedule_note { get; set; }
        /// <summary>
        /// 计划单自定义单号
        /// </summary>
        public string schedule_bill { get; set; }
        /// <summary>
        /// 运费（导入时计算）
        /// </summary>
        public decimal shipping_fee { get; set; }





    }
}