using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Core.Entities
{
    public class Note:BaseEntity
    {
        public string ContentText { get; set; }
        public string  WriterId { get; set; }
        
    }
}
