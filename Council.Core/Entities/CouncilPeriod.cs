using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Core.Entities
{
    public class CouncilPeriod: BaseEntity
    {
        [DisplayName("نام دوره")]
        [Display(Name = "نام دوره")]
        [Required(ErrorMessage = "{0}  الزامی میباشد<br />")]
        [MaxLength(30)]
        public string Name { get; set; }

        [DisplayName("کد دوره")]
        [Display(Name = "کد دوره")]
       // [Required(ErrorMessage = "{0}  الزامی میباشد<br />")]
        [MaxLength(30)]
        public string Code { get; set; }


        [Display(Name = "وضعیت فعال بودن دوره")]
        public bool IsActive { get; set; }
        
        [Display(Name = "توضیحات")]
        [DisplayName("توضیحات")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public virtual ICollection<Letter> Letters { get; set; }
        public virtual ICollection<OutLetter> OutLetters { get; set; }
    }

    

}
