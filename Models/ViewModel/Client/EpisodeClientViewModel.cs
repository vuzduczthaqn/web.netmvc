﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAnime.Models.ViewModel.Client
{
    public class EpisodeClientViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int SortOrder { get; set; }
        public string Url { get; set; }
    }
}
