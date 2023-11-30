﻿
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebAnime.Components;
using WebAnime.Repository.Interface;

namespace WebAnime.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentRepository _commentRepository;

        public CommentController(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        [HttpGet]
        public async Task<JsonResult> LoadComment(int animeId, int pageNumber, int pageSize)
        {
            if (pageSize == 0) pageSize = CommonConstants.AnimeCommentPageSize;

            if (pageNumber <= 0) pageNumber = 1;

            var commentPage = await _commentRepository.GetPaging(animeId, pageNumber, pageSize);

            var jsonResult = new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new
                {
                    data = commentPage.Select(x =>
                    {
                        if (String.IsNullOrEmpty(x.AvatarUrl)) x.AvatarUrl = CommonConstants.DefaultAvatarUrl;
                        if (String.IsNullOrEmpty(x.UserFullName)) x.UserFullName = "Không biết";

                        if (String.IsNullOrEmpty(x.EpisodeTitle))
                        {
                            x.EpisodeTitle = "";
                        }
                        else
                        {
                            if (!(x.EpisodeTitle.Contains("Tập") || x.EpisodeTitle.Contains("Ep")))
                            {
                                x.EpisodeTitle = "Tập " + x.EpisodeTitle;
                            }
                        }
                        return new
                        {
                            x.AnimeId,
                            x.AvatarUrl,
                            x.UserFullName,
                            x.EpisodeTitle,
                            x.Content,
                            x.Id,
                            CreatedDate = x.CreatedDate?.ToString("dd/MM/yyyy - HH:mm:ss") ?? DateTime.Now.ToString("dd/MM/yyyy - HH:mm:ss")
                        };
                    })
                }
            };

            return jsonResult;
        }
    }
}