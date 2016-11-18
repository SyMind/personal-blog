using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;

namespace MyBlog.Models
{
    public class IpControl
    {
        public int Id { get; set; }

        [Display(Name = "IP")]
        public string IpAddress { get; set; }

        [Display(Name = "请求的来源")]
        public string IpSource { get; set; }

        [Display(Name = "访问的页面")]
        public string PageUrl { get; set; }

        [Display(Name = "请求时间")]
        public DateTime RequestDateTime { get; set; }
    }

    public class IpControlRepository
    {
        private string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["PostDb"].ConnectionString;

        public bool TryInsertIpControl(IpControl ipControl)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                MySqlCommand cmd = new MySqlCommand("insert into IpControls(IpAddress,IpSource,PageUrl,RequestDateTime) values(@IpAddress,@IpSource,@PageUrl,@RequestDateTime)", conn);
                cmd.Parameters.AddWithValue("@IpAddress", ipControl.IpAddress);
                cmd.Parameters.AddWithValue("@IpSource", ipControl.IpSource);
                cmd.Parameters.AddWithValue("@PageUrl", ipControl.PageUrl);
                cmd.Parameters.AddWithValue("@RequestDateTime", ipControl.RequestDateTime);
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

        public IEnumerable<IpControl> GetAllIpControl(int startIndex = 0, int? length = null)
        {
            var result = new List<IpControl>();
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                MySqlCommand cmd = new MySqlCommand("select * from IpControls order by RequestDateTime desc", conn);
                
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
                            if (startIndex == 0)
                            {
                                if (length == null)
                                {
                                    result.Add(new IpControl()
                                    {
                                        IpAddress = reader["IpAddress"].ToString(),
                                        IpSource = reader["IpSource"].ToString(),
                                        PageUrl = reader["PageUrl"].ToString(),
                                        RequestDateTime = DateTime.Parse(reader["RequestDateTime"].ToString())
                                    });
                                }
                                else
                                {
                                    if (length > 0)
                                    {
                                        length--;
                                        result.Add(new IpControl()
                                        {
                                            IpAddress = reader["IpAddress"].ToString(),
                                            IpSource = reader["IpSource"].ToString(),
                                            PageUrl = reader["PageUrl"].ToString(),
                                            RequestDateTime = DateTime.Parse(reader["RequestDateTime"].ToString())
                                        });
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                startIndex--;
                            }
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

        public int GetIpControlNumber()
        {
            int result = -1;
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                MySqlCommand cmd = new MySqlCommand("select * from IpControls order by RequestDateTime desc", conn);

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
                    return result;
                }
            }
            return result;
        }
    }
}