using Microsoft.AspNetCore.Mvc;

namespace OSCv2.Controllers.v1.Servers;

//ToDo: finish this off
[ApiController]
[Route("[controller]")]
public class LogoutController : ControllerBase
{
    private readonly ILogger<LogoutController> _logger;

    public LogoutController(ILogger<LogoutController> logger)
    {
        _logger = logger;
    }

    [HttpPost("{name}")]
    public async Task<IActionResult> Post(string name)
    {
        return Ok();
    }
}