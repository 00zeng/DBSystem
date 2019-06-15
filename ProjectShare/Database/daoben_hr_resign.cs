using System;

namespace ProjectShare.Database
{
    public class daoben_hr_resign
    {
        /// <summary>
        /// 员工离职信息表
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 工号
        /// </summary>
        public string work_number { get; set; }
        /// <summary>
        /// 对应员工ID
        /// </summary>
        public string emp_id { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string emp_name { get; set; }
        /// <summary>
        /// 入职时间
        /// </summary>
        public DateTime? entry_date { get; set; }
        /// <summary>
        /// 性别: 0-未指定，1-男，2-女
        /// </summary>
        public int gender { get; set; }
        /// <summary>
        /// 文化程度
        /// </summary>
        public string education { get; set; }
        /// <summary>
        /// 等级
        /// </summary>
        public string grade { get; set; }
        /// <summary>
        /// 所属职位ID
        /// </summary>
        public int position_id { get; set; }
        /// <summary>
        /// 所属职位名称
        /// </summary>
        public string position_name { get; set; }
        /// <summary>
        /// 所属部门ID
        /// </summary>
        public int dept_id { get; set; }
        /// <summary>
        /// 所属部门名称
        /// </summary>
        public string dept_name { get; set; }
        /// <summary>
        /// 所属区域ID
        /// </summary>
        public int area_id { get; set; }
        /// <summary>
        /// 所属区域名称
        /// </summary>
        public string area_name { get; set; }
        /// <summary>
        /// 所属机构ID
        /// </summary>
        public int company_id { get; set; }
        /// <summary>
        /// 所属机构名称
        /// </summary>
        public string company_name { get; set; }
        /// <summary>
        /// 所属机构上级ID
        /// </summary>
        public int company_id_parent { get; set; }
        /// <summary>
        /// 所在经销商ID
        /// </summary>
        public string distributor_id { get; set; }
        /// <summary>
        /// 所在经销商
        /// </summary>
        public string distributor_name { get; set; }
        /// <summary>
        /// 离职方式：1-正常离职；2-辞退；3-自离
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 离职时间
        /// </summary>
        public DateTime? request_date { get; set; }
        /// <summary>
        /// 核准离职时间
        /// </summary>
        public DateTime? date_approve { get; set; }
        /// <summary>
        /// 离职原因
        /// </summary>
        public string content_reason { get; set; }
        /// <summary>
        /// 后续发展安排
        /// </summary>
        public string content_arrangement { get; set; }
        /// <summary>
        /// 意见
        /// </summary>
        public string content_opinion { get; set; }
        /// <summary>
        /// 状态：-2-审批未结束；-1未执行；2-已结束
        /// </summary>
        public int execute_status { get; set; }
        /// <summary>
        /// 审批状态：0-未审批；100表示审批完成且通过
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
        /// 创建时间
        /// </summary>
        public DateTime create_time { get; set; }

    }
}