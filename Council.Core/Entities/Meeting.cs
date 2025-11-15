using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Council.Core.Entities
{
    public class Meeting : BaseEntity
    {
        [Display(Name = "شماره جلسه")]
        [Required(ErrorMessage = "شماره الزامی میباشد<br />")]
        public string MeetingNumber { get; set; }

        [Display(Name = "آرا مخفی باشد؟")]
        [Required(ErrorMessage = "شماره الزامی میباشد<br />")]
        public bool HiddenVoting { get; set; }

        [Display(Name = "موضوع جلسه")]
        public string Subject { get; set; }

        
        [Display(Name = "شرح توضیحات جلسه"), AllowHtml]
        //[Required(ErrorMessage = "متن الزامی میباشد<br />")]
        public string Content { get; set; }

        public bool NoCouncil { get; set; }

        [Display(Name = "افراد میهمان"), AllowHtml]
        [DataType(DataType.MultilineText)]
        public string Peoples { get; set; }

        public virtual ICollection<MeetingUsers> MeetingUsers { get; set; }

        public virtual MeetingHeader MeetingHeader { get; set; }

        public string CommissionId { get; set; }
    }
}
