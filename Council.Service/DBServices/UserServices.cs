using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Council.Core.Entities;
using Council.Core.Interfaces;
using System.Web;
using Council.Core.Enums;
using Council.Core.Models;
using Council.Data;
using Council.Data.Contexts;

namespace Council.Service.DBServices
{
    public class UserServices : CRUDServices<User>, IUser
    {
        PublicMetods publicMethods = new PublicMetods();
        DataBase db = new DataBase();
        private static MainContext _db = new MainContext();
        public bool RemoveRole(string userId , string Role)
        {
            var role = _db.Roles.Where(r => r.RoleTitle == Role).FirstOrDefault();
            if (role == null)
            {
                return false;
            }

            var UserRole = _db.UserRole.Where(m => m.UserID == userId && m.RoleID == role.ID).FirstOrDefault();
            if (UserRole == null)
            {
                return false;
            }

            _db.UserRole.Remove(UserRole);
            _db.SaveChanges();
            return true;
        }

        public bool SetUserRole(string userId , string Role)
        {
            var user = All().FirstOrDefault(u => u.ID == userId);
            var role = _db.Roles.Where(r => r.RoleTitle == Role).FirstOrDefault();
            if (user == null || role == null)
            {
                return false;
            }

            _db.UserRole.Add(new UserRole
            {
                UserID = userId,
                RoleID = role.ID
            });

            _db.SaveChanges();
            return true;
        }

        public User CheckUserPass(string UserName, string Password)
        {
            var user = All().FirstOrDefault(u => u.UserName == UserName);

            if (user != null)
            {
                bool verify = BCrypt.Net.BCrypt.Verify(Password, user.PasswordHash);
                return verify ? user : null;
            }

            return null;
        }

        public User GetUserByID(string userId)
        {
            return All().Where(u => u.ID == userId).FirstOrDefault();
        }

        public User GetUserByUserName(string UserName)
        {
            return All().Where(u => u.UserName == UserName).FirstOrDefault();
        }

        public IQueryable<User> GetActiveUsers()
        {
            return All().Where(u => u.IsActive);
        }

        public User UpdateUser(User user, HttpPostedFileBase file)
        {
            var oldUser = this.GetAsNoTracking(user.ID);

            string fileName = publicMethods.UploadSignature(file, "Signature");
            if (fileName != "1")
            {
                user.Signature = file != null ? fileName : oldUser.Signature;
                return Update(user);
            }
            return null;    
        }

        public bool CheckAvailability(string UserName)
        {
            return this.All().Any(u => u.UserName == UserName);
        }
        public string CreateUser(User user, HttpPostedFileBase file)
        {
            user.Signature = publicMethods.UploadFile(file, "Signature");
            Create(user);
            return "کاربر ثبت شد";
        }
        public User CreateNewUser(User user, HttpPostedFileBase file)
        {
            user.Signature = publicMethods.UploadSignature(file, "Signature");
            return Create(user);            
        }

        public IList<User> AllActive()
        {   
            return this.All().Where(u => u.IsActive).ToList();

        }
        //public bool ChnageOnlineStatus(string userid)
        //{
        //    try
        //    {
        //        var user = this.Get<string>(userid);
        //        user.IsOnline = false;
        //        Save();
        //        return true;
        //    }
        //    catch(Exception ex)
        //    {
        //        return false;
        //    }
        //}

