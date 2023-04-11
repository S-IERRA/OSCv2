namespace Shared.Layouts;

public class Message
{
    public required Guid Id { get; set; }
    public string Discriminator { get; set; }

    public required Guid AuthorId { get; set; }
    public Member Author { get; set; }

    public required Guid ServerId { get; set; }
    public Server Server { get; set; }

    public required Guid ChannelId { get; set; }
    public Channel Channel { get; set; }

    public DateTimeOffset Created { get; set; }

    public string Content { get; set; }
}