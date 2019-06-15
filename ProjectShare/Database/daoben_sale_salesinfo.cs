using System;

namespace ProjectShare.Database
{
    public class daoben_sale_salesinfo
    {
        /// <summary>
        /// id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 单据名
        /// </summary>
        public string bill_name { get; set; }
        /// <summary>
        /// 型号
        /// </summary>
        public string model { get; set; }
        /// <summary>
        /// 颜色
        /// </summary>
        public string color { get; set; }
        /// <summary>
        /// 串码
        /// </summary>
        public string phone_sn { get; set; }
        /// <summary>
        /// 经销商
        /// </summary>
        public string distributor_id { get; set; }
        /// <summary>
        /// 经销商（导入时经销商名称一般含编码，故在创建经销商时需以此为经销商名称）
        /// </summary>
        public string distributor_name { get; set; }
        /// <summary>
        /// 上报人（可为导购员、店员或经销商编码，上报人为经销商时，导购员无提成）
        /// </summary>
        public string reporter_name { get; set; }
        /// <summary>
        /// 上报人类型，1-导购员；2-店员；3-经销商
        /// </summary>
        public int reporter_type { get; set; }
        /// <summary>
        /// 上报人ID，根据reporter_type分别对应不同的表
        /// </summary>
        public string reporter_id { get; set; }
        /// <summary>
        /// 业务员（业务片区）
        /// </summary>
        public string sales_id { get; set; }
        /// <summary>
        /// 业务员（业务片区）
        /// </summary>
        public string sales_name { get; set; }
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
        /// 业务片区
        /// </summary>
        public string area_l2_name { get; set; }
        /// <summary>
        /// 所属事业部ID，表daoben_org_company
        /// </summary>
        public int company_id_parent { get; set; }
        /// <summary>
        /// 所属事业部名称（二级地区）
        /// </summary>
        public string company_name_parent { get; set; }
        /// <summary>
        /// 所属分公司ID，表daoben_org_company
        /// </summary>
        public int company_id { get; set; }
        /// <summary>
        /// 所属机构名称（事业部-分公司）
        /// </summary>
        public string company_linkname { get; set; }
        /// <summary>
        /// 分公司名称（业务大区）
        /// </summary>
        public string company_name { get; set; }
        /// <summary>
        /// 状态（终端售出等）
        /// </summary>
        public string sale_status { get; set; }
        /// <summary>
        /// 类型（未设品类等）
        /// </summary>
        public string category { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime? accur_time { get; set; }
        /// <summary>
        /// 表daoben_sales_outstorage_approve
        /// </summary>
        public string import_file_id { get; set; }
        /// <summary>
        /// 数据检查状态：0-未检查；1-检查通过；
        ///     21-相同串码不同信息，未处理；23-相同串码不同信息，已删除；
        ///     -100：审批不通过
        /// </summary>
        public int check_status { get; set; }

    }
}