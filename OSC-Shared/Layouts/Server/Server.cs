namespace Shared.Layouts;

public class Server
{
    public required Guid Id { get; set; }

    public required Guid OwnerId { get; set; }
    public Account Owner { get; set; }

    public required string Name { get; set; }

    public virtual ICollection<Role> Roles { get; set; } = new HashSet<Role>();
    public virtual ICollection<Member> Members { get; set; } = new HashSet<Member>();
    public virtual ICollection<Channel> Channels { get; set; } = new HashSet<Channel>();
    public virtual ICollection<Invite> InviteCodes { get; set; } = new HashSet<Invite>();
    public virtual ICollection<ServerSubscriber> Subscribers { get; set; } = new HashSet<ServerSubscriber>();
}