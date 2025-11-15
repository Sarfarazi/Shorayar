using Council.Service;
using Council.Service.DBServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Council.UI.Helpers
{
    public static class PublicHelpers
    {
        public static string CurrentUserFullName(this HtmlHelper html, string appId)
        {
            UserServices userService = new UserServices();
            return userService.CurrentNameAndLastName(appId);
        }
        public static string LetterIdOfOutLetter(this HtmlHelper html, string outLetterID)
        {
            LetterServices letterService = new LetterServices();
            return letterService.GetLetterOfOutLetter(outLetterID);
        }
        public static MvcHtmlString GetPersianDate(this HtmlHelper html, DateTime dateTime)
        {
            return MvcHtmlString.Create(new PublicMetods().ConvertToJalali(dateTime));
        }
        public static MvcHtmlString GetPersonNameWithAppID(this HtmlHelper html, string UserID)
        {
            UserServices userService = new UserServices();
            var user = userService.All().FirstOrDefault(u => u.ID == UserID);
            return user == null ? MvcHtmlString.Create("") : MvcHtmlString.Create(user.FirstName + " " + user.LastName);
        }
        public static MvcHtmlString ContentSubstring(this HtmlHelper html, string content, int numberOfChars = 700)
        {
            content = Regex.Replace(content, @"<[^>]+>|&nbsp;", "").Trim();

            if (content.Length > numberOfChars)
                return MvcHtmlString.Create(content.Substring(0, numberOfChars) + "...");
            else return MvcHtmlString.Create(content);
        }
        public static MvcHtmlString ContentSubstring(this HtmlHelper html, string content)
        {
            content =!string.IsNullOrEmpty(content)?Regex.Replace(content, @"<[^>]+>|&nbsp;", "").Trim():"";
            int numberOfChars = 350;
            if (content.Length > numberOfChars)
                return MvcHtmlString.Create(content.Substring(0, numberOfChars) + "...");
            else return MvcHtmlString.Create(content);
        }
        public static string RenderRazorViewToString(this Controller controller, string viewName, object model)
        {
            controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(controller.ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}