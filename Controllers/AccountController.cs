using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebAnime.Components;
using WebAnime.Models.Entities.Identity;
using WebAnime.Models.Helpers;
using WebAnime.Models.ViewModel.Client;

namespace WebAnime.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
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
                    int loginFailCount = (int)(Session[CommonConstants.LoginFailCount] ?? 0);

                    if (loginFailCount == AuthConstants.MaxFailedAccessAttemptsBeforeLockout - 1)
                    {
                        ModelState.AddModelError("Hint", @"Chưa có tài khoản? Hãy tạo tài khoản mới");
                        Session.Remove(CommonConstants.LoginFailCount);

                        return View(model);
                    }
                    ModelState.AddModelError(string.Empty,
                        $@"Đăng nhập thất bại, vui lòng thử lại (còn {AuthConstants.MaxFailedAccessAttemptsBeforeLockout - 1 - loginFailCount} lượt)");

                    loginFailCount++;
                    Session[CommonConstants.LoginFailCount] = loginFailCount;
                    return View(model);

                }

                SignInStatus signInStatus =
                    await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, true);

                switch (signInStatus)
                {
                    case SignInStatus.Success:
                        Session.Remove(CommonConstants.LoginFailCount);
                        await _userManager.SetLockoutEnabledAsync(user.Id, false);
                        await _userManager.ResetAccessFailedCountAsync(user.Id);

                        if (!await _userManager.IsEmailConfirmedAsync(user.Id))
                        {
                            _authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                            return RedirectToAction("UnconfirmedEmail", new { email = user.Email });
                        }

                        return RedirectToLocal(returnUrl);

                    case SignInStatus.LockedOut:
                        ModelState.AddModelError("LockoutMessage",
                            $@"Tài khoản của bạn đã bị khóa do đăng nhập sai quá {AuthConstants.MaxFailedAccessAttemptsBeforeLockout} lần hoặc bị admin khóa.");
                        ModelState.AddModelError("LogoutHint", $@"Vui lòng thử lại sau {AuthConstants.LockoutMinutes} phút hoặc liên hệ admin.");
                        return View(model);

                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, model.RememberMe });

                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError(string.Empty,
                            $@"Đăng nhập thất bại, vui lòng thử lại (còn {AuthConstants.MaxFailedAccessAttemptsBeforeLockout - 1 - user.AccessFailedCount} lượt)");
                        return View(model);
                }

            }
            ModelState.AddModelError(string.Empty, @"Đầu vào chưa hợp lệ");
            return View(model);
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
        [ValidateAntiForgeryToken]
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
                    var returnUrl = Request["returnUrl"];
                    return RedirectToLocal(returnUrl);
                }
                AddErrors(result);
            }
            return View(model);
        }
        private string HandleFile(HttpPostedFileBase uploadImage)
        {
                var folderPath = Server.MapPath("~/Uploads/images/AvatarUsers");
                uploadImage.SaveAs(Path.Combine(folderPath, uploadImage.FileName));
                return ("\\" + Path.Combine("Uploads", "Images", "AvatarUsers", uploadImage.FileName)).Replace('\\', '/');
  
            return CommonConstants.DefaultAvatarUrl;
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

    }
}