using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

using OSCv2.Logic.Database;
using OSCv2.Logic.Hashing;
using OSCv2.Logic.Validators;
using Shared.Constants;
using Shared.DTOs;
using Shared.DTOs.Verifications;
using Shared.Layouts;

namespace OSCv2.Controllers.v1.accounts;

[ApiController]
[Route("[controller]")]
public class RegisterController : ControllerBase
{
    private static readonly CreateAccountValidator AccountValidator = new CreateAccountValidator();

    private readonly ILogger<RegisterController> _logger;

    public RegisterController(ILogger<RegisterController> logger)
    {
        _logger = logger;
    }
    
    //ToDo: add Re-Captcha validation
    //ToDo: impl password hashing
    [HttpPost("{username}/{email}/{password}")]
    public async Task<IActionResult> Post(string username, string email, string password)
    {
        var userAccount = new RegisterDTo()
        {
            Username = username,
            Email = email,
            Password = password,
        };
        
        ValidationResult result = await AccountValidator.ValidateAsync(userAccount);
        if (!result.IsValid)
            return BadRequest(result.Errors);
        
        userAccount.HashedPassword = Pbkdf2.CreateHash(password);

        var ctxFactory = new EntityFrameworkFactory();
        await using var ctx = ctxFactory.CreateDbContext();

        if (ctx.Users.Any(x => x.Email == email))
            return BadRequest(ErrorMessages.EmailAlreadyExists);

        ctx.Users.Add((Account)userAccount);
        await ctx.SaveChangesAsync();
        
        return Ok((AccountReturnDto)userAccount);
    }
}
