using Microsoft.Owin;
using Owin;
using System;
using System.Threading.Tasks;
using WebAnime.App_Start;
using WebAnime.Components;
using WebAnime.Models.Entities.Identity;
using WebAnime.Models.Helpers;
using static WebAnime.App_Start.OwinCofig;

[assembly: OwinStartup(typeof(WebAnime.Startup))]

namespace WebAnime
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            AuthConfig(app);
        //RegisterServices();
        //CreateDefaultRoles();
        //CreateDefaultUsers();
    }
    RoleManager _roleManager;
    UserManager _userManager;
    void RegisterServices()
    {
        _roleManager = NInjectConfig.GetService<RoleManager>();
        _userManager = NInjectConfig.GetService<UserManager>();
    }
    void CreateDefaultRoles()
    {
        _roleManager
            .CreateRoleIfNotExist("adMIN")
            .CreateRoleIfNotExist("MANAgeR")
            .CreateRoleIfNotExist("User");
    }

    void CreateDefaultUsers()
    {
        var adminUser = new Users()
        {
            UserName = "talonezio",
            BirthDay = new DateTime(2003, 7, 17),
            Email = "vuthemanh1707@gmail.com",
            AvatarUrl = CommonConstants.DefaultAvatarUrl,
            PhoneNumber = "0988344814",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true
        };
        var managerUser = new Users()
        {
            UserName = "vuthemanh1707",
            BirthDay = new DateTime(2003, 7, 17),
            Email = "vuthemanh333@gmail.com",
            AvatarUrl = CommonConstants.DefaultAvatarUrl,
            PhoneNumber = "0988344814",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true
        };

        var defaultPassword = "manhngu123";

        _userManager
            .CreateUserIfNotExist(adminUser, defaultPassword, "Admin")
            .CreateUserIfNotExist(managerUser, defaultPassword, "Manager");
    }
}
}
