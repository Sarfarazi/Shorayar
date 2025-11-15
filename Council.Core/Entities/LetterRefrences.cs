using Council.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Council.Core.Entities
{
    public class LetterRefrences : BaseEntity
    {
        public string SenderAppID { get; set; }
        public bool SenderArchived { get; set; }
        public bool SenderRemoved { get; set; }

        [Display(Name = "رونوشت")]
        public string Transcript { get; set; }
        public DateTime CreateOn { get; set; }
        public string CommissionId { get; set; }
        public RefrenceType RefrenceType { get; set; }
        public virtual ICollection<LetterStatuses> LetterStatuses { get; set; }
        public virtual ICollection<User> Recivers { get; set; }

    }
}
