namespace Shared.DTOs;

//Convert to DTo with mapster
public class AccountDto
{
    public Guid Id { get; set; }
    public Guid? SessionId { get; set; }
    
    public required string Username { get; set; }
    public required string Email { get; set; }
}