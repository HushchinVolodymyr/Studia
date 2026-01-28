using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[AllowAnonymous]
[Route("[controller]")]
public class HomeController : ControllerBase
{
    [HttpGet("echo")]
    public IActionResult Echo()
    {
        return Ok("Echo response");
    }
}