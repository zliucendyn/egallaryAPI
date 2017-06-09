using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EGalleryAPI.Models
{
    public class FolderViewDTO
    {
        public FolderViewDTO()
        {
            this.FolderList = new List<FolderDTO>();
            this.FileList = new List<FileDTO>();
        }
        public IList<FolderDTO> FolderList { get; set; }
        public IList<FileDTO> FileList { get; set; }
    }
}