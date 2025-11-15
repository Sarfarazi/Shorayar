using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Core.Entities
{
    public class Organ:BaseEntity
    {
        [Display(Name = "نام سازمان/شخص")]
        [Required(ErrorMessage = "{0}  الزامی میباشد")]
        [MaxLength(200)]
        public string  Name { get; set; }

        [Display(Name = "کد ")]
       // [Required(ErrorMessage = "{0}  الزامی میباشد")]
        [MaxLength(20)]
        public string Code { get; set; }

        [Display(Name = "ایمیل ")]
       // [Required(ErrorMessage = "{0}  الزامی میباشد")]
        [EmailAddress(ErrorMessage = "آدرس ایمیل صحیح نمی باشد")]
        //[DataType(DataType.EmailAddress,ErrorMessage ="آدرس ایمیل صحیح نمی باشد")]
        //[RegularExpression(@"^[\w-]+(\.[\w-]+)*@([a-z0-9-]+(\.[a-z0-9-]+)*?\.[a-z]{2,6}|(\d{1,3}\.){3}\d{1,3})(:\d{4})?$", ErrorMessage = "آدرس ایمیل صحیح نمی باشد")]
        public string Email { get; set; }

        [Display(Name = "آدرس سازمان")]       
        public string Address { get; set; }

        public virtual ICollection<OutLetter>  OutLetters { get; set; }

    }
}
