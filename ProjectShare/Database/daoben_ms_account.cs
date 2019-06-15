using System;

namespace ProjectShare.Database
{
    /// <summary>
    /// 账号信息
    /// </summary>
    public class daoben_ms_account
    {
        /// <summary>
		/// id
        /// </summary>		
        public int id { get; set; }
        /// <summary>
        /// 登录名
        /// </summary>		
        public string account { get; set; }
        /// <summary>
        /// 对应员工或经销商ID
        /// </summary>
        public string employee_id { get; set; }
        /// <summary>
        /// 对应员工名称或经销商名称
        /// </summary>
        public string employee_name { get; set; }
        /// <summary>
        /// 0-员工账户（employed_id对应表*_hr_emp_info）；
        /// 1-经销商账户（employee_id对应表*_distributor_info）；
        /// 2-店员账户（employee_id对应表*_distributor_clerk）
        /// </summary>
        public int employee_type { get; set; }
        /// <summary>
        /// 角色id
        /// </summary>
        public int role_id { get; set; }
        /// <summary>
        /// 角色名称，表daoben_ms_role
        /// </summary>
        public string role_name { get; set; }
        /// <summary>
        /// password
        /// </summary>		
        public string password { get; set; }
        /// <summary>
        /// 注册日期
        /// </summary>		
        public DateTime reg_time { get; set; }
        /// <summary>
        /// 最后登录日期
        /// </summary>		
        public DateTime? last_login { get; set; }
        /// <summary>
        /// 输入错误密码的次数
        /// </summary>		
        public int attempt_login { get; set; }
        /// <summary>
        /// 最后一次输入错误密码的时间
        /// </summary>		
        public DateTime? last_attempt { get; set; }
        /// <summary>
        /// 多次输入错误密码的情况下触发帐户锁定
        /// </summary>		
        public bool lockout { get; set; }     
        /// <summary>
        /// 0-正常 1-注销
        /// </summary>
        public bool inactive { get; set; }
        /// <summary>
        /// 注销人id
        /// </summary>
        public int inactive_id { get; set; }
        /// <summary>
        /// 注销人姓名（非账户名）
        /// </summary>
        public string inactive_name { get; set; }
        /// <summary>
        /// 注销时间
        /// </summary>
        public DateTime? inactive_time { get; set; }
        /// <summary>
        /// 创建人id
        /// </summary>
        public int creator_id { get; set; }
        /// <summary>
        /// 创建人姓名（非账户名）
        /// </summary>
        public string creator_name { get; set; }
    }
}
