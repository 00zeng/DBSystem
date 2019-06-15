using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ProjectWeb.HttpHandler
{
    /// <summary>
    /// TemplateDownload 的摘要说明
    /// </summary>
    public class TemplateDownload : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            try
            {
                string fileName = context.Request.QueryString["name"];
                string filePath = context.Server.MapPath("~/TemplateExcel/" + fileName);
                byte[] bytes = null;
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    bytes = new byte[fs.Length];
                    fs.Read(bytes, 0, bytes.Length);
                    fs.Dispose();
                }
                context.Response.ContentType = "application/octet-stream";
                context.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                context.Response.BinaryWrite(bytes);
                context.Response.Flush();
            }
            catch (Exception ex)
            {
                context.Response.Write("Error: " + ex.Message);
            }

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}