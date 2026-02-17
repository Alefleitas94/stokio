using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stokio.Domain.Entities;

namespace Stokio.Infrastructure.Persistence.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles");

        builder.HasKey(ur => new { ur.TenantId, ur.UserId, ur.RoleId });

        builder.HasIndex(ur => new { ur.TenantId, ur.UserId });

        builder.HasOne(ur => ur.Tenant)
            .WithMany(t => t.UserRoles)
            .HasForeignKey(ur => ur.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => new { ur.TenantId, ur.UserId })
            .HasPrincipalKey(u => new { u.TenantId, u.Id })
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => new { ur.TenantId, ur.RoleId })
            .HasPrincipalKey(r => new { r.TenantId, r.Id })
            .OnDelete(DeleteBehavior.Cascade);
    }
}
