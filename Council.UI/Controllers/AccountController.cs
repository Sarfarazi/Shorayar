using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Council.Service.DBServices;
using Council.Core.Entities;
using Council.Data.Contexts;
using System.Web.Services;
using Council.UI.Models;
using Council.UI.CustomAuthentication;
using Newtonsoft.Json;
using System.Web.Security;
using Council.Core.Models;
using System.Text;
using System.Web.Configuration;

namespace Council.UI.Controllers
{
    
    public class AccountController : Controller
    {
        UserServices userService = new UserServices();
        RoleService roleService = new RoleService();
        UserRoleService UrService = new UserRoleService();
        SystemSettingsService settingsService = new SystemSettingsService();

        //public AccountController()
        //    : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new AccountContext())))
        //{
        //}

        //public AccountController(UserManager<ApplicationUser> userManager)
        //{
        //    UserManager = userManager;
        //}

        //public UserManager<ApplicationUser> UserManager { get; private set; }

        #region ورود به سیستم
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = userService.CheckUserPass(model.UserName.Trim() , model.Password.Trim());

                if (user != null && user.IsActive)
                {
                    //user.IsOnline = true;
                    //userService.Update(user);

                    CustomSerializeModel userModel = new CustomSerializeModel()
                    {
                        UserId = user.ID,
                        UserName = user.UserName.Trim(),
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Roles = user.UserRole.Select(m => m.Role.RoleTitle).ToArray()
                    };

                    string userData = JsonConvert.SerializeObject(userModel);

                    FormsAuthentication.SetAuthCookie(model.UserName.Trim() , false);

                    FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket
                    (
                        1, model.UserName.Trim(), DateTime.Now, DateTime.Now, false, userData
                    );

                    string enTicket = FormsAuthentication.Encrypt(authTicket);
                    HttpCookie faCookie = new HttpCookie("_ash", enTicket);
                    Response.Cookies.Add(faCookie);


                    //تنظیمات پیش فرض سیستم
                    //HttpCookie cs = HttpContext.Request.Cookies["cs"];
                    //if (cs == null)
                    //{
                    //    var curentSetting = settingsService.FirstOrDefault(m => m.Used);
                    //    var setting = new Council.Core.Models.CurrentSetting
                    //    {
                    //        CouncilName = Server.UrlEncode(curentSetting.CouncilName),
                    //        CouncilLogo = curentSetting.CouncilLogo
                    //    };

                    //    HttpCookie _cs = new HttpCookie("cs", JsonConvert.SerializeObject(setting));
                    //    Response.Cookies.Add(_cs);
                    //}

                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        var roles = user.UserRole.Select(r => r.Role.RoleTitle).ToArray();

                        return RedirectToAction("Index", "Home");
                        //if (roles.Contains("Customer"))
                        //{
                        //    return RedirectToAction("Index", "Home", new { Area = "Customer" });
                        //}
                        //else if (roles.Contains("Admin"))
                        //{
                        //    return RedirectToAction("Index", "Home", new { Area = "Management" });
                        //}
                    }
                }
                else if (user == null)
                {
                    ModelState.AddModelError("", "نام کاربری یا رمز عبور اشتباه است");
                }
                else if (!user.IsActive)
                {
                    ModelState.AddModelError("", "کاربری غیر فعال است");
                }
            }

            return View(model);
        }
        #endregion

        public ActionResult RetNotAuthPage()
        {
            return View();
        }

        //public void ResetPasswords()
        //{
        //    var userIds = UserManager.Users.Select(u => u.Id).ToList();
        //    string _userId = "";
        //    foreach (var id in userIds)
        //    {
        //        _userId = id;
        //        string newPassword = "111111";
        //        UserManager.RemovePassword(_userId);
        //        UserManager.AddPassword(_userId, newPassword);
        //    }

        //}


        #region ثبت کاربری
        public ActionResult Register(string userID)
        {
            var user = userService.Get<string>(userID);
            ViewBag.UserName = user.FirstName + " " + user.LastName;
            ViewBag.UserID = userID;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model, string userID, string Manager)
        {
            if (ModelState.IsValid)
            {
                var myUser = userService.Get<string>(userID);

                myUser.UserName = model.UserName;
                myUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

                if (Manager == "on")
                {
                    myUser.IsManager = true;
                    userService.Save();

                    userService.SetUserRole(userID, "Manager");
                }
                else
                    userService.Save();

                TempData["message"] = new ShowMessage
                {
                    Message = "عملیات با موفقیت انجام شد",
                    MessageType = ShowMessage.MessageTypes.success
                };

                return RedirectToAction("Users");
            }

            return View(model);
        }
        #endregion

        #region ************ ساختار سیستم *****************
        #region مدیریت کاربران
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Users()
        {
            //ResetPasswords();
            var GuestUsersIds = new List<string>();
            var MamagerUsersIds = new List<string>();

            RegisterViewModel reg = new RegisterViewModel
            {
                UserType = roleService.GetRoles()
            };

            ViewBag.Users = userService.GetAllUsersInfo();
            return View(reg);
        }

        [HttpPost]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Users(RegisterViewModel user, HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                RegisterViewModel reg = new RegisterViewModel
                {
                    UserType = roleService.GetRoles()
                };
                ViewBag.Users = userService.GetAllUsersInfo();
                return View(user);
            }

            if (!userService.CheckAvailability(user.UserName))
            {
                List<string> str = new List<string>();

                foreach (var item in user.Types)
                {
                    var result = roleService.CanTakeRole(item);
                    if (result.ContainsKey(false))
                    {
                        str.Add(result[false]);
                    }
                }
                if (str.Count() == 0)
                {
                    var _user = new User
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        UserName = user.UserName,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password),
                        Email = user.Email,
                        Gender = user.Gender,
                        Mobile = user.Mobile,
                        NationalCode = user.NationalCode,
                        Tell = user.Tell,
                        IsActive = true
                    };

                    var InsertedUser = userService.CreateNewUser(_user, file);

                    if (InsertedUser != null)
                    {
                        string UserID = InsertedUser.ID;

                        foreach (var item in user.Types)
                        {
                            UrService.SetUserRole(UserID, item);
                        }

                        TempData["message"] = new ShowMessage
                        {
                            Message = "کاربر با موفقیت اضافه شد",
                            MessageType = ShowMessage.MessageTypes.success
                        };

                        return RedirectToAction("Users");
                    }

                    TempData["message"] = new ShowMessage
                    {
                        Message = "خطایی رخ داده است",
                        MessageType = ShowMessage.MessageTypes.error
                    };

                    return RedirectToAction("Users");
                }
                else
                {
                    ModelState.AddModelError("SelectedTypes", string.Format("تنها یک فرد می تواند نقش(های) \"{0}\" را داشته باشد", string.Join("، ", str.ToArray())));

                    user.UserType = roleService.GetRoles();
                    ViewBag.Users = userService.GetAllUsersInfo();
                    return View(user);
                }
            }
            else
            {
                ModelState.AddModelError("UserName", "نام کاربری وارد شده در سیستم موجود است");

                user.UserType = roleService.GetRoles();
                ViewBag.Users = userService.GetAllUsersInfo();
                return View(user);
            }
        }
        #endregion
        #endregion

        //#region کاربران آنلاین
        //[Authorize(Users = "admin")]
        //public ActionResult OnlineUsers()
        //{
        //    //ResetPasswords();
        //    ViewBag.Users = userService.GetAllUsers().Where(m => m.IsOnline).OrderBy(m => m.LastName).ToList();
        //    return View();
        //}

        ////[Authorize(Users = "admin")]
        ////public ActionResult ChangeOnlineStatus(string userID)
        ////{
        ////    //ResetPasswords();
        ////    bool result = userService.ChnageOnlineStatus(userID);
        ////    ViewBag.Users = userService.GetAllUsers().Where(m => m.IsOnline).OrderBy(m => m.LastName).ToList();
        ////    return RedirectToAction("OnlineUsers");
        ////}
        //#endregion


        //public string AsignRoleNameToUser(string roleName, string userId)
        //{
        //    try
        //    {
        //        UserManager.AddToRole(userId, roleName);
        //        return "ok";
        //    }
        //    catch (Exception ex)
        //    {
        //        return "errro";
        //    }
        //}


        #region ********** منوی عملیات ************
        #region فعال کردن کاربر
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult ActiveUser(string userID)
        {
            userService.ActiveUser(userID);

            TempData["message"] = new ShowMessage
            {
                Message = "عملیات با موفقیت انجام شد",
                MessageType = ShowMessage.MessageTypes.success
            };

            return RedirectToAction("Users");
        }
        #endregion

        #region غیر فعال کردن کاربر
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult DeActiveUser(string userID)
        {
            userService.DeActiveUser(userID);

            TempData["message"] = new ShowMessage
            {
                Message = "عملیات با موفقیت انجام شد",
                MessageType = ShowMessage.MessageTypes.success
            };

            return RedirectToAction("Users");
        }
        #endregion

        #region انتخاب به عنوان رئیس شورا
        public ActionResult ChangeCouncilBoss(string userID)
        {
            var oldBosses = userService.ChangeCouncilBoss(userID);
            foreach (var old in oldBosses)
            {
                userService.RemoveRole(old, "Boss");
            }
            userService.SetUserRole(userID, "Boss");

            TempData["message"] = new ShowMessage
            {
                Message = "عملیات با موفقیت انجام شد",
                MessageType = ShowMessage.MessageTypes.success
            };
            return RedirectToAction("Users");
        }
        #endregion

        #region انتخاب به عنوان مهمان
        public ActionResult AddGusetCouncilMember(string userID)
        {
            userService.AddGusetCouncilMember(userID);
            TempData["message"] = new ShowMessage
            {
                Message = "عملیات با موفقیت انجام شد",
                MessageType = ShowMessage.MessageTypes.success
            };
            return RedirectToAction("Users");
        }
        #endregion

        #region انتخاب به عنوان منشی جلسه یک
        public ActionResult ChangeCouncilWriter1(string userID)
        {
            var oldWriter = userService.ChangeCouncilWriter1(userID);
            foreach (var old in oldWriter)
            {
                userService.RemoveRole(old, "Writer1");
            }
            userService.SetUserRole(userID, "Writer1");
            TempData["message"] = new ShowMessage
            {
                Message = "عملیات با موفقیت انجام شد",
                MessageType = ShowMessage.MessageTypes.success
            };
            return RedirectToAction("Users");
        }
        #endregion

        #region انتخاب به عنوان منشی جلسه 2
        public ActionResult ChangeCouncilWriter2(string userID)
        {
            var newWriter = userService.Get<string>(userID);
            var oldWriter = userService.ChangeCouncilWriter2(userID);
            foreach (var old in oldWriter)
            {
                userService.RemoveRole(old, "Writer2");
            }
            userService.SetUserRole(userID, "Writer2");
            TempData["message"] = new ShowMessage
            {
                Message = "عملیات با موفقیت انجام شد",
                MessageType = ShowMessage.MessageTypes.success
            };
            return RedirectToAction("Users");
        }
        #endregion

        #region انتخاب به عنوان نائب رئیس شورا
        public ActionResult ChangeCouncilBossHelper(string userID)
        {
            var oldBossHelper = userService.ChangeCouncilBossHelper(userID);
            foreach (var old in oldBossHelper)
            {
                userService.RemoveRole(old, "BossHelper");
            }
            userService.SetUserRole(userID, "BossHelper");
            TempData["message"] = new ShowMessage
            {
                Message = "عملیات با موفقیت انجام شد",
                MessageType = ShowMessage.MessageTypes.success
            };
            return RedirectToAction("Users");
        }
        #endregion

        #region حذف عضو علی البدل
        public ActionResult RemoveOtherCouncilMember(string userID)
        {
            userService.RemoveOtherCouncilMember(userID);

            TempData["message"] = new ShowMessage
            {
                Message = "عملیات با موفقیت انجام شد",
                MessageType = ShowMessage.MessageTypes.success
            };
            return RedirectToAction("Users");
        }
        #endregion

        #region ویرایش کاربر
        [Authorize]
        public ActionResult EditUser(string id)
        {
            var user = userService.Get<string>(id);

            var _user = new EditUser
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Gender = user.Gender,
                Mobile = user.Mobile,
                NationalCode = user.NationalCode,
                Tell = user.Tell,
                ID = id,
                SelectedTypes = string.Join(",", user.UserRole.Select(m => m.RoleID).ToArray()),
                UserType = roleService.GetRoles()
            };
            return View(_user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult EditUser(EditUser model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                var user = userService.GetUserByID(model.ID);
                var OldTypes = user.UserRole.Select(m => m.RoleID).ToArray();

                //به روز رسانی اطلاعات کاربر
                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.Gender = model.Gender;
                user.ID = model.ID;
                user.LastName = model.LastName;
                user.Mobile = model.Mobile;
                user.Tell = model.Tell;
                user.NationalCode = model.NationalCode;

                userService.UpdateUser(user, file);

                var arraysAreEqual = Enumerable.SequenceEqual(OldTypes, model.Types);
                //اگر نقش ها تغییری داشتند
                if (!Enumerable.SequenceEqual(OldTypes, model.Types))
                {
                    //مشترک ها
                    var RoleCommon = OldTypes.Intersect(model.Types).ToArray();

                    //نقش های قبلی بدون مشترک ها
                    var OldRole = OldTypes.Except(RoleCommon).ToArray();

                    //نقش های جدید بدون مشترک ها
                    var NewRole = (model.Types).Except(RoleCommon).ToArray();

                    List<string> str = new List<string>();

                    if (NewRole.Count() > 0)
                    {
                        foreach (var item in NewRole)
                        {
                            var result = roleService.CanTakeRole(item);
                            if (result.ContainsKey(false))
                            {
                                str.Add(result[false]);
                            }
                        }

                        //نقش های جدید قابل درج هستند
                        if (str.Count() == 0)
                        {
                            //درج نقش های جدید
                            foreach (var item in NewRole)
                            {
                                UrService.SetUserRole(model.ID, item);
                            }

                            //حذف نقش های باقی مانده قدیم
                            if (OldRole.Count() > 0)
                            {
                                foreach (var item in OldRole)
                                {
                                    UrService.RemoveUserRole(model.ID, item);
                                }
                            }

                            TempData["message"] = new ShowMessage
                            {
                                Message = "به روز رسانی با موفقیت انجام شد",
                                MessageType = ShowMessage.MessageTypes.success
                            };
                            return RedirectToAction("Users");
                        }
                        else
                        {
                            ModelState.AddModelError("SelectedTypes", string.Format("تنها یک فرد می تواند نقش(های) \"{0}\" را داشته باشد", string.Join("، ", str.ToArray())));

                            model.UserType = roleService.GetRoles();
                            return View(model);
                        }
                    }
                    else
                    {
                        //به روز رسانی اطلاعات کاربر
                        //user.Email = model.Email;
                        //user.FirstName = model.FirstName;
                        //user.Gender = model.Gender;
                        //user.ID = model.ID;
                        //user.LastName = model.LastName;
                        //user.Mobile = model.Mobile;
                        //user.Tell = model.Tell;
                        //user.NationalCode = model.NationalCode;

                        //userService.UpdateUser(user, file);

                        //حذف نقش های باقی مانده قدیم
                        if (OldRole.Count() > 0)
                        {
                            foreach (var item in OldRole)
                            {
                                UrService.RemoveUserRole(model.ID, item);
                            }
                        }

                        TempData["message"] = new ShowMessage
                        {
                            Message = "به روز رسانی با موفقیت انجام شد",
                            MessageType = ShowMessage.MessageTypes.success
                        };
                        return RedirectToAction("Users");
                    }
                }
                else
                {
                    //به روز رسانی اطلاعات کاربر
                    //user.Email = model.Email;
                    //user.FirstName = model.FirstName;
                    //user.Gender = model.Gender;
                    //user.ID = model.ID;
                    //user.LastName = model.LastName;
                    //user.Mobile = model.Mobile;
                    //user.Tell = model.Tell;
                    //user.NationalCode = model.NationalCode;

                    //userService.UpdateUser(user, file);

                    TempData["message"] = new ShowMessage
                    {
                        Message = "به روز رسانی با موفقیت انجام شد",
                        MessageType = ShowMessage.MessageTypes.success
                    };
                    return RedirectToAction("Users");
                }
            }

            model.UserType = roleService.GetRoles();
            return View(model);
        }
        #endregion
        #endregion

        public ActionResult ChangeCouncilGuest(string userID)
        {
            var newGuest = userService.Get<string>(userID);
            var oldGuest = userService.ChangeCouncilGuest(userID);
            foreach (var old in oldGuest)
            {
                userService.RemoveRole(old, "Guest");
            }
            userService.SetUserRole(userID, "Guest");

            TempData["message"] = new ShowMessage
            {
                Message = "عملیات با موفقیت انجام شد",
                MessageType = ShowMessage.MessageTypes.success
            };
            return RedirectToAction("Users");
        }

        public ActionResult ChangeSiteManager(string userID)
        {
            userService.ChangeSiteManager(userID);
            return RedirectToAction("Users");
        }

        public ActionResult AddOtherCouncilMember(string userID)
        {
            userService.AddOtherCouncilMember(userID);
            return RedirectToAction("Users");
        }

        public ActionResult AddManagerCouncilMember(string userID)
        {
            userService.AddManagerCouncilMember(userID);
            return RedirectToAction("Users");
        }
        public ActionResult RemoveManagerCouncilMember(string userID)
        {
            userService.RemoveManagerCouncilMember(userID);
            return RedirectToAction("Users");
        }

        public ActionResult RemoveGusetCouncilMember(string userID)
        {
            userService.RemoveGusetCouncilMember(userID);
            return RedirectToAction("Users");
        }

        // POST: /Account/Disassociate
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        //{
        //    ManageMessageId? message = null;
        //    IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
        //    if (result.Succeeded)
        //    {
        //        message = ManageMessageId.RemoveLoginSuccess;
        //    }
        //    else
        //    {
        //        message = ManageMessageId.Error;
        //    }
        //    return RedirectToAction("Manage", new { Message = message });
        //}

        //
        // GET: /Account/Manage
        //public ActionResult Manage(ManageMessageId? message)
        //{
        //    ViewBag.StatusMessage =
        //        message == ManageMessageId.ChangePasswordSuccess ? "رمز عبور شما تغییر کرد."
        //        : message == ManageMessageId.SetPasswordSuccess ? "رمز عبور شما ست شد."
        //        : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
        //        : message == ManageMessageId.Error ? "بروز خطا."
        //        : "";
        //    ViewBag.HasLocalPassword = HasPassword();
        //    ViewBag.ReturnUrl = Url.Action("Manage");
        //    return View();
        //}

        //
        // POST: /Account/Manage
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Manage(ManageUserViewModel model)
        //{
        //    bool hasPassword = HasPassword();
        //    ViewBag.HasLocalPassword = hasPassword;
        //    ViewBag.ReturnUrl = Url.Action("Manage");
        //    if (hasPassword)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
        //            if (result.Succeeded)
        //            {
        //                return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
        //            }
        //            else
        //            {
        //                AddErrors(result);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        // User does not have a password so remove any validation errors caused by a missing OldPassword field
        //        ModelState state = ModelState["OldPassword"];
        //        if (state != null)
        //        {
        //            state.Errors.Clear();
        //        }

        //        if (ModelState.IsValid)
        //        {
        //            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
        //            if (result.Succeeded)
        //            {
        //                return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
        //            }
        //            else
        //            {
        //                AddErrors(result);
        //            }
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}


        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExternalLogin(string provider, string returnUrl)
        //{
        //    // Request a redirect to the external login provider
        //    return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        //}

        //
        // GET: /Account/ExternalLoginCallback
        //[AllowAnonymous]
        //public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        //{
        //    var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
        //    if (loginInfo == null)
        //    {
        //        return RedirectToAction("Login");
        //    }

        //    // Sign in the user with this external login provider if the user already has a login
        //    var user = await UserManager.FindAsync(loginInfo.Login);
        //    if (user != null)
        //    {
        //        await SignInAsync(user, isPersistent: false);
        //        return RedirectToLocal(returnUrl);
        //    }
        //    else
        //    {
        //        // If the user does not have an account, then prompt the user to create an account
        //        ViewBag.ReturnUrl = returnUrl;
        //        ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
        //        return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
        //    }
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult LinkLogin(string provider)
        //{
        //    // Request a redirect to the external login provider to link a login for the current user
        //    return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        //}

        //
        // GET: /Account/LinkLoginCallback
        //public async Task<ActionResult> LinkLoginCallback()
        //{
        //    var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
        //    if (loginInfo == null)
        //    {
        //        return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        //    }
        //    var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
        //    if (result.Succeeded)
        //    {
        //        return RedirectToAction("Manage");
        //    }
        //    return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        //}

        //
        // POST: /Account/ExternalLoginConfirmation
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Manage");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        // Get the information about the user from the external login provider
        //        var info = await AuthenticationManager.GetExternalLoginInfoAsync();
        //        if (info == null)
        //        {
        //            return View("ExternalLoginFailure");
        //        }
        //        var user = new ApplicationUser() { UserName = model.UserName };
        //        var result = await UserManager.CreateAsync(user);
        //        if (result.Succeeded)
        //        {
        //            result = await UserManager.AddLoginAsync(user.Id, info.Login);
        //            if (result.Succeeded)
        //            {
        //                await SignInAsync(user, isPersistent: false);
        //                return RedirectToLocal(returnUrl);
        //            }
        //        }
        //        AddErrors(result);
        //    }

        //    ViewBag.ReturnUrl = returnUrl;
        //    return View(model);
        //}

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();

            // clear authentication cookie
            HttpCookie cookie1 = new HttpCookie("_ash", "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);

            // clear session cookie (not necessary for your current problem but i would recommend you do it anyway)
            SessionStateSection sessionStateSection = (SessionStateSection)WebConfigurationManager.GetSection("system.web/sessionState");
            HttpCookie cookie2 = new HttpCookie(sessionStateSection.CookieName, "");
            cookie2.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie2);

            return RedirectToAction("Login", "Account");
        }

        //public JsonResult AjaxLogOff()
        //{
        //    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie, DefaultAuthenticationTypes.ExternalCookie);
        //    Global.AbandonSession();
        //    //var id = userService.GetUserByUserName(User.Identity.Name).ID;
        //    //var user = userService.Where(m => m.ID == id).FirstOrDefault();
        //    //if (user != null)
        //    //{
        //    //    user.IsOnline = false;
        //    //    userService.Update(user);
        //    //}
        //    Session.Abandon();
        //    return Json("ok");
        //}

        //
        // GET: /Account/ExternalLoginFailure
        //[AllowAnonymous]
        //public ActionResult ExternalLoginFailure()
        //{
        //    return View();
        //}

        //[ChildActionOnly]
        //public ActionResult RemoveAccountList()
        //{
        //    var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
        //    ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
        //    return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && UserManager != null)
        //    {
        //        UserManager.Dispose();
        //        UserManager = null;
        //    }
        //    base.Dispose(disposing);
        //}

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        //private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        //{
        //    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
        //    var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
        //    AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        //}

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        //private bool HasPassword()
        //{
        //    var user = UserManager.FindById(User.Identity.GetUserId());
        //    if (user != null)
        //    {
        //        return user.PasswordHash != null;
        //    }
        //    return false;
        //}

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }
        //public string GetRoleName(string id)
        //{
        //    List<string> roleNames = UserManager.GetRoles(id).ToList();
        //    for (int i = 0; i < roleNames.Count(); i++)
        //    {
        //        if (roleNames[i] == "Admin")
        //        {
        //            return "Admin";

        //        }
        //    }
        //    return "##";
        //}

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }


        }
        #endregion


        #region تغییر رمز عبور
        public ActionResult ChangePassword(string id)
        {
            ChangePasswordViewModel model = new ChangePasswordViewModel();
            if (ModelState.IsValid)
            {
                var user = userService.Get<string>(id);

                if (user == null)
                {
                    ViewBag.Message = "کاربر مورد نظر پیدا نشد";
                }
                else
                {
                    model.ID = user.ID;
                    model.UserName = user.UserName;
                    model.Password = "";
                }
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel usermodel)
        {
            if (ModelState.IsValid)
            {
                var user = userService.Get<string>(usermodel.ID);
                if (user == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(usermodel.Password);
                    var result = userService.Update(user);
                    if (result == null)
                    {
                        TempData["message"] = new ShowMessage
                        {
                            Message = "در ثبت تغییرات مشکلی وجود دارد",
                            MessageType = ShowMessage.MessageTypes.error
                        };
                        return View();
                    }

                    TempData["message"] = new ShowMessage
                    {
                        Message = "رمز عبور با موفقیت تغییر یافت",
                        MessageType = ShowMessage.MessageTypes.success
                    };
                    usermodel.UserName = user.UserName;
                    return View(usermodel);
                  //  return RedirectToAction("Index","Home");
                }
            }
            return View();
        }
        #endregion
    }
}