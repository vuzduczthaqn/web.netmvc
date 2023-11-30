using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAnime.Models;
using WebAnime.Models.Entities;
using WebAnime.Models.ViewModel.Admin;
using WebAnime.Models.Entities.Identity;

namespace WebAnime.Util
{
    public class MappingData
    {
        private static AnimeDbContext context=new AnimeDbContext();

        public static AnimeViewModel convertToAnimeViewModel(Animes anime)
        {
            return new AnimeViewModel
            {
                Id = anime.Id,
                Title = anime.Title,
                OriginalTitle = anime.OriginalTitle,
                Synopsis = anime.Synopsis,
                Poster = anime.Poster,
                Duration = anime.Duration,
                Release = anime.Release,
                Trailer = anime.Trailer,
                TotalEpisodes = anime.TotalEpisodes,
                StatusId = anime.StatusId,
                TypeId = anime.TypeId,
                CountryId = anime.CountryId,
                AgeRatingId = anime.AgeRatingId,
                CategoriesId = anime.Categories.Select(category => category.Id).ToArray(),
                StudiosId = anime.Studios.Select(studio => studio.Id).ToArray(),
            };
        }
        public static Animes converToAnimes(AnimeViewModel animeViewModel)
        {
            return new Animes
            {
                Id = animeViewModel.Id,
                Title = animeViewModel.Title,
                OriginalTitle = animeViewModel.OriginalTitle,
                Synopsis = animeViewModel.Synopsis,
                Poster = animeViewModel.Poster,
                Duration = animeViewModel.Duration,
                Release = animeViewModel.Release,
                Trailer = animeViewModel.Trailer,
                TotalEpisodes = animeViewModel.TotalEpisodes,
                StatusId = animeViewModel.StatusId,
                TypeId = animeViewModel.TypeId,
                CountryId = animeViewModel.CountryId,
                AgeRatingId = animeViewModel.AgeRatingId,
                
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                IsDeleted = false,

                CategoriesId = animeViewModel.CategoriesId,
                StudiosId = animeViewModel.StudiosId
            };
        }
        public static EpisodeViewModel convertToEpisodeViewModel(Episodes episodes)
        {
            return new EpisodeViewModel
            {
                Id = episodes.Id,
                Title = episodes.Title,
                AnimeId = episodes.AnimeId,
                ServerId = episodes.ServerId,
                SortOrder = episodes.SortOrder,
                CreatedDate = (DateTime)episodes.CreatedDate,
                Url = episodes.Url,
                CreatedBy = (int)episodes.CreatedBy 
            };
        }
        public static Episodes convertToEpisodes(EpisodeViewModel episodes)
        {
            return new Episodes
            {
                AnimeId= episodes.AnimeId,
                CreatedBy= (int)episodes.CreatedBy,
                Url= episodes.Url,
                Title= episodes.Title,
                Id= episodes.Id,
                CreatedDate= (DateTime)episodes.CreatedDate,
                IsDeleted= false,
                ServerId= episodes.ServerId,
                SortOrder= episodes.SortOrder,        
            };
        }
        public static UserViewModel converToUserModel(Users users)
        {
            return new UserViewModel
            {
                Id = users.Id,
                AvatarUrl = users.AvatarUrl,
                Email = users.Email,
                FullName = users.FullName,
                UserName = users.UserName,
                PhoneNumber = users.PhoneNumber,
                LockoutEnabled = users.LockoutEnabled,
                LockoutEndDateUtc = users.LockoutEndDateUtc,
                
            };
        }
        public static Users converToUsersModel(UserViewModel users)
        {
            return new Users
            {
                Id = users.Id,
                AvatarUrl = users.AvatarUrl,
                Email = users.Email,
                FullName = users.FullName,
                    UserName = users.UserName,
                    PhoneNumber = users.PhoneNumber,
                    LockoutEnabled = users.LockoutEnabled,
                    IsDeleted = false,
                    
            };
        }
    }
}