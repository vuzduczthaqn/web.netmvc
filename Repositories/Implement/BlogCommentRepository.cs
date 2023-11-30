using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebAnime.Models.Entities;
using WebAnime.Repository.Interface;

namespace WebAnime.Repositories.Implement
{
    public class BlogCommentRepository : IBlogCommentRepository
    {
        public Task<int> Comment(BlogComments comment)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Create(BlogComments entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(int id, int deletedBy = 0)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<BlogComments>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<BlogComments>> GetByBlogId(int blogId)
        {
            throw new NotImplementedException();
        }

        public Task<BlogComments> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(BlogComments entity)
        {
            throw new NotImplementedException();
        }
    }
}