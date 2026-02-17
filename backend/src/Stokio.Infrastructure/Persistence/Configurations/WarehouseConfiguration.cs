using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stokio.Domain.Entities;

namespace Stokio.Infrastructure.Persistence.Configurations;

public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(w => w.Address)
            .HasMaxLength(500);

        builder.Property(w => w.City)
            .HasMaxLength(100);

        builder.Property(w => w.Country)
            .HasMaxLength(100);

        builder.HasOne(w => w.Tenant)
            .WithMany(t => t.Warehouses)
            .HasForeignKey(w => w.TenantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
