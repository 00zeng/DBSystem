using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using ProjectShare.Models;

namespace ProjectWeb.Controllers
{
    [HandlerLogin]
    public class FileController : ControllerBase
    {
        public ActionResult UploadPictures()
        {
            return View();
        }

        public ActionResult UploadImages(int type, int src = 0, int module = 1)
        {
            UploadFileInfo upFileInfo = new UploadFileInfo();
            upFileInfo.type = type;
            upFileInfo.src = src;
            upFileInfo.module = module;
            try
            {
                //接受上传文件
                HttpPostedFileBase postFile = Request.Files["file"];
                string result = GetFileEntity(upFileInfo, postFile);
                if (result == "success")
                {
                    return Success("上传成功", upFileInfo);
                }
                else
                    return Error(result);
            }
            catch (Exception ex)
            {
                return Error("上传失败：" + ex.Message);
            }
        }

        public string GetFileEntity(UploadFileInfo fileEntity,
                    HttpPostedFileBase postFile)
        {
            if (postFile != null)
            {
                DateTime time = DateTime.Now;
                //获取上传目录 转换为物理路径
                string folder = null;
                if (fileEntity.module == 1)
                {
                    switch (fileEntity.src)
                    {
                        case 1: folder = ConstData.EMP_FILE_PATH; break;
                        case 2: folder = ConstData.EMP_LEAVING_PATH; break;
                        case 3: folder = ConstData.EMP_CAREER_PATH; break;
                        case 4: folder = ConstData.EMP_GRADE_PATH; break;
                        case 5: folder = ConstData.EMP_RESIGN_PATH; break;
                    }
                }
                else if (fileEntity.module == 2)
                {
                    switch (fileEntity.src)
                    {
                        case 1: folder = ConstData.DISTRIBUTOR_FILE_PATH; break;
                        case 2: folder = ConstData.REBATE_FILE_PATH; break;
                    }
                }
                else
                    return "系统模块错误";
                if (folder == null)
                    folder = ConstData.FILE_UP_PATH;

                string uploadPath = Server.MapPath(folder);
                //文件名
                string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");// Common.CreateNo();

                string name = postFile.FileName;
                name = name.Substring(name.LastIndexOf("\\") + 1);
                fileEntity.original_name = name;

                //后缀名称
                string fileExt = Path.GetExtension(name);
                fileExt = fileExt.ToLower();

                //如果不存在path目录
                if (!Directory.Exists(uploadPath))
                {
                    //那么就创建它
                    Directory.CreateDirectory(uploadPath);
                }
                //保存原文件
                string saveFile = uploadPath + fileName + fileExt;

                postFile.SaveAs(saveFile);

                fileEntity.url_path = "http://" + Request.Url.Host;
                if (Request.Url.Port > 0)
                    fileEntity.url_path += ":" + Request.Url.Port;
                fileEntity.url_path += folder + fileName + fileExt;
                fileEntity.file_path = saveFile;
                return "success";
            }
            else
            {
                return "请选择文件";
            }
        }

        [HttpPost]
        [HandlerAjaxOnly]
        // [ValidateAntiForgeryToken]
        public ActionResult DeleteLocalFile(UploadFileInfo fileEntity)
        {
            if (fileEntity != null)
            {
                string fullPath = fileEntity.file_path;
                if (!string.IsNullOrEmpty(fullPath))
                {
                    try
                    {
                        System.IO.File.Delete(fullPath);
                    }
                    catch { }
                }
            }
            return Success("删除成功");
        }

        //public ActionResult Import(List<daoben_product_price> priceList, string importFile)
        //{
        //    HttpPostedFileBase file = Request.Files["file"];
        //    if (file.ContentLength > 0)
        //    {
        //        string ext = Path.GetExtension(file.FileName).ToString().ToLower();
        //        if (ext != ".xls" && ext != ".xlsx")
        //        {
        //            return Error("导入失败：请上传Excel文件");
        //        }
        //        string fileName = file.FileName;//获取文件夹名称
        //        //获取上传目录 转换为物理路径
        //        string uploadPath = Server.MapPath(ConstData.EXCEL_UP_PATH);
        //        if (!Directory.Exists(uploadPath))
        //            Directory.CreateDirectory(uploadPath);
        //        string fullPath = uploadPath + fileName;
        //        file.SaveAs(fullPath);//将文件保存到服务器

        //        return Success("操作成功。");
        //    }
        //    else
        //        return Error("导入失败：文件出错");
        //}

    }
}