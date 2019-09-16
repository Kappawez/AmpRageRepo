using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;


namespace AmpRageRepo.Models
{
    public partial class AmpContext : DbContext
    {

        public AmpContext()
        {
        }

        public AmpContext(DbContextOptions<AmpContext> options)
    : base(options)
        {
        }

        public virtual DbSet<Car> Cars { get; set; }
        public virtual DbSet<RootObject> RootObjects { get; set; }
        public virtual DbSet<SearchedCar> SearchedCars { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=tcp:amprage.database.windows.net,1433;Initial Catalog=AmpRageDB;Persist Security Info=False;User ID=Shadowacademy;Password=PatrikWiksten2019;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            }
        }



    }
}
