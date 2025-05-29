﻿using System.ComponentModel.DataAnnotations;

namespace UI.Web.Models.Account
{
    public class LoginViewModel
    {
        [Required, Display(Name = "UserName or Email Address", Prompt = "UserName or Email")]
        public required string UserNameorEmail { get; set; }

        [Required, Display(Name = "Password", Prompt = "Password"), DataType(DataType.Password)]
        public required string Password { get; set; }

        [Display(Name= "Remember Me", Prompt = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
