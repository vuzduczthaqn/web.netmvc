using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebAnime.Models;
using WebAnime.Models.Entities;
using WebAnime.Repository.Interface;

namespace WebAnime.Repositories.Implement
{
    public class TypeRepository : ITypeRepository
    {
        public AnimeDbContext context { get; set; }
        public TypeRepository(AnimeDbContext context)
        {
            this.context = context;
        }
        public Task<bool> Create(Types entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(int id, int deletedBy = 0)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Types>> GetAll()
        {
            return await context.Types.Where(x=>!x.IsDeleted).ToListAsync();
        }

        public Task<Types> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(Types entity)
        {
            throw new NotImplementedException();
        }
    }
}