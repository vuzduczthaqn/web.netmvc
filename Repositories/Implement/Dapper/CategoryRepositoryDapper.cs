﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using WebAnime.Models.Entities;
using WebAnime.Repository.Interface;

namespace WebAnime.Repositories.Implement.Dapper
{
    public class CategoryRepositoryDapper : ICategoryRepository
    {
        private readonly IDbConnection _connection;
        public CategoryRepositoryDapper(IDbConnection connection)
        {
            _connection = connection;
        }
        public async Task<IEnumerable<Categories>> GetAll()
        {
            return await _connection.QueryAsync<Categories>("Select* from dbo.Categories  where IsDeleted = 0 ");

        }

        public Task<Categories> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Create(Categories entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(Categories entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(int id, int deletedBy = default)
        {
            throw new NotImplementedException();
        }
    }
}
