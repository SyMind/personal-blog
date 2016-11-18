using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyBlog.Models;

namespace MyBlog.Controllers
{
    public class HomeController : Controller
    {
        private HomeViewHelper hvHelper;
        private PostRepository pr;

        public HomeController()
        {
            pr = new PostRepository();
            hvHelper = new HomeViewHelper();
            // 设置页面长度
            hvHelper.PageLength = 5;
        }

        public ActionResult Index(int pageIndex = 0)
        {
            var model = hvHelper.GetHomeViewModel(pageIndex);
            // 当前访客数量
            // ViewBag.VisitorCount = HttpContext.Application["VisitorCount"].ToString();
            return View(model);
        }

        public ActionResult ReadPost(int articalId)
        {
            return View(pr.GetPostById(articalId));
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult AboutWebSite()
        {
            return View();
        }
    }
}