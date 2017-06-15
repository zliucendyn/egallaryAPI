using EGalleryAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using EGalleryAPIData;
using EGalleryAPIData.Repo;

namespace EGalleryAPI.Controllers.api
{
    public class EGalleryImageController : ApiController
    {

        private string baseUrl = @"/eGallery/upload";

        //Function receive a url or file path, return the meta data for the folder or the file
        [HttpGet]
        [Route("api/EGalleryImage/{*folderPath}")]
        public IList<FileDTO> GetImagesByFolder(string folderPath)
        {
            IList<FileDTO> list = new List<FileDTO>();
            //var path = ConfigurationManager.AppSettings["FileSystemPath"];
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~");
            //path += @"/eGallery/upload/AMResorts/AMResorts/zhaotest";
            path += folderPath;
            if (Directory.Exists(path))
            {
                DirectoryInfo di = new DirectoryInfo(path);
                foreach (var fi in di.GetFiles())
                {
                    FileDTO image = new FileDTO();
                    image.FileName = fi.Name;
                    image.FileType = fi.Name.Split('.')[1];
                    image.FileUrl = folderPath + "/" + fi.Name;
                    image.FolderName = fi.Directory.Name;
                    list.Add(image);
                }
            }
            return list;
        }

        //Function receive a url or file path, return the meta data for the folder or the file
        //GET API CALL ex get /test/
        [HttpGet]
        [Route("api/EGallery/{*folderPath}")]
        public HttpResponseMessage GetIFolderInfo(string folderPath)
        {
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~");
            HttpResponseMessage hrm = new HttpResponseMessage();
            BeeFreeResultsDTO result = new BeeFreeResultsDTO();
            bool isDir = true;
            EGalleryAPIRepo repo = new EGalleryAPIRepo();
            baseUrl = repo.GetFolderPathByCompanyID(7098);
            if (String.IsNullOrEmpty(folderPath))
                folderPath = "/";
            else
                folderPath = "/" + folderPath;
            isDir=folderPath[folderPath.Length-1]=='/';
            path += baseUrl + folderPath;
            if (isDir)
            {
                if (Directory.Exists(path))
                {
                    result.status = "success";
                    result.data = GetFolderView(path, folderPath);   
                    hrm.StatusCode = HttpStatusCode.OK;
                }else{
                    hrm.StatusCode = HttpStatusCode.NotFound;
                    result.status = "fail";
                    result.data.message = "Folder Not Found";
                    result.data.details = path + " not exists";
                }
            }
            else
            {
                if (File.Exists(path))
                {
                    FileInfo fi = new FileInfo(path);
                    result.status = "success";
                    FileMetaDTO file=new FileMetaDTO();
                    file.name = fi.Name;
                    file.path = folderPath;
                    file.mimeType = CheckFileType(fi.Name);
                    file.lastModified = fi.LastWriteTime.Ticks;
                    file.size = fi.Length;
                    file.permissions = "rw";
                    file.publicUrl = ConfigurationManager.AppSettings["FileSystemPath"] + folderPath;
                    file.thumbnail = ConfigurationManager.AppSettings["FileSystemPath"] + folderPath;
                    file.extra = null;
                    BeeFreeResultsDataDTO datares = new BeeFreeResultsDataDTO();
                    datares.meta = file;
                    result.data = datares;
                    hrm.StatusCode = HttpStatusCode.OK;
                }
                else
                {
                    result.status = "fail";
                    BeeFreeResultsDataDTO datares = new BeeFreeResultsDataDTO();
                    datares.message = "Resource Not Found";
                    datares.details = path + " not exists";
                    result.data = datares;
                    hrm.StatusCode = HttpStatusCode.NotFound;
                }
            }
            string jsondata = JsonConvert.SerializeObject(result);
            jsondata = jsondata.Replace("publicUrl", "public-url").Replace("lastModified", "last-modified").Replace("mimeType", "mime-type").Replace("itemCount","item-count");
            hrm.Content = new StringContent(jsondata, Encoding.UTF8, "application/json");
            return hrm;
        }

        private string CheckFileType(string fileName){
            string ext=fileName.Split('.')[1].ToLower();
            if(ext =="jpg"||ext=="png"||ext=="jpeg")
                return "image/png";
            return "application/directory";
        }


