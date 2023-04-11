using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Layouts;

namespace OSCv2.Logic.Database;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");
        
        builder.HasOne(e => e.Server)
            .WithMany(e=>e.Roles)
            .HasForeignKey(e => e.ServerId);
    }
}