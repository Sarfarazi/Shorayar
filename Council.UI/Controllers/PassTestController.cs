using Council.Data.Contexts;
using Council.Service.DBServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Council.UI.Controllers
{
    public class PassTestController : Controller
    {
        UserRoleService userroleService = new UserRoleService();
        public ActionResult Index()
        {
            //using (MainContext db = new MainContext())
            //{
            //    var users = db.Users.Where(m => m.PasswordHash != null).ToList();

            //    foreach (var item in users)
            //    {
            //        string passwordHash = BCrypt.Net.BCrypt.HashPassword(item.PasswordHash);

            //        item.PasswordHash = passwordHash;
            //        db.SaveChanges();
            //    }
            //}

            //var list1 = new string[] { "a", "b", "c" };
            //var list2 = new string[] { "a", "b" };
            ////مشترکا
            //var listCommon = list1.Intersect(list2);

            ViewBag.id = userroleService.UserIdOfRole("Boss");
            ViewBag.boss = userroleService.UserIsBoss("e852b4011f7f469abff1a6c1f18cfe43");
            return View();
        }
    }
}