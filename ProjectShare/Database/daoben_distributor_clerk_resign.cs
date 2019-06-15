using System;

namespace ProjectShare.Database
{
    public class daoben_distributor_clerk_resign
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string phone { get; set; }
        /// <summary>
        /// 微信号
        /// </summary>
        public string wechat { get; set; }
        /// <summary>
        /// 经销商ID
        /// </summary>
        public string distributor_id { get; set; }
        /// <summary>
        /// 经销商名称
        /// </summary>
        public string distributor_name { get; set; }
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
        /// 当前挂勾导购员ID
        /// </summary>
        public string guide_id { get; set; }
        /// <summary>
        /// 当前挂勾导购员姓名
        /// </summary>
        public string guide_name { get; set; }
        /// <summary>
        /// 入职时间
        /// </summary>
        public DateTime entry_date { get; set; }
        /// <summary>
        /// 离职时间
        /// </summary>
        public DateTime resign_date { get; set; }
        /// <summary>
        /// 离职操作人ID
        /// </summary>
        public int operator_id { get; set; }
        /// <summary>
        /// 离职操作人姓名（非账户名）
        /// </summary>
        public string operator_name { get; set; }
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