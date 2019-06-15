namespace ProjectShare.Database
{
    /// <summary>
    /// 权限
    /// </summary>
    public class daoben_sys_authority
    {
        /// <summary>
		/// 权限ID
        /// </summary>		
        public int id { get; set; }
        /// <summary>
        /// 角色ID
        /// </summary>		
        public int role_id { get; set; }
        /// <summary>
        /// 1-菜单；2-按钮
        /// </summary>		
        public int type { get; set; }
        /// <summary>
        /// 菜单/按钮ID
        /// </summary>		
        public string value_id { get; set; }
    }
}
