using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Ninject;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebAnime.App_Start;
using WebAnime.Components;
using WebAnime.Models.Entities;
using WebAnime.Models.ViewModel.Admin;
using WebAnime.Repository.Interface;
using static WebAnime.Util.MappingData;

namespace WebAnime.Areas.Admin.Controllers
{
    //[AdminAreaAuthorize]
    public class AnimeController : Controller
    {
        private readonly IAnimeRepository _animeRepository;
        private readonly IAgeRatingRepository _ageRatingRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITypeRepository _typeRepository;
        private readonly IStudioRepository _studioRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IStatusRepository _statusRepository;
        private readonly IServerRepository _serverRepository;
        public AnimeController(
            IAnimeRepository animeRepository,
            IAgeRatingRepository ageRatingRepository,
            ICategoryRepository categoryRepository,
            ITypeRepository typeRepository,
            IStudioRepository studioRepository,
            ICountryRepository countryRepository,
            IStatusRepository statusRepository,
            IServerRepository serverRepository
            )
        {
            _animeRepository = animeRepository;
            _ageRatingRepository = ageRatingRepository;
            _categoryRepository = categoryRepository;
            _typeRepository = typeRepository;
            _studioRepository = studioRepository;
            _countryRepository = countryRepository;
            _statusRepository = statusRepository;
            _serverRepository = serverRepository;
        }

        // GET: Admin/Anime
        public async Task<ActionResult> Index()
        {
            var firstServer = await _serverRepository.GetFirst();
            ViewBag.FirstServerId = firstServer.Id;
            return View();
        }
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            await LoadEditData();
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Create(AnimeViewModel model)
        {
            if (ModelState.IsValid)
            {

                var entity = converToAnimes(model);
                //entity.ModifiedBy = User.Identity.GetUserId<int>();
                entity.ModifiedBy = 1;
                if (await _animeRepository.Create(entity))
                {
                    return RedirectToAction("Index", "Anime");
                }
                await LoadEditData();
                ModelState.AddModelError(string.Empty, @"Lỗi thêm mới, vui lòng thử lại");
            }
            await LoadEditData();
            ModelState.AddModelError(string.Empty, @"Lỗi đầu vào, vui lòng kiểm tra lại");
            return View();
        }


        [HttpGet]
        public async Task<ActionResult> update(int id)
        {
            await LoadEditData();
            var anime = await _animeRepository.GetById(id);
            if (anime == null)
            {
                return HttpNotFound(string.Empty);
            }
            var animeViewModel = convertToAnimeViewModel(anime);
            return View(animeViewModel);
        }
        [HttpPost]
        public async Task<ActionResult> Update(AnimeViewModel model)
        {
            if (ModelState.IsValid)
            {

                var entity = converToAnimes(model);

                entity.ModifiedBy = User.Identity.GetUserId<int>();

                if (await _animeRepository.Update(entity))
                {
                    return RedirectToAction("Index", "Anime");
                }
                await LoadEditData();
                ModelState.AddModelError(string.Empty, @"Lỗi cập nhật, vui lòng thử lại");
            }
            await LoadEditData();
            ModelState.AddModelError(string.Empty, @"Lỗi đầu vào, vui lòng kiểm tra lại");
            return View(model);
        }
        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            var anime = await _animeRepository.GetById(id);
            if (anime == null) return HttpNotFound(string.Empty);
            var animeViewModel = convertToAnimeViewModel(anime);
            return View(animeViewModel);
        }
        [HttpPost]
        public async Task<ActionResult> Delete(AnimeViewModel model)
        {
            int deletedBy = 1;

            if (await _animeRepository.Delete(model.Id, deletedBy))
            {
                return RedirectToAction("Index", "Anime");
            }
            ModelState.AddModelError(string.Empty, @"Lỗi xoá, vui lòng thử lại");
            return View(model);
        }
        [HttpGet]
        public async Task<ActionResult> GetPaging(string searchTitle, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0) return HttpNotFound(string.Empty);

            var queryResult = await _animeRepository.GetPaging(searchTitle, pageNumber, pageSize);
            var result = queryResult.Data
                .Select(
                    x => new
                    {
                        x.Title,
                        x.OriginalTitle,
                        x.Id,
                        x.Duration,
                        x.TotalEpisodes,
                        x.Synopsis,
                        Release = x.Release?.ToString("dd/MM/yyyy") ?? "Khong biet",
                        Categories = String.Join(",", x.Categories.Where(z => !z.IsDeleted).Select(c => c.Name)) + "."
                    }
                );

            return Json(new
            {
                data = result,
                queryResult.TotalPages

            }, JsonRequestBehavior.AllowGet);
        }
        async Task LoadEditData()
        {
            var ageRatingTask = _ageRatingRepository.GetAll();
            var categoryTask = _categoryRepository.GetAll();
            var countryTask = _countryRepository.GetAll();
            var statusTask = _statusRepository.GetAll();
            var studioTask = _studioRepository.GetAll();
            var typeTask = _typeRepository.GetAll();

            await Task.WhenAll(ageRatingTask, categoryTask, countryTask, statusTask, studioTask, typeTask);

            ViewBag.AgeRating = ageRatingTask.Result;
            ViewBag.Category = categoryTask.Result;
            ViewBag.Country = countryTask.Result;
            ViewBag.Status = statusTask.Result;
            ViewBag.Studio = studioTask.Result;
            ViewBag.Type = typeTask.Result;
        }
    }
}