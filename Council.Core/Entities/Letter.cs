using Council.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Council.Core.Entities
{
    public class Letter : BaseEntity
    {
        [Display(Name = "شماره نامه")]
       // [Required(ErrorMessage = "شماره نامه الزامی میباشد<br />")]
        [MaxLength(20)]
        public string LetterNumber { get; set; }

        [Display(Name = "موضوع")]
        [Required(ErrorMessage = "موضوع الزامی میباشد<br />")]
        [MaxLength(200)]
        public string Title { get; set; }

        [AllowHtml]
        [Display(Name = "متن")]        
        public string Content { get; set; }

        [Display(Name = "تاریخ")]
        [Required(ErrorMessage = "تاریخ الزامی میباشد<br />")]
        [MaxLength(20)]
        public string CreateOn { get; set; }

        [Display(Name = "اضطراری")]
        public bool Urgency { get; set; }

        [Display(Name = "فایل ضمیمه")]
        public string Files { get; set; }
        public bool? IsRule { get; set; }
       
        public LetterStatus LetterStatus { get; set; }
        public virtual ICollection<LetterRefrences> LetterRefrences { get; set; }
        public virtual ICollection<Voting> Votings { get; set; }
        public virtual OutLetter OutLetter { get; set; }
        public virtual Meeting Meeting { get; set; }
        public virtual Meeting CommissionMeeting { get; set; }
        public virtual ICollection<Upload> Uploads { get; set; }
        public virtual ICollection<Note> Notes { get; set; }
        public virtual CouncilPeriod CouncilPeriod { get; set; }
    }
}
