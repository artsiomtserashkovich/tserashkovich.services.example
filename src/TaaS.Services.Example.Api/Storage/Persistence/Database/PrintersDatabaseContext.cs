using Microsoft.EntityFrameworkCore;
using TaaS.Services.Example.Api.Storage.Persistence.Model;

namespace TaaS.Services.Example.Api.Storage.Persistence.Database
{
    public class PrintersDatabaseContext : DbContext
    {
        public PrintersDatabaseContext(DbContextOptions<PrintersDatabaseContext> options) : base(options) {}

        public DbSet<Printer> Printers { get; private set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(PrintersDatabaseContext).Assembly);
        }
    }
}