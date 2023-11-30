using WebAnime.Models.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;

namespace WebAnime.Models.Entities.Identity
{
    public class UserManager : UserManager<Users, int>
    {
        public UserManager(IUserStore<Users, int> store) : base(store)
        {
            ResigterAuth(this);
        }
        private static void ResigterAuth(UserManager userManager)
        {
            userManager.UserValidator = new UserValidator<Users, int>(userManager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true,

            };

            userManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,

            };

            var dataProtectionProvider = new DpapiDataProtectionProvider("WebAnime.DpapiDataProtectionProvider.TalonEzio.123!@#");

            userManager.UserTokenProvider =
                new DataProtectorTokenProvider<Users, int>(
                    dataProtectionProvider.Create("WebAnime.MVC.ResetTokenKey.Abc!@#)(&"));

            userManager.UserLockoutEnabledByDefault = true;
            userManager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(AuthConstants.LockoutMinutes);
            userManager.MaxFailedAccessAttemptsBeforeLockout = AuthConstants.MaxFailedAccessAttemptsBeforeLockout;
        }

    }
}
