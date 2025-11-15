using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Core.Entities
{
    public class SMSErrorCode:BaseEntity
    {
        public Int64 ErrorCode { get; set; }
        public string ErrorTxt { get; set; }
        public ICollection<SMSDeliversLog> SMSDeliversLog { get; set; }
    }
}
