namespace Shared.Layouts;

public class Channel
{
    public required Guid Id { get; set; }

    public required Guid ServerId { get; set; }
    public Server Server { get; set; }

    public required string Name { get; set; }

    public Permissions ViewPermissions { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new HashSet<Message>();
}