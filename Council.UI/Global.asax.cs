using Council.UI.CustomAuthentication;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace Council.UI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
           // Stimulsoft.Base.StiLicense.Key = "6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHkcgIvwL0jnpsDqRpWg5FI5kt2G7A0tYIcUygBh1sPs7sUZct1/USoUgY9XBwlar38S6cneciUIxW2fNCDuEQxH+YsZ3av3XB9ANA1ky5svdTCkz8cpYIDcdGLGIWE76CjGIxA2sDA09veSwsJnNcow901SYAFjoDMinyUIHQRPcmVo9clh7Tl3T4yqZnTemwtQAssvvsnhBsvSmZhVGEw2d+b/835Q4ZgwOii29RvKn9CoIuI5mxETV3DcNugq5FDqOjYjuAMfTE4n26zhuaEaRrtxeYZk/Z854LcFoKkBTPkJb/o0glIJQKvN+YzaNMzRuTVGQvc1qJZZC7KonTEehGRUkT1dTqWVlKDkv1uxNZ+/WtH8hDZj40ioWqc2cp0yAGkI5ItFDZ0FlFfhqMOiE0Wk/EaVTjAeo9sdfmHxuzby0hU2qUSa77ItCAbjL5yudrcAzWUHlPLESyy2Sa/20KxzU7qeUemem6ersrSbGmTfmv1BSlFMn6fDQjD/2G97K3FZriV5BC0bM3awRA1Z";
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        //protected void Application_Error()
        //{
        //    var exception = Server.GetLastError();
        //    if (exception is HttpException)
        //    {
        //        var httpException = (HttpException)exception;
        //        Response.StatusCode = httpException.GetHttpCode();
        //    }
        //}

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = HttpContext.Current.Request.Cookies["_ash"];

            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                CustomSerializeModel serializeModel = JsonConvert.DeserializeObject<CustomSerializeModel>(authTicket.UserData);
                CustomPrincipal User = new CustomPrincipal(authTicket.Name);
                User.UserId = serializeModel.UserId;
                User.FirstName = serializeModel.FirstName;
                User.LastName = serializeModel.LastName;
                User.Roles = serializeModel.Roles;

                HttpContext.Current.User = User;
            }
        }
    }
}
