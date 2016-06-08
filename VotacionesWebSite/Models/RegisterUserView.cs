using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotacionesWebSite.Models
{
    public class RegisterUserView : UserView
    {
        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(500, ErrorMessage = "The field {0} must contain maximum {1} and minimum {2} characteres", MinimumLength = 6)]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(500, ErrorMessage = "The field {0} must contain maximum {1} and minimum {2} characteres", MinimumLength = 6)]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage = "Password and confirm does not match.")]
        public string ConfirmPassword { get; set; }

    }
}