        public IQueryable<User> GetCouncilMembers()
        {
            return All().Where(u => u.IsCouncilMember);
        }
        public IQueryable<User> GetActiveCouncilMembers()
        {
            return All().Where(u => u.IsCouncilMember && u.IsActive);
        }
        public bool UserIsBoss(string Id)
        {
            return this.All().FirstOrDefault(u => u.ID == Id).IsCouncilBoss;
        }
        public bool UserIsBossHelper(string Id)
        {
            return this.All().FirstOrDefault(u => u.ID == Id).BossHelper;
        }
        public bool UserIsSiteManager(string Id)
        {
            return this.All().FirstOrDefault(u => u.ID == Id).IsSiteManager;
        }
        public bool UserIsWriter1(string Id)
        {
            return this.All().FirstOrDefault(u => u.ID == Id).IsWriter1;
        }
        public bool UserIsWriter2(string Id)
        {
            return this.All().FirstOrDefault(u => u.ID == Id).IsWriter2;
        }
        public bool UserIsOtherCouncilMember(string Id)
        {
            return this.All().FirstOrDefault(u => u.ID == Id).IsOtherCouncilMember;
        }
        public bool UserIsManager(string Id)
        {
            return this.All().FirstOrDefault(u => u.ID == Id).IsManager;
        }
        public bool UserIsMember(string Id)
        {
            return this.All().FirstOrDefault(u => u.ID == Id).IsCouncilMember;
        }
        public IList<UserAndUserName> AllowSendRequestUsers(string UserId)
        {
            Dictionary<string, object> _params = new Dictionary<string, object>();
            _params.Add("@UserAppID", UserId);
            string query = publicMethods.CreateSQLQueryForSP("vw_GetUsers", _params);
            var results = db.SelectFromStoreProcedure<UserAndUserName>(query);
            results.Remove(results.FirstOrDefault(r => r.ApplicationUserID == UserId));
            // results = publicMethods.SmallUsersImageToThumb(results);
            return results;
        }
        public List<string> ChangeCouncilBoss(string userID)
        {
            var oldBosses = new List<string>();
            var bosses = this.All().Where(u => u.IsCouncilBoss).ToList();
            foreach (var item in bosses)
            {
                oldBosses.Add(item.ID);
                item.IsCouncilBoss = false;                
            }
            var boss = this.Get<string>(userID);
            
            boss.IsCouncilBoss = true;
            boss.BossHelper = false;
            this.Save();

            return oldBosses;
        }
        public List<string> ChangeCouncilGuest(string userID)
        {
            var oldGuest = new List<string>();
            var guests = this.All().Where(u => u.IsCouncilGuest).ToList();
            foreach (var item in guests)
            {
                oldGuest.Add(item.ID);
                item.IsCouncilGuest = false;
            }
            var guest = this.Get<string>(userID);

            guest.IsCouncilGuest = true;           
            this.Save();

            return oldGuest;
        }
        public User RemoveOtherCouncilMember(string userID)
        {            
            var member = this.Get<string>(userID);

            member.IsOtherCouncilMember = false;           
            this.Save();

            return member;
        }
        public User AddOtherCouncilMember(string userID)
        {
            
            var member = this.Get<string>(userID);

            member.IsOtherCouncilMember = true;
            this.Save();

            return member;
        }
        public User RemoveGusetCouncilMember(string userID)
        {
            var member = this.Get<string>(userID);

            member.IsCouncilGuest = false;
            this.Save();

            return member;
        }
        public User AddGusetCouncilMember(string userID)
        {
            var member = this.Get<string>(userID);

            member.IsCouncilGuest = true;
            this.Save();

            return member;
        }
        public User RemoveManagerCouncilMember(string userID)
        {
            var member = this.Get<string>(userID);

            member.IsManager = false;
            this.Save();

            return member;
        }
        public User AddManagerCouncilMember(string userID)
        {

            var member = this.Get<string>(userID);

            member.IsManager = true;
            this.Save();

            return member;
        }

        public List<string> ChangeCouncilWriter1(string userID)
        {
            var oldWriters = new List<string>();
            var writers = this.All().Where(u => u.IsWriter1).ToList();
            foreach (var item in writers)
            {
                oldWriters.Add(item.ID);
                item.IsWriter1 = false;
            }
            var writer = this.Get<string>(userID);

            writer.IsWriter1 = true;
            writer.IsWriter2 = false;
            this.Save();

            return oldWriters;
        }
        public List<string> ChangeCouncilWriter2(string userID)
        {
            var oldWriters = new List<string>();
            var writers = this.All().Where(u => u.IsWriter2).ToList();
            foreach (var item in writers)
            {
                oldWriters.Add(item.ID);
                item.IsWriter2 = false;
            }
            var writer = this.Get<string>(userID);

            writer.IsWriter2 = true;
            writer.IsWriter1 = false;
            this.Save();

            return oldWriters;
        }
        public List<string> ChangeCouncilBossHelper(string userID)
        {
            var oldBossHelper = new List<string>();
            var BossHelpers = this.All().Where(u => u.BossHelper).ToList();
            foreach (var item in BossHelpers)
            {
                oldBossHelper.Add(item.ID);
                item.BossHelper = false;                              
            }
            var helper = this.Get<string>(userID);

            helper.BossHelper = true;
            helper.IsCouncilBoss = false;
            this.Save();

            return oldBossHelper;
        }
        public void ChangeSiteManager(string userID)
        {
            var siteManagers = this.All().Where(u => u.IsSiteManager).ToList();
            foreach (var item in siteManagers)
            {
                item.IsSiteManager = false;
            }
            var siteManager = this.Get<string>(userID);
            siteManager.IsSiteManager = true;
            this.Save();
        }
       
        public void ActiveUser(string userID)
        {
            var user = this.Get<string>(userID);
            user.IsActive = true;
            this.Save();
        }
        public void DeActiveUser(string userID)
        {
            var user = this.Get<string>(userID);
            user.IsActive = false;
            this.Save();
        }

        public string CurrentNameAndLastName(string appId)
        {
            var user = All().FirstOrDefault(u => u.ID == appId);
            return user.FirstName + " " + user.LastName;
        }

        public IList<UserAndUserName> GetAllUsers()
        {
            return db.Select<UserAndUserName>("vw_GetUsers");
        }

        public IEnumerable<User> GetAllUsersInfo()
        {
            return this.All().OrderBy(m => m.LastName).ToList();
        }

        public string GetWriterOneFullName()
        {
            var user = All().Where(m => m.IsWriter1).FirstOrDefault();
            return user.FirstName + ' ' + user.LastName;
        }
        public string GetWriterOneId()
        {
            var user = All().Where(m => m.IsWriter1).FirstOrDefault();
            return user.ID;
        }
    }
}
