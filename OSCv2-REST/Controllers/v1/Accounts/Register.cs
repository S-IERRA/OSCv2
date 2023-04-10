using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

using OSCv2.Logic.Database;
using OSCv2.Logic.Validators;
using OSCv2.Objects.Layouts;
using Shared.Constants;

namespace OSCv2.Controllers.v1.accounts;

[ApiController]
[Route("[controller]")]
public class RegisterController : ControllerBase
{
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
        var userAccount = new Account
        {
            Username = username,
            Email = email,
            Password = password
        };
        
        var ctxFactory = new EntityFrameworkFactory();
        await using var ctx = ctxFactory.CreateDbContext();

        var accountValidator = new CreateAccountValidator();
        ValidationResult result = await accountValidator.ValidateAsync(userAccount);
        if (!result.IsValid)
            return BadRequest(result.Errors);

        //Check if account already exists
        if (ctx.Users.Any(x => x.Email == email))
            return BadRequest(ErrorMessages.EmailAlreadyExists);

        ctx.Add(userAccount);
        await ctx.SaveChangesAsync();
        
        return Ok(userAccount);
    }
}
