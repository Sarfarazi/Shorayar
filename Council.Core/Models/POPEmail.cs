using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Council.Core.Models
{
    [Serializable]
    public class POPEmail
    {
        public POPEmail()
        {
            this.Attachments = new List<Attachment>();
        }
        public int MessageNumber { get; set; }

        public string FromEmail { get; set; }

        [AllowHtml]
        public string From { get; set; }

        public string To { get; set; }

        [AllowHtml]
        public string Subject { get; set; }

        [AllowHtml]
        public string Body { get; set; }

        public DateTime DateSent { get; set; }

        [AllowHtml]
        public List<Attachment> Attachments { get; set; }
    }

    [Serializable]
    public class Attachment
    {
        public string FileId { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
    }


    [Serializable]
    public class POPEmailDetails
    {
        public POPEmailDetails()
        {
            this.Letter = new LetterInfo();
        }
        public int MessageNumber { get; set; }

        public string FromEmail { get; set; }

        [AllowHtml]
        public string From { get; set; }

        public string To { get; set; }

        [AllowHtml]
        public string Subject { get; set; }

        [AllowHtml]
        public string Body { get; set; }

        public DateTime DateSent { get; set; }

        [AllowHtml]
        public LetterInfo Letter { get; set; }
    }
}