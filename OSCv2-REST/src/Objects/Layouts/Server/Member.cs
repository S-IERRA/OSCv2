using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OSCv2.Objects.Layouts;

namespace OSCv2.Logic.Database;

public class Member : IEntityTypeConfiguration<Member>
{
    public required Guid UserId { get; set; }
    public Account User { get; set; }

    public required Guid ServerId { get; set; }
    public Server Server { get; set; }

    public Permissions Permissions { get; set; }

    public virtual ICollection<Role> Roles { get; set; } = new HashSet<Role>();
    
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("Members");

        builder.HasKey(x => new { x.UserId, x.ServerId });
        
        builder.HasOne(x => x.Server)
            .WithMany(x => x.Members)
            .HasForeignKey(x => x.ServerId);
    }
}