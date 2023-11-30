﻿using AutoMapper;
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

namespace WebAnime.Areas.Admin.Controllers
{
    [AdminAreaAuthorize]
    public class BlogController : Controller
    {
        // GET: Admin/Blog
        private readonly IBlogCategoryRepository _blogCategoryRepository;
        private readonly IBlogRepository _blogRepository;
        private readonly IMapper _mapper;

        public BlogController(IBlogCategoryRepository blogCategoryRepository, IBlogRepository blogRepository, IMapper mapper)
        {
            _blogCategoryRepository = blogCategoryRepository;
            _blogRepository = blogRepository;
            _mapper = mapper;

        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {

            var blogList = await _blogRepository.GetAll();
            var blogListViewModel = _mapper.Map<IEnumerable<Blogs>, IEnumerable<BlogViewModel>>(blogList);

            return await Task.FromResult<ActionResult>(View(blogListViewModel));
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            await LoadDependencies();
            return await Task.FromResult(View());
        }
        [HttpPost]
        public async Task<ActionResult> Create(BlogViewModel model)
        {
            model.CreatedBy = User.Identity.GetUserId<int>();
            var blog = _mapper.Map<Blogs>(model);
            var result = await _blogRepository.Create(blog);
            if (result)
            {
                TempData[AlertConstants.SuccessMessage] = "Thêm mới bài viết thành công!";
                return RedirectToAction("Index");
            }

            TempData[AlertConstants.ErrorMessage] = "Có lỗi xảy ra, vui lòng thử lại";
            return await Task.FromResult(View(model));
        }

        [HttpGet]
        public async Task<ActionResult> Update(int id)
        {
            await LoadDependencies();

            var blog = await _blogRepository.GetById(id);
            var blogViewModel = _mapper.Map<BlogViewModel>(blog);

            return await Task.FromResult(View(blogViewModel));
        }
        [HttpPost]
        public async Task<ActionResult> Update(BlogViewModel model)
        {
            await LoadDependencies();

            if (ModelState.IsValid)
            {
                model.ModifiedBy = User.Identity.GetUserId<int>();
                var blog = _mapper.Map<Blogs>(model);
                var result = await _blogRepository.Update(blog);
                if (result)
                {
                    TempData[AlertConstants.SuccessMessage] = "Cập nhật bài viết thành công!";
                    return RedirectToAction("Index");

                }
            }

            TempData[AlertConstants.ErrorMessage] = "Có lỗi xảy ra, vui lòng thử lại";
            return await Task.FromResult(View(model));
        }
        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            await LoadDependencies();
            var blogViewModel = _mapper.Map<BlogViewModel>(await _blogRepository.GetById(id));

            return await Task.FromResult(View(blogViewModel));
        }

        [HttpPost]
        public async Task<ActionResult> Delete(BlogViewModel model)
        {
            var deletedBy = User.Identity.GetUserId<int>();
            bool result = await _blogRepository.Delete(model.Id, deletedBy);
            if (result)
            {
                TempData[AlertConstants.SuccessMessage] = "Xóa bài viết thành công";
                return RedirectToAction("Index");
            }

            TempData[AlertConstants.SuccessMessage] = "Có lỗi xảy ra, vui lòng thử lại";
            ModelState.AddModelError(string.Empty, @"Lỗi xóa, vui lòng thử lại");
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> GetPaging(string searchTitle, int pageNumber, int pageSize)
        {
            var queryResult = await _blogRepository.GetPaping(searchTitle, pageSize, pageNumber);
            var result = queryResult.Data
                .Select(
                    x => new
                    {
                        x.Title,
                        x.Content,
                        x.ImageUrl,
                        x.Id
                    });

            if (result == null)
                return HttpNotFound();

            return Json(new
            {
                data = result,
                queryResult.TotalPages,
            }, JsonRequestBehavior.AllowGet);
        }

        async Task LoadDependencies()
        {
            var blogCategories = await _blogCategoryRepository.GetAll();
            ViewBag.BlogCategories = blogCategories;
        }
    }
}