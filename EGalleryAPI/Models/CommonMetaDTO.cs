using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EGalleryAPI.Models
{
    public class CommonMetaDTO
    {
        public CommonMetaDTO(string mineType,string name,string path, long lastModified,long size,string permission,Extra extra)
        {
            this.mimeType = mineType;
            this.name = name;
            this.path = path;
            this.lastModified = lastModified;
            this.size = size;
            this.permissions = permissions;
            this.extra = extra;
        }

        public string mimeType { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public long lastModified { get; set; }
        public long size { get; set; }
        public string permissions { get; set; }
        public Extra extra { get; set; }
    }
}