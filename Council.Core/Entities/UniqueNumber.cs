using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Core.Entities
{
    public class UniqueNumber : BaseEntity
    {
        public string DateNumber { get; set; }
        public string Code { get; set; }
        public string FullNumber { get; set; }
        public int LetterType { get; set; }
    }
}
