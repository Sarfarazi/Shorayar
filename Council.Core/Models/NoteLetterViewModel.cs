using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Core.Models
{
   public class NoteLetterViewModel
    {
        public string ID { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool Deleted { get; set; }
        public string ContentText { get; set; }
        public string WriterId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
