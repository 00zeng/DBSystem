using System.ComponentModel;

namespace ProjectShare.Models
{
    public class AppCache
    {
    }
    public enum CachePrefix
    {
        [Description("权限")]
        AUTHORITY = 2,

        [Description("经销商树状信息")]
        DISTRIBUTORTREE = 3,
        [Description("员工树状信息")]
        EMPLOYEETREE = 4,
    }
}
