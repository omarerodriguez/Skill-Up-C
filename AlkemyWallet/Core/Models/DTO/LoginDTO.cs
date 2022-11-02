﻿using System.ComponentModel.DataAnnotations;

namespace AlkemyWallet.Core.Models.DTO
{
    public class LoginDTO
    {
        [Key]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
