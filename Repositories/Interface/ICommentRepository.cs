using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAnime.Models.Entities;
using WebAnime.Models.ViewModel.Client;

namespace WebAnime.Repository.Interface
{
    public interface ICommentRepository : IRepositoryBase<Comments,int>
    {
        Task<IEnumerable<CommentShowViewModel>> GetPaging(int animeId, int pageNumber, int pageSize);
        Task<CommentShowViewModel> Comment(Comments comment);
    }
}
