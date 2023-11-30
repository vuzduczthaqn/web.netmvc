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
    public class StudioRepository : IStudioRepository
    {
        public AnimeDbContext context { get; set; }
        public StudioRepository(AnimeDbContext context)
        {
            this.context = context;
        }
        public Task<bool> Create(Studios entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(int id, int deletedBy = 0)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Studios>> GetAll()
        {
            return await context.Studios.Where(x=>!x.IsDeleted).ToListAsync();
        }

        public Task<Studios> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(Studios entity)
        {
            throw new NotImplementedException();
        }
    }
}