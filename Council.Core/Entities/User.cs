using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Council.Core.Entities
{
    public class User : BaseEntity
    {
        

        [Display(Name = "فعال؟")]
        public bool IsActive { get; set; }

        [Display(Name = "نوع کاربری")]
        public bool IsCouncilMember { get; set; }

        public bool IsSiteManager { get; set; }

        public bool IsWriter1 { get; set; }

        public bool IsWriter2 { get; set; }
        public bool IsCouncilGuest { get; set; }
        public bool IsOtherCouncilMember { get; set; }
        public bool IsManager { get; set; }
        public bool BossHelper { get; set; }
        public bool IsCouncilBoss { get; set; }

        //public bool  IsOnline { get; set; }

        [Display(Name = "نام *")]
        [Required(ErrorMessage = "{0}  الزامی میباشد<br />")]
        [MaxLength(30)]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی *")]
        [Required(ErrorMessage = "{0} الزامی میباشد<br />")]
        [MaxLength(40)]
        public string LastName { get; set; }

        [Display(Name = "کد ملی *")]
        [Required(ErrorMessage = "{0}  الزامی میباشد<br />")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "{0}  معتبر نمیباشد<br />")]
        [MaxLength(10)]
        public string NationalCode { get; set; }

        [Display(Name = "جنسیت")]
        public bool Gender { get; set; }

        [Display(Name = "تلفن")]             
        [MaxLength(20)]
        public string Tell { get; set; }
        [Display(Name = "موبایل *")]
        [Required(ErrorMessage = "{0}  الزامی میباشد<br />")]
        [MaxLength(20)]
        [RegularExpression(@"^(\+98|0)?9\d{9}$", ErrorMessage = "{0}  معتبر نمیباشد<br />")]
        public string Mobile { get; set; }
        [Display(Name = "آدرس ایمیل *")]
        [Required(ErrorMessage = "{0}  الزامی میباشد<br />")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "{0}  معتبر نمیباشد<br />")]
        public string Email { get; set; }

        [Display(Name = "امضاء")]
        public string Signature { get; set; }

        [Required]
        [MaxLength(256)]
        public string UserName { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public virtual ICollection<LetterRefrences> LetterRefrences { get; set; }
        public virtual ICollection<Commission> Commissions { get; set; }

        //public User User { get; set; }

        //public string CommissionID { get; set; }
        public virtual ICollection<Voting> Votings { get; set; }
        public virtual ICollection<Draft> Drafts { get; set; }
        public virtual ICollection<SMSDeliversLog> SmsDeliversLog { get; set; }
        
        //public string ApplicationUserID { get; set; }
        //public virtual ICollection<Meeting> Meetings { get; set; }

        public virtual ICollection<UserRole> UserRole { get; set; }

        public bool CanSignatureForLetter { get; set; }
    }
}
