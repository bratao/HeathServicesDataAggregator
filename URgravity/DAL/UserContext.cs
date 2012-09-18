using System;
using System.Collections.Generic;
using System.Data.Entity;
using URgravity.Models;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace URgravity.Models
{
    public class UserContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}