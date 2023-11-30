﻿
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebAnime.Models.Entities.Identity;
using WebAnime.Models.ViewModel.Admin;
using static WebAnime.Util.MappingData;
using WebAnime.Models.Entities;
using WebAnime.Models.Helpers;
using WebAnime.Components;
using AutoMapper;

namespace WebAnime.Areas.Admin.Controllers
{
    [AdminAreaAuthorize]
    public class UserController : Controller
    {
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IMapper _mapper;

        public UserController(UserManager userManager, RoleManager roleManager, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }
        public async Task<ActionResult> Index()
        {
            var users = _userManager.Users.Where(x => !x.IsDeleted);
            var usersViewModel = _mapper.Map<IQueryable<Users>, IEnumerable<WebAnime.Models.ViewModel.Admin.UserViewModel>> (users);

            return await Task.FromResult(View(usersViewModel));
        }
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var roleList = _roleManager.Roles;
            ViewBag.Roles = roleList;

            return await Task.FromResult(View());
        }

        [HttpPost]
        public async Task<ActionResult> Create(UserViewModel model)
        {
            var roleList = _roleManager.Roles.ToList();
            ViewBag.Roles = roleList;

            if (ModelState.IsValid)
            {
                if (!model.Password.Equals(model.ReTypePassword))
                {
                    ModelState.AddModelError("ErrorConfirmPassword", @"Mật khẩu xác nhận không đúng, hãy thử lại");
                    return View(model);
                }

                var existUsername = await _userManager.FindByNameAsync(model.UserName);
                if (existUsername != null)
                {
                    ModelState.AddModelError("ExistUsername", @"Tài khoản đã tồn tại");
                    return View(model);
                }

                var existEmail = await _userManager.FindByEmailAsync(model.Email);
                if (existEmail != null)
                {
                    ModelState.AddModelError("ExistEmail", @"Địa chỉ đã tồn tại");
                    return View(model);
                }


                var user = converToUsersModel(model);

                var insertRoleList = roleList.Where(a => model.RoleListIds.Contains(a.Id)).Select(a => a.Name).ToArray();

                user.CreatedBy = User.Identity.GetUserId<int>();

                user.CreatedDate = DateTime.Now;

                user.EmailConfirmed = true;
                user.PhoneNumberConfirmed = true;

                IdentityResult result = await _userManager.CreateAsync(user, model.Password);
                int x;
                if (result.Succeeded)
                {
                    IdentityResult roleResult = await _userManager.AddToRolesAsync(user.Id, insertRoleList);

                    if (!roleResult.Succeeded)
                    {
                        x = 0;
                        foreach (var error in roleResult.Errors)
                        {
                            ModelState.AddModelError($"Error_{++x}", error);
                            return View(model);
                        }
                    }
                    return RedirectToAction("Index");
                }

                x = 0;
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError($"Error_{++x}", error);
                }

                return View(model);
            }
            ModelState.AddModelError("TotalError", @"Lỗi dữ liệu đầu vào, hãy kiểm tra lại");
            return View(model);
        }
        [HttpGet]
        public async Task<ActionResult> Update(int id)
        {
            var roleList = _roleManager.Roles.ToList();
            ViewBag.Roles = roleList;
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return new HttpNotFoundResult("Cannot find user");
            }

            var usersViewModel = converToUserModel(user);
            usersViewModel.Password = usersViewModel.ReTypePassword = "abc";//fake
            return await Task.FromResult(View(usersViewModel));
        }

        [HttpPost]
        public async Task<ActionResult> Update(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var roleList = _roleManager.Roles.ToArray();
                ViewBag.Roles = roleList;
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    return new HttpNotFoundResult("Cannot find user");
                }
                user.BirthDay = model.BirthDay;
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                user.FullName = model.FullName;
                user.AvatarUrl = model.AvatarUrl;

                var oldRoleIds = _roleManager.GetRoleIdsFromUser(_userManager, user.Id).ToArray();
                var newRoleIds = model.RoleListIds ?? Array.Empty<int>();
                var removeUserRoleIds = oldRoleIds.Except(newRoleIds);
                var insertUserRoleIds = newRoleIds.Except(oldRoleIds);

                var roleListIds = roleList.Select(x => x.Id).ToArray();

                int countError = 0;
                foreach (var removeRoleId in removeUserRoleIds)
                {
                    if (roleListIds.Contains(removeRoleId))
                    {
                        var removeRole = roleList.FirstOrDefault(x => x.Id == removeRoleId);
                        if (removeRole != null)
                        {
                            IdentityResult removeResult = await _userManager.RemoveFromRoleAsync(user.Id, removeRole.Name);
                            if (!removeResult.Succeeded)
                            {
                                foreach (var removeResultError in removeResult.Errors)
                                {
                                    countError++;
                                    ModelState.AddModelError(removeResultError, removeResultError);
                                }
                            }
                        }
                    }
                }
                foreach (var insertRoleId in insertUserRoleIds)
                {
                    if (roleListIds.Contains(insertRoleId))
                    {
                        var insertRole = roleList.FirstOrDefault(x => x.Id == insertRoleId);

                        if (insertRole != null)
                        {
                            var insertRoleResult = await _userManager.AddToRoleAsync(user.Id, insertRole.Name);
                            if (!insertRoleResult.Succeeded)
                            {
                                foreach (var insertResultError in insertRoleResult.Errors)
                                {
                                    countError++;
                                    ModelState.AddModelError(insertResultError, insertResultError);
                                }
                            }
                        }
                    }
                }

                user.ModifiedBy = User.Identity.GetUserId<int>();
                user.ModifiedDate = DateTime.Now;

                IdentityResult updateUserResult = await _userManager.UpdateAsync(user);

                foreach (var userError in updateUserResult.Errors)
                {
                    countError++;
                    ModelState.AddModelError(userError, userError);
                }
                if (countError > 0)
                {
                    return View(model);
                }

                if (updateUserResult.Succeeded)
                {
                    return RedirectToAction("Index");

                }
            }
            return View(model);
        }
        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return new HttpNotFoundResult("Cannot find user");
            }

            var usersViewModel = converToUserModel(user);
            return await Task.FromResult(View(usersViewModel));
        }

        [HttpPost]
        public async Task<ActionResult> Delete(UserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return new HttpNotFoundResult("Cannot find user");
            }

            user.IsDeleted = true;
            user.DeletedBy = User.Identity.GetUserId<int>();

            IdentityResult result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                return View(model);
            }
            return RedirectToAction("Index");
        }
    }
}