using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Core.Entities
{
    public class DefaultStatement : BaseEntity
    {
        [Display(Name = "جمله")]
        [Required(ErrorMessage = "{0} الزامی میباشد<br />")]
        public string Statement { get; set; }
    }
}
