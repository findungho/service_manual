using Microsoft.EntityFrameworkCore;

namespace ServiceManual.ApplicationCore.Entities
{
    public class FactoryDeviceContext : DbContext
    {
        public DbSet<FactoryDevice> FactoryDevices { get; set; }
        public FactoryDeviceContext(DbContextOptions<FactoryDeviceContext> options)
            : base(options)
        {

        }
    }
}
