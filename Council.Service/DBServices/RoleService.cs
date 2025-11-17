using Council.Core.Entities;
using Council.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Council.Service.DBServices
{
    public class RoleService : CRUDServices<Role>
    {
        private static MainContext _db = new MainContext();
        public IEnumerable<SelectListItem> GetRoles()
        {
            List<SelectListItem> types = this.All()
                    .OrderBy(n => n.Order)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.ID.ToString(),
                            Text = n.RoleDesc
                        }).ToList();

            return new SelectList(types, "Value", "Text");
        }

        public IDictionary<bool,string> CanTakeRole(string ID)
        {
            Dictionary<bool, string> result = new Dictionary<bool, string>();

            var Role = this.Where(r => r.ID == ID).FirstOrDefault();

            if (Role.JustOne && Role.UserRole.Count() > 0)
            {
                //can not take this role
                result.Add(false, Role.RoleDesc);
                return result;
            }

            result.Add(true, "");
            return result;
            //can take this role
        }


        public IDictionary<bool, string> UserCanTakeRole(string ID , string UserID)
        {
            Dictionary<bool, string> result = new Dictionary<bool, string>();

            var Role = this.Where(r => r.ID == ID).FirstOrDefault();

            if (Role.JustOne && Role.UserRole.Count() > 0)
            {
                //can not take this role
                result.Add(false, Role.RoleDesc);
                return result;
            }

            result.Add(true, "");
            return result;
            //can take this role
        }
    }
}
