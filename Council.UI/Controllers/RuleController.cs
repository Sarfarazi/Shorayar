using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Council.Service.DBServices;
using Council.Core.Enums;
using Council.Core.Entities;
using Microsoft.AspNet.Identity;
using Council.Core.Models;
using Council.UI.CustomAuthentication;

namespace Council.UI.Controllers
{
   
    public class RuleController : Controller
    {
        // GET: Rule
        LetterServices letterService = new LetterServices();
        OutLetterServices outletterService = new OutLetterServices();
        MeetingServices meetingServices = new MeetingServices();
        CommissionServices commissionService = new CommissionServices();
        UserServices userService = new UserServices();

        #region مدیریت مصوبات شورا
        [CustomAuthorize(Roles = "Admin,Writer1,Manager,Writer2")]
        public ActionResult Index()
        {
            ViewBag.ruleLetterCount = letterService.GetAllRuleLetter().Count();
            return View();
        }

        [CustomAuthorize(Roles = "Admin,Writer1,Manager,Writer2")]
        public ActionResult GetRuleLetters(int skip = 0, int take = 0)
        {
            var result = take == -1 ? letterService.All()
                                      //.Where(m => m.LetterStatus == LetterStatus.End)
                                      .Where(m => m.CouncilPeriod.IsActive)
                                      .OrderByDescending(m => m.Meeting.MeetingNumber)
                                      .ToList()
                                    : letterService.GetRuleLetter(skip, take);
            ViewBag.rulePageNumber = skip;
            ViewBag.ruleLetterCount = letterService.GetAllRuleLetter().Count();
            ViewBag.Commisions = commissionService.All().Select(m => new CommisionNames
            {
                Id = m.ID,
                Name = m.Name,
            }).ToList();
            return PartialView("_ShowRuleLetter", result);
        }
        #endregion

        #region نمایش مصوبات شورا
        [CustomAuthorize(Roles = "Admin,Boss,Writer1,Writer2,CouncilMember,Manager,BossHelper,Guest")]
        public ActionResult CouncilRule()
        {
            ViewBag.ruleLetterCount = letterService.GetAllRuleLetter().Count();
            return View();
        }
        public ActionResult GetCouncilRule(int skip = 0, int take = 0)
        {
            var result = take == -1 ? letterService.All()
                                      //.Where(m => m.LetterStatus == LetterStatus.End)
                                      .Where(m => m.CouncilPeriod.IsActive)
                                      .OrderByDescending(m => m.Meeting.MeetingNumber)
                                      .ToList()
                                    : letterService.GetRuleLetter(skip, take);
            ViewBag.rulePageNumber = skip;
            ViewBag.ruleLetterCount = letterService.GetAllRuleLetter().Count();
            ViewBag.Commisions = commissionService.All().Select(m => new CommisionNames
            {
                Id = m.ID,
                Name = m.Name,
            }).ToList();
            return PartialView("_GetCouncilRule", result);
        }
        #endregion

        #region مدیریت مصوبات کمیسیون
        [CustomAuthorize(Roles = "Admin,Writer1,Manager,Writer2")]
        public ActionResult CommisionRules()
        {
            CustomPrincipal _User = (CustomPrincipal)HttpContext.User;
            var userId = _User.UserId;

            var MyCommision = commissionService.All().Where(c => c.CommissionChairman.ID == userId).Select(m => m.ID).ToList();

            ViewBag.ruleLetterCount = letterService.All()
                .Where(m => m.CouncilPeriod.IsActive)
                .Where(l => l.LetterRefrences.Any(m => m.CommissionId != null))
                .Where(m => m.LetterStatus == LetterStatus.CommissionEnd).Count();
            return View();
        }

