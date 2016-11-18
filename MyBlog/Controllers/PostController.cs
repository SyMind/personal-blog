using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyBlog.Models;

namespace MyBlog.Controllers
{
    public class PostController : Controller
    {
        private PostRepository pr;
        private CategoryRepository cr;

        public PostController()
        {
            pr = new PostRepository();
            cr = new CategoryRepository();
        }

        public ActionResult Create()
        {
            return Redirect("/Editor");
        }

        public ActionResult Edit(int id)
        {
            return Redirect("/Editor?id="+id);
        }

        public ActionResult Details(int id)
        { 
            return View(pr.GetPostById(id));
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (pr.DeletePostById(id) == true)
            {
                return Json(new { Result = true });
            }
            else
            {
                return Json(new { Result = false });
            }
        }

        public ActionResult Research(int categoryId = -1)
        {
            IEnumerable<Post> result = null;
            if (categoryId != -1)
            {
                List<Post> posts = pr.ResearchByCategoryId(categoryId.ToString());
                ViewBag.ResearchTag = cr.GetCategoryName(categoryId);
                ViewBag.Number = posts.Count;
                result = posts;
            }
            else
            {
                result = pr.GetPostsOrderByTime();
                ViewBag.ResearchTag = "（请选择类型）";
                ViewBag.Number = pr.GetPostsNumber();
            } 
            return View(result);
        }
    }
}