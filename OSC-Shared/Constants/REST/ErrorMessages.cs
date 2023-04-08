namespace Shared.Constants;

public static class ErrorMessages
{
    public const string MalformedJson = "Malformed JSON.";

    public const string InvalidServerNameLength = "Invalid server name length.";
    public const string NotOwner = "You need to be the owner of this server to delete it.";
    public const string InvalidInvite = "Invalid server invite code.";
    public const string ServerDoesNotExist = "The specified server does not exist.";
    public const string NotAMember = "You are not a member of this server.";
    public const string AlreadyAMember = "You are already a member of this server.";

    public const string MissingFields = "Missing fields.";
    public const string InvalidEmail = "Invalid Email.";
    public const string EmailAlreadyExists = "Email is already registered.";
    public const string InvalidUserOrPass = "Invalid username or password.";
    
    public const string InvalidPasswordLength = "Invalid password length, password must be 6-27 characters long.";
    public const string InvalidPasswordCharacters = "Password must have atleast 1 upper-case letter and 1 special character in it.";

    public const string InvalidSession = "Session does not exist.";
}