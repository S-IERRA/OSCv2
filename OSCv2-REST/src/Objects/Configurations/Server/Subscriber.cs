using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Layouts;

namespace OSCv2.Logic.Database;

public class ServerSubscriberConfiguration : IEntityTypeConfiguration<ServerSubscriber>
{
    public void Configure(EntityTypeBuilder<ServerSubscriber> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasOne(e => e.Server)
            .WithMany(e => e.Subscribers)
            .HasForeignKey(e => e.ServerId);
    }
}