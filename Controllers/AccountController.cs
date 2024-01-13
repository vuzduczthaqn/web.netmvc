﻿using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebAnime.Components;
using WebAnime.Models.Entities.Identity;
using WebAnime.Models.Helpers;
using WebAnime.Models.ViewModel.Client;
using WebAnime.Services;

namespace WebAnime.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager _userManager;
        private readonly IAuthenticationManager _authenticationManager;
        private readonly SignInManager<Users, int> _signInManager;
        private readonly RoleManager _roleManager;
        private readonly IMapper _mapper;

        public AccountController(IAuthenticationManager authenticationManager, SignInManager<Users, int> signInManager, UserManager userManager, RoleManager roleManager, IMapper mapper)
        {
            _authenticationManager = authenticationManager;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public async Task<ActionResult> Index()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("User"))
            {
                return new HttpUnauthorizedResult("Already login, please go back");
            }
            return await Task.FromResult(View());
        }

        [UserAuthorize]
        [HttpGet]
        public async Task<ActionResult> Info()
        {
            var user = _userManager.FindById(User.Identity.GetUserId<int>());
            var userViewModel = _mapper.Map<UserViewModel>(user);
            return await Task.FromResult(View(userViewModel));
        }

        [UserAuthorize]
        [HttpPost]
        public async Task<ActionResult> Info(UserViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return await Task.FromResult(new HttpNotFoundResult("cannot found account"));

            var uploadImage = Request.Files["AvatarFile"];
            if (uploadImage != null && user.AvatarUrl.Equals(CommonConstants.DefaultAvatarUrl))
            {
                model.AvatarUrl = HandleFile(uploadImage);
            }
            if (model!=null)
            {
                user.FullName = model.FullName;
                user.AvatarUrl = model.AvatarUrl;
                user.BirthDay = model.BirthDay;
                user.PhoneNumber = model.PhoneNumber;
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    TempData[AlertConstants.SuccessMessage] = "Cập nhật thông tin thành công";
                    return View(model);
                }

                // Handle errors, for example, by adding ModelState errors
                var builder = new StringBuilder();
                foreach (var error in result.Errors)
                {
                    builder.AppendLine(error);
                }

                TempData[AlertConstants.ErrorMessage] = builder.ToString();
                return View(model);
            }

            TempData[AlertConstants.ErrorMessage] = "Lỗi không xác định, vui lòng thử lại";
            return await Task.FromResult(View(model));
        }

        [UserAuthorize]
        [HttpGet]
        public async Task<ActionResult> ChangePassword()
        {
            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId<int>());
            if (user == null)
            {
                TempData[AlertConstants.ErrorMessage] = "Yêu cầu đăng nhập";
                return RedirectToAction("Login");
            }

            var userViewModel = _mapper.Map<UserChangePasswordViewModel>(user);
            return View(userViewModel);
        }

        [UserAuthorize]
        [HttpPost]
        public async Task<ActionResult> ChangePassword(UserChangePasswordViewModel model)
        {
            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId<int>());
            if (user == null)
            {
                TempData[AlertConstants.ErrorMessage] = "Yêu cầu đăng nhập";
                return RedirectToAction("Login");
            }

            if (ModelState.IsValid)
            {
                var result = await _userManager.ChangePasswordAsync(user.Id, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    TempData[AlertConstants.SuccessMessage] = "Đổi mật thành công, vui lòng đăng nhập lại";
                    _authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    return RedirectToAction("Login");
                }

                var builder = new StringBuilder();
                foreach (var error in result.Errors)
                {
                    builder.AppendLine(error);
                }

                TempData[AlertConstants.ErrorMessage] = builder.ToString();
                return View(model);
            }
            TempData[AlertConstants.ErrorMessage] = "Lỗi không xác định, vui lòng thử lại";
            return await Task.FromResult(View(model));
        }

        [HttpPost]
        public async Task<ActionResult> Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                _authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return await Task.FromResult<ActionResult>(RedirectToAction("Login"));
            }
            return await Task.FromResult(RedirectToAction("NotFound", "Error"));
        }
        [HttpGet]
        public async Task<ActionResult> Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return await Task.FromResult(RedirectToAction("NotFound", "Error"));
            }
            return await Task.FromResult(View());

        }
        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            string returnUrl = Request.QueryString["returnUrl"] ?? string.Empty;

            if (ModelState.IsValid)
            {
                Users user = await _userManager.FindByNameAsync(model.UserName);

                if (user != null)
                {
                    if (user.AccessFailedCount == AuthConstants.MaxFailedAccessAttemptsBeforeLockout - 1)
                    {
                        await _userManager.SetLockoutEnabledAsync(user.Id, true);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty,
                        $@"Đăng nhập thất bại, vui lòng thử lại");
                    return View(model);

                }

                SignInStatus signInStatus =
                    await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, true);

                if (signInStatus == SignInStatus.Success)
                {

                    TempData[AlertConstants.SuccessMessage] = $"Chào mừng trở lại, {user.FullName}";
                    return RedirectToLocal(returnUrl);
                }
                else { 
                        ModelState.AddModelError(string.Empty,
                            $@"Đăng nhập thất bại, vui lòng thử lại ");
                        return View(model);
                }

            }
            ModelState.AddModelError(string.Empty, @"Đầu vào chưa hợp lệ");
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> UnconfirmedEmail(string email)
        {

            return await Task.FromResult(View(model: email));
        }
        [HttpPost]
        public async Task<ActionResult> UnconfirmedEmail(string email, string temp)
        {

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return new HttpNotFoundResult("Cannot find user by email " + email);

            string code = await _userManager.GenerateEmailConfirmationTokenAsync(user.Id);

            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code },
                protocol: Request.Url?.Scheme ?? "http");

            StringBuilder bodyBuilder = new StringBuilder();
            bodyBuilder.AppendLine(
                $"<p>Xin chào thành viên mới, bạn đã yêu cầu tạo tài khoản!</p>");
            bodyBuilder.AppendLine(
                $"<p>Để xác thực tài khoản mới, vui lòng bấm vào <a href=\"{callbackUrl}\"><strong>Đây</strong></a>");
            bodyBuilder.AppendLine("<p>Thư sẽ hết hạn sau 1 giờ.</p>");
            bodyBuilder.AppendLine("<h3>Cảm ơn bạn!</h3>");

            bool isSendEmail = await EmailService.SendMailAsync(new IdentityMessage()
            {
                Body = bodyBuilder.ToString(),
                Destination = user.Email,
                Subject = "Xác nhận tài khoản"
            });

            if (isSendEmail)
            {
                return RedirectToAction("VerifyEmailConfirmation", "Account");
            }

            var returnUrl = Request["returnUrl"];
            return RedirectToLocal(returnUrl);
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                _authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Login");
            }

            return View(new RegisterViewModel()
            {
                AvatarUrl = CommonConstants.DefaultAvatarUrl
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            var userRole = _roleManager.Roles.FirstOrDefault(x => x.Name.ToLower().Equals("user"));
            model.RoleListIds = new[] { userRole.Id };
            var uploadImage = Request.Files["AvatarFile"];
            model.AvatarUrl = HandleFile(uploadImage);
            if (ModelState.IsValid)
            {

                var user = _mapper.Map<Users>(model);

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user.Id, userRole.Name ?? "User");

                    string code = await _userManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    await _userManager.ConfirmEmailAsync(user.Id, code);
                    var returnUrl = Request["returnUrl"];
                    return RedirectToLocal(returnUrl);
                }
                AddErrors(result);
            }
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> VerifyEmailConfirmation()
        {
            return await Task.FromResult(View());
        }

        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(int userId, string code)
        {
            if (code == null)
            {
                return RedirectToAction("NotFound", "Error");
            }
            var result = await _userManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword(string email)
        {

            return View(new ForgotPasswordViewModel() { Email = email ?? string.Empty });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    //return View("ForgotPasswordConfirmation");
                    TempData[AlertConstants.ErrorMessage] = "Không thấy user";
                    ModelState.AddModelError("NotFoundUser", @"Không thấy user");
                    return View(model);
                }

                if (!(await _userManager.IsEmailConfirmedAsync(user.Id)))
                {
                    TempData[AlertConstants.ErrorMessage] = "Email chưa được xác nhận";

                    ModelState.AddModelError("NoConfirmEmail", @"Email chưa được xác nhận");
                    return View(model);
                }

                if (user.IsDeleted)
                {
                    TempData[AlertConstants.ErrorMessage] = "Tài khoản đã bị xóa khỏi hết thống";
                    ModelState.AddModelError("DeletedAccount",
                        @"Tài khoản đã bị xóa khỏi hệ thống, vui lòng liên hệ admin để cập nhật");
                    return View(model);
                }

                string resetCode = await _userManager.GeneratePasswordResetTokenAsync(user.Id);

                if (Request.Url != null)
                {
                    var callbackUrl = Url.Action("ResetPassword", "Account",
                        new { userId = user.Id, resetCode },
                        protocol: Request.Url.Scheme);


                    StringBuilder bodyBuilder = new StringBuilder();
                    bodyBuilder.AppendLine(
                        $"<p>Xin chào <strong>{user.FullName}</strong>, bạn đã yêu cầu khôi phục mật khẩu!</p>");
                    bodyBuilder.AppendLine(
                        $"<p>Để khôi phục mật khẩu của bạn, vui lòng bấm vào <a href=\"{callbackUrl}\">Đây</a>");
                    bodyBuilder.AppendLine("<p>Thư sẽ hết hạn sau 1 giờ.</p>");
                    bodyBuilder.AppendLine("<h3>Cảm ơn bạn!</h3>");

                    bool isSendEmail = await EmailService.SendMailAsync(new IdentityMessage()
                    {
                        Body = bodyBuilder.ToString(),
                        Destination = user.Email,
                        Subject = "Xác nhận khôi phục mật khẩu"
                    });

                    if (isSendEmail)
                    {
                        return RedirectToAction("ForgotPasswordConfirmation", "Account",
                            new { fromForgot = true });
                    }
                }
            }

            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> ForgotPasswordConfirmation(bool? fromForgot)
        {
            if (fromForgot.HasValue && fromForgot.Value)
                return await Task.FromResult(View());
            return RedirectToAction("NotFound", "Error");
        }

        [HttpGet]
        public async Task<ActionResult> ResetPassword(int userId, string resetCode)
        {
            if (resetCode == null)
                return new HttpNotFoundResult();

            return await Task.FromResult(View(new ResetPasswordViewModel()
            {
                UserId = userId,
                ResetCode = resetCode
            }));
        }

        [HttpPost]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!model.Password.Equals(model.ConfirmPassword))
                {
                    TempData[AlertConstants.ErrorMessage] = "Mật khẩu xác nhận không đúng, vui lòng thử lại";
                    ModelState.AddModelError("PasswordError", @"Mật khẩu xác nhận không đúng, vui lòng thử lại");
                    return View(model);
                }
                IdentityResult result = await _userManager.ResetPasswordAsync(model.UserId, model.ResetCode, model.Password);
                if (result.Succeeded)
                {
                    _authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    return RedirectToAction("Login", "Home");
                }

                foreach (var error in result.Errors)
                {

                    ModelState.AddModelError(error, error);
                }
                return View(model);
            }
            ModelState.AddModelError("TotalError", @"Có lỗi xảy ra, vui lòng thử lại");
            return View(model);
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (!await _signInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return RedirectToAction("NotFound", "Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, model.ReturnUrl, model.RememberMe });
        }


        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await _authenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await _signInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;

                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel()
                    {
                        UserName = loginInfo.Email,
                        FullName = loginInfo.DefaultUserName,
                        Email = loginInfo.Email,
                        AvatarUrl = CommonConstants.DefaultAvatarUrl,
                        Password = "1234567",//Fake
                        ReTypePassword = "1234567"
                    });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {

            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            var userRole = _roleManager.Roles.FirstOrDefault(x => x.Name.ToLower().Equals("user"))?.Id ?? 3;
            model.RoleListIds = new[] { userRole };

            if (ModelState.IsValid)
            {
                var info = await _authenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }

                var uploadImage = Request.Files["AvatarFile"];
                model.AvatarUrl = HandleFile(uploadImage);

                var user = _mapper.Map<Users>(model);
                var result = await _userManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            _authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
        }

        private string HandleFile(HttpPostedFileBase uploadImage)
        {


            if (uploadImage.ContentLength > 0)
            {
                var folderPath = Server.MapPath("~/Uploads/images/AvatarUsers");
                uploadImage.SaveAs(Path.Combine(folderPath, uploadImage.FileName));
                return ("\\" + Path.Combine("Uploads", "Images", "AvatarUsers", uploadImage.FileName)).Replace('\\', '/');
            }
            return CommonConstants.DefaultAvatarUrl;

        }

    }
}