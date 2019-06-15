using System;

namespace ProjectShare.Database
{
    public class daoben_org_dept_area
    {
        /// <summary>
        /// 部门及区域
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 部门或区域名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 部门/区域经理的员工信息ID，表daoben_emp_info
        /// </summary>
        public int manager_emp_id { get; set; }
        /// <summary>
        /// 部门/区域经理名称
        /// </summary>
        public string manager { get; set; }
        /// <summary>
        /// 上级部门/区域ID，表daoben_org_dept_area
        /// </summary>
        public int parent_id { get; set; }
        /// <summary>
        /// 上级机构部门名称
        /// </summary>
        public string parent_name { get; set; }
        /// <summary>
        /// 所属机构的上级机构ID，表daoben_org_company
        /// </summary>
        public int company_id_parent { get; set; }
        /// <summary>
        /// 所属机构ID，表daoben_org_company
        /// </summary>
        public int company_id { get; set; }
        /// <summary>
        /// 所属机构完整名称链，表daoben_org_company
        /// </summary>
        public string company_linkname { get; set; }
        /// <summary>
        /// 等级工资(除导购员)类型。事业部：1-行政管理、4-运营商中心；分公司：1-行政管理、2-市场销售、3-终端管理、4-运营商中心
        /// </summary>
        public int grade_category { get; set; }
        /// <summary>
        /// 行政管理、运营商中心、市场销售、终端管理
        /// </summary>
        public string grade_category_display { get; set; }
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
        /// 0-部门；1-区域
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string note { get; set; }
        /// <summary>
        /// 创建人信息记录ID；表daoben_hr_emp_job_history
        /// </summary>
        public string creator_job_history_id { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime create_time { get; set; }
        /// <summary>
        /// 创建人名称
        /// </summary>
        public string creator_name { get; set; }
        /// <summary>
        /// 是否能被删除：0-不可删除，1-可删除
        /// </summary>
        public bool delible { get; set; }
    }
}