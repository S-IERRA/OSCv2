using System.Text.RegularExpressions;
using FluentValidation;
using OSCv2.Objects.Layouts;
using Shared.Constants;

namespace OSCv2.Logic.Validators;

public class CreateAccountValidator : AbstractValidator<Account>
{
    public CreateAccountValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .Length(3, 24)
            .WithMessage("");
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .Length(8, 27)
            .WithMessage(ErrorMessages.InvalidPasswordLength)
            .Matches(new Regex("""^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*#?&])[a-zA-Z\d@$!%*#?&]{8,}$""", RegexOptions.Compiled | RegexOptions.NonBacktracking))
            .WithMessage(ErrorMessages.InvalidPasswordCharacters); 
    }
}