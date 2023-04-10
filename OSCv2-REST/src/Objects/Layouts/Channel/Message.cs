using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OSCv2.Logic.Database;

namespace OSCv2.Objects.Layouts;

public class Message : IEntityTypeConfiguration<Message>
{
    public required Guid Id { get; set; }
    public string Discriminator { get; set; }

    public required Guid AuthorId { get; set; }
    public Member Author { get; set; }

    public required Guid ServerId { get; set; }
    public Server Server { get; set; }

    public required Guid ChannelId { get; set; }
    public Channel Channel { get; set; }

    public DateTimeOffset Created { get; set; }

    public string Content { get; set; }
    
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