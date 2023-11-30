﻿using WebAnime.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using WebAnime.Models;

namespace WebAnime.Models.Helpers
{
    public static class LoadFirstData
    {
        public static void LoadStudios(this AnimeDbContext context)
        {
            if (!context.Studios.Any())
            {
                var listStudios = new List<Studios>
                {
                    new Studios { Name = "Studio Bind",CreatedDate = DateTime.Now,
                        CreatedBy = 1},
                    new Studios { Name = "Mappa" ,CreatedDate = DateTime.Now,
                        CreatedBy = 1},
                    new Studios { Name = "Studio Jemi",CreatedDate = DateTime.Now,
                        CreatedBy = 1 }
                };
                context.Studios.AddRange(listStudios);
                context.SaveChanges();
            }
        }

        public static void LoadTypes(this AnimeDbContext context)
        {
            if (!context.Types.Any())
            {
                var listTypes = new List<Types>
                {
                    new Types { Name = "TV Series",CreatedDate = DateTime.Now,
                        CreatedBy = 1 },
                    new Types { Name = "Movies",CreatedDate = DateTime.Now,
                        CreatedBy = 1 },
                    new Types { Name = "Bluray",CreatedDate = DateTime.Now,
                        CreatedBy = 1 },
                    new Types { Name = "OVA" ,CreatedDate = DateTime.Now,
                        CreatedBy = 1}
                };
                context.Types.AddRange(listTypes);
                context.SaveChanges();
            }
        }

        public static void LoadStatuses(this AnimeDbContext context)
        {
            if (!context.Statuses.Any())
            {
                var listStatuses = new List<Statuses>
                {
                    new Statuses
                    {
                        Name = "Đã hoàn thành",CreatedDate = DateTime.Now,
                        CreatedBy = 1
                    },
                    new Statuses
                    {
                        Name = "Chưa hoàn thành",CreatedDate = DateTime.Now,
                        CreatedBy = 1
                    },
                    new Statuses
                    {
                        Name = "Chưa phát sóng",CreatedDate = DateTime.Now,
                        CreatedBy = 1
                    }
                };
                context.Statuses.AddRange(listStatuses);
                context.SaveChanges();
            }
        }

        public static void LoadAgeRatings(this AnimeDbContext context)
        {
            if (!context.AgeRatings.Any())
            {
                var listAgeRatings = new List<AgeRatings>
                {
                    new AgeRatings { Name = "Mọi lứa tuổi" ,CreatedDate = DateTime.Now,
                        CreatedBy = 1},
                    new AgeRatings { Name = "Trẻ em (dưới 13 tuổi)" ,CreatedDate = DateTime.Now,
                        CreatedBy = 1},
                    new AgeRatings { Name = "13+",CreatedDate = DateTime.Now ,
                        CreatedBy = 1},
                    new AgeRatings { Name = "18+" ,CreatedDate = DateTime.Now,
                        CreatedBy = 1}
                };
                context.AgeRatings.AddRange(listAgeRatings);
                context.SaveChanges();
            }
        }

        public static void LoadCountries(this AnimeDbContext context)
        {
            if (!context.Countries.Any())
            {
                var listCountries = new List<Countries>
                {
                    new Countries
                    {
                        Name = "Nhật Bản",CreatedDate = DateTime.Now,
                        CreatedBy = 1
                    },
                    new Countries
                    {
                        Name = "Trung Quốc",CreatedDate = DateTime.Now,
                        CreatedBy = 1
                    },
                    new Countries
                    {
                        Name = "Hàn Quốc",CreatedDate = DateTime.Now,
                        CreatedBy = 1
                    },
                    new Countries
                    {
                        Name = "Quốc gia khác",CreatedDate = DateTime.Now,
                        CreatedBy = 1
                    }
                };
                context.Countries.AddRange(listCountries);
                context.SaveChanges();
            }
        }

        public static void LoadCategories(this AnimeDbContext context)
        {
            if (!context.Categories.Any())
            {
                var listCategoryName = new[]
                {
                    "Action", "Adventure", "Cartoon", "Comedy", "Dementia", "Demons", "Drama", "Ecchi", "Fantasy",
                    "Game", "Harem", "Historical", "Horror", "Josei", "Kids", "Live Action", "Magic", "Martial Arts",
                    "Mecha", "Military", "Music", "Mystery", "Parody", "Police", "Psychological", "Romance", "Samurai",
                    "School", "Sci-Fi", "Seinen", "Shoujo", "Shoujo Ai", "Shounen", "Shounen Ai", "Slice of Life",
                    "Space", "Sports", "Super Power", "Supernatural", "Thriller", "Tokusatsu", "Vampire", "Yaoi", "Yuri"

                };
                int n = listCategoryName.Length;
                for (int i = 0; i < n; i++)
                {
                    context.Categories.Add(new Categories()
                    {
                        Name = listCategoryName[i],
                        CreatedDate = DateTime.Now,
                        CreatedBy = 1
                    });
                }

                context.SaveChanges();
            }
        }

        public static void LoadServers(this AnimeDbContext context)
        {
            if (!context.Servers.Any())
            {
                var listServerName = new[] { "PlayerX", "Doodstream", "StreamTape", "MP4Upload" };
                foreach (var s in listServerName)
                {
                    context.Servers.Add(new Servers()
                    {
                        Name = s,
                        CreatedDate = DateTime.Now,
                        CreatedBy = 1
                    });
                }

                context.SaveChanges();
            }
        }

        public static void LoadBlogCategories(this AnimeDbContext context)
        {
            if (!context.BlogCategories.Any())
            {
                var listBlogCategories = new[] { "Anime", "Manga ", "Light Novel", "Game", "Cosplay" };
                foreach (var s in listBlogCategories)
                {
                    context.BlogCategories.Add(new BlogCategories()
                    {
                        Name = s,
                        CreatedDate = DateTime.Now,
                        CreatedBy = 1
                    });
                }

                context.SaveChanges();
            }
        }
    }
}
