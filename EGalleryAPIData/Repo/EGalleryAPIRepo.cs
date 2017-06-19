using EGalleryAPIData.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGalleryAPIData.Repo
{
    public class EGalleryAPIRepo
    {
        private EGalleryApiDataModelEntities context;

        public EGalleryAPIRepo()
        {
            context = new EGalleryApiDataModelEntities();
        }

        public string GetFolderPathByCompanyID(int companyId)
        {
            if (context.eContact_Settings.Any(x => x.CompanyID == companyId) && context.Companies.Any(x => x.CompanyID == companyId))
            {
                return context.eContact_Settings.Where(x => x.CompanyID == companyId && x.SettingName == "SEImageGalleryPath").First().SettingValue + '/' + context.Companies.Where(x=>x.CompanyID==companyId).First().CompanyName;
            }
            return "";
        }

        public string GetClientFolderPathByCompanyID(int companyId)
        {
            if (context.eContact_Settings.Any(x => x.CompanyID == companyId))
            {
                return context.eContact_Settings.Where(x => x.CompanyID == companyId && x.SettingName == "SEImageGalleryPath").First().SettingValue;
            }
            return "";
        }

    }
}
