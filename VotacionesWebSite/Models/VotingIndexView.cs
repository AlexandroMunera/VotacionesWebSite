using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace VotacionesWebSite.Models
{
    [NotMapped]
    public class VotingIndexView : Voting
    {
        public User Winner { get; set; }
    }
}