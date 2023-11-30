using System.Threading.Tasks;
using WebAnime.Models.Entities;
using WebAnime.Models.ViewModel.Admin;
using BlogViewModel = WebAnime.Models.ViewModel.Client.BlogViewModel;

namespace WebAnime.Repository.Interface
{
    public interface IBlogRepository : IRepositoryBase<Blogs, int>
    {
        Task<BlogViewModel> GetBlogViewModel(int blogId);
        Task<Paging<Blogs>> GetPaping(string searchTitle, int pageSize, int pageNumber);
    }
}
