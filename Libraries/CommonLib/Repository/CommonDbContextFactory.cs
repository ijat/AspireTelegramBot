using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.VisualBasic;

namespace CommonLib.Repository
{
    internal class CommonDbContextFactory : IDesignTimeDbContextFactory<CommonDbContext>
    {
        public CommonDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CommonDbContext>();
            optionsBuilder.UseSqlServer("Server=127.0.0.1,58080;Initial Catalog=TempDb;Integrated Security=False;Encrypt=False;TrustServerCertificate=False;User ID=sa;Password=hsjkHEJRK34@@!!");

            return new CommonDbContext(optionsBuilder.Options);
        }
    }
}
