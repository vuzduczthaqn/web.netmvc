using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAnime.Models.Entities;
using WebAnime.Models.ViewModel.Admin;
using WebAnime.Models.ViewModel.Client;

namespace WebAnime.Repository.Interface
{
    public interface IAnimeRepository : IRepositoryBase<Animes, int>
    {
        Task<IEnumerable<AnimeItemViewModel>> GetAnimeTrending(int take=9);
        Task<IEnumerable<AnimeItemViewModel>> GetAnimeRecenly(int take=9);
        Task<AnimeDetailViewModel> GetAnimeDetail(int id);

        Task<AnimeWatchingViewModel> GetAnimeWatching(int id);

       Task<bool> IncreaseView(int id);

       Task<Paging<Animes>> GetPaging(string searchTitle, int pageNumber, int pageSize);
       Task<Paging<AnimeItemViewModel>> AdvanceSearch(AnimeSearchViewModel model);
        Task<Paging<AnimeItemViewModel>> GetPageAnimeTrending(int start);
        Task<Paging<AnimeItemViewModel>> GetPageAnimeRecenly(int start);
        Task<int> GetCountTotalAnime();
    }
}
