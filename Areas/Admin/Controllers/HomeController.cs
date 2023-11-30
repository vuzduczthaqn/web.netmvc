using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAnime.Models;
using WebAnime.Models.Entities;
using System.Data.SqlClient;
using System.Configuration;
using Dapper;
using WebAnime.Components;
using WebAnime.Repository.Interface;

namespace WebAnime.Areas.Admin.Controllers
{
    [AdminAreaAuthorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}