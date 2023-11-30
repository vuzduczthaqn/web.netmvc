using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAnime.Models.Entities;

namespace WebAnime.Repository.Interface
{
    public interface IBlogCategoryRepository : IRepositoryBase<BlogCategories, int>
    {
        Task<IEnumerable<BlogCategories>> GetAllBlogCategoriesByBlogId(int blogId);
    }
}
