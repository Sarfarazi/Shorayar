using Council.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Core.Interfaces
{
    public interface IUniqueNumber : ICRUD<UniqueNumber>
    {
        string GetUniqueNumber();
        string GetLastTodayUniqueCode(string dateNumber);
        string GetDateNumber();
        UniqueNumber GetUniqueNumber(string dateNumber, string code);
        UniqueNumber GetUniqueNumber(string fullNumber);
        string SetCodeFourDigit(int code,int Lenght);
    }
}
