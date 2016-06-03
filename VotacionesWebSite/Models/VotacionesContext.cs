using System.Data.Entity;

namespace VotacionesWebSite.Models
{
    public class VotacionesContext : DbContext
    {
        public VotacionesContext()
            : base("DefaultConnection")
        {

        }

        public DbSet<State> States { get; set; }

        public System.Data.Entity.DbSet<VotacionesWebSite.Models.Group> Groups { get; set; }
    }
}
