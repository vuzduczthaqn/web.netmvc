using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAnime.Models.Entities.Identity;
using WebAnime.Models.Entities;
using WebAnime.Models.ViewModel.Admin;
using WebAnime.Models.ViewModel.Client;
using WebAnime.Models.Helpers;
using Microsoft.AspNet.Identity;

namespace WebAnime.App_Start
{
    public class AutoMapperConfig
    {
        public static IMapper RegisterAutoMapper()
        {
            var config = new MapperConfiguration(
                cfg =>
                {
                    cfg.CreateMap<Countries, CountryViewModel>();
                    cfg.CreateMap<CountryViewModel, Countries>();


                    cfg.CreateMap<Animes, AnimeViewModel>()
                        .ForMember(
                            x => x.StudiosId,
                            option => { option.MapFrom(src => src.Studios.Select(x => x.Id).ToArray()); }
                        )
                        .ForMember(
                            x => x.CategoriesId,
                            option => { option.MapFrom(src => src.Categories.Select(x => x.Id).ToArray()); }
                        );

                    cfg.CreateMap<AnimeViewModel, Animes>();

                    cfg.CreateMap<StudioViewModel, Studios>();
                    cfg.CreateMap<Studios, StudioViewModel>();


                    cfg.CreateMap<ServerViewModel, Servers>();
                    cfg.CreateMap<Servers, ServerViewModel>();

                    cfg.CreateMap<EpisodeViewModel, Episodes>();
                    cfg.CreateMap<Episodes, EpisodeViewModel>();


                    cfg.CreateMap<Users, WebAnime.Models.ViewModel.Client.UserViewModel>()
                        .ForMember
                        (
                            x => x.RoleList,
                            options => options.MapFrom(
                                user =>
                                    NInjectConfig.GetService<UserManager>().GetRoles(user.Id).ToArray()
                            )
                        )
                        .ForMember(
                            x => x.RoleListIds,
                            options => options.MapFrom(user =>
                                NInjectConfig.GetService<RoleManager>()
                                    .GetRoleIdsFromUser(NInjectConfig.GetService<UserManager>(), user.Id)
                            )
                        );

                    cfg.CreateMap<ExternalLoginConfirmationViewModel, Users>();


                    cfg.CreateMap<WebAnime.Models.ViewModel.Client.UserViewModel, Users>();

                    cfg.CreateMap<Blogs, WebAnime.Models.ViewModel.Admin.BlogViewModel>()
                        .ForMember(
                            x => x.BlogCategoryIds,
                            memberOptions: options =>
                            {
                                options.MapFrom(
                                    blog =>
                                        blog.BlogCategories
                                            .Where(x => !x.IsDeleted)
                                            .Select(x => x.Id)
                                            .ToArray()
                                    );
                            }
                            )
                        ;
                    cfg.CreateMap<WebAnime.Models.ViewModel.Admin.BlogViewModel, Blogs>();

                    cfg.CreateMap<Blogs, BlogLitteViewModel>();


                    cfg.CreateMap<BlogCommentViewModel, BlogComments>();

                    cfg.CreateMap<BlogComments, BlogCommentViewModel>();

                    cfg.CreateMap<BlogCategories, BlogCategoryViewModel>();
                    cfg.CreateMap<BlogCategoryViewModel, BlogCategories>();

                    cfg.CreateMap<CommentViewModel, Comments>();
                    cfg.CreateMap<Comments, CommentViewModel>();

                    cfg.CreateMap<RegisterViewModel, Users>();

                }
            );
            return config.CreateMapper();
        }
    }
}