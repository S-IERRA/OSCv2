using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OSCv2.Logic.Database;
using Shared.Constants;

namespace OSCv2.Controllers.v1.accounts;

[ApiController]
[Route("[controller]")]
public class LogoutController : ControllerBase
{
    private readonly ILogger<LogoutController> _logger;

    public LogoutController(ILogger<LogoutController> logger)
    {
        _logger = logger;
    }

    [HttpPost("{sessionId}")]
    public async Task<IActionResult> Post(Guid sessionId)
    {
        var ctxFactory = new EntityFrameworkFactory();
        await using var ctx = ctxFactory.CreateDbContext();

        if (await ctx.Users.FirstOrDefaultAsync(x => x.SessionId == sessionId) is not { } userAccount)
            return BadRequest(ErrorMessages.InvalidSession);

        userAccount.SessionId = null;
        await ctx.SaveChangesAsync();

        return Ok();
    }
}