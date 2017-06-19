using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EGalleryAPI.Models
{
    public class DirectoryMetaDTO : CommonMetaDTO
    {
        public DirectoryMetaDTO(string mineType, string name, string path, long lastModified, long size, string permission, Extra extra, int itemCount) : base(mineType, name, path, lastModified, size, permission, extra)
        {
            this.itemCount = itemCount;
        }
        public int itemCount { get; set; }
    }
}