using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Council.Service
{
    public static class ECEMethods
    {
        public static bool SendECE(string fileName)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                mail.From = new MailAddress("zohreh.salamzadeh@gmail.com");
                mail.To.Add("zohreh.salamzadeh@gmail.com");
                //mail.To.Add("toaddress2@gmail.com");
                mail.Subject = "Password Recovery ";
                mail.Body += " <html>";
                mail.Body += "<body>";
                mail.Body += "<table>";
                mail.Body += "<tr>";
                mail.Body += "<td>User Name : </td><td> HAi </td>";
                mail.Body += "</tr>";

                mail.Body += "<tr>";
                mail.Body += "<td>Password : </td><td>aaaaaaaaaa</td>";
                mail.Body += "</tr>";
                mail.Body += "</table>";
                mail.Body += "</body>";
                mail.Body += "</html>";
                mail.IsBodyHtml = true;
                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("zohreh.salamzadeh@gmail.com", "ZohrehS@1358");
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static string Pdftobase64(string file)
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
        public static string EncodeTo64(string toEncode)

        {

            byte[] toEncodeAsBytes

                  = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);

            string returnValue

                  = System.Convert.ToBase64String(toEncodeAsBytes);

            return returnValue;

        }
        public static string DecodeFrom64(string encodedData)

        {

            byte[] encodedDataAsBytes

                = System.Convert.FromBase64String(encodedData);

            string returnValue =

               System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);

            return returnValue;

        }
        public static string ImageToBase64(string filePath)
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
        public static System.Drawing.Image Base64ToImage(string imageBase64)
        {
            byte[] imageBytes = Convert.FromBase64String(imageBase64);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            ms.Write(imageBytes, 0, imageBytes.Length);
            System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
            return image;
        }
        public static string CreateBase64(string filename)
        {
            return Convert.ToBase64String(File.ReadAllBytes(filename));
        }
    }
}
