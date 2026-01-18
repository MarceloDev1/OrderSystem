using Microsoft.AspNetCore.Mvc;

namespace OrderSystem.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // MVP simples (login fake)
        if (request.Email == "test@test.com" && request.Password == "123")
        {
            return Ok(new
            {
                token = "fake-jwt-token"
            });
        }

        return Unauthorized(new { message = "Credenciais inválidas" });
    }
}

public class LoginRequest
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}