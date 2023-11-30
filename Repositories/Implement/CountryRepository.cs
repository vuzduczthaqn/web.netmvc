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
    public class CountryRepository : ICountryRepository
    {

        public AnimeDbContext context { get; set; }
        public CountryRepository(AnimeDbContext context)
        {
            this.context = context;
        }
        public Task<bool> Create(Countries entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(int id, int deletedBy = 0)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Countries>> GetAll()
        {
            return await context.Countries.Where(x=>!x.IsDeleted).ToListAsync();
        }

        public Task<Countries> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(Countries entity)
        {
            throw new NotImplementedException();
        }
    }
}