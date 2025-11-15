using Council.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Core.Models
{
    public class HomePageViewModel
    {
        public List<IndexLetter> MyLetters { get; set; }
        public List<string> MyReadyVotingIds { get; set; }
        public List<string> MyReadyCommisionVotingIds { get; set; }
        public bool UserIsSiteManager { get; set; }
        public bool UserIsBoss { get; set; }
        public bool UserIsBossHelper { get; set; }
        public bool UserIsMember { get; set; }
        public List<string>  CommisionLettersType { get; set; }
        public List<Letter> Finished { get; set; }
        public List<CommissionAndBossIds> AllCommissions { get; set; }
        public List<CommissionAndBossIds> MyCommissions { get; set; }
    }

    public class CommissionAndBossIds
    {
        public string CommissionId { get; set; }
        public string BossId { get; set; }
    }
    public class SessionCommissionAndBossIds
    {
        public string CommissionId { get; set; }
        public string BossId { get; set; }
        public string CommisionName { get; set; }
    }
    public class LastOpinionModel
    {
        public bool UserIsBoss { get; set; }
        public bool UserIsBossHelper { get; set; }
        public List<Letter> Finished { get; set; }
        public List<CommissionAndBossIds> MyCommissions { get; set; }
        
    }
    public class LastCommisionModel
    {
        public bool UserIsBoss { get; set; }
        public bool UserIsBossHelper { get; set; }
        public List<IndexLetter> MyLetters { get; set; }
        public List<CommissionAndBossIds> MyCommissions { get; set; }
    }
    public class SearchLetterModel
    {
        public List<IndexLetter> MyLetters { get; set; }
        public List<CommissionAndBossIds> MyCommissions { get; set; }
        public List<string> MyReadyVotingIds { get; set; }
        public bool UserIsSiteManager { get; set; }
        public bool UserIsBoss { get; set; }
        public bool UserIsBossHelper { get; set; }
        public bool UserIsMember { get; set; }
    }
    public class SessionViewModel
    {
        public List<IndexLetter> MyLetters { get; set; }        
        public List<SessionCommissionAndBossIds> MyCommissions { get; set; }
        public bool UserIsSiteManager { get; set; }
        public bool UserIsBoss { get; set; }
        public bool UserIsBossHelper { get; set; }
        public bool UserIsMember { get; set; }


    }
}
