using Council.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Core.Models
{
    public class PrintModel
    {
        public List<Voting> Votings { get; set; }
        public MeetingResult MeetingResult { get; set; }
        public bool IsCommission { get; set; }
        public CommissionInfo CommissionInfo { get; set; }
    }
    public class InvitePrintModel
    {
        public List<MemberGender> Members { get; set; }
        public string SessionDate { get; set; }
        public string SesionFromTime { get; set; }
        public string SessionToTime { get; set; }
        public List<MeetingTitleItems> Meeting { get; set; }

        public User Writer1 { get; set; }
    }
    public class MeetingTitleItems
    {
        public string Title { get; set; }
        public string CommisionName { get; set; }
        public bool IsCommision { get; set; }
    }
    public class MemberGender
    {
        public string  Name { get; set; }
        public bool IsFemale { get; set; }
    }
}
