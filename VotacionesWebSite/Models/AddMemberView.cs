using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotacionesWebSite.Models
{
    public class AddMemberView
    {
        public int GroupId { get; set; }

        [Required(ErrorMessage = "You must select a user.")]
        public int UserId { get; set; }

    }
}
