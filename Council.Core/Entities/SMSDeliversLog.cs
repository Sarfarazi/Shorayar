using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Core.Entities
{
    public class SMSDeliversLog:BaseEntity
    {    
        public string SendDate { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public virtual User User { get; set; }
        public virtual SMSErrorCode SMSErrorCode { get; set; }
    }
}
