using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Layouts;

namespace OSCv2.Objects.Layouts;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Users");

        builder.Property(x => x.Username).IsRequired();
        builder.Property(x => x.HashedPassword).IsRequired();
        builder.Property(x => x.Email).IsRequired();
    }
}