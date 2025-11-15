using Council.Core.Entities;
using Council.Core.Enums;
using Council.Core.Interfaces;
using Council.Core.Models;
using Council.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Council.Service.DBServices
{
    public class LetterServices : CRUDServices<Letter>, ILetter
    {
        DataBase database = new DataBase();
        UserServices userService;// = new UserServices();
        PublicMetods publicMethods = new PublicMetods();
        OutLetterServices outLetterService;
        CommissionServices commissionService = new CommissionServices();
        VotingService votingService = new VotingService();
        MeetingServices meetingService = new MeetingServices();
        PeriodService peroidService = new PeriodService();
        MeetingServices mettingService = new MeetingServices();


        public string GetLetterOfOutLetter(string outLetterID)
        {
            outLetterService = new OutLetterServices();
            var OutLetterSpec = outLetterService.Get<string>(outLetterID);
            var letter = this.All().Where(k => k.CouncilPeriod.IsActive).Where(l => l.OutLetter.ID == OutLetterSpec.ID).FirstOrDefault();
            return letter == null ? null : letter.ID;
        }
        public UserPosition GetUserPosition(string appId)
        {
            var user = userService.All().FirstOrDefault(u => u.ID == appId);
            if (user.IsCouncilBoss || user.BossHelper)
                return UserPosition.CouncilBoss;
            if (user.IsCouncilMember)
                return UserPosition.CouncilMember;
            //ToDo: Return Admin
            return UserPosition.Other;
        }
        public UserPosition GetUserPosition(string UserId, string letterId)
        {
            var user = userService.All().FirstOrDefault(u => u.ID == UserId);
            var letter = this.Get<string>(letterId);
            if (UserIsCurrentCommissionBoss(letter, UserId))
                return UserPosition.CurrentCommissionBoss;
            if (UserIsCurrentCommissionMember(letter, UserId))
                return UserPosition.CurrentCommissionMember;
            if (user.IsCouncilBoss || user.BossHelper)
                return UserPosition.CouncilBoss;
            if (user.IsCouncilMember)
                return UserPosition.CouncilMember;
            //ToDo: Return Admin
            return UserPosition.Other;
        }
        bool UserIsCurrentCommissionBoss(Letter letter, string UserId)
        {
            var lastRefrence = GetLastRefrence(letter);
            if (lastRefrence == null)
            {
                return false;
            }
            if (lastRefrence.Recivers.Count > 0)
            {
                if (lastRefrence.Recivers.FirstOrDefault().ID == UserId && lastRefrence.RefrenceType == RefrenceType.SendToCommission)
                    return true;
            }
          
            return false;
        }
        bool UserIsCurrentCommissionMember(Letter letter, string appId)
        {
            var lastRefrence = GetLastRefrence(letter);
            if (lastRefrence.CommissionId == null) return false;
            var commission = commissionService.Get<string>(lastRefrence.CommissionId);
            return commission.Members.Any(m => m.ID == appId);
        }
        public LetterRefrences GetLastRefrence(Letter letter)
        {
            return letter.LetterRefrences.OrderBy(r => r.CreatedOn).Last();
        }
        public LetterRefrences GetLastRefrence(string letterId)
        {
            var letter = this.Get<string>(letterId);
            return letter.LetterRefrences.OrderBy(r => r.CreatedOn).Last();
        }
        public string FlowOutLetter(Letter letter, string OutLetterSpecID, HttpPostedFileBase file, string appID)
        {

            return "";
        }
        public string AddValToOutLetter(Letter letter, string OutLetterSpecID, HttpPostedFileBase file, string appID, List<HttpPostedFileBase> uploads)
        {
            userService = new UserServices();
            var bossId = userService.All().FirstOrDefault(u => u.IsCouncilBoss || u.BossHelper).ID;
            //OutLetterSpec.Letter = letter;
            if (bossId != null)
            {
                PeriodService periodService = new PeriodService();
                var activePeriod = periodService.All().Where(m => m.IsActive).FirstOrDefault();
                outLetterService = new OutLetterServices();
                var OutLetterSpec = outLetterService.Get<string>(OutLetterSpecID);
                letter.ID = Guid.NewGuid().ToString().Replace("-", "");
                letter.OutLetter = OutLetterSpec;
                letter.CouncilPeriod = activePeriod;

                return AddValueToOutLetter(letter, file, "", appID, uploads, RefrenceType.SendToBoss, bossId);
            }
            return "";
        }
        public string CreateOutLetter(Letter letter, string OutLetterSpecID, HttpPostedFileBase file, string appID, List<HttpPostedFileBase> uploads, string SendType)
        {
            userService = new UserServices();
            var bossId = "";
            if (SendType == "0")
                bossId = userService.All().FirstOrDefault(u => u.IsCouncilBoss).ID;
            else if (SendType == "1") //helperId
                bossId = userService.All().FirstOrDefault(u => u.BossHelper).ID;

            //OutLetterSpec.Letter = letter;
            if (bossId != null)
            {
                PeriodService periodService = new PeriodService();
                var activePeriod = periodService.All().Where(m => m.IsActive).FirstOrDefault();
                outLetterService = new OutLetterServices();
                var OutLetterSpec = outLetterService.Get<string>(OutLetterSpecID);
                letter.ID = Guid.NewGuid().ToString().Replace("-", "");
                letter.OutLetter = OutLetterSpec;
                letter.CouncilPeriod = activePeriod;

                return CreateLetter(letter, file, "", appID, uploads, RefrenceType.SendToBoss, bossId);
            }
            return "";
        }
        public IEnumerable<MyLetter> GetMyLettersCount(string userID, int letterType)
        {
            Dictionary<string, object> _params = new Dictionary<string, object>();
            _params.Add("@userId", userID);
            _params.Add("@LetterType", letterType);
            string query = publicMethods.CreateSQLQueryForSP("sp_GetLettersCount", _params);
            return database.SelectFromStoreProcedure<MyLetter>(query).ToList();
        }
        public IEnumerable<LowPlansModel> GetLawsPlansLetters(int skip, int take)
        {
            if (take == -1)
            {
                string query = publicMethods.CreateSQLQueryForSP("GetAllLaws_Plans");
                return database.SelectFromStoreProcedure<LowPlansModel>(query).ToList();
            }
            else
            {
                Dictionary<string, object> _params = new Dictionary<string, object>();
                _params.Add("@Skip", skip);
                _params.Add("@Take", take);
                string query = publicMethods.CreateSQLQueryForSP("GetSomeLaws_Plans", _params);
                return database.SelectFromStoreProcedure<LowPlansModel>(query).ToList();
            }
        }
        public IEnumerable<MyLetter> GetMyLetters(string userID, int letterType, int skip, int take)
        {
            Dictionary<string, object> _params = new Dictionary<string, object>();
            _params.Add("@userId", userID);
            _params.Add("@LetterType", letterType);
            _params.Add("@Skip", skip);
            _params.Add("@Take", take);
            string query = publicMethods.CreateSQLQueryForSP("sp_GetLetters", _params);
            //sstring query = publicMethods.CreateSQLQueryForSP("sp_GetLetters", _params);
            return database.SelectFromStoreProcedure<MyLetter>(query).ToList();
            //@UserAppID
        }

        public IEnumerable<MyLetter> GetOutLetters(string userID, int skip, int take)
        {
            Dictionary<string, object> _params = new Dictionary<string, object>();
            _params.Add("@userId", userID);
            _params.Add("@Skip", skip);
            _params.Add("@Take", take);
            string query = publicMethods.CreateSQLQueryForSP("sp_GetOutLetters", _params);
            return database.SelectFromStoreProcedure<MyLetter>(query).ToList();
            //@UserAppID
        }

        public IEnumerable<MyLetter> GetCouncilLetters(string userID, int skip, int take)
        {
            Dictionary<string, object> _params = new Dictionary<string, object>();
            _params.Add("@userId", userID);
            _params.Add("@Skip", skip);
            _params.Add("@Take", take);
            string query = publicMethods.CreateSQLQueryForSP("sp_GetCouncilLetters", _params);
            return database.SelectFromStoreProcedure<MyLetter>(query).ToList();
            //@UserAppID
        }

        public IEnumerable<MyLetter> GetOutOfReadyForCouncilLetters(string userID, int skip, int take)
        {
            Dictionary<string, object> _params = new Dictionary<string, object>();
            _params.Add("@userId", userID);
            _params.Add("@Skip", skip);
            _params.Add("@Take", take);
            string query = publicMethods.CreateSQLQueryForSP("sp_GetOutOfReadyForCouncilLetters", _params);
            return database.SelectFromStoreProcedure<MyLetter>(query).ToList();
            //@UserAppID
        }

        public IEnumerable<MyLetter> GetRemovedLetters(string userID, int skip, int take)
        {
            Dictionary<string, object> _params = new Dictionary<string, object>();
            _params.Add("@userId", userID);
            _params.Add("@Skip", skip);
            _params.Add("@Take", take);
            string query = publicMethods.CreateSQLQueryForSP("sp_GetRemovedLetters", _params);
            return database.SelectFromStoreProcedure<MyLetter>(query).ToList();
            //@UserAppID
        }

        public IEnumerable<MyLetter> GetArchivedLetters(string userID, int skip, int take)
        {
            Dictionary<string, object> _params = new Dictionary<string, object>();
            _params.Add("@userId", userID);
            _params.Add("@Skip", skip);
            _params.Add("@Take", take);
            string query = publicMethods.CreateSQLQueryForSP("sp_GetArchivedLetters", _params);
            return database.SelectFromStoreProcedure<MyLetter>(query).ToList();
            //@UserAppID

        }
        public IEnumerable<MyLetter> GetAllLetters(string userID, int skip, int take)
        {
            Dictionary<string, object> _params = new Dictionary<string, object>();
            _params.Add("@userId", userID);
            _params.Add("@Skip", skip);
            _params.Add("@Take", take);
            string query = publicMethods.CreateSQLQueryForSP("sp_GetAllLetters", _params);
            return database.SelectFromStoreProcedure<MyLetter>(query).ToList();
            //@UserAppID
        }

        public IQueryable<Letter> MyLetters(string userId)
        {
            return All().Where(l => l.LetterRefrences.Any(lr => lr.SenderAppID == userId || lr.Recivers.Any(r => r.ID == userId)));
        }
        public IQueryable<Letter> Search(string filter, string appId)
        {
            userService = new UserServices();
            return MyLetters(appId).Where(l => l.Content.Contains(filter) ||
                l.LetterNumber.Contains(filter) ||
                l.Title.Contains(filter) ||
                l.LetterRefrences.Any(r =>
                r.Transcript.Contains(filter) || r.Recivers.Any(rr => rr.FirstName.Contains(filter) || rr.LastName.Contains(filter) || (rr.FirstName + " " + rr.LastName).Contains(filter)))
                ).OrderByDescending(l => l.CreatedOn);
        }
        public string CreateLetter(Letter letter, HttpPostedFileBase file, string recivers, string appID, List<HttpPostedFileBase> uploads, RefrenceType refrenceType, string reciverAppId = null)
        {
            string fileName = publicMethods.UploadFile(file, "OutLetter");
            if (fileName != "1")
            {
                var activePeriod = peroidService.All().Where(m => m.IsActive).FirstOrDefault();
                var _recivers = new List<Reciver>();
                if (reciverAppId == null)
                    _recivers = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Reciver>>(recivers);
                else
                    _recivers.Add(new Reciver { ID = reciverAppId, Status = "1" });
                var reciverUsers = GetReciverUsers(_recivers);

                //===================
                var letterRefrences = new List<LetterRefrences>();
                letterRefrences.Add(CreateLetterRefrence(recivers, appID, "", refrenceType, reciverAppId));
                //if(helperId!=null)
                //    letterRefrences.Add(CreateLetterRefrence(recivers, appID, "", refrenceType, helperId));
                letter.LetterRefrences = letterRefrences;
                letter.CreateOn = publicMethods.ConvertToJalali(DateTime.Now);
                var filename = publicMethods.UploadFile(file, "Letters");
                letter.Files = filename != "-1" ? filename : "";
                letter.Uploads = Uploads(uploads);
                letter.CouncilPeriod = activePeriod;
                this.Create(letter);
                return letter.ID;
            }
            return "error";

        }

        public string AddValueToOutLetter(Letter letter, HttpPostedFileBase file, string recivers, string appID, List<HttpPostedFileBase> uploads, RefrenceType refrenceType, string reciverAppId = null)
        {
            string fileName = publicMethods.UploadFile(file, "OutLetter");
            if (fileName != "1")
            {
                var activePeriod = peroidService.All().Where(m => m.IsActive).FirstOrDefault();
                var _recivers = new List<Reciver>();
                if (reciverAppId == null)
                    _recivers = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Reciver>>(recivers);
                else
                    _recivers.Add(new Reciver { ID = reciverAppId, Status = "1" });

                var reciverUsers = GetReciverUsers(_recivers);
                //===================
                var letterRefrences = new List<LetterRefrences>();
                letterRefrences.Add(CreateLetterRefrence(recivers, appID, "", refrenceType, reciverAppId));
                letter.LetterRefrences = letterRefrences;
                letter.CreateOn = publicMethods.ConvertToJalali(DateTime.Now);

                //letter.Files = publicMethods.UploadFile(file, "Letters");
                Upload _upload = new Upload();
                var filename = publicMethods.UploadFile(file, "Letters");
                _upload.FileAddress = filename != "1" ? filename : "";
                if (_upload != null && _upload.FileAddress != "")
                    letter.Uploads.Add(_upload);
                foreach (var item in Uploads(uploads))
                {
                    letter.Uploads.Add(item);
                }
                letter.CouncilPeriod = activePeriod;
                this.Create(letter);
                return letter.ID;
            }
            return "error";

        }
        List<Upload> Uploads(List<HttpPostedFileBase> uploads)
        {
            if (uploads == null)
                return null;
            var result = new List<Upload>();
            foreach (var item in uploads)
                if (item != null && !item.ContentType.Contains("audio") && !item.ContentType.Contains("video") && !item.ContentType.Contains("html"))
                {
                    var filename = publicMethods.UploadFile(item, "Letters");
                    result.Add(new Upload
                    {
                        FileAddress = filename != "1" ? filename : ""
                    });
                }

            if (result.Count > 0)
                return result;

            return null;
        }
        public string AddLetterRefrence(string recivers, string UserID, string transcript, string letterID, RefrenceType refrenceType, string reciverAppId = null, string commissionId = null)
        {
            var letter = this.Get<string>(letterID);
            letter.LetterRefrences.Add(CreateLetterRefrence(recivers, UserID, transcript, refrenceType, reciverAppId, commissionId: commissionId));
            this.Save();
            return "ارجاع نامه انجام شد";
        }
        LetterRefrences CreateLetterRefrence(string recivers, string UserID, string transcript, RefrenceType refrenceType, string reciverAppId = null, string commissionId = null)
        {
            LetterRefrences letterRefrence = new LetterRefrences();
            userService = new UserServices();

            var _recivers = new List<Reciver>();
            if (reciverAppId == null)
                _recivers = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Reciver>>(recivers);
            else
                _recivers.Add(new Reciver { ID = reciverAppId, Status = "1" });
            //_recivers.Add(new Reciver { ID = reciverAppId, Status = "1" });            

            letterRefrence.Recivers = GetReciverUsers(_recivers);
            letterRefrence.LetterStatuses = SetLetterStatuses(letterRefrence.Recivers.ToList(), _recivers);
            letterRefrence.SenderAppID = UserID;
            letterRefrence.Transcript = transcript;
            letterRefrence.CreateOn = DateTime.Now;
            letterRefrence.RefrenceType = refrenceType;
            letterRefrence.CommissionId = commissionId;
            return letterRefrence;
        }

        IList<LetterStatuses> SetLetterStatuses(List<User> users, List<Reciver> recivers)
        {
            List<LetterStatuses> result = new List<LetterStatuses>();
            foreach (var item in users)
            {
                result.Add(new LetterStatuses { IsNew = true, UserID = item.ID, ReadTime=DateTime.Now, LetterStatusForUser = GetReciversStatus(item.ID, recivers) });
            }
            return result;

        }
        bool GetReciversStatus(string appID, List<Reciver> recivers)
        {
            return (recivers.Find(r => r.ID == appID).Status == "1");
        }
        IList<User> GetReciverUsers(List<Reciver> recivers)
        {
            userService = new UserServices();
            var reciversId = GetReciversID(recivers);
            var reciverUsers = (from users in userService.All()
                                where reciversId.Contains(users.ID)
                                select users).ToList();

            //SendNotify(reciverUsers);
            return reciverUsers;

        }
        List<string> GetReciversID(List<Reciver> recivers)
        {
            List<string> result = new List<string>();
            if (recivers != null)
                foreach (var item in recivers)
                {
                    result.Add(item.ID);
                }
            return result;
        }
        public void RemoveLetterStatusItem(string letterID, LetterStatuses letterStatus)
        {
            //var letter = this.Get<string>(letterID);
            //letter.LetterStatuses.Remove(letterStatus);
            //this.Save();
        }
        public RefrenceType GetLastRefrenceType(string letterId)
        {
            var letter = this.Get<string>(letterId);
            return GetLastRefrence(letter).RefrenceType;
        }
        public void ReadLetter(string letterID, string UserID)
        {
            var letter = this.Get<string>(letterID);
            userService = new UserServices();
            var user = userService.All().FirstOrDefault(u => u.ID == UserID);
            var refrences = letter.LetterRefrences.Where(l => l.LetterStatuses.Any(s => s.UserID == UserID));
            //.LetterStatuses.First(ls => ls.UserID == appID).IsNew = false;
            foreach (var item in refrences)
            {
                item.LetterStatuses.First(ls => ls.UserID == UserID).IsNew = false;
                item.LetterStatuses.First(ls => ls.UserID == UserID).ReadTime = DateTime.Now;
            }
            this.Save();
        }
        public void UnReadLetter(string letterID, string appID)
        {
            var letter = this.Get<string>(letterID);
            userService = new UserServices();
            var user = userService.All().FirstOrDefault(u => u.ID == appID);
            var refrences = letter.LetterRefrences.Where(l => l.LetterStatuses.Any(s => s.UserID == appID));
            foreach (var item in refrences)
            {
                item.LetterStatuses.First(ls => ls.UserID == appID).IsNew = true;
            }
            this.Save();
        }
        public void UnReadForBoss(string letterId)
        {
            userService = new UserServices();
            var bossAppId = userService.All().FirstOrDefault(u => u.IsCouncilBoss || u.BossHelper).ID;
            UnReadLetter(letterId, bossAppId);
        }
        public string MarkAsArchived(string letterID, string appID)
        {
            Letter letter = this.Get<string>(letterID);

            var letterRefrences = letter.LetterRefrences.Where(lr => lr.SenderAppID == appID || lr.LetterStatuses.Any(ls => ls.UserID == appID));//.FirstOrDefault();//
            foreach (var letterRefrence in letterRefrences)
            {
                if (UserIsSender(letterRefrence, appID))
                {
                    letterRefrence.SenderArchived = true;
                }
                foreach (var item in letterRefrence.LetterStatuses)
                {
                    if (item.UserID == appID)
                        item.Archived = true;
                }
            }
            this.Save();
            return "نامه بایگانی شد";
        }
        public string MarkAsEndLetter(string letterID, string appID)
        {
            Letter letter = this.Get<string>(letterID);

            var letterRefrences = letter.LetterRefrences.Where(lr => lr.SenderAppID == appID).FirstOrDefault();//.FirstOrDefault();//
            letterRefrences.RefrenceType = RefrenceType.EndOfLetter;
            this.Save();
            return "نامه خاتمه یافت";
        }
        public string MarkAsRemoved(string letterID, string appID)
        {
            Letter letter = this.Get<string>(letterID);

            var letterRefrences = letter.LetterRefrences.Where(lr => lr.SenderAppID == appID || lr.LetterStatuses.Any(ls => ls.UserID == appID));//.FirstOrDefault();//
            foreach (var letterRefrence in letterRefrences)
            {
                if (UserIsSender(letterRefrence, appID))
                {
                    letterRefrence.SenderRemoved = true;
                }
                foreach (var item in letterRefrence.LetterStatuses)
                {
                    if (item.UserID == appID)
                        item.Removed = true;
                }
            }
            this.Save();
            return "نامه حذف شد";
        }
        public string UndoRemove(string letterID, string appID)
        {
            Letter letter = this.Get<string>(letterID);

            var letterRefrences = letter.LetterRefrences.Where(lr => lr.SenderAppID == appID || lr.LetterStatuses.Any(ls => ls.UserID == appID));//.FirstOrDefault();//
            foreach (var letterRefrence in letterRefrences)
            {
                if (UserIsSender(letterRefrence, appID))
                {
                    letterRefrence.SenderRemoved = false;
                }
                foreach (var item in letterRefrence.LetterStatuses)
                {
                    if (item.UserID == appID)
                        item.Removed = false;
                }
            }
            this.Save();
            return "نامه برگردانده شد";
        }
        bool UserIsSender(LetterRefrences letterRefrence, string appID)
        {
            return letterRefrence.SenderAppID == appID;
        }

        public string SendToCommision(string Transcript, string letterID, string commisionID, string appId)
        {
            var reciverAppId = commissionService.Get<string>(commisionID).CommissionChairman.ID;
            AddLetterRefrence("", appId, Transcript, letterID, RefrenceType.SendToCommission, reciverAppId, commissionId: commisionID);
            ChangLetterStatus(letterID, LetterStatus.SentToCommision);
            return "";
        }
        public string SendToBoss(string Transcript, string letterID, string UserId, string SendType)
        {
            userService = new UserServices();
            var bossId = "";
            if (SendType == "0")
                bossId = userService.All().FirstOrDefault(u => u.IsCouncilBoss).ID;
            else if (SendType == "1") //helperId
                bossId = userService.All().FirstOrDefault(u => u.BossHelper).ID;

            //var bossId = userService.All().FirstOrDefault(u => u.IsCouncilBoss).ID;
            AddLetterRefrence("", UserId, Transcript, letterID, RefrenceType.SendToBoss, bossId);
            ChangLetterStatus(letterID, LetterStatus.SendForBoss);
            return "";
        }
        public string SendToHelper(string Transcript, string letterID, string appId)
        {
            userService = new UserServices();
            var helperId = userService.All().FirstOrDefault(u => u.BossHelper).ID;
            AddLetterRefrence("", appId, Transcript, letterID, RefrenceType.SendToBoss, helperId);
            ChangLetterStatus(letterID, LetterStatus.SendForBoss);
            return "";
        }

        public List<User> GetCommissionMembers(string commissionId)
        {
            var commission = commissionService.Get<string>(commissionId);
            var result = commission.Members.ToList();
            result.Add(commission.CommissionChairman);
            return result;
        }

        string GetCommissionIdFromLetter(string letterId)
        {
            var letter = this.Get<string>(letterId);
            return GetCommissionIdFromLetter(letter);
        }
        string GetCommissionIdFromLetter(Letter letter)
        {
            return letter.LetterRefrences.OrderBy(r => r.CreatedOn).Last(l => l.CommissionId != null).CommissionId;
        }
        //فقط جایی استفاده میشود که مطمنیم نامه در دست پیگیری کمیسیون است
        public List<User> GetCommissionMembers(string letterId, bool isLetter)
        {
            var letter = Get<string>(letterId);

            var commissionRefrence = letter.LetterRefrences.OrderBy(r => r.CreatedOn).Last(rr => rr.CommissionId != null);
            var commission = commissionService.Get<string>(commissionRefrence.CommissionId);
            var result = commission.Members.Where(m => m.IsActive && m.IsCouncilMember).ToList();
            result.Add(commission.CommissionChairman);
            return result;
        }
        public string SendToCommissionMemberss(string Transcript, string letterID, string appId)
        {
            var lastRefrence = GetLastRefrence(letterID);
            if (lastRefrence.CommissionId == null)
                return "NoK";

            var commission = commissionService.Get<string>(lastRefrence.CommissionId);
            List<string> reciversIds = commission.Members.Select(uu => uu.ID).ToList();
            var _recivers = new List<Reciver>();
            foreach (var item in reciversIds)
                _recivers.Add(new Reciver { ID = item, Status = "1" });

            var jsonRecivers = Newtonsoft.Json.JsonConvert.SerializeObject(_recivers);
            AddLetterRefrence(jsonRecivers, appId, Transcript, letterID, RefrenceType.SendToCommissionMembers);
            ChangLetterStatus(letterID, LetterStatus.CommissionVoting);
            return "Ok";
        }
        public string SendToMembers(string Transcript, string letterID, string UserId)
        {
            userService = new UserServices();
            //Members and SiteAdmin
            List<string> reciversIds = userService.All().Where(u => u.IsCouncilMember || u.ID == UserId).Select(uu => uu.ID).ToList();
            var _recivers = new List<Reciver>();
            foreach (var item in reciversIds)
                _recivers.Add(new Reciver { ID = item, Status = "1" });

            var jsonRecivers = Newtonsoft.Json.JsonConvert.SerializeObject(_recivers);
            AddLetterRefrence(jsonRecivers, UserId, Transcript, letterID, RefrenceType.SendToMembers);
            ChangLetterStatus(letterID, LetterStatus.SendToMember);
            return "";
        }
        public string ReVoting(string letterId, bool isCommission = false)
        {
            var votings = votingService.All().Where(v => v.Letter.ID == letterId && v.IsCommission == isCommission)
                                             .Where(n => !n.Deleted)
                                             .ToList();

            var letter = Get<string>(letterId);
            if (isCommission)
            {
                letter.CommissionMeeting = null;
                Save();
                ChangLetterStatus(letterId, LetterStatus.CommissionVoting);
            }
            else
            {
                letter.Meeting = null;
                ChangLetterStatus(letterId, LetterStatus.Votting);
            }

            foreach (var item in votings)
                votingService.Remove(item);
            return "";
        }

        public void ChangLetterStatus(string letterId, LetterStatus status)
        {
            var letter = this.Get<string>(letterId);
            letter.LetterStatus = status;
            this.Save();
        }
        public string CreateMeeting(Meeting meeting, string meetingMembers)
        {
            userService = new UserServices();
            var _recivers = new List<Reciver>();
            _recivers = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Reciver>>(meetingMembers);

            var _meetingMembers = new List<MeetingUsers>();
            if (_recivers != null)
                foreach (var item in _recivers)
                {
                    var user = userService.Get<string>(item.ID);
                    _meetingMembers.Add(new MeetingUsers { status = int.Parse(item.Status), User = user });
                }
            meeting.MeetingUsers = _meetingMembers;
            meeting.NoCouncil = true;
            meetingService.Create(meeting);
            return "";
        }
        public string ReadyForCouncil(string letterID, Meeting meeting, string meetingMembers, bool isCommission = false)
        {
            userService = new UserServices();
            var sessionService = new SessionServices();
            var session = sessionService.All().Where(m => m.MeetingNumber.ToString() == meeting.MeetingNumber && m.IsActive).FirstOrDefault();

            var _recivers = new List<Reciver>();
            _recivers = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Reciver>>(meetingMembers);

            var _meetingMembers = new List<MeetingUsers>();
            foreach (var item in _recivers)
            {
                var user = userService.Get<string>(item.ID);
                _meetingMembers.Add(new MeetingUsers { status = int.Parse(item.Status), User = user });
            }
            meeting.MeetingUsers = _meetingMembers;

            var letter = Get<string>(letterID);

            if (isCommission)
            {
                meeting.CommissionId = GetCommissionIdFromLetter(letter);
                letter.CommissionMeeting = meeting;
            }
            else
                letter.Meeting = meeting;
            session.Meetings.Add(meeting);
            letter.LetterStatus = isCommission ? LetterStatus.CommissionReadyForVoting : LetterStatus.ReadyForVoting;
            Save();
            return "";
        }
        public string ReadyForCommisionCouncil(string letterID, Meeting meeting, string meetingMembers, bool isCommission = false)
        {
            userService = new UserServices();

            var _recivers = new List<Reciver>();
            _recivers = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Reciver>>(meetingMembers);

            var _meetingMembers = new List<MeetingUsers>();
            foreach (var item in _recivers)
            {
                var user = userService.Get<string>(item.ID);
                _meetingMembers.Add(new MeetingUsers { status = int.Parse(item.Status), User = user });
            }
            meeting.MeetingUsers = _meetingMembers;

            var letter = Get<string>(letterID);

            if (isCommission)
            {
                meeting.CommissionId = GetCommissionIdFromLetter(letter);
                letter.CommissionMeeting = meeting;
            }
            else
                letter.Meeting = meeting;

            letter.LetterStatus = isCommission ? LetterStatus.CommissionReadyForVoting : LetterStatus.ReadyForVoting;
            Save();
            return "";
        }

        public string AllowCouncil(string letterID)
        {
            ChangLetterStatus(letterID, LetterStatus.AllowVotting);
            return "";
        }
        public string CommissionAllowCouncil(string letterID)
        {
            ChangLetterStatus(letterID, LetterStatus.CommissionAllowVotting);
            return "";
        }
        public string ChangeOutOfReadyForCouncil(string letterID)
        {
            if (RemoveSessionFromLetter(letterID) == ResultType.Success || RemoveSessionFromLetter(letterID) == ResultType.NoResult)
            {
                RemoveMeetingFromLetter(letterID);
                //ChangLetterStatus(letterID, LetterStatus.OutOfVotting);
                ChangLetterStatus(letterID, LetterStatus.SendToMember);
            }
            return "";
        }
        private ResultType RemoveSessionFromLetter(string letterID)
        {
            var letter = Get<string>(letterID);
            try
            {
                if (letter.Meeting != null)
                {
                    letter.Meeting.MeetingNumber = "-200";
                    this.Update(letter);
                    return ResultType.Success;
                }
                else
                {
                    return ResultType.NoResult;
                }
            }
            catch (Exception ex)
            {
                return ResultType.Exception;
            }

        }
        public void RemoveMeetingFromLetter(string letterID)
        {
            var letter = Get<string>(letterID);
            letter.Meeting = null;
            Save();

        }
        public string ChangeOutOfReadyForCommisionCouncil(string letterID)
        {
            var letter = Get<string>(letterID);
            RemoveMeetingFromLetter(letterID);
            //ChangLetterStatus(letterID, LetterStatus.OutOfCommisionVotting);
            ChangLetterStatus(letterID, LetterStatus.CommissionVoting);
            return "";
        }
        public string UndoOutOfReadyForCouncil(string letterID)
        {
            ChangLetterStatus(letterID, LetterStatus.SendInvite);
            return "";
        }
        public string UndoOutOfReadyForCommisionCouncil(string letterID)
        {
            ChangLetterStatus(letterID, LetterStatus.CommissionVoting);
            return "";
        }

        public bool IsVotingHidden(string lettrerId)
        {
            var letter = Get<string>(lettrerId);
            if (letter.CommissionMeeting == null && letter.Meeting == null)
            {
                return false;
            }
            else
            {
                if (letter.CommissionMeeting != null && letter.Meeting == null)
                {
                    var metting = mettingService.All().Where(m => m.ID == letter.CommissionMeeting.ID).FirstOrDefault();
                    return metting.HiddenVoting ? true : false;
                }
                else if (letter.CommissionMeeting == null && letter.Meeting != null)
                {
                    var metting = mettingService.All().Where(m => m.ID == letter.Meeting.ID).FirstOrDefault();
                    return metting.HiddenVoting ? true : false;
                }
            }
            return false;
        }

        public string DoVoting(string userAppId, string letterId, string voteStatus, string comment, bool isCommission = false)
        {
            Voting voting = new Voting();
            userService = new UserServices();
            var letter = Get<string>(letterId);
            var user = userService.All().FirstOrDefault(u => u.ID == userAppId);

            if (!votingService.All().Any(v => v.User.ID == user.ID && v.Letter.ID == letter.ID && v.IsCommission == isCommission))
            {
                voting.Letter = letter;
                voting.User = user;
                voting.VoteStatus = voteStatus;
                voting.Comment = comment;
                voting.IsCommission = isCommission;
                votingService.Create(voting);
            }
            return "";
        }

        public string EndMeeting(string LetterID, bool isCommission = false)
        {
            if (isCommission)
            {
                ChangLetterStatus(LetterID, LetterStatus.CommissionEnd);
                UnReadForBoss(LetterID);
                return "Ok";
            }
            ChangLetterStatus(LetterID, LetterStatus.End);
            return "Ok";
        }
        public IQueryable<Letter> GlobalSearch(string query)
        {
            return All().Where(l => l.Content.Contains(query) || l.LetterNumber.Contains(query) || l.Title.Contains(query));

        }
        public MeetingResult GetMeetingResult(string leterID, bool isCommission = false)
        {
            string viewName = isCommission ? "CommissionMeetingResult" : "MeetingResult";
            return database.Get<MeetingResult>(viewName, "where LetterID='" + leterID + "'");
        }
        CommissionInfo GetCommissionInfo(string letterId)
        {
            var commissionId = this.Get<string>(letterId).LetterRefrences.OrderBy(r => r.CreatedOn).Last(r => r.CommissionId != null).CommissionId;
            CommissionInfo result = commissionService.All().Where(c => c.ID == commissionId)
                                    .Select(c => new CommissionInfo
                                    {
                                        CommissionChairman = c.CommissionChairman.FirstName + " " + c.CommissionChairman.LastName,
                                        Id = c.ID,
                                        Name = c.Name
                                    }).First();
            return result;

        }
        public PrintModel GetPrintInfo(string letterID, bool isCommission)
        {
            var result = new PrintModel()
            {
                MeetingResult = GetMeetingResult(letterID, isCommission),
                Votings = votingService.GetVotingOfLetter(letterID, isCommission),
                IsCommission = isCommission,
                CommissionInfo = (isCommission ? GetCommissionInfo(letterID) : null)
            };

            return result;
        }
        public IEnumerable<Letter> GetRuleLetter(int skip, int take)
        {

            return All()
                   //.Where(m => m.LetterStatus == LetterStatus.End)
                   //.Where(m => m.CouncilPeriod.IsActive).OrderByDescending(m => m.Meeting.MeetingNumber)
                   //.Where(m=>m.Meeting.MeetingNumber=="61")
                   .Where(m => m.CouncilPeriod.IsActive).OrderByDescending(m => m.Meeting.MeetingNumber)
                   .Skip(skip).Take(take).ToList();

        }
        public IEnumerable<Letter> GetAllRuleLetter()
        {
            return All().Where(m => m.LetterStatus == LetterStatus.End && m.CouncilPeriod.IsActive)
                   .OrderByDescending(m => m.Meeting.MeetingNumber)
                   .ToList();

        }
        public void UpdateRuleLetter(string id, bool rule, string comment)
        {
            try
            {
                var letter = this.Get<string>(id);
                if (letter != null && letter.Meeting != null)
                    letter.Meeting.Content = string.IsNullOrEmpty(comment) ? "" : comment;
                else
                    letter.CommissionMeeting.Content = string.IsNullOrEmpty(comment) ? "" : comment;
                letter.IsRule = rule;
                this.Save();
            }
            catch (Exception ex)
            {

            }

        }
        public bool CheckAllUserVoted(string letterId)
        {
            List<User> mettingUsers = new List<User>();
            List<User> votingUsers = new List<User>();
            var letter = this.Get<string>(letterId);
            List<MeetingUsers> mettings = new List<MeetingUsers>();
            List<Voting> vottings = new List<Voting>();
            if (letter != null && letter.LetterStatus == LetterStatus.AllowVotting)
            {
                if (letter.Meeting!=null)
                {
                    mettings = meetingService.All().Where(m => m.ID == letter.Meeting.ID).FirstOrDefault().MeetingUsers.ToList();
                    vottings = votingService.All().Where(m => m.Letter.ID == letterId && !m.IsCommission).ToList();

                }
                if (letter.CommissionMeeting!=null)
                {
                    mettings = meetingService.All().Where(m => m.ID == letter.CommissionMeeting.ID).FirstOrDefault().MeetingUsers.ToList();
                    vottings = votingService.All().Where(m => m.Letter.ID == letterId && m.IsCommission).ToList();

                }
            }
            else
            {
               }
            //foreach (var item in mettings)
            //{
            //    if (!vottings.Any(m => m.User.ID == item.User.ID))
            //        return false;
            //}
            if (vottings.Count<=1)
            {
                return false;
            }

            return true;
        }
        public List<User> GetAllUserNotVoted(string letterId)
        {
            List<User> mettingUsers = new List<User>();
            List<User> votingUsers = new List<User>();
            List<User> notVotedUser = new List<User>();
            List<MeetingUsers> mettings = new List<MeetingUsers>();
            List<Voting> vottings = new List<Voting>();
            var letter = this.Get<string>(letterId);
            if (letter != null && letter.LetterStatus == LetterStatus.AllowVotting)
            {
                mettings = meetingService.All().Where(m => m.ID == letter.Meeting.ID).FirstOrDefault().MeetingUsers.Where(m => m.status == 1).ToList();
                vottings = votingService.All().Where(m => m.Letter.ID == letterId && !m.IsCommission).ToList();
            }
            else
            {
                mettings = meetingService.All().Where(m => m.ID == letter.CommissionMeeting.ID).FirstOrDefault().MeetingUsers.Where(m => m.status == 1).ToList();
                vottings = votingService.All().Where(m => m.Letter.ID == letterId && m.IsCommission).ToList();
            }
            foreach (var item in mettings)
            {
                if (!vottings.Any(m => m.User.ID == item.User.ID))
                {
                    notVotedUser.Add(new User { FirstName = item.User.FirstName, LastName = item.User.LastName, ID = item.User.ID });
                }
            }
            return notVotedUser;
        }
        public bool LetterIsCommision(Letter letter)
        {
            var lastReff = GetLastRefrence(letter);
            return lastReff.CommissionId != null ? true : false;
        }
        public string GetLetterCommisionName(Letter letter)
        {
            var lastReff = GetLastRefrence(letter);
            return LetterIsCommision(letter) ? commissionService.Get<string>(lastReff.CommissionId).Name : "";
        }

    }
}