        [HttpPost]
        [Route("api/EGallery")]
        public HttpResponseMessage PostNewDirectory()
        {
            HttpResponseMessage hrm = new HttpResponseMessage();
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~");
            BeeFreeResultsDTO2 result = new BeeFreeResultsDTO2();
            string folderpath = null;
            if(Request.Headers.Contains("X-BEE-FSP-Directory"))
                folderpath= Request.Headers.GetValues("X-BEE-FSP-Directory").First();
            if (folderpath == null || folderpath[0] != '/' || folderpath[folderpath.Length - 1] != '/')
                hrm.StatusCode = HttpStatusCode.NotAcceptable;
            int index = FindLastSecondSlashIndex(folderpath);
            if (Directory.Exists(path + baseUrl + folderpath.Substring(0, index + 1)))
            {
                if(!File.Exists(path + baseUrl + folderpath)){
                    Directory.CreateDirectory(path + baseUrl + folderpath);
                    DirectoryInfo di = new DirectoryInfo(path + baseUrl+ folderpath);
                    DirectoryMetaDTO dir = new DirectoryMetaDTO();
                    dir.name = di.Name;
                    dir.path = folderpath.Substring(0,folderpath.Length-1);
                    dir.mimeType = "application/directory";
                    dir.lastModified = di.LastWriteTime.Ticks / TimeSpan.TicksPerMillisecond;
                    dir.size = 0;
                    dir.permissions = "rw";
                    dir.itemCount = di.GetDirectories().Length + di.GetFiles().Length;
                    dir.extra = new Extra();
                    BeeFreeResultsDataDTO2 bfr = new BeeFreeResultsDataDTO2();
                    bfr.meta = dir;
                    result.status = "success";
                    result.data = bfr;
                    hrm.StatusCode = HttpStatusCode.OK;
                }else{

                }
            }
            string jsondata = JsonConvert.SerializeObject(result);
            jsondata = jsondata.Replace("publicUrl", "public-url").Replace("lastModified", "last-modified").Replace("mimeType", "mime-type").Replace("itemCount", "item-count");
            hrm.Content = new StringContent(jsondata, Encoding.UTF8, "application/json");
            return hrm;
        }

        public int FindLastSecondSlashIndex(string path)
        {
            for (int i = path.Length - 2; i > 0; i--)
            {
                if (path[i] == '/')
                    return i;
            }
            return 0;
        }



        private BeeFreeResultsDataDTO GetFolderView(string path, string folderPath)
        {
            BeeFreeResultsDataDTO bfr = new BeeFreeResultsDataDTO();
            DirectoryInfo di = new DirectoryInfo(path);
            DirectoryMetaDTO dir = new DirectoryMetaDTO();
            dir.name = folderPath == "/" ? "root" : di.Name;
            dir.path = folderPath == "/" ? "/" :  folderPath;
            dir.mimeType = "application/directory";
            dir.lastModified = di.LastWriteTime.Ticks / TimeSpan.TicksPerMillisecond;
            dir.size = 0;
            dir.permissions = "rw";
            dir.itemCount = di.GetDirectories().Length + di.GetFiles().Length;
            dir.extra = new Extra();
            if (dir.itemCount != 0)
            {
                IList<CommonMetaDTO> itemlists = new List<CommonMetaDTO>();
                foreach (var subdir in di.GetDirectories())
                {
                    DirectoryMetaDTO newdir = new DirectoryMetaDTO();
                    newdir.name = subdir.Name;
                    newdir.path = folderPath + subdir.Name + '/';
                    newdir.mimeType = "application/directory";
                    newdir.lastModified = di.LastWriteTime.Ticks / TimeSpan.TicksPerMillisecond;
                    newdir.size = 0;
                    newdir.permissions = "rw";
                    newdir.itemCount = subdir.GetDirectories().Length + subdir.GetFiles().Length;
                    newdir.extra = new Extra();
                    itemlists.Add(newdir);
                }
                foreach (var fi in di.GetFiles())
                {
                    FileMetaDTO file = new FileMetaDTO();
                    file.name = fi.Name;
                    file.path = folderPath + fi.Name;
                    file.mimeType = CheckFileType(fi.Name);
                    file.lastModified = fi.LastWriteTime.Ticks / TimeSpan.TicksPerMillisecond;
                    file.size = fi.Length;
                    file.permissions = "rw";
                    file.publicUrl = ConfigurationManager.AppSettings["FileSystemPath"] + file.path;
                    file.thumbnail = ConfigurationManager.AppSettings["FileSystemPath"] + file.path;
                    file.extra = new Extra();
                    itemlists.Add(file);
                }
                bfr.items = itemlists;
                bfr.meta = dir;
            }
            else
            {
                dir.path = folderPath;
                bfr.meta = dir;
            }
            return bfr;
        }

    }
}
