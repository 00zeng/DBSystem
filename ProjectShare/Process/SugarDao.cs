using System;
using MySqlSugar;
using System.Configuration;
namespace ProjectShare.Process
{
    /// <summary>
    /// SqlSugar
    /// </summary>
    public class SugarDao
    {
        //禁止实例化
        private SugarDao()
        {

        }
        public static string ConnectionString
        {
            get
            {
                string reval = ConfigurationManager.ConnectionStrings["DbConnect"].ConnectionString;
                return reval;
            }
        }
        private static string ConnectionCmsString(string con)
        {
            string reval = ConfigurationManager.ConnectionStrings[con].ConnectionString;
            return reval;
        }
        public static SqlSugarClient GetInstance(string con = "DbConnect")
        {
            var db = new SqlSugarClient(ConnectionCmsString(con));
            db.IsIgnoreErrorColumns = true;//忽略非数据库列
            db.IsEnableLogEvent = true;//启用日志事件
            db.LogEventStarting = (sql, par) => { Console.WriteLine(sql + " " + par + "\r\n"); };
            return db;
        }
    }
}