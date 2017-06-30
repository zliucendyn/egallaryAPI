using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGalleryAPIData.RepoInterface
{
    public interface IEGalleryAPIRepo
    {
        string GetFolderPathByCompanyID(int companyId);
        string GetClientFolderPathByCompanyID(int companyId);

    }
}
