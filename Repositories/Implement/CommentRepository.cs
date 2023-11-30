using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using WebAnime.Models;
using WebAnime.Models.Entities;
using WebAnime.Models.ViewModel.Client;
using WebAnime.Repository.Interface;
using WebAnime.Models.Entities.Identity;

namespace WebAnime.Repositories.Implement
{
    public class CommentRepository : ICommentRepository
    {
        public AnimeDbContext Context { get; set; }
        public CommentRepository(AnimeDbContext context)
        {
            Context = context;
        }
        public Task<IEnumerable<Comments>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<Comments> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Create(Comments entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(Comments entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(int id, int deletedBy = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<CommentShowViewModel>> GetPaging(int animeId, int pageNumber, int pageSize)
        {
            //can lay comment cua anime gi do, trang thu x, trang day co y comment
            //var anime = await 
            //    Context.Animes
            //    .Include(animes => animes.Comments.Select(comments => comments.User))
            //    .FirstOrDefaultAsync(x => x.Id == animeId && !x.IsDeleted);

            //if (anime == null) return null;

            //var result = anime.Comments
            //    .Where(x => !x.IsDeleted)
            //    .OrderByDescending(x => x.CreatedDate)
            //    .Select(x => new CommentShowViewModel()
            //    {
            //        AnimeId = animeId,
            //        AvatarUrl = x.User.AvatarUrl ?? "",
            //        Content = x.Content,
            //        CreatedBy = x.User.Id,
            //        CreatedDate = x.CreatedDate,
            //    })
            //    .Skip((pageNumber - 1) * pageSize)
            //    .Take(pageSize);
            //return result;


            var commentViewModels =
                Context.Comments
                .Where(x => !x.IsDeleted && x.AnimeId == animeId)
                .Join(Context.Users, c => c.CreatedBy, u => u.Id, (c, u) =>
                    new CommentShowViewModel()
                    {
                        AnimeId = animeId,
                        Content = c.Content,
                        CreatedBy = c.CreatedBy,
                        CreatedDate = c.CreatedDate ?? DateTime.Now,
                        AvatarUrl = u.AvatarUrl,
                        UserFullName = u.FullName,
                        Id = c.Id,
                        EpisodeTitle = Context.Episodes.FirstOrDefault(x => !x.IsDeleted && c.EpisodeId == x.Id).Title ?? "",
                    }
                )
                .OrderByDescending(x => x.CreatedDate)//Order before skip and take
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize);

            return await Task.FromResult(commentViewModels);
        }
        public async Task<CommentShowViewModel> Comment(Comments comment)
        {
            try
            {
                comment.CreatedDate = DateTime.Now;
                Context.Comments.Add(comment);

                await Context.SaveChangesAsync();
                
                var user = Context.Users.FirstOrDefault(x => !x.IsDeleted && x.Id == comment.CreatedBy);

                return new CommentShowViewModel()
                {
                    AnimeId = comment.AnimeId,
                    Content = comment.Content,
                    CreatedBy = comment.CreatedBy,
                    AvatarUrl = user?.AvatarUrl,
                    UserFullName = user?.FullName,
                    CreatedDate = comment.CreatedDate ?? DateTime.Now,
                    EpisodeTitle = Context.Episodes.FirstOrDefault(x => !x.IsDeleted && x.Id == comment.EpisodeId)?.Title ?? ""
                };
            }
            catch 
            {
                return null;
            }
        }
    }
}
