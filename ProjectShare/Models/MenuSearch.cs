using System.Collections.Generic;

namespace ProjectShare.Models
{
    public class AuthMenuButton
    {
        public List<MenuSearch> menuList { get; set; }
        public List<MenuSearch> buttonList { get; set; }
    }
    public class MenuSearch
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
        /// 父级编码
        /// </summary>		
        public string parent_code { get; set; }
        /// <summary>
        /// 菜单名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 参数(用&分开)
        /// </summary>		
        public string param { get; set; }

        /// <summary>
        /// 跳转路径
        /// </summary>		
        public string url { get; set; }
        /// <summary>
        /// 是否有效（0无效，1有效）
        /// </summary>		
        public bool active { get; set; }
        /// <summary>
        /// 类型（1菜单，2按钮）
        /// </summary>		
        public int category { get; set; }
        /// <summary>
        /// 排序
        /// </summary>		
        public string sort { get; set; }
        /// <summary>
        /// 菜单图标(class名称)
        /// </summary>		
        public string icon { get; set; }
        /// <summary>
        /// 权限id
        /// </summary>		
        public int authority_id { get; set; }
        public List<MenuSearch> ChildNodes { get; set; }
    }
}
