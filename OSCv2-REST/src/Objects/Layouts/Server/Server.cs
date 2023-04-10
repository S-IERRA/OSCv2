using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OSCv2.Objects.Layouts;

namespace OSCv2.Logic.Database;

public class Server : IEntityTypeConfiguration<Server>
{
    public required Guid Id { get; set; }

    public required Guid OwnerId { get; set; }
    public Account Owner { get; set; }

    public required string Name { get; set; }

    public virtual ICollection<Role> Roles { get; set; } = new HashSet<Role>();
    public virtual ICollection<Member> Members { get; set; } = new HashSet<Member>();
    public virtual ICollection<Channel> Channels { get; set; } = new HashSet<Channel>();
    public virtual ICollection<Invite> InviteCodes { get; set; } = new HashSet<Invite>();
    public virtual ICollection<ServerSubscriber> Subscribers { get; set; } = new HashSet<ServerSubscriber>();

    public void Configure(EntityTypeBuilder<Server> builder)
    {
        builder.ToTable("Servers");
        
        builder.Property(e => e.Name).IsRequired();
        
        builder.HasOne(e => e.Owner)
            .WithMany()
            .HasForeignKey(e => e.OwnerId)
            .IsRequired();
    }
}