using System;

namespace ProjectShare.Database
{
    public class daoben_hr_grade_change_file
    {
        /// <summary>
        /// 晋升降级文件表
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 1-工作交接表；
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

    }
}