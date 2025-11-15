using Council.Core.Entities;
using Council.Service.DBServices;
using Council.UI.CustomAuthentication;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Council.UI.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public partial class DraftController : Controller
    {       
        DraftService draftService = new DraftService();
        UserServices userService = new UserServices();
        UniqueNumberServices uniqueNumberService = new UniqueNumberServices();
        OrganService organService = new OrganService();
        public virtual ActionResult Index()
        {
            var UserID = userService.GetUserByUserName(User.Identity.Name).ID;
            var model = draftService.MyDrafts(UserID);
            return View(model);
        }

        public virtual ActionResult Create()
        {
            ViewBag.Recivers = userService.AllActive().ToList();
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Create(Draft draft, string otherUsers)
        {
            var UserID = userService.GetUserByUserName(User.Identity.Name).ID;
            draftService.Create(draft, otherUsers, UserID);
            return RedirectToAction("Index");
        }

        public virtual ActionResult Display(string id)
        {
            var model = draftService.Get<string>(id);
            ViewBag.UserId = userService.GetUserByUserName(User.Identity.Name).ID; ;
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Display(Draft draft)
        {
            draftService.Update(draft);
            return RedirectToAction("Display", new { id = draft.ID });
        }

        public virtual ActionResult Delete(string id)
        {
            draftService.Delete(id);
            return RedirectToAction("Index");
        }
        
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public virtual ActionResult SendDraft(Draft draft, string type = "Letter")
        {
            var letterModel = new OutLetter();
            
            ViewBag.UniqueNumber = uniqueNumberService.GetUniqueNumber();
            ViewBag.Recivers =userService.AllActive().ToList();
            ViewBag.Organs = organService.All().ToList();
            var boss = userService.AllActive().Where(m => m.IsCouncilBoss).FirstOrDefault();
            ViewBag.BossCouncil = boss.FirstName + ' ' + boss.LastName + " " + "[رئیس  شورا]";

            letterModel.Comment = draft.Content;
            letterModel.Subject = draft.Title;
            // return   RedirectToAction("SendOutLetter", "OutLetter", letterModel);
            return View("../OutLetter/SendOutLetter", letterModel);
        }
    }
}