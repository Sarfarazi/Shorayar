using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Core.Models
{
    public class ShowMessage
    {
        public enum MessageTypes
        {
            success = 0,
            info = 1,
            warning = 2,
            error = 3,
        }
        public ShowMessage()
        {

        }

        public string Message { get; set; }

        public MessageTypes MessageType { get; set; }
    }
}
