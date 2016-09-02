using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotacionesWebSite.Models
{
    [NotMapped]
    public class UserIndexView : User
    {
        [Display(Name = "Is Admin?")]
        public bool IsAdmin { get; set; }
    }
}
