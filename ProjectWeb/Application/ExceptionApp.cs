using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Base.Code;
using System.Text;

namespace ProjectWeb.Application
{
    /// <summary>
    /// 操作错误日志
    /// </summary>
    public class ExceptionApp
    {
        public static void WriteLog(string error)
        {
            string logPath = "/SystemLog/Exception/";
            string physicalPath = AppDomain.CurrentDomain.BaseDirectory + logPath;
            if (!Directory.Exists(physicalPath))
                Directory.CreateDirectory(physicalPath);
            string fullName = physicalPath + DateTime.Now.ToString("yyyyMMdd") + ".txt";

            string fileContent = DateTime.Now.ToString() + "  " + error + "\r\n";//错误信息
            File.AppendAllText(fullName, fileContent, Encoding.UTF8);
        }
    }
}
