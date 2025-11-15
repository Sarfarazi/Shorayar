using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Core.Entities
{
    public class MeetingUsers : BaseEntity
    {
        public virtual User User { get; set; }
        public int status { get; set; }
        public string Comment { get; set; }
    }
}
