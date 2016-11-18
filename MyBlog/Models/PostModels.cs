using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;

namespace MyBlog.Models
{
    public class Post
    {
        [Key]
        [Display(Name = "文章")]
        public int ArticalId { get; set; }

        [Display(Name = "类别")]
        public string CategoryId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "标题")]
        public string Title { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "摘要")]
        public string ContentTxt { get; set; }

        [Display(Name = "正文")]
        public string Content { get; set; }

        [Display(Name = "发表时间")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime PostTime { get; set; }
    }

    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Display(Name = "类别")]
        [StringLength(20)]
        public string Name { get; set; }
    }

    // 数据链接
    public class PostRepository
    {
        private string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["PostDb"].ConnectionString;

        public bool TryInsertPost(Post post)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                MySqlCommand cmd = new MySqlCommand(
                    "insert into Posts(CategoryId,Title,ContentTxt,Content,PostTime) values(@CategoryId,@Title,@ContentTxt,@Content,@PostTime)"
                    , conn
                );
                cmd.Parameters.AddWithValue("@CategoryId", post.CategoryId);
                cmd.Parameters.AddWithValue("@Title", post.Title);
                cmd.Parameters.AddWithValue("@ContentTxt", post.ContentTxt);
                cmd.Parameters.AddWithValue("@Content", post.Content);
                cmd.Parameters.AddWithValue("@PostTime", post.PostTime);
                try
                {
                    conn.Open();
                }
                catch
                {
                    return false;
                }
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }

            }
            return true;
        }

        public IEnumerable<Post> GetPostsOrderByTime(int startIndex = 0, int? length = null)
        {
            List<Post> posts = new List<Post>();
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                MySqlCommand cmd = new MySqlCommand(
                    "select ArticalId,CategoryId,Title,ContentTxt,Content,PostTime from Posts order by PostTime desc"
                    , conn
                );
                try
                {
                    conn.Open();
                }
                catch
                {
                    return posts;
                }
                try
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (startIndex != 0)
                            {
                                startIndex--;
                                continue;
                            }
                            else
                            {
                                if (length == null)
                                {
                                    posts.Add(new Post()
                                    {
                                        ArticalId = int.Parse(reader["ArticalId"].ToString()),
                                        CategoryId = reader["CategoryId"].ToString(),
                                        Title = reader["Title"].ToString(),
                                        ContentTxt = reader["ContentTxt"].ToString(),
                                        Content = reader["Content"].ToString(),
                                        PostTime = DateTime.Parse(reader["PostTime"].ToString())
                                    });
                                }
                                else
                                {
                                    if (length != 0)
                                    {
                                        posts.Add(new Post()
                                        {
                                            ArticalId = int.Parse(reader["ArticalId"].ToString()),
                                            CategoryId = reader["CategoryId"].ToString(),
                                            Title = reader["Title"].ToString(),
                                            ContentTxt = reader["ContentTxt"].ToString(),
                                            Content = reader["Content"].ToString(),
                                            PostTime = DateTime.Parse(reader["PostTime"].ToString())
                                        });
                                        length--;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return posts;
                }

            }
            return posts;
        }

        public int GetPostsNumber()
        {
            int result = -1;
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                MySqlCommand cmd = new MySqlCommand(
                    "select ArticalId from Posts"
                    , conn
                );
                try
                {
                    conn.Open();
                }
                catch
                {
                    return result;
                }
                try
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return result;
                }

            }
            return result;
        }

        public Post GetPostById(int articalId)
        {
            Post post = null;
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                MySqlCommand cmd = new MySqlCommand(
                    "select * from Posts where ArticalId=@ArticalId"
                    , conn
                );
                cmd.Parameters.AddWithValue("@ArticalId", articalId);
                try
                {
                    conn.Open();
                }
                catch
                {
                    return post;
                }
                try
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            post = new Post()
                            {
                                ArticalId = int.Parse(reader["ArticalId"].ToString()),
                                CategoryId = reader["CategoryId"].ToString(),
                                Title = reader["Title"].ToString(),
                                ContentTxt = reader["ContentTxt"].ToString(),
                                Content = reader["Content"].ToString(),
                                PostTime = DateTime.Parse(reader["PostTime"].ToString())
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return post;
                }

            }
            return post;
        }

        public bool TryAlterPost(Post post)
        {
            List<Post> posts = new List<Post>();
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                MySqlCommand cmd = new MySqlCommand(
                    "update Posts set ContentTxt=@ContentTxt,Content=@Content,CategoryId=@CategoryId where ArticalId=@ArticalId"
                    , conn
                );

                cmd.Parameters.AddWithValue("@ContentTxt", post.ContentTxt);
                cmd.Parameters.AddWithValue("@Content", post.Content);
                cmd.Parameters.AddWithValue("@ArticalId", post.ArticalId);
                cmd.Parameters.AddWithValue("@CategoryId", post.CategoryId);

                try
                {
                    conn.Open();
                }
                catch
                {
                    return false;
                }
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    return false;
                }

            }
            return true;
        }

        public bool DeletePostById(int articalId)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                MySqlCommand cmd = new MySqlCommand(
                    "delete from Posts where ArticalId=@ArticalId"
                    , conn
                );
                cmd.Parameters.AddWithValue("@ArticalId", articalId);
                try
                {
                    conn.Open();
                }
                catch
                {
                    return false;
                }
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }

            }
            return true;
        }

        public List<Post> ResearchByCategoryId(string CategoryId)
        {
            List<Post> posts = new List<Post>();
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                MySqlCommand cmd = new MySqlCommand(
                    "select ArticalId,CategoryId,Title,ContentTxt,Content,PostTime from Posts order by PostTime desc"
                    , conn
                );
                try
                {
                    conn.Open();
                }
                catch
                {
                    return posts;
                }
                try
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (reader["CategoryId"].ToString().IndexOf(CategoryId) != -1)
                            {
                                posts.Add(new Post()
                                {
                                    ArticalId = int.Parse(reader["ArticalId"].ToString()),
                                    CategoryId = reader["CategoryId"].ToString(),
                                    Title = reader["Title"].ToString(),
                                    ContentTxt = reader["ContentTxt"].ToString(),
                                    Content = reader["Content"].ToString(),
                                    PostTime = DateTime.Parse(reader["PostTime"].ToString())
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return posts;
                }
            }
            return posts;
        }
    }

    public class CategoryRepository
    {
        private string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["PostDb"].ConnectionString;

        public bool TryInsertCategory(Category category)
        {
            MySqlCommand cmd = new MySqlCommand(
                "insert into Posts(CategoryId,Name) values(@CategoryId,@Name)"
            );
            cmd.Parameters.AddWithValue("@CategoryId", category.CategoryId);
            cmd.Parameters.AddWithValue("@Name", category.Name);

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                }
                catch
                {
                    return false;
                }
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch
                {
                    return false;
                }

            }
            return true;
        }

        public string GetCategoryName(int CategoryId)
        {
            string result = null;
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                MySqlCommand cmd = new MySqlCommand(
                    "select Name from Categories where CategoryId=@CategoryId"
                    , conn
                );
                cmd.Parameters.AddWithValue("@CategoryId", CategoryId);
                try
                {
                    conn.Open();
                }
                catch
                {
                    return result;
                }
                try
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result = reader["Name"].ToString();
                        }
                    }               
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                    return result;
                }

            }
            return result;
        }

        public IEnumerable<Category> GetAllCategories()
        {
            List<Category> result = new List<Category>();
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                MySqlCommand cmd = new MySqlCommand(
                    "select CategoryId,Name from Categories"
                    , conn
                );
                try
                {
                    conn.Open();
                }
                catch
                {
                    return result;
                }
                try
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result.Add(new Category()
                            {
                                CategoryId = int.Parse(reader["CategoryId"].ToString()),
                                Name = reader["Name"].ToString()       
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return result;
                }

            }
            return result;
        }
    }
}