using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OSCv2.Logic.Database;

public class Invite : IEntityTypeConfiguration<Invite>
{
    public required Guid Id { get; set; }

    public required Guid ServerId { get; set; }
    public required string InviteCode { get; set; }
    
    public void Configure(EntityTypeBuilder<Invite> builder)
    {
        builder.ToTable("Invites");

        builder.HasIndex(e => e.InviteCode);
        builder.HasKey(e => new {e.ServerId, e.InviteCode});

        builder.Property(e => e.InviteCode).HasMaxLength(25);
    }
}