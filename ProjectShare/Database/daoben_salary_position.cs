using System;

namespace ProjectShare.Database
{
    public class daoben_salary_position
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 生效时间
        /// </summary>
        public DateTime? effect_date { get; set; }
        /// <summary>
        /// 1-立即生效(审批后)
        /// </summary>
        public bool effect_now { get; set; }
        /// <summary>
        /// -2-审核未结束；-1未生效；1有效；2-已失效
        /// </summary>
        public int effect_status { get; set; }
        /// <summary>
        /// 导入文件名
        /// </summary>
        public string file_name { get; set; }
        /// <summary>
        /// 被设置职位ID
        /// </summary>
        public int position_id { get; set; }
        /// <summary>
        /// 被设置职位名称
        /// </summary>
        public string position_name { get; set; }
        /// <summary>
        /// 被设置部门ID
        /// </summary>
        public int dept_id { get; set; }
        /// <summary>
        /// 被设置部门名称
        /// </summary>
        public string dept_name { get; set; }
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
        public string company_name { get; set; }
        /// <summary>
        /// 类型显示：1-导购薪资，3-工龄工资，4-职等薪资导入，11-培训师KPI(培训薪资)，12-培训经理KPI(培训薪资)，13-部门薪资; 21-业务经理KPI；22-业务员KPI
        /// </summary>
        public uint category { get; set; }
        /// <summary>
        /// 类型显示：导购薪资，工龄工资，职等薪资导入，培训师KPI，培训经理KPI，部门薪资，业务经理KPI，业务员KPI，
        /// </summary>
        public string category_display { get; set; }
        /// <summary>
        /// 1-公版且覆盖；2-公版不覆盖；3-非公版
        /// </summary>
        public int is_template { get; set; }
        /// <summary>
        /// 当前审批状态（流程）
        /// </summary>
        public int approve_status { get; set; }
        /// <summary>
        /// 创建人信息记录ID；表daoben_hr_emp_job_history
        /// </summary>
        public string creator_job_history_id { get; set; }
        /// <summary>
        /// 创建人名称
        /// </summary>
        public string creator_name { get; set; }
        /// <summary>
        /// 创建人职位
        /// </summary>
        public string creator_position_name { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime create_time { get; set; }
    }
}