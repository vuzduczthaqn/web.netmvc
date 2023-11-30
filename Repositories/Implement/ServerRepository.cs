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
    public class ServerRepository : IServerRepository
    {
        public AnimeDbContext context { get; set; }
        public ServerRepository(AnimeDbContext context)
        {
            this.context = context;
        }
        public Task<bool> Create(Servers entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(int id, int deletedBy = 0)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Servers>> GetAll()
        {
            return await context.Servers.Where(x=>x.IsDeleted==false).ToListAsync();
        }

        public async Task<Servers> GetById(int id)
        {
            return await context.Servers.FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id);
        }

        public async Task<Servers> GetFirst()
        {
            return await context.Servers.FirstOrDefaultAsync();
        }

        public Task<bool> Update(Servers entity)
        {
            throw new NotImplementedException();
        }
    }
}