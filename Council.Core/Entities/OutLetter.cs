using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Council.Core.Entities
{
    public class OutLetter : BaseEntity
    {
        public bool Received { get; set; }

        [Display(Name = "موضوع *")]
        [Required(ErrorMessage = "*")]
        public string Subject { get; set; }

        [Display(Name = "شماره ثبت در دبیرخانه ")]
      //  [Required(ErrorMessage = "{0}  الزامی میباشد")]
        public string OutLetterNumber { get; set; }



        [Display(Name = "شماره نامه داخلی")]
        public string Number { get; set; }

        [Display(Name = "تایید ارسال نامه")]
        public bool ConfirmBoss { get; set; }

        [Display(Name = "شماره نامه وارده")]
        public string RegisterNumber { get; set; }

        [Display(Name = "تاریخ نامه وارده")]
        //[Required(ErrorMessage = "{0}  الزامی میباشد")]
        public string OutLetterDate { get; set; }

        [Display(Name = "تاریخ ثبت در دبیرخانه")]
       // [Required(ErrorMessage = "{0}  الزامی میباشد")]
        public string SendDate { get; set; }

        [Display(Name = "گیرنده نامه")]
        public string Reciver { get; set; }

        [Display(Name = "ارسال کننده")]
        public string Sender { get; set; }

        [Display(Name = "تحویل گیرنده")]
        public string Transferee { get; set; }

        [Display(Name = "تحویل دهنده")]
        public string Bringer { get; set; }

        [Display(Name = "شماره قبلی")]
        public string BeforeNumber { get; set; }

        [Display(Name = "شماره بعدی")]
        public string NextNumber { get; set; }

        [Display(Name = "نامه های مرتبط")]
        public string LinkedLetters { get; set; }

        [Display(Name = "ارجاعات")]
        public string OrginalLetter { get; set; }

        [Display(Name = "رونوشت")]
        public string CopyTo { get; set; }

        [Display(Name = "شماره زونکن بایگانی")]
        public string BoxNumber { get; set; }

        [Display(Name = "کد بایگانی")]
        public string ArchiveCode { get; set; }

        [AllowHtml]
        [Display(Name = "توضیحات / متن نامه")]
        [DataType(DataType.MultilineText)]
        public string Comment { get; set; }

        [Display(Name = "فایل ضمیمه")]
        public string Files { get; set; }
        public bool Archived { get; set; }
        public bool Removed { get; set; }
        public string LetterID { get; set; }
        public byte OutLetterStatus { get; set; }
        public virtual ICollection<Upload> Uploads { get; set; }
        public virtual ICollection<Note> Notes { get; set; }
        public virtual CouncilPeriod CouncilPeriod { get; set; }
        [Display(Name ="سازمان")]
        
        public virtual Organ Organ { get; set; }
        public string SignatureUserID { get; set; }
        [Display(Name = "دلیل عدم تایید")]
        public string RejectReason { get; set; }

        


    }
}
