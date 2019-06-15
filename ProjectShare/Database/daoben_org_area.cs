using System;

namespace ProjectShare.Database
{
    public class daoben_org_area
    {
        /// <summary>
        /// 经理片区/业务片区
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 所属经理片区ID（仅业务片区）
        /// </summary>
        public int parent_id { get; set; }
        /// <summary>
        /// 所属经理片区（仅业务片区）
        /// </summary>
        public string parent_name { get; set; }
        /// <summary>
        /// 所属机构ID，表daoben_org_company
        /// </summary>
        public int company_id { get; set; }
        /// <summary>
        /// 所属机构，表daoben_org_company
        /// </summary>
        public string company_name { get; set; }
        /// <summary>
        /// 所属机构的上级机构ID，表daoben_org_company
        /// </summary>
        public int company_id_parent { get; set; }
        /// <summary>
        /// 所属机构完整名称链，表daoben_org_company
        /// </summary>
        public string company_linkname { get; set; }
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
        /// 1-经理片区；2-业务片区
        /// </summary>
        public int type { get; set; }
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