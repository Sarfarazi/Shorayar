using Council.Core.Entities;
using Council.Core.Interfaces;
using Council.Core.Models;
using Council.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Service.DBServices
{
    public class DraftService : CRUDServices<Draft>, IDraft
    {
        DataBase database = new DataBase();
        UserServices userService = new UserServices();
        PublicMetods publicMethods = new PublicMetods();
        public DraftService()
            : base("MainContext")
        {
        }
        public Draft Create(Draft draft, string users, string UserId)
        {
            var allowUsers = GetAllowUsers(users, UserId);
            draft.OwnerId = UserId;
            //allowUsers.Add(userService.All().FirstOrDefault(u => u.ApplicationUserID == appId));
            draft.Users = allowUsers;
            // draft.Creator = userService.All().FirstOrDefault(u => u.ApplicationUserID == appId);
            return Create(draft);
        }
        IList<User> GetAllowUsers(string reciversID, string appId = null)
        {

            var _reciversID = new List<string>();

            if (!String.IsNullOrWhiteSpace(reciversID))
                _reciversID = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Reciver>>(reciversID).Select(r => r.ID).ToList();
            if (appId != null) _reciversID.Add(appId);

            if (_reciversID.Count == 0)
                return new List<User>();

            return (from users in userService.All()
                    where _reciversID.Contains(users.ID) || _reciversID.Contains(users.ID)
                    select users).ToList();

        }
        public IList<DraftViewModel> MyDrafts(string UserId)
        {
            Dictionary<string, object> _params = new Dictionary<string, object>();
            _params.Add("@userId", UserId);
            string query = publicMethods.CreateSQLQueryForSP("sp_MyDrafts", _params);
            return database.SelectFromStoreProcedure<DraftViewModel>(query);
        }
    }
}
