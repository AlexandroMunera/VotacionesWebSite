using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotacionesWebSite.Models
{
    public class State
    {
        [Key]
        public int StateId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(50,ErrorMessage = "The field {0} must contain maximum {1} and minimum {2} characteres", MinimumLength = 3)]
        [Display(Name = " State description")]
        public string Description { get; set; }

        //Relations
        public virtual ICollection<Voting> Votings { get; set; }
    }
}
