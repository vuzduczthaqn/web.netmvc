using System;

using System.ComponentModel.DataAnnotations;

namespace WebAnime.Models.ViewModel.Client
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; }

        public int AnimeId { get; set; }

        public int? EpisodeId { get; set; }

        [DataType(DataType.DateTime)] public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
    }
}
