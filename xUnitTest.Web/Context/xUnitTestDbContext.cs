using Microsoft.EntityFrameworkCore;
using xUnitTest.Web.Entities;

namespace xUnitTest.Web.Context
{
    public class xUnitTestDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=.; Initial Catalog=xUnitTestDb; Integrated Security=true;TrustServerCertificate = true;");

        }

        public DbSet<Product> Products { get; set; }
    }
}