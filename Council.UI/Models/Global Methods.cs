using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace Council.UI.Models
{
    public static class Global
    {
        [WebMethod]
        public static void AbandonSession()
        {
            HttpContext.Current.Session.Abandon();
        }
    }
    
}