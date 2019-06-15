using System;

namespace ProjectShare.Database
{
    public class daoben_hr_emp_file
    {
        /// <summary>
        /// id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 1-头像；2-身份证；3-简历；4-毕业证；5-学位证；11-入职登记表；12-合同
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 关联主表id
        /// </summary>
        public string main_id { get; set; }
        /// <summary>
        /// 完整URL
        /// </summary>
        public string url_path { get; set; }
        /// <summary>
        /// 文件存放路径，含完整文件名
        /// </summary>
        public string file_path { get; set; }
        /// <summary>
        /// 文件名，含后缀名
        /// </summary>
        public string file_name { get; set; }
        /// <summary>
        /// 文件原始名称
        /// </summary>
        public string original_name { get; set; }
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
        /// <summary>
        /// 删除操作人ID
        /// </summary>
        public int del_account_id { get; set; }
        /// <summary>
        /// 0-未删除；1-已删除
        /// </summary>
        public bool is_del { get; set; }
        /// <summary>
        /// 删除时间
        /// </summary>
        public DateTime del_time { get; set; }
    }
}