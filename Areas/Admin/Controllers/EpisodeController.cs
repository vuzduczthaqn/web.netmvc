using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebAnime.Components;
using WebAnime.Models.Entities;
using WebAnime.Models.ViewModel.Admin;
using WebAnime.Repository.Interface;
using static WebAnime.Util.MappingData;

namespace WebAnime.Areas.Admin.Controllers
{
    [AdminAreaAuthorize]
    public class EpisodeController : Controller
    {
        // GET: Admin/Episode
        private readonly IServerRepository _serverRepository;
        private readonly IEpisodeRepository _episodeRepository;
        private readonly IAnimeRepository _animeRepository;


        public EpisodeController( IServerRepository serverRepository, IEpisodeRepository episodeRepository, IAnimeRepository animeRepository)
        {
            _serverRepository = serverRepository;
            _episodeRepository = episodeRepository;
            _animeRepository = animeRepository;

        }
        public async Task<ActionResult> Index(int animeId, int serverId)
        {
            var firstServer = await _serverRepository.GetFirst();
            if (firstServer == null) return HttpNotFound();

            await LoadEditData(animeId);
            ViewBag.Servers = await _serverRepository.GetAll();

            var episode= await _episodeRepository.GetAll(animeId, serverId);
            var episodeViewModelList = episode.Select(x=>convertToEpisodeViewModel(x)).ToList();
            ViewBag.ServerId = serverId;

            return View(episodeViewModelList);
        }
        [HttpGet]
        public async Task<ActionResult> Create(int animeId, int serverId)
        {
            var server = await _serverRepository.GetById(serverId);
            ViewBag.ServerName = server.Name;
            await LoadEditData(animeId);

            return View(new EpisodeViewModel()
            {
                Id = 0,
                AnimeId = animeId,
                ServerId = serverId,
                SortOrder = await _episodeRepository.GetMaxOrderId(animeId, serverId) + 1
            });
        }
        [HttpPost]
        public async Task<ActionResult> CreateMultiple(List<EpisodeViewModel> model)
        {
            var episodeList = model.Select(x=>convertToEpisodes(x)).ToList();

            var result = await _episodeRepository.AddRange(episodeList);

            return Json(new
            {
                success = result
            });

        }

        [HttpPost]
        public async Task<ActionResult> Create(EpisodeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var episode = convertToEpisodes(model);
                episode.CreatedBy = User.Identity.GetUserId<int>();

                if (await _episodeRepository.Create(episode))
                {
                    return RedirectToAction("Index", "Episode", new { area = "Admin", animeId = model.AnimeId, serverId = model.ServerId });
                }
                ModelState.AddModelError(string.Empty, @"Lỗi không thêm được, vui lòng thử lại");
            }
            ModelState.AddModelError(string.Empty, @"Đầu vào lỗi, vui lòng kiểm tra lại");
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Update(int id)
        {
            var episode = await _episodeRepository.GetById(id);
            if (episode == null) return HttpNotFound();
            var episodeViewModel = convertToEpisodeViewModel(episode);
            await LoadEditData(episodeViewModel.AnimeId);
            ViewBag.Server = await _serverRepository.GetById(episodeViewModel.ServerId);
            return View(episodeViewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Update(EpisodeViewModel model)
        {
            await LoadEditData(model.AnimeId);

            if (ModelState.IsValid)
            {
                var episode = convertToEpisodes(model);
                episode.ModifiedBy = User.Identity.GetUserId<int>();

                if (await _episodeRepository.Update(episode))
                {
                    return RedirectToAction("Index", "Episode", new { area = "Admin", animeId = episode.AnimeId, serverId = episode.ServerId });
                }
                ModelState.AddModelError(string.Empty, @"Lỗi không cập nhật được, vui lòng thử lại");
            }
            ModelState.AddModelError(string.Empty, @"Đầu vào lỗi, vui lòng kiểm tra lại");
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            var episode = await _episodeRepository.GetById(id);
            if (episode == null) return HttpNotFound();
            var episodeViewModel = convertToEpisodeViewModel(episode);
            await LoadEditData(episodeViewModel.AnimeId);
            ViewBag.Server = await _serverRepository.GetById(episodeViewModel.ServerId);
            return View(episodeViewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(EpisodeViewModel model)
        {

            var deleted = await _episodeRepository.GetById(model.Id);
            var animeId = deleted.AnimeId;
            var serverId = deleted.ServerId;

            int deletedBy = User.Identity.GetUserId<int>();

            if (await _episodeRepository.Delete(model.Id, deletedBy))
            {
                return RedirectToAction("Index", "Episode", new { area = "Admin", animeId, serverId });
            }
            ModelState.AddModelError(string.Empty, @"Lỗi không xoá được, vui lòng thử lại");
            return View();
        }
        async Task LoadEditData(int animeId)
        {
            ViewBag.Anime = await _animeRepository.GetById(animeId);
        }
    }
}