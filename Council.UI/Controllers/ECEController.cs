using Council.Core.Entities;
using Council.Core.Models;
using Council.Service.DBServices;
using Council.UI.CustomAuthentication;
using OpenPop.Mime;
using OpenPop.Pop3;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace Council.UI.Controllers
{
    //Install-Package OpenPop.NET -Version 2.0.6.1120
    [CustomAuthorize(Roles = "Admin,Manager")]
    public class ECEController : Controller
    {
        EmailService emailService = new EmailService();
        OutLetterServices outLetterServices = new OutLetterServices();
        private const int PageSize = 10;

        public ActionResult Index()
        {
            return View();
        }

        #region دزیافت لیست ایمیل های دریافتی
        public ActionResult _GetECE()
        {
            var PEmails = emailService.GetEceInfo();
            Pop3Client pop3Client;
            if (Session["Pop3Client"] == null)
            {
                pop3Client = new Pop3Client();
                pop3Client.Connect(PEmails.Host, PEmails.Port, PEmails.Ssl);
                pop3Client.Authenticate(PEmails.UserName, PEmails.Password);
                Session["Pop3Client"] = pop3Client;
            }
            else
            {
                pop3Client = (Pop3Client)Session["Pop3Client"];
            }
            int count = pop3Client.GetMessageCount();
            var Emails = new List<POPEmail>();
            int counter = 0;
            for (int i = count; i > count - PageSize; i--)
            {
                Message message = pop3Client.GetMessage(i);
                POPEmail email = new POPEmail()
                {
                    MessageNumber = i,
                    Subject = message.Headers.Subject,
                    DateSent = message.Headers.DateSent,
                    FromEmail = message.Headers.From.Address,
                    From = string.Format("<a href = 'mailto:{1}'>{0}</a>", message.Headers.From.DisplayName, message.Headers.From.Address),
                    To = message.Headers.To.FirstOrDefault().ToString()
                };

                List<MessagePart> attachments = message.FindAllAttachments();

                foreach (MessagePart attachment in attachments)
                {
                    email.Attachments.Add(new Attachment
                    {
                        FileId = attachment.ContentId,
                        FileName = attachment.FileName,
                        ContentType = attachment.ContentType.MediaType,
                        //Content = attachment.Body
                    });
                }
                Emails.Add(email);
                counter++;
                //if (counter > 2)
                //{
                //    break;
                //}
            }
            var emails = Emails;

            var Result = new PagedData<POPEmail>();
            Result.Data = emails;
            Result.NumberOfPages = Convert.ToInt32(Math.Ceiling((double)count / PageSize));

            return PartialView("_ECETable", Result);
        }

        public ActionResult GetByPage(int page)
        {
            var PEmails = emailService.GetEceInfo();
            Pop3Client pop3Client;
            if (Session["Pop3Client"] == null)
            {
                pop3Client = new Pop3Client();
                pop3Client.Connect(PEmails.Host, PEmails.Port, PEmails.Ssl);
                pop3Client.Authenticate(PEmails.UserName, PEmails.Password);
                Session["Pop3Client"] = pop3Client;
            }
            else
            {
                pop3Client = (Pop3Client)Session["Pop3Client"];
            }
            int count = pop3Client.GetMessageCount();
            var Emails = new List<POPEmail>();
            int counter = 0;
            int StartFrom = count - PageSize * (page - 1);
            int End = StartFrom > PageSize ? (count - PageSize * (page - 1)) - PageSize : 0;

            for (int i = StartFrom; i > End; i--)
            {
                Message message = pop3Client.GetMessage(i);
                POPEmail email = new POPEmail()
                {
                    MessageNumber = i,
                    Subject = message.Headers.Subject,
                    DateSent = message.Headers.DateSent,
                    FromEmail = message.Headers.From.Address,
                    From = string.Format("<a href = 'mailto:{1}'>{0}</a>", message.Headers.From.DisplayName, message.Headers.From.Address),
                    To = message.Headers.To.FirstOrDefault().ToString()
                };

                //MessagePart body = message.FindFirstHtmlVersion();
                //if (body != null)
                //{
                //    email.Body = body.GetBodyAsText();
                //}
                //else
                //{
                //    body = message.FindFirstPlainTextVersion();
                //    if (body != null)
                //    {
                //        email.Body = body.GetBodyAsText();
                //    }
                //}

                //List<MessagePart> attachments = message.FindAllAttachments();

                //foreach (MessagePart attachment in attachments)
                //{
                //    email.Attachments.Add(new Attachment
                //    {
                //        FileId = attachment.ContentId,
                //        FileName = attachment.FileName,
                //        ContentType = attachment.ContentType.MediaType,
                //        //Content = attachment.Body
                //    });
                //}
                Emails.Add(email);
                counter++;
                //if (counter > 2)
                //{
                //    break;
                //}
            }
            var emails = Emails;

            var Result = new PagedData<POPEmail>();
            Result.Data = emails;
            Result.NumberOfPages = Convert.ToInt32(Math.Ceiling((double)count / PageSize));
            Result.CurrentPage = page;

            return PartialView("_ECETable", Result);
        }
        #endregion

        #region دریافت ضمائم ایمیل
        #region درافت ضمائم به صورت فشرده
        public ActionResult GetZipFile(int zFileId)
        {
            var PEmails = emailService.GetEceInfo();
            Pop3Client pop3Client;
            if (Session["Pop3Client"] == null)
            {
                pop3Client = new Pop3Client();
                pop3Client.Connect(PEmails.Host, PEmails.Port, PEmails.Ssl);
                pop3Client.Authenticate(PEmails.UserName, PEmails.Password);
                Session["Pop3Client"] = pop3Client;
            }
            else
            {
                pop3Client = (Pop3Client)Session["Pop3Client"];
            }

            int count = pop3Client.GetMessageCount();

            Message message = pop3Client.GetMessage(zFileId);
            List<MessagePart> attachments = message.FindAllAttachments();


            byte[] fileBytes = null;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (ZipArchive zip = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var f in attachments)
                    {
                        ZipArchiveEntry zipItem = zip.CreateEntry(f.FileName);
                        using (MemoryStream originalFileMemoryStream = new MemoryStream(f.Body))
                        {
                            using (System.IO.Stream entryStream = zipItem.Open())
                            {
                                originalFileMemoryStream.CopyTo(entryStream);
                            }
                        }
                    }
                }
                fileBytes = memoryStream.ToArray();
            }

            return File(fileBytes.ToArray(), "application/zip", "download.zip");
        }
        #endregion

        #region دریافت ضمیمه
        public ActionResult GetFile(int MessageId, string FileId)
        {
            var PEmails = emailService.GetEceInfo();
            Pop3Client pop3Client;
            if (Session["Pop3Client"] == null)
            {
                pop3Client = new Pop3Client();
                pop3Client.Connect(PEmails.Host, PEmails.Port, PEmails.Ssl);
                pop3Client.Authenticate(PEmails.UserName, PEmails.Password);
                Session["Pop3Client"] = pop3Client;
            }
            else
            {
                pop3Client = (Pop3Client)Session["Pop3Client"];
            }

            Message message = pop3Client.GetMessage(MessageId);

            MessagePart attachment = message.FindAllAttachments().FirstOrDefault();

            string xml = Encoding.UTF8.GetString(attachment.Body);

            string _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
            if (xml.StartsWith(_byteOrderMarkUtf8))
            {
                xml = xml.Remove(0, _byteOrderMarkUtf8.Length);
            }

            ECE.Letter deletter = ECE.Letter.DeSerialize(xml);

            int Index = 1;
            foreach (var item in deletter.Attachments)
            {
                if (Index.ToString() == FileId)
                {
                    try
                    {
                        string dummyData = item.Value.Trim().Replace(" ", "+");
                        if (dummyData.Length % 4 > 0)
                            dummyData = dummyData.PadRight(dummyData.Length + 4 - dummyData.Length % 4, '=');
                        byte[] byteArray = Convert.FromBase64String(dummyData);

                        var bytes = Convert.FromBase64String(dummyData);
                        return base.File(bytes, item.ContentType, item.Any);
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                }
                Index++;
            }

            return HttpNotFound();
        }
        #endregion
        #endregion

        #region جزییات ایمیل
        public ActionResult Details(int id)
        {
            try
            {
                var PEmails = emailService.GetEceInfo();
                Pop3Client pop3Client;
                if (Session["Pop3Client"] == null)
                {
                    pop3Client = new Pop3Client();
                    pop3Client.Connect(PEmails.Host, PEmails.Port, PEmails.Ssl);
                    pop3Client.Authenticate(PEmails.UserName, PEmails.Password);
                    Session["Pop3Client"] = pop3Client;
                }
                else
                {
                    pop3Client = (Pop3Client)Session["Pop3Client"];
                }

                Message message = pop3Client.GetMessage(id);
                POPEmailDetails email = new POPEmailDetails()
                {
                    MessageNumber = id,
                    Subject = message.Headers.Subject,
                    DateSent = message.Headers.DateSent,
                    FromEmail = message.Headers.From.Address,
                    From = string.Format("<a href = 'mailto:{1}'>{0}</a>", message.Headers.From.DisplayName, message.Headers.From.Address),
                    To = message.Headers.To.FirstOrDefault().ToString()
                };

                MessagePart body = message.FindFirstHtmlVersion();
                if (body != null)
                {
                    email.Body = body.GetBodyAsText();
                }
                else
                {
                    body = message.FindFirstPlainTextVersion();
                    if (body != null)
                    {
                        email.Body = body.GetBodyAsText();
                    }
                }

                List<MessagePart> attachments = message.FindAllAttachments();

                MessagePart attachment = attachments[0];

                string xml = Encoding.UTF8.GetString(attachment.Body);

                string _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
                if (xml.StartsWith(_byteOrderMarkUtf8))
                {
                    xml = xml.Remove(0, _byteOrderMarkUtf8.Length);
                }

                ECE.Letter deletter = ECE.Letter.DeSerialize(xml);

                var letter = new LetterInfo
                {
                    Subject = deletter.Subject,
                    Classification = deletter.Classification,
                    Keywords = deletter.Keywords,
                    LetterDateTime = deletter.LetterDateTime,
                    LetterNo = deletter.LetterNo,
                    OtherReceivers = deletter.OtherReceivers,
                    Priority = deletter.Priority,
                    Protocol = deletter.Protocol,
                    Receiver = deletter.Receiver,
                    RelatedLetters = deletter.RelatedLetters,
                    Sender = deletter.Sender,
                    Software = deletter.Software
                };

                List<ECE.Origin> newOrigions = new List<ECE.Origin>();

                foreach (var tiff in deletter.Origins)
                {
                    byte[] bytes = Convert.FromBase64String(tiff.Value);

                    using (var ms = new MemoryStream(bytes))
                    {
                        Bitmap bmp = new Bitmap(System.Drawing.Image.FromStream(ms, true));
                        using (var nms = new MemoryStream())
                        {
                            bmp.Save(nms, ImageFormat.Jpeg);
                            byte[] img = nms.ToArray();
                            string base64String = Convert.ToBase64String(img);

                            newOrigions.Add(new ECE.Origin
                            {
                                ContentType = "image/jpeg",
                                Value = base64String
                            });
                        }
                    }
                }

                letter.Origins = newOrigions;

                int Index = 1;
                foreach (var item in deletter.Attachments)
                {
                    var file = new Attachment
                    {
                        FileId = Index.ToString(),
                        ContentType = item.ContentType,
                        FileName = item.Any
                    };

                    letter.Attachments.Add(file);
                    Index++;
                }
                email.Letter = letter;

                return View(email);
            }
            catch (Exception)
            {
                TempData["message"] = new ShowMessage
                {
                    Message = "error",
                    MessageType = ShowMessage.MessageTypes.error
                };

                return View();
            }
        }
        #endregion

        #region حذف ایمیل

        public ActionResult Delete(int id)
        {
            var PEmails = emailService.GetEceInfo();
            //using (Pop3Client client = new Pop3Client())
            //{
            //    client.Connect(PEmails.Host, PEmails.Port, PEmails.Ssl);
            //    client.Authenticate(PEmails.UserName, PEmails.Password);

            //    // Get the number of messages on the POP3 server
            //    int messageCount = client.GetMessageCount();

            //    // Run trough each of these messages and download the headers
            //    for (int messageItem = messageCount; messageItem > 0; messageItem--)
            //    {
            //        // If the Message ID of the current message is the same as the parameter given, delete that message
            //        if (client.GetMessageHeaders(messageItem).MessageId == id)
            //        {
            //            // Delete
            //            client.DeleteMessage(messageItem);
            //            return true;
            //        }
            //    }
            //    // We did not find any message with the given messageId, report this back
            //    return false;
            //}



            using (Pop3Client pop3Client = new Pop3Client())
            {
                pop3Client.Connect(PEmails.Host, PEmails.Port, PEmails.Ssl);
                pop3Client.Authenticate(PEmails.UserName, PEmails.Password);
                Message message = pop3Client.GetMessage(id);

                if (message != null)
                {
                    pop3Client.DeleteMessage(id);
                }
                pop3Client.Disconnect();
            }

            TempData["message"] = new ShowMessage
            {
                Message = "",
                MessageType = ShowMessage.MessageTypes.success
            };

            return RedirectToAction("GetECE");
        }
        #endregion

        #region درج نامه رسیده در دیتابیس
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AddLetter(int id)
        {
            try
            {
                var PEmails = emailService.GetEceInfo();
                Pop3Client pop3Client;
                if (Session["Pop3Client"] == null)
                {
                    pop3Client = new Pop3Client();
                    pop3Client.Connect(PEmails.Host, PEmails.Port, PEmails.Ssl);
                    pop3Client.Authenticate(PEmails.UserName, PEmails.Password);
                    Session["Pop3Client"] = pop3Client;
                }
                else
                {
                    pop3Client = (Pop3Client)Session["Pop3Client"];
                }

                Message message = pop3Client.GetMessage(id);

                MessagePart attachment = message.FindAllAttachments().FirstOrDefault();

                string xml = Encoding.UTF8.GetString(attachment.Body);

                string _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
                if (xml.StartsWith(_byteOrderMarkUtf8))
                {
                    xml = xml.Remove(0, _byteOrderMarkUtf8.Length);
                }

                ECE.Letter deletter = ECE.Letter.DeSerialize(xml);

                var model = new OutLetter
                {
                    Subject = deletter.Subject,
                    OutLetterNumber = deletter.LetterNo,
                    OutLetterDate = deletter.LetterDateTime.Value,
                    SendDate = "----",
                };

                string letterID = outLetterServices.CreateOutLetter(model, null, null);
                return RedirectToAction("ShowOutLetters" , "OutLetter");
            }
            catch (Exception ex)
            {
                TempData["message"] = new ShowMessage
                {
                    Message = "error",
                    MessageType = ShowMessage.MessageTypes.error
                };

                return RedirectToAction("Details", new { id = id});
            }
        }
        #endregion
    }
}