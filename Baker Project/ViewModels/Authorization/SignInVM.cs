using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Baker_Project.ViewModels.Authorization
{
    public class SignInVM
    {
        [Required]
        public string UsernameorEmail { get; set; } 
        [Required,DataType(DataType.Password)]
        public string Password { get; set; }
        public bool Remember { get; set; }
    }
}
