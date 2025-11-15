using Council.Service.DBServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Council.Core.Entities;
using Council.Core.Extensions;
using Council.UI.Helpers;
using System.Xml;
using System.Drawing;
using System.ComponentModel;
using System.Xml.Linq;
using System.Data;
using System.IO;
using Council.Service;
using Council.Core.Enums;
using Council.UI.CustomAuthentication;
using System.Drawing.Imaging;
using iTextSharp.text;
using System.Drawing.Drawing2D;
using Council.Core.Models;
using System.Web.Hosting;

namespace Council.UI.Controllers
{
    //[CustomAuthorize(Users = "Admin")]
    public class OutLetterController : Controller
    {
        OutLetterServices outLetterServices = new OutLetterServices();
        UserServices userServices = new UserServices();
        OrganService organService = new OrganService();
        LetterServices letterServices = new LetterServices();
        UniqueNumberServices uniqueNumberServices = new UniqueNumberServices();
        VotingService votingService = new VotingService();
        LetterServices letterService = new LetterServices();
        UserServices userService = new UserServices();
        PublicMetods publicMethods = new PublicMetods();
        SystemSettingsService CurrtenSettingService = new SystemSettingsService();
        CopyToLetterService copyToLetterService = new CopyToLetterService();
        SystemSettingsService settingsService = new SystemSettingsService();
        public ActionResult Index()
        {
            return View();
        }

        #region تبدیل تصاویر اسکن شده به pdf
        public ActionResult ToPdf()
        {
            if (Request.Form.Count > 0)
            {
                var blob = Request.Form["blob"];
                int index = int.Parse(Request.Form["index"]);
                var name = Request.Form["name"];
                int allCount = int.Parse(Request.Form["allCount"]);

                string ScPath = HttpContext.Server.MapPath("~/ScTemp");

                //blob = blob.Replace("data:application/octet-stream;base64,", "");
                blob = blob.Replace("data:image/jpeg;base64,", "");
                byte[] bytes = Convert.FromBase64String(blob);

                using (var ms = new MemoryStream(bytes))
                {
                    Bitmap bmp = new Bitmap(System.Drawing.Image.FromStream(ms, true));
                    //SaveImage(bmp, 1200, 1200, 80, string.Format("{0}/IMG-{1}-{2}.jpg", ScPath, name, index));
                    bmp.Save(string.Format("{0}/IMG-{1}-{2}.jpg", ScPath, name, index), ImageFormat.Jpeg);
                }

                if (index == allCount)
                {
                    string pattern = string.Format("IMG-{0}-*.jpg", name);

                    var filePaths = Directory.GetFiles(ScPath, pattern);

                    if (filePaths != null)
                    {
                        using (var doc = new iTextSharp.text.Document(PageSize.A4, 10f, 10f, 10f, 10f))
                        {
                            iTextSharp.text.pdf.PdfWriter.GetInstance(doc, new FileStream(string.Format("{0}\\{1}.pdf", ScPath, name), FileMode.Create));
                            doc.Open();
                            foreach (var item in filePaths)
                            {
                                iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(item);
                                jpg.ScaleToFit(PageSize.A4);
                                jpg.SpacingBefore = 0;
                                jpg.SpacingAfter = 0;
                                jpg.Alignment = Element.ALIGN_CENTER;
                                doc.Add(jpg);
                            }
                        }

                        var pdfPath = string.Format("{0}\\{1}.pdf", ScPath, name);

                        if (System.IO.File.Exists(pdfPath)) {
                            foreach (var item in filePaths)
                            {
                                if (System.IO.File.Exists(item))
                                {
                                    System.IO.File.Delete(item);
                                }
                            }
                        }

                        return Json(new { pdfname = string.Format("{0}.pdf", name)}, JsonRequestBehavior.AllowGet);
                    }
                }
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        public void SaveImage(Bitmap image, int maxWidth, int maxHeight, int quality, string filePath)
        {
            // Get the image's original width and height
            int originalWidth = image.Width;
            int originalHeight = image.Height;

            // To preserve the aspect ratio
            float ratioX = (float)maxWidth / (float)originalWidth;
            float ratioY = (float)maxHeight / (float)originalHeight;
            float ratio = Math.Min(ratioX, ratioY);

            // New width and height based on aspect ratio
            int newWidth = (int)(originalWidth * ratio);
            int newHeight = (int)(originalHeight * ratio);

            // Convert other formats (including CMYK) to RGB.
            Bitmap newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);

            // Draws the image in the specified size with quality mode set to HighQuality
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            // Get an ImageCodecInfo object that represents the JPEG codec.
            ImageCodecInfo imageCodecInfo = this.GetEncoderInfo(ImageFormat.Jpeg);

            // Create an Encoder object for the Quality parameter.
            Encoder encoder = Encoder.Quality;

            // Create an EncoderParameters object. 
            EncoderParameters encoderParameters = new EncoderParameters(1);

            // Save the image as a JPEG file with quality level.
            EncoderParameter encoderParameter = new EncoderParameter(encoder, quality);
            encoderParameters.Param[0] = encoderParameter;
            newImage.Save(filePath, imageCodecInfo, encoderParameters);
        }

        private ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().SingleOrDefault(c => c.FormatID == format.Guid);
        }
        #endregion

        #region OutLetters
        public ActionResult Search(string filter)
        {
            var model = outLetterServices.Search(filter).ToList();
            return View(model);
        }
        public ActionResult SearchLawLetter(string filter)
        {
            var model = outLetterServices.SearchLaws(filter).ToList();
            return View(model);
        }
        public ActionResult SearchPlanLetter(string filter)
        {
            var result = letterService.GetLawsPlansLetters(-1, -1);
            if (result != null)
            {
                var listmodel = result.Where(m => m.Title.Contains(filter)).ToList();
                return View(listmodel);
            }
            else
            {
                return View();
            }
        }

