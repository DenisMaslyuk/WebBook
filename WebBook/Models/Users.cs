using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebBook.Models
{
    public partial class Users
    {
        public Users()
        {
            History = new HashSet<History>();
        }

        public int Id { get; set; }
        [Display(Name = "Пароль")]
        public string Password { get; set; }
        [Display(Name = "Имя")]
        public string Email { get; set; }
        [Display(Name = "Email")]
        public string Role { get; set; }

        public ICollection<History> History { get; set; }
    }
}
