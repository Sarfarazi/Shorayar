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
    public class MeetingHeader:BaseEntity
    {
        public MeetingHeader()
        {
            FinalApproved = false;
        }

        [Display(Name = "شماره جلسه")]
        //[Required(ErrorMessage ="وارد کردن شماره جلسه اجباری است")]
        public int MeetingNumber { get; set; }

        [Display(Name = "ساعت شروع جلسه")]
        [RegularExpression("^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$",ErrorMessage ="فرمت ساعت باید بصورت 00:00 باشد")]
        [Required(ErrorMessage = "وارد کردن ساعت شروع جلسه اجباری است")]
        public string StartTime { get; set; }

        [Display(Name = "ساعت خاتمه جلسه")]
        [Required(ErrorMessage = "وارد کردن ساعت خاتمه جلسه اجباری است")]
        [RegularExpression("^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "فرمت ساعت باید بصورت 00:00 باشد")]
        public string EndTime { get; set; }


        [Display(Name = "متن مذاکرات"), AllowHtml]        
        public string Content { get; set; }

        [Display(Name = "تاریخ ثبت")]
        [Required(ErrorMessage = "{0}  الزامی میباشد")]
        [MaxLength(10)]
        public string RegisterDate { get; set; }

        [Display(Name = "نوع جلسه")]
        public byte MeetingType { get; set; }


        public SessionStatus SessionStatus { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<Meeting> Meetings { get; set; }

        public bool FinalApproved { get; set; }

        public string CouncilPeriodsID { get; set; }

        public string Code { get; set; }


    }
}
