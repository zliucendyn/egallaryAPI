using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGalleryAPIData.Repo
{
    public class EGalleryAPIRepo
    {
        private eContact_Cendyn_TeamDevEntities context;

        public EGalleryAPIRepo()
        {
            context = new eContact_Cendyn_TeamDevEntities();
        }

        public string GetFolderPathByCompanyID(int companyId)
        {
            if (context.eContact_Settings.Any(x => x.CompanyID == companyId && x.SettingName== "SEImageGalleryPath"))
                return context.eContact_Settings.Where(x => x.CompanyID == companyId && x.SettingName == "SEImageGalleryPath").First().SettingValue;
            return "";
        }
    }
}
