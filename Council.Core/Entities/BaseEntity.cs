using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Council.Core.Entities
{
    public class BaseEntity
    {
        public BaseEntity()
        {
            ID = Guid.NewGuid().ToString().Replace("-", "");
            CreatedOn = DateTime.Now;
        }
        public string ID { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool Deleted { get; set; }
    }
}
