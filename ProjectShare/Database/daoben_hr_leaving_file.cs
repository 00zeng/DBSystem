using System;

namespace ProjectShare.Database
{
    public class daoben_hr_leaving_file
    {
        /// <summary>
        /// 员工请假文件表
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 1-准生证；2-生育证明；3-结婚证；4-死亡证明；5-请假条；6-其他
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