using AutoMapper;
using Microsoft.Owin.Security;
using Ninject;
using Ninject.Web.Common;
using System;
using System.Reflection;
using System.Web;
using WebAnime.Models;
using WebAnime.Repositories.Implement;
using WebAnime.Repositories;
using WebAnime.Repository.Interface;
using DataModels.EF;
using System.Data.SqlClient;
using System.Data;
using WebAnime.Repositories.Implement.Dapper;

namespace WebAnime.App_Start
{
    public class NInjectConfig
    {
        private static bool _cannotGet;
        private static IKernel _kernel;

        public static IKernel Kernel
        {
            get
            {
                if (_cannotGet)
                {
                    throw new NotSupportedException("Cannot get kernel!");
                }
                if (_kernel == null)
                {
                    _kernel = new StandardKernel();
                    _kernel.Load(Assembly.GetExecutingAssembly());
                    RegisterServices(_kernel);
                    _cannotGet = true;
                }
                return _kernel;
            }
            set => _kernel = value;
        }
        public static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<AnimeDbContext>().ToSelf();
            kernel.Bind<IMapper>().ToConstant(AutoMapperConfig.RegisterAutoMapper());
            RegisterRepository(kernel);
            RegisterRepositoryDapper(kernel);
            //RegisterOwinContext(kernel);

        }

        private static void RegisterRepository(IKernel kernel)
        {
            kernel.Bind<IAnimeRepository>().To<AnimeRepository>();
            kernel.Bind<IAgeRatingRepository>().To<AgeRatingRepository>();
            kernel.Bind<IBlogRepository>().To<BlogRepository>();
            kernel.Bind<ICategoryRepository>().To<CategoryRepository>();
            kernel.Bind<ICommentRepository>().To<CommentRepository>();
            kernel.Bind<IEpisodeRepository>().To<EpisodeRepository>();
            kernel.Bind<ICountryRepository>().To<CountryRepository>();
            kernel.Bind<IRatingRepository>().To<RatingRepository>();
            kernel.Bind<IServerRepository>().To<ServerRepository>();
            kernel.Bind<IStudioRepository>().To<StudioRepository>();
            kernel.Bind<ITypeRepository>().To<TypeRepository>();
            kernel.Bind<IStatusRepository>().To<StatusRepository>();
        }
        private static void RegisterOwinContext(IKernel kernel)
        {
            kernel.Bind<IAuthenticationManager>().ToMethod(
                _ =>
                    HttpContext.Current.GetOwinContext().Authentication
            );
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
        }
        public static T GetService<T>()
        {
            _cannotGet = false;
            T service = Kernel.TryGet<T>();
            _cannotGet = true;
            return service;
        }
        static void RegisterRepositoryDapper(IKernel kernel)
        {
            RegisterConnection(kernel);
            kernel.Bind<IBlogCategoryRepository>().To<BlogCategoryRepositoryDapper>();
            kernel.Bind<IBlogCommentRepository>().To<BlogCommentRepositoryDapper>();
        }

        private static void RegisterConnection(IKernel kernel)
        {
            kernel.Bind<IDbConnection>().ToMethod(_ =>
            {
                string connectionString =
                    System.Web.Configuration.WebConfigurationManager
                        .ConnectionStrings["ConnectionString"].ConnectionString;

                return new SqlConnection(connectionString);
            });
        }
    }
}