using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stokio.Domain.Entities;

namespace Stokio.Infrastructure.Persistence.Configurations;

public class TenantModuleConfiguration : IEntityTypeConfiguration<TenantModule>
{
    public void Configure(EntityTypeBuilder<TenantModule> builder)
    {
        builder.HasKey(tm => tm.Id);

        builder.HasIndex(tm => new { tm.TenantId, tm.ModuleId })
            .IsUnique();

        builder.HasOne(tm => tm.Tenant)
            .WithMany(t => t.TenantModules)
            .HasForeignKey(tm => tm.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tm => tm.Module)
            .WithMany(m => m.TenantModules)
            .HasForeignKey(tm => tm.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
