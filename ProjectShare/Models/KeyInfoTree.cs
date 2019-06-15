using System;
using System.Collections.Generic;

namespace ProjectShare.Models
{
    /// <summary>
    /// 机构树形信息
    /// </summary>
    public class CompanyTree
    {
        public int company_id { get; set; }
        public string company_name { get; set; }
        /// <summary>
        /// Link Name
        /// </summary>
        public string company_linkname { get; set; }

        public List<AreaL2Tree> area_l2_list { get; set; }
    }


    /// <summary>
    /// 小区域/部门树形信息
    /// </summary>
    public class AreaL2Tree
    {
        public int area_l2_id { get; set; }

        public string area_l2_name { get; set; }

        public List<EmpKeyInfo> key_list { get; set; }
    }

    /// <summary>
    /// 员工/经销商层级信息
    /// </summary>
    public class EmpKeyInfo
    {
        public string id { get; set; }
        /// <summary>
        /// 经销编码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 经销商名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 经销商V2系统报量名称
        /// </summary>
        public string name_v2 { get; set; }
        public int company_id { get; set; }

        public string company_name { get; set; }
        public string company_linkname { get; set; }
        /// <summary>
        /// 经理片区（大区域）
        /// </summary>
        public int area_l1_id { get; set; }
        /// <summary>
        /// 经理片区（大区域）
        /// </summary>
        public string area_l1_name { get; set; }
        /// <summary>
        /// 业务片区（小区域）
        /// </summary>
        public int area_l2_id { get; set; }
        /// <summary>
        /// 业务片区（小区域）
        /// </summary>
        public string area_l2_name { get; set; }
        /// <summary>
        /// 雇员类别:实习生、劳务工、员工、职员
        /// </summary>
        public string emp_category { get; set; }
        /// <summary>
        /// 等级（含职等、星级）
        /// </summary>
        public string grade { get; set; }
        /// <summary>
        /// 可变的显示信息
        /// </summary>
        public string display_info { get; set; }
    }

    /// <summary>
    /// 机型树形信息
    /// </summary>
    public class ProductTypeTree
    {
        public string product_type { get; set; }

        public List<ProductKeyInfo> product_list { get; set; }
    }

    public class ProductModelTree
    {
        public string name { get; set; } // model

        public List<ProductKeyInfo> product_list { get; set; }
    }

    public class ProductKeyInfo
    {
        public string product_type { get; set; }
        public int id { get; set; }
        public string name { get; set; } // model
        public decimal price_buyout { get; set; }
        public string color { get; set; }
    }

    public class CompanyAreaTree
    {
        public int company_id { get; set; }

        public string company_name { get; set; }

        public string company_linkname { get; set; }

        public List<AreaL1Info> area_l1_list { get; set; }

    }

    public class AreaL1Info
    {
        public int area_l1_id { get; set; }

        public string area_l1_name { get; set; }
        public List<Area2Info> area_l2_list { get; set; }

    }

    public class Area2Info
    {
        public int area_l2_id { get; set; }

        public string area_l2_name { get; set; }

        public int area_l1_id { get; set; }
        public string area_l1_name { get; set; }
        public int company_id { get; set; }

        public string company_name { get; set; }
        public string company_linkname { get; set; }
        public int company_id_parent { get; set; }


        public string display_info { get; set; }
    }

    public class organizationTree
    {
        public string name { get; set; }
        /// <summary>
        /// value: 1-董事会 2-事业部 3-分公司
        /// value：11-部门 12-经理片区 13-业务片区 
        /// value：21-门店
        /// </summary>      
        public string value { get; set; }
        public List<organizationTree> children { get; set; }
    }

    public class totalInfo
    {
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        /// <summary>
        /// 总次数
        /// </summary>
        public int total_count { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        public int total_amount { get; set; }
        /// <summary>
        /// 总提成
        /// </summary>
        public int total_commission { get; set; }        
        /// <summary>
        /// 返利次数
        /// </summary>
        public int total_activity { get; set; }
        /// <summary>
        /// 累计奖金
        /// </summary>
        public decimal total_reward { get; set; }
        /// <summary>
        /// 累计罚款
        /// </summary>
        public decimal total_penalty { get; set; }

        /// <summary>
        /// 总下货量
        /// </summary>
        public int outstorage_count { get; set; }
        /// <summary>
        /// 总下货量金额
        /// </summary>
        public decimal outstorage_amount { get; set; }
        /// <summary>
        /// 总实销量
        /// </summary>
        public int sale_count { get; set; }
        /// <summary>
        /// 总实销量金额
        /// </summary>
        public decimal sale_amount { get; set; }
        /// <summary>
        /// 总实销量提成
        /// </summary>
        public decimal sale_commission { get; set; }
        /// <summary>
        /// 正常机总销量
        /// </summary>
        public int normal_count { get; set; }
        /// <summary>
        /// 正常机总金额
        /// </summary>
        public decimal normal_amount { get; set; }
        /// <summary>
        /// 正常机总提成
        /// </summary>
        public decimal normal_commission { get; set; }
        /// <summary>
        /// 买断总销量
        /// </summary>
        public int buyout_count { get; set; }
        /// <summary>
        /// 买断总金额
        /// </summary>
        public decimal buyout_amount { get; set; }
        /// <summary>
        /// 买断总提成
        /// </summary>
        public decimal buyout_commission { get; set; }
        /// <summary>
        /// 包销机总销量
        /// </summary>
        public int ex_count { get; set; }
        /// <summary>
        /// 包销机总金额
        /// </summary>
        public decimal ex_amount { get; set; }
        /// <summary>
        /// 包销机总提成
        /// </summary>
        public decimal ex_commission { get; set; }
        /// <summary>
        /// 特价机总销量
        /// </summary>
        public int special_count { get; set; }
        /// <summary>
        /// 特价机总金额
        /// </summary>
        public decimal special_amount { get; set; }
        /// <summary>
        /// 特价机总提成
        /// </summary>
        public decimal special_commission { get; set; }
        public int sale_type { get; set; }
        /// <summary>
        /// 经销商id
        /// </summary>
        public string distributor_id { get; set; }
        /// <summary>
        /// 经销商名称
        /// </summary>
        public string distributor_name { get; set; }
        /// <summary>
        /// 机构名称
        /// </summary>
        public string company_linkname { get; set; }
        /// <summary>
        /// 业务员id/业务经理id
        /// </summary>
        public string emp_id { get; set; }
        /// <summary>
        /// 业务员/业务经理
        /// </summary>
        public string emp_name { get; set; }

    }



}
