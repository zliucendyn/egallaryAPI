using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EGalleryAPI.Models
{
    public class FileMetaDTO : CommonMetaDTO
    {
        public string publicUrl { get; set; }
        public string thumbnail { get; set; }
    }
}