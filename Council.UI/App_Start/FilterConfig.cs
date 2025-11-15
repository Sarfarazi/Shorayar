using System.Web;
using System.Web.Mvc;

namespace Council.UI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());
            filters.Add(new CustomErrorHandler());
        }
    }
}
