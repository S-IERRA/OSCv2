using System.Text.RegularExpressions;
using FluentValidation;
using Shared.Constants;
using Shared.DTOs.Verifications;
using Shared.Layouts;

namespace OSCv2.Logic.Validators;

public class CreateAccountValidator : AbstractValidator<RegisterDTo>
{
    private static readonly Regex UsernameRegex =
        new Regex("""^[a-zA-Z0-9_-]""", RegexOptions.Compiled | RegexOptions.NonBacktracking);

    private static readonly Regex PasswordRegex =
        new Regex("""^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*#?&])[a-zA-Z\d@$!%*#?&]""", RegexOptions.Compiled);
    
    public CreateAccountValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .Length(3, 24)
            .WithMessage(ErrorMessages.InvalidUsernameLength)
            .Matches(UsernameRegex)
            .WithMessage(ErrorMessages.InvalidUsernameCharacters);
        
        RuleFor(x => x.Email)
            .EmailAddress()
            .Length(0, 254)
            .WithMessage(ErrorMessages.InvalidEmail);

        RuleFor(x => x.Password)
            .NotEmpty()
            .Length(8, 27)
            .WithMessage(ErrorMessages.InvalidPasswordLength)
            .Matches(PasswordRegex)
            .WithMessage(ErrorMessages.InvalidPasswordCharacters); 
    }
}