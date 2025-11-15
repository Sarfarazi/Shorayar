using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Core.Models
{
    public class DraftViewModel
    {
        public string ID { get; set; }
        [Display(Name = "موضوع")]
        [MaxLength(200)]
        public string Title { get; set; }

        [Display(Name = "متن")]
        public string Content { get; set; }
    }
    public class LowPlansModel
    {
        public string letterId { get; set; }
        public string OutLetterId { get; set; }
        public string Title { get; set; }
        public string MeetingId { get; set; }
        public int TypeLetter { get; set; }
        public string OutLetterNumber { get; set; }
        public DateTime OutLettersCreatedOn { get; set; }
        public string OrganName { get; set; }
        public string OrganId { get; set; }
    }
    
}
