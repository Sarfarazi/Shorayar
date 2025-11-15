using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Core.Entities
{
    public  class CopyToLetter: BaseEntity
    {

        public string OrganID { get; set; }
        public virtual Organ Organ { get; set; }
        public string OutLetterID { get; set; }
        // public string name { get; set; }
        public string Description { get; set; }
        public int TypeCopy { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RowOrder { get; set; }

    }
}
