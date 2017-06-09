using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EGalleryAPI.Models
{
    public class BeeFreeResultsDTO
    {
        public string status { get; set; }
        public BeeFreeResultsDataDTO data { get; set; }
    }

    public class BeeFreeResultsDataDTO
    {
        public CommonMetaDTO meta { get; set; }
        public IList<CommonMetaDTO> items { get; set; }
        public string message { get; set; }
        public string details { get; set; }
    }
}