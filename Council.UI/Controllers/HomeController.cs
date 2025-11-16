using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Council.Service.DBServices;
using Council.Core.Entities;
using Council.Core.Extensions;
using Council.Core.Enums;
using Council.UI.Helpers;
using Council.Core.Statics;
using Council.Core.Models;
using Council.Data.Contexts;
using Council.UI.CustomAuthentication;

using Microsoft.AspNetCore.Http;


namespace Council.UI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        LetterServices letterService = new LetterServices();
        OutLetterServices OutLetterService = new OutLetterServices();
        UserServices userService = new UserServices();
        DefaultStatementService defaultStatementService = new DefaultStatementService();
        CommissionServices commissionService = new CommissionServices();
        HomePageService homeService;
        OrganService organService = new OrganService();

        //public UserManager<ApplicationUser> UserManager { get; private set; }


        #region *********** داشبور **************
        public ActionResult Index()
        {
            var processorId = GetPCInformations.GetProcessorId();
            var UserID = userService.GetUserByUserName(User.Identity.Name).ID;
            var account = new AccountController();
            //ViewBag.RoleName=account.GetRoleName(appID);
            ViewBag.Organs = organService.Where(m => m.Deleted == false).ToList();
            homeService = new HomePageService(UserID);
            var model = homeService.GetModel();
            //ViewBag.UserIsBoss = userService.UserIsBoss(appID);
            //ViewBag.UserIsSiteManager = userService.UserIsSiteManager(appID);
            //ViewBag.UserIsMember = userService.UserIsMember(appID);
            //ViewBag.MyLetters = letterService.All().Where(l => l.LetterRefrences.Any(r => r.SenderAppID == appID || r.Recivers.Any(u => u.ApplicationUserID == appID)));
            //ViewBag.Finished = letterService.All().Where(l => l.LetterStatus == LetterStatus.End).OrderByDescending(l => l.CreatedOn).Take(10).ToList();
            return View(model);
        }

        #region کارتابل من
        public ActionResult LastMyLetter()
        {
            var UserID = userService.GetUserByUserName(User.Identity.Name).ID;
            var homeService = new HomePageService(UserID);
            var model = homeService.GetModel();
            string result = PublicHelpers.RenderRazorViewToString(this, "_LastMyLettersPrtial", null);
            return Json(result);
        }
        public ActionResult LastMyOutLetter()
        {
            var UserID = userService.GetUserByUserName(User.Identity.Name).ID;
            var homeService = new HomePageService(UserID);
            var model = homeService.GetModel();
            string result = PublicHelpers.RenderRazorViewToString(this, "_LastMyOutLettersPrtial", null);
            return Json(result);
        }
        #endregion

        #region تنظیمات رای گیری شورا
        public ActionResult GetLastReadyForCouncil()
        {
            var UserID = userService.GetUserByUserName(User.Identity.Name).ID;
            var homeService = new HomePageService(UserID);
            var model = homeService.GetModel();
            string result = PublicHelpers.RenderRazorViewToString(this, "_LastMyReadyForCouncilPartial", model);
            return Json(result);
        }
        #endregion


        #region تنظیمات رای گیری کمیسیون
        public ActionResult GetLastReadyForCommsion()
        {
            var UserID = userService.GetUserByUserName(User.Identity.Name).ID;
            var homeService = new HomePageService(UserID);
            var model = homeService.GetModel();
            string result = PublicHelpers.RenderRazorViewToString(this, "_LastMyReadyForCommisionPartial", model);
            return Json(result);
        }
        #endregion

        #region اجازه رای گیری
        public ActionResult GetLastAllowForVote()
        {
            var UserID = userService.GetUserByUserName(User.Identity.Name).ID;
            var homeService = new HomePageService(UserID);
            var model = homeService.GetModel();
            string result = PublicHelpers.RenderRazorViewToString(this, "_LastAllowForVotePartial", model);
            return Json(result);
        }
        #endregion

        #region آماده رای گیری
        public ActionResult GetLastReadyForVote()
        {
            var UserID = userService.GetUserByUserName(User.Identity.Name).ID;
            var homeService = new HomePageService(UserID);
            var model = homeService.GetModel();
            string result = PublicHelpers.RenderRazorViewToString(this, "_LastMyReadyForVotePartial", model);
            return Json(result);
        }
        #endregion

        #region نتایج
        public ActionResult AllVotedResult()
        {
            var UserID = userService.GetUserByUserName(User.Identity.Name).ID;
            var homeService = new HomePageService(UserID);
            var model = homeService.GetModel();
            string result = PublicHelpers.RenderRazorViewToString(this, "_LastMyResultPrtial", null);
            return Json(result);
        }

        #region آخرین رای گیری های انجام شده (کمیسیون)
        [CustomAuthorize(Roles = "Admin,Boss, Writer1, CouncilMember, Manager, Writer2, BossHelper,Guest")]
        public ActionResult GetLastCommisionData(string skip, string take)
        {
            var UserID = userService.GetUserByUserName(User.Identity.Name).ID;
            if (userService.UserIsMember(UserID))
            {
                homeService = new HomePageService(UserID);
                ViewBag.CommisionCount = homeService.GetComisionCount();
                ViewBag.CommPageNumber = skip;
                var model = homeService.GetLastCommisionModel(skip, take);
                return PartialView("_LastCommisionPartial", model);
            }
            return null;
        } 
        #endregion
        #endregion

        #region مصوبات
        public ActionResult AllRuleResult()
        {
            var UserID = userService.GetUserByUserName(User.Identity.Name).ID;
            var homeService = new HomePageService(UserID);
            var model = homeService.GetModel();
            string result = PublicHelpers.RenderRazorViewToString(this, "_LastMyResultPrtial", null);
            return Json(result);
        }
        #endregion 
        #endregion

        public ActionResult NewLetters()
        {
            return View();
        }

        #region آخرین رای گیری های کمیسیون
        [CustomAuthorize(Roles = "Admin,Boss,BossHelper, Writer1,Writer2,CouncilMember,Guest")]
        public ActionResult CommisionVoted()
        {
            return View();
        } 
        #endregion

        #region آخرین رای گیری های شورا
        [CustomAuthorize(Roles = "Admin,Boss,BossHelper,Manager, Writer1,Writer2,CouncilMember,Guest")]
        public ActionResult CouncilVoted()
        {
            return View();
        }
        #endregion

        #region آخرین رای گیری های انجام شده (شورا)
        [CustomAuthorize(Roles = "Admin,Boss,BossHelper,Manager, Writer1,Writer2,CouncilMember,Guest")]
        public ActionResult GetLastOpinionData(string skip, string take)
        {
            var UserID = userService.GetUserByUserName(User.Identity.Name).ID;
            //if (userService.UserIsMember(UserID))
            //{
            //    homeService = new HomePageService(UserID);
            //    ViewBag.OpinionCount = homeService.GetMyLetterCount();
            //    ViewBag.OpinPageNumber = skip;
            //    ViewBag.Commisions = commissionService.All().Select(m => new CommisionNames { Name = m.Name, Id = m.ID }).ToList();
            //    var model = homeService.GetLastOpinionModel(skip, take);
            //    return PartialView("_LastOpinionPartial", model);
            //}

            homeService = new HomePageService(UserID);
            ViewBag.OpinionCount = homeService.GetMyLetterCount();
            ViewBag.OpinPageNumber = skip;
            ViewBag.Commisions = commissionService.All().Select(m => new CommisionNames { Name = m.Name, Id = m.ID }).ToList();
            var model = homeService.GetLastOpinionModel(skip, take);
            return PartialView("_LastOpinionPartial", model);
        } 
        #endregion

        #region جستجو در سایت
        public ActionResult SearchResult(string searchTxt)
        {
            var UserID = userService.GetUserByUserName(User.Identity.Name).ID;
            homeService = new HomePageService(UserID);
            var model = homeService.GetSearchModel(searchTxt);
            ViewBag.SearchText = searchTxt;
            return View(model);
        } 
        #endregion

        public ActionResult GetNewLetters()
        {
            var UserID = userService.GetUserByUserName(User.Identity.Name).ID;

            //var ret = letterService.All().Where(n=>n.CouncilPeriod.IsActive).Where(h=>!h.Deleted)
            //    .Where(n=>n.LetterStatus==LetterStatus.NormalLetter || n.LetterStatus== LetterStatus.SentToCommision || n.LetterStatus == LetterStatus.SendForBoss || n.LetterStatus == LetterStatus.CommissionEnd)
            //    .Where(l => l.LetterRefrences.Any(r => r.LetterStatuses.Any(s => s.UserID == UserID && !s.Removed )))               
            //    .OrderByDescending(m => m.CreatedOn);

            var all = letterService.All().ToList();
            var s1 = letterService.All().Where(n => n.CouncilPeriod.IsActive && !n.Deleted).ToList();
            var s2 = letterService.All().Where(n => n.CouncilPeriod.IsActive && !n.Deleted
            && (n.LetterStatus == LetterStatus.NormalLetter || n.LetterStatus == LetterStatus.SentToCommision || n.LetterStatus == LetterStatus.SendForBoss || n.LetterStatus == LetterStatus.CommissionEnd)).ToList();

            var ret = letterService.All()
                .Where( n => n.CouncilPeriod.IsActive && !n.Deleted &&
                          (n.LetterStatus == LetterStatus.NormalLetter ||
                           n.LetterStatus == LetterStatus.SentToCommision ||
                           n.LetterStatus == LetterStatus.SendForBoss || 
                           n.LetterStatus == LetterStatus.CommissionEnd || 
                           n.LetterStatus == LetterStatus.SendToMember ||
                           n.LetterStatus == LetterStatus.CommissionVoting) &&
                        n.LetterRefrences.Any(r => r.LetterStatuses.Any(s => s.UserID == UserID && !s.Removed))
                       ).OrderByDescending(m => m.CreatedOn).ToList();

            //var model= ret.Where(l => l.LetterRefrences.Any(r => r.LetterStatuses.Any(s => s.UserID == appID && s.IsNew))).OrderByDescending(l => l.CreatedOn).ToList();
            ViewBag.LettersCount = ret.Count();
            ViewBag.RowNumber = 1;
            return PartialView("_NewLetterPartial", ret.Take(50).ToList());
        }

        public ActionResult GetNewOutLetters()
        {
            var UserID = userService.GetUserByUserName(User.Identity.Name).ID;

            //var ret = letterService.All().Where(n=>n.CouncilPeriod.IsActive).Where(h=>!h.Deleted)
            //    .Where(n=>n.LetterStatus==LetterStatus.NormalLetter || n.LetterStatus== LetterStatus.SentToCommision || n.LetterStatus == LetterStatus.SendForBoss || n.LetterStatus == LetterStatus.CommissionEnd)
            //    .Where(l => l.LetterRefrences.Any(r => r.LetterStatuses.Any(s => s.UserID == UserID && !s.Removed )))               
            //    .OrderByDescending(m => m.CreatedOn);

           // var all = OutLetterService.All().ToList();
          //  var s1 = OutLetterService.All().Where(n => n.CouncilPeriod.IsActive && !n.Deleted).ToList();
           // var s2 = OutLetterService.All().Where(n => n.CouncilPeriod.IsActive && !n.Deleted).ToList();
           //monire && (n.LetterStatus == LetterStatus.NormalLetter || n.LetterStatus == LetterStatus.SentToCommision || n.LetterStatus == LetterStatus.SendForBoss || n.LetterStatus == LetterStatus.CommissionEnd)).ToList();

            var ret = OutLetterService.All()
                .Where(n => n.CouncilPeriod.IsActive && !n.Deleted && n.ConfirmBoss == false && n.OutLetterStatus==((byte)OutLetterStatus.WaitForComfirm)).Where(l => !l.Received && !l.Archived && !l.Removed ).Where(m=>m.SignatureUserID==UserID).OrderByDescending(m => m.CreatedOn).ToList();

            //var model= ret.Where(l => l.LetterRefrences.Any(r => r.LetterStatuses.Any(s => s.UserID == appID && s.IsNew))).OrderByDescending(l => l.CreatedOn).ToList();
            ViewBag.LettersCount = ret.Count();
            ViewBag.RowNumber = 1;
            return PartialView("_NewOutLetterPartial", ret.Take(50).ToList());
        }

        public ActionResult SearchNewLetters(string txt)
        {
            var UserID = userService.GetUserByUserName(User.Identity.Name).ID;
            // var ret = letterService.All().Where(l => l.LetterRefrences.Any(r => r.SenderAppID == appID || r.Recivers.Any(u => u.ApplicationUserID == appID)) && l.Title.Contains(txt)).OrderBy(m => m.CreateOn);
            var ret = letterService.All().Where(k => k.CouncilPeriod.IsActive).Where(l => l.LetterRefrences.Any(r => r.SenderAppID == UserID ) && l.Title.Contains(txt)).OrderBy(m => m.CreateOn);
            var model = ret.Where(l => l.LetterRefrences.Any(r => r.LetterStatuses.Any(s => s.UserID == UserID && s.IsNew && !s.Removed))).OrderByDescending(l => l.CreatedOn);
            ViewBag.LettersCount = -1;
            ViewBag.RowNumber = 1;
            return PartialView("_NewLetterPartial", model.ToList());
        }

        public ActionResult SearchLastOpinionData(string txt)
        {
            var UserID = userService.GetUserByUserName(User.Identity.Name).ID;
            if (userService.UserIsMember(UserID))
            {
                homeService = new HomePageService(UserID);
                var model = homeService.SearchLastOpinionModel(txt);
                ViewBag.OpinionCount = -1;
                ViewBag.OpinPageNumber = 0;
                return PartialView("_LastOpinionPartial", model);
            }
            return null;
        }

        public ActionResult SearchLastCommisionData(string txt)
        {
            var UserID = userService.GetUserByUserName(User.Identity.Name).ID;
            if (userService.UserIsMember(UserID))
            {
                homeService = new HomePageService(UserID);
                var model = homeService.SearchLastCommisionModel(txt);
                ViewBag.CommisionCount = -1;
                ViewBag.CommPageNumber = 0;
                return PartialView("_LastCommisionPartial", model);
            }
            return null;
        }

        public ActionResult GetExtraLetters(string skip,string take)
        {
            var UserID = userService.GetUserByUserName(User.Identity.Name).ID;
            var ret =take=="-1"? letterService.All()
                .Where(k => k.CouncilPeriod.IsActive)
                .Where(n => n.LetterStatus == LetterStatus.NormalLetter || n.LetterStatus == LetterStatus.SentToCommision || n.LetterStatus == LetterStatus.SendForBoss || n.LetterStatus == LetterStatus.CommissionEnd)
                .Where(l => l.LetterRefrences.Any(r => r.LetterStatuses.Any(s => s.UserID == UserID && !s.Removed)))
                .OrderByDescending(m => m.CreatedOn)
                .Skip(Convert.ToInt32(skip)).ToList()
                : letterService.All()
                .Where(k => k.CouncilPeriod.IsActive)
                //.Where(l => l.LetterRefrences.Any(r => r.SenderAppID == appID || r.Recivers.Any(u => u.ApplicationUserID == appID)))
                .Where(n => n.LetterStatus == LetterStatus.NormalLetter || n.LetterStatus == LetterStatus.SentToCommision || n.LetterStatus == LetterStatus.SendForBoss || n.LetterStatus == LetterStatus.CommissionEnd)
                .Where(l => l.LetterRefrences.Any(r => r.LetterStatuses.Any(s => s.UserID == UserID && !s.Removed)))
                .OrderByDescending(m => m.CreatedOn)
                .Skip(Convert.ToInt32(skip)).Take(Convert.ToInt32(take)).ToList();
            ViewBag.RowNumber = Convert.ToInt32(skip) + 1 ;
            string result = PublicHelpers.RenderRazorViewToString(this, "_NewLetterPartial", ret);
            return Json(result);
        }

        #region جملات پیش فرض
        #region لیست جملات
        [Authorize(Roles = "Admin")]
        public ActionResult DefaultStatements()
        {
            ViewBag.DefaultStatements = defaultStatementService.All().Where(m => !m.Deleted).ToList();
            return View();
        } 
        #endregion

        #region افزودن جمله
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult DefaultStatements(DefaultStatement defaultStatement)
        {
            defaultStatementService.Create(defaultStatement);
            TempData["message"] = new ShowMessage
            {
                Message = "عملیات با موفقیت انجام شد",
                MessageType = ShowMessage.MessageTypes.success
            };
            return RedirectToAction("DefaultStatements");
        }
        #endregion

        #region حذف جمله
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult DeleteStatement(string ID)
        {
            defaultStatementService.Delete(ID);
            TempData["message"] = new ShowMessage
            {
                Message = "عملیات با موفقیت انجام شد",
                MessageType = ShowMessage.MessageTypes.success
            };
            return RedirectToAction("DefaultStatements");
        }
        #endregion
        #endregion


        public ActionResult Print()
        {
            string id = "2f39c11372374b4b9a30bf9d7cfba3ec";
            var letter =OutLetterService.Get<string>(id);
            var boss = userService.All().Where(m => m.IsCouncilBoss).FirstOrDefault();
            PrintOutLetter printLetter = new PrintOutLetter();
            printLetter.Content = letter.Comment;
            printLetter.LetterDate = letter.OutLetterDate;
            printLetter.LetterNumber = letter.OutLetterNumber;
            printLetter.SaveLetterDate = letter.SendDate;
            printLetter.Subject = letter.Subject;
            printLetter.Reciever = letter.Organ.Name;
            printLetter.UploadCount = letter.Uploads.Count.ToString();
            printLetter.SignatureUrl = boss.Signature;
           // printLetter.CopyTo = letter.CopyTo;
            printLetter.BossName = boss.FirstName + " " + boss.LastName;
            TempData["tmp"] = printLetter;
            return View(TempData);           
        }
      

    }
}