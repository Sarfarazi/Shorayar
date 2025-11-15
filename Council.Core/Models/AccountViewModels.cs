//using Council.Core.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Core.Entities
{
    class AccountViewModels
    {
    }
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }
    }

    public class ManageUserViewModel
    {
        [Display(Name = "پسورد جاری")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "{0} الزامی میباشد")]
        public string OldPassword { get; set; }


        [Display(Name = "پسورد جدید")]
        [Required(ErrorMessage = "{0} الزامی میباشد")]
        [StringLength(100, ErrorMessage = "باید حد اقل دارای 6 کاراکتر باشد", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Display(Name = "تکرار پسورد")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "{0} الزامی میباشد")]
        [Compare("NewPassword", ErrorMessage = "خطا در ورود اطلاعات")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Display(Name = "نام کاربری")]
        [Required(ErrorMessage = "{0} را وارد کنید")]
        public string UserName { get; set; }

        [Display(Name = "رمز عبور")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "{0} را وارد کنید")]
        public string Password { get; set; }

        [Display(Name = "مرا به خاطر بسپار")]
        public bool RememberMe { get; set; }

        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        //[Display(Name = "حاصل جمع")]
        //public string Captcha { get; set; }
    }

    public class RegisterViewModel
    {
        [Display(Name = "فعال")]
        public bool IsActive { get; set; }

        [Display(Name = "نام *")]
        [Required(ErrorMessage = "{0}  الزامی میباشد")]
        [MaxLength(30)]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی *")]
        [Required(ErrorMessage = "{0} الزامی میباشد")]
        [MaxLength(40)]
        public string LastName { get; set; }

        [Display(Name = "کد ملی *")]
        [Required(ErrorMessage = "{0}  الزامی میباشد")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "{0}  معتبر نمیباشد")]
        [MaxLength(10)]
        public string NationalCode { get; set; }

        [Display(Name = "جنسیت")]
        public bool Gender { get; set; }

        [Display(Name = "تلفن")]
        [MaxLength(20)]
        public string Tell { get; set; }

        [Display(Name = "موبایل *")]
        [Required(ErrorMessage = "{0}  الزامی میباشد")]
        [MaxLength(20)]
        [RegularExpression(@"^(\+98|0)?9\d{9}$", ErrorMessage = "{0} معتبر نمیباشد")]
        public string Mobile { get; set; }

        [Display(Name = "آدرس ایمیل *")]
        [Required(ErrorMessage = "{0}  الزامی میباشد<br />")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "{0}  معتبر نمیباشد")]
        public string Email { get; set; }

        [Display(Name = "امضاء")]
        public string Signature { get; set; }

        [Required(ErrorMessage = "نام کاربری را وارد کنید")]
        [Display(Name = "نام کاربری *")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "رمز عبور را وارد کنید")]
        [StringLength(6, ErrorMessage = "{0} باید حد اقل دارای {1} کاراکتر باشد", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور *")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تکرار رمز عبور *")]
        [Compare("Password", ErrorMessage = "رمز عبور و تکرار رمز عبر یکسان نیستند")]
        public string ConfirmPassword { get; set; }
       
        [Display(Name = "نوع کاربری")]
        public string SelectedType { get; set; }
        public IEnumerable<System.Web.Mvc.SelectListItem> UserType { get; set; }

        [Required(ErrorMessage ="نوع کاربری را مشخص کنید")]
        public string SelectedTypes { get; set; }
        public string[] Types
        {
            get
            {
                return SelectedTypes.Split(',');
            }
        }
    }
    public class ChangePasswordViewModel
    {
        public string ID { get; set; }

        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "رمز عبور الزامی می باشد")]
        [StringLength(100, ErrorMessage = "باید حد اقل دارای 6 کاراکتر باشد", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور")]
        public string Password { get; set; }

        [Required(ErrorMessage = "تکرار رمز عبور الزامی می باشد")]
        [DataType(DataType.Password)]
        [Display(Name = "تکرار رمز عبور")]
        [Compare("Password", ErrorMessage = "رمز عبور با تکرار رمز عبور یکسان نیست")]

        public string ConfirmPassword { get; set; }
    }

    public class EditUser
    {
        public string ID { get; set; }

        [Display(Name = "نام *")]
        [Required(ErrorMessage = "{0}  الزامی میباشد")]
        [MaxLength(30)]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی *")]
        [Required(ErrorMessage = "{0} الزامی میباشد")]
        [MaxLength(40)]
        public string LastName { get; set; }

        [Display(Name = "کد ملی *")]
        [Required(ErrorMessage = "{0}  الزامی میباشد")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "{0}  معتبر نمیباشد")]
        [MaxLength(10)]
        public string NationalCode { get; set; }

        [Display(Name = "جنسیت")]
        public bool Gender { get; set; }

        [Display(Name = "تلفن")]
        [MaxLength(20)]
        public string Tell { get; set; }

        [Display(Name = "موبایل *")]
        [Required(ErrorMessage = "{0}  الزامی میباشد")]
        [MaxLength(20)]
        [RegularExpression(@"^(\+98|0)?9\d{9}$", ErrorMessage = "{0} معتبر نمیباشد")]
        public string Mobile { get; set; }

        [Display(Name = "آدرس ایمیل *")]
        [Required(ErrorMessage = "{0}  الزامی میباشد<br />")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "{0}  معتبر نمیباشد")]
        public string Email { get; set; }

        [Display(Name = "امضاء")]
        public string Signature { get; set; }

        [Display(Name = "نوع کاربری")]
        public string SelectedType { get; set; }
        public IEnumerable<System.Web.Mvc.SelectListItem> UserType { get; set; }

        [Required(ErrorMessage = "نوع کاربری را مشخص کنید")]
        public string SelectedTypes { get; set; }
        public string[] Types
        {
            get
            {
                return SelectedTypes.Split(',');
            }
        }
    }
}
