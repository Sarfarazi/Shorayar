using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Council.Data.Contexts
{
    internal static class ContextPerRequest
    {
        internal static MainContext MainContext
        {
            get
            {
                if (!HttpContext.Current.Items.Contains("mainContext"))
                {
                    HttpContext.Current.Items.Add("mainContext", new MainContext());
                }
                return HttpContext.Current.Items["mainContext"] as MainContext;
            }
        }
    }
}
