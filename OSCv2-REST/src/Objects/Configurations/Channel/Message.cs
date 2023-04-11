using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Layouts;

namespace OSCv2.Objects.Layouts;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.AuthorId).IsRequired();
        builder.HasOne(e => e.Author)
            .WithMany()
            .HasForeignKey(e => new { e.AuthorId, e.ServerId });

        //Todo: Critical, doesnt allow dms
        builder.Property(e => e.ServerId).IsRequired();
        builder.HasOne(e => e.Server)
            .WithMany()
            .HasForeignKey(e => e.ServerId);

        builder.Property(e => e.ChannelId).IsRequired();
        builder.HasOne(e => e.Channel)
            .WithMany(e => e.Messages)
            .HasForeignKey(e => new { e.ChannelId, e.ServerId });

        builder.Property(e => e.Content)
            .IsRequired();
    }
}