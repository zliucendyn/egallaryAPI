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
using EGalleryAPIData.Repo;
using System.Drawing.Imaging;
using System.Drawing;

namespace EGalleryAPI.Controllers.api
{
    public class EGalleryImageController : ApiController
    {
        //Function receive a url or file path, return the meta data for the folder or the file
        //GET API CALL ex get /test/
        [HttpGet]
        [Route("api/EGallery/{*folderPath}")]
        public HttpResponseMessage GetIFolderInfo(string folderPath)
        {
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~");
            bool rootPath = String.IsNullOrEmpty(folderPath);
            //For c# webapi, if the request comes like api/egallery/, the last forward slash will be removed
            //in that case need to manual add the forward slash.
            if (rootPath)
                folderPath = "/";
            else
                folderPath = "/" + folderPath;
            bool isShareCompany = folderPath.Contains("Shared_Company");
            bool isStock = folderPath.Contains("Stock");
            bool isDir = folderPath[folderPath.Length - 1] == '/';
            // This Section is to check if the folderpath is point to stock or sharedcompany, if yes, we need to do different logic.
            // the reason to do this is the stock and shared company folder has different location then the property folder
            if (isStock)
            {
                return GetDataForStock(path, folderPath, isDir);
            }else if (isShareCompany)
            {
                return GetDataForShareFolder(path, folderPath, 621, isDir);
            }else
            {
                return GetDataForPropertyFolder(path, folderPath, 621, isDir, rootPath);
            }
        }

        private string CheckFileType(string fileName){
            string ext=fileName.Split('.')[1].ToLower();
            if(ext =="jpg"||ext=="png"||ext=="jpeg")
                return "image/png";
            return "application/directory";
        }


        //[HttpPost]
        //[Route("api/EGallery")]
        //public HttpResponseMessage PostNewDirectory()
        //{
        //    HttpResponseMessage hrm = new HttpResponseMessage();
        //    string path = System.Web.Hosting.HostingEnvironment.MapPath("~");
        //    BeeFreeResultsDTO2 result = new BeeFreeResultsDTO2();
        //    string folderpath = null;
        //    if(Request.Headers.Contains("X-BEE-FSP-Directory"))
        //        folderpath= Request.Headers.GetValues("X-BEE-FSP-Directory").First();
        //    if (folderpath == null || folderpath[0] != '/' || folderpath[folderpath.Length - 1] != '/')
        //        hrm.StatusCode = HttpStatusCode.NotAcceptable;
        //    int index = FindLastSecondSlashIndex(folderpath);
        //    if (Directory.Exists(path + baseUrl + folderpath.Substring(0, index + 1)))
        //    {
        //        if(!File.Exists(path + baseUrl + folderpath)){
        //            Directory.CreateDirectory(path + baseUrl + folderpath);
        //            DirectoryInfo di = new DirectoryInfo(path + baseUrl+ folderpath);
        //            DirectoryMetaDTO dir = new DirectoryMetaDTO("application/directory", di.Name, folderpath.Substring(0, folderpath.Length - 1), di.LastWriteTime.Ticks / TimeSpan.TicksPerMillisecond, 0, "rw", new Extra(), di.GetDirectories().Length + di.GetFiles().Length);
        //            BeeFreeResultsDataDTO2 bfr = new BeeFreeResultsDataDTO2();
        //            bfr.meta = dir;
        //            result.status = "success";
        //            result.data = bfr;
        //            hrm.StatusCode = HttpStatusCode.OK;
        //        }else{

        //        }
        //    }
        //    string jsondata = JsonConvert.SerializeObject(result);
        //    jsondata = jsondata.Replace("publicUrl", "public-url").Replace("lastModified", "last-modified").Replace("mimeType", "mime-type").Replace("itemCount", "item-count");
        //    hrm.Content = new StringContent(jsondata, Encoding.UTF8, "application/json");
        //    return hrm;
        //}

