using System;
using Base.Code;

namespace ProjectWeb.Application
{
    public class CacheApp
    {
        #region 清除所有缓存
        public string RemoveCache()
        {
            try
            {
                CacheFactory.Cache().RemoveCache();
                return "success";
            }
            catch (Exception ex)
            {
                ExceptionApp.WriteLog("CacheApp(RemoveCache)：" + ex.Message);
                return "error";
            }
        }
        #endregion

        #region 清除所有缓存
        public void RemoveSingleCache(string cacheName)
        {
            try
            {
                CacheFactory.Cache().RemoveCache(cacheName);
            }
            catch (Exception ex)
            {
                ExceptionApp.WriteLog("CacheApp(RemoveSingleCache)：" + ex.Message);
            }
        }
        #endregion
    }
}
