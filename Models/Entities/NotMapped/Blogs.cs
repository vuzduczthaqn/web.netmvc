using System.ComponentModel.DataAnnotations.Schema;

namespace WebAnime.Models.Entities
{

    public partial class Blogs
    {
        [NotMapped] public int[] BlogCategoryIds { get; set; }

    }
}
