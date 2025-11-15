using Council.Core.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Council.Core.Models
{
    public class CreateSettings
    {
        [Display(Name ="نام شورا *")]
        [Required(ErrorMessage ="نام شورا را وارد نمایید")]
        public string CouncilName { get; set; }

        [Display(Name = "لوگو *")]
        [Required(ErrorMessage = "لوگو را انتخاب نمایید")]
        [MaximumFileSizeValidator(0.5)]
        [ValidFileTypeValidator("jpg", "jpeg", "png")]
        public HttpPostedFileBase CouncilLogo { get; set; }

        [Display(Name = "لوگو صفحه ورود *")]
        [Required(ErrorMessage = "لوگو صفحه ورود را انتخاب نمایید")]
        [MaximumFileSizeValidator(0.5)]
        [ValidFileTypeValidator("jpg", "jpeg", "png")]
        public HttpPostedFileBase CouncilLoginLogo { get; set; }

        [Display(Name ="استفاده به عنوان تنظیمات پیش فرض")]
        public bool Used { get; set; }
    }

    public class UpdateSettings
    {
        public string ID { get; set; }

        [Display(Name = "نام شورا *")]
        [Required(ErrorMessage = "نام شورا را وارد نمایید")]
        public string CouncilName { get; set; }

        [Display(Name = "لوگو *")]
        [MaximumFileSizeValidator(0.5)]
        [ValidFileTypeValidator("jpg", "jpeg", "png")]
        public HttpPostedFileBase CouncilLogo { get; set; }

        [Display(Name = "لوگو صفحه ورود *")]
        [MaximumFileSizeValidator(0.5)]
        [ValidFileTypeValidator("jpg", "jpeg", "png")]
        public HttpPostedFileBase CouncilLoginLogo { get; set; }

        public string LogoName { get; set; }

        [Display(Name = "استفاده به عنوان تنظیمات پیش فرض")]
        public bool Used { get; set; }
    }

    public class CurrentSetting
    {
        public string CouncilName { get; set; }
        public string CouncilLogo { get; set; }
        public string CouncilLoginLogo { get; set; }
    }

}
