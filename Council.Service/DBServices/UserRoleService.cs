using Council.Core.Entities;
using Council.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Service.DBServices
{
    public class UserRoleService
    {
        private static MainContext db = new MainContext();

        public void SetUserRole(string UserID , string RoleID)
        {
            using (MainContext db = new MainContext())
            {
                var ur = new UserRole
                {
                    UserID = UserID,
                    RoleID = RoleID
                };

                db.UserRole.Add(ur);
                db.SaveChanges();

                var RoleTitle = db.Roles.Find(RoleID).RoleTitle;
                var User = db.Users.Find(UserID);
                switch (RoleTitle)
                {
                    case "Admin":
                        User.IsSiteManager = true;
                        break;

                    case "Boss":
                        User.IsCouncilBoss = true;
                        break;

                    case "Writer1":
                        User.IsWriter1 = true;
                        break;

                    case "Writer2":
                        User.IsWriter2 = true;
                        break;

                    case "CouncilMember":
                        User.IsCouncilMember = true;
                        break;

                    case "Manager":
                        User.IsManager = true;
                        break;

                    case "OtherCouncilMember":
                        User.IsOtherCouncilMember = true;
                        break;

                    case "BossHelper":
                        User.BossHelper = true;
                        break;

                    case "Guest":
                        User.IsCouncilGuest = true;
                        break;
                }

                db.SaveChanges();
            }
        }

        public void RemoveUserRole(string UserID , string RoleID)
        {
            using (MainContext db = new MainContext())
            {
                var userole = db.UserRole.Where(c => c.UserID == UserID && c.RoleID == RoleID).FirstOrDefault();
                db.UserRole.Remove(userole);
                db.SaveChanges();

                var RoleTitle = db.Roles.Find(RoleID).RoleTitle;
                var User = db.Users.Find(UserID);
                switch (RoleTitle)
                {
                    case "Admin":
                        User.IsSiteManager = false;
                        break;

                    case "Boss":
                        User.IsCouncilBoss = false;
                        break;

                    case "Writer1":
                        User.IsWriter1 = false;
                        break;

                    case "Writer2":
                        User.IsWriter2 = false;
                        break;

                    case "CouncilMember":
                        User.IsCouncilMember = false;
                        break;

                    case "Manager":
                        User.IsManager = false;
                        break;

                    case "OtherCouncilMember":
                        User.IsOtherCouncilMember = false;
                        break;

                    case "BossHelper":
                        User.BossHelper = false;
                        break;

                    case "Guest":
                        User.IsCouncilGuest = false;
                        break;
                }

                db.SaveChanges();
            }
        }

        public void SetRoleOfUserTable(string UserID, string RoleID)
        {
            using (MainContext db = new MainContext())
            {
                var RoleTitle = db.Roles.Find(RoleID).RoleTitle;
                var User = db.Users.Find(UserID);

                switch (RoleTitle)
                {
                    case "Admin":
                        User.IsSiteManager = true;
                        break;

                    case "Boss":
                        User.IsCouncilBoss = true;
                        break;

                    case "Writer1":
                        User.IsWriter1 = true;
                        break;

                    case "Writer2":
                        User.IsWriter2 = true;
                        break;

                    case "CouncilMember":
                        User.IsCouncilMember = true;
                        break;

                    case "Manager":
                        User.IsManager = true;
                        break;                    

                    case "OtherCouncilMember":
                        User.IsOtherCouncilMember = true;
                        break;

                    case "BossHelper":
                        User.BossHelper = true;
                        break;

                    case "Guest":
                        User.IsCouncilGuest = true;
                        break;
                }

                db.SaveChanges();
            }
        }

        public string UserIdOfRole(string RoleTitle)
        {
            return (from userRole in db.UserRole
                      join role in db.Roles on userRole.RoleID equals role.ID
                      join user in db.Users on userRole.UserID equals user.ID
                      where role.RoleTitle == RoleTitle
                      select user.ID).FirstOrDefault();
        }

        public IQueryable<User> GetCouncilMembers()
        {
            return from userRole in db.UserRole
                   join role in db.Roles on userRole.RoleID equals role.ID
                   join user in db.Users on userRole.UserID equals user.ID
                   where role.RoleTitle == "CouncilMember"
                   select user;
        }
        public IQueryable<User> GetActiveCouncilMembers()
        {
            return from userRole in db.UserRole
                   join role in db.Roles on userRole.RoleID equals role.ID
                   join user in db.Users on userRole.UserID equals user.ID
                   where role.RoleTitle == "CouncilMember" && user.IsActive
                   select user;
        }

        public bool UserIsBoss(string Id)
        {
            var FindedUser = from userRole in db.UserRole
                             where userRole.UserID == Id
                             join role in db.Roles on userRole.RoleID equals role.ID
                             join user in db.Users on userRole.UserID equals user.ID
                             where role.RoleTitle == "Boss"
                             select user;
            return FindedUser.Count() > 0 ? true : false;
        }

        //public bool UserIsBossHelper(string Id)
        //{
        //    return this.All().FirstOrDefault(u => u.ID == Id).BossHelper;
        //}
        //public bool UserIsSiteManager(string Id)
        //{
        //    return this.All().FirstOrDefault(u => u.ID == Id).IsSiteManager;
        //}
        //public bool UserIsWriter1(string Id)
        //{
        //    return this.All().FirstOrDefault(u => u.ID == Id).IsWriter1;
        //}
        //public bool UserIsWriter2(string Id)
        //{
        //    return this.All().FirstOrDefault(u => u.ID == Id).IsWriter2;
        //}
        //public bool UserIsOtherCouncilMember(string Id)
        //{
        //    return this.All().FirstOrDefault(u => u.ID == Id).IsOtherCouncilMember;
        //}
        //public bool UserIsManager(string Id)
        //{
        //    return this.All().FirstOrDefault(u => u.ID == Id).IsManager;
        //}
        //public bool UserIsMember(string Id)
        //{
        //    return this.All().FirstOrDefault(u => u.ID == Id).IsCouncilMember;
        //}
    }
}
