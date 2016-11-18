using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyBlog.Models;
using System.Text;

namespace MyBlog.Controllers
{
    public class AccountController : Controller
    {
        private AccountRepository ar;
        private PostRepository pr;
        private IpControlRepository icr;

        public AccountController()
        {
            ar = new AccountRepository();
            pr = new PostRepository();
            icr = new IpControlRepository();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(AccountLogin account)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }

            if (account.VerificationCode.ToUpper() != Session["VerificationCode"].ToString().ToUpper())
            {
                ModelState.AddModelError("VerificationCode", "验证码 输入错误。");
                return View();
            }

            AccountRepository.ValidateResult result = ar.ValidateAccount(account.Name, ar.Str2MD5(account.Password));
            if (result == AccountRepository.ValidateResult.Success)
            {
                HttpCookie cookie = new HttpCookie("Account");
                cookie["Name"] = account.Name;
                cookie["Password"] = ar.Str2MD5(account.Password);
                Response.Cookies.Add(cookie);

                if (Session["ReturnUrl"] != null)
                {
                    return Redirect(Session["ReturnUrl"].ToString());
                }
                return Redirect("/Account");
            }

            if (result == AccountRepository.ValidateResult.WarryPassword)
            {
                ModelState.AddModelError("Password", "密码 输入错误。");
                return View();
            }

            return View();
        }

        public ActionResult Logout()
        {
            HttpCookie cookie = Request.Cookies["Account"];
            cookie.Expires = DateTime.Now;
            Response.Cookies.Add(cookie);
            return Redirect(Request.UrlReferrer.ToString());
        }

        [ActionName("Index")]
        [CustomizedAuthotize]
        public ActionResult Management()    // 管理所写文章（管理页默认页面）
        {
            IEnumerable<Post> posts = pr.GetPostsOrderByTime();
            return View("Management", posts);
        }

        [CustomizedAuthotize]
        public ActionResult ManagementInfo()    // 管理个人信息
        {
            HttpCookie cookie = Request.Cookies["Account"];
            Account account = ar.GetAccountByName(cookie["Name"]);
            return View(account);
        }

        [HttpPost]
        [CustomizedAuthotize]
        public ActionResult AlterInfo(Account account)     // 修改个人信息（不包含密码修改）
        {
            if(ar.TryUpdateAccoount(account))
            {
                return Json(new { result = true });
            }
            else
            {
                return Json(new { result = false });
            }
        }

        public ActionResult ManagementWeb(int pageIndex = 0)
        {
            int pageLength = 10;    // 定义列表项为10
            var icr = new IpControlRepository();
            var number = icr.GetIpControlNumber();
            ViewBag.PageIndex = pageIndex;
            ViewBag.PageNumber = number / pageLength;
            return View(icr.GetAllIpControl(pageIndex * pageLength, pageLength));
        }

        // 生成验证码的方法
        public ActionResult VerificationCode()
        {
            string chkCode = string.Empty;
            Color[] color = { Color.Black, Color.Red, Color.Green, Color.Orange, Color.Brown };
            string[] font = { "Times New Roman", "MS Mincho", "Book Antiqua", "Gungsuh", "PMingLiU", "Impact" };
            char[] character = { '2', '3', '4', '5', '6', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'R', 'S', 'T', 'W', 'X', 'Y' };
            Random rnd = new Random();
            for (int i = 0; i < 4; i++)
            {
                chkCode += character[rnd.Next(character.Length)];
            }

            Session["VerificationCode"] = chkCode;

            Bitmap bmp = new Bitmap(100, 40);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.White);

            for (int i = 0; i < 10; i++)
            {
                int x1 = rnd.Next(100);
                int y1 = rnd.Next(40);
                int x2 = rnd.Next(100);
                int y2 = rnd.Next(40);
                Color clr = color[rnd.Next(color.Length)];
                g.DrawLine(new Pen(clr), x1, y1, x2, y2);
            }

            for (int i = 0; i < chkCode.Length; i++)
            {
                string fnt = font[rnd.Next(font.Length)];
                Font ft = new Font(fnt, 18);
                Color clr = color[rnd.Next(color.Length)];
                g.DrawString(chkCode[i].ToString(), ft, new SolidBrush(clr), (float)i * 20 + 8, (float)8);
            }

            for (int i = 0; i < 100; i++)
            {
                int x = rnd.Next(bmp.Width);
                int y = rnd.Next(bmp.Height);
                Color clr = color[rnd.Next(color.Length)];
                bmp.SetPixel(x, y, clr);
            }
            MemoryStream ms = new MemoryStream();
            try
            {
                bmp.Save(ms, ImageFormat.Png);
                return File(ms.ToArray(), @"image/Png");
            }
            finally
            {
                bmp.Dispose();
                g.Dispose();
            }
        }
    }

    // 自定义用户验证过滤器
    public class CustomizedAuthotizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var ar = new AccountRepository();
            var cookie = httpContext.Request.Cookies["Account"];
            httpContext.Session["ReturnUrl"] = httpContext.Request.Url.ToString();
            if(cookie != null)
            {
                if (ar.ValidateAccount(cookie["Name"], cookie["Password"]) == AccountRepository.ValidateResult.Success)
                {
                    return true;
                }
            }
            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.HttpContext.Response.Redirect("/Account/Login");
        }
    }
}