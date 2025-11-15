using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Utility.DateTimeConvert
{
    public class DateTimeConvert
    {
        public static DateTime tzIran()
        {
            TimeZoneInfo timeZoneInfo;
            DateTime dateTime;
            //Set the time zone information to US Mountain Standard Time 
            timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Iran Standard Time");

            dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
            return dateTime;
        }

        public static string PersianDate(DateTime InputDate)
        {
            PersianCalendar oPersianC = new PersianCalendar();
            string Day, Month, Year, Week, Date = "";

            if (InputDate != null)
            {
                Year = oPersianC.GetYear(InputDate).ToString();
                Month = oPersianC.GetMonth(InputDate).ToString();
                Week = oPersianC.GetDayOfWeek(InputDate).ToString();
                Day = oPersianC.GetDayOfMonth(InputDate).ToString();

                switch (Month)
                {
                    case "1":
                        Month = "فروردین";
                        break;
                    case "2":
                        Month = "اردیبهشت";
                        break;
                    case "3":
                        Month = "خرداد";
                        break;
                    case "4":
                        Month = "تیر";
                        break;
                    case "5":
                        Month = "مرداد";
                        break;
                    case "6":
                        Month = "شهریور";
                        break;
                    case "7":
                        Month = "مهر";
                        break;
                    case "8":
                        Month = "آبان";
                        break;
                    case "9":
                        Month = "آذر";
                        break;
                    case "10":
                        Month = "دی";
                        break;
                    case "11":
                        Month = "بهمن";
                        break;
                    case "12":
                        Month = "اسفند";
                        break;
                    default:
                        break;
                }

                switch (Week)
                {
                    case "Saturday":
                        Week = "شنبه";
                        break;
                    case "Sunday":
                        Week = "يک شنبه";
                        break;
                    case "Monday":
                        Week = "دوشنبه";
                        break;
                    case "Tuesday":
                        Week = "سه شنبه";
                        break;
                    case "Wednesday":
                        Week = "چهارشنبه";
                        break;
                    case "Thursday":
                        Week = "پنج شنبه";
                        break;
                    case "Friday":
                        Week = "جمعه";
                        break;
                    default:
                        break;
                }


                Date = Week + " " + Day + " " + Month + " " + Year;
            }
            else
            {
            }
            return Date;
        }

        public static string PersianNumDate(DateTime inputDate)
        {
            PersianCalendar oPersianC = new PersianCalendar();
            string Day, Month, Year, Date = "";

            if (inputDate != null)
            {
                Year = oPersianC.GetYear(inputDate).ToString();
                Month = oPersianC.GetMonth(inputDate).ToString();
                Day = oPersianC.GetDayOfMonth(inputDate).ToString();


                Date = Year + "/" + Month + "/" + Day;
            }
            else
            {
            }
            return Date;
        }

        public static string PersianDashNumDate(DateTime InputDate)
        {
            PersianCalendar oPersianC = new PersianCalendar();
            string Day, Month, Year, Date = "";

            if (InputDate != null)
            {
                Year = oPersianC.GetYear(InputDate).ToString();
                Month = oPersianC.GetMonth(InputDate).ToString();
                Day = oPersianC.GetDayOfMonth(InputDate).ToString();


                Date = Year + "-" + Month + "-" + Day;
            }
            else
            {
            }
            return Date;
        }

        public static string PersianMonthDate(DateTime InputDate)
        {
            PersianCalendar oPersianC = new PersianCalendar();
            string Day, Month, Year, Week, Date = "";

            if (InputDate != null)
            {
                Year = oPersianC.GetYear(InputDate).ToString();
                Month = oPersianC.GetMonth(InputDate).ToString();
                Week = oPersianC.GetDayOfWeek(InputDate).ToString();
                Day = oPersianC.GetDayOfMonth(InputDate).ToString();

                switch (Month)
                {
                    case "1":
                        Month = "فروردین";
                        break;
                    case "2":
                        Month = "اردیبهشت";
                        break;
                    case "3":
                        Month = "خرداد";
                        break;
                    case "4":
                        Month = "تیر";
                        break;
                    case "5":
                        Month = "مرداد";
                        break;
                    case "6":
                        Month = "شهریور";
                        break;
                    case "7":
                        Month = "مهر";
                        break;
                    case "8":
                        Month = "آبان";
                        break;
                    case "9":
                        Month = "آذر";
                        break;
                    case "10":
                        Month = "دی";
                        break;
                    case "11":
                        Month = "بهمن";
                        break;
                    case "12":
                        Month = "اسفند";
                        break;
                    default:
                        break;
                }
                Date = Month + " " + Year;
            }
            else
            {
            }
            return Date;
        }

        public static DateTime PersianToGregorian(string shamsi)
        {
            var Now = DateTime.Now;

            System.Globalization.PersianCalendar Mdate = new System.Globalization.PersianCalendar();
            DateTime MDate;
            string[] prdate = shamsi.Split('/');
            MDate = Mdate.ToDateTime(Convert.ToInt32((prdate[0])), Convert.ToInt32(prdate[1]), Convert.ToInt32(prdate[2]), Now.Hour, Now.Minute, Now.Second, Now.Millisecond, System.Globalization.GregorianCalendar.ADEra);
            return MDate;
        }

        public static DateTime PersianToGregorianFirstTime(string shamsi)
        {
            var Now = DateTime.Now;

            System.Globalization.PersianCalendar Mdate = new System.Globalization.PersianCalendar();
            DateTime MDate;
            string[] prdate = shamsi.Split('/');
            MDate = Mdate.ToDateTime(Convert.ToInt32((prdate[0])), Convert.ToInt32(prdate[1]), Convert.ToInt32(prdate[2]), 0, 0, 0, 0, System.Globalization.GregorianCalendar.ADEra);
            return MDate;
        }

        public static string[] PersianArrayDate(DateTime InputDate)
        {
            string Day, Month, Year, Week, ArDay, ArMonth, ArYear, EnMonth = "";
            String[] Date = new String[4];

            PersianCalendar oPersianC = new PersianCalendar();
            Calendar ArabicDate = new HijriCalendar();

            if (InputDate != null)
            {
                //persian
                Year = oPersianC.GetYear(InputDate).ToString();
                Month = oPersianC.GetMonth(InputDate).ToString();
                Week = oPersianC.GetDayOfWeek(InputDate).ToString();
                Day = oPersianC.GetDayOfMonth(InputDate).ToString();

                switch (Month)
                {
                    case "1":
                        Month = "فروردین";
                        break;
                    case "2":
                        Month = "اردیبهشت";
                        break;
                    case "3":
                        Month = "خرداد";
                        break;
                    case "4":
                        Month = "تیر";
                        break;
                    case "5":
                        Month = "مرداد";
                        break;
                    case "6":
                        Month = "شهریور";
                        break;
                    case "7":
                        Month = "مهر";
                        break;
                    case "8":
                        Month = "آبان";
                        break;
                    case "9":
                        Month = "آذر";
                        break;
                    case "10":
                        Month = "دی";
                        break;
                    case "11":
                        Month = "بهمن";
                        break;
                    case "12":
                        Month = "اسفند";
                        break;
                    default:
                        break;
                }

                string PesianDate = Day + " " + Month + " " + Year;

                //hijri 
                ArDay = ArabicDate.GetDayOfMonth(InputDate).ToString();
                ArMonth = ArabicDate.GetMonth(InputDate).ToString();
                ArYear = ArabicDate.GetYear(InputDate).ToString();

                switch (ArMonth)
                {
                    case "1":
                        ArMonth = "محرم";
                        break;
                    case "2":
                        ArMonth = "صفر";
                        break;
                    case "3":
                        ArMonth = "ربیع الاول";
                        break;
                    case "4":
                        ArMonth = "ربیع الثانی";
                        break;
                    case "5":
                        ArMonth = "جمادی الاول";
                        break;
                    case "6":
                        ArMonth = "جمادی الثانی";
                        break;
                    case "7":
                        ArMonth = "رجب";
                        break;
                    case "8":
                        ArMonth = "شعبان";
                        break;
                    case "9":
                        ArMonth = "رمضان";
                        break;
                    case "10":
                        ArMonth = "شوال";
                        break;
                    case "11":
                        ArMonth = "ذی القعده";
                        break;
                    case "12":
                        ArMonth = "ذی الحجه";
                        break;
                    default:
                        break;
                }

                string ArDate = ArDay + " " + ArMonth + " " + ArYear;

                //georgian
                EnMonth = InputDate.Month.ToString();
                switch (EnMonth)
                {
                    case "1":
                        EnMonth = "ژانویه";
                        break;
                    case "2":
                        EnMonth = "فوریه";
                        break;
                    case "3":
                        EnMonth = "مارچ";
                        break;
                    case "4":
                        EnMonth = "آوریل";
                        break;
                    case "5":
                        EnMonth = "می";
                        break;
                    case "6":
                        EnMonth = "ژوئن";
                        break;
                    case "7":
                        EnMonth = "ژولای";
                        break;
                    case "8":
                        EnMonth = "آگوست";
                        break;
                    case "9":
                        EnMonth = "سپتامبر";
                        break;
                    case "10":
                        EnMonth = "اکتبر";
                        break;
                    case "11":
                        EnMonth = "نوامبر";
                        break;
                    case "12":
                        EnMonth = "دسامبر";
                        break;
                    default:
                        break;
                }

                string EnDate = InputDate.Day.ToString() + " " + EnMonth + " " + InputDate.Year.ToString();

                switch (Week)
                {
                    case "Saturday":
                        Week = "شنبه";
                        break;
                    case "Sunday":
                        Week = "يک شنبه";
                        break;
                    case "Monday":
                        Week = "دوشنبه";
                        break;
                    case "Tuesday":
                        Week = "سه شنبه";
                        break;
                    case "Wednesday":
                        Week = "چهارشنبه";
                        break;
                    case "Thursday":
                        Week = "پنج شنبه";
                        break;
                    case "Friday":
                        Week = "جمعه";
                        break;
                    default:
                        break;
                }

                Date[0] = PesianDate;
                Date[1] = ArDate;
                Date[2] = EnDate;

                Date[3] = Week;
            }
            return Date;
        }


        public static string PersianNumDateWithHours(DateTime inputDate)
        {
            PersianCalendar oPersianC = new PersianCalendar();
            string Day, Month, Year, Date = "";

            if (inputDate != null)
            {
                Year = oPersianC.GetYear(inputDate).ToString();
                Month = oPersianC.GetMonth(inputDate).ToString();
                Day = oPersianC.GetDayOfMonth(inputDate).ToString();


                Date = string.Format("({3}:{4}:{5}) {0}/{1}/{2}", Year, Month, Day, inputDate.Hour, inputDate.Minute, inputDate.Second);
            }
            return Date;
        }
    }
}
