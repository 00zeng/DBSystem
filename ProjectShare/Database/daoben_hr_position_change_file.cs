using System;

namespace ProjectShare.Database
{
    public class daoben_hr_position_change_file
    {
        /// <summary>
        /// 岗位调整文件表
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 1-岗位调动交接表；2-盘库表；3-物料交接表；4-串码盘库交接表；5-物料盘库交接表；6-区域人员交接表；7-日常工作流程交接表
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