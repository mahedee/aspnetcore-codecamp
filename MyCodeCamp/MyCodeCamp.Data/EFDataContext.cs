using Microsoft.EntityFrameworkCore;
using MyCodeCamp.Data.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCodeCamp.Data
{
    public class EFDataContext : DbContext
    {
        public DbSet<Department> Departments { get; set; }

        public DbSet<Designation> Designations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MyCodeCampDb;Trusted_Connection=True;MultipleActiveResultSets=true");
        }
    }
}
