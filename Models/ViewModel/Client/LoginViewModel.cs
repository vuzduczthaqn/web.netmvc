﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebAnime.Models.ViewModel.Client
{
    public class LoginViewModel
    {

        [Display(Name = "Tài khoản")]
        [Required(ErrorMessage = "{0} là bắt buộc")]
        [StringLength(32, MinimumLength = 5, ErrorMessage = "{0} phải dài từ {2} tới {1} ký tự")]
        public string UserName { get; set; }
        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "{0} là bắt buộc")]
        [StringLength(32, MinimumLength = 5, ErrorMessage = "{0} phải dài từ {2} tới {1} ký tự")]
        public string Password { get; set; }
        [Display(Name = "Lưu đăng nhập")]
        public bool RememberMe { get; set; }


    }
}