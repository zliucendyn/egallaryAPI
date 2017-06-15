using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace EGalleryAPI.Filters
{
    public class BeeFreeFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);
            if (actionContext.Request.Headers.Contains("X-BEE-Uid"))
            {
                if (actionContext.Request.Headers.GetValues("X-BEE-Uid").First() == "test1-dotnet")
                {

                }else
                {
                    actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
                }
            }else
            {
                actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            }
        }
    }
}