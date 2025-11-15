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
    public class CommissionController : Controller
    {
        CommissionServices commissionService = new CommissionServices();
        UserServices userService = new UserServices();

        #region لیست کمیسیون ها
        public ActionResult Index()
        {
            ViewBag.Members = userService.GetCouncilMembers().ToList();
            ViewBag.Commissions = commissionService.All().Where(m => !m.Deleted).ToList();
            return View();
        } 
        #endregion

        #region ثبت کمیسیون
        [HttpPost]
        public ActionResult Index(Commission commission, string chairManID)
        {
            commissionService.Create(commission, chairManID);
            TempData["message"] = new ShowMessage
            {
                Message = "عملیات با موفقیت انجام شد",
                MessageType = ShowMessage.MessageTypes.success
            };
            return RedirectToAction("Index");
        }
        #endregion

        #region ویرایش کیمیسون
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Edit(string ID)
        {
            ViewBag.Members = userService.GetActiveCouncilMembers().ToList();
            ViewBag.CommisionMembers = commissionService.GetMembers(ID);
            var model = commissionService.Get<string>(ID);
            return View(model);
        }

        [HttpPost]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Edit(Commission commission, string chairManID)
        {
            commissionService.Edit(commission, chairManID);
            TempData["message"] = new ShowMessage
            {
                Message = "عملیات با موفقیت انجام شد",
                MessageType = ShowMessage.MessageTypes.success
            };
            return RedirectToAction("Edit");
        }
        #endregion

        #region افزودن عضو به کمیسیون
        public ActionResult AddCommissionMember(string memberId, string commissionId)
        {
            commissionService.AddCommissionMember(memberId, commissionId);
            return RedirectToAction("Edit", new { ID = commissionId });
        }
        #endregion

        #region حذف عضو از کمیسیون
        public ActionResult RemoveCommissionMember(string memberId, string commissionId)
        {
            commissionService.RemoveCommissionMember(memberId, commissionId);
            return RedirectToAction("Edit", new { ID = commissionId });
        }
        #endregion

        #region اعضای کمیسیون
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult CommissionMembers(string ID)
        {
            ViewBag.Members = userService.GetActiveCouncilMembers().ToList();
            ViewBag.CommisionMembers = commissionService.GetMembers(ID);
            var model = commissionService.Get<string>(ID);
            return View(model);
        }
        #region حذف عضو از کمیسیون
        public ActionResult RemoveMember(string memberId, string commissionId)
        {
            commissionService.RemoveCommissionMember(memberId, commissionId);
            return RedirectToAction("CommissionMembers", new { ID = commissionId });
        }
        public ActionResult AddMember(string memberId, string commissionId)
        {
            commissionService.AddCommissionMember(memberId, commissionId);
            return RedirectToAction("CommissionMembers", new { ID = commissionId });
        }
        #endregion
        #endregion
    }
}