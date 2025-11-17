using Council.Core.Entities;
using Council.Service.DBServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Council.Core.Models;
using Council.Core.Statics;
using Council.Core.Enums;

using Council.UI.Helpers;
using Council.Data.Contexts;
using Council.UI.CustomAuthentication;

namespace Council.UI.Controllers
{
    //[CustomAuthorize(Roles = "Admin")]
    public class LetterController : Controller
    {
        UniqueNumberServices uniqueNumberService = new UniqueNumberServices();
        UserServices userService = new UserServices();
        LetterServices letterService = new LetterServices();
        CommissionServices commisionService = new CommissionServices();
        MeetingServices meetingService = new MeetingServices();
        VotingService votingService = new VotingService();
        DefaultStatementService defaultStatementService = new DefaultStatementService();
        SessionServices sessionService = new SessionServices();
        public ActionResult Index()
        {
            ViewBag.UniqueNumber = uniqueNumberService.GetUniqueNumber();
            ViewBag.Recivers = userService.AllActive().ToList();
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Index(Letter letter, string recivers, HttpPostedFileBase file, List<HttpPostedFileBase> uploads, string Note)
        {
            if (!string.IsNullOrEmpty(Note))
                letter = AddLetterNote(letter, Note);

            var UserID = userService.GetUserByUserName(User.Identity.Name).ID;
            string letterID = letterService.CreateLetter(letter, file, recivers, UserID , uploads, RefrenceType.Normal);
            if (letterID == "error")
            {
                ViewBag.ErrorMessage = "فرمت فایل برای الصاق صحیح نمی باشد";
                ViewBag.Recivers = userService.AllActive().ToList();
                return View("Index");
            }
            else
            {
                return RedirectToAction("LetterItem", new { letterID = letterID });
            }
        }
        private Letter AddLetterNote(Letter letter, string note)
        {
            var userId = userService.GetUserByUserName(User.Identity.Name).ID;
            letter.Notes = new List<Note>();
            Note _note = new Note() { ContentText = note, WriterId = userId };
            letter.Notes.Add(_note);
            return letter;
        }

        public ActionResult Display(string skip = "0", string take = "50")
        {
            DisplayModel model = new DisplayModel();
            var UserId = userService.GetUserByUserName(User.Identity.Name).ID;
            var noteService = new NoteService();
            List<Note> noteList = new List<Note>();

            var model1 = take == "-1" ? letterService.GetMyLettersCount(UserId, Settings.AllLetter) : letterService.GetMyLetters(UserId, Settings.AllLetter, Convert.ToInt32(skip), Convert.ToInt32(take));
            var LetterCounts = letterService.GetMyLettersCount(UserId, Settings.AllLetter);

            /* My Sp*/
            model.AllLetter = letterService.GetAllLetters(UserId, Convert.ToInt32(skip), Convert.ToInt32(take)).Where(m => !m.Deleted).ToList();
            ViewBag.AllLetterCount = LetterCounts.Where(l => !l.Archived && !l.Deleted).OrderByDescending(l => l.CreatedOn).Count();
            //ViewBag.AllLetterCount = LetterCounts.Where(l => l.Archived == false && l.Deleted == false).OrderByDescending(l => l.CreatedOn).Count();
            model.ArchivedLetter = letterService.GetArchivedLetters(UserId, Convert.ToInt32(skip), Convert.ToInt32(take)).ToList();
            ViewBag.ArchivedLetters = LetterCounts.Where(l => l.Archived).Count();
            //ViewBag.ArchivedLetters = LetterCounts.Where(l => l.Archived).Count();
            model.RemovedLetter = letterService.GetRemovedLetters(UserId, Convert.ToInt32(skip), Convert.ToInt32(take)).ToList();
            ViewBag.RemovedLetters = LetterCounts.Where(l => l.Deleted).Count();
            //ViewBag.RemovedLetters = LetterCounts.Where(l => l.Deleted).Count();
            model.OutLetter = letterService.GetOutLetters(UserId, Convert.ToInt32(skip), Convert.ToInt32(take)).ToList();
            ViewBag.OutLetters = LetterCounts.Where(l => !String.IsNullOrEmpty(l.OutLetter_ID)).Count();
            //ViewBag.OutLetters = LetterCounts.Where(l => !String.IsNullOrEmpty(l.OutLetter_ID)).Count();
            model.CouncilLetter = letterService.GetCouncilLetters(UserId, Convert.ToInt32(skip), Convert.ToInt32(take)).ToList();
            ViewBag.CouncilLetters = LetterCounts.Where(l => l.LetterStatus > 3).Count();
            //ViewBag.CouncilLetters = LetterCounts.Where(l => l.LetterStatus > 3).Count();
            model.OutOfReadyCouncilLetter = letterService.GetOutOfReadyForCouncilLetters(UserId, Convert.ToInt32(skip), Convert.ToInt32(take)).ToList();
            ViewBag.OutOfReadyLetters = LetterCounts.Where(l => l.LetterStatus == 10 || l.LetterStatus == 11).Count();
            /* My Sp*/


            ViewBag.AllLetterCount = LetterCounts.Where(l => l.Archived == false && l.Deleted == false).OrderByDescending(l => l.CreatedOn).Count();
            ViewBag.CouncilLetters = LetterCounts.Where(l => l.LetterStatus > 3).Count();
            ViewBag.ArchivedLetters = LetterCounts.Where(l => l.Archived).Count();
            ViewBag.RemovedLetters = LetterCounts.Where(l => l.Deleted).Count();
            ViewBag.OutLetters = LetterCounts.Where(l => !String.IsNullOrEmpty(l.OutLetter_ID)).Count();
            ViewBag.ArchivedRowNumber = skip == "0" && take == "-1" ? 1 : Convert.ToInt32(skip) + 1;
            ViewBag.CouncilRowNumber = skip == "0" && take == "-1" ? 1 : Convert.ToInt32(skip) + 1;
            ViewBag.RemovedRowNumber = skip == "0" && take == "-1" ? 1 : Convert.ToInt32(skip) + 1;
            ViewBag.OutLetterRowNumber = skip == "0" && take == "-1" ? 1 : Convert.ToInt32(skip) + 1;
            ViewBag.AllLetterRowNumber = skip == "0" && take == "-1" ? 1 : Convert.ToInt32(skip) + 1;
            ViewBag.User = userService.FirstOrDefault(u => u.ID == UserId);
            ViewBag.EndLetters = letterService.Where(m => m.CouncilPeriod.IsActive && m.OutLetter == null && m.LetterRefrences.Any(n => n.SenderAppID == UserId && n.RefrenceType == RefrenceType.EndOfLetter)).Select(b => b.ID).ToList();

            return View(model);
        }
        public ActionResult DisplayOnlyArchivedLetters(string skip = "0", string take = "50")
        {
            var UserId = userService.GetUserByUserName(User.Identity.Name).ID;

            var model = take == "-1" ? letterService.GetMyLettersCount(UserId, Settings.ArchivedLetter) : letterService.GetMyLetters(UserId, Settings.ArchivedLetter, Convert.ToInt32(skip), Convert.ToInt32(take));
            var LetterCounts = letterService.GetMyLettersCount(UserId, Settings.ArchivedLetter);

            ViewBag.ArchivedLetters = LetterCounts.Where(l => l.Archived).Count();

            ViewBag.ArchivedRowNumber = skip == "0" && take == "-1" ? 1 : Convert.ToInt32(skip) + 1;
            ViewBag.User = userService.FirstOrDefault(u => u.ID == UserId);
            return PartialView("_ArchivedLetters", model.Where(l => l.Archived));
        }
        public ActionResult DisplayOnlyCouncilLetters(string skip = "0", string take = "50")
        {
            var UserId = userService.GetUserByUserName(User.Identity.Name).ID;

            var model = take == "-1" ? letterService.GetMyLettersCount(UserId, Settings.CouncilLetter) : letterService.GetMyLetters(UserId, Settings.CouncilLetter, Convert.ToInt32(skip), Convert.ToInt32(take));
            var LetterCounts = letterService.GetMyLettersCount(UserId, Settings.CouncilLetter);

            ViewBag.CouncilLetters = LetterCounts.Where(l => l.LetterStatus > 3).Count();

            ViewBag.CouncilRowNumber = skip == "0" && take == "-1" ? 1 : Convert.ToInt32(skip) + 1;
            ViewBag.User = userService.FirstOrDefault(u => u.ID == UserId);
            return PartialView("_ForCouncilLetters", model.Where(l => l.LetterStatus > 3));
        }
        public ActionResult DisplayOnlyOutOfReadyLetters(string skip = "0", string take = "50")
        {
            var UserId = userService.GetUserByUserName(User.Identity.Name).ID;

            var model = take == "-1" ? letterService.GetMyLettersCount(UserId, Settings.CouncilLetter) : letterService.GetMyLetters(UserId, Settings.CouncilLetter, Convert.ToInt32(skip), Convert.ToInt32(take));
            var LetterCounts = letterService.GetMyLettersCount(UserId, Settings.CouncilLetter);

            ViewBag.OutOfReadyLetters = LetterCounts.Where(l => l.LetterStatus == 10 || l.LetterStatus == 11).Count();

            ViewBag.OutOfReadyRowNumber = skip == "0" && take == "-1" ? 1 : Convert.ToInt32(skip) + 1;
            ViewBag.User = userService.FirstOrDefault(u => u.ID == UserId);
            return PartialView("_ForCouncilLetters", model.Where(l => l.LetterStatus > 3));
        }
        public ActionResult DisplayOnlyRemovedLetters(string skip = "0", string take = "50")
        {
            var UserId = userService.GetUserByUserName(User.Identity.Name).ID;

            var model = take == "-1" ? letterService.GetMyLettersCount(UserId, Settings.RemovedLetter) : letterService.GetMyLetters(UserId, Settings.RemovedLetter, Convert.ToInt32(skip), Convert.ToInt32(take));
            var LetterCounts = letterService.GetMyLettersCount(UserId, Settings.RemovedLetter);


            ViewBag.RemovedLetters = LetterCounts.Where(l => l.Deleted).Count();

            ViewBag.RemovedRowNumber = skip == "0" && take == "-1" ? 1 : Convert.ToInt32(skip) + 1;
            ViewBag.User = userService.FirstOrDefault(u => u.ID == UserId);
            return PartialView("_RemovedLetters", model.Where(l => l.Deleted));
        }
        public ActionResult DisplayOnlyOutLetters(string skip = "0", string take = "50")
        {
            var UserId = userService.GetUserByUserName(User.Identity.Name).ID;

            var model = take == "-1" ? letterService.GetMyLettersCount(UserId, Settings.OutLetter) : letterService.GetMyLetters(UserId, Settings.OutLetter, Convert.ToInt32(skip), Convert.ToInt32(take));
            var LetterCounts = letterService.GetMyLettersCount(UserId, Settings.OutLetter);

            ViewBag.OutLetters = LetterCounts.Where(l => !String.IsNullOrEmpty(l.OutLetter_ID)).Count();
            ViewBag.OutLetterRowNumber = skip == "0" && take == "-1" ? 1 : Convert.ToInt32(skip) + 1;
            ViewBag.User = userService.FirstOrDefault(u => u.ID == UserId);
            return PartialView("_OutLetters", model.Where(l => !String.IsNullOrEmpty(l.OutLetter_ID)));
        }
        public ActionResult GetOnlyAllLetters(string skip, string take)
        {
            var UserId = userService.GetUserByUserName(User.Identity.Name).ID;

            var model = take == "-1" ? letterService.GetMyLettersCount(UserId, Settings.NormalLetter) : letterService.GetMyLetters(UserId, Settings.NormalLetter, Convert.ToInt32(skip), Convert.ToInt32(take));
            var LetterCounts = letterService.GetMyLettersCount(UserId, Settings.NormalLetter);
            ViewBag.AllLetterCount = LetterCounts.Where(l => l.Archived == false && l.Deleted == false).OrderByDescending(l => l.CreatedOn).Count();

            ViewBag.AllLetterRowNumber = skip == "0" && take == "-1" ? 1 : Convert.ToInt32(skip) + 1;
            ViewBag.User = userService.FirstOrDefault(u => u.ID == UserId);
            return PartialView("_ExtraAllLetters", model.Where(l => l.Archived == false && l.Deleted == false).OrderByDescending(l => l.CreatedOn));
        }


        #region جزییات نامه
        public ActionResult LetterItem(string letterID)
        {
            var item = letterService.Where(l => l.ID == letterID).FirstOrDefault();
            var activeUsers = item.LetterRefrences.OrderByDescending(l => l.CreatedOn).FirstOrDefault().Recivers.Where(m => m.IsActive);
            var users = activeUsers != null ? activeUsers.FirstOrDefault() : null;
            var UserId = userService.GetUserByUserName(User.Identity.Name).ID;
            letterService.ReadLetter(item.ID, UserId);
            LetterItemModel model = new LetterItemModel();
            model.Recivers = userService.AllActive();
            model.Letter = item;
            model.LastReciverID = users != null ? users.ID : "";
            model.UserIsBoss = userService.UserIsBoss(UserId);
            model.UserIsBossHelper = userService.UserIsBossHelper(UserId);
            model.Commisions = commisionService.Where(m => !m.Deleted).ToList();
            model.Statements = defaultStatementService.Where(m => !m.Deleted).ToList();
            model.LastRefrence = letterService.GetLastRefrence(letterID);
            model.UserPosition = letterService.GetUserPosition(UserId, item.ID);
            return View(model);
        }

    

        public JsonResult HasMettingContent(string letterID)
        {
            var letter = letterService.Get<string>(letterID);

            if (letter.CommissionMeeting != null)
            {
                if (letter.CommissionMeeting.Content != null)
                {
                    return Json("ok", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("error", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json("ok", JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        public ActionResult Search(string query = null)
        {
            if (query == null)
                return RedirectToAction("Index" , "Home");
            var UserId = userService.GetUserByUserName(User.Identity.Name).ID;
            var model = letterService.Search(query, UserId).ToList();
            ViewBag.Filter = query;
            return View(model);
        }

        public ActionResult AddLetterRefrence(string Transcript, string recivers, string letterID)
        {
            var UserID = userService.GetUserByUserName(User.Identity.Name).ID;
            letterService.AddLetterRefrence(recivers, UserID, Transcript, letterID, RefrenceType.Normal);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult SendToBoss(string Transcript, string letterID , string SendType)
        {
            var UserID = userService.GetUserByUserName(User.Identity.Name).ID;
            letterService.SendToBoss(Transcript, letterID, UserID, SendType);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult SendToCommision(string Transcript, string letterID, string CommisionId)
        {
            var UserID = userService.GetUserByUserName(User.Identity.Name).ID;
            letterService.SendToCommision(Transcript, letterID, CommisionId, UserID);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult SendToCommissionMemberss(string Transcript, string letterID)
        {
            var UserID = userService.GetUserByUserName(User.Identity.Name).ID;
            letterService.SendToCommissionMemberss(Transcript, letterID, UserID);
            return RedirectToAction("Index", "Home");
        }
        
        public ActionResult SendToMembers(string Transcript, string letterID)
        {
            var UserID = userService.GetUserByUserName(User.Identity.Name).ID;
            letterService.SendToMembers(Transcript, letterID, UserID);
            return RedirectToAction("Index", "Home");
        }


        public ActionResult ReadyForCouncil(string letterID)
        {
            ViewBag.UniqueNumber = uniqueNumberService.GetUniqueNumber();
            ViewBag.LetterTitle = letterService.Get<string>(letterID).Title;
            ViewBag.LetterID = letterID;
            ViewBag.Users = userService.Where(m => m.IsActive && m.IsCouncilMember).ToList();
            var session = sessionService.Where(m => m.IsActive).FirstOrDefault();
            ViewBag.SessionNumber = session != null ? session.MeetingNumber : -200;
            ViewBag.Meetings = meetingService.Where(m => m.NoCouncil).Where(m => !m.MeetingUsers.Any()).ToList();
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ReadyForCouncil(string letterID, Meeting meeting, string meetingMembers)
        {
            //meeting.HiddenVoting = true; //only for Fardis                     
            letterService.ReadyForCouncil(letterID, meeting, meetingMembers);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult CommissionReadyForCouncil(string letterID)
        {
            ViewBag.UniqueNumber = uniqueNumberService.GetUniqueNumber();
            ViewBag.LetterTitle = letterService.Get<string>(letterID).Title;
            ViewBag.LetterID = letterID;
            ViewBag.Users = letterService.GetCommissionMembers(letterID, true);
            ViewBag.SessionNumber = sessionService.Where(m => m.IsActive).FirstOrDefault().MeetingNumber;
            ViewBag.Meetings = meetingService.Where(m => m.NoCouncil).Where(m => !m.MeetingUsers.Any()).ToList();
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CommissionReadyForCouncil(string letterID, Meeting meeting, string meetingMembers)
        {
            //meeting.HiddenVoting = true; //only for Fardis
            letterService.ReadyForCommisionCouncil(letterID, meeting, meetingMembers, true);
            return RedirectToAction("Index", "Home");
        }
        public ActionResult MarkAsRemoved(string letterID)
        {
            var UserID = userService.GetUserByUserName(User.Identity.Name).ID;
            letterService.MarkAsRemoved(letterID, UserID);
            return RedirectToAction("Index", "Home");
        }
        public ActionResult MarkAsArchived(string letterID)
        {
            var UserID = userService.GetUserByUserName(User.Identity.Name).ID;
            letterService.MarkAsArchived(letterID, UserID);
            return RedirectToAction("Index", "Home");
        }
        public ActionResult MarkAsEnd(string letterID)
        {
            var UserID = userService.GetUserByUserName(User.Identity.Name).ID;
            letterService.MarkAsEndLetter(letterID, UserID);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult AllowCouncil(string letterID)
        {
            letterService.AllowCouncil(letterID);
            return RedirectToAction("Index", "Home");
        }
        public ActionResult CommissionAllowCouncil(string letterID)
        {
            letterService.CommissionAllowCouncil(letterID);
            return RedirectToAction("Index", "Home");
        }
        public ActionResult UndoOutOfReadyCouncil(string letterID)
        {
            letterService.UndoOutOfReadyForCouncil(letterID);
            return RedirectToAction("Index", "Home");
        }
        public ActionResult UndoOutOfCommisionReadyCouncil(string letterID)
        {
            letterService.UndoOutOfReadyForCommisionCouncil(letterID);
            return RedirectToAction("Index", "Home");
        }

        #region خروج از دستور جلسه
        public ActionResult ChangeOutOfReadyForCouncil(string letterID)
        {
            letterService.ChangeOutOfReadyForCouncil(letterID);
            return RedirectToAction("Index", "Home");
        }
        public ActionResult ChangeOutOfReadyForCommisionCouncil(string letterID)
        {
            letterService.ChangeOutOfReadyForCommisionCouncil(letterID);
            return RedirectToAction("Index", "Home");
        } 
        #endregion

        [CustomAuthorize(Roles = "Admin,CouncilMember")]
        public ActionResult Voting(string letterID)
        {
            var UserID = userService.GetUserByUserName(User.Identity.Name).ID;
            ViewBag.UserVoted = votingService.UserIsVoted(UserID, letterID);
            ViewBag.Letter = letterService.Get<string>(letterID);
            return View();
        }
        public ActionResult CommissionVoting(string letterID)
        {
            var UserID = userService.GetUserByUserName(User.Identity.Name).ID;
            ViewBag.UserVoted = votingService.UserIsVoted(UserID, letterID, true);
            ViewBag.Letter = letterService.Get<string>(letterID);
            return View();
        }
        public ActionResult ReVoting(string letterID)
        {
            letterService.ReVoting(letterID);
            return RedirectToAction("Index", "Home");
        }
        public ActionResult CommissionReVoting(string letterID)
        {
            letterService.ReVoting(letterID, true);
            return RedirectToAction("Index", "Home");
        }
        public ActionResult DoVoting(string comment, string votingStatus, string letterID)
        {
            var UserID = userService.GetUserByUserName(User.Identity.Name).ID;
            letterService.DoVoting(UserID, letterID, votingStatus, comment);
            return RedirectToAction("Index", "Home");
        }
        public ActionResult CommissionDoVoting(string comment, string votingStatus, string letterID)
        {
            var UserID = userService.GetUserByUserName(User.Identity.Name).ID;
            letterService.DoVoting(UserID, letterID, votingStatus, comment, true);
            return RedirectToAction("Index", "Home");
        }
        public JsonResult EndMeeting(string LetterID)
        {
            string successMessage = letterService.EndMeeting(LetterID);

            //return RedirectToAction("Index", "Home");
            return Json("ok", JsonRequestBehavior.AllowGet);
        }
        public JsonResult CheckEndMeeting(string LetterID)
        {
            List<User> notVotedUsers = new List<User>();
            string isCommision = letterService.Get<string>(LetterID).LetterStatus == LetterStatus.CommissionAllowVotting ? "1" : "0";
            if (letterService.CheckAllUserVoted(LetterID))
            {
                return Json(new { message = "End", notVoted = "null", isCommission = isCommision }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                notVotedUsers = letterService.GetAllUserNotVoted(LetterID);
                return Json(new { message = "notEnd", notVoted = notVotedUsers, isCommission = isCommision }, JsonRequestBehavior.AllowGet);
            }
            // return RedirectToAction("Index", "Home");
        }
        public JsonResult CommissionEndMeeting(string LetterID)
        {
            letterService.EndMeeting(LetterID, true);
            return Json("ok", JsonRequestBehavior.AllowGet);
        }
        public ActionResult MeetingResult(string letterID, bool isCommission = false)
        {
            ViewBag.Votings = votingService.GetVotingOfLetter(letterID, isCommission);
            ViewBag.Letter = letterService.Get<string>(letterID);
            ViewBag.IsVotingHidden = letterService.IsVotingHidden(letterID);
            ViewBag.IsCommission = isCommission ? "IsCommission" : "Normal";
            return View();
        }
        public ActionResult UndoRemoved(string LetterID)
        {
            var UserID = userService.GetUserByUserName(User.Identity.Name).ID;
            letterService.UndoRemove(LetterID, UserID);
            return RedirectToAction("Index", "Home");
        }
        public JsonResult SaveVoting(string LetterID, string users, string votValue)
        {
            bool isCommision = letterService.Get<string>(LetterID).LetterStatus == LetterStatus.CommissionAllowVotting ? true : false;
            List<User> notVotedUsers = Newtonsoft.Json.JsonConvert.DeserializeObject<List<User>>(users);
            try
            {
                foreach (var item in notVotedUsers)
                {
                    if (isCommision)
                        letterService.DoVoting(item.ID, LetterID, votValue, "", true);
                    else
                        letterService.DoVoting(item.ID, LetterID, votValue, "");
                }
                return Json("ok", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }


            //return RedirectToAction("Index", "Home");
        }
        public ActionResult AddNote(string letterId, string Note)
        {
            NoteService noteService = new NoteService();
            var userId = userService.GetUserByUserName(User.Identity.Name).ID;
            Note note = new Note { WriterId = userId, ContentText = Note };
            noteService.Create(note);
            var Letter = letterService.Get<string>(letterId);
            if (Letter.OutLetter == null)
            {
                Letter.Notes.Add(note);
            }
            else
            {
                Letter.OutLetter.Notes.Add(note);
            }
            letterService.Update(Letter);
            return RedirectToAction("Index" , "Home");
        }
        public JsonResult GetletterNote(string letterId)
        {
            var notes = letterService.Get<string>(letterId).Notes.OrderByDescending(m => m.CreatedOn).ToList();
            string result = PublicHelpers.RenderRazorViewToString(this, "_NoteLetter", notes);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CheckDeliverLetter(string letterId)
        {
            var letter = letterService.Get<string>(letterId);
            var letterIsNew = letter.LetterRefrences.Any(n => n.LetterStatuses.Any(m => !m.IsNew));
            return Json(letterIsNew ? "false" : "true", JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteLetter(string letterId)
        {
            letterService.Delete(letterId);
            return Json("true", JsonRequestBehavior.AllowGet);
        }

    }
}