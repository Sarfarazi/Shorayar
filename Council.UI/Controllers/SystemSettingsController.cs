using Council.Core.Entities;
using Council.Core.Models;
using Council.Service.DBServices;
using Council.UI.CustomAuthentication;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Council.UI.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class SystemSettingsController : Controller
    {
        SystemSettingsService settingService = new SystemSettingsService();

        public ActionResult Index()
        {
            ViewBag.Settings = settingService.Where(m => !m.Deleted).ToList();
            return View("Index", null);
        }

        #region ویرایش اطلاعات 
        [HttpGet]
        public ActionResult Edit(string id)
        {
            ViewBag.Organs = settingService.Where(m => !m.Deleted).ToList();
            var current = settingService.Get<string>(id);
            var model = new UpdateSettings
            {
                ID = id,
                LogoName = current.CouncilLogo,                
                CouncilName = current.CouncilName,
                Used = current.Used
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UpdateSettings model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var settings = settingService.Get<string>(model.ID);

            if (model.CouncilLogo != null)
            {
                //delete old logo
                if (System.IO.File.Exists(Server.MapPath("~/Images/logo/" + settings.CouncilLogo)))
                {
                    System.IO.File.Delete(Server.MapPath("~/Images/logo/" + settings.CouncilLogo));
                }

                string FileName = string.Format("{0}-{1}", DateTime.Now.Ticks, model.CouncilLogo.FileName);
                string Location = "~/Images/logo/" + FileName;
                model.CouncilLogo.SaveAs(Server.MapPath(Location));

                settings.CouncilLogo = FileName;
            }

            if (model.CouncilLoginLogo != null)
            {
                //delete old logo
                if (System.IO.File.Exists(Server.MapPath("~/Images/logo/" + settings.CouncilLoginLogo)))
                {
                    System.IO.File.Delete(Server.MapPath("~/Images/logo/" + settings.CouncilLoginLogo));
                }

                string LoginFileName = string.Format("{0}-{1}", DateTime.Now.Ticks, model.CouncilLoginLogo.FileName);
                string LoginLogoLocation = "~/Images/logo/" + LoginFileName;
                model.CouncilLoginLogo.SaveAs(Server.MapPath(LoginLogoLocation));

                settings.CouncilLoginLogo = LoginFileName;
            }

            if (model.Used)
            {
                settingService.DeactiveAllSettings();
                settings.Used = true;
            }
            else if(settingService.ISCurrentSetting(model.ID))
            {
                ModelState.AddModelError("Used", "قبل از تغییر تنظیمات، لطفا تنظیمات پیش فرض را مشخص کنید");
                return View(model);
            }
            else
            {
                settings.Used = false;
            }

            settings.CouncilName = model.CouncilName;       

            settingService.Update(settings);

            TempData["message"] = new ShowMessage
            {
                Message = "عملیات با موفقیت انجام شد",
                MessageType = ShowMessage.MessageTypes.success
            };
            return RedirectToAction("Index");
        }
        #endregion

        #region ثبت 
        [HttpPost]
        public ActionResult Create(CreateSettings model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //ذخیره لوگو پنل(صفحات داخلی)
            string LogoFileName = string.Format("{0}-{1}", DateTime.Now.Ticks, model.CouncilLogo.FileName);
            string LogoLocation = "~/Images/logo/" + LogoFileName;
            model.CouncilLogo.SaveAs(Server.MapPath(LogoLocation));

            //ذخیره لوگو صفحه لاگین
            string LoginFileName = string.Format("{0}-{1}", DateTime.Now.Ticks, model.CouncilLoginLogo.FileName);
            string LoginLogoLocation = "~/Images/logo/" + LoginFileName;
            model.CouncilLoginLogo.SaveAs(Server.MapPath(LoginLogoLocation));

            if (model.Used)
            {
                settingService.DeactiveAllSettings();

                //تنظیمات پیش فرض سیستم
                //HttpCookie cookie = HttpContext.Request.Cookies["cs"];
                //if (cookie != null)
                //{
                //    cookie.Expires = DateTime.Now.AddYears(-1);

                //    var setting = new Council.Core.Models.CurrentSetting
                //    {
                //        CouncilName = Server.UrlDecode(model.CouncilName),
                //        CouncilLogo = FileName
                //    };

                //    HttpCookie _cs = new HttpCookie("cs", JsonConvert.SerializeObject(setting));
                //    Response.Cookies.Add(_cs);
                //}
            }

            SystemSettings newSettigs = new SystemSettings
            {
                CouncilLogo = LogoFileName,
                CouncilLoginLogo = LoginFileName,
                CouncilName = model.CouncilName,
                Used = model.Used
            };
            settingService.Create(newSettigs);

            TempData["message"] = new ShowMessage
            {
                Message = "عملیات با موفقیت انجام شد",
                MessageType = ShowMessage.MessageTypes.success
            };
            return RedirectToAction("Index");
        }
        #endregion


        public ActionResult ActiveSettings(string id)
        {
            var model = settingService.Get<string>(id);

            if (model == null)
            {
                return HttpNotFound();
            }

            //تنظیمات پیش فرض سیستم
            HttpCookie cookie = HttpContext.Request.Cookies["cs"];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddYears(-1);

                var Settings = JsonConvert.DeserializeObject<CurrentSetting>(Request.Cookies["cs"].Value);

                var setting = new Council.Core.Models.CurrentSetting
                {
                    CouncilName = Server.UrlDecode(model.CouncilName),
                    CouncilLogo = model.CouncilLogo
                };

                HttpCookie _cs = new HttpCookie("cs", JsonConvert.SerializeObject(setting));
                Response.Cookies.Add(_cs);
            }

            settingService.DeactiveAllSettings();
            model.Used = true;

            settingService.Update(model);

            TempData["message"] = new ShowMessage
            {
                Message = "عملیات با موفقیت انجام شد",
                MessageType = ShowMessage.MessageTypes.success
            };
            return RedirectToAction("Index");
        }


        public ActionResult DeleteSettings(string id)
        {
            var model = settingService.Get<string>(id);

            if (model == null)
            {
                return HttpNotFound();
            }

            if (settingService.ISCurrentSetting(model.ID))
            {
                TempData["message"] = new ShowMessage
                {
                    Message = "قبل از حذف، لطفا تنظیمات پیش فرض را مشخص کنید",
                    MessageType = ShowMessage.MessageTypes.warning
                };
                return RedirectToAction("Index");
            }

            model.Deleted = true;
            settingService.Update(model);

            TempData["message"] = new ShowMessage
            {
                Message = "عملیات با موفقیت انجام شد",
                MessageType = ShowMessage.MessageTypes.success
            };
            return RedirectToAction("Index");
        }
    }
}