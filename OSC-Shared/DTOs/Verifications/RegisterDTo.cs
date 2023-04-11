using Mapster;
using Shared.Layouts;

namespace Shared.DTOs.Verifications;

[AdaptTo(typeof(Account)), GenerateMapper]
public class RegisterDTo
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public string HashedPassword { get; set; }
    
    public static explicit operator Account(RegisterDTo user)
        => user.Adapt<Account>();
}