using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyBlog.Models
{
    public class HomeView
    {
        public IEnumerable<Post> posts { get; set; }
        public int PageNumber { get; set; }     // 共几页
        public int PageIndex { get; set; }      // 当前页码
    }

    public class HomeViewHelper
    {
        private PostRepository pr = new PostRepository();
        // 设置单页长度
        public int PageLength { get; set; }

        public HomeView GetHomeViewModel(int pageIndex = 0)
        {
            return new HomeView()
            {
                posts = pr.GetPostsOrderByTime(pageIndex * PageLength, PageLength),
                PageNumber = (int)Math.Floor(pr.GetPostsNumber() * 1.0 / PageLength),
                PageIndex = pageIndex
            };
        }
    }
}