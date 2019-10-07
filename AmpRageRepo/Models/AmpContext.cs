using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace AmpRageRepo.Models
{
    public partial class AmpContext : DbContext
    {

        //public AmpContext()
        //{
        //}

        //public AmpContext(DbContextOptions<AmpContext> options) : base(options)
        //{
        //    var x = options;
        //}
        public AmpContext(IConfiguration configuration)
        {
            dbConnectionString = configuration.GetConnectionString("DbLocal");
        }

        private string dbConnectionString;

        public virtual DbSet<Car> Cars { get; set; }
        //public virtual DbSet<RootObject> RootObjects { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserCar> UserCars { get; set; }
        public virtual DbSet<CountryEmission> CountryEmissions { get; set; }
        public virtual DbSet<Key> Keys { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //optionsBuilder.UseSqlServer("Server=tcp:amprage.database.windows.net,1433;Initial Catalog=AmpRageDB;Persist Security Info=False;User ID=Shadowacademy;Password=PatrikWiksten2019;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
                optionsBuilder.UseSqlServer(dbConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Car>().Property(a => a.Id).ValueGeneratedNever();
            //modelBuilder.Entity<UserCar>().Property(a => a.Id).ValueGeneratedNever();
            //modelBuilder.Entity<UserCar>().Property(a => a.UserId).ValueGeneratedNever();
            //modelBuilder.Entity<UserCar>().Property(a => a.CarId).ValueGeneratedNever();
        }
    }
}
