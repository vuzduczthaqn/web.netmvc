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
    public class StatusRepository : IStatusRepository
    {
        public AnimeDbContext context { get; set; }
        public StatusRepository(AnimeDbContext context) {
            this.context = context;
        }
        public Task<bool> Create(Statuses entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(int id, int deletedBy = 0)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Statuses>> GetAll()
        {
            var statusViewList=await context.Statuses.Where(x=>!x.IsDeleted)
                .ToListAsync();
            return statusViewList;
        }

        public Task<Statuses> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(Statuses entity)
        {
            throw new NotImplementedException();
        }
    }
}