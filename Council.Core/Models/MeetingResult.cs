using System;
using System.Collections.Generic;
using Council.Core.Entities;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Core.Models
{
    public class MeetingResult
    {

        public string LetterID { get; set; }
        public string OutLetterID { get; set; }
        public string MeetingID { get; set; }

        [Display(Name = "شماره نامه")]
        public string LetterNumber { get; set; }

        [Display(Name = "فرستنده")]
        public string Sender { get; set; }

        [Display(Name = "تاریخ ثبت")]
        public string SendDate { get; set; }

        [Display(Name = "تاریخ نامه")]
        public string OutLetterDate { get; set; }

        [Display(Name = "تاریخ ثبت")]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "شماره ثبت")]
        public string RegisterNumber { get; set; }

        [Display(Name = "تاریخ جلسه")]
        public DateTime MeetingDate { get; set; }

        [Display(Name = "موضوع")]
        public string Subject { get; set; }

        [Display(Name = "موضوع نامه")]
        public string Title { get; set; }

        [Display(Name = "شرح تصمیمات شورا")]
        public string Content { get; set; }

        [Display(Name = "شماره جلسه")]
        public string MeetingNumber { get; set; }
        
    }

    public class FinalMeetingPrint
    {
        public string  SessionNumber { get; set; }
        public string SessionContent { get; set; }
        public string RegisterDate{ get; set; }
        public string StartTime { get; set; }
        public string  EndTime { get; set; }
        public string SessionType { get; set; }      
        public List<SignersUser> Signers { get; set; }
        public List<Meeting> Meetings { get; set; }
        public List<MeetingIdWithLetterId> LettersSubject { get; set; }

    }
    public class SignersUser
    {
        public string Name { get; set; }
        public string Signature { get; set; }
        public int UserPosition { get; set; }
        public string Gender { get; set; }
    }
    public class MeetingIdWithLetterId
    {
        public string LetterId { get; set; }
        public string MeetingId { get; set; }
        public string  LetterTitle { get; set; }
    }
    
}
