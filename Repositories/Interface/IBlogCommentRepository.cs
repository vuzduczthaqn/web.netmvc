using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAnime.Models.Entities;

namespace WebAnime.Repository.Interface
{
    public interface IBlogCommentRepository : IRepositoryBase<BlogComments,int>
    {
         Task<IEnumerable<BlogComments>> GetByBlogId(int blogId);
        Task<int> Comment(BlogComments comment);
    }
}
