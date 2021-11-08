using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TaaS.Services.Example.Api.Storage.Persistence.Model;

namespace TaaS.Services.Example.Api.Storage.Persistence.Database
{
    public class PrintersDatabaseConfiguration : IEntityTypeConfiguration<Printer>
    {
        public void Configure(EntityTypeBuilder<Printer> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();
            
            builder.Property(x => x.OwnerEmail)
                .IsRequired();
            
            builder.Property(x => x.Type)
                .IsRequired()
                .HasConversion(new EnumToNumberConverter<PrinterType, short>());

            builder.Property(x => x.XSize)
                .IsRequired();
            
            builder.Property(x => x.YSize)
                .IsRequired();
            
            builder.Property(x => x.ZSize)
                .IsRequired();
        }
    }
}