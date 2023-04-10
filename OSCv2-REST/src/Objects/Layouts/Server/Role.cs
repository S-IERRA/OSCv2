using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OSCv2.Logic.Database;

public class Role : IEntityTypeConfiguration<Role>
{
    public required Guid Id { get; set; }

    public required uint HexColour { get; set; }

    public required Guid ServerId { get; set; }
    public Server Server { get; set; }
    
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");
        
        builder.HasOne(e => e.Server)
            .WithMany(e=>e.Roles)
            .HasForeignKey(e => e.ServerId);
    }
}