using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using WebAnime.Models.Entities;

namespace WebAnime.Repository.Interface
{
    public interface IRepositoryBase<T, in TKey> where T : class
    {
         Task<IEnumerable<T>> GetAll();
         Task<T> GetById(TKey id);
         Task<bool> Create(T entity);
         Task<bool> Update(T entity);
         Task<bool> Delete(TKey id, int deletedBy = default);

    }
}
