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
using JobSearchingSystem.Models;
using JobSearchingSystem.DAL;
using System.Net.Mail;

namespace JobSearchingSystem.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private AccountUnitOfWork accountUnitOfWork = new AccountUnitOfWork();

        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())),
                   new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext())))
        {
        }

        public AccountController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }
        public RoleManager<IdentityRole> RoleManager { get; private set; }

        #region Common
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login([Bind(Include = "UserName, Password, RememberMe")]LoginViewModel model, string returnUrl)
        {
            var user = await UserManager.FindAsync(model.UserName, model.Password);
            if (user != null)
            {
                await SignInAsync(user, model.RememberMe);
            }
            else
            {
                TempData["message"] = "Username hoặc Mật khẩu không đúng";
            }

            if (!String.IsNullOrEmpty(returnUrl))
            {
                return Redirect("../" + returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Register([Bind(Include = "UserName, Password, ConfirmPassword, Email, FullName, PhoneNumber, RoleName")]RegisterViewModel model)
        {
            UnitOfWork unitOfWork = new UnitOfWork();
            AspNetUser u = unitOfWork.AspNetUserRepository.Get(s => s.UserName == model.UserName).FirstOrDefault();
            if (u != null)
            {
                TempData["message"] = "Username đã tồn tại, xin hãy dùng Username khác";
                return RedirectToAction("Index", "Home");
            }

            var user = new ApplicationUser() { UserName = model.UserName };
            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var createdUser = await UserManager.FindAsync(model.UserName, model.Password);
                var roleResult = await UserManager.AddToRoleAsync(createdUser.Id, model.RoleName);
                if (model.RoleName == "Recruiter")
                {
                    Recruiter recruiter = new Recruiter();
                    recruiter.RecruiterID = createdUser.Id;
                    recruiter.Email = model.Email;
                    recruiter.IsDeleted = false;
                    unitOfWork.RecruiterRepository.Insert(recruiter);
                    unitOfWork.Save();

                    await SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Update", "CompanyInfo");
                }
                else
                {
                    Jobseeker jobseeker = new Jobseeker();
                    jobseeker.JobSeekerID = createdUser.Id;
                    jobseeker.Email = model.Email;
                    jobseeker.FullName = model.FullName;
                    jobseeker.PhoneNumber = model.PhoneNumber;
                    jobseeker.IsDeleted = false;
                    unitOfWork.JobseekerRepository.Insert(jobseeker);
                    unitOfWork.Save();

                    await SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["message"] = "Đăng ký thất bại";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<ActionResult> ChangePassword([Bind(Include = "OldPassword, NewPassword, ConfirmPassword")]ChangePasswordViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            if (hasPassword)
            {
                IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    TempData["message"] = "Đổi mật khẩu mới thành công";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["message"] = "Mật khẩu cũ không đúng";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    TempData["message"] = "Đổi mật khẩu mới thành công";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["message"] = "Quá trình đổi mật khẩu mới gặp lỗi";
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(string emailAdress)
        {
            if (!String.IsNullOrEmpty(emailAdress))
            {
                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var random = new Random();
                var newPassword = new string(Enumerable.Repeat(chars, 8)
                                             .Select(s => s[random.Next(s.Length)])
                                             .ToArray());
                string userId = "";

                try
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");

                    UnitOfWork unitOfWork = new UnitOfWork();
                    mail.From = new MailAddress("jss.noreply.email@gmail.com");
                    Jobseeker jobseeker = unitOfWork.JobseekerRepository.Get(s => s.Email == emailAdress && s.IsDeleted == false).FirstOrDefault();
                    Recruiter recruiter = unitOfWork.RecruiterRepository.Get(s => s.Email == emailAdress && s.IsDeleted == false).FirstOrDefault();
                    if (jobseeker != null){
                        mail.To.Add(jobseeker.Email);
                        userId = jobseeker.JobSeekerID;
                    }
                    else if (recruiter != null)
                    {
                        mail.To.Add(recruiter.Email);
                        userId = recruiter.RecruiterID;
                    }
                    else
                    {
                        TempData["message"] = "Không tìm thấy Id tài khoản";
                        return RedirectToAction("Index", "Home");
                    }
                    mail.Subject = "[JSS] Thông báo mật khẩu mới";
                    mail.Body = "Mật khẩu của bạn đã được tạo mới là: " + newPassword;

                    smtpClient.Port = 587;
                    smtpClient.Credentials = new System.Net.NetworkCredential("jss.noreply.email@gmail.com", "Kogarashi789");
                    smtpClient.EnableSsl = true;

                    smtpClient.Send(mail);
                }
                catch (Exception)
                {
                    TempData["message"] = "Quá trình gửi mật khẩu mới gặp lỗi";
                    return RedirectToAction("Index", "Home");
                }

                await UserManager.RemovePasswordAsync(userId);
                IdentityResult result = await UserManager.AddPasswordAsync(userId, newPassword);
                if (result.Succeeded)
                {
                    TempData["message"] = "Bạn hãy vào mail để xem mật khẩu mới";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["message"] = "Quá trình tạo mật khẩu mới gặp lỗi";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["message"] = "Email trống";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region Account Manage
        [Authorize(Roles = "Admin")]
        public ActionResult List(string showoption)
        {
            AccListViewModel model = new AccListViewModel();
            model.showoption = showoption;

            if (String.IsNullOrEmpty(showoption))
            {
                model.userList = (from u in accountUnitOfWork.AspNetUserRepository.Get()
                                  select new AccUserItem
                                  {
                                      UserName = u.UserName,
                                      RoleName = UserManager.GetRoles(u.Id).AsEnumerable().Count() != 0 ? UserManager.GetRoles(u.Id).AsEnumerable().ElementAt(0) : "",
                                      IsActive = u.IsActive
                                  }).AsEnumerable();
            }
            else if ("0".Equals(showoption))
            {
                model.userList = (from u in accountUnitOfWork.AspNetUserRepository.Get()
                                  select new AccUserItem
                                  {
                                      UserName = u.UserName,
                                      RoleName = UserManager.GetRoles(u.Id).AsEnumerable().Count() != 0 ? UserManager.GetRoles(u.Id).AsEnumerable().ElementAt(0) : "",
                                      IsActive = u.IsActive
                                  }).Where(s => s.RoleName == "Staff" || s.RoleName == "Manager").AsEnumerable();
            }
            else
            {
                model.userList = (from u in accountUnitOfWork.AspNetUserRepository.Get()
                                  select new AccUserItem
                                  {
                                      UserName = u.UserName,
                                      RoleName = UserManager.GetRoles(u.Id).AsEnumerable().Count() != 0 ? UserManager.GetRoles(u.Id).AsEnumerable().ElementAt(0) : "",
                                      IsActive = u.IsActive
                                  }).Where(s => s.RoleName == "Jobseeker" || s.RoleName == "Recruiter").AsEnumerable();
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateAccount([Bind(Include = "UserName, Password, ConfirmPassword, RoleName")]RegisterViewModel model, string showoption)
        {
            if (!"Staff".Equals(model.RoleName) && !"Manager".Equals(model.RoleName))
            {
                return RedirectToAction("List", "Account", new { showoption = showoption });
            }

            UnitOfWork unitOfWork = new UnitOfWork();
            AspNetUser u = unitOfWork.AspNetUserRepository.Get(s => s.UserName == model.UserName).FirstOrDefault();
            if (u != null)
            {
                return RedirectToAction("List", "Account", new { showoption = showoption });
            }

            var user = new ApplicationUser() { UserName = model.UserName };
            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var createdUser = await UserManager.FindAsync(model.UserName, model.Password);
                var roleResult = await UserManager.AddToRoleAsync(createdUser.Id, model.RoleName);
                if (model.RoleName == "Staff")
                {
                    Staff staff = new Staff();
                    staff.StaffID = createdUser.Id;
                    staff.IsDeleted = false;
                    unitOfWork.StaffRepository.Insert(staff);
                    unitOfWork.Save();
                }
                else
                {
                    Manager manager = new Manager();
                    manager.ManagerID = createdUser.Id;
                    manager.IsDeleted = false;
                    unitOfWork.ManagerRepository.Insert(manager);
                    unitOfWork.Save();
                }

                return RedirectToAction("List", "Account", new { showoption = showoption });
            }
            else
            {
                AddErrors(result);
            }

            return RedirectToAction("List", "Account", new { showoption = showoption });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult DeactiveAccount(string UserName, string showoption)
        {
            UnitOfWork unitOfWork = new UnitOfWork();
            AspNetUser user = unitOfWork.AspNetUserRepository.Get(s => s.UserName == UserName).FirstOrDefault();

            if (user != null)
            {
                user.IsActive = false;
                unitOfWork.AspNetUserRepository.Update(user);
                unitOfWork.Save();
            }

            return RedirectToAction("List", "Account", new { showoption = showoption });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult ActiveAccount(string UserName, string showoption)
        {
            UnitOfWork unitOfWork = new UnitOfWork();
            AspNetUser user = unitOfWork.AspNetUserRepository.Get(s => s.UserName == UserName).FirstOrDefault();

            if (user != null)
            {
                user.IsActive = true;
                unitOfWork.AspNetUserRepository.Update(user);
                unitOfWork.Save();
            }

            return RedirectToAction("List", "Account", new { showoption = showoption });
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

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
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
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
    }
}