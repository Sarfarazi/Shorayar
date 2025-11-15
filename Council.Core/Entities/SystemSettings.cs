using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Core.Entities
{
    public class SystemSettings : BaseEntity
    {
        [Required]
        public string CouncilName { get; set; }

        [Required]
        public string CouncilLogo { get; set; }

        [Required]
        public string CouncilLoginLogo { get; set; }

        public bool Used { get; set; }
    }
}
