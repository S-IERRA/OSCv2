namespace Shared.Layouts;

public class Role
{
    public required Guid Id { get; set; }

    public required uint HexColour { get; set; }

    public required Guid ServerId { get; set; }
    public Server Server { get; set; }
}