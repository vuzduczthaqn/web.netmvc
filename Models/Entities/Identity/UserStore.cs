using Microsoft.AspNet.Identity.EntityFramework;
namespace WebAnime.Models.Entities.Identity
{

    public class UserStore : UserStore<Users, Roles, int, UserLogins, UserRoles, UserClaims>
    {
        public UserStore(AnimeDbContext context) : base(context)
        {
        }
    }
}
