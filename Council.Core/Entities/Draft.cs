using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Council.Core.Entities
{
    public class Draft : BaseEntity
    {
        [Display(Name = "موضوع")]
        [Required(ErrorMessage = "موضوع الزامی میباشد")]
        [MaxLength(200)]
        public string Title { get; set; }

        [AllowHtml]
        [Display(Name = "متن")]
               
        public string Content { get; set; }
        public string OwnerId { get; set; }
        //  public virtual User Creator { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
