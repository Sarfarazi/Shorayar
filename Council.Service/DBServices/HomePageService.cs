using Council.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Council.Core.Enums;
using Council.Core.Models;
using Council.Data;

namespace Council.Service.DBServices
{
    public class HomePageService
    {
        PublicMetods publicMethods;
        LetterServices letterService;
        CommissionServices commissionService;
        UserServices userService;
        string _userId;
        List<CommissionAndBossIds> _myCommissionIds;
        DataBase database;
        public HomePageService(string userId)
        {            
            publicMethods = new PublicMetods();
            _userId = userId;
            letterService = new LetterServices();
            commissionService = new CommissionServices();
            userService = new UserServices();
            database = new DataBase();
        }

        public HomePageViewModel GetModel()
        {
            var model = new HomePageViewModel()
            {
                MyLetters = GetMyLetters().ToList(),
                UserIsBoss = userService.UserIsBoss(_userId),
                UserIsBossHelper = userService.UserIsBossHelper(_userId),
                UserIsMember = userService.UserIsMember(_userId),
                UserIsSiteManager = userService.UserIsSiteManager(_userId),
                //Finished = letterService.Where(l => l.LetterStatus == LetterStatus.End).OrderByDescending(l => l.CreatedOn).Take(10).ToList(),
                Finished = null,
                AllCommissions = AllCommissionIds(),
                MyCommissions = MyCommissionIds(),
                MyReadyVotingIds = letterService.MyLetters(_userId)
                                   .Where(k => k.CouncilPeriod.IsActive)                                   
                                   .Where(l => (!l.Votings.Any(v => v.User.ID == _userId && v.Deleted == false && !v.IsCommission)) &&
                                       (l.LetterStatus == LetterStatus.AllowVotting || l.LetterStatus == LetterStatus.CommissionAllowVotting))
                                    .Where(m => m.Meeting.MeetingUsers.Any(t => t.User.ID == _userId && t.status == 1))
                                   .Select(lr => lr.ID).ToList(),
                CommisionLettersType = letterService.MyLetters(_userId)
                            .Where(k => k.CouncilPeriod.IsActive)
                            .Where(l => (l.Votings.Any(v => v.User.ID == _userId && v.Deleted == false && v.IsCommission)) &&
                                       (l.LetterStatus == LetterStatus.AllowVotting || l.LetterStatus == LetterStatus.CommissionAllowVotting))
                                   .Select(lr => lr.ID).ToList(),
                MyReadyCommisionVotingIds = letterService.MyLetters(_userId)
                                   .Where(k => k.CouncilPeriod.IsActive)
                                   .Where(l => 
                                        (!l.Votings.Any(v => v.User.ID == _userId && v.Deleted == false && v.IsCommission)) &&
                                        (l.LetterStatus == LetterStatus.AllowVotting || l.LetterStatus == LetterStatus.CommissionAllowVotting)
                                    )
                                    //.Where(m => m.Meeting.MeetingUsers.Any(t => t.User.ID == _userId && t.status == 1))
                                   .Select(lr => lr.ID ).ToList()
            };
            return model;
        }
        public SearchLetterModel GetSearchModel(string txt="")
        {

            var model = new SearchLetterModel()
            {
                MyLetters = GetMyLetters().Where(m=>m.Title.Contains(txt)).OrderByDescending(l => l.CreatedOn).ToList(),
                MyCommissions = MyCommissionIds(),
                UserIsBoss = userService.UserIsBoss(_userId),
                UserIsBossHelper = userService.UserIsBossHelper(_userId),
                UserIsMember = userService.UserIsMember(_userId),
                UserIsSiteManager = userService.UserIsSiteManager(_userId),
                MyReadyVotingIds = letterService.MyLetters(_userId)
                                   .Where(l => (!l.Votings.Any(v => v.User.ID == _userId && v.Deleted == false)) && (l.LetterStatus == LetterStatus.AllowVotting || l.LetterStatus == LetterStatus.CommissionAllowVotting))
                                   .Where(m=>m.Title.Contains(txt)).OrderByDescending(l=>l.CreatedOn).ToList()
                                   .Select(lr => lr.ID).ToList()
            };
            return model;
        }
        public int GetComisionCount()
        {
            return GetMyLetters().Where(l => l.LetterStatus == LetterStatus.CommissionEnd).ToList().Count();           
        }
        public int GetMyLetterCount()
        {
           return letterService.Where(k => k.CouncilPeriod.IsActive).Where(l => l.LetterStatus == LetterStatus.End).ToList().Count();            
        }
        public LastOpinionModel GetLastOpinionModel(string skip, string take,string searchText="")
        {                        
            var model = new LastOpinionModel()
            {
                UserIsBoss = userService.UserIsBoss(_userId),
                UserIsBossHelper= userService.UserIsBossHelper(_userId),
                //Finished = take == "-1"? letterService.Where(j=>j.CouncilPeriod.IsActive).Where(m=>m.LetterRefrences.Any(p=>p.Recivers.Any(n=>n.ID== _userId))).Where(l => l.LetterStatus == LetterStatus.End).OrderByDescending(l => l.CreatedOn).Skip(Convert.ToInt32(skip)).ToList():                                                       
                //                         letterService.Where(j => j.CouncilPeriod.IsActive).Where(m => m.LetterRefrences.Any(p => p.Recivers.Any(n => n.ID == _userId))).Where(l => l.LetterStatus == LetterStatus.End).OrderByDescending(l => l.CreatedOn).Skip(Convert.ToInt32(skip)).Take(Convert.ToInt32(take)).ToList(),
                Finished = take == "-1" ? letterService.Where(j => j.CouncilPeriod.IsActive).Where(l => l.LetterStatus == LetterStatus.End).OrderByDescending(l => l.CreatedOn).Skip(Convert.ToInt32(skip)).ToList() :
                                         letterService.Where(j => j.CouncilPeriod.IsActive).Where(l => l.LetterStatus == LetterStatus.End).OrderByDescending(l => l.CreatedOn).Skip(Convert.ToInt32(skip)).Take(Convert.ToInt32(take)).ToList(),
                MyCommissions = MyCommissionIds()
            };
            return model;
        }
        public LastCommisionModel GetLastCommisionModel(string skip, string take,string searchText="")
        {
            var model = new LastCommisionModel()
            {
                UserIsBossHelper = userService.UserIsBossHelper(_userId),
                UserIsBoss = userService.UserIsBoss(_userId),
                MyLetters =take=="-1"? GetMyLetters().Where(l => l.LetterStatus == LetterStatus.CommissionEnd).OrderByDescending(l => l.CreatedOn).Skip(Convert.ToInt32(skip)).ToList():                            
                                       GetMyLetters().Where(l => l.LetterStatus == LetterStatus.CommissionEnd).OrderByDescending(l => l.CreatedOn).Skip(Convert.ToInt32(skip)).Take(Convert.ToInt32(take)).ToList(),
                MyCommissions = MyCommissionIds()
            };
            return model;
        }
        public LastOpinionModel SearchLastOpinionModel(string txt)
        {
            var model = new LastOpinionModel()
            {
                UserIsBoss = userService.UserIsBoss(_userId),
                UserIsBossHelper = userService.UserIsBossHelper(_userId),
                Finished = letterService.Where(k => k.CouncilPeriod.IsActive).Where(l => l.LetterStatus == LetterStatus.End && l.Title.Contains(txt)).OrderByDescending(l => l.CreatedOn).ToList() ,                                   
                MyCommissions = MyCommissionIds()
            };
            return model;
        }
        public LastCommisionModel SearchLastCommisionModel(string txt)
        {
            var model = new LastCommisionModel()
            {
                UserIsBoss = userService.UserIsBoss(_userId),
                UserIsBossHelper = userService.UserIsBossHelper(_userId),
                MyLetters = GetMyLetters().Where(l => l.LetterStatus == LetterStatus.CommissionEnd && l.Title.Contains(txt)).OrderByDescending(l => l.CreatedOn).ToList(),
                MyCommissions = MyCommissionIds()
            };
            return model;
        }
        public List<IndexLetter> GetMyLetters()
        {
            Dictionary<string, object> _params = new Dictionary<string, object>();
            _params.Add("@UserID", _userId);
            string query = publicMethods.CreateSQLQueryForSP("sp_MyLetters", _params);
            //string query = publicMethods.CreateSQLQueryForSP("sp_MyLetters", _params);
            return database.SelectFromStoreProcedure<IndexLetter>(query).ToList();
        }
        public List<IndexLetter> GetMyOutLetters()
        {
            Dictionary<string, object> _params = new Dictionary<string, object>();
            _params.Add("@userId", _userId);
            _params.Add("@Skip", "1");
            _params.Add("@Take", "20");
            string query = publicMethods.CreateSQLQueryForSP("sp_GetOutLetters", _params);          
            return database.SelectFromStoreProcedure<IndexLetter>(query).ToList();
        }
        public IQueryable<Letter> MyLetter()
        {
            return letterService.Where(k => k.CouncilPeriod.IsActive).Where(m=>!m.Deleted).Where(l => l.LetterRefrences.Any(r => r.SenderAppID == _userId || r.Recivers.Any(u => u.ID == _userId)));
        }
        //public IQueryable<Letter> MyCommissionLetter()
        //{
        //    var myCommissionIds = MyCommissionIds();
        //    return letterService.Where(l => l.LetterRefrences.OrderBy(lr => lr.CreatedOn).Last().RefrenceType == RefrenceType.SendToCommissionMembers &&
        //                myCommissionIds.Contains(l.LetterRefrences.OrderBy(lr => lr.CreatedOn)
        //                    .Last(lr => !String.IsNullOrEmpty(lr.CommissionId)).CommissionId));
        //}
        public IQueryable<Letter> CommissionCouncilLetter()
        {
            return letterService.Where(l => l.LetterStatus == LetterStatus.CommissionVoting);
        }
        List<CommissionAndBossIds> MyCommissionIds()
        {
            if (_myCommissionIds == null)
                _myCommissionIds = commissionService.Where(c => c.CommissionChairman.ID == _userId
                              || c.Members.Any(m => m.ID == _userId))
                                .Select(c => new CommissionAndBossIds { BossId = c.CommissionChairman.ID, CommissionId = c.ID }).ToList();

            return _myCommissionIds;
        }
        List<string> MyLetterVotedBefor(string userId)
        {
            List<string> myLetter = new List<string>();

            return myLetter;
        }
        List<CommissionAndBossIds> AllCommissionIds()
        {
            return commissionService.All()
                   .Select(c => new CommissionAndBossIds { BossId = c.CommissionChairman.ID, CommissionId = c.ID }).ToList();

        }

    }
}
