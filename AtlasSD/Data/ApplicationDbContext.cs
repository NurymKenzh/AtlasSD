using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AtlasSD.Models;

namespace AtlasSD.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
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

        public DbSet<AtlasSD.Models.Bloc> Bloc { get; set; }

        public DbSet<AtlasSD.Models.ApplicationUser> ApplicationUser { get; set; }

        public DbSet<AtlasSD.Models.Group> Group { get; set; }

        public DbSet<AtlasSD.Models.Indicator> Indicator { get; set; }

        public DbSet<AtlasSD.Models.Region> Region { get; set; }

        public DbSet<AtlasSD.Models.IndicatorValue> IndicatorValue { get; set; }

        public DbSet<AtlasSD.Models.Map> Map { get; set; }

        public DbSet<AtlasSD.Models.ReferencePoint> ReferencePoint { get; set; }

        public DbSet<AtlasSD.Models.Source> Source { get; set; }

        public DbSet<AtlasSD.Models.DollarRate> DollarRate { get; set; }

        public DbSet<AtlasSD.Models.Log> Log { get; set; }

        public DbSet<AtlasSD.Models.Feedback> Feedback { get; set; }
    }
}
