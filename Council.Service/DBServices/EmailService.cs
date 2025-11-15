using Council.Core.Entities;
using Council.Core.Interfaces;
using Council.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Service.DBServices
{
    public class EmailService : CRUDServices<EmailInfo>, IEmailInfo
    {
        DataBase database = new DataBase();

        public EmailInfo GetEceInfo()
        {
            return this.All().Where(m => m.UserName == "info@shorayar.com").FirstOrDefault();             
        }
    }
}
