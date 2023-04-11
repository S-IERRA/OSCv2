namespace Shared.Layouts;

public class Account 
{
    public Guid Id { get; set; }
    public Guid? SessionId { get; set; }
    
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string HashedPassword { get; set; }
}