﻿using DataModels.EF;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebAnime.Models;
using WebAnime.Models.Entities;
using WebAnime.Models.ViewModel.Admin;
using WebAnime.Repository.Interface;

namespace WebAnime.Repositories.Implement
{
    public class BlogRepository : IBlogRepository
    {
        public BlogRepository(AnimeDbContext context)
        {
            Context = context;
        }
        public AnimeDbContext Context { get; set; }
        public async Task<IEnumerable<Blogs>> GetAll()
        {
            return await Task.FromResult(Context.Blogs.Where(x => !x.IsDeleted).AsEnumerable());

        }

        public async Task<Blogs> GetById(int id)
        {
            return await Context.Blogs.FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id);
        }

        public async Task<bool> Create(Blogs entity)
        {
            try
            {
                var sameSlugList = Context.Blogs.Where(x => !x.IsDeleted && x.Slug.Contains(entity.Slug))
                    .Select(x => x.Slug);
                if (sameSlugList.Any())
                {
                    entity.Slug = $"{await sameSlugList.FirstOrDefaultAsync()}-{await sameSlugList.CountAsync()}";
                }
                entity.CreatedDate = DateTime.Now;
                entity.IsDeleted = false;
                entity.BlogCategories = new HashSet<BlogCategories>();

                foreach (int blogCategoryId in entity.BlogCategoryIds)
                {
                    var blogCategories =
                        await Context.BlogCategories.FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == blogCategoryId);
                    if (blogCategories != null)
                    {
                        entity.BlogCategories.Add(blogCategories);
                    }
                }
                Context.Blogs.Add(entity);

                await Context.SaveChangesAsync();

                return entity.Id > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Update(Blogs entity)
        {
            try
            {
                var updateEntity = await Context.Blogs.FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == entity.Id);
                if (updateEntity == null) return false;

                UpdateSingleProperties(entity, updateEntity);

                await UpdateCategories(entity, updateEntity);

                updateEntity.ModifiedDate = DateTime.Now;

                await Context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Delete(int id, int deletedBy = default)
        {
            try
            {
                var deleteEntity = Context.Blogs.FirstOrDefault(x => !x.IsDeleted && x.Id == id);
                if (deleteEntity == null) return false;

                deleteEntity.IsDeleted = true;
                deleteEntity.DeletedBy = deletedBy;
                deleteEntity.DeletedDate = DateTime.Now;


                await Context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Task<BlogViewModel> GetBlogViewModel(int blogId)
        {
            throw new NotImplementedException();
        }

        public async Task<Paging<Blogs>> GetPaping(string searchTitile, int pageSize, int pageNumber)
        {
            var searchResult = Context.Blogs
                .Where(x =>
                    (!x.IsDeleted) && (x.Title.Contains(searchTitile) || (String.IsNullOrEmpty(searchTitile))));

            var searchShow = searchResult
                .OrderBy(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
            var searchCount = await searchResult.CountAsync();
            var result = new Paging<Blogs>()
            {
                Data = searchShow,
                PageSize = pageSize,
                PageNumber = pageNumber,
                TotalPages = (int)Math.Ceiling(searchCount * 1.0 / pageSize)
            };
            return await Task.FromResult(result);
        }

        public Task<Blogs> GetByIdForClient(int id)
        {
            throw new NotImplementedException();
        }

        private async Task UpdateCategories(Blogs entity, Blogs updateEntity)
        {
            var oldCategoryIds = updateEntity.BlogCategories.Where(x => !x.IsDeleted).Select(x => x.Id).ToArray();
            var newCategoryIds = entity.BlogCategoryIds;

            var removeCategoryIds = oldCategoryIds.Except(newCategoryIds);
            var insertCategoryIds = newCategoryIds.Except(oldCategoryIds);

            foreach (var categoryId in removeCategoryIds)
            {
                var removeCategory = updateEntity.BlogCategories.FirstOrDefault(x => x.Id == categoryId && !x.IsDeleted);
                if (removeCategory != null)
                {
                    updateEntity.BlogCategories.Remove(removeCategory);
                }
            }

            foreach (var categoryId in insertCategoryIds)
            {
                var insertCategory = await Context.BlogCategories.FirstOrDefaultAsync(x => x.Id == categoryId && !x.IsDeleted);
                if (insertCategory != null)
                {
                    updateEntity.BlogCategories.Add(insertCategory);
                }
            }
        }
        void UpdateSingleProperties(Blogs source, Blogs dest)
        {
            dest.Slug = source.Slug;
            dest.Title = source.Title;
            dest.Content = source.Content;
            dest.ImageUrl = source.ImageUrl;
            dest.ModifiedBy = source.ModifiedBy;
        }

        Task<Models.ViewModel.Client.BlogViewModel> IBlogRepository.GetBlogViewModel(int blogId)
        {
            throw new NotImplementedException();
        }
    }
}