using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Core.Enums
{
    public enum RefrenceType
    {
        Normal = 0,
        SendToBoss,
        SendToCommission,
        SendToMembers,

        Other,
        SendToCommissionMembers,
        EndOfLetter = 10 // اختتام نامه عادی
    }
}
