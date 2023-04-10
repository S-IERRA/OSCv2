using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OSCv2.Logic.Database;

public class ServerSubscriber : IEntityTypeConfiguration<ServerSubscriber>
{
    public Guid Id { get; set; }

    public Server Server { get; set; }
    public Guid ServerId { get; set; }

    public required byte[] AddressBytes { get; set; }
    public required int Port { get; set; }

    public void Configure(EntityTypeBuilder<ServerSubscriber> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasOne(e => e.Server)
            .WithMany(e => e.Subscribers)
            .HasForeignKey(e => e.ServerId);
    }
}