using Council.Core.Entities;
using Council.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Core.Models
{
     public class OutLetterItemModel
    {
        public OutLetter Letter { get; set; }
        public IList<User> Recivers { get; set; }
        public string LastReciverID { get; set; }
        public bool UserIsBoss { get; set; }
        public bool UserIsBossHelper { get; set; }
        public List<Commission> Commisions { get; set; }
        public List<DefaultStatement> Statements { get; set; }
        public LetterRefrences LastRefrence { get; set; }
        public UserPosition UserPosition { get; set; }
    }

    
}
