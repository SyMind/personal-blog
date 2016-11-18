using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyBlog.Models;

namespace MyBlog.Controllers
{
    [CustomizedAuthotize]
    public class EditorController : Controller
    {
        private CategoryRepository cr;
        private PostRepository pr;
        private EditorViewModelsHelper evmh;

        public EditorController()
        {
            cr = new CategoryRepository();
            pr = new PostRepository();
            evmh = new EditorViewModelsHelper();
        }

        public ActionResult Index(int? id = null)
        {
            var model = evmh.GetEditorView(id);
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreatePost(Post post)
        {
            post.Content = post.Content.Replace(@"\u0026", "&");
            post.PostTime = DateTime.Now;
            if(true == pr.TryInsertPost(post))
            {
                return Json(new { Result = true });
            }
            else
            {
                return Json(new { Result = false });
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AlterPost(Post post)
        {
            post.Content = post.Content.Replace(@"\u0026", "&");
            if (pr.TryAlterPost(post)==true)
            {
                return Json(new { Result = true });
            }
            else
            {
                return Json(new { Result = false });
            }           
        }
    }
}