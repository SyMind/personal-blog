using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;

namespace MyBlog.Models
{
    public class Account
    {
        [Key]
        [Display(Name = "账户名")]
        public string Name { get; set; }

        [Display(Name = "密码")]
        public string Password { get; set; }

        [Display(Name = "个人描述")]
        public string Description { get; set; }

        [Display(Name = "昵称")]
        public string DisplayName { get; set; }

        [Display(Name = "邮箱")]
        public string Email { get; set; }

        [Display(Name = "GitHub")]
        public string GitHub { get; set; }

        [Display(Name = "微博")]
        public string WeiBo { get; set; }
    }

    public class AccountRepository
    {
        // ValidateAccount()方法返回的结果集
        public enum ValidateResult
        {
            HaveNotAccount,
            WarryPassword,
            Success,
            ConnectionError,
            CommandError
        }

        // 字符串转MD5编码
        public string Str2MD5(string inputStr)
        {
            byte[] inputBytes = System.Text.Encoding.Unicode.GetBytes(inputStr);
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5CryptoServiceProvider.Create();
            byte[] outputBytes = md5.ComputeHash(inputBytes);
            return BitConverter.ToString(outputBytes).Replace("-", "");
        }

        private string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["PostDb"].ConnectionString;

        public bool TryInsertAccount(Account account)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                MySqlCommand cmd = new MySqlCommand(
                    "insert into Accounts(Name,Password) values(@Name,@Password)"
                    , conn
                );
                cmd.Parameters.AddWithValue("@Name", account.Name);
                cmd.Parameters.AddWithValue("@Password", Str2MD5(account.Password));
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

        public Account GetAccountByName(string name)
        {
            Account account = null;
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                MySqlCommand cmd = new MySqlCommand(
                    "select * from Accounts where Name=@Name"
                    , conn
                );
                cmd.Parameters.AddWithValue("@Name", name);
                try
                {
                    conn.Open();
                }
                catch
                {
                    return account;
                }
                try
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if(reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            account = new Models.Account()
                            {
                                Name = reader["Name"].ToString(),
                                Password = reader["Password"].ToString(),
                                DisplayName = reader["DisplayName"].ToString(),
                                Description = reader["Description"].ToString(),
                                Email = reader["Email"].ToString(),
                                GitHub = reader["GitHub"].ToString(),
                                WeiBo = reader["WeiBo"].ToString()
                            };
                        }
                    }
                    else
                    {
                        return account;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return account;
                }
            }
            return account;
        }

        public ValidateResult ValidateAccount(string name, string passwordMD5)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                MySqlCommand cmd = new MySqlCommand(
                    "select Name, Password from Accounts where Name=@Name"
                    , conn
                );
                cmd.Parameters.AddWithValue("@Name", name);
                try
                {
                    conn.Open();
                }
                catch
                {
                    return ValidateResult.ConnectionError;
                }
                try
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while(reader.Read())
                        {
                            if(reader["Password"].ToString() != passwordMD5)
                            {
                                return ValidateResult.WarryPassword;
                            }
                        }
                    }
                    else
                    {
                        return ValidateResult.HaveNotAccount;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return ValidateResult.CommandError;
                }

            }
            return ValidateResult.Success;
        }

        public bool TryUpdateAccoount(Account account)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                MySqlCommand cmd = new MySqlCommand(
                    "update Accounts set DisplayName=@DisplayName,Description=@Description,Email=@Email,GitHub=@GitHub,WeiBo=@WeiBo where Name=@Name"
                    , conn
                );
                cmd.Parameters.AddWithValue("@Name", account.Name);
                cmd.Parameters.AddWithValue("@DisplayName", account.DisplayName);
                cmd.Parameters.AddWithValue("@Description", account.Description);
                cmd.Parameters.AddWithValue("@Email", account.Email);
                cmd.Parameters.AddWithValue("@GitHub", account.GitHub);
                cmd.Parameters.AddWithValue("@WeiBo", account.WeiBo);
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
    }
}