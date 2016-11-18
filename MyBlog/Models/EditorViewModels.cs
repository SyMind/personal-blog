using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MyBlog.Models
{
    public class EditorView
    {
        [Display(Name ="标题")]
        public string Title { get; set; }

        [Display(Name = "类别")]
        public IEnumerable<Category> Categories { get; set; }

        public IEnumerable<Category> HadCategories { get; set; }

        public string ContentTxt { get; set; }

        public bool IsAlter { get; set; }

        public int ArticalId { get; set; }
    }

    // 数据处理
    public class EditorViewModelsHelper
    {
        private PostRepository pr = new PostRepository();
        private CategoryRepository cr = new CategoryRepository();

        public EditorView GetEditorView(int? id = null)
        {
            EditorView result = new EditorView();
            result.Categories = cr.GetAllCategories();
            if (id != null)
            {
                var post = pr.GetPostById((int)id);
                if (post != null)
                {
                    result.ContentTxt = post.Content;
                    result.Title = post.Title;
                    result.ArticalId = post.ArticalId;
                    result.IsAlter = true;
                    List<Category> categories = new List<Category>();
                    string[] categoryIds = post.CategoryId.Split(';');
                    for (var i = 0; i < categoryIds.Length - 1; i++)
                    {
                        categories.Add(new Category()
                        {
                            CategoryId = int.Parse(categoryIds[i]),
                            Name = cr.GetCategoryName(int.Parse(categoryIds[i]))
                        });
                    }
                    result.HadCategories = categories;
                    return result;
                }          
            }

            result.ContentTxt = "";
            result.Title = "";
            result.ArticalId = -1;
            result.IsAlter = false;
            return result;
        }
    }
}