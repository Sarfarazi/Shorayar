using Council.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Drawing;
using System.Xml;
using System.Net.Mail;
using Council.Core.Models;

namespace Council.Service
{
    public class PublicMetods
    {
        static Random _random = new Random();
        public int RandomNumber(int maxValue = 99, int minValue = 10)
        {
            return _random.Next(minValue, maxValue);
        }
        public string UploadFile(HttpPostedFileBase file, string subPath)
        {
            if (file != null )
            {
                if(!file.ContentType.Contains("audio") && !file.ContentType.Contains("video") && !file.ContentType.Contains("html"))
                {
                    string monthFolderName = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString();
                     string path = "/Upload/" + subPath + "/" + monthFolderName + "/Images/";
                   //string path = "/Upload/Signature/";
                    string extension = Path.GetExtension(file.FileName);
                    string randomNumber = RandomNumber().ToString();
                    string fullPath = HttpContext.Current.Server.MapPath(path);
                    string fileName = DateTime.Now.Ticks.ToString() + randomNumber + extension;
                    createDicIfNotExist(fullPath);
                    file.SaveAs(fullPath + fileName);                   
                    return path + fileName;
                }else
                {
                    return "1";
                }                
            }
            return "";
        }
        public string UploadSignature(HttpPostedFileBase file, string subPath)
        {
            if (file != null)
            {
                if (!file.ContentType.Contains("audio") && !file.ContentType.Contains("video") && !file.ContentType.Contains("html"))
                {
                    string monthFolderName = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString();
                   // string path = "/Upload/" + subPath + "/" + monthFolderName + "/Images/";
                    string path = "/Upload/Signature/";
                    string extension = Path.GetExtension(file.FileName);
                    string randomNumber = RandomNumber().ToString();
                    string fullPath = HttpContext.Current.Server.MapPath(path);
                    string fileName = DateTime.Now.Ticks.ToString() + randomNumber + extension;
                    createDicIfNotExist(fullPath);
                    file.SaveAs(fullPath + fileName);
                    return  fileName;
                }
                else
                {
                    return "1";
                }
            }
            return "";
        }
        public IList<SmallUserInfo> SmallUsersImageToThumb(IList<SmallUserInfo> users)
        {
            foreach (var item in users)
            {
                if (!String.IsNullOrEmpty(item.Picture))
                {
                    var _index = item.Picture.LastIndexOf("/");
                    var _imageName = item.Picture.Remove(0, _index + 1);
                    var _Path = item.Picture.Remove(_index + 1, (item.Picture.Length - (_index + 1)));
                    item.Picture = _Path + "Thumbs/" + _imageName;
                }
            }
            return users;
        }
        public string UploadXmlFile(XmlDocument doc)
        {
            if (doc != null)
            {               
                string monthFolderName = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString();
                string path = "/Upload/XML/" + monthFolderName + "/";
                // string extension = Path.GetExtension("xml");
                string randomNumber = RandomNumber().ToString();
                string fullPath = HttpContext.Current.Server.MapPath(path);
                string fileName = DateTime.Now.Ticks.ToString() + randomNumber + ".xml";
                createDicIfNotExist(fullPath);
                doc.Save(fullPath + fileName);
                
                return path + fileName;
            }
            return "";
        }
        void createDicIfNotExist(string dic)
        {
            if (!Directory.Exists(dic))
            {
                Directory.CreateDirectory(dic);
            }
        }
        public void DeleteFile(string fullPath)
        {
            var path = HttpContext.Current.Server.MapPath(fullPath);
            File.Delete(path);
        }
        public DateTime ConvertToMiladi(DateTime jalaliDate)
        {
            var pc = new PersianCalendar();
            return pc.ToDateTime(jalaliDate.Year, jalaliDate.Month, jalaliDate.Day, 0, 0, 0, 0);
        }
        public string ConvertToJalali(DateTime miladiDate)
        {
            PersianCalendar persianDate = new PersianCalendar();
            var persianYear = persianDate.GetYear(miladiDate).ToString();
            var persianMonth = persianDate.GetMonth(miladiDate).ToString();
            var persianDay = persianDate.GetDayOfMonth(miladiDate).ToString();
            persianMonth = persianMonth.Length < 2 ? "0" + persianMonth : persianMonth;
            persianDay = persianDay.Length < 2 ? "0" + persianDay : persianDay;
            return String.Format("{0}/{1}/{2}", persianYear, persianMonth, persianDay); ;
        }
        public string toPersianNumber(string input)
        {
            string[] persian = new string[10] { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };
            for (int j = 0; j < persian.Length; j++)
            {
                input = input.Replace(j.ToString(), persian[j]);
            }
            return input;
        }
        public string GetYearShamsi(DateTime miladiDate)
        {
            PersianCalendar persianDate = new PersianCalendar();
            var persianYear = persianDate.GetYear(miladiDate).ToString();
            return persianYear;

        }
        public DateTime ServerDateTime()
        {
            DataBase dataBase = new DataBase();

            string result = dataBase.ServerDateTime();
            return DateTime.Parse(result);
        }
        public string CreateSQLQueryForSP(string spName, Dictionary<string, object> _params = null)
        {
            string query = "EXEC " + spName + " ";
            bool haveSplitor = false;
            if (_params != null)
                foreach (var param in _params)
                {
                    var splitor = (haveSplitor ? "," : "");
                    if (param.Value.GetType() == typeof(String))
                        query += splitor + param.Key + "=N'" + param.Value + "'";
                    else if (param.Value.GetType() == typeof(Boolean))
                        query += splitor + param.Key + "=" + ((bool)param.Value == true ? "1" : "0");
                    else
                        query += splitor + param.Key + "=" + param.Value.ToString();

                    query += " ";
                    haveSplitor = true;
                }
            return query;
        }
        //public string GetFileScaner()
        //{
        //    var deviceManager = new DeviceManager();

        //    // Create an empty variable to store the scanner instance
        //    DeviceInfo firstScannerAvailable = null;

        //    // Loop through the list of devices to choose the first available
        //    for (int i = 1; i <= deviceManager.DeviceInfos.Count; i++)
        //    {
        //        // Skip the device if it's not a scanner
        //        if (deviceManager.DeviceInfos[i].Type != WiaDeviceType.ScannerDeviceType)
        //        {
        //            continue;
        //        }

        //        firstScannerAvailable = deviceManager.DeviceInfos[i];

        //        break;
        //    }

        //    // Connect to the first available scanner
        //    var device = firstScannerAvailable.Connect();

        //    // Select the scanner
        //    var scannerItem = device.Items[1];

        //    // Retrieve a image in JPEG format and store it into a variable
        //    var imageFile = (ImageFile)scannerItem.Transfer(FormatID.wiaFormatJPEG);

        //    // Save the image in some path with filename
        //    var path = @"C:\Users\<username>\Desktop\scan.jpeg";

        //    if (File.Exists(path))
        //    {
        //        File.Delete(path);
        //    }

        //    // Save image !
        //    imageFile.SaveFile(path);
        //    return path;
        //}

    }
}
