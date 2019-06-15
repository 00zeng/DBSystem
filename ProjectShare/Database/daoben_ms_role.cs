using System;

namespace ProjectShare.Database
{
    /// <summary>
    /// 角色管理
    /// </summary>
    public class daoben_ms_role
    {
        /// <summary>
        /// 角色id
        /// </summary>		
        public int id { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 角色描述
        /// </summary>	
        public string role_desc { get; set; }
        /// <summary>
        /// 所属机构ID，预留
        /// </summary>
        public int org_id { get; set; }
        /// <summary>
        /// 0正常；1注销
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
        /// <summary>
        /// 创建日期
        /// </summary>		
        public DateTime create_time { get; set; }
        /// <summary>
        /// 是否能被删除：0-不可删除，1-可删除
        /// </summary>
        public bool delible { get; set; }

    }
}
