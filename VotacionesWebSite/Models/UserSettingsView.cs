using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace VotacionesWebSite.Models
{
    public class UserSettingsView : User
    {
        [NotMapped]
        [Display(Name = "New photo")]
        public HttpPostedFileBase NewPhoto { get; set; }

    }
}
