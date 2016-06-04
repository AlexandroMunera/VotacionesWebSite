﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VotacionesWebSite.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(100, ErrorMessage = "The field {0} must contain maximum {1} and minimum {2} characteres", MinimumLength = 7)]
        [Display(Name = "E-Mail")]
        [DataType(DataType.EmailAddress)]
        [Index("UserNameIndex", IsUnique = true)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(50, ErrorMessage = "The field {0} must contain maximum {1} and minimum {2} characteres", MinimumLength = 2)]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(50, ErrorMessage = "The field {0} must contain maximum {1} and minimum {2} characteres", MinimumLength = 2)]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Display(Name = "User")]
        public string FullName { get { return string.Format("{0} {1}",this.FirstName,this.LastName); } }

        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(20, ErrorMessage = "The field {0} must contain maximum {1} and minimum {2} characteres", MinimumLength = 7)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(100, ErrorMessage = "The field {0} must contain maximum {1} and minimum {2} characteres", MinimumLength = 10)]
        public string Address { get; set; }

        public string Grade { get; set; }

        public string Group { get; set; }

        [DataType(DataType.ImageUrl)]
        public string Photo { get; set; }

        //Relations
        public virtual ICollection<GroupMember> GroupMembers { get; set; }


    }
}