        [CustomAuthorize(Roles = "Admin,Writer1,Manager,Writer2")]
        public ActionResult GetCommisionRuleLetters(int skip = 0, int take = 0)
        {
            CustomPrincipal _User = (CustomPrincipal)HttpContext.User;
            var userId = _User.UserId;

            var MyCommision = commissionService.All().Where(c => c.CommissionChairman.ID == userId).Select(m => m.ID).ToList();

            var result = take == -1
                ? letterService.All()
                .Where(m => m.CouncilPeriod.IsActive)
                .Where(l => l.LetterRefrences.Any(m => m.CommissionId != null ))
                .Where(m => m.LetterStatus == LetterStatus.CommissionEnd)
                .OrderByDescending(m => m.CreatedOn)
                .ToList()
                : letterService.All()
                .Where(m => m.CouncilPeriod.IsActive)
                .Where(l => l.LetterRefrences.Any(m => m.CommissionId != null))
                .Where(m => m.LetterStatus == LetterStatus.CommissionEnd)
                .OrderByDescending(m => m.CreatedOn)
                .Skip(skip).Take(take).ToList();
            ViewBag.rulePageNumber = skip;
            ViewBag.ruleLetterCount = letterService.GetAllRuleLetter().Count();
            ViewBag.Commisions = commissionService.All().Select(m => new CommisionNames
            {
                Id = m.ID,
                Name = m.Name,
            }).ToList();
            return PartialView("_ShowCommisionRuleLetter", result);
        }
        #endregion

        #region نمایش مصوبات کمیسیون
        [CustomAuthorize(Roles = "Admin,Boss,Writer1,Writer2,CouncilMember,Manager,BossHelper,Guest")]
        public ActionResult ShowCommisionRules()
        {
            CustomPrincipal _User = (CustomPrincipal)HttpContext.User;
            var userId = _User.UserId;

            var MyCommision = commissionService.All().Where(c => c.CommissionChairman.ID == userId).Select(m => m.ID).ToList();

            ViewBag.ruleLetterCount = letterService.All()
                .Where(m => m.CouncilPeriod.IsActive)
                .Where(l => l.LetterRefrences.Any(m => m.CommissionId != null))
                .Where(m => m.LetterStatus == LetterStatus.CommissionEnd).Count();
            return View();
        }

        [CustomAuthorize(Roles = "Admin,Boss,Writer1,Writer2,CouncilMember,Manager,BossHelper,Guest")]
        public ActionResult GetCommisionRule(int skip = 0, int take = 0)
        {
            CustomPrincipal _User = (CustomPrincipal)HttpContext.User;
            var userId = _User.UserId;

            var MyCommision = commissionService.All().Where(c => c.CommissionChairman.ID == userId).Select(m => m.ID).ToList();

            var result = take == -1
                ? letterService.All()
                .Where(m => m.CouncilPeriod.IsActive)
                .Where(l => l.LetterRefrences.Any(m => m.CommissionId != null))
                .Where(m => m.LetterStatus == LetterStatus.CommissionEnd)
                .OrderByDescending(m => m.CreatedOn)
                .ToList()
                : letterService.All()
                .Where(m => m.CouncilPeriod.IsActive)
                .Where(l => l.LetterRefrences.Any(m => m.CommissionId != null))
                .Where(m => m.LetterStatus == LetterStatus.CommissionEnd)
                .OrderByDescending(m => m.CreatedOn)
                .Skip(skip).Take(take).ToList();
            ViewBag.rulePageNumber = skip;
            ViewBag.ruleLetterCount = letterService.GetAllRuleLetter().Count();
            ViewBag.Commisions = commissionService.All().Select(m => new CommisionNames
            {
                Id = m.ID,
                Name = m.Name,
            }).ToList();
            return PartialView("_ShowCommisionRule", result);
        }
        #endregion

