using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MyBlog.Models
{
    public class AccountLogin
    {
        [Required]
        [Display(Name = "用户名")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Display(Name = "验证码", Description = "验证码 字段是必需的。")]
        [Required(ErrorMessage = "验证码 输入错误。")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "验证码 输入错误。")]
        public string VerificationCode { get; set; }
    }
}