using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OSCv2.Logic;
using OSCv2.Logic.Database;
using OSCv2.Logic.Hashing;
using Shared.Constants;
using Shared.Constants.RedisCommunication;
using Shared.DTOs;

namespace OSCv2.Controllers.v1.accounts;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    private readonly ILogger<LoginController> _logger;
    private readonly IWebsocketCommunicationService _websocketService;

    public LoginController(ILogger<LoginController> logger, IWebsocketCommunicationService websocketService)
    {
        _logger = logger;
        _websocketService = websocketService;
    }

    //ToDo: add Re-Captcha validation
    //ToDo: json serialize the return
    [HttpPost("{email}/{password}")]
    public async Task<IActionResult> Post(string email, string password)
    {
        var ctxFactory = new EntityFrameworkFactory();
        await using var ctx = ctxFactory.CreateDbContext();

        if (await ctx.Users.FirstOrDefaultAsync(x => x.Email == email) is not { } userAccount)
            return BadRequest(ErrorMessages.InvalidUserOrPass);

        if(!Pbkdf2.ValidatePassword(password, userAccount.HashedPassword))
            return BadRequest(ErrorMessages.InvalidUserOrPass);

        Guid sessionId = Guid.NewGuid();
        userAccount.SessionId = sessionId;
        
        await ctx.SaveChangesAsync();

        //ToDo: add WS check

        var connection = HttpContext.Connection;
        await _websocketService.SendAsync(RedisOpCodes.Login, sessionId, $"{connection.RemoteIpAddress}:{connection.RemotePort}");

        return Ok((AccountReturnDto)userAccount);
    }
}