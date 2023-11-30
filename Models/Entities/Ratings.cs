using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebAnime.Models.Entities.Identity;

namespace WebAnime.Models.Entities
{
    public class Ratings
    {
        public int Id { get; set; }
        [Range(0, 10)]
        public double RatePoint { get; set; }
        [ForeignKey(nameof(User))] public int UserId { get; set; }
        public Users User { get; set; }
        [ForeignKey(nameof(Anime))] public int AnimeId { get; set; }
        public Animes Anime { get; set; }


    }
}