        public ActionResult SearchOrganLetters(int skip, int take, string organId)
        {
            var result = new { recieved = ExtraRecivedOutLetters(skip, take, organId), sended = ExtraSendedOutLetters(skip, take, organId), archived = ExtraArchivedOutLetters(skip, take, organId), removed = ExtraRemovedOutLetters(skip, take, organId) };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchLawOrganLetters(int skip, int take, string organId)
        {
            var result = ExtraLawOutLetters(skip, take, organId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchPlanOrganLetters(string organId)
        {
            var result = letterService.GetLawsPlansLetters(-1, -1);
            if (result != null)
            {
                var listmodel = result.Where(m => m.OrganId == organId).ToList();
                ViewBag.PlanSearchPageNumber = 1;
                ViewBag.LetterSearchCount = listmodel.Count();
                string render = PublicHelpers.RenderRazorViewToString(this, "_SearchPlanOrganLetter", listmodel);
                return Json(render, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        #region نمایش و مدیریت مکاتبات سازمانی
        [CustomAuthorize(Roles = "Admin,Manager")]
        public ActionResult ShowOutLetters()
        {
            //var model = outLetterServices.All().Where(o => !o.Deleted).OrderByDescending(o =>o.RegisterNumber).Take(50).ToList();
            ViewBag.Organs = organService.All().ToList();
            return View();
        }

        #region نامه های دریافتی
        [CustomAuthorize(Roles = "Admin,Manager")]
        public ActionResult ShowRecivedOutLetters(int skip, int take)
        {
            var model = outLetterServices.All().Where(m => m.CouncilPeriod.IsActive).Where(o => !o.Deleted)
                                               .Where(l => l.Received && !l.Archived && !l.Removed).OrderByDescending(o => o.CreatedOn)
                                               .Skip(skip).Take(take).ToList();
            ViewBag.RecievedOutLetterCount = outLetterServices.All().Where(m => m.CouncilPeriod.IsActive).Where(o => !o.Deleted).Where(l => l.Received && !l.Archived && !l.Removed).Count();
            ViewBag.RecivedOutLettersPageNumber = skip;
            return PartialView("_RecievedOutletter", model);
        }
        #endregion

        #region نامه های ارسالی
        [CustomAuthorize(Roles = "Admin,Manager")]
        public ActionResult ShowSendedOutLetters(int skip, int take)
        {
            var model = outLetterServices.All().Where(m => m.CouncilPeriod.IsActive).Where(o => !o.Deleted)
                                               .Where(l => !l.Received && !l.Archived && !l.Removed).OrderByDescending(o => o.CreatedOn)
                                               .Skip(skip).Take(take).ToList();
            ViewBag.SendedOutLetterCount = outLetterServices.All().Where(m => m.CouncilPeriod.IsActive).Where(o => !o.Deleted).Where(l => !l.Received && !l.Archived && !l.Removed).Count();
            ViewBag.SendedOutLettersPageNumber = skip;
            return PartialView("_SendedOutLetter", model);
        }

        public ActionResult SendLetterItem(string letterID)
        {
            ViewBag.Organs = organService.All().Where(m => m.Deleted == false).ToList();
            var item = outLetterServices.All().Where(l => l.ID == letterID).FirstOrDefault();
            // var activeUsers = item.LetterRefrences.OrderByDescending(l => l.CreatedOn).FirstOrDefault().Recivers.Where(m => m.IsActive);
            // var users = activeUsers != null ? activeUsers.FirstOrDefault() : null;
            var UserId = userService.GetUserByUserName(User.Identity.Name).ID;
            // letterService.ReadLetter(item.ID, UserId);
            OutLetterItemModel model = new OutLetterItemModel();
            //  model.Recivers = userService.AllActive();
            model.Letter = item;
            //  model.LastReciverID = users != null ? users.ID : "";
            // model.UserIsBoss = userService.UserIsBoss(UserId);
            // model.UserIsBossHelper = userService.UserIsBossHelper(UserId);
            //  model.Commisions = commisionService.All().Where(m => !m.Deleted).ToList();
            //  model.Statements = defaultStatementService.All().Where(m => !m.Deleted).ToList();
            // model.LastRefrence = letterService.GetLastRefrence(letterID);
            //  model.UserPosition = letterService.GetUserPosition(UserId, item.ID);
            return View(model);
        }


        #endregion

        #region نامه های بایگانی شده
        [CustomAuthorize(Roles = "Admin,Manager")]
        public ActionResult ShowArchivedOutLetters(int skip, int take)
        {
            var model = outLetterServices.All().Where(m => m.CouncilPeriod.IsActive).Where(o => !o.Deleted)
                                               .Where(l => l.Archived && !l.Removed).OrderByDescending(o => o.CreatedOn)
                                               .Skip(skip).Take(take).ToList();
            ViewBag.ArchivedOutLetterCount = outLetterServices.All().Where(m => m.CouncilPeriod.IsActive).Where(o => !o.Deleted).Where(l => l.Archived && !l.Removed).Count();
            ViewBag.ArchivedOutLettersPageNumber = skip;
            return PartialView("_ArchivedOutLetter", model);
        }
        #endregion

        #region نامه های حذف شده
        [CustomAuthorize(Roles = "Admin,Manager")]
        public ActionResult ShowRemovedOutLetters(int skip, int take)
        {
            var model = outLetterServices.All().Where(m => m.CouncilPeriod.IsActive)
                                               .Where(o => o.Deleted)
                                               .OrderByDescending(o => o.CreatedOn)
                                               .Skip(skip).Take(take).ToList();
            ViewBag.RemovedOutLetterCount = outLetterServices.All().Where(m => m.CouncilPeriod.IsActive).Where(o => o.Deleted).Count();
            ViewBag.RemovedOutLettersPageNumber = skip;
            return PartialView("_RemovedOutLetter", model);
        }
        #endregion
        #endregion

        #region لوایح/طرح ها
        [CustomAuthorize(Roles = "Admin,Boss,BossHelper, Writer1,Writer2,Manager,CouncilMember,Guest")]
        public ActionResult ShowPlanLawsLetters()
        {
            ViewBag.PlanLawsLetterCount = letterService.GetLawsPlansLetters(-1, -1).Count();
            ViewBag.Organs = organService.All().ToList();
            return View();
        }
        #endregion

        [CustomAuthorize(Roles = "Admin,Boss,BossHelper, Writer1,Writer2,Manager,CouncilMember,Guest")]
        public ActionResult GetPlanLawsLetters(int skip, int take)
        {
            var model = letterService.GetLawsPlansLetters(skip, take);
            ViewBag.LetterCount = model.Count();
            ViewBag.LettersPageNumber = skip;
            return PartialView("_PlanLawsOutletters", model);
        }

        public ActionResult PlanOutLetters(int skip, int take)
        {
            var model = outLetterServices.All().Where(m => m.CouncilPeriod.IsActive).Where(o => !o.Deleted)
                                               .Where(l => l.Received && l.OutLetterStatus == 2 && !l.Archived && !l.Removed).OrderByDescending(o => o.CreatedOn)
                                               .Skip(skip).Take(take).ToList();
            ViewBag.PlanOutLetterCount = outLetterServices.All().Where(m => m.CouncilPeriod.IsActive).Where(o => !o.Deleted).Where(l => !l.Received && l.OutLetterStatus == 2 && !l.Archived && !l.Removed).Count();
            ViewBag.PlanOutLettersPageNumber = skip;

            return PartialView("_PlanOutletter", model);
        }
        public ActionResult LawOutLetters(int skip, int take)
        {
            var model = outLetterServices.All().Where(m => m.CouncilPeriod.IsActive).Where(o => !o.Deleted)
                                               .Where(l => l.Received && !l.Archived && !l.Removed).OrderByDescending(o => o.CreatedOn)
                                               .Where(l => l.OutLetterStatus >= 4 && l.OutLetterStatus <= 6)
                                               .Skip(skip).Take(take).ToList();
            ViewBag.LawOutLetterCount = outLetterServices.All().Where(m => m.CouncilPeriod.IsActive)
                                        .Where(o => !o.Deleted).Where(l => l.Received & !l.Archived && !l.Removed)
                                        .Where(l => l.OutLetterStatus >= 4 && l.OutLetterStatus <= 6).Count();
            ViewBag.LawOutLettersPageNumber = skip;

            return PartialView("_LawOutletter", model);
        }

        [CustomAuthorize(Roles = "Admin,Boss,BossHelper, Writer1,Writer2,Manager,CouncilMember,Guest")]
        public ActionResult ExtraRemovedOutLetters(int skip, int take, string organId = "")
        {
            var model = organId == "" ? outLetterServices.All().Where(m => m.CouncilPeriod.IsActive).Where(o => o.Deleted)
                                               .OrderByDescending(o => o.CreatedOn)
                                               .Skip(skip).Take(take).ToList()
                                   : outLetterServices.All()
                                               .Where(m => m.Organ.ID == organId)
                                               .Where(m => m.CouncilPeriod.IsActive)
                                               .Where(o => o.Deleted)
                                               .OrderByDescending(o => o.CreatedOn)
                                               .Skip(skip).Take(take).ToList();
            ViewBag.RemovedOutLetterCount = organId == "" ? outLetterServices.All().Where(m => m.CouncilPeriod.IsActive).Where(o => o.Deleted).Count() :
                                                           outLetterServices.All().Where(m => m.CouncilPeriod.IsActive).Where(p => p.Organ.ID == organId).Where(o => o.Deleted).Count();
            ViewBag.RemovedOutLettersPageNumber = skip;
            string result = PublicHelpers.RenderRazorViewToString(this, "_RemovedOutLetter", model);
            return Json(result);
        }

        [CustomAuthorize(Roles = "Admin,Manager")]
        public ActionResult ExtraRecivedOutLetters(int skip, int take, string organId = "")
        {
            var model = organId == "" ? outLetterServices.All().Where(m => m.CouncilPeriod.IsActive).Where(o => !o.Deleted).Where(l => l.Received && !l.Archived && !l.Removed).OrderByDescending(o => o.CreatedOn).Skip(skip).Take(take).ToList()
                                   : outLetterServices.All()
                                               .Where(m => m.Organ.ID == organId)
                                               .Where(m => m.CouncilPeriod.IsActive).Where(o => !o.Deleted)
                                               .Where(l => l.Received && !l.Archived && !l.Removed).OrderByDescending(o => o.CreatedOn)
                                               .Skip(skip).Take(take).ToList();
            ViewBag.RecievedOutLetterCount = organId == "" ? outLetterServices.All().Where(m => m.CouncilPeriod.IsActive).Where(o => !o.Deleted).Where(l => l.Received && !l.Archived && !l.Removed).Count()
                                                       : outLetterServices.All().Where(m => m.CouncilPeriod.IsActive).Where(p => p.Organ.ID == organId).Where(o => !o.Deleted).Where(l => l.Received && !l.Archived && !l.Removed).Count();
            ViewBag.RecivedOutLettersPageNumber = skip;
            string result = PublicHelpers.RenderRazorViewToString(this, "_RecievedOutletter", model);
            return Json(result);

        }


        [CustomAuthorize(Roles = "Admin,Manager")]
        public ActionResult ExtraSendedOutLetters(int skip, int take, string organId = "")
        {
            var model = organId == "" ? outLetterServices.All().Where(m => m.CouncilPeriod.IsActive).Where(o => !o.Deleted)
                                               .Where(l => !l.Received && !l.Archived && !l.Removed).OrderByDescending(o => o.CreatedOn)
                                               .Skip(skip).Take(take).ToList()
                                       : outLetterServices.All()
                                               .Where(m => m.Organ.ID == organId)
                                               .Where(m => m.CouncilPeriod.IsActive).Where(o => !o.Deleted)
                                               .Where(l => !l.Received && !l.Archived && !l.Removed).OrderByDescending(o => o.CreatedOn)
                                               .Skip(skip).Take(take).ToList();
            ViewBag.SendedOutLetterCount = organId == "" ? outLetterServices.All().Where(m => m.CouncilPeriod.IsActive).Where(o => !o.Deleted).Where(l => !l.Received && !l.Archived && !l.Removed).Count()
                                                     : outLetterServices.All().Where(m => m.CouncilPeriod.IsActive).Where(p => p.Organ.ID == organId).Where(o => !o.Deleted).Where(l => !l.Received && !l.Archived && !l.Removed).Count();
            ViewBag.SendedOutLettersPageNumber = skip;

            string result = PublicHelpers.RenderRazorViewToString(this, "_SendedOutLetter", model);
            return Json(result);
        }


        [CustomAuthorize(Roles = "Admin,Manager")]
        public ActionResult ExtraArchivedOutLetters(int skip, int take, string organId)
        {
            var model = organId == "" ? outLetterServices.All()
                                               .Where(m => m.CouncilPeriod.IsActive).Where(o => !o.Deleted)
                                               .Where(l => l.Archived && !l.Removed).OrderByDescending(o => o.CreatedOn)
                                               .Skip(skip).Take(take).ToList()
                                      : outLetterServices.All()
                                               .Where(m => m.Organ.ID == organId)
                                               .Where(m => m.CouncilPeriod.IsActive).Where(o => !o.Deleted)
                                               .Where(l => l.Archived && !l.Removed).OrderByDescending(o => o.CreatedOn)
                                               .Skip(skip).Take(take).ToList();
            ViewBag.ArchivedOutLetterCount = organId == "" ? outLetterServices.All().Where(m => m.CouncilPeriod.IsActive).Where(o => !o.Deleted).Where(l => l.Archived && !l.Removed).Count()
                                                      : outLetterServices.All().Where(m => m.CouncilPeriod.IsActive).Where(p => p.Organ.ID == organId).Where(o => !o.Deleted).Where(l => l.Archived && !l.Removed).Count();
            ViewBag.ArchivedOutLettersPageNumber = skip;
            string result = PublicHelpers.RenderRazorViewToString(this, "_ArchivedOutLetter", model);
            return Json(result);

        }

        public ActionResult ExtraLawOutLetters(int skip, int take, string organId)
        {
            var model = organId == "" ? outLetterServices.All()
                                               .Where(m => m.CouncilPeriod.IsActive).Where(o => !o.Deleted)
                                               .Where(l => l.Received && !l.Archived && !l.Removed)
                                               .Where(l => l.OutLetterStatus >= 4 && l.OutLetterStatus <= 6)
                                               .OrderByDescending(o => o.CreatedOn)
                                               .Skip(skip).Take(take).ToList()
                                      : outLetterServices.All()
                                               .Where(m => m.Organ.ID == organId)
                                               .Where(m => m.CouncilPeriod.IsActive).Where(o => !o.Deleted)
                                               .Where(l => l.Received && !l.Archived && !l.Removed)
                                               .Where(l => l.OutLetterStatus >= 4 && l.OutLetterStatus <= 6)
                                               .OrderByDescending(o => o.CreatedOn)
                                               .Skip(skip).Take(take).ToList();
            ViewBag.LawOutLetterCount = organId == "" ? outLetterServices.All()
                                                        .Where(m => m.CouncilPeriod.IsActive)
                                                        .Where(o => !o.Deleted)
                                                        .Where(l => l.Received && !l.Archived && !l.Removed)
                                                        .Where(l => l.OutLetterStatus >= 4 && l.OutLetterStatus <= 6).Count()
                                                      : outLetterServices.All().Where(m => m.CouncilPeriod.IsActive)
                                                        .Where(p => p.Organ.ID == organId)
                                                        .Where(m => m.CouncilPeriod.IsActive)
                                                        .Where(o => !o.Deleted)
                                                        .Where(l => l.Received && !l.Archived && !l.Removed)
                                                        .Where(l => l.OutLetterStatus >= 4 && l.OutLetterStatus <= 6).Count();
            ViewBag.LawPageNumber = skip;
            string result = PublicHelpers.RenderRazorViewToString(this, "_LawOutLetter", model);
            return Json(result);
        }

        public ActionResult ExtraPlanOutLetters(int skip, int take, string organId)
        {
            var model = organId == "" ? outLetterServices.All()
                                               .Where(m => m.CouncilPeriod.IsActive).Where(o => !o.Deleted)
                                               .Where(l => !l.Received && l.OutLetterStatus == 2 && !l.Archived && !l.Removed)
                                               .OrderByDescending(o => o.CreatedOn)
                                               .Skip(skip).Take(take).ToList()
                                      : outLetterServices.All()
                                               .Where(m => m.Organ.ID == organId)
                                               .Where(m => m.CouncilPeriod.IsActive).Where(o => !o.Deleted)
                                               .Where(l => !l.Received && l.OutLetterStatus == 2 && !l.Archived && !l.Removed)
                                               .OrderByDescending(o => o.CreatedOn)
                                               .Skip(skip).Take(take).ToList();
            ViewBag.PlanOutLetterCount = organId == "" ? outLetterServices.All().Where(m => m.CouncilPeriod.IsActive).Where(o => !o.Deleted).Where(l => !l.Received && l.OutLetterStatus == 2 && !l.Archived && !l.Removed).Count()
                                                      : outLetterServices.All().Where(m => m.CouncilPeriod.IsActive).Where(p => p.Organ.ID == organId).Where(o => !o.Deleted).Where(l => !l.Received && l.OutLetterStatus == 2 && !l.Archived && !l.Removed).Count();
            ViewBag.PlanPageNumber = skip;
            string result = PublicHelpers.RenderRazorViewToString(this, "_PlanOutLetter", model);
            return Json(result);
        }

        #region ارسال مکاتبات به سازمان ها
        [CustomAuthorize(Roles = "Admin,Manager")]
        public ActionResult SendOutLetter()
        {
            ViewBag.Recivers = userServices.All().ToList();
           // ViewBag.UniqueNumber = uniqueNumberServices.GetUniqueNumber();
            ViewBag.Organs = organService.All().Where(m=>m.Deleted==false).ToList();
            // var boss = userServices.All().Where(m => m.IsCouncilBoss).FirstOrDefault();
            ViewBag.CanSignatureList = userService.All().Where(m => m.CanSignatureForLetter == true).ToList();//boss != null ? boss.FirstName + ' ' + boss.LastName + " " + "[رئیس  شورا]" :  null;
            return View();
        }

        [CustomAuthorize(Roles = "Admin,Manager")]
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult SendOutLetter(
               OutLetter outLetter,
               HttpPostedFileBase file, 
               List<HttpPostedFileBase> uploads,
               string letterType,
               string Note,
               string rowID,
               string txtContent)
        {
            outLetter.Received = false;
            var Signature= Request["SignatureUser"];
            var dd = Request["Organ"];
            string[] q = dd.Split(',');
            string[] OrganIDCopy = rowID.Split(',');
            string[] OrganTextCopy = txtContent.Split(',');

            if (letterType == "ECE")
            {
                CreateXMLFile(outLetter, file, uploads);
            }
            else
            {
                byte outLetterstatus = 7;
                if (!string.IsNullOrEmpty(Note))
                    outLetter = AddOutLetterNote(outLetter, Note);
                //byte outLetterstatus = !string.IsNullOrEmpty(letterType) ? Convert.ToByte(letterType) : Convert.ToByte("200");
                outLetter.OutLetterStatus = outLetterstatus;
                outLetter.OutLetterNumber = uniqueNumberServices.GetUniqueNumberType(1);
                outLetter.SendDate = publicMethods.toPersianNumber(publicMethods.ConvertToJalali(DateTime.Now)); 
                var organ = organService.All().Where(m => m.ID == "1").FirstOrDefault();
                outLetter.SignatureUserID = Signature; //userService.All().Where(m => m.ID == Signature).FirstOrDefault();
               // outLetter.Organ = organ;
                string letterID = outLetterServices.CreateOutLetter(outLetter, file, uploads);
                if (letterID!=null)
                {

                    for (int i = 0; i < q.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(q[i]))
                        {
                            CopyToLetter cpl = new CopyToLetter();
                            cpl.OrganID = q[i].Trim();
                            cpl.OutLetterID = letterID;
                            cpl.TypeCopy = 1;// گیرنده اصلی

                            copyToLetterService.Create(cpl);                            
                        }
                    }

                    for (int i = 0; i < OrganIDCopy.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(OrganIDCopy[i]))
                        {
                            CopyToLetter cpl = new CopyToLetter();
                            cpl.OrganID = OrganIDCopy[i].Trim();
                            cpl.OutLetterID = letterID;
                            cpl.TypeCopy = 2;// گیرنده رونوشت
                            cpl.Description = OrganTextCopy[i];
                            copyToLetterService.Create(cpl);
                        }

                    }

                }
                if (letterID == "error")
                {
                    ViewBag.ErrorMessage = "فرمت فایل برای الصاق صحیح نمی باشد";
                    ViewBag.Organs = organService.All().ToList();
                    return View("SendOutLetter");
                }
                else
                {
                    return RedirectToAction("ShowOutLetters");
                }
            }
            return RedirectToAction("ShowOutLetters");
        }

        [CustomAuthorize(Roles = "Admin,Manager")]
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOutLetter(OutLetter outLetter, HttpPostedFileBase file, List<HttpPostedFileBase> uploads, string letterType, string Note)
        {
            outLetter.Received = false;
            var dd = Request["Organ"];
            //if (letterType == "ECE")
            //{
            //    CreateXMLFile(outLetter, file, uploads);
            //}
         //   else
          //  {
                byte outLetterstatus = 0;
                if (!string.IsNullOrEmpty(Note))
                    outLetter = AddOutLetterNote(outLetter, Note);
                //byte outLetterstatus = !string.IsNullOrEmpty(letterType) ? Convert.ToByte(letterType) : Convert.ToByte("200");
                outLetter.OutLetterStatus = outLetterstatus;
                outLetter.OutLetterNumber = uniqueNumberServices.GetUniqueNumberType(1);
                outLetter.SendDate = DateTime.Now.ToString();
                var organ = organService.All().Where(m => m.ID == dd).FirstOrDefault();
                outLetter.Organ = organ;

                string letterID = outLetterServices.CreateOutLetter(outLetter, file, uploads);
                if (letterID == "error")
                {
                    ViewBag.ErrorMessage = "فرمت فایل برای الصاق صحیح نمی باشد";
                    ViewBag.Organs = organService.All().ToList();
                    return View("SendOutLetter");
                }
                else
                {
                    return RedirectToAction("ShowOutLetters");
                }
          //  }
           // return RedirectToAction("ShowOutLetters");
        }


        #endregion
        private OutLetter AddOutLetterNote(OutLetter outLetter, string note)
        {
            CustomPrincipal _User = (CustomPrincipal)HttpContext.User;
            var userId = _User.UserId;

            Note _note = new Note() { ContentText = note, WriterId = userId };
            outLetter.Notes = new List<Note>();
            outLetter.Notes.Add(new Note() { ContentText = note, WriterId = userId });
            return outLetter;
        }
        private void CreateXMLFile(OutLetter outLetter, HttpPostedFileBase file, List<HttpPostedFileBase> uploads)
        {
            //XmlDocument doc = new XmlDocument();
            //string base64 = ECEMethods.CreateBase64(file.FileName);
            //XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            //doc.AppendChild(docNode);

            //XmlNode productsNode = doc.CreateElement("products");
            //doc.AppendChild(productsNode);

            //XmlNode productNode = doc.CreateElement("product");
            //XmlAttribute productAttribute = doc.CreateAttribute("id");
            //productAttribute.Value = "01";
            //productNode.Attributes.Append(productAttribute);
            //productsNode.AppendChild(productNode);

            //XmlNode nameNode = doc.CreateElement("Name");
            //nameNode.AppendChild(doc.CreateTextNode("Java"));
            //productNode.AppendChild(nameNode);
            //XmlNode priceNode = doc.CreateElement("Price");
            //priceNode.AppendChild(doc.CreateTextNode("Free"));
            //productNode.AppendChild(priceNode);

            //// Create and add another product node.
            //productNode = doc.CreateElement("product");
            //productAttribute = doc.CreateAttribute("id");
            //productAttribute.Value = "02";
            //productNode.Attributes.Append(productAttribute);
            //productsNode.AppendChild(productNode);

            //nameNode = doc.CreateElement("Name");
            //nameNode.AppendChild(doc.CreateTextNode("C#"));
            //productNode.AppendChild(nameNode);

            //priceNode = doc.CreateElement("Price");
            //priceNode.AppendChild(doc.CreateTextNode("Free"));
            //productNode.AppendChild(priceNode);

            ////Save attach
            //XmlNode imageNode = doc.CreateElement("SIGNATURE");           
            //XmlAttribute imageAttribute = doc.CreateAttribute("base");
            //imageAttribute.Value = "xs:base64Binary";
            //imageNode.Attributes.Append(imageAttribute);
            //productNode.AppendChild(imageNode);



            string SaveXMLLocation = @"D:\directory\XmlDocument.xml";
            string ImageFileLocation = @"D:\directory\SomePicture.jpg";

            byte[] oFileBytes = null;
            if (System.IO.File.Exists(ImageFileLocation))
            {
                oFileBytes = System.IO.File.ReadAllBytes(ImageFileLocation);
            }

            var xmlDocument =
                new XElement("Contacts",
                                        new XElement("Attachment", new XAttribute("contentencoding", "base64"), new XAttribute("contenttype", "image/jpeg"), new XCData(Convert.ToBase64String(oFileBytes, Base64FormattingOptions.InsertLineBreaks))),
                                        new XElement("Contact",
                                        new XElement("Name", "Patrick Hines"),
                                        new XElement("Phone", "206-555-0144"),
                                        new XElement("Address",
                                            new XElement("Street1", "123 Main St"),
                                            new XElement("City", "Mercer Island"),
                                            new XElement("State", "WA"),
                                            new XElement("Postal", "68042"))));

            xmlDocument.Save(SaveXMLLocation);
            var send = ECEMethods.SendECE(SaveXMLLocation);
            //doc.Save(Console.Out);

            //List<Upload> uploadfileName=outLetterServices.UploadXmlFiles(doc);
        }

        #region ثبت مکاتبات، لوایح، طرحها
        [CustomAuthorize(Roles = "Admin,Manager")]
        public ActionResult ReciveOutLetter()
        {
            ViewBag.Recivers = userServices.All().ToList();
           // ViewBag.UniqueNumber = uniqueNumberServices.GetUniqueNumber();
            ViewBag.Organs = organService.All().Where(m=>m.Deleted==false).ToList();
            return View();
        }

        [CustomAuthorize(Roles = "Admin,Manager")]
        [HttpPost]
        public ActionResult ReciveOutLetter(OutLetter outLetterSpecs, HttpPostedFileBase file, List<HttpPostedFileBase> uploads, string letterType, string Note , string AttachmentType , string ScanedFile)
        {
            var dd = Request["Organ"];
            byte outLetterstatus = !string.IsNullOrEmpty(letterType) ? Convert.ToByte(letterType) : Convert.ToByte("200");
            var organ = organService.All().Where(m => m.ID == dd).FirstOrDefault();
            outLetterSpecs.SendDate = publicMethods.toPersianNumber(publicMethods.ConvertToJalali(DateTime.Now));
            outLetterSpecs.OutLetterNumber = uniqueNumberServices.GetUniqueNumberType(2);
            outLetterSpecs.Organ = organ;
            outLetterSpecs.OutLetterStatus = outLetterstatus;
            outLetterSpecs.Received = true;


            if (!string.IsNullOrEmpty(Note))
                outLetterSpecs = AddOutLetterNote(outLetterSpecs, Note);

            string letterID = string.Empty;
            if (AttachmentType == "scanner")
                letterID = outLetterServices.CreateOutLetterScaned(outLetterSpecs, ScanedFile);
            else
                letterID = outLetterServices.CreateOutLetter(outLetterSpecs, file, uploads);

            if (letterID == "error")
            {
                ViewBag.ErrorMessage = "فرمت فایل برای الصاق صحیح نمی باشد";
                ViewBag.Organs = organService.All().ToList();
                return View("ReciveOutLetter");
            }
            else
            {
                ViewBag.Message = "نامه با شماره "+outLetterSpecs.OutLetterNumber + " ثبت گردید.";
                return RedirectToAction("ShowOutLetters");
            }
        }
        #endregion
       



        [CustomAuthorize(Roles = "Admin,Manager")]
        public ActionResult FlowOutLetter(string ID)
        {
            ViewBag.Recivers = userServices.All().ToList();
           // ViewBag.UniqueNumber = uniqueNumberServices.GetUniqueNumberType(2);//uniqueNumberServices.GetUniqueNumber();
            ViewBag.OutLetter = outLetterServices.Get<string>(ID);
            return View();
        }

        [CustomAuthorize(Roles = "Admin,Manager")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult FlowOutLetter(Letter letter, string recivers, HttpPostedFileBase file, string outLetterID, List<HttpPostedFileBase> uploads, string SendType)
        {

            var UserID = userService.GetUserByUserName(User.Identity.Name).ID;
            var outLetterNumber = outLetterServices.Get(letter.ID);
            letter.LetterNumber= uniqueNumberServices.GetUniqueNumberForFlowLetter(outLetterNumber.OutLetterNumber);

            //string letterID = letterServices.AddValueToOutLetter(letter, outLetterID, file, User.Identity.GetUserId(), uploads);
            string letterID = letterServices.CreateOutLetter(letter, outLetterID, file, UserID, uploads, SendType);
            //return RedirectToAction("LetterItem", "Letter", new { letterID = letterID });
            return RedirectToAction("ShowOutLetters");
        }

        #region بایگانی
        [CustomAuthorize(Roles = "Admin,Manager")]
        public ActionResult MarkOutLetterAsArchived(string ID)
        {
            outLetterServices.MarkAsArchived(ID);
            return RedirectToAction("ShowOutLetters");
        }
        #endregion


        #region عدم بایگانی
        [CustomAuthorize(Roles = "Admin,Manager")]
        public ActionResult MarkOutLetterAsUnArchived(string ID)
        {
            outLetterServices.MarkAsUnArchived(ID);
            return RedirectToAction("ShowOutLetters");
        } 
        #endregion

        [CustomAuthorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddOutLetterTab(string tabName)
        {
            //outLetterServices.AddTab(tabName, User.Identity.GetUserId());
            return RedirectToAction("ShowOutLetters");
        }

        [CustomAuthorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddOutLetterToTab(string outLetterID, string letterTabID)
        {
            //outLetterServices.AddToTab(outLetterID, letterTabID, User.Identity.GetUserId());
            return RedirectToAction("ShowOutLetters");
        }

        [CustomAuthorize(Roles = "Admin,Manager")]
        public ActionResult MarkOutLetterAsRemoved(string outLetterID)
        {
            outLetterServices.MarkAsRemoved(outLetterID);
            return RedirectToAction("ShowOutLetters");
        }

        [CustomAuthorize(Roles = "Admin,Manager")]
        public ActionResult UndoOutLetterRemoved(string outLetterID)
        {
            outLetterServices.UndoOutLetterRemoved(outLetterID);
            return RedirectToAction("ShowOutLetters");
        }

        public ActionResult Print(string letterID, bool isCommission = false)
        {
            var model = letterServices.GetPrintInfo(letterID, isCommission);
            ViewBag.IsVotingHidden = letterServices.IsVotingHidden(letterID);
            return View(model);
        }

        [CustomAuthorize(Roles = "Admin,Manager")]
        public ActionResult EditSendLetter(string letterID)
        {
            var model = outLetterServices.Get<string>(letterID);
           var OrganList = copyToLetterService.All().Where(m => m.OutLetterID == letterID && m.Deleted == false).ToList().OrderBy(m => m.RowOrder);

           
            ViewBag.Organs = organService.All().Where(m=>m.Deleted==false).ToList();
            ViewBag.CanSignatureList = userService.All().Where(m => m.CanSignatureForLetter == true).ToList();//boss != null ? boss.FirstName + ' ' + boss.LastName + " " + "[رئیس  شورا]" :  null;
            int count = 0;
            string OrganIDCopystr = "";
            string OrgantxtContent = "";
            string organNameList = "";

            foreach (var item in OrganList)
            {
                count++;
                if (count <= OrganList.Count())
                {
                    if (item.TypeCopy == 2)
                    {
                        OrganIDCopystr += item.OrganID + ", ";
                        OrgantxtContent += item.Description + ",";
                        organNameList+= organService.All().Where(m => m.ID == item.OrganID).FirstOrDefault().Name+",";
                    }
                }
            };

            ViewBag.OrganList = OrganList.Where(o=>o.TypeCopy==1).ToList();
            ViewBag.OrganIDCopy = OrganIDCopystr;
            ViewBag.OrgantxtContent = OrgantxtContent;
            ViewBag.organNameList = organNameList;


            return View(model);
        }

        [CustomAuthorize(Roles = "Admin,Manager")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult EditSendLetter(OutLetter outLetter, string Organ, string rowID,
               string txtContent)
        {
            var dd = Request["Organ"];
            var Signature = Request["SignatureUser"];
            string[] q = dd.Split(',');
            string[] OrganIDCopy = rowID.Split(',');
            string[] OrganTextCopy = txtContent.Split(',');
            var organ = organService.All().Where(m => m.ID == "1").FirstOrDefault();
            outLetter.Organ = organ;// organService.Get<string>(Organ);
            var item = outLetterServices.GetAsNoTracking(outLetter.ID);
            outLetter.Uploads = item.Uploads;
            outLetter.Files = item.Files;
            outLetter.LetterID = null;
            outLetter.SignatureUserID = Signature;
            outLetter.OutLetterStatus = 7;
            var organList = copyToLetterService.All().Where(m => m.OutLetterID == outLetter.ID && m.Deleted==false).ToList().OrderBy(m => m.RowOrder);
            foreach (var item2 in organList)
            {
                copyToLetterService.Delete(item2.ID);
            }
            if (outLetter != null)
            {

                for (int i = 0; i < q.Length; i++)
                {
                    if (!string.IsNullOrEmpty(q[i]))
                    {
                        CopyToLetter cpl = new CopyToLetter();
                        cpl.OrganID = q[i].Trim();
                        cpl.OutLetterID = outLetter.ID;
                        cpl.TypeCopy = 1;// گیرنده اصلی

                        copyToLetterService.Create(cpl);
                    }
                }

                for (int i = 0; i < OrganIDCopy.Length; i++)
                {
                    if (!string.IsNullOrEmpty(OrganIDCopy[i]))
                    {
                        if (OrganIDCopy[i]!=string.Empty && !string.IsNullOrWhiteSpace(OrganIDCopy[i]))
                        {
                            CopyToLetter cpl = new CopyToLetter();
                            cpl.OrganID = OrganIDCopy[i].Trim();
                            cpl.OutLetterID = outLetter.ID;
                            cpl.TypeCopy = 2;// گیرنده رونوشت
                            cpl.Description = OrganTextCopy[i];
                            copyToLetterService.Create(cpl);
                        }
                       
                    }

                }

            }
            //if (outLetter.ID != null)
            //{

            //    for (int i = 0; i < q.Length; i++)
            //    {
            //        if (!string.IsNullOrEmpty(q[i]) && !string.IsNullOrWhiteSpace(q[i]))
            //        {
            //            CopyToLetter cpl = new CopyToLetter();
            //            cpl.OrganID = q[i];
            //            cpl.OutLetterID = outLetter.ID;
            //            copyToLetterService.Create(cpl);
            //        }
            //    }

            //}
            //// 
            outLetterServices.Update(outLetter);
            return RedirectToAction("ShowOutLetters");
        }

        public ActionResult DisplaySendLetter(string letterID)
        {
            var model = outLetterServices.Get<string>(letterID);
            string OrganListstr = "";
            string OrganCopy = "";
            if (model.Received == false)
            {
                var OrganList = copyToLetterService.All().Where(m => m.OutLetterID == model.ID && m.Deleted == false ).ToList().OrderBy(m => m.RowOrder);
                int count = 0;

                foreach (var item in OrganList)
                {
                    count++;
                    if (count < OrganList.Count())
                    {
                        if (item.TypeCopy==1)
                        {
                              OrganListstr += organService.All().Where(m => m.ID == item.OrganID).FirstOrDefault().Name + ", ";
                        }
                        else
                        {
                            var q = organService.All().Where(m => m.ID == item.OrganID).FirstOrDefault();
                            OrganCopy += "\n" + q.Name + " ";
                            if (!string.IsNullOrWhiteSpace(item.Description) && !string.IsNullOrEmpty(item.Description))
                            {
                                OrganCopy += item.Description + ",\n";
                            }
                            else
                            {
                                OrganCopy += ",";
                            }
                                
                        }

                    }
                    else
                    {
                        if (item.TypeCopy == 1)
                        {
                            OrganListstr += organService.All().Where(m => m.ID == item.OrganID).FirstOrDefault().Name;
                        }
                        else
                        {
                            var q = organService.All().Where(m => m.ID == item.OrganID).FirstOrDefault();
                            OrganCopy += "\n" + q.Name + " ";
                            if (!string.IsNullOrWhiteSpace(item.Description) && !string.IsNullOrEmpty(item.Description))
                            {
                                OrganCopy += item.Description + ",\n";
                            }
                        }
                       
                    }

                };

            }
            
            ViewBag.OrganList = OrganListstr;
            ViewBag.OrganCopy = OrganCopy;
            return View(model);
        }

        public ActionResult DisplayReciveLetter(string letterID)
        {
            var model = outLetterServices.Get<string>(letterID);
            return View(model);
        }

        [CustomAuthorize(Roles = "Admin,Manager")]
        public ActionResult EditReciveLetter(string letterID)
        {
            var model = outLetterServices.Get<string>(letterID);
            ViewBag.Organs = organService.All().Where(m=>m.Deleted==false).ToList();
            return View(model);
        }

        [CustomAuthorize(Roles = "Admin,Manager")]
        [HttpPost]
        public ActionResult EditReciveLetter(OutLetter outLetter, string Organ, List<HttpPostedFileBase> uploads, string letterType)
        {
            //var item = outLetterServices.GetAsNoTracking(outLetter.ID);
            //outLetter.Uploads = item.Uploads;
            //outLetter.Files = item.Files;
            //outLetter.LetterID = null;
            //outLetterServices.Update(outLetter);
            byte outLetterstatus = !string.IsNullOrEmpty(letterType) ? Convert.ToByte(letterType) : Convert.ToByte("200");
            outLetter.OutLetterStatus = outLetterstatus;
            outLetter.Organ = organService.Get<string>(Organ);
            outLetterServices.EditOutLetter(outLetter, uploads);
            return RedirectToAction("ShowOutLetters");
        }

        #region حذف نامه
        [CustomAuthorize(Roles = "Admin,Manager")]
        public ActionResult DeleteOutLetter(string outLetterId)
        {
            var model = outLetterServices.Get<string>(outLetterId);

            ViewBag.HasLetter = (letterServices.All().Where(m => m.CouncilPeriod.IsActive).Any(l => l.OutLetter.ID == outLetterId)) ? "True" : "False";
            string OrganListstr = "";
            if (model.Received==false)
            {
                var OrganList = copyToLetterService.All().Where(m => m.OutLetterID == model.ID && m.Deleted==false).ToList().OrderBy(m => m.RowOrder);
                int count = 0;
                foreach (var item in OrganList)
                {
                    count++;
                    if (count < OrganList.Count())
                    {
                        OrganListstr += organService.All().Where(m => m.ID == item.OrganID).FirstOrDefault().Name + ", ";
                    }
                    else
                    {
                        OrganListstr += organService.All().Where(m => m.ID == item.OrganID).FirstOrDefault().Name;
                    }

                };

            }
            ViewBag.OrganList = OrganListstr;

            return View(model);
        }

        [HttpPost]
        public ActionResult DoDeleteOutLetter(string Id)
        {
            var oldOuletter = outLetterServices.Get<string>(Id);
            outLetterServices.DeleteOutLetter(oldOuletter);
            return RedirectToAction("ShowOutLetters");
        }
        #endregion



        public ActionResult SendOutLetterPrintA4(string letterId)
        {
            ViewBag.logo = settingsService.All().Where(m => m.Deleted == false).FirstOrDefault().CouncilLogo;

            var letter = outLetterServices.Get<string>(letterId);
            var boss = userServices.All().Where(m => m.IsCouncilBoss).FirstOrDefault();
            string OrganListstr = "";
            string OrganCopy = "<p>";
            if (letter != null)
            {
                var OrganList = copyToLetterService.All().Where(m => m.OutLetterID == letter.ID && m.Deleted == false).ToList().OrderBy(m => m.RowOrder);
                int count = 0;

                foreach (var item in OrganList)
                {
                    count++;
                    if (count < OrganList.Count())
                    {
                        if (item.TypeCopy == 1)
                        {
                            OrganListstr += organService.All().Where(m => m.ID == item.OrganID).FirstOrDefault().Name + " ";
                        }
                        else
                        {
                            var q = organService.All().Where(m => m.ID == item.OrganID).FirstOrDefault();
                            OrganCopy += "-  " + q.Name + " ";
                            if (!string.IsNullOrWhiteSpace(item.Description) && !string.IsNullOrEmpty(item.Description))
                            {
                                OrganCopy +=  item.Description + "<br/>";
                            }
                            else
                            {
                                OrganCopy += "<br/>";
                            }

                        }

                    }
                    else
                    {
                        if (item.TypeCopy == 1)
                        {
                            OrganListstr += organService.All().Where(m => m.ID == item.OrganID).FirstOrDefault().Name;
                        }
                        else
                        {
                            var q = organService.All().Where(m => m.ID == item.OrganID).FirstOrDefault();
                            OrganCopy += "- " + q.Name + " ";
                            if (!string.IsNullOrWhiteSpace(item.Description) && !string.IsNullOrEmpty(item.Description))
                            {
                                OrganCopy +=  item.Description + "<br/>";
                            }
                        }

                    }

                };

            }
            OrganCopy += "</p>";

            ViewBag.OrganList = OrganListstr;
            ViewBag.OrganCopy = OrganCopy;
            PrintOutLetter printLetter = new PrintOutLetter();
            printLetter.Content = letter.Comment;
            printLetter.LetterDate = letter.OutLetterDate;
            printLetter.LetterNumber = letter.OutLetterNumber;
            printLetter.SaveLetterDate = letter.SendDate;
            printLetter.Subject = letter.Subject;
            printLetter.Reciever = OrganListstr;// letter.Organ.Name;
            printLetter.UploadCount = letter.Files != null && letter.Files != "" ? (letter.Uploads.Count() + 1).ToString() : letter.Uploads.Count().ToString();
            printLetter.SignatureUrl = boss.Signature;
            printLetter.CopyTo = OrganCopy;
            printLetter.BossName = boss.FirstName + " " + boss.LastName;
            printLetter.Title1 = letter.Transferee;
            printLetter.Title2 = letter.Reciver;

            return View(printLetter);
        }
        public ActionResult SendOutLetterPrintA5Header(string letterId)
        {
            ViewBag.logo = settingsService.All().Where(m => m.Deleted == false).FirstOrDefault().CouncilLogo;

            var letter = outLetterServices.Get<string>(letterId);
            var boss = userServices.All().Where(m => m.IsCouncilBoss).FirstOrDefault();
            string OrganListstr = "";
            string OrganCopy = "<p>";
            if (letter != null)
            {
                var OrganList = copyToLetterService.All().Where(m => m.OutLetterID == letter.ID && m.Deleted == false).ToList().OrderBy(m => m.RowOrder);
                int count = 0;

                foreach (var item in OrganList)
                {
                    count++;
                    if (count < OrganList.Count())
                    {
                        if (item.TypeCopy == 1)
                        {
                            OrganListstr += organService.All().Where(m => m.ID == item.OrganID).FirstOrDefault().Name + " ";
                        }
                        else
                        {
                            var q = organService.All().Where(m => m.ID == item.OrganID).FirstOrDefault();
                            OrganCopy += "-  " + q.Name + " ";
                            if (!string.IsNullOrWhiteSpace(item.Description) && !string.IsNullOrEmpty(item.Description))
                            {
                                OrganCopy +=  item.Description + "<br/>";
                            }
                            else
                            {
                                OrganCopy += "<br/>";
                            }

                        }

                    }
                    else
                    {
                        if (item.TypeCopy == 1)
                        {
                            OrganListstr += organService.All().Where(m => m.ID == item.OrganID).FirstOrDefault().Name;
                        }
                        else
                        {
                            var q = organService.All().Where(m => m.ID == item.OrganID).FirstOrDefault();
                            OrganCopy += "- " + q.Name + " ";
                            if (!string.IsNullOrWhiteSpace(item.Description) && !string.IsNullOrEmpty(item.Description))
                            {
                                OrganCopy +=  item.Description + "<br/>";
                            }
                        }

                    }

                };

            }
            OrganCopy += "</p>";
            ViewBag.OrganList = OrganListstr;
            ViewBag.OrganCopy = OrganCopy;

           
            PrintOutLetter printLetter = new PrintOutLetter();
            printLetter.Content = letter.Comment;
            printLetter.LetterDate = letter.OutLetterDate;
            printLetter.LetterNumber = letter.OutLetterNumber;
            printLetter.SaveLetterDate = letter.SendDate;
            printLetter.Subject = letter.Subject;
            printLetter.Reciever = OrganListstr;// letter.Organ.Name;
            printLetter.UploadCount = letter.Files != null && letter.Files != "" ? (letter.Uploads.Count() + 1).ToString() : letter.Uploads.Count().ToString();
            printLetter.SignatureUrl = boss.Signature; 
            printLetter.CopyTo = OrganCopy;
            printLetter.BossName = boss.FirstName + " " + boss.LastName;
            printLetter.Title1 = letter.Transferee;
            printLetter.Title2 = letter.Reciver;
            return View(printLetter);
        }

        public ActionResult SendOutLetterPrintA4Header(string letterId)
        {
            ViewBag.logo = settingsService.All().Where(m => m.Deleted == false).FirstOrDefault().CouncilLogo;

            var letter = outLetterServices.Get<string>(letterId);
            var boss = userServices.All().Where(m => m.IsCouncilBoss).FirstOrDefault();
            string OrganListstr = "";
            string OrganCopy = "<p>";
            if (letter != null)
            {
                var OrganList = copyToLetterService.All().Where(m => m.OutLetterID == letter.ID && m.Deleted == false).ToList().OrderBy(m=>m.RowOrder);
                int count = 0;

                foreach (var item in OrganList)
                {
                    count++;
                    if (count < OrganList.Count())
                    {
                        if (item.TypeCopy == 1)
                        {
                            OrganListstr += organService.All().Where(m => m.ID == item.OrganID).FirstOrDefault().Name + " ";
                        }
                        else
                        {
                            var q = organService.All().Where(m => m.ID == item.OrganID).FirstOrDefault();
                            OrganCopy += "-  " + q.Name + " ";
                            if (!string.IsNullOrWhiteSpace(item.Description) && !string.IsNullOrEmpty(item.Description))
                            {
                                OrganCopy +=  item.Description + "<br/>";
                            }
                            else
                            {
                                OrganCopy += "<br/>";
                            }

                        }

                    }
                    else
                    {
                        if (item.TypeCopy == 1)
                        {
                            OrganListstr += organService.All().Where(m => m.ID == item.OrganID).FirstOrDefault().Name;
                        }
                        else
                        {
                            var q = organService.All().Where(m => m.ID == item.OrganID).FirstOrDefault();
                            OrganCopy += "- " + q.Name + " ";
                            if (!string.IsNullOrWhiteSpace(item.Description) && !string.IsNullOrEmpty(item.Description))
                            {
                                OrganCopy +=  item.Description + "<br/>";
                            }
                        }

                    }

                };

            }
            OrganCopy += "</p>";

            ViewBag.OrganList = OrganListstr;
            ViewBag.OrganCopy = OrganCopy;
            PrintOutLetter printLetter = new PrintOutLetter();
            printLetter.Content = letter.Comment;
            printLetter.LetterDate = letter.OutLetterDate;
            printLetter.LetterNumber = letter.OutLetterNumber;
            printLetter.SaveLetterDate = letter.SendDate;
            printLetter.Subject = letter.Subject;
            printLetter.Reciever = OrganListstr;// letter.Organ.Name;
            printLetter.UploadCount = letter.Files != null && letter.Files != "" ? (letter.Uploads.Count() + 1).ToString() : letter.Uploads.Count().ToString();
            printLetter.SignatureUrl = boss.Signature;
            printLetter.CopyTo = OrganCopy;
            printLetter.BossName = boss.FirstName + " " + boss.LastName;
            printLetter.Title1 = letter.Transferee;
            printLetter.Title2 = letter.Reciver;
            return View(printLetter);
        }

        public ActionResult SendOutLetterPrintA5(string letterId)
        {
            ViewBag.logo = settingsService.All().Where(m => m.Deleted == false).FirstOrDefault().CouncilLogo;

            var letter = outLetterServices.Get<string>(letterId);
            var boss = userServices.All().Where(m => m.IsCouncilBoss).FirstOrDefault();
            string OrganListstr = "";
            string OrganCopy = "<p>";
            if (letter != null)
            {
                var OrganList = copyToLetterService.All().Where(m => m.OutLetterID == letter.ID && m.Deleted == false).ToList().OrderBy(m=>m.RowOrder);
                int count = 0;

                foreach (var item in OrganList)
                {
                    count++;
                    if (count < OrganList.Count())
                    {
                        if (item.TypeCopy == 1)
                        {
                            OrganListstr += organService.All().Where(m => m.ID == item.OrganID).FirstOrDefault().Name + " ";
                        }
                        else
                        {
                            var q = organService.All().Where(m => m.ID == item.OrganID).FirstOrDefault();
                            OrganCopy += "-  " + q.Name + " ";
                            if (!string.IsNullOrWhiteSpace(item.Description) && !string.IsNullOrEmpty(item.Description))
                            {
                                OrganCopy +=  item.Description + "<br/>";
                            }
                            else
                            {
                                OrganCopy += "<br/>";
                            }

                        }

                    }
                    else
                    {
                        if (item.TypeCopy == 1)
                        {
                            OrganListstr += organService.All().Where(m => m.ID == item.OrganID).FirstOrDefault().Name;
                        }
                        else
                        {
                            var q = organService.All().Where(m => m.ID == item.OrganID).FirstOrDefault();
                            OrganCopy += "- " + q.Name + " ";
                            if (!string.IsNullOrWhiteSpace(item.Description) && !string.IsNullOrEmpty(item.Description))
                            {
                                OrganCopy += item.Description + "<br/>";
                            }
                        }

                    }

                };

            }
            OrganCopy += "</p>";

            ViewBag.OrganList = OrganListstr;
            ViewBag.OrganCopy = OrganCopy;
            PrintOutLetter printLetter = new PrintOutLetter();
            printLetter.Content = letter.Comment;
            printLetter.LetterDate = letter.OutLetterDate;
            printLetter.LetterNumber = letter.OutLetterNumber;
            printLetter.SaveLetterDate = letter.SendDate;
            printLetter.Subject = letter.Subject;
            printLetter.Reciever = OrganListstr;// letter.Organ.Name;
            printLetter.UploadCount = letter.Files != null && letter.Files != "" ? (letter.Uploads.Count() + 1).ToString() : letter.Uploads.Count().ToString();
            printLetter.SignatureUrl = boss.Signature;
            printLetter.CopyTo = OrganCopy;
            printLetter.BossName = boss.FirstName + " " + boss.LastName;
            printLetter.Title1 = letter.Transferee;
            printLetter.Title2 = letter.Reciver;
            return View(printLetter);
        }

        //#region Report

        //public ActionResult rptSendOutLetter(string letterId,int WhitHeader)
        //{
        //    var letter = outLetterServices.Get<string>(letterId);
        //    var Signatory = userServices.All().Where(m => m.ID==letter.SignatureUserID).FirstOrDefault();
        //    var currentSetting = CurrtenSettingService.All().Where(m => m.Deleted == false).FirstOrDefault();
        //    PrintOutLetter printLetter = new PrintOutLetter();
        //    printLetter.Content = letter.Comment;
        //    printLetter.LetterDate = letter.OutLetterDate;
        //    printLetter.LetterNumber = letter.OutLetterNumber;
        //    printLetter.SaveLetterDate = letter.SendDate;
        //    printLetter.Subject = letter.Subject;
        //   // = letter.Organ.Name;
        //    printLetter.UploadCount = letter.Uploads.Count.ToString();
        //    printLetter.SignatureUrl = HostingEnvironment.ApplicationPhysicalPath + "Upload\\Signature\\" + Signatory.Signature;
        //    printLetter.CopyTo = letter.CopyTo;
        //    // printLetter.BossName = boss.FirstName + " " + boss.LastName;
        //    string OrganListstr = "";
        //    var OrganList = copyToLetterService.All().Where(m => m.OutLetterID == letterId && m.Deleted == false).ToList();
        //    int count = 0;
        //    foreach (var item in OrganList)
        //    {
        //        count++;
        //        if (count<OrganList.Count())
        //        {
        //            OrganListstr += organService.All().Where(m => m.ID == item.OrganID).FirstOrDefault().Name + ", ";
        //        }
        //        else
        //        {
        //            OrganListstr += organService.All().Where(m => m.ID == item.OrganID).FirstOrDefault().Name;
        //        }

        //    };
        //    printLetter.Reciever = OrganListstr;
        //    printLetter.LogoName = HostingEnvironment.ApplicationPhysicalPath + "Images\\logo\\" + currentSetting.CouncilLogo;
        //    printLetter.Signatory = Signatory.FirstName + "" + Signatory.LastName;
        //    TempData["ReportData"] = printLetter;

        //    if (WhitHeader==1)
        //    {
        //        TempData["ReportName"] = "LetterHeaderA5.mrt";
        //    }
        //    else if(WhitHeader==2)
        //    {
        //        TempData["ReportName"] = "letterA5.mrt";
        //    }
        //    else if (WhitHeader == 3)
        //    {
        //        TempData["ReportName"] = "LetterHeaderA4.mrt";
        //    }
        //    else if (WhitHeader == 4)
        //    {
        //        TempData["ReportName"] = "LetterA4.mrt";
        //    }

        //    return View(TempData);
        //}


        //public ActionResult rptRecievedOutLetterPrint(string letterId)
        //{
        //    var letter = outLetterServices.Get<string>(letterId);
        //    var boss = userServices.All().Where(m => m.IsCouncilBoss).FirstOrDefault();
        //    LetterModels printLetter = new LetterModels();
        //    var currentSetting = CurrtenSettingService.All().Where(m => m.Deleted == false).FirstOrDefault();

        //    printLetter.OutLetterDate = letter.OutLetterDate;
        //    printLetter.OutLetterNumber = letter.OutLetterNumber;
        //    printLetter.SendDate = letter.SendDate;
        //    printLetter.Subject = letter.Subject;
        //    printLetter.Bringer = letter.Bringer;
        //    printLetter.Organ = letter.Organ.Name;
        //    printLetter.RegisterNumber = letter.RegisterNumber;
        //    printLetter.LogoName = HostingEnvironment.ApplicationPhysicalPath + "Images\\logo\\" + currentSetting.CouncilLogo;

        //    TempData["ReportData"] = printLetter;           
        //    TempData["ReportName"] = "RecivedLetter.mrt";

        //    return View(TempData);
        //}

        //// public ActionResult GetReportSnapshot(string sort)
        ////{

        ////    //Stimulsoft.Report.StiStyle style = new Stimulsoft.Report.StiStyle();
        ////    //style.Font = new System.Drawing.Font("B Nazanin", 14F);

        ////   // int pid = int.Parse(Session["__SelectedProjectId"].ToString());
        ////    StiReport report = new StiReport();
        ////    report.Load(Server.MapPath("~/Reports/Jobs.mrt"));
        ////    //report.Styles.Clear();
        ////    //report.Styles.AddRange(new Stimulsoft.Report.StiBaseStyle[] { style }); 

        ////    return StiMvcViewer.GetReportResult(report);
        ////}

        //public ActionResult ViewerEvent()
        //{            
        //    return StiMvcViewer.ViewerEventResult();
        //}
        //public ActionResult PrintReport()
        //{

        //return StiMvcViewer.PrintReportResult();
        //}
        //public ActionResult ExportReport()
        //{
        //    return StiMvcViewer.ExportReportResult();
        //}
        //public ActionResult report()
        //{


        //    var report = new StiReport();
        //    //imulsoft.Base.stifontCollection.addfontfile()

        //    //var fileContent = Stimulsoft.System.IO.File.getFile("Roboto-Black.ttf", true);
        //    //var resource = new Stimulsoft.Report.Dictionary.StiResource(
        //    //    "Roboto-Black", "Roboto-Black", false, Stimulsoft.Report.Dictionary.StiResourceType.FontTtf, fileContent);
        //    //report.dictionary.resources.add(resource);

        //    //        logo 
        //    // StiImage im = new StiImage();

        //    // im.Image = System.Drawing.Image.FromFile(@"D:\\shorayar\\Council\\Council.UI\\Images\\logo\\637908944011987478-images.png");
        //   // Stimulsoft.Base.StiFontCollection.AddFontFile(Server.MapPath("~/fonts/B Yekan.ttf"));


        //    report.Load(Server.MapPath("/StimulSoftReport/"+ TempData["ReportName"]));
        //  //  report.Compile();
        //  //  report["image"] = im.Image;
        //   // report.Dictionary.Variables["image"].ValueObject= System.Drawing.Image.FromFile(@"D:\\shorayar\\Council\\Council.UI\\Images\\logo\\637908944011987478-images.png");
        //    report.RegBusinessObject("dt", TempData["ReportData"]);

        //    return StiMvcViewer.GetReportResult(report);
        //}
        ////public ActionResult viewerEvent()
        ////{
        ////    return StiMvcViewer.ViewerEventResult(HttpContext);
        ////    //return null;
        ////}
        //public ActionResult DesignerEvent(StiReport report)
        //{
        //   // return StiMvcViewer.GetReportResult(report);
        //    return StiMvcDesigner.DesignerEventResult();
        //}
        //#endregion
        public ActionResult RecievedOutLetterPrint(string letterId)
        {
            var letter = outLetterServices.Get<string>(letterId);
            var boss = userServices.All().Where(m => m.IsCouncilBoss).FirstOrDefault();
            LetterModels printLetter = new LetterModels();

            printLetter.OutLetterDate = letter.OutLetterDate;
            printLetter.OutLetterNumber = letter.OutLetterNumber;
            printLetter.SendDate = letter.SendDate;
            printLetter.Subject = letter.Subject;
            printLetter.Bringer = letter.Bringer;
            printLetter.Organ = letter.Organ.Name;
            printLetter.RegisterNumber = letter.RegisterNumber;
            ViewBag.logo = settingsService.All().Where(m=>m.Deleted==false).FirstOrDefault().CouncilLogo;
            return View(printLetter);
        }
      
        public JsonResult OutletterNote(string letterId)
        {
            var Notes = outLetterServices.GetNoteLetterInfo(letterId);

            string result = PublicHelpers.RenderRazorViewToString(this, "_NoteOutLetter", Notes);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetOutletterNote(string letterId)
        {
            var letter = letterServices.Get<string>(letterId);
            string result = PublicHelpers.RenderRazorViewToString(this, "_NoteOutLetter", letter.OutLetter.Notes.OrderByDescending(m => m.CreatedOn));
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddNote(string letterId, string text)
        {
            NoteService noteService = new NoteService();
            var userId = userService.GetUserByUserName(User.Identity.Name).ID;
            Note note = new Note { WriterId = userId, ContentText = text };
            var outLetter = outLetterServices.Get<string>(letterId);
            noteService.Create(note);
            outLetter.Notes.Add(note);
            outLetterServices.Update(outLetter);
            return RedirectToAction("ShowOutLetters");
        }

        public ActionResult ApproveSendLetter(string id)
        {
            
             var outLetter = outLetterServices.Get<string>(id);
            outLetter.ConfirmBoss = true;
            outLetter.OutLetterStatus = ((byte)OutLetterStatus.Comfirmed);// تایید شده
            outLetterServices.Update(outLetter);
            return RedirectToAction("Index","Home");
        }
        public ActionResult RejectLetter(string id, OutLetter model)
        {

            var outLetter = outLetterServices.Get<string>(id);
            outLetter.ConfirmBoss = false;
            outLetter.OutLetterStatus = ((byte)OutLetterStatus.ForEdit);// اصلاح شود            
            outLetter.RejectReason = model.RejectReason;
            outLetterServices.Update(outLetter);
            return RedirectToAction("Index", "Home");
        }
        #endregion
    }
}