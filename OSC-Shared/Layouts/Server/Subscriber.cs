namespace Shared.Layouts;

public class ServerSubscriber
{
    public Guid Id { get; set; }

    public Server Server { get; set; }
    public Guid ServerId { get; set; }

    public required byte[] AddressBytes { get; set; }
    public required int Port { get; set; }
}