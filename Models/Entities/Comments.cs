﻿using WebAnime.Models.Entities.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAnime.Models.Entities
{
    public class Comments
    {
        public int Id { get; set; }
        [MaxLength(500)] public string Content { get; set; }


        [ForeignKey(nameof(Anime))] public int AnimeId { get; set; }
        public Animes Anime { get; set; }

        [ForeignKey(nameof(Episode))] public int? EpisodeId { get; set; }
        public virtual Episodes Episode { get; set; }

        [DataType(DataType.DateTime)] public DateTime? CreatedDate { get; set; }
        [ForeignKey(nameof(User))] public int CreatedBy { get; set; }
        public Users User { get; set; }

        [DataType(DataType.DateTime)] public DateTime? DeletedDate { get; set; }
        public int? DeletedBy { get; set; }

        public bool IsDeleted { get; set; }


    }
}
