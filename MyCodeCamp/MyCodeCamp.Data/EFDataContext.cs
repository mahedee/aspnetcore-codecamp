using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyCodeCamp.Data.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCodeCamp.Data
{
    //public class EFDataContext : DbContext
    public class EFDataContext : IdentityDbContext
    {
        //private IConfigurationRoot _config;

        //public EFDataContext(DbContextOptions options, IConfigurationRoot config)
        //  : base(options)
        //{
        //    _config = config;
        //}

        public DbSet<Department> Departments { get; set; }

        public DbSet<Designation> Designations { get; set; }


        public DbSet<Camp> Camps { get; set; }
        public DbSet<Speaker> Speakers { get; set; }
        public DbSet<Talk> Talks { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MyCodeCampDb;Trusted_Connection=True;MultipleActiveResultSets=true");
        //}

        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);

        //    builder.Entity<Camp>()
        //      .Property(c => c.Moniker)
        //      .IsRequired();
        //    builder.Entity<Camp>()
        //      .Property(c => c.RowVersion)
        //      .ValueGeneratedOnAddOrUpdate()
        //      .IsConcurrencyToken();
        //    builder.Entity<Speaker>()
        //      .Property(c => c.RowVersion)
        //      .ValueGeneratedOnAddOrUpdate()
        //      .IsConcurrencyToken();
        //    builder.Entity<Talk>()
        //      .Property(c => c.RowVersion)
        //      .ValueGeneratedOnAddOrUpdate()
        //      .IsConcurrencyToken();
        //}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //base.OnConfiguring(optionsBuilder);

            //optionsBuilder.UseSqlServer(_config["ConnectionStrings:DefaultConnection"]);

            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MyCodeCampDb;Trusted_Connection=True;MultipleActiveResultSets=true");

        }
    }
}
