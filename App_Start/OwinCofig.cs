using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Microsoft.AspNet.Identity.Owin;
using WebAnime.Models.Entities.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebAnime.App_Start
{
    public class OwinCofig
    {
        public static void AuthConfig(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Admin/Auth/Login"),
                Provider = new CookieAuthenticationProvider()
                {
                    OnValidateIdentity = SecurityStampValidator
                        .OnValidateIdentity<UserManager, Users, int>(
                            validateInterval: TimeSpan.FromMinutes(30),
                            regenerateIdentityCallback: (manager, user) =>
                                GenerateUserIdentityAsync(user, manager),
                            getUserIdCallback: (id) => (id.GetUserId<int>()))
                }
            });
        }
        public static async Task<ClaimsIdentity> GenerateUserIdentityAsync(Users user, UserManager manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
    }
}