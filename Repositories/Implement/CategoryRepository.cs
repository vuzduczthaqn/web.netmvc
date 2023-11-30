using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebAnime.Models;
using WebAnime.Models.Entities;
using WebAnime.Repository.Interface;

namespace WebAnime.Repositories.Implement
{
    public class CategoryRepository : ICategoryRepository
    {
        public AnimeDbContext context { get; set; }
        public CategoryRepository(AnimeDbContext context)
        {
            this.context = context;
        }
        public Task<bool> Create(Categories entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(int id, int deletedBy = 0)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Categories>> GetAll()
        {
            return await Task.FromResult(context.Categories.Where(x => !x.IsDeleted));
        }

        public Task<Categories> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(Categories entity)
        {
            throw new NotImplementedException();
        }
    }
}