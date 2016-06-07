using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VotacionesWebSite.Models
{
    public class Group
    {
        [Key]
        public int GroupId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(50, ErrorMessage = "The field {0} must contain maximum {1} and minimum {2} characteres", MinimumLength = 3)]
        public string Description { get; set; }

        //Relations
        public virtual ICollection<GroupMember> GroupMembers { get; set; }

        public virtual ICollection<VotingGroup> VotingGroups { get; set; }

    }
}
