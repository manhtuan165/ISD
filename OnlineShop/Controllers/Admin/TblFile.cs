﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TK.Business.Common;
using TK.Business.Model;
using TK.Business.Dao;
using TK.Business.Data;
using System.Web.Mvc;
using System.IO;
using System.Net;

namespace TkSchoolNews.Controllers
{
    public partial class AdminController
    {
        [Authorize]
        [HttpGet]
        public ActionResult TblFileIndex()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                logger.Info(ControllerName + "::TblClassListIndex::" + ex.Message);
                return RedirectToAction("Index", "Error");
            }
        }
        public JsonResult TblFileList()
        {
            try
            {
                var list = new TblFileDao().FindByAll();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Info(ControllerName + "::TblClassList::" + ex.Message);
                return Json(JsonRequestBehavior.AllowGet);
            }
        }
        
        public ActionResult TblFileCreate(HttpPostedFileBase file)
        {
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    var filename = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/App_Data/Uploads"), filename);
                    file.SaveAs(path);
                    TblFile o = new TblFile();
                    o.Name = filename;
                    o.Directory = "~/App_Data/Uploads/";
                    o.CreateUser = GetUserName();
                    o.CreateDate = DateTime.Now;
                    new TblFileDao().Create(o);
                    SetAlert("tải thành công", "success");
                    return RedirectToAction("TblFileIndex");
                }
                SetAlert("tải thành công", "success");
                return RedirectToAction("TblFileIndex");
            }
            catch (Exception ex)
            {
                logger.Info(ControllerName + "::TblClassListCreate::" + ex.Message);
                return RedirectToAction("Index", "Error");
            }
        }
        public void DownloadFile(string filepath, string filename)
        {
            try
            {
                HttpContext.Response.ContentType = "APPLICATION/OCTET-STREAM";
                string name = filename.Replace(',' ,'_');
                String Header = "Attachment; Filename="+name;
                HttpContext.Response.AppendHeader("Content-Disposition", Header);
                FileInfo Dfile = new FileInfo(HttpContext.Server.MapPath(filepath + filename));
                HttpContext.Response.WriteFile(Dfile.FullName);
                HttpContext.Response.End();
            }
            catch (Exception ex)
            {
                logger.Info(ControllerName + "::DonwloadFile::" + ex.Message);
            }
        }
        public JsonResult TblFileDelete(long id)
        {
            TblFile o = new TblFile();
            o = new TblFileDao().FindById(id);
            o.Id = id;
            FileInfo Dfile = new FileInfo(HttpContext.Server.MapPath("~/App_Data/Uploads/" + o.Name));
            if (System.IO.File.Exists(Dfile.ToString()))
            {
                try
                {
                    System.IO.File.Delete(Dfile.ToString());
                }
                catch(System.IO.IOException ex)
                {
                    logger.Info("::DeleteFile::" + ex.Message);
                }
            }
            new TblFileDao().Delete(o);
            return Json(JsonRequestBehavior.AllowGet);
        }
    }
}