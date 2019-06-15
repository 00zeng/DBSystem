namespace ProjectShare.Database
{
    public class daoben_sys_menu
    {
        /// <summary>
        /// id
        /// </summary>		
        public string id { get; set; }
        /// <summary>
        /// 编码
        /// </summary>		
        public string encode { get; set; }
        /// <summary>
        /// 菜单名
        /// </summary>		
        public string name { get; set; }
        /// <summary>
        /// 父元素code
        /// </summary>		
        public string parent_code { get; set; }
        /// <summary>
        /// 排序
        /// </summary>		
        public string sort { get; set; }
        /// <summary>
        /// 跳转路径
        /// </summary>		
        //[DisplayFormat(ConvertEmptyStringToNull = false)]
        public string url { get; set; }
        /// <summary>
        /// 参数(用&分开)
        /// </summary>		
        //[DisplayFormat(ConvertEmptyStringToNull = false)]
        public string param { get; set; }
        /// <summary>
        /// 菜单图标(class名称)
        /// </summary>		
        //[DisplayFormat(ConvertEmptyStringToNull = false)]
        public string icon { get; set; }
        /// <summary>
        /// 是否有效（0无效，1有效）
        /// </summary>		
        public bool active { get; set; }
    }
}