        [HttpPost]
        [Route("api/EGallery")]
        public HttpResponseMessage PostChangedImage()
        {
            HttpResponseMessage hrm = new HttpResponseMessage();
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~");
            BeeFreeResultsDTO2 result = new BeeFreeResultsDTO2();
            string folderpath = null;
            string imageSource = null;
            string imageName = null;
            if (Request.Headers.Contains("X-BEE-FSP-Directory"))
                folderpath = Request.Headers.GetValues("X-BEE-FSP-Directory").First();
            if (Request.Headers.Contains("X-BEE-FileName"))
                imageName = Request.Headers.GetValues("X-BEE-FileName").First();
            if (Request.Headers.Contains("X-BEE-Source"))
                imageSource = Request.Headers.GetValues("X-BEE-Source").First();
            if (folderpath == null || folderpath[0] != '/' || folderpath[folderpath.Length - 1] != '/')
                hrm.StatusCode = HttpStatusCode.NotAcceptable;
            EGalleryAPIRepo repo = new EGalleryAPIRepo();
            string baseUrl = repo.GetFolderPathByCompanyID(621);
            path += baseUrl;
            folderpath =  "/myfiles" + folderpath;
            using (WebClient client = new WebClient())
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                client.DownloadFile(new Uri(imageSource), path + folderpath + imageName);
            }
            return GetMetaDataForFile(path, folderpath, imageName);
            //int index = FindLastSecondSlashIndex(folderpath);
            //if (Directory.Exists(path  + folderpath.Substring(0, index + 1)))
            //{
            //    if (!File.Exists(path  + folderpath))
            //    {
            //        Directory.CreateDirectory(path  + folderpath);
            //        DirectoryInfo di = new DirectoryInfo(path  + folderpath);
            //        DirectoryMetaDTO dir = new DirectoryMetaDTO("application/directory", di.Name, folderpath.Substring(0, folderpath.Length - 1), di.LastWriteTime.Ticks / TimeSpan.TicksPerMillisecond, 0, "rw", new Extra(), di.GetDirectories().Length + di.GetFiles().Length);
            //        BeeFreeResultsDataDTO2 bfr = new BeeFreeResultsDataDTO2();
            //        bfr.meta = dir;
            //        result.status = "success";
            //        result.data = bfr;
            //        hrm.StatusCode = HttpStatusCode.OK;
            //    }
            //    else
            //    {

            //    }
            //}
            //string jsondata = JsonConvert.SerializeObject(result);
            //jsondata = jsondata.Replace("publicUrl", "public-url").Replace("lastModified", "last-modified").Replace("mimeType", "mime-type").Replace("itemCount", "item-count");
            //hrm.Content = new StringContent(jsondata, Encoding.UTF8, "application/json");
            //return hrm;
        }

