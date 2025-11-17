using Council.Core.Entities;
using Council.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Core.Models
{
    public  class LetterModels
    {
        public bool Received { get; set; }
       
        public string Subject { get; set; }

       
        public string OutLetterNumber { get; set; }

       
        public string Number { get; set; }

       
        public bool ConfirmBoss { get; set; }

      public string LogoName { get; set; }
        public string RegisterNumber { get; set; }

        public string OutLetterDate { get; set; }

      
        public string SendDate { get; set; }

       
        public string Reciver { get; set; }

       
        public string Sender { get; set; }

       
        public string Transferee { get; set; }

        
        public string Bringer { get; set; }

        public string BeforeNumber { get; set; }

        
        public string NextNumber { get; set; }

        
        public string LinkedLetters { get; set; }

    
        public string OrginalLetter { get; set; }

      
        public string CopyTo { get; set; }

        public string BoxNumber { get; set; }

       
        public string ArchiveCode { get; set; }

        public string Comment { get; set; }

        public string Files { get; set; }
        public bool Archived { get; set; }
        public bool Removed { get; set; }
        public string LetterID { get; set; }
        public byte OutLetterStatus { get; set; }
        public string Organ { get; set; }


    }
    public class LetterItemModel
    {
        public Letter Letter { get; set; }
        public IList<User> Recivers { get; set; }
        public string LastReciverID { get; set; }
        public bool UserIsBoss { get; set; }
        public bool UserIsBossHelper { get; set; }
        public List<Commission> Commisions { get; set; }
        public List<DefaultStatement> Statements { get; set; }
        public LetterRefrences LastRefrence { get; set; }
        public UserPosition UserPosition { get; set; }
    }

    public class IndexLetter
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public int IsNew { get; set; }
        public string LetterNumber { get; set; }
        public string CommissionId { get; set; }
        public DateTime CreatedOn { get; set; }
        public LetterStatus LetterStatus { get; set; }
        public int? IsRule { get; set; }
        public string CommissionName { get; set; }
    }

    public class MyLetter
    {
        public string ID { get; set; }
        public string Title { get; set;}
        public DateTime CreatedOn { get; set; }
        public string LastReciverID { get; set; }
        public bool Archived { get; set; }
        public bool Deleted { get; set; }
        public string SenderAppID { get; set; }
        public DateTime LastRefrenceDate { get; set; }
        public string LetterNumber { get; set; }
        public int LetterStatus { get; set; }
        public string OutLetter_ID { get; set; }
        public bool Urgency { get; set; }
        public bool HasLetterNote { get; set; }
        public bool HasOutLetterNote { get; set; }
        public RefrenceType? LetterRefrenceType { get; set; }

    }
    public class DisplayModel
    {
        public List<MyLetter> AllLetter { get; set; }
        public List<MyLetter> CouncilLetter { get; set; }
        public List<MyLetter> RemovedLetter { get; set; }
        public List<MyLetter> ArchivedLetter { get; set; }
        public List<MyLetter> OutLetter { get; set; }
        public List<MyLetter> OutOfReadyCouncilLetter { get; set; }

    }
}
