using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Core.Entities
{
    public class SMSToken:BaseEntity
    {
        [Display(Name = "نام کاریر")]
        [Required(ErrorMessage = "نام کاریر الزامی میباشد")]
        [MaxLength(50)]
        public string UserName { get; set; }
        [Display(Name = "رمز عبور")]
        [Required(ErrorMessage = "رمز عبور الزامی میباشد")]
        [MaxLength(50)]
        public string PassWord { get; set; }
        public int ServerType { get; set; }
        public int PortalCode { get; set; }
    }
}
