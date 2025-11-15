using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Core.Models
{
    public class RuleModel
    {
        public string  LetterId { get; set; }
        public string Title { get; set; }
        public string  LetterNumber { get; set; }
        public string PeriodId { get; set; }
        public string CreateOn { get; set; }
        public string MeetingId { get; set; }
        public Int16 IsCommision { get; set; }

    }
}
