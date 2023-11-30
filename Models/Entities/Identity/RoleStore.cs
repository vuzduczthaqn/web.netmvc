using Microsoft.AspNet.Identity.EntityFramework;

namespace WebAnime.Models.Entities.Identity
{
    public class RoleStore : RoleStore<Roles, int, UserRoles>
    {
        public RoleStore(AnimeDbContext context) : base(context)
        {
        }
    }
}