        public HttpResponseMessage GetMetaDataForFile(string path, string folderpath, string fileName)
        {
            HttpResponseMessage hrm = new HttpResponseMessage();
            BeeFreeResultsDTO2 result = new BeeFreeResultsDTO2();
            if (File.Exists(path+folderpath+fileName))
            {
                FileInfo fi = new FileInfo(path + folderpath + fileName);
                result.status = "success";
                FileMetaDTO file = new FileMetaDTO(CheckFileType(fi.Name), fi.Name, folderpath +fi.Name, fi.LastWriteTime.Subtract(new DateTime(1970, 1, 1)).Ticks, fi.Length, "rw", new Extra(), ConfigurationManager.AppSettings["FileSystemPath"] + folderpath + fi.Name, ConfigurationManager.AppSettings["FileSystemPath"] + folderpath+ fi.Name);
                BeeFreeResultsDataDTO2 datares = new BeeFreeResultsDataDTO2();
                datares.meta = file;
                result.data = datares;
                hrm.StatusCode = HttpStatusCode.OK;
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

        // This function is to handle the request which goes to the stock folder, stock folder is shared by clients
        private HttpResponseMessage GetDataForStock(string path, string folderPath, bool isDir)
        {
            HttpResponseMessage hrm = new HttpResponseMessage();
            BeeFreeResultsDTO result = new BeeFreeResultsDTO();
            string baseUrl = "/egallery/upload";
            path += baseUrl + "/" + folderPath;
            if (isDir)
            {
                //check if path exist, if exists, we return the exists folder structure, else we check if it is root path
                //root path means the top property level folder, if yes, create a folder with the property name, meanwhile
                //create subfolder named files for the property level folder
                if (Directory.Exists(path))
                {
                    result.status = "success";
                    result.data = GetFolderView(path, folderPath, baseUrl);
                    hrm.StatusCode = HttpStatusCode.OK;
                }
                else
                {
                    hrm.StatusCode = HttpStatusCode.NotFound;
                    result.status = "fail";
                    result.data.message = "Folder Not Found";
                    result.data.details = path + " not exists";
                }
            }
            else
            {
                //if the path is not a directory path, API will take it as a file, return the file meta data if exists
                //or return file not found, 404 http error
                if (File.Exists(path))
                {
                    FileInfo fi = new FileInfo(path);
                    result.status = "success";
                    FileMetaDTO file = new FileMetaDTO(CheckFileType(fi.Name), fi.Name, folderPath, fi.LastWriteTime.Subtract(new DateTime(1970, 1, 1)).Ticks, fi.Length, "rw", null, ConfigurationManager.AppSettings["FileSystemPath"] + folderPath, ConfigurationManager.AppSettings["FileSystemPath"] + folderPath);
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
            //the bee free api need the some property has '-', which is not support in C# as Property
            //so I change them in json string, then it could be understand by bee free
            string jsondata = JsonConvert.SerializeObject(result);
            jsondata = jsondata.Replace("publicUrl", "public-url").Replace("lastModified", "last-modified").Replace("mimeType", "mime-type").Replace("itemCount", "item-count");
            hrm.Content = new StringContent(jsondata, Encoding.UTF8, "application/json");
            return hrm;
        }

        private HttpResponseMessage GetDataForShareFolder(string path, string folderPath, int compantid, bool isDir)
        {
            HttpResponseMessage hrm = new HttpResponseMessage();
            BeeFreeResultsDTO result = new BeeFreeResultsDTO();
            EGalleryAPIRepo repo = new EGalleryAPIRepo();
            string baseUrl = repo.GetClientFolderPathByCompanyID(compantid);
            path += baseUrl + "/" + folderPath;
            if (isDir)
            {
                //check if path exist, if exists, we return the exists folder structure, else we check if it is root path
                //root path means the top property level folder, if yes, create a folder with the property name, meanwhile
                //create subfolder named files for the property level folder
                if (Directory.Exists(path))
                {
                    result.status = "success";
                    result.data = GetFolderView(path, folderPath, baseUrl);
                    hrm.StatusCode = HttpStatusCode.OK;
                }
                else
                {
                    if(folderPath== "Shared_Company/")
                    {
                        Directory.CreateDirectory(path);
                        result.status = "success";
                        result.data = GetFolderView(path, folderPath, baseUrl);
                        hrm.StatusCode = HttpStatusCode.OK;
                    }
                    else
                    {
                        hrm.StatusCode = HttpStatusCode.NotFound;
                        result.status = "fail";
                        result.data.message = "Folder Not Found";
                        result.data.details = path + " not exists";
                    }
                }
            }
            else
            {
                //if the path is not a directory path, API will take it as a file, return the file meta data if exists
                //or return file not found, 404 http error
                if (File.Exists(path))
                {
                    FileInfo fi = new FileInfo(path);
                    result.status = "success";
                    FileMetaDTO file = new FileMetaDTO(CheckFileType(fi.Name), fi.Name, folderPath, fi.LastWriteTime.Subtract(new DateTime(1970, 1, 1)).Ticks, fi.Length, "rw", null, ConfigurationManager.AppSettings["FileSystemPath"] + folderPath, ConfigurationManager.AppSettings["FileSystemPath"] + folderPath);
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
            //the bee free api need the some property has '-', which is not support in C# as Property
            //so I change them in json string, then it could be understand by bee free
            jsondata = jsondata.Replace("publicUrl", "public-url").Replace("lastModified", "last-modified").Replace("mimeType", "mime-type").Replace("itemCount", "item-count");
            hrm.Content = new StringContent(jsondata, Encoding.UTF8, "application/json");
            return hrm;
        }

        private HttpResponseMessage GetDataForPropertyFolder(string path, string folderPath, int compantid, bool isDir, bool rootPath)
        {
            HttpResponseMessage hrm = new HttpResponseMessage();
            BeeFreeResultsDTO result = new BeeFreeResultsDTO();
            EGalleryAPIRepo repo = new EGalleryAPIRepo();
            string baseUrl = repo.GetFolderPathByCompanyID(compantid);
            if (String.IsNullOrEmpty(baseUrl))
            {
                //need to check baseUrl value
            }
            path += baseUrl + folderPath;
            //check if path is directory, if it is directory, we need to return the DirectoryMeta data to bee free,
            //then they will know this is a folder metadata
            if (isDir)
            {
                //check if path exist, if exists, we return the exists folder structure, else we check if it is root path
                //root path means the top property level folder, if yes, create a folder with the property name, meanwhile
                //create subfolder named files for the property level folder
                if (Directory.Exists(path))
                {
                    result.status = "success";
                    result.data = GetFolderView(path, folderPath, baseUrl);
                    hrm.StatusCode = HttpStatusCode.OK;
                }
                else
                {
                    if (rootPath)
                    {
                        CreateRootPathFolder(path);
                        result.data = GetFolderView(path, folderPath, baseUrl);
                        hrm.StatusCode = HttpStatusCode.OK;
                    }
                    else
                    {
                        hrm.StatusCode = HttpStatusCode.NotFound;
                        result.status = "fail";
                        result.data.message = "Folder Not Found";
                        result.data.details = path + " not exists";
                    }
                }
                //if it is a root Path, need to add the shared company folder
                if (rootPath)
                {
                    GetCommonSharedFolder(result, path);
                }
            }
            else
            {
                //if the path is not a directory path, API will take it as a file, return the file meta data if exists
                //or return file not found, 404 http error
                if (File.Exists(path))
                {
                    FileInfo fi = new FileInfo(path);
                    result.status = "success";
                    FileMetaDTO file = new FileMetaDTO(CheckFileType(fi.Name), fi.Name, folderPath, fi.LastWriteTime.Subtract(new DateTime(1970, 1, 1)).Ticks, fi.Length, "rw", null, ConfigurationManager.AppSettings["FileSystemPath"] + folderPath, ConfigurationManager.AppSettings["FileSystemPath"] + folderPath);
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
            //the bee free api need the some property has '-', which is not support in C# as Property
            //so I change them in json string, then it could be understand by bee free
            jsondata = jsondata.Replace("publicUrl", "public-url").Replace("lastModified", "last-modified").Replace("mimeType", "mime-type").Replace("itemCount", "item-count");
            hrm.Content = new StringContent(jsondata, Encoding.UTF8, "application/json");
            return hrm;
        }

        private void CreateRootPathFolder(string path)
        {
            try
            {
                DirectoryInfo dir = Directory.CreateDirectory(path);
                dir.CreateSubdirectory("Files");
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }finally
            {
            }
        }

        private BeeFreeResultsDataDTO GetFolderView(string path, string folderPath, string baseUrl)
        {
            BeeFreeResultsDataDTO bfr = new BeeFreeResultsDataDTO();
            DirectoryInfo di = new DirectoryInfo(path);
            //To show the folder name add this baseUrl.Substring(baseUrl.LastIndexOf('/')) +
            DirectoryMetaDTO dir = new DirectoryMetaDTO("application/directory", folderPath == "/" ? "root" : di.Name, folderPath == "/" ?  "/" : folderPath, di.LastWriteTime.Subtract(new DateTime(1970, 1, 1)).Ticks / TimeSpan.TicksPerMillisecond, 0, "rw", new Extra(), di.GetDirectories().Length + di.GetFiles().Where(x => !x.Name.StartsWith("tn-")).Count()) ;
            if (dir.itemCount != 0)
            {
                IList<CommonMetaDTO> itemlists = new List<CommonMetaDTO>();
                foreach (var subdir in di.GetDirectories())
                {
                    DirectoryMetaDTO newdir = new DirectoryMetaDTO("application/directory", subdir.Name, folderPath + subdir.Name + '/', di.LastWriteTime.Subtract(new DateTime(1970, 1, 1)).Ticks / TimeSpan.TicksPerMillisecond,0,"rw", new Extra(), subdir.GetDirectories().Length + subdir.GetFiles().Where(x => !x.Name.StartsWith("tn-")).Count());
                    itemlists.Add(newdir);
                }
                //when get files exclude those thumbnails
                foreach (var fi in di.GetFiles().Where(x=>!x.Name.StartsWith("tn-")))
                {
                    //if folder has the image thumbnail, use the thumbnail, if not use the original pic
                    string thumbnailUrl = di.GetFiles().Any(x => x.Name == "tn-" + fi.Name) ? ConfigurationManager.AppSettings["FileSystemPath"] + baseUrl + folderPath + "tn-" + fi.Name : ConfigurationManager.AppSettings["FileSystemPath"] + baseUrl + folderPath + fi.Name;
                    FileMetaDTO file = new FileMetaDTO(CheckFileType(fi.Name), fi.Name, folderPath + fi.Name, fi.LastWriteTime.Subtract(new DateTime(1970, 1, 1)).Ticks / TimeSpan.TicksPerMillisecond, fi.Length, "rw", null, ConfigurationManager.AppSettings["FileSystemPath"] + baseUrl + folderPath + fi.Name, thumbnailUrl);
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

        private void GetCommonSharedFolder(BeeFreeResultsDTO result, string path)
        {
            int propertyIndex = 0;
            int clientIndex = 0;
            for(int i = path.Length - 2; i > 0; i--)
            {
                if (path[i] == '/')
                {
                    if (propertyIndex == 0)
                        propertyIndex = i;
                    else
                    {
                        clientIndex = i;
                        break;
                    }
                }
            }
            if(Directory.Exists(path.Substring(0, propertyIndex + 1) + "/Shared_Company/"))
            {
                DirectoryInfo dir = new DirectoryInfo(path.Substring(0, propertyIndex + 1) + "/Shared_Company/");
                DirectoryMetaDTO newdir = new DirectoryMetaDTO("application/directory", dir.Name, '/' + dir.Name + '/', dir.LastWriteTime.Subtract(new DateTime(1970,1,1)).Ticks / TimeSpan.TicksPerMillisecond, 0, "rw", new Extra(), dir.GetDirectories().Length + dir.GetFiles().Length);
                result.data.items.Add(newdir);
            }else
            {
                DirectoryInfo dir = Directory.CreateDirectory(path.Substring(0, propertyIndex + 1) + "/Shared_Company/");
                DirectoryMetaDTO newdir = new DirectoryMetaDTO("application/directory", dir.Name, '/' + dir.Name + '/', dir.LastWriteTime.Subtract(new DateTime(1970,1,1)).Ticks / TimeSpan.TicksPerMillisecond, 0, "rw", new Extra(), dir.GetDirectories().Length + dir.GetFiles().Length);
                result.data.items.Add(newdir);
            }
            if (Directory.Exists(path.Substring(0, clientIndex + 1) + "/Stock/"))
            {
                DirectoryInfo dir = new DirectoryInfo(path.Substring(0, clientIndex + 1) + "/Stock/");
                DirectoryMetaDTO newdir = new DirectoryMetaDTO("application/directory", dir.Name, '/' + dir.Name + '/', dir.LastWriteTime.Subtract(new DateTime(1970,1,1)).Ticks / TimeSpan.TicksPerMillisecond, 0, "rw", new Extra(), dir.GetDirectories().Length + dir.GetFiles().Length);
                result.data.items.Add(newdir);
            }
            else
            {
                result.status = "fail";
                BeeFreeResultsDataDTO datares = new BeeFreeResultsDataDTO();
                datares.message = "Stock Not Found";
                datares.details = path + " not exists";
                result.data = datares;
            }
        }

    }
}
