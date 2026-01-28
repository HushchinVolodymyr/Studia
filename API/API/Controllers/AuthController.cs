using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[AllowAnonymous]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("register")]
    public IActionResult Register()
    {
        return Ok();
    }


}