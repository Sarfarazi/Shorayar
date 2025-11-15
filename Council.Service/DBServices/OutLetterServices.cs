using Council.Core.Entities;
using Council.Core.Interfaces;
using Council.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace Council.Service.DBServices
{

    public class OutLetterServices : CRUDServices<OutLetter>, IOutLetter
    {
        PublicMetods publicMethods = new PublicMetods();
        PeriodService periodService = new PeriodService();
        NoteService NoteService = new NoteService();
        UserServices userService=new UserServices();
        public string CreateOutLetter(OutLetter outLetterSpecs, HttpPostedFileBase file, List<HttpPostedFileBase> uploads)
        {
            string fileName= publicMethods.UploadFile(file, "OutLetter");
            if(fileName != "1")
            {
                var activePeriod = periodService.All().Where(m => m.IsActive).FirstOrDefault();
                outLetterSpecs.Files = fileName;
                outLetterSpecs.Uploads = Uploads(uploads);
                outLetterSpecs.CouncilPeriod = activePeriod;
                outLetterSpecs.ConfirmBoss = false;
                OutLetter Result=  this.Create(outLetterSpecs);
                return Result.ID;
               // return "  نامه ثبت شد";
            }
            return "error";
        }

        public string CreateOutLetterScaned(OutLetter outLetterSpecs, string fileName)
        {
            string monthFolderName = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString();
            string FinalPath = HttpContext.Current.Server.MapPath(string.Format("/Upload/OutLetter/{0}/{1}" , monthFolderName , fileName));
            string ScPath = HttpContext.Current.Server.MapPath("~/ScTemp/"+ fileName);

            if (fileName != null && System.IO.File.Exists(ScPath))
            {
                var activePeriod = periodService.All().Where(m => m.IsActive).FirstOrDefault();
                outLetterSpecs.Files = fileName;
                outLetterSpecs.CouncilPeriod = activePeriod;
                this.Create(outLetterSpecs);

                if (System.IO.File.Exists(ScPath))
                {
                    System.IO.File.Move(ScPath, FinalPath);
                }

                return "نامه ثبت شد";
            }
            return "error";
        }

        public string DeleteOutLetter(string outLetterId)
        {
            All().First(o => o.ID == outLetterId).Deleted = true;
            Save();

            return "حذف شد";
        }
        public string EditOutLetter(OutLetter outLetter, List<HttpPostedFileBase> uploads)
        {
            var oldItem = Get<string>(outLetter.ID);
            //oldItem.Uploads.
            if (uploads != null)
                foreach (var upload in uploads)
                {
                    var filename = publicMethods.UploadFile(upload, "OutLetter");
                    oldItem.Uploads.Add(new Upload { FileAddress = filename!="1"?filename:"" });
                }
            oldItem.ArchiveCode = outLetter.ArchiveCode;
            oldItem.Archived = outLetter.Archived;
            oldItem.BeforeNumber = outLetter.BeforeNumber;
            oldItem.BoxNumber = outLetter.BoxNumber;
            oldItem.Bringer = outLetter.Bringer;
            oldItem.Comment = outLetter.Comment;
            oldItem.LetterID = null;
            oldItem.LinkedLetters = outLetter.LinkedLetters;
            oldItem.NextNumber = outLetter.NextNumber;
            oldItem.OrginalLetter = outLetter.OrginalLetter;
            oldItem.OutLetterDate = outLetter.OutLetterDate;
            oldItem.OutLetterNumber = outLetter.OutLetterNumber;
            oldItem.Received = outLetter.Received;
            oldItem.Reciver = outLetter.Reciver;
            oldItem.RegisterNumber = outLetter.RegisterNumber;
            oldItem.Removed = outLetter.Removed;
         //   oldItem.SendDate = outLetter.SendDate;
            oldItem.Sender = outLetter.Sender;
            oldItem.Subject = outLetter.Subject;
            oldItem.Transferee = outLetter.Transferee;
            oldItem.Organ = outLetter.Organ;
            oldItem.OutLetterStatus = outLetter.OutLetterStatus;

            Update(oldItem);
            return "ذخیره شد";
        }


        public string DeleteOutLetter(OutLetter outLetter)
        {
            var oldItem = Get<string>(outLetter.ID);
            oldItem.Organ = null;
            oldItem.Deleted = true;

            Update(oldItem);
            return "ذخیره شد";
        }

        List<Upload> Uploads(List<HttpPostedFileBase> uploads)
        {
            if (uploads == null)
                return null;
            var result = new List<Upload>();
            foreach (var item in uploads)
                if (item != null && !item.ContentType.Contains("audio") && !item.ContentType.Contains("video") && !item.ContentType.Contains("html")) 
                    {
                    var filename = publicMethods.UploadFile(item, "OutLetter");
                    result.Add(new Upload
                    {

                        FileAddress = filename != "1" ? filename : ""
                    });
                }

            if (result.Count > 0)
                return result;

            return null;
        }
        public List<Upload> UploadXmlFiles(XmlDocument doc)
        {
            if (doc == null)
                return null;
            var result = new List<Upload>();

            result.Add(new Upload
            {
                FileAddress = publicMethods.UploadXmlFile(doc)
            });
            return result;
        }

      public IEnumerable< NoteLetterViewModel> GetNoteLetterInfo(string letterId)
        {
            var Notes = this.Get<string>(letterId).Notes;
           
            var result = Notes.Select(o=> new NoteLetterViewModel { 
                  FirstName=userService.GetUserByID(o.WriterId).FirstName,
                  LastName=userService.GetUserByID(o.WriterId).LastName,
                  CreatedOn=o.CreatedOn,
                 // WriterId=o.WriterId,
                  ContentText=o.ContentText, 
            }).ToList();
            
         
            return result;

        }

        public string MarkAsArchived(string ID)
        {
            var item = this.Get<string>(ID);
            item.Archived = true;
            this.Save();
            return "نامه بایگانی شد";
        }

        public string MarkAsUnArchived(string ID)
        {
            var item = this.Get<string>(ID);
            item.Archived = false;
            this.Save();
            return "نامه از بایگانی خارج شد";
        }

        public string MarkAsRemoved(string ID)
        {
            var item = this.Get<string>(ID);
            item.Removed = true;
            this.Save();
            return "نامه حذف شد";
        }
        public string UndoOutLetterRemoved(string ID)
        {
            var item = this.Get<string>(ID);
            item.Deleted = false;
            this.Save();
            return "";
        }

        public IQueryable<OutLetter> Search(string filter)
        {
            return All().Where(m => m.Bringer.Contains(filter)
                || m.Subject.Contains(filter)
                || m.Comment.Contains(filter)
                || m.OutLetterNumber.Contains(filter)
                || m.Reciver.Contains(filter)
                || m.Sender.Contains(filter)
                || m.Subject.Contains(filter)
                || m.Transferee.Contains(filter)
                || m.Number.Contains(filter)
                || m.OutLetterDate.Contains(filter)
                || m.SendDate.Contains(filter)
                || m.Bringer.Contains(filter)
                || m.Files.Contains(filter)
                || m.RegisterNumber.Contains(filter)

                ).OrderByDescending(l => l.CreatedOn);
        }
        public IQueryable<OutLetter> SearchLaws(string filter)
        {
            return All().Where(m=>m.OutLetterStatus>=4 && m.OutLetterStatus<=6)
                .Where(m => m.Bringer.Contains(filter)
                || m.Subject.Contains(filter)
                || m.Comment.Contains(filter)
                || m.OutLetterNumber.Contains(filter)
                || m.Reciver.Contains(filter)
                || m.Sender.Contains(filter)
                || m.Subject.Contains(filter)
                || m.Transferee.Contains(filter)
                || m.Number.Contains(filter)
                ).OrderByDescending(l => l.CreatedOn);
        }
        public IQueryable<OutLetter> SearchPlans(string filter)
        {
            return All().Where(m => m.OutLetterStatus==2)
                .Where(m => m.Bringer.Contains(filter)
                || m.Subject.Contains(filter)
                || m.Comment.Contains(filter)
                || m.OutLetterNumber.Contains(filter)
                || m.Reciver.Contains(filter)
                || m.Sender.Contains(filter)
                || m.Subject.Contains(filter)
                || m.Transferee.Contains(filter)
                || m.Number.Contains(filter)
                ).OrderByDescending(l => l.CreatedOn);
        }
        public bool SendECE()
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("mail.shorayar.com");

                mail.From = new MailAddress("");
                mail.To.Add("zohreh.salamzadeh@gmail.com");
                mail.Subject = "Contact Us Email";
                mail.Body = "";
                System.Net.Mail.Attachment attachment;
                attachment = new System.Net.Mail.Attachment("your attachment file");
                mail.Attachments.Add(attachment);
                SmtpServer.Port = 25;
                SmtpServer.Send(mail);

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public string Pdftobase64(string file)
        {
            string ConvertedText = null;
            using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader r = new BinaryReader(fs))
                {
                    byte[] docBytes = r.ReadBytes(Convert.ToInt32(r.BaseStream.Length));
                    ConvertedText = Convert.ToBase64String(docBytes);
                }
            }
            return ConvertedText;
        }
        public string EncodeTo64(string toEncode)

        {

            byte[] toEncodeAsBytes

                  = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);

            string returnValue

                  = System.Convert.ToBase64String(toEncodeAsBytes);

            return returnValue;

        }
        public string DecodeFrom64(string encodedData)

        {

            byte[] encodedDataAsBytes

                = System.Convert.FromBase64String(encodedData);

            string returnValue =

               System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);

            return returnValue;

        }
        public string ImageToBase64(string filePath)
        {
            string path = "D:\\SampleImage.jpg";
            using (System.Drawing.Image image = System.Drawing.Image.FromFile(filePath))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();
                    return Convert.ToBase64String(imageBytes);
                }
            }
        }
        public System.Drawing.Image Base64ToImage(string imageBase64 )
        {
            byte[] imageBytes = Convert.FromBase64String(imageBase64);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            ms.Write(imageBytes, 0, imageBytes.Length);
            System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
            return image;
        }
    }
}
