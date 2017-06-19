using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EGalleryAPI.Models
{
    public class FileMetaDTO : CommonMetaDTO
    {

        public FileMetaDTO(string mineType, string name, string path, long lastModified, long size, string permission, Extra extra, string publicUrl, string thumbnail) : base(mineType, name, path, lastModified, size, permission, extra)
        {
            this.publicUrl = publicUrl;
            this.thumbnail = thumbnail;
        }

        public string publicUrl { get; set; }
        public string thumbnail { get; set; }
    }
}