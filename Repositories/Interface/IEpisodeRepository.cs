using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAnime.Models.Entities;

namespace WebAnime.Repository.Interface
{
    public interface IEpisodeRepository : IRepositoryBase<Episodes, int>
    {
        Task<IEnumerable<Episodes>> GetAll(int animeId, int serverId);
        Task<int> GetMaxOrderId(int animeId, int serverId);
        Task<bool> AddRange(List<Episodes> episodes);


    }
}
