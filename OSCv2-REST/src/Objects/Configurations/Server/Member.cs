using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Layouts;

namespace OSCv2.Logic.Database;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("Members");

        builder.HasKey(x => new { x.UserId, x.ServerId });
        
        builder.HasOne(x => x.Server)
            .WithMany(x => x.Members)
            .HasForeignKey(x => x.ServerId);
    }
}