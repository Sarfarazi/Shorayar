using Council.Core.Entities;
using Council.Core.Models;
using Council.Service.DBServices;
using Council.UI.CustomAuthentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Council.UI.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class OrganController : Controller
    {
        OrganService organService = new OrganService();

        #region لیست سازمان ها
        public ActionResult Index()
        {
            ViewBag.Organs = organService.Where(m => !m.Deleted).ToList();
            return View("Index", null);
        }
        #endregion

        #region گرفتن اطلاعات یک سازمان
        [HttpGet]
        public ActionResult Index(string id)
        {
            ViewBag.Organs = organService.Where(m => !m.Deleted).ToList();
            Organ model = organService.Get<string>(id);
            return View(model);
        } 
        #endregion

        #region ثبت و به روز رسانی
        [HttpPost]
        public ActionResult Create(Organ organ)
        {
            if (organ.ID.Length > 2)
                organService.Update(organ);
            else
            {
                Organ newOrgan = new Organ { Name = organ.Name, Code = organ.Code, Email = organ.Email, Address = organ.Address };
                organService.Create(newOrgan);
            }

            TempData["message"] = new ShowMessage
            {
                Message = "عملیات با موفقیت انجام شد",
                MessageType = ShowMessage.MessageTypes.success
            };
            return RedirectToAction("Index");
        }
        #endregion

        #region حذف سازمان
        public ActionResult Delete(string id)
        {
            Organ model = organService.Get<string>(id);
            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Organ model = organService.Get<string>(id);

            model.Deleted = true;
            organService.Update(model);

            TempData["message"] = new ShowMessage
            {
                Message = "عملیات با موفقیت انجام شد",
                MessageType = ShowMessage.MessageTypes.success
            };
            
            return RedirectToAction("Index");
        }
        #endregion
    }
}