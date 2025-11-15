using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Core.Values
{
    public static class Values
    {
        // static string ConnectionString = "Data Source=192.168.1.130;Initial Catalog=Council;Persist Security Info=True;User ID=Meybod;Password=Meybod@10155";    
          static string ConnectionString = "Data Source=77.238.123.197;Initial Catalog=Council;Persist Security Info=True;User ID=sa;Password=sa@10155!@";
        // static string ConnectionString = "Data Source=.;Initial Catalog=Council;Persist Security Info=True;User ID=sa;Password=sa@10155";

        public static string MainConnectionString
        {
            get
            {
                return ConnectionString;
            }
        }       
    }
}
