using DataModels.EF;
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
    public class AgeRatingRepository:IAgeRatingRepository
    {
        public AgeRatingRepository(AnimeDbContext context)
        {
            Context = context;
        }
        public AnimeDbContext Context { get; set; }
        public async Task<IEnumerable<AgeRatings>> GetAll()
        {
            return await Context.AgeRatings
                .Where(x => !x.IsDeleted).ToListAsync();
        }

        public Task<AgeRatings> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Create(AgeRatings entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(AgeRatings entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(int id, int deletedBy = default)
        {
            throw new NotImplementedException();
        }
    }
}