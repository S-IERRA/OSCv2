using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OSCv2.Logic.Database;

namespace OSCv2.Objects.Layouts;

public class Channel : IEntityTypeConfiguration<Channel>
{
    public required Guid Id { get; set; }

    public required Guid ServerId { get; set; }
    public Server Server { get; set; }

    public required string Name { get; set; }

    public Permissions ViewPermissions { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new HashSet<Message>();
    
    public void Configure(EntityTypeBuilder<Channel> builder)
    {
        builder.ToTable("Channels");

        builder.HasKey(x => new { x.Id, x.ServerId });

        builder.HasMany(x => x.Messages)
            .WithOne(x => x.Channel)
            .HasForeignKey(x => new { x.ChannelId, x.ServerId });
    }
}