namespace Shared.DatabaseObjects;

//ToDo: convert to DTo
public class Account
{
    public Guid Id { get; set; }
    public Guid? SessionId { get; set; }
    
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}