using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace prova.data
{
    public class ProvaDbContextFactory : IDesignTimeDbContextFactory<ProvaDbContext>
    {
        public ProvaDbContext CreateDbContext(String[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ProvaDbContext>();

            //Build
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 26)));

            return new ProvaDbContext(optionsBuilder.Options);

        }
    }
}