        public ActionResult ShowRuleLetter(string letterId)
        {
            VotingService votingService = new VotingService();
            var letter = letterService.Get<string>(letterId);
            var isCommision = letter.LetterRefrences.Where(m => m.CommissionId != null).FirstOrDefault();
            var votings = votingService.GetVotingOfLetter(letterId,isCommision!=null && isCommision.CommissionId!=null?true:false);
            ViewBag.Voted = votings.Where(m => m.VoteStatus == "1").Count();
            ViewBag.NoVoted = votings.Where(m => m.VoteStatus == "2").Count();
            ViewBag.CancelVoted = votings.Where(m => m.VoteStatus == "3").Count();
            ViewBag.LetterId = letterId;
            ViewBag.TitleLetter = letter.Title;
            var outletter = outletterService.All().Where(m => m.ID == letter.OutLetter.ID).FirstOrDefault();
            if (outletter != null)
            {
                ViewBag.LetterNumber = outletter.OutLetterNumber;
                ViewBag.RegisterDate = outletter.OutLetterDate;                
                ViewBag.SendOutletterDate = outletter.SendDate;
            }
            if (letter.LetterStatus == LetterStatus.CommissionEnd)
                ViewBag.MeetingNumber = letter.CommissionMeeting.MeetingNumber;
            else
                ViewBag.MeetingNumber = letter.Meeting.MeetingNumber;
            return View();
        }
        public ActionResult ShowCommisionRuleLetter(string letterId)
        {
            VotingService votingService = new VotingService();
            var letter = letterService.Get<string>(letterId);
            var isCommision = letter.LetterRefrences.Where(m => m.CommissionId != null).FirstOrDefault();
            var votings = votingService.GetVotingOfLetter(letterId, isCommision.CommissionId != null ? true : false);
            ViewBag.Voted = votings.Where(m => m.VoteStatus == "1").Count();
            ViewBag.NoVoted = votings.Where(m => m.VoteStatus == "2").Count();
            ViewBag.CancelVoted = votings.Where(m => m.VoteStatus == "3").Count();
            ViewBag.LetterId = letterId;
            ViewBag.TitleLetter = letter.Title;
            var outletter = outletterService.All().Where(m => m.ID == letter.OutLetter.ID).FirstOrDefault();
            if (outletter != null)
            {
                ViewBag.LetterNumber = outletter.OutLetterNumber;
                ViewBag.RegisterDate = outletter.OutLetterDate;
                ViewBag.SendOutletterDate = outletter.SendDate;
            }
            if (letter.LetterStatus == LetterStatus.CommissionEnd)
                ViewBag.MeetingNumber = letter.CommissionMeeting.MeetingNumber;
            else
                ViewBag.MeetingNumber = letter.Meeting.MeetingNumber;
            return View();
        }


        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult ChangeRuleLetter(string id, string rule,string Comment)
        {
            letterService.UpdateRuleLetter(id, Convert.ToBoolean(rule), Comment);
            return RedirectToAction("Index");
        }


        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult ChangeCommisionRuleLetter(string id, string rule, string Comment)
        {

            letterService.UpdateRuleLetter(id, Convert.ToBoolean(rule), Comment);
            return RedirectToAction("CommisionRules");
        }

        [HttpPost]
        public ActionResult AddRuleContent(string letterID)
        {
            return View();
        }

        public ActionResult EditRuleLetter(string letterID)
        {
            var letter = letterService.Get<string>(letterID);
            var model = letter != null ? letter.Meeting != null ? letter.Meeting :letter.CommissionMeeting:null;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditRuleLetter(Meeting meeting)
        {
            var _metting = meetingServices.Get<string>(meeting.ID);
            _metting.Content = meeting.Content;
            meetingServices.Update(_metting);
            return RedirectToAction("Index");
        }

        public ActionResult EditCommisionRuleLetter(string letterID)
        {
            var letter = letterService.Get<string>(letterID);
            var model = letter != null ? letter.Meeting != null ? letter.Meeting : letter.CommissionMeeting : null;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditCommisionRuleLetter(Meeting meeting)
        {
            var _metting = meetingServices.Get<string>(meeting.ID);
            _metting.Content = meeting.Content;
            meetingServices.Update(_metting);
            return RedirectToAction("CommisionRules");
        }

        public ActionResult Test()
        {
            return View();
        }
    }
}