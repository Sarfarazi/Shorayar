using Council.Core.Entities;
using Council.Service.DBServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Council.Core.Models;
using Council.UI.Helpers;
using System.Threading;
using Council.Core.Statics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Council.Core.Enums;
using Council.UI.CustomAuthentication;

namespace Council.UI.Controllers
{
    [CustomAuthorize(Roles = "Admin,Manager,Writer1,Writer2")]
    public class MeetingController : Controller
    {
        UniqueNumberServices uniqueNumberService = new UniqueNumberServices();

        UserServices userService = new UserServices();
        LetterServices letterService = new LetterServices();
        MeetingServices meetingService = new MeetingServices();
        SessionServices meetingHeaderService = new SessionServices();
        DefaultStatementService defaultStatementService = new DefaultStatementService();
        SessionServices sessionService = new SessionServices();
        CommissionServices commissionService = new CommissionServices();
        SystemSettingsService settingService = new SystemSettingsService();
        PeriodService PeriodService = new PeriodService();


        public MeetingController()
        {

        }
        public ActionResult Index()
        {
            return View();
        }

        #region ثبت صورت جلسه شورا
        public ActionResult CreateMeeting(int? SessionNumber)
        {
            //CustomPrincipal _User = (CustomPrincipal)HttpContext.User;
            //var userId = _User.UserId;

            //var session = SessionNumber != null ? sessionService.Where(m => m.MeetingNumber == SessionNumber).FirstOrDefault() : null;

            //ViewBag.Sessions = sessionService.All().ToList().Where(m => m.Meetings.Any() && m.Meetings.All(p => p.Content != null)).OrderBy(m => m.MeetingNumber).ToList();
            //ViewBag.MeetingNumber = SessionNumber == null ? "" : session.MeetingNumber.ToString();
            //ViewBag.Content = SessionNumber == null ? "" : session.Content;
            //var model = sessionService.Where(m => m.Content == null).ToList();
            //ViewBag.User = userService.Where(m => m.ID == userId).FirstOrDefault();
            //return View(model);


            CustomPrincipal _User = (CustomPrincipal)HttpContext.User;
            var userId = _User.UserId;

            var session = SessionNumber != null ? sessionService.Where(m => m.MeetingNumber == SessionNumber).FirstOrDefault() : sessionService.Where(m => m.IsActive).FirstOrDefault();

            ViewBag.Sessions = sessionService.All().ToList().Where(m => m.Meetings.Any()).OrderBy(m => m.MeetingNumber).ToList();
            ViewBag.MeetingNumber = session == null ? "" : session.MeetingNumber.ToString();
            ViewBag.Content = SessionNumber == null ? "" : session.Content;
            var model = sessionService.Where(m => m.Content == null).ToList();
            ViewBag.User = userService.Where(m => m.ID == userId).FirstOrDefault();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult CreateMeeting(int MeetingNumber, string Content)
        {
            var session = sessionService.Where(m => m.MeetingNumber == MeetingNumber).FirstOrDefault();
            session.Content = Content;
            sessionService.DeActiveAllSession();
            return RedirectToAction("CreateMeeting");
        }
        #endregion


        #region صدور دعوت نامه
        [CustomAuthorize(Roles = "Admin,Writer1,Writer2,Manager")]
        public ActionResult CreateInviteLetter()
        {
            CustomPrincipal _User = (CustomPrincipal)HttpContext.User;
            string userId = _User.UserId;

            var session = sessionService.Where(m => m.IsActive).FirstOrDefault();
            if (session != null)
            {
                if (session.SessionStatus == SessionStatus.Created)
                {
                    var activeSession = session != null ? session.MeetingNumber : -200;
                    var homePageService = new HomePageService(userId);
                    var myCommision = commissionService.Where(c => c.CommissionChairman.ID == userId
                                         || c.Members.Any(m => m.ID == userId))
                                        .Select(c => new SessionCommissionAndBossIds { BossId = c.CommissionChairman.ID, CommissionId = c.ID, CommisionName = c.Name }).ToList();
                    var MyLetters = homePageService.GetMyLetters();
                    var model = new SessionViewModel()
                    {
                        UserIsBoss = userService.UserIsBoss(userId),
                        UserIsBossHelper = userService.UserIsBossHelper(userId),
                        UserIsMember = userService.UserIsMember(userId),
                        UserIsSiteManager = userService.UserIsSiteManager(userId),
                        MyLetters = MyLetters,
                        MyCommissions = myCommision
                    };
                    ViewBag.Letters = model;
                    ViewBag.ActiveSession = activeSession.ToString();
                    ViewBag.Message = "";
                    //session.SessionStatus = SessionStatus.SendInviteLetter;
                    //sessionService.Update(session);
                }
                else if (session.SessionStatus == SessionStatus.SendInviteLetter)
                {
                    ViewBag.Letters = null;
                    ViewBag.ActiveSession = "";
                    ViewBag.Message = "برای این جلسه دعوتنامه صادر شده است";
                }
                else if (session.SessionStatus == SessionStatus.Dispose)
                {
                    ViewBag.Letters = null;
                    ViewBag.ActiveSession = "";
                    ViewBag.Message = "جلسه اختتام شده. لطفا جلسه جدید را ایجاد کنید";
                }
            }
            else
            {
                ViewBag.Letters = null;
                ViewBag.ActiveSession = "";
                ViewBag.Message = "جلسه فعال در سیستم وجود ندارد";
            }
            return View();
        }
        #endregion

        #region ایجاد جلسه
        [CustomAuthorize(Roles = "Admin,Writer1,Writer2,Manager")]
        public ActionResult CreateSession()
        {
            ViewBag.DuplicateSession = 0;
            ViewBag.Sessions = meetingHeaderService.All().OrderByDescending(m => m.CreatedOn).ToList();
            return View();
        }

        [CustomAuthorize(Roles = "Admin,Writer1,Writer2,Manager")]
        [HttpPost]
        public ActionResult CreateSession(MeetingHeader meetingHeader)
        {
            if (!sessionService.All().Any(m => m.IsActive))
            {

                //if (!sessionService.All().Any(m => m.MeetingNumber == meetingHeader.MeetingNumber))
                //{
                //    meetingHeader.IsActive = true;
                //    sessionService.Create(meetingHeader);
                //}
                //else
                //{
                //    ViewBag.DuplicateSession = 1;
                //}
                meetingHeader.IsActive = true;
                var Period = PeriodService.Where(m => m.IsActive == true).FirstOrDefault();
                meetingHeader.Code= sessionService.GetUniqueNumberForSession(Period.ID);
                // string MeetingNumber = Period.Code + meetingHeader.MeetingOwner + meetingHeader.Code;
                 string MeetingNumber = meetingHeader.Code; 
                meetingHeader.MeetingNumber = Convert.ToInt32(MeetingNumber);
                meetingHeader.CouncilPeriodsID = Period.ID;
                
                sessionService.Create(meetingHeader);
            }
            else
            {
                ViewBag.DuplicateSession = 2;
            }

            ViewBag.Sessions = meetingHeaderService.All().OrderByDescending(m => m.CreatedOn).ToList();
            return View();
        }
        #endregion

        public ActionResult FinalMeetingPrint(string SessionNumber)
        {
            var Setting = settingService.FirstOrDefault(m => m.Used);
            string CouncilName = Setting != null ? Setting.CouncilName : null;
            ViewBag.Setting = Setting;

            var meetings = new List<Meeting>();

            var signersList = new List<SignersUser>();
            var LetterMeetingId = new List<MeetingIdWithLetterId>();
            SignersUser signeruser = null;

            var boss = userService.Where(m => m.IsCouncilBoss && m.IsActive).FirstOrDefault();
            signeruser = new SignersUser { Name = boss.FirstName + ' ' + boss.LastName, Signature = boss.Signature, UserPosition = (int)Council.Core.Enums.UserPosition.CouncilBoss, Gender = !boss.Gender ? "آقا" : "خانم" };
            signersList.Add(signeruser);

            var helper = userService.Where(m => m.BossHelper && m.IsActive).FirstOrDefault();
            if (helper != null)
            {
                signeruser = new SignersUser { Name = helper.FirstName + ' ' + helper.LastName, Signature = helper.Signature, UserPosition = (int)Council.Core.Enums.UserPosition.CouncilHelper, Gender = !helper.Gender ? "آقا" : "خانم" };
                signersList.Add(signeruser);
            }            
            
            var writer1 = userService.Where(m => m.IsWriter1 && m.IsActive).FirstOrDefault();
            if (writer1 != null)
            {
                signeruser = new SignersUser { Name = writer1.FirstName + ' ' + writer1.LastName, Signature = writer1.Signature, UserPosition = (int)Council.Core.Enums.UserPosition.CouncilWriter1, Gender = !writer1.Gender ? "آقا" : "خانم" };
                signersList.Add(signeruser); 
            }

            var writer2 = userService.Where(m => m.IsWriter2 && m.IsActive).FirstOrDefault();
            if (writer2 != null)
            {
                signeruser = new SignersUser { Name = writer2.FirstName + ' ' + writer2.LastName, Signature = writer2.Signature, UserPosition = (int)Council.Core.Enums.UserPosition.CouncilWriter2, Gender = !writer2.Gender ? "آقا" : "خانم" };
                signersList.Add(signeruser); 
            }

            var session = sessionService.Where(m => m.MeetingNumber.ToString() == SessionNumber).FirstOrDefault();
            meetings = meetingService.Where(m => m.MeetingNumber == SessionNumber).ToList();
            if (meetings != null && meetings.Count() > 0)
            {
                foreach (var item in meetings)
                {
                    var letter = letterService.Where(m => m.Meeting.ID == item.ID).FirstOrDefault();
                    if (letter != null)
                    {
                        LetterMeetingId.Add(new MeetingIdWithLetterId { LetterId = letter.ID, LetterTitle = letter.Title, MeetingId = item.ID });
                    }                    
                }
            }
            var model = new FinalMeetingPrint
            {
                SessionNumber = session.MeetingNumber.ToString(),
                SessionContent = session.Content,
                RegisterDate = session.RegisterDate,
                StartTime = session.StartTime,
                EndTime = session.EndTime,
                SessionType = session.MeetingType == 1 ? "عادی" : "فوق العاده",
                Signers = signersList,
                Meetings = meetings,
                LettersSubject = LetterMeetingId
            };

            return View(model);
        }

        [HttpPost]
        public JsonResult DeActiveAllSession()
        {
            try
            {
                sessionService.DeActiveAllSession();
                return Json("true", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("false", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CheckHasContent(string meetingNumber)
        {
            int number = Convert.ToInt32(meetingNumber);
            var session = sessionService.Where(m => m.MeetingNumber == number).FirstOrDefault();
            if (session != null && session.Content != null)
            {
                return Json("yes", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("no", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult MeetingDetails(string meetingID)
        {
            var model = meetingService.Get<string>(meetingID);
            ViewBag.UserStatuses = Newtonsoft.Json.JsonConvert.SerializeObject(model.MeetingUsers.Where(m => !m.Deleted).Select(m => new { m.status, m.User.FirstName, m.User.LastName }));
            return View(model);
        }
        public ActionResult Search(string query = null)
        {
            if (query == null)
                return RedirectToAction("Index");
            //var appId = User.Identity.GetUserId();
            //var model2 = meetingService.Search(query);
            var model = meetingService.Search(query.Trim()).ToList();
            ViewBag.Filter = query;
            return View(model);
        }

        #region ارسال دعوت نامه برای اعضا
        [HttpPost]
        public ActionResult InvitePrint(string letterIds, string sessionId)
        {
            ChangeLetterStatusForReady(letterIds);
            var result = GetInviteModel(letterIds, sessionId);
            return View(result);
        }
        #endregion

        #region تغییر وضعیت نامه
        private void ChangeLetterStatusForReady(string letterIds)
        {
            List<string> letters = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(letterIds);
            foreach (var id in letters)
            {
                var item = letterService.Get<string>(id);
                item.LetterStatus = LetterStatus.Votting;
                letterService.Update(item);
            }
        } 
        #endregion

        #region دریافت اطلاعات نامه برای چاپ
        private InvitePrintModel GetInviteModel(string letterIds, string sessionId)
        {
            List<string> letters = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(letterIds);
            List<MemberGender> members = new List<MemberGender>();
            var councilMembers = userService.Where(n => n.IsActive).Where(u => u.IsCouncilMember).ToList();
            foreach (var item in councilMembers)
            {
                members.Add(new MemberGender { Name = item.FirstName + ' ' + item.LastName, IsFemale = item.Gender ? true : false });
            }

            //یافتن کاربری که نقش دبیر جلسه 1 را دارد
            //var user = userService.Where(m => m.IsWriter1 && m.IsActive).FirstOrDefault();

            CustomPrincipal Currentuser = (CustomPrincipal)HttpContext.User;
            var user = userService.Where(m => m.ID == Currentuser.UserId).FirstOrDefault();

            var session = sessionService.Where(m => m.MeetingNumber.ToString() == sessionId).FirstOrDefault();
            InvitePrintModel printItem = new InvitePrintModel();
            printItem.SessionDate = session.RegisterDate;
            printItem.SesionFromTime = session.StartTime;
            printItem.SessionToTime = session.EndTime;
            printItem.Writer1 = new User();
            printItem.Writer1 = user;
            printItem.Members = new List<MemberGender>();
            printItem.Members = members;
            printItem.Meeting = new List<MeetingTitleItems>();
            foreach (var letter in letters)
            {
                var item = letterService.Get<string>(letter);
                item.LetterStatus = LetterStatus.SendInvite;
                letterService.Update(item);
                printItem.Meeting.Add(new MeetingTitleItems { Title = item.Title, IsCommision = letterService.LetterIsCommision(item), CommisionName = letterService.GetLetterCommisionName(item) });
            }
            return printItem;
        } 
        #endregion

        #region ارسال دعوتنامه از طریق sms
        public async Task<JsonResult> InviteSMS()
        {
            SmsServices smsService = new SmsServices();
            await smsService.SendSMS();
            //Thread trdSms = new Thread(new ThreadStart(smsService.SendSMS));
            return Json("ok", JsonRequestBehavior.AllowGet);
        } 
        #endregion


        #region ارسال الکترونیکی دعوتنامه
        public JsonResult InviteEmail(string letterIds, string sessionId)
        {
            List<string> letters = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(letterIds);
            string content = "";
            content += HttpUtility.HtmlEncode("با سلام ");
            var session = sessionService.Where(m => m.MeetingNumber.ToString() == sessionId).FirstOrDefault();
            var councilMembers = userService.Where(n => n.IsActive).Where(u => u.IsCouncilMember).ToList();
            var letterNumber = uniqueNumberService.GetUniqueNumber();
            var Setting = settingService.FirstOrDefault(m => m.Used);
            string CouncilName = Setting != null ? Setting.CouncilName : null;

            foreach (var councilMember in councilMembers)
            {
                MemberGender member = new MemberGender { Name = councilMember.FirstName + ' ' + councilMember.LastName, IsFemale = councilMember.Gender ? true : false };
                var txtGender = member.IsFemale ? "سرکار خانم" : "جناب آقای ";
                txtGender += member.Name + " ";
                content += HttpUtility.HtmlEncode(txtGender);

                var text = string.Format("از شما دعوت می شود در جلسه به شماره {0} که در تاریخ {1} و از ساعت {2} تا ساعت {3}  برگزار می شود شرکت فرمایید . در ذیل دستورات جلسات خدمتتان ارائه می شود ", session.MeetingNumber, session.RegisterDate, session.StartTime, session.EndTime);
                content += HttpUtility.HtmlEncode(text + " ");
                int counter = 1;
                foreach (var letter in letters)
                {
                    var item = letterService.Get<string>(letter);
                    var txt = string.Format("{0}: {1} ", counter++, item.Title);
                    content += HttpUtility.HtmlEncode(txt);
                }
                content += HttpUtility.HtmlEncode(string.Format("با تشکر شورای اسلامی شهر {0} ", CouncilName));
                SendInviteLetter(councilMembers, content);
            }
            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        public void SendInviteLetter(List<User> users, string content)
        {
            var UserID = userService.GetUserByUserName(User.Identity.Name).ID;

            var letter = new Letter();
            letter.LetterNumber = uniqueNumberService.GetUniqueNumber();
            letter.Content = content;

            letter.Title = string.Format("دعوتنامه برای شورای اسلامی شهر {0}", Settings.CityName);
            List<Reciver> members = new List<Reciver>();
            foreach (var user in users)
            {
                members.Add(new Reciver { ID = user.ID, Status = "1" });
            }
            foreach (var user in users)
            {
                string letterID = letterService.CreateLetter(letter, null, JsonConvert.SerializeObject(members), UserID, null, RefrenceType.Normal);
            }
        }
        #endregion


        #region ثبت نهایی
        public ActionResult FinalApproved(string id)
        {
            Final_Approved model = new Final_Approved
            {
                MeettingID = id
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FinalApproved(Final_Approved model)
        {
            if (ModelState.IsValid)
            {
                var meetingHeader = meetingHeaderService.Where(m => m.ID == model.MeettingID).FirstOrDefault();

                meetingHeader.FinalApproved = true;
                meetingHeaderService.Update(meetingHeader);

                return RedirectToAction("CreateMeeting");
            }

            return View(model);
        }
        #endregion
    }
}