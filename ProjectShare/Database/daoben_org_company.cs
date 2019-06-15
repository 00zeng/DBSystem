using System;

namespace ProjectShare.Database
{
    /// <summary>
    /// 组织架构-机构管理
    /// </summary>
    public class daoben_org_company
    {
        /// <summary>
        /// 组织架构-机构管理
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 机构名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// “董事会”、“事业部”、“分公司”
        /// </summary>
        public string category { get; set; }
        /// <summary>
        /// 分公司经营属性：1-直营；2-加盟
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 上级机构ID，表daoben_org_company
        /// </summary>
        public int parent_id { get; set; }
        /// <summary>
        /// 上级机构名称，表daoben_org_company
        /// </summary>
        public string parent_name { get; set; }
        /// <summary>
        /// 机构层级的完整名称链（不含”董事会“），用”-“分隔，如”xx事业部-yy分公司“
        /// </summary>
        public string link_name { get; set; }
        /// <summary>
        /// 所在省市，包含省
        /// </summary>
        public string city { get; set; }
        /// <summary>
        /// 所在省市代码
        /// </summary>
        public string city_code { get; set; }
        /// <summary>
        /// 地址，不含省市信息
        /// </summary>
        public string address { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string contact_phone { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string note { get; set; }
        /// <summary>
        /// 状态：1-正常；-1-已删除
        /// </summary>
        public int status { get; set; }
    }
}
