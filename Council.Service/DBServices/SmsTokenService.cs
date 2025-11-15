using System;
using Council.Core.Entities;
using Council.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Service.DBServices
{
    public class SmsTokenService:CRUDServices<SMSToken>,ISmsToken
    {
    }
}
