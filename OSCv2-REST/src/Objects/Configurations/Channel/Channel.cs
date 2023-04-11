using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Layouts;

namespace OSCv2.Objects.Layouts;

public class ChannelConfiguration : IEntityTypeConfiguration<Channel>
{
    public void Configure(EntityTypeBuilder<Channel> builder)
    {
        builder.ToTable("Channels");

        builder.HasKey(x => new { x.Id, x.ServerId });

        builder.HasMany(x => x.Messages)
            .WithOne(x => x.Channel)
            .HasForeignKey(x => new { x.ChannelId, x.ServerId });
    }
}