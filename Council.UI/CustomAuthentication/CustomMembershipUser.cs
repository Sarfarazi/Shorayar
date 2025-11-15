using Council.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Council.UI.CustomAuthentication
{
    public class CustomMembershipUser : MembershipUser
    {
        #region User Properties
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public ICollection<Role> Roles { get; set; }

        #endregion

        public CustomMembershipUser(User user) : base("CustomMembership", user.UserName, user.ID, user.UserName, string.Empty, string.Empty, true, false, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now)
        {
            List<Role> roles = new List<Role>();
            foreach (var item in user.UserRole.Select(r => r.Role.RoleTitle))
            {
                Roles.Add(new Role { RoleTitle = item });
            }
            UserId = user.ID;
            FullName = user.UserName;
            Roles = Roles;
        }
    }
}