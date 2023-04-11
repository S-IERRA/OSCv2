namespace Shared.Layouts;

public class Member 
{
    public required Guid UserId { get; set; }
    public Account User { get; set; }

    public required Guid ServerId { get; set; }
    public Server Server { get; set; }

    public Permissions Permissions { get; set; }

    public virtual ICollection<Role> Roles { get; set; } = new HashSet<Role>();
}