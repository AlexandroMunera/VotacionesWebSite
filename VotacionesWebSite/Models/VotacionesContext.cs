﻿using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace VotacionesWebSite.Models
{
    public class VotacionesContext : DbContext
    {
        public VotacionesContext()
            : base("DefaultConnection")
        {

        }

        // this method is used to "disable" the cascade deleting mode
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

        public DbSet<State> States { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<Voting> Votings { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<GroupMember> GroupMembers { get; set; }

    }
}
