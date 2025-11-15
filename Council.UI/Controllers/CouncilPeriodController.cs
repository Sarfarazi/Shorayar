using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Council.Service;
using Council.Service.DBServices;
using Council.Core.Entities;
using Council.Core.Models;
using Council.UI.CustomAuthentication;

namespace Council.UI.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class CouncilPeriodController : Controller
    {
        PeriodService periodService = new PeriodService();
        
        public ActionResult Index()
        {
            ViewBag.Periods = periodService.All().ToList();
            return View();
        }

        #region تعریف دوره جدید فعال
        [HttpPost]
        public ActionResult Save(CouncilPeriod cp)
        {
            if (!ModelState.IsValid)
            {
                return View(cp);
            }

            if (cp.IsActive)
            {
                periodService.DeactiveAllPeriod();
            }

            cp.CreatedOn = DateTime.Now;
            cp.Deleted = false;
            periodService.Create(cp);
            ViewBag.Periods = periodService.All().ToList();

            TempData["message"] = new ShowMessage
            {
                Message = "عملیات با موفقیت انجام شد",
                MessageType = ShowMessage.MessageTypes.success
            };
            return RedirectToAction("Index");
        }
        #endregion

        #region تغییر دوره فعال
        public ActionResult ChangePeriod(string Id)
        {
            var model = periodService.All().Where(m => m.ID == Id).FirstOrDefault();

            if (model != null)
            {
                periodService.DeactiveAllPeriod();
                model.IsActive = true;
                periodService.ActivePeriod(model);
            }
            ViewBag.Periods = periodService.All().ToList();

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