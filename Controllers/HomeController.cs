﻿using System.Data;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebAnime.Repository.Interface;

namespace WebAnime.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAgeRatingRepository _ageRatingRepository;
        private readonly IAnimeRepository _animeRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITypeRepository _typeRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IStatusRepository _statusRepository;

        public HomeController(IAgeRatingRepository ageRatingRepository, IAnimeRepository animeRepository, ICategoryRepository categoryRepository, ITypeRepository typeRepository, ICountryRepository countryRepository, IStatusRepository statusRepository)
        {
            _ageRatingRepository = ageRatingRepository;
            _animeRepository = animeRepository;
            _categoryRepository = categoryRepository;
            _typeRepository = typeRepository;
            _countryRepository = countryRepository;
            _statusRepository = statusRepository;
        }
        public ActionResult Index()
        {

            return View();
        }
        public async Task<ActionResult> Search()
        {
            await LoadDependencies();

            return await Task.FromResult(View());
        }
        async Task LoadDependencies()
        {
            var ageRatingTask = _ageRatingRepository.GetAll();
            var categoryTask = _categoryRepository.GetAll();
            var countryTask = _countryRepository.GetAll();
            var statusTask = _statusRepository.GetAll();
            var typeTask = _typeRepository.GetAll();

            await Task.WhenAll(ageRatingTask, categoryTask, countryTask, statusTask, typeTask);

            ViewBag.AgeRating = ageRatingTask.Result;
            ViewBag.Category = categoryTask.Result;
            ViewBag.Country = countryTask.Result;
            ViewBag.Status = statusTask.Result;
            ViewBag.Type = typeTask.Result;
        }
    }
}