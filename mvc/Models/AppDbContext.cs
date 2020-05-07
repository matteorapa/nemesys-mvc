using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<User>().Property(u => u.IsInvestigator).HasDefaultValue(0);
        }

        public DbSet<Report> Reports { get; set; }
        public DbSet<Investigation> Investigations { get; set; }
        public DbSet<Upvote> Upvotes { get; set; }
    }
}


