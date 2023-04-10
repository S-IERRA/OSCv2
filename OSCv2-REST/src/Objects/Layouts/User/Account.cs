using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OSCv2.Objects.Layouts;

public class Account : IEntityTypeConfiguration<Account>
{
    public Guid Id { get; set; }
    public Guid? SessionId { get; set; }
    
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Users");

        builder.Property(x => x.Username).IsRequired();
        builder.Property(x => x.Password).IsRequired();
        builder.Property(x => x.Email).IsRequired();
    }
}