using WebAnime.Models.Entities;

namespace WebAnime.Models.Helpers

{
    public abstract class BaseDto
    {
        protected readonly AnimeDbContext Context = new AnimeDbContext();

    }
}
