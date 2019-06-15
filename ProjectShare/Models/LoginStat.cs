namespace ProjectShare.Models
{
    public enum LoginStat
    {
        Success = 0,
        LockedOut = 1,
        RequiresVerification = 2,
        Failure = 3,
        SysError = 4,
        Forbidden = 5
    }

    public class DefaultAccountSetup
    {
        // 账号锁定时间（分钟）；连续输错密码的间隔时长（分钟），超过该间隔时长则重置连续输错次数
        public const int LockoutTimeSpan_FromMinutes = 5;
        // 连续输错密码最多的次数，超过时锁定账号
        public const int MaxAttemptsBeforeLockout = 5;
    }
}
