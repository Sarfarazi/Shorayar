using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Council.Core.Entities
{
    public class LetterStatuses : BaseEntity
    {
        public string UserID { get; set; }
        public bool IsNew { get; set; }
        public DateTime  ReadTime { get; set; }
        public bool LetterStatusForUser { get; set; }
        public virtual LetterRefrences LetterRefrence { get; set; }
        public bool Archived { get; set; }
        public bool Removed { get; set; }
        
    }
}
