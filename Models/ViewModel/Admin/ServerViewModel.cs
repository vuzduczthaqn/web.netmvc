﻿using System.ComponentModel.DataAnnotations;

namespace WebAnime.Models.ViewModel.Admin
{
    public class ServerViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} là bắt buộc")]
        [StringLength(50, ErrorMessage = "{0} phải từ {2} tới {1} ký tự", MinimumLength = 2)]
        [Display(Name = "Tên server")]
        public string Name { get; set; }

        [Required(ErrorMessage = "{0} là bắt buộc")]
        [Display(Name = "Mô tả")]
        public string Description { get; set; }
    }
}