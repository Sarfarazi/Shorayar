using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Council.Core.Entities;
using Council.Core.Interfaces;

namespace Council.Service.DBServices
{
    public class VotingService : CRUDServices<Voting>, IVoting
    {
        public bool UserIsVoted(string userId, string letterId, bool isCommission = false)
        {
            return All().Any(v => v.User.ID == userId && v.Letter.ID == letterId && v.Deleted == false && v.IsCommission == isCommission);
        }

        public List<Voting> GetVotingOfLetter(string letterID, bool isCommission = false)
        {
            return Where(v => v.Letter.ID == letterID && v.IsCommission == isCommission).Where(m=>!m.Deleted).ToList();
        }        
    }
}
