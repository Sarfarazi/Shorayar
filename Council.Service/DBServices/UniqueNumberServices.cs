using Council.Core.Entities;
using Council.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Service.DBServices
{

    public class UniqueNumberServices : CRUDServices<UniqueNumber>, IUniqueNumber
    {
        public UniqueNumberServices()
            : base("MainContext")
        {

        }
        PublicMetods publicMethods = new PublicMetods();
       
        long ConvertStringToTicks(string value)
        {
            return DateTime.Parse(value).Ticks;
        }
        public string GetLastTodayUniqueCode(string dateNumber)
        {
            UniqueNumber uniqueNumber = this.Where(u => u.DateNumber == dateNumber).OrderByDescending(u => u.CreatedOn).FirstOrDefault();
            if (uniqueNumber == null)
                return "0";

            return uniqueNumber.Code;
        }


        public string GetLastUniqueCode(int LetterType)
        {

            var serverDateTime = publicMethods.ServerDateTime();
            string y= publicMethods.GetYearShamsi(serverDateTime);
            UniqueNumber uniqueNumber = this.Where(u => u.LetterType == LetterType && u.DateNumber==y).OrderByDescending(u => u.CreatedOn).FirstOrDefault();
            if (uniqueNumber == null)
                return "0";

            return uniqueNumber.Code;
        }
        public string GetLastUniqueCodeForFlow(string LetterNumber)
        {
            UniqueNumber uniqueNumber = this.Where(u => u.LetterType == 3 && u.FullNumber==LetterNumber).OrderByDescending(u => u.CreatedOn).FirstOrDefault();
            if (uniqueNumber == null)
                return "0";

            return uniqueNumber.Code;
        }
        public string GetDateNumber()
        {
            var serverDateTime = publicMethods.ServerDateTime();
            return publicMethods.ConvertToJalali(serverDateTime).Remove(0, 2).Replace(".", "").Replace("/", "");
        }

        public string GetYearNumber()
        {
            var serverDateTime = publicMethods.ServerDateTime();
            return publicMethods.GetYearShamsi(serverDateTime);
        }

        public UniqueNumber GetUniqueNumber(string dateNumber, string code)
        {
            return this.Where(u => u.DateNumber == dateNumber && u.Code == code).FirstOrDefault();
        }

        public UniqueNumber GetUniqueNumber(string fullNumber)
        {
            return this.Where(u => u.FullNumber == fullNumber).FirstOrDefault();
        }

        public string GetUniqueNumber()
        {
            string dateNumber = GetDateNumber();
            string lastTodayUniqueCode = GetLastTodayUniqueCode(dateNumber);
            string code = SetCodeFourDigit(int.Parse(lastTodayUniqueCode),4);


            UniqueNumber uniqueNumber = new UniqueNumber()
            {
                Code = code,
                DateNumber = dateNumber,
                FullNumber = dateNumber + code
            };
            this.Create(uniqueNumber);
            return uniqueNumber.FullNumber;
        }

        public string GetUniqueNumberType(int TypeLetter)
        {

            // 1 Sended 
            // 2 Recive
            // 3 Flow

            string dateNumber = GetYearNumber();
            string lastTodayUniqueCode = GetLastUniqueCode(TypeLetter);
            string code = SetCodeFourDigit(int.Parse(lastTodayUniqueCode),4);


            UniqueNumber uniqueNumber = new UniqueNumber()
            {
                Code = code,
                DateNumber = dateNumber,
                FullNumber = code,
              //  FullNumber =TypeLetter ==2? dateNumber + code+ "/و" : dateNumber + code+ "/ص" ,
                 LetterType=TypeLetter
                 
            };
            this.Create(uniqueNumber);
            return uniqueNumber.FullNumber;
        }

        public string GetUniqueNumberForFlowLetter(string LetterNumber)
        {

            string dateNumber = GetYearNumber();
            string lastTodayUniqueCode = GetLastUniqueCodeForFlow(LetterNumber);
            string code = SetCodeFourDigit(int.Parse(lastTodayUniqueCode),2);


            UniqueNumber uniqueNumber = new UniqueNumber()
            {
                Code = code,
                DateNumber = dateNumber,
                FullNumber =LetterNumber+"-"+ code,
                //  FullNumber =TypeLetter ==2? dateNumber + code+ "/و" : dateNumber + code+ "/ص" ,
                LetterType = 3

            };
            this.Create(uniqueNumber);
            return uniqueNumber.FullNumber;
        }

        public string SetCodeFourDigit(int code, int Lenght)
        {
            code++;
            string result = "";
            result = code.ToString();
            if (result.Length < Lenght)
                while (result.Length < Lenght)
                    result = "0" + result;

            return result;
        }
    }
}
