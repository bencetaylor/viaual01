using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LunchTrain.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        //todo extend DB based on instructions here https://docs.microsoft.com/en-us/ef/core/get-started/aspnetcore/new-db

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public DbSet<Group> Groups { get; set; }

        public DbSet<GroupApplication> GroupApplications { get; set; }

        public DbSet<GroupMemberFlag> GroupMemberFlags { get; set; }

        public DbSet<GroupMembership> GroupMemberships { get; set; }
    }
}
