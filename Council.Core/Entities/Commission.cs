using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Core.Entities
{
    public class Commission : BaseEntity
    {
        [Display(Name = "نام کمیسیون")]
        [Required(ErrorMessage = "{0}  الزامی میباشد<br />")]
        [MaxLength(255)]
        public string Name { get; set; }

        [Display(Name = "توضیحات")]
        [DataType(DataType.MultilineText)]
        public string Tittle { get; set; }

        //[Required]
        public virtual User CommissionChairman { get; set; }
        public virtual ICollection<User> Members { get; set; }
    }
}
