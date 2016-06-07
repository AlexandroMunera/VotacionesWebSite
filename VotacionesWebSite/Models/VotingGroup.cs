using System.ComponentModel.DataAnnotations;

namespace VotacionesWebSite.Models
{
    public class VotingGroup
    {
        [Key]
        public int VotingGroupId { get; set; }

        public int VotingId { get; set; }

        public int GroupId { get; set; }

        //Relations
        public virtual Voting Voting { get; set; }

        public virtual Group Group { get; set; }


    }
}
