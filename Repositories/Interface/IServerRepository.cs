using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAnime.Models.Entities;

namespace WebAnime.Repository.Interface
{
    public interface IServerRepository : IRepositoryBase<Servers, int>
    {
        Task<Servers> GetFirst();
    }
}
