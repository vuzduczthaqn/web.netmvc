using Microsoft.AspNet.Identity;

namespace WebAnime.Models.Entities.Identity
{
    public class RoleManager : RoleManager<Roles, int>
    {
        public RoleManager(IRoleStore<Roles, int> store) : base(store)
        {
        }
    }
}
