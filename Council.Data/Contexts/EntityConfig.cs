using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Council.Core.Entities;

namespace Council.Data.Contexts
{
    public class LetterConfig : EntityTypeConfiguration<Letter>
    {
        public LetterConfig()
        {
            this.HasRequired(p => p.CouncilPeriod).WithMany(m => m.Letters);
        }
    }
}
