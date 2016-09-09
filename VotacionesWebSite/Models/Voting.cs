using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VotacionesWebSite.Models
{
    public class Voting
    {
        [Key]
        public int VotingId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(50, ErrorMessage = "The field {0} must contain maximum {1} and minimum {2} characteres", MinimumLength = 3)]
        [Display(Name = " Voting description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Display(Name = "State")]
        public int StateId { get; set; }

        [DataType(DataType.MultilineText)]
        public string Remarks { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Display(Name = "Date start")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}",ApplyFormatInEditMode = true)]
        public DateTime DateTimeStart { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Display(Name = "Date end")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime DateTimeEnd { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Display(Name = "Is for all users?")]
        public bool IsForAllUsers { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Display(Name = "Is enable blank vote?")]
        public bool IsEnableBlankVote { get; set; }

        [Display(Name = "Quantity votes")]
        public int QuantityVotes { get; set; }

        [Display(Name = "Quantity blank votes")]
        public int QuantityBlankVotes { get; set; }

        [Display(Name = "Winner")]
        public int CandidateWinId { get; set; }

        //Relations
        public virtual State State { get; set; }

        public virtual ICollection<VotingGroup> VotingGroups { get; set; }

        public virtual ICollection<Candidate> Candidates { get; set; }

        public virtual ICollection<VotingDetail> VotingDetails { get; set; }


    }
}
