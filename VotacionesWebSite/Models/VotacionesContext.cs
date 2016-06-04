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

        public DbSet<Group> Groups { get; set; }

        public DbSet<Voting> Votings { get; set; }

        public DbSet<User> Users { get; set; }
    }
}
