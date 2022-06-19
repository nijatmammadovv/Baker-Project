using Baker_Project.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Baker_Project.Data_Access_Layer
{
    public class AppDbContext:IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions options):base(options)
        {

        }
        public DbSet<HomePageProducts> HomePageProducts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Design> Designs { get; set; }
    }
}
