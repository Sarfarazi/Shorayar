using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Council.Core.Entities;
using Council.Core.Interfaces;

namespace Council.Service.DBServices
{
    public class DefaultStatementService : CRUDServices<DefaultStatement>, IDefaultStatement
    {
    }
}
