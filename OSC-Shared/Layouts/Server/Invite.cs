namespace Shared.Layouts;

public class Invite
{
    public required Guid Id { get; set; }

    public required Guid ServerId { get; set; }
    public required string InviteCode { get; set; }
}