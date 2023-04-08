using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OSCv2.Logic.Database;
using Shared.Constants;

namespace OSCv2.Controllers.v1.accounts;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    private readonly ILogger<LoginController> _logger;

    public LoginController(ILogger<LoginController> logger)
    {
        _logger = logger;
    }

    //ToDo: add Re-Captcha validation
    //ToDo: impl password hashing
    //ToDo: json serialize the return
    [HttpPost("{email}/{password}")]
    public async Task<IActionResult> Post(string email, string password)
    {
        var ctxFactory = new EntityFrameworkFactory();
        await using var ctx = ctxFactory.CreateDbContext();

        if (await ctx.Users.FirstOrDefaultAsync(x => x.Email == email && x.Password == password) is not { } userAccount)
            return BadRequest(ErrorMessages.InvalidUserOrPass);

        userAccount.SessionId = Guid.NewGuid();
        await ctx.SaveChangesAsync();

        return Ok(userAccount);
    }
}