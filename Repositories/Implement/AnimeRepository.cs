﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebAnime.Models;
using WebAnime.Models.Entities;
using WebAnime.Models.ViewModel.Admin;
using WebAnime.Models.ViewModel.Client;
using WebAnime.Repository.Interface;

namespace WebAnime.Repositories.Implement
{

    public class AnimeRepository : IAnimeRepository
    {
        public AnimeDbContext context { get; set; }
        public AnimeRepository(AnimeDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> Create(Animes entity)
        {
            try
            {
                foreach (var categoryId in entity.CategoriesId)
                {
                    var category = await context.Categories
                        .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == categoryId);

                    if (category != null) entity.Categories.Add(category);
                }

                foreach (var studioId in entity.StudiosId)
                {
                    var studio = await context.Studios
                        .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == studioId);

                    if (studio != null) entity.Studios.Add(studio);
                }

                entity.CreatedDate = entity.ModifiedDate = DateTime.Now;
                entity.IsDeleted = false;

                context.Animes.Add(entity);
                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Delete(int id, int deletedBy)
        {
            try
            {
                var deleteEntity = await context.Animes.FindAsync(id);
                if (deleteEntity == null) return false;

                deleteEntity.IsDeleted = true;
                deleteEntity.DeletedDate = DateTime.Now;
                deleteEntity.DeletedBy = deletedBy;

                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<Animes>> GetAll()
        {
            return await Task.FromResult(context.Animes.Where(x => !x.IsDeleted));
        }

        public async Task<AnimeDetailViewModel> GetAnimeDetail(int id)
        {
            var data = await context.Animes
                .Include(animes => animes.Types)
                .Include(animes => animes.Studios)
                .Include(animes => animes.Categories)
                .Include(animes => animes.Statuses)
                .Include(animes => animes.Ratings)
                .Include(animes => animes.Comments)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (data == null) return null;

            var ratingCount = data.Ratings.Count();

            var result = new AnimeDetailViewModel()
            {
                Id = data.Id,
                Title = data.Title,
                OriginalTitle = data.OriginalTitle,
                Synopsis = data.Synopsis,
                Poster = data.Poster,
                Duration = data.Duration,
                Release = data.Release,
                Type = data.Types.Name,
                Studios = data.Studios.Where(x => !x.IsDeleted).Select(x => x.Name).ToArray(),
                Categories = data.Categories.Where(x => !x.IsDeleted).Select(x => x.Name).ToArray(),
                Status = data.Statuses.Name,
                Score = ratingCount > 0 ? Math.Round(data.Ratings.Sum(x => x.RatePoint) / (ratingCount * 1.0), 2) : 0,
                RateCount = ratingCount,
                CommentCount = data.Comments.Count(),
                ViewCount = data.ViewCount,
            };

            return result;
        }

        public async Task<IEnumerable<AnimeItemViewModel>> GetAnimeRecenly(int take)
        {
            var data = context.Animes
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedDate)
                .ThenByDescending(x => x.ModifiedDate)
                .ThenByDescending(x => x.ViewCount)
                .Select(x => new AnimeItemViewModel()
                {
                    Id = x.Id,
                    Title = x.Title,
                    Poster = x.Poster,
                    ViewCount = x.ViewCount,
                    Type = x.Types.Name,
                    Status = x.Statuses.Name,
                    CurrentEpisode = x.Episodes
                        .Where(z => !z.IsDeleted)
                        .GroupBy(z => z.ServerId)
                        .Max(t => t.Count()),
                    TotalEpisode = x.TotalEpisodes,
                    CommentCount = context.Comments.Count(y => y.AnimeId == x.Id)
                }).Take(take);
            return await Task.FromResult(data);
        }

        public async Task<IEnumerable<AnimeItemViewModel>> GetAnimeTrending(int take = 9)
        {
            var listAnimeTrending = context.Animes.Where(anime => !anime.IsDeleted)
                .OrderByDescending(x => x.ViewCount)
                .ThenByDescending(x => x.CreatedDate)
                .ThenByDescending(x => x.ModifiedDate)
                .Select(x => new AnimeItemViewModel()
                {
                    Id = x.Id,
                    Title = x.Title,
                    Poster = x.Poster,
                    ViewCount = x.ViewCount,
                    Type = x.Types.Name,
                    Status = x.Statuses.Name,
                    CurrentEpisode = x.Episodes
                        .Where(episodes => !episodes.IsDeleted)
                        .GroupBy(z => z.ServerId)
                        .Max(t => t.Count()),
                    TotalEpisode = x.TotalEpisodes,
                    CommentCount = context.Comments.Count(y => y.AnimeId == x.Id)
                }).Take(take);
            return await Task.FromResult(listAnimeTrending);
        }

        public async Task<Animes> GetById(int id)
        {
            return await context.Animes
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public async Task<bool> Update(Animes entity)
        {
            try
            {
                var updateEntity = await context.Animes.FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == entity.Id);
                if (updateEntity == null) return false;
                //updateEntity.CategoriesId ??= new int[] { };
                //updateEntity.StudiosId ??= new int[] { };

                await UpdateCategories(entity, updateEntity);
                await UpdateStudios(entity, updateEntity);

                UpdateSingleProperties(entity, updateEntity);

                await context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }
        private void UpdateSingleProperties(Animes newEntity, Animes updateEntity)
        {
            updateEntity.Title = newEntity.Title;
            updateEntity.OriginalTitle = newEntity.OriginalTitle;
            updateEntity.Synopsis = newEntity.Synopsis;
            updateEntity.Poster = newEntity.Poster;
            updateEntity.Duration = newEntity.Duration;
            updateEntity.Release = newEntity.Release;
            updateEntity.Trailer = newEntity.Trailer;
            updateEntity.TotalEpisodes = newEntity.TotalEpisodes;
            updateEntity.StatusId = newEntity.StatusId;
            updateEntity.TypeId = newEntity.TypeId;
            updateEntity.CountryId = newEntity.CountryId;
            updateEntity.AgeRatingId = newEntity.AgeRatingId;

            updateEntity.ModifiedDate = DateTime.Now;
        }
        private async Task UpdateStudios(Animes newEntity, Animes updateEntity)
        {
            var oldStudioIds = updateEntity.Studios.Where(x => !x.IsDeleted).Select(s => s.Id).ToArray();
            var newStudioIds = newEntity.StudiosId;

            var removeStudioIds = oldStudioIds.Except(newStudioIds);
            var insertStudioIds = newStudioIds.Except(oldStudioIds);

            foreach (var studioId in removeStudioIds)
            {
                var removeStudio = updateEntity.Studios.FirstOrDefault(x => x.Id == studioId && !x.IsDeleted);
                if (removeStudio == null) continue;
                updateEntity.Studios.Remove(removeStudio);
            }

            foreach (var studioId in insertStudioIds)
            {
                var insertStudio = await context.Studios.FirstOrDefaultAsync(x => x.Id == studioId && !x.IsDeleted);
                if (insertStudio == null) continue;
                updateEntity.Studios.Add(insertStudio);
            }
        }

        private async Task UpdateCategories(Animes newEntity, Animes updateEntity)
        {
            var oldCategoryIds = updateEntity.Categories.Where(x => !x.IsDeleted).Select(x => x.Id).ToArray();
            var newCategoryIds = newEntity.CategoriesId;

            var removeCategoryIds = oldCategoryIds.Except(newCategoryIds);
            var insertCategoryIds = newCategoryIds.Except(oldCategoryIds);

            foreach (var categoryId in removeCategoryIds)
            {
                var removeCategory = updateEntity.Categories.FirstOrDefault(x => x.Id == categoryId && !x.IsDeleted);
                if (removeCategory != null)
                {
                    updateEntity.Categories.Remove(removeCategory);
                }
            }

            foreach (var categoryId in insertCategoryIds)
            {
                var insertCategory = await context.Categories.FirstOrDefaultAsync(x => x.Id == categoryId && !x.IsDeleted);
                if (insertCategory != null)
                {
                    updateEntity.Categories.Add(insertCategory);
                }
            }
        }
        public async Task<Paging<Animes>> GetPaging(string searchTitle, int pageNumber, int pageSize)
        {
            var searchResult = await context.Animes
                .Where(x => (x.Title.Contains(searchTitle) || String.IsNullOrEmpty(searchTitle)) && !x.IsDeleted)
                .OrderBy(x => x.Title)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToListAsync();
            ;

            var searchCount = await context.Animes
                .Where(x => (x.Title.Contains(searchTitle) || String.IsNullOrEmpty(searchTitle)) && !x.IsDeleted)
                .CountAsync();

            var result = new Paging<Animes>()
            {
                Data = searchResult,
                TotalPages = (int)Math.Ceiling(searchCount * 1.0 / pageSize),
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return result;
        }
        public async Task<AnimeWatchingViewModel> GetAnimeWatching(int id)
        {
            var anime = await context.Animes.FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id);
            if (anime == null) return null;
            var result = new AnimeWatchingViewModel()
            {
                Id = anime.Id,
                Title = anime.Title,
                Server = context.Servers
                    .Where(server => !server.IsDeleted && server.Episodes.Any(y => y.AnimeId == id))
                    .Select(server =>
                        new ServerClientViewModel()
                        {
                            Id = server.Id,
                            Name = server.Name,
                            Description = server.Description,
                            Episodes = server.Episodes
                                .Where(episode => episode.AnimeId == id && !episode.IsDeleted)
                                .Select(episode => new EpisodeClientViewModel()
                                {
                                    Id = episode.Id,
                                    SortOrder = episode.SortOrder,
                                    Title = episode.Title,
                                    Url = episode.Url
                                }),
                        }),
                CommentCount = await context.Comments.Where(comment => !comment.IsDeleted && comment.AnimeId == anime.Id).CountAsync()
            };

            return await Task.FromResult(result);
        }

        public Task<bool> IncreaseView(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Paging<AnimeItemViewModel>> AdvanceSearch(AnimeSearchViewModel model)
        {
            if (model.SearchTitle == null)
                model.SearchTitle = "";
            model.SearchTitle = model.SearchTitle.ToLower();
            int length = model.CategoryIds.Length;
            var result = context.Animes
                .Where((x) =>
                            !x.IsDeleted &&
                            ((x.Title.ToLower().Contains(model.SearchTitle) || String.IsNullOrEmpty(model.SearchTitle)) ||
                            (x.OriginalTitle.ToLower().Contains(model.SearchTitle) || String.IsNullOrEmpty(model.SearchTitle))) &&
                            (x.TypeId == model.TypeId || model.TypeId == 0) &&
                            (x.CountryId == model.CountryId || model.CountryId == 0) &&
                            (x.StatusId == model.StatusId || model.StatusId == 0) &&
                            (x.AgeRatingId == model.AgeRatingId || model.AgeRatingId == 0) &&
                            (model.CategoryIds.Except(
                                x.Categories
                                .Where(c => !c.IsDeleted)
                                .Select(c => c.Id)).Count() != length || length == 0) &&
                            (model.RatingHigher <=
                             (x.Ratings.Any() ? x.Ratings.Sum(r => r.RatePoint) * 1.0 / x.Ratings.Count() : 1) ||
                             model.RatingHigher == 0)
                            && (model.ViewCountHigher <= x.ViewCount || model.ViewCountHigher == 0)
                )
                .OrderByDescending(x => x.ViewCount);

            //var totalPages = result.Count();
            var totalPages = await result.CountAsync();
            return await Task.FromResult(new Paging<AnimeItemViewModel>()
            {
                Data = result
                    .Skip((model.PageNumber - 1) * model.PageSize)
                    .Take(model.PageSize)
                    .Select(x => new AnimeItemViewModel()
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Poster = x.Poster,
                        ViewCount = x.ViewCount,
                        Type = x.Types.Name,
                        Status = x.Statuses.Name,
                        CurrentEpisode = x.Episodes
                            .Where(z => !z.IsDeleted)
                            .GroupBy(z => z.ServerId)
                            .Max(t => t.Count()),
                        TotalEpisode = x.TotalEpisodes,
                        CommentCount = context.Comments.Count(y => y.AnimeId == x.Id)
                    }),
                TotalPages = totalPages,
                PageCount = (int)Math.Ceiling(totalPages * 1.0 / model.PageSize),
                PageSize = model.PageSize,
                PageNumber = model.PageNumber
            });
        }

        public async Task<Paging<AnimeItemViewModel>> GetPageAnimeTrending(int start)
        {
            var result = context.Animes.Where(anime => !anime.IsDeleted)
               .OrderByDescending(x => x.ViewCount)
               .ThenByDescending(x => x.CreatedDate)
               .ThenByDescending(x => x.ModifiedDate)
               .Select(x => new AnimeItemViewModel()
               {
                   Id = x.Id,
                   Title = x.Title,
                   Poster = x.Poster,
                   ViewCount = x.ViewCount,
                   Type = x.Types.Name,
                   Status = x.Statuses.Name,
                   CurrentEpisode = x.Episodes
                        .Where(z => !z.IsDeleted)
                        .GroupBy(z => z.ServerId)
                        .Max(t => t.Count()),
                   TotalEpisode = x.TotalEpisodes,
                   CommentCount = context.Comments.Count(y => y.AnimeId == x.Id)
               }).Skip((start * 9)).Take(9);
            return await Task.FromResult(new Paging<AnimeItemViewModel>()
            {
                Data = result,
                PageCount = start + 1,
                PageSize = 9
            });
        }

        public async Task<Paging<AnimeItemViewModel>> GetPageAnimeRecenly(int start)
        {
            var result = context.Animes
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedDate)
                .ThenByDescending(x => x.ModifiedDate)
                .ThenByDescending(x => x.ViewCount)
                .Select(x => new AnimeItemViewModel()
                {
                    Id = x.Id,
                    Title = x.Title,
                    Poster = x.Poster,
                    ViewCount = x.ViewCount,
                    Type = x.Types.Name,
                    Status = x.Statuses.Name,
                    CurrentEpisode = x.Episodes
                        .Where(z => !z.IsDeleted)
                        .GroupBy(z => z.ServerId)
                        .Max(t => t.Count()),
                    TotalEpisode = x.TotalEpisodes,
                    CommentCount = context.Comments.Count(y => y.AnimeId == x.Id)
                }).Skip((start*9)).Take(9);
            return await Task.FromResult(new Paging<AnimeItemViewModel>()
            {
                Data = result,
                PageCount = start+1,
                PageSize= 9
            });
        }

        public async Task<int> GetCountTotalAnime()
        {
            int count = await context.Animes.CountAsync(x => !x.IsDeleted);
            return count/9;
        }
    }

}