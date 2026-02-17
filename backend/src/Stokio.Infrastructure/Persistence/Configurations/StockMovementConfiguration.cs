using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stokio.Domain.Entities;

namespace Stokio.Infrastructure.Persistence.Configurations;

public class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.HasKey(sm => sm.Id);

        builder.Property(sm => sm.MovementType)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(sm => sm.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(sm => sm.Notes)
            .HasMaxLength(1000);

        builder.HasOne(sm => sm.Tenant)
            .WithMany()
            .HasForeignKey(sm => sm.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sm => sm.Product)
            .WithMany(p => p.StockMovements)
            .HasForeignKey(sm => sm.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sm => sm.Warehouse)
            .WithMany(w => w.StockMovements)
            .HasForeignKey(sm => sm.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sm => sm.RelatedWarehouse)
            .WithMany()
            .HasForeignKey(sm => sm.RelatedWarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
