using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Core.Entities
{
    public class Voting : BaseEntity
    {
        public string VoteStatus { get; set; }
        public string Comment { get; set; }
        public virtual User User { get; set; }
        public virtual Letter Letter { get; set; }
        public bool IsCommission { get; set; }
    }
}
