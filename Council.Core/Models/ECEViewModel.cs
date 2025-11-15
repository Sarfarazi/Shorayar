using ECE;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Council.Core;

namespace Council.Core.Models
{
    public class LetterInfo
    {
        public Protocol Protocol { get; set; }
        public Software Software { get; set; }
        public Sender Sender { get; set; }
        public Receiver Receiver { get; set; }
        public List<OtherReceiver> OtherReceivers { get; set; } = new List<OtherReceiver>();
        public string LetterNo { get; set; }
        public LetterDateTime LetterDateTime { get; set; }
        public List<RelatedLetter> RelatedLetters { get; set; } = new List<RelatedLetter>();
        public string Subject { get; set; }
        public Priority Priority { get; set; }
        public Classification Classification { get; set; }
        public List<Keyword> Keywords { get; set; } = new List<Keyword>();
        public List<Origin> Origins { get; set; } = new List<Origin>();
        public List<Attachment> Attachments { get; set; } = new List<Attachment>();
    }

    public class SendECE
    {
        //اطلاعات فرستنده
        public Sender Sender { get; set; }
        //اطلاعات گیرنده
        public Receiver Receiver { get; set; }
        //سایر گیرندگان
        public List<OtherReceiver> OtherReceivers { get; set; } = new List<OtherReceiver>();
        public string LetterNo { get; set; }
        public LetterDateTime LetterDateTime { get; set; }
        public List<RelatedLetter> RelatedLetters { get; set; } = new List<RelatedLetter>();
        public string Subject { get; set; }
        public Priority Priority { get; set; }
        public Classification Classification { get; set; }
        public List<Keyword> Keywords { get; set; } = new List<Keyword>();

        [Required(ErrorMessage = "Please select file.")]
        [Display(Name = "Browse File")]
        public HttpPostedFileBase[] Origins { get; set; }

        [Required(ErrorMessage = "Please select file.")]
        [Display(Name = "Browse File")]
        public HttpPostedFileBase[] Attachments { get; set; }
    }
}
