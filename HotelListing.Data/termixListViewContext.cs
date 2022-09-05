using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using TermixListing.API.Data.Configurations;

namespace TermixListing.API.Data
{
    public class termixListViewContext : IdentityDbContext<ApiUser>
    {
        public termixListViewContext(DbContextOptions options) : base(options)
        {
                
        }

        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Country> Countries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new CountryConfiguration());
            modelBuilder.ApplyConfiguration(new HotelConfiguration());
        }  
    }
    public class termixListViewContextFactory : IDesignTimeDbContextFactory<termixListViewContext>
    {
        public termixListViewContext CreateDbContext(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            var optionsBuilder = new DbContextOptionsBuilder<termixListViewContext>();
            var con = config.GetConnectionString("termixListView");
            optionsBuilder.UseSqlServer(con);
            return new termixListViewContext(optionsBuilder.Options);
        }
    }
}
