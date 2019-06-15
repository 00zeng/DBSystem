using System;

namespace ProjectShare.Models
{
    public class UploadFileInfo
    {
        /// <summary>
        /// 文件/图片类型（对应关联主表）：0-无；1-emp_info;
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 图片存储路径：员工：1-emp_info，2-emp_leaving，3-emp_career，4-emp_grade，5-emp_resign
        ///         经销商：1-经销商信息，2-形象返利
        /// </summary>
        public int src { get; set; }
        /// <summary>
        /// 系统模块：1-员工，2-经销商
        /// </summary>
        public int module { get; set; }
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
    